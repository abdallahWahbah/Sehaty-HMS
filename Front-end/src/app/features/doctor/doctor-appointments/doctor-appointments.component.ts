import { Component } from '@angular/core';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorService } from '../../../core/services/doctor.service';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FeedbackService } from '../../../core/services/feedback.service';
import { FeedbackResponseModel } from '../../../core/models/feedback.response';

@Component({
  selector: 'app-doctor-appointments',
  imports: [CommonModule],
  templateUrl: './doctor-appointments.component.html',
  styleUrl: './doctor-appointments.component.scss',
})
export class DoctorAppointmentsComponent {
  appointments: AppointmentResponseModel[] = [];
  currentDoctor!: DoctorResponseModel;
  isLoading: boolean = true;

  // لمتابعة الموعد المفتوح حالياً لعرض الفيدباك
  openedAppointmentId: number | null = null;

  // لتخزين الفيدباك لكل موعد تم فتحه
  selectedFeedbackMap: {
    [appointmentId: number]: FeedbackResponseModel | null;
  } = {};

  constructor(
    private _appointmentsService: AppointmentService,
    private _doctorService: DoctorService,
    private feedbackService: FeedbackService,
    private router: Router
  ) {}

  ngOnInit() {
    let storedUser: any = localStorage.getItem('userData');
    storedUser = JSON.parse(storedUser);

    // احضر بيانات الدكتور الحالي
    this._doctorService.getAllDoctors().subscribe({
      next: (allDoctors) => {
        this.currentDoctor = allDoctors.find(
          (doc) => doc.userId === storedUser.userId
        )!;
        this.loadAppointments();
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      },
    });
  }

  private loadAppointments() {
    this._appointmentsService.getAll().subscribe({
      next: (data) => {
        this.appointments = data
          .filter((app) => app.doctorId === this.currentDoctor?.id)
          .sort((a, b) => {
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
        console.error(err);
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
      // لو الفيدباك ظاهر بالفعل، نخفيه
      this.openedAppointmentId = null;
      return;
    }

    this.openedAppointmentId = appointmentId;

    // لو الفيدباك موجود مسبقاً، لا نعيد تحميله
    if (this.selectedFeedbackMap[appointmentId]) {
      return;
    }

    // تحميل الفيدباك من الخدمة
    this.feedbackService.getByAppointmentId(appointmentId).subscribe({
      next: (feedback: unknown) => {
        // لو الداتا جاية في شكل object يحتوي المفتاح "0"
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
}
