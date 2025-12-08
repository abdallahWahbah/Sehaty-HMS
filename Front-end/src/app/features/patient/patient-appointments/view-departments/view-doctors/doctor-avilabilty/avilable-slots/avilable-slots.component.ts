import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DoctorAvailabilityService } from '../../../../../../../core/services/doctoravilable.service';
import { AvailableDayModel } from '../../../../../../../core/models/available-day.model';
import { Slot } from '../../../../../../../core/models/available-slot.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientsService } from '../../../../../../../core/services/patients.service';
import { map } from 'rxjs/operators';
import { AppointmentService } from '../../../../../../../core/services/appointment.service';

// AuthService + UserRole
import {
  AuthService,
  UserRole,
} from '../../../../../../../core/services/auth.service';

@Component({
  selector: 'app-available-slots',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
  showPopup: boolean = false;
  popupMessage: string = '';
  isRescheduling: boolean = false;

  // لكل slot هنخزن السبب بالـ slotId
  reasonInputs: { [slotId: number]: string } = {};

  // Role
  currentRole: UserRole | null = null;

  get isPatient(): boolean {
    return this.currentRole === 'Patient';
  }

  get isReception(): boolean {
    return this.currentRole === 'Receptionist';
  }

  constructor(
    private route: ActivatedRoute,
    private doctorSlotsService: DoctorAvailabilityService,
    private patientService: PatientsService,
    private router: Router,
    private _appointmentService: AppointmentService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentRole = this.authService.getCurrentUserRole();
    console.log('🧪 [AvailableSlots] Current Role = ', this.currentRole);

    this.isRescheduling = history.state.reschedule || false;

    this.route.params.subscribe((params) => {
      this.doctorId = +params['doctorId'];
      this.selectedDate = params['date'];
      this.loadSlots(this.selectedDate);
    });
  }

  // Load available days
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

  // Load slots for selected date
  loadSlots(date: string): void {
    // Replace '/' with '-'
    date = date.replace(/\//g, '-');
    this.selectedDate = date;

    this.loading = true;
    this.doctorSlotsService.getAvailableSlots(this.doctorId, date).subscribe({
      next: (slotsData) => {
        this.slots = slotsData ?? [];
        // نفضي الـ reasons القديمة لما التاريخ يتغيّر
        this.reasonInputs = {};
        this.loading = false;
      },
      error: () => {
        this.serverError = 'Failed to load slots.';
        this.loading = false;
      },
    });
  }

  // Get logged-in patient ID from token
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

  // Book slot (Receptionist / Patient) + reason from textbox
  bookSlot(slotId: number, slotParam: any): void {
    this.serverError = '';

    const appointmentDateTime = `${slotParam.date}T${slotParam.startTime}.000Z`;

    // نقرأ السبب من الـ map حسب slotId
    const reasonRaw = this.reasonInputs[slotId];
    const reasonForVisit =
      (reasonRaw && reasonRaw.toString().trim()) || 'General checkup';

    // Receptionist logic
    if (this.isReception) {
      if (this.isRescheduling) {
        this._appointmentService
          .reschedule(history.state.appointmentId, {
            newAppointmentDateTime: appointmentDateTime,
          })
          .subscribe({
            next: () => {
              this.openPopup(
                `Appointment rescheduled successfully at ${new Date()}`
              );

              setTimeout(() => {
                this.router?.navigate(['reception/appointments']);
              }, 1000);
            },
            error: (err) => {
              this.serverError = err.error?.message || 'Failed to reschedule.';
            },
          });
      } else {
        this._appointmentService
          .bookAppointmentByReception(
            this.doctorId,
            appointmentDateTime,
            reasonForVisit
          )
          .subscribe({
            next: (data) => {
              this.openPopup(
                `Appointment booked successfully at ${data.startTime}`
              );
              setTimeout(() => {
                this.router?.navigate(['reception/appointments']);
              }, 1000);
            },
            error: (err) => {
              this.serverError = err.error?.message || 'Failed to book.';
            },
          });
      }

      return;
    }

    // Patient logic
    const patientId$ = this.getLoggedInPatientId();

    if (!patientId$) {
      console.error('Could not resolve logged-in patient ID');
      return;
    }

    patientId$.subscribe({
      next: (patientId) => {
        this.doctorSlotsService
          .bookSlot(slotId, patientId || 5, reasonForVisit)
          .subscribe({
            next: (res) => {
              this.openPopup(
                `Appointment booked successfully at ${res.startTime}`
              );
              setTimeout(() => {
                this.router?.navigate(['/home/appointments']);
              }, 1000);
              this.loadSlots(this.selectedDate);
            },
            error: (err) => {
              this.serverError =
                err.error?.message || 'Failed to book appointment.';
            },
          });
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
    }, 2500);
  }
}
