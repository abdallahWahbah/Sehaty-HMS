import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MedicalRecordService } from '../../../core/services/medical-record.service';
import { PrescriptionService } from '../../../core/services/prescription.service';
import { MedicalRecord } from '../../../core/models/medicalrecord-response.model';
import { PrescriptionHistory } from '../../../core/models/prescriptionhistory-models';

@Component({
  selector: 'app-doctor-medicalrecordandprescription',
  imports: [CommonModule],
  templateUrl: './doctor-medicalrecordandprescription.component.html',
  styleUrls: ['./doctor-medicalrecordandprescription.component.scss'],
})
export class DoctorMedicalrecordandprescriptionComponent implements OnInit {
  patientId!: number;
  medicalRecord!: MedicalRecord;
  prescriptions: PrescriptionHistory[] = [];
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private medicalRecordService: MedicalRecordService,
    private prescriptionService: PrescriptionService
  ) {}

  ngOnInit(): void {
    this.patientId = Number(this.route.snapshot.paramMap.get('id'));

    // جلب Medical Record
    this.medicalRecordService
      .getMedicalRecordByPatientId(this.patientId)
      .subscribe({
        next: (record) => (this.medicalRecord = record),
        error: (err) => console.error('Error fetching medical record', err),
      });

    // جلب Prescription History
    this.prescriptionService
      .getPatientPrescriptionHistory(this.patientId)
      .subscribe({
        next: data => {
          this.prescriptions = data;
          console.log("dddddddddd", this.prescriptions);
        },
        error: (err) => console.error('Error fetching prescriptions', err),
    });

    this.loading = false;
  }
}
