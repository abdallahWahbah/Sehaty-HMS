import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DoctorAvailabilityService } from '../../../../../../../core/services/doctoravilable.service';
import { AvailableDayModel } from '../../../../../../../core/models/available-day.model';
import { Slot } from '../../../../../../../core/models/available-slot.model';
import { CommonModule } from '@angular/common';
import { PatientsService } from '../../../../../../../core/services/patients.service';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AppointmentService } from '../../../../../../../core/services/appointment.service';

@Component({
  selector: 'app-available-slots',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './avilable-slots.component.html',
  styleUrls: ['./avilable-slots.component.scss'],
  providers: [PatientsService],
})
export class AvailableSlotsComponent implements OnInit {
  doctorId!: number;
  selectedDate: string = '';
  availableDays: AvailableDayModel[] = [];
  slots: Slot[] = [];
  loading: boolean = true;
  serverError: string = '';
  showPopup = false;
  popupMessage = '';
  isRescheduling: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private doctorSlotsService: DoctorAvailabilityService,
    private patientService: PatientsService,
    private router: Router,
    private _appointmentService: AppointmentService
  ) {}

  ngOnInit(): void {
    this.isRescheduling = history.state.reschedule || false;
    this.route.params.subscribe((params) => {
      this.doctorId = +params['doctorId'];
      this.selectedDate = params['date']; // ← ← ← أهم سطر
      this.loadSlots(this.selectedDate);
    });
  }

  // ✅ Load doctor available days
  loadAvailableDays(): void {
    this.loading = true;
    this.doctorSlotsService.getAvailableDaysForDoctor(this.doctorId).subscribe({
      next: (data: AvailableDayModel) => {
        const hasRecurring = (data.recurringSchedule?.length ?? 0) > 0;
        const hasSpecific = (data.specificDates?.length ?? 0) > 0;

        if (hasRecurring || hasSpecific) {
          this.availableDays = [data];
          this.selectedDate =
            data.recurringSchedule?.[0]?.date ??
            data.specificDates?.[0]?.date ??
            '';

          if (this.selectedDate) {
            this.loadSlots(this.selectedDate);
          } else {
            this.serverError = 'No valid dates available.';
            this.loading = false;
          }
        } else {
          this.serverError = 'Doctor has no available days.';
          this.loading = false;
        }
      },
      error: () => {
        this.serverError = 'Failed to load available days.';
        this.loading = false;
      },
    });
  }

  // ✅ Load available slots for selected date
  loadSlots(date: string): void {
    // Replace '/' with '-'
    date = date.replace(/\//g, '-');
    this.selectedDate = date;

    this.loading = true;
    this.doctorSlotsService.getAvailableSlots(this.doctorId, date).subscribe({
      next: (slotsData) => {
        this.slots = slotsData ?? [];
        this.loading = false;
      },
      error: () => {
        this.serverError = 'Failed to load slots.';
        this.loading = false;
      },
    });
  }

  // ✅ Get logged-in patient's real DB ID using userId from token
  private getLoggedInPatientId() {
    const storedData = localStorage.getItem('userData');
    if (!storedData) return null;

    const userData = JSON.parse(storedData);
    const userId = userData?.userId;

    if (!userId) return null;

    return this.patientService.getAll().pipe(
      map((patients) => {
        const matched = patients.find((p) => p.userId === userId);
        return matched?.id ?? null;
      })
    );
  }

  // ✅ Book slot using the correct patient.id
  bookSlot(slotId: number, slotParam: any): void {
    this.serverError = '';
    const patientId$ = this.getLoggedInPatientId();

    if (!patientId$) {
      return;
    }

    patientId$.subscribe({
      next: (patientId) => {
        const appointmentDateTime = `${slotParam.date}T${slotParam.startTime}.000Z`;
        const reasonForVisit = 'Checkup';
        if(patientId === null){ // receptionist --> book 
          if(this.isRescheduling){ // receptionist --> reschedule
            this._appointmentService
            .reschedule(history.state.appointmentId, {newAppointmentDateTime: appointmentDateTime})
            .subscribe({
              next: data => {
                this.openPopup( `Appointment rescheduled successfully at ${new Date()}`);

                setTimeout(() => {
                  this.router?.navigate(['reception/appointments']);
                }, 1000);
              },
              error: err => {
                this.serverError = err.error?.message;
              }
            })
          }
          else { // receptionist --> book 
            this._appointmentService.bookAppointmentByReception(this.doctorId, appointmentDateTime, reasonForVisit)
            .subscribe({
              next: data => {
                this.openPopup( `Appointment booked successfully at ${data.startTime}`);
                setTimeout(() => {
                  this.router?.navigate(['reception/appointments']);
                }, 1000);
              },
              error: err => {
                this.serverError = err.error?.message;
              }
            })
          }
        }
        else { // patient -- > book 
          this.doctorSlotsService
            .bookSlot(slotId, patientId || 5, reasonForVisit) // "5" fixed patient for (elder) people not having account
            .subscribe({
              next: (res) => {
                this.openPopup( `Appointment booked successfully at ${res.startTime}`);
                setTimeout(() => {
                  this.router?.navigate(['/patient/appointments']);
                }, 1000);
                this.loadSlots(this.selectedDate);
              },
              error: (err) => {
                this.serverError = err.error.message
              },
          });
        }
      },
      error: () => {
        console.log('Failed to get patient data.');
      },
    });
  }
  openPopup(message: string) {
    this.popupMessage = message;
    this.showPopup = true;

    setTimeout(() => {
      this.showPopup = false;
    }, 2500); // يختفي بعد 2.5 ثانية تلقائياً
  }
}
