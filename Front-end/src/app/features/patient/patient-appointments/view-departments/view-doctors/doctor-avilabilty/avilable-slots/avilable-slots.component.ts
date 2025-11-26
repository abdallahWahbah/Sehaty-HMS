import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DoctorAvailabilityService } from '../../../../../../../core/services/doctoravilable.service';
import { AvailableDayModel } from '../../../../../../../core/models/available-day.model';
import { Slot } from '../../../../../../../core/models/available-slot.model';
import { CommonModule } from '@angular/common';
import { PatientsService } from '../../../../../../../core/services/patients.service';
import { map } from 'rxjs/operators';

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
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private doctorSlotsService: DoctorAvailabilityService,
    private patientService: PatientsService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.doctorId = +params['doctorId'];
      this.loadAvailableDays();
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
            this.errorMessage = 'No valid dates available.';
            this.loading = false;
          }
        } else {
          this.errorMessage = 'Doctor has no available days.';
          this.loading = false;
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load available days.';
        this.loading = false;
      },
    });
  }

  // ✅ Load available slots for selected date
  loadSlots(date: string): void {
    this.loading = true;
    this.selectedDate = date;

    this.doctorSlotsService.getAvailableSlots(this.doctorId, date).subscribe({
      next: (slotsData: Slot[] | undefined) => {
        this.slots = slotsData ?? [];
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load slots.';
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
  bookSlot(slotId: number): void {
    const patientId$ = this.getLoggedInPatientId();

    if (!patientId$) {
      alert('User not logged in');
      return;
    }

    patientId$.subscribe({
      next: (patientId) => {
        if (!patientId) {
          alert('Patient record not found.');
          return;
        }

        const reasonForVisit = 'Checkup';

        this.doctorSlotsService
          .bookSlot(slotId, patientId, reasonForVisit)
          .subscribe({
            next: (res) => {
              alert(`Appointment booked successfully at ${res.startTime}`);
              this.loadSlots(this.selectedDate);
            },
            error: () => {
              alert('Failed to book slot.');
            },
          });
      },
      error: () => {
        alert('Failed to get patient data.');
      },
    });
  }
}
