import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { RescheduleAppointmentDto } from '../../../../core/models/appointment-updateDate-model';
import { AppointmentResponseModel } from '../../../../core/models/appointment-response-model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-update-schedule',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-update-schedule.component.html',
  styleUrls: ['./admin-update-schedule.component.scss'],
})
export class AdminUpdateScheduleComponent implements OnInit {
  appointment!: AppointmentResponseModel;
  newAppointmentDateTime: string = '';
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private appointmentService: AppointmentService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.router.navigate(['/admin/appointments']);
      return;
    }

    this.appointmentService.getById(id).subscribe({
      next: (data) => {
        this.appointment = data;
        this.newAppointmentDateTime = this.toDatetimeLocal(
          new Date(data.appointmentDateTime)
        );
      },
      error: () => this.router.navigate(['/admin/appointments']),
    });
  }

  saveChanges(): void {
    if (!this.appointment || !this.newAppointmentDateTime) return;

    const newDate = new Date(this.newAppointmentDateTime);
    const now = new Date();

    // تحقق من 24 ساعة لو مش Emergency
    if (this.appointment.status !== 'Emergency') {
      const diffInHours =
        (newDate.getTime() - now.getTime()) / (1000 * 60 * 60);
      if (diffInHours < 24) {
        this.errorMessage =
          'Cannot update non-emergency appointments to less than 24 hours from now';
        return;
      }
    }

    const dto: RescheduleAppointmentDto = { newAppointmentDateTime: newDate };

    this.appointmentService.reschedule(this.appointment.id, dto).subscribe({
      next: () => {
        this.router.navigate(['/admin/appointments']);
      },
      error: (err) => {
        console.error(err);
        this.errorMessage =
          err?.error?.message || 'Failed to update appointment date.';
      },
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/appointments']);
  }

  // تحويل تاريخ JS الى صيغة input type="datetime-local"
  private toDatetimeLocal(date: Date): string {
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(
      date.getDate()
    )}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }
}
