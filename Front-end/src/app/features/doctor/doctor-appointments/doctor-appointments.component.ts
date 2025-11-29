import { Component } from '@angular/core';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FeedbackService } from '../../../core/services/feedback.service';
import { FeedbackResponseModel } from '../../../core/models/feedback.response';

@Component({
  selector: 'app-doctor-appointments',
  imports: [CommonModule],
  templateUrl: './doctor-appointments.component.html',
  styleUrls: ['./doctor-appointments.component.scss'],
})
export class DoctorAppointmentsComponent {
  appointments: AppointmentResponseModel[] = [];
  isLoading: boolean = true;
  currentDoctor!: DoctorResponseModel; // <-- لازم يكون موجود

  // لمتابعة الموعد المفتوح حالياً لعرض الفيدباك
  openedAppointmentId: number | null = null;

  // لتخزين الفيدباك لكل موعد تم فتحه
  selectedFeedbackMap: {
    [appointmentId: number]: FeedbackResponseModel | null;
  } = {};

  constructor(
    private _appointmentsService: AppointmentService,
    private feedbackService: FeedbackService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadAppointments();
  }

  private loadAppointments() {
    this.isLoading = true;

    this._appointmentsService.getDoctorAppointments().subscribe({
      next: (data: AppointmentResponseModel[]) => {
        this.appointments = data.sort((a, b) => {
          const dateA = new Date(a.appointmentDateTime);
          const dateB = new Date(b.appointmentDateTime);

          const dayA = new Date(
            dateA.getFullYear(),
            dateA.getMonth(),
            dateA.getDate()
          ).getTime();
          const dayB = new Date(
            dateB.getFullYear(),
            dateB.getMonth(),
            dateB.getDate()
          ).getTime();

          if (dayA !== dayB) return dayB - dayA;
          return dateA.getTime() - dateB.getTime();
        });

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching doctor appointments', err);
        this.isLoading = false;
      },
    });
  }

  // TrackBy function لتحسين أداء ngFor
  trackById(index: number, item: AppointmentResponseModel) {
    return item.id;
  }

  // زر Add Prescription
  addPrescription(appointment: AppointmentResponseModel) {
    this.router.navigate(['/doctor/prescriptions/add'], {
      state: {
        patientId: appointment.patientId,
        appointmentId: appointment.id,
      },
    });
  }

  // عرض أو إخفاء الفيدباك للموعد المحدد
  toggleFeedback(appointmentId: number) {
    if (this.openedAppointmentId === appointmentId) {
      this.openedAppointmentId = null;
      return;
    }

    this.openedAppointmentId = appointmentId;

    if (this.selectedFeedbackMap[appointmentId]) return;

    this.feedbackService.getByAppointmentId(appointmentId).subscribe({
      next: (feedback: unknown) => {
        if (feedback && typeof feedback === 'object' && '0' in feedback) {
          this.selectedFeedbackMap[appointmentId] = (feedback as any)[
            '0'
          ] as FeedbackResponseModel;
        } else {
          this.selectedFeedbackMap[appointmentId] =
            feedback as FeedbackResponseModel | null;
        }
      },
      error: (err) => {
        console.error('Error loading feedback', err);
        this.selectedFeedbackMap[appointmentId] = null;
      },
    });
  }

  goToPatientDetails(patientId?: number) {
    if (patientId != null) {
      this.router.navigate(['/doctor/patient/details', patientId]);
    } else {
      console.warn('Patient ID is undefined!');
    }
  }
}
