import { Component } from '@angular/core';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FeedbackService } from '../../../core/services/feedback.service';
import { FeedbackResponseModel } from '../../../core/models/feedback.response';
import { DoctorService } from '../../../core/services/doctor.service';

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
    private router: Router,
    private _doctorService: DoctorService,
  ) {}

  ngOnInit() {
    this.loadAppointments();

    // load doctor
    let storedUser: any = localStorage.getItem("userData");
    storedUser = JSON.parse(storedUser);
    this._doctorService.getAllDoctors().subscribe({
      next: allDoctors => {
        this.currentDoctor = allDoctors.filter(doc => doc.userId === storedUser.userId)[0];
      }
    })
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
      next: (feedback: any) => {
        if (Array.isArray(feedback) && feedback.length > 0) {
          this.selectedFeedbackMap[appointmentId] = feedback[0];
          return;
        }
        // If feedback is empty array → no feedback
        if (Array.isArray(feedback) && feedback.length === 0) {
          this.selectedFeedbackMap[appointmentId] = null;
          return;
        }
        // If backend gives object with index "0"
        if (feedback && typeof feedback === "object" && feedback["0"]) {
          this.selectedFeedbackMap[appointmentId] = feedback["0"];
          return;
        }
        // If backend gives empty object {}
        this.selectedFeedbackMap[appointmentId] = null;
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
