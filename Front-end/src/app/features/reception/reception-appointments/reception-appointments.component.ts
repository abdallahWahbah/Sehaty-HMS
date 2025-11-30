import { Component } from '@angular/core';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { CommonModule } from '@angular/common';
import { AppointmentService } from '../../../core/services/appointment.service';
import { Router } from '@angular/router';
import { Ripple } from 'primeng/ripple';
import { ButtonModule } from 'primeng/button';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-reception-appointments',
  imports: [CommonModule, Toast, ButtonModule, Ripple, FormsModule],
  templateUrl: './reception-appointments.component.html',
  styleUrl: './reception-appointments.component.scss',
  providers: [MessageService],
})
export class ReceptionAppointmentsComponent {
  allAppointments: AppointmentResponseModel[] = [];
  appointments: AppointmentResponseModel[] = [];
  loading: boolean = false;
  serverError: string = '';
  showTodayOnly: boolean = false;

  constructor(
    private appointmentService: AppointmentService,
    private router: Router,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loading = true;
    this.appointmentService.getAll().subscribe({
      next: (data) => {
        data = data.filter(
          (item) =>
            item.status === 'Confirmed' ||
            item.status === 'InProgress' ||
            item.status === 'Completed'
        );
        this.allAppointments = data;
        this.appointments = data;

        this.loading = false;
      },
      error: (err) => {
        // this.serverError = err.error?.message;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.message || 'Error',
        });
        this.loading = false;
      },
    });
  }
  filterAppointments() {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (this.showTodayOnly) {
      this.appointments = this.allAppointments.filter((app) => {
        const date = new Date(app.appointmentDateTime);
        date.setHours(0, 0, 0, 0);
        return date.getTime() === today.getTime();
      });
    } else {
      this.appointments = this.allAppointments;
    }
  }
  cancelAppointment(id: number) {
    this.appointmentService.cancel(id).subscribe({
      next: (data) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Done',
          detail: 'Cancelled Successfully',
        });
        window.location.reload();
        // this.router.navigate(['/reception/appointments'])
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.message || 'Error',
        });
      },
    });
  }
  handleReschedule(appointment: AppointmentResponseModel) {
    this.router.navigate(
      ['/patient/appointments/available-days', appointment.doctorId],
      {
        state: {
          reschedule: true,
          appointmentId: appointment.id,
        },
      }
    );
  }
  checkIn(appointment: AppointmentResponseModel) {
    this.appointmentService.checkIn(appointment.id).subscribe({
      next: (data) => {
        window.location.reload();
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error.message,
        });
        // this.serverError = err.error.message;
      },
    });
  }
}
