import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-admin-appointments',
  imports: [CommonModule],
  templateUrl: './admin-appointments.component.html',
  styleUrl: './admin-appointments.component.scss',
})
export class AdminAppointmentsComponent implements OnInit {
  appointments: AppointmentResponseModel[] = [];
  loading: boolean = false;

  error: string = '';
  constructor(
    private appointmentService: AppointmentService,
    private router: Router
  ) {}
  ngOnInit(): void {
    this.loadAppointments();
  }
  loadAppointments(): void {
    this.loading = true;
    this.appointmentService.getAll().subscribe({
      next: (data) => {
        this.appointments = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load appointments', err);
        this.error = 'Failed to load appointments.';
        this.loading = false;
      },
    });
  }
  trackById(index: number, item: AppointmentResponseModel): number {
    return item.id;
  }

  viewDetails(appointment: AppointmentResponseModel): void {
    this.router.navigate(['/admin/appointments', appointment.id]);
  }
  navigateToMedicalRecords(appointment: AppointmentResponseModel){
    this.router.navigate(['/doctor/patient/details', appointment.patientId]);
  }

}
