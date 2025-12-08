import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { FeedbackResponseModel } from '../../../core/models/feedback.response';
import { MedicalRecord } from '../../../core/models/medicalrecord-response.model';
import { PatientHistoryAnalysis } from '../../../core/models/PatientHistoryAnalysis.model';

import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorService } from '../../../core/services/doctor.service';
import { FeedbackService } from '../../../core/services/feedback.service';
import { MedicalRecordService } from '../../../core/services/medical-record.service';
import { PrescriptionAnalysisService } from '../../../core/services/prescription-analysis.service';

import { catchError, map, Observable, of } from 'rxjs';

@Component({
  selector: 'app-doctor-appointments',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './doctor-appointments.component.html',
  styleUrls: ['./doctor-appointments.component.scss'],
})
export class DoctorAppointmentsComponent {
  appointments: AppointmentResponseModel[] = [];
  isLoading: boolean = true;
  currentDoctor!: DoctorResponseModel;
  medicalRecord!: MedicalRecord;

  openedAppointmentId: number | null = null;

  selectedFeedbackMap: {
    [appointmentId: number]: FeedbackResponseModel | null;
  } = {};

  showApologyPopup = false;
  apologyMessage = '';
  isApologyError = false;

  // ====== AI History Modal state ======
  isHistoryAiModalOpen = false;
  historyLoading = false;
  historyError: string | null = null;
  historyData: PatientHistoryAnalysis | null = null;

  medicalRecordExistsMap: { [patientId: number]: boolean } = {};

  // ✅ مود الفلترة:
  // active => Confirmed + InProgress
  // completed => Completed فقط
  viewMode: 'active' | 'completed' = 'active';

  constructor(
    private _appointmentsService: AppointmentService,
    private feedbackService: FeedbackService,
    private router: Router,
    private _doctorService: DoctorService,
    private prescriptionAnalysisService: PrescriptionAnalysisService,
    private medicalRecordService: MedicalRecordService
  ) {}

  ngOnInit() {
    this.loadAppointments();

    // load doctor
    let storedUser: any = localStorage.getItem('userData');
    storedUser = JSON.parse(storedUser);
    this._doctorService.getAllDoctors().subscribe({
      next: (allDoctors) => {
        this.currentDoctor = allDoctors.filter(
          (doc) => doc.userId === storedUser.userId
        )[0];
      },
    });
  }

  private loadAppointments() {
    this.isLoading = true;

    this._appointmentsService.getDoctorAppointments().subscribe({
      next: (data: AppointmentResponseModel[]) => {
        // ترتيب المواعيد
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

        // check existence of medical record for each patient
        this.appointments.forEach((appt: any) => {
          this.checkMedicalRecordExistance(appt.patientId).subscribe(
            (exists) => {
              this.medicalRecordExistsMap[appt.patientId] = exists;
            }
          );
        });

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching doctor appointments', err);
        this.isLoading = false;
      },
    });
  }

  // ✅ Getter يرجّع الـ appointments المعروضة حسب الـ mode
  get filteredAppointments(): AppointmentResponseModel[] {
    return this.appointments.filter((a) => {
      const rawStatus = a.status || '';
      // نخليها lowercase ونشيل المسافات عشان نغطي in progress / InProgress
      const status = rawStatus.toLowerCase().replace(/\s+/g, '');

      if (this.viewMode === 'completed') {
        return status === 'completed';
      }

      // viewMode === 'active' -> Confirmed + InProgress
      return status === 'confirmed' || status === 'inprogress';
    });
  }

  // ✅ تغيير الـ mode من الزرارين
  setViewMode(mode: 'active' | 'completed') {
    this.viewMode = mode;
  }

  trackById(index: number, item: AppointmentResponseModel) {
    return item.id;
  }

  checkMedicalRecordExistance(patientId: any): Observable<boolean> {
    return this.medicalRecordService
      .getMedicalRecordByPatientId(patientId)
      .pipe(
        map((record) => !!record),
        catchError(() => of(false))
      );
  }

  addPrescription(appointment: AppointmentResponseModel) {
    this.router.navigate(['/doctor/prescriptions/add'], {
      state: {
        patientId: appointment.patientId,
        appointmentId: appointment.id,
      },
    });
  }

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

        if (Array.isArray(feedback) && feedback.length === 0) {
          this.selectedFeedbackMap[appointmentId] = null;
          return;
        }

        if (feedback && typeof feedback === 'object' && feedback['0']) {
          this.selectedFeedbackMap[appointmentId] = feedback['0'];
          return;
        }

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

  goToEditMedicalRecord(patientId: number) {
    this.medicalRecordService.getMedicalRecordByPatientId(patientId).subscribe({
      next: (data) => {
        this.router.navigate(['/doctor/patient/medicalRecord/edit', data.id], {
          state: {
            medicalRecord: data,
            patientId,
          },
        });
      },
    });
  }

  // =========================
  //   AI Patient History
  // =========================
  openHistoryAiModal(patientId?: number): void {
    if (!patientId) {
      console.warn('Patient ID is undefined for AI history');
      return;
    }

    this.isHistoryAiModalOpen = true;
    this.historyLoading = true;
    this.historyError = null;
    this.historyData = null;

    this.prescriptionAnalysisService
      .analyzePatientHistory(patientId)
      .subscribe({
        next: (res: any) => {
          if (res.isSuccess && res.data) {
            this.historyData = res.data;
          } else {
            this.historyError =
              res.error || 'Failed to load patient history analysis.';
          }
          this.historyLoading = false;
        },
        error: (err) => {
          console.error('Error calling AI history endpoint', err);
          this.historyError = 'Something went wrong while calling AI.';
          this.historyLoading = false;
        },
      });
  }

  closeHistoryAiModal(): void {
    this.isHistoryAiModalOpen = false;
  }

  apologize(appointment: AppointmentResponseModel) {
    if (!appointment.id) {
      console.warn('Appointment id is missing for apologize');
      return;
    }

    this._appointmentsService.apologizeForDoctor(appointment.id).subscribe({
      next: (res: any) => {
        this.isApologyError = false;
        this.apologyMessage =
          res?.message || 'Appointment cancelled and refund processed.';
        this.showApologyPopup = true;

        setTimeout(() => {
          this.showApologyPopup = false;
        }, 2500);

        this.loadAppointments();
      },
      error: (err) => {
        console.error('Error apologizing for appointment', err);
        this.isApologyError = true;
        this.apologyMessage = 'Something went wrong while cancelling.';
        this.showApologyPopup = true;

        setTimeout(() => {
          this.showApologyPopup = false;
        }, 2500);
      },
    });
  }

  isAppointmentPassed(date: Date | string): boolean {
    const now = new Date();
    const appDate = new Date(date);
    return appDate.getTime() < now.getTime();
  }
}
