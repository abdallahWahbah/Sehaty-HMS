import { Component, OnInit } from '@angular/core';
import { PrescriptionService } from '../../../core/services/prescription.service';
import { Prescription } from '../../../core/models/prescription-response-model';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-patient-prescription',
  imports: [DatePipe, CommonModule, FormsModule],
  templateUrl: './patient-prescription.component.html',
  styleUrl: './patient-prescription.component.scss',
})
export class PatientPrescriptionComponent implements OnInit {
  prescriptions: Prescription[] = [];

  constructor(private prescriptionService: PrescriptionService) {}

  ngOnInit() {
    this.loadPrescriptions();
  }

  loadPrescriptions() {
    this.prescriptionService.getPatientPrescriptions().subscribe({
      next: (rawData: any[]) => {
        // استقبل البيانات كـ any[] عشان تقدر تستخدم prescriptionId
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
          patiantName: p.patiantName || '', // تعيين القيمة الفارغة إذا مش موجودة
          mrn: p.mrn || '',
          doctorId: p.doctorId || 0,
          licenseNumber: p.licenseNumber || '',
        }));
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
}
