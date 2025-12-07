import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppointmentService } from '../../../core/services/appointment.service';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { CommonModule } from '@angular/common';
import { PatientsService } from '../../../core/services/patients.service';
import { FeedbackResponseModel } from '../../../core/models/feedback.response';
import { FeedbackService } from '../../../core/services/feedback.service';

@Component({
  selector: 'app-patient-feedback',
  imports: [CommonModule],
  templateUrl: './patient-feedback.component.html',
  styleUrls: ['./patient-feedback.component.scss'], // صححتها من styleUrl إلى styleUrls
})
export class PatientFeedbackComponent implements OnInit {
  appointments: AppointmentResponseModel[] = [];
  isLoading = false;

  // تخزين الفيدباك لكل appointment
  feedbackMap: {
    [appointmentId: number]: FeedbackResponseModel | null | undefined;
  } = {};

  // حالة لود
  feedbackLoading: { [appointmentId: number]: boolean } = {};

  constructor(
    private appointmentService: AppointmentService,
    private patientService: PatientsService,
    private feedbackService: FeedbackService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAppointmentsForCurrentPatient();
  }

  //-----------------------------------------------------
  // 1) تحميل المواعيد الخاصة بالمريض الحالي
  //-----------------------------------------------------
  loadAppointmentsForCurrentPatient() {
    const storedUserString = localStorage.getItem('userData');
    if (!storedUserString) {
      console.error('No userdata in localStorage');
      return;
    }

    const storedUser = JSON.parse(storedUserString);
    const userId = storedUser.userId;

    this.patientService.getAll().subscribe({
      next: (patients) => {
        const patient = patients.find((p) => p.userId === userId);
        if (!patient) {
          console.error('Patient not found for userId:', userId);
          return;
        }

        const patientId = patient.id;

        this.appointmentService.getAll().subscribe({
          next: (appointments) => {
            this.appointments = appointments.filter(
              (a) => a.patientId === patientId && a.status === 'Completed' //filter only completed appointments
            );

            console.log('Filtered appointments:', this.appointments);
          },
          error: (err) => console.error('Error loading appointments', err),
        });
      },
      error: (err) => console.error('Error loading patients', err),
    });
  }

  //-----------------------------------------------------
  // 2) Show Feedback / Hide Feedback
  //-----------------------------------------------------
  toggleShowFeedback(appointmentId: number) {
    // لو الفيدباك ظاهر — نخفيه
    if (this.feedbackMap[appointmentId] !== undefined) {
      delete this.feedbackMap[appointmentId];
      return;
    }

    // else: نجيب الفيدباك من السيرفر
    this.feedbackLoading[appointmentId] = true;

    this.feedbackService.getByAppointmentId(appointmentId).subscribe({
      next: (fb) => {
        this.feedbackMap[appointmentId] = fb || null;
        this.feedbackLoading[appointmentId] = false;
      },
      error: (err) => {
        console.error('Error loading feedback', err);
        this.feedbackMap[appointmentId] = null; // معناها "لا يوجد"
        this.feedbackLoading[appointmentId] = false;
      },
    });
  }

  //-----------------------------------------------------
  // 3) Add feedback -> صفحة الإضافة
  //-----------------------------------------------------
  addFeedback(appointmentId: number) {
    this.router.navigate(['/home/feedback/add', appointmentId]);
  }

  //-----------------------------------------------------
  // 4) TrackBy
  //-----------------------------------------------------
  trackById(index: number, item: AppointmentResponseModel): number {
    return item.id;
  }

  //-----------------------------------------------------
  // 5) stars array helper
  //-----------------------------------------------------
  getStarsArray(rating: number) {
    return new Array(5).fill(0).map((_, i) => i < rating);
  }

  //-----------------------------------------------------
  // 6) Get feedback from map (needed for template)
  //-----------------------------------------------------
  getFeedbackForAppointment(
    appointmentId: number
  ): FeedbackResponseModel | null | undefined {
    return this.feedbackMap[appointmentId];
  }
}
