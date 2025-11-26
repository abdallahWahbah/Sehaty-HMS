import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DoctorAvailabilityService } from '../../../../../../core/services/doctoravilable.service';
import {
  AvailableDayModel,
  RecurringSchedule,
  SpecificDate,
} from '../../../../../../core/models/available-day.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-doctor-availability',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './doctor-avilabilty.component.html',
  styleUrls: ['./doctor-avilabilty.component.scss'],
})
export class DoctorAvailabilityComponent implements OnInit {
  doctorId!: number;
  doctorAvailability?: AvailableDayModel;
  loading = true;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private doctorAvailabilityService: DoctorAvailabilityService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // استلام doctorId من الـ route
    this.route.params.subscribe((params) => {
      this.doctorId = +params['doctorId'];
      this.loadAvailability();
    });
  }

  loadAvailability() {
    this.doctorAvailabilityService
      .getAvailableDaysForDoctor(this.doctorId)
      .subscribe({
        next: (data: AvailableDayModel) => {
          // لو مفيش أيام متاحة أو الـ array فاضية
          if (
            !data ||
            !data.recurringSchedule ||
            data.recurringSchedule.length === 0 ||
            data.recurringSchedule.every((d) => d.available === 'Not Available')
          ) {
            this.errorMessage = 'Doctor has no availability';
            this.loading = false;
            return;
          }

          this.doctorAvailability = data;
          this.loading = false;
        },
        error: () => {
          this.errorMessage = 'Doctor has no availability';
          this.loading = false;
        },
      });
  }

  viewSlots(date: string) {
    // استخدم المتغير date وليس selectedDate
    this.router.navigate([
      '/patient/appointments/available-slots',
      this.doctorId,
      date,
    ]);
  }
}
