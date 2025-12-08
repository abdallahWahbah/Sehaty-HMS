import { Component, OnInit } from '@angular/core';
import { PrescriptionService } from '../../../core/services/prescription.service';
import { Prescription } from '../../../core/models/prescription-response-model';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { saveAs } from 'file-saver';
import { PrescriptionAnalysisService } from '../../../core/services/prescription-analysis.service';
import { PrescriptionAnalysis } from '../../../core/models/prescription-analysis.model';
import { PrescriptionAnalysisAlternative } from '../../../core/models/prescriptionAnalysisalternative-model';
import { LoadingSpinnerComponent } from "../../../layout/loading-spinner/loading-spinner.component";

@Component({
  selector: 'app-patient-prescription',
  imports: [DatePipe, CommonModule, FormsModule, LoadingSpinnerComponent],
  templateUrl: './patient-prescription.component.html',
  styleUrl: './patient-prescription.component.scss',
})
export class PatientPrescriptionComponent implements OnInit {
  prescriptions: Prescription[] = [];

  // AI Analysis state
  isAnalysisModalOpen = false;
  analysisLoading = false;
  analysisError: string | null = null;
  analysisData: PrescriptionAnalysis | null = null;
  analysisMode: 'analysis' | 'alternative' = 'analysis';
  alternativeData: PrescriptionAnalysisAlternative | null = null;
  currentPrescriptionDate: Date | null = null;
  loading: boolean = true;

  constructor(
    private prescriptionService: PrescriptionService,
    private analysisService: PrescriptionAnalysisService
  ) {}

  ngOnInit() {
    this.loadPrescriptions();
  }

  loadPrescriptions() {
    this.prescriptionService.getPatientPrescriptions().subscribe({
      next: (rawData: any[]) => {
        this.prescriptions = rawData.map((p) => ({
          id: p.prescriptionId,
          doctorName: p.doctorName,
          dateIssued: new Date(p.dateIssued),
          status: p.status,
          medicalRecordNotes: p.doctorNotes,
          medications: p.medications ?? [],
          digitalSignature: p.digitalSignature || '',
          specialInstructions: p.specialInstructions || '',
          appointmentId: p.appointmentId || 0,
          medicalRecordId: p.medicalRecordId || 0,
          patientId: p.patientId || 0,
          patiantName: p.patiantName || '',
          mrn: p.mrn || '',
          doctorId: p.doctorId || 0,
          licenseNumber: p.licenseNumber || '',
        }));
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading prescriptions', err);
      },
    });
  }

  downloadPrescription(id: number) {
    if (!id) {
      console.error('Invalid prescription id:', id);
      return;
    }

    this.prescriptionService.downloadPrescription(id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `prescription_${id}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Download error:', err);
      },
    });
  }
  // ====== AI Analysis / Alternatives Handlers ======
  openAnalysisModal(
    prescriptionId: number,
    mode: 'analysis' | 'alternative' = 'analysis'
  ) {
    if (!prescriptionId) return;

    this.isAnalysisModalOpen = true;
    this.analysisLoading = true;
    this.analysisError = null;

    this.analysisData = null;
    this.alternativeData = null;
    this.analysisMode = mode;

    if (mode === 'analysis') {
      // 🔍 تحليل الروشتة
      this.analysisService.analyzePrescription(prescriptionId).subscribe({
        next: (res) => {
          console.log('AI analysis response:', res);
          this.analysisData = res;
          this.analysisLoading = false;
        },
        error: (err) => {
          console.error('AI analysis error:', err);
          this.analysisError = 'حدث خطأ أثناء جلب تحليل الروشتة بالـ AI.';
          this.analysisLoading = false;
        },
      });
    } else {
      // 🆕 بدائل الأدوية
      this.analysisService.findalternative(prescriptionId).subscribe({
        next: (res) => {
          console.log('AI alternatives response:', res);
          this.alternativeData = res;
          this.analysisLoading = false;
        },
        error: (err) => {
          console.error('AI alternatives error:', err);
          this.analysisError =
            'حدث خطأ أثناء جلب بدائل الأدوية وتحليلها بالـ AI.';
          this.analysisLoading = false;
        },
      });
    }
  }

  closeAnalysisModal() {
    this.isAnalysisModalOpen = false;
  }
}
