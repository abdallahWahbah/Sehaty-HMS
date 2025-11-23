import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { AppointmentResponseModel } from '../../../../core/models/appointment-response-model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-appointment-details',
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-appointment-details.component.html',
  styleUrls: ['./admin-appointment-details.component.scss'],
})
export class AdminAppointmentDetailsComponent implements OnInit {
  appointment!: AppointmentResponseModel;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private appointmentService: AppointmentService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (id) {
        this.appointmentService.getById(id).subscribe({
          next: (data) => (this.appointment = data),
          error: (err) => this.router.navigate(['/admin/appointments']),
        });
      }
    });
  }

  getStatusClass(): string {
    if (!this.appointment) return '';
    switch (this.appointment.status) {
      case 'Confirmed':
        return 'status-confirmed';
      case 'Pending':
        return 'status-pending';
      case 'Delayed':
        return 'status-delayed';
      case 'Cancelled':
        return 'status-cancelled';
      case 'NoShow':
        return 'status-noshow';
      default:
        return '';
    }
  }

  editAppointment(): void {
    if (!this.appointment) return;
    this.router.navigate(['/admin/appointments/edit', this.appointment.id]);
  }

  goBack(): void {
    this.router.navigate(['/admin/appointments']);
  }
}
