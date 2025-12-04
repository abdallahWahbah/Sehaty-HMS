import { Component, OnInit } from '@angular/core';
import { Prescription } from '../../../../core/models/prescription-response-model';
import { Medication } from '../../../../core/models/medication-response-model';
import { PrescriptionService } from '../../../../core/services/prescription.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Location } from '@angular/common';
import { stat } from 'fs';
import { LoadingSpinnerComponent } from "../../../../layout/loading-spinner/loading-spinner.component";


@Component({
  selector: 'app-doctor-add-prescription',
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  templateUrl: './doctor-add-prescription.component.html',
  styleUrl: './doctor-add-prescription.component.scss'
})
export class DoctorAddPrescriptionComponent {

  isSubmitting = false;
  serverError = '';
  patientId: number = 0;
  appointmentId :number =0;

  prescription: Partial<Prescription> = {
    status: 'Active',
    patientId: 0,       // سيتحدد لاحقًا حسب المريض
    appointmentId: 0,   // سيتحدد لاحقًا حسب الموعد
    specialInstructions: '',
    digitalSignature: '',
    medications: [{ medicationName: '', dosage: '', frequency: '', duration: '' }]
  };


  constructor(private prescriptionService: PrescriptionService, private router: Router, private location: Location) { }

  ngOnInit() {
    const state: any = this.location?.getState();
    this.patientId = state.patientId ;
    this.appointmentId = state.appointmentId ;
  }
  addMedication() {
    this.prescription.medications?.push({ medicationName: '', dosage: '', frequency: '', duration: '' });
  }

  removeMedication(index: number) {
    if (this.prescription.medications && this.prescription.medications.length > 1) {
      this.prescription.medications.splice(index, 1);
    }
  }

  savePrescription() {
    this.serverError = '';
    if (
      !this.prescription.status ||
      !this.prescription.medications?.length ||
      !this.prescription.digitalSignature ||
      !this.prescription.specialInstructions
    ) {
      this.serverError = 'Please fill all required fields';
      return;
    }
    this.isSubmitting = true;
    this.prescriptionService.addPrescription(this.prescription, this.patientId, this.appointmentId).subscribe({
      next: () => {
        this.router.navigate(['/doctor/prescription']);
      },
      error: (err) => {
        console.error(err);
        // this.serverError = err.error.errors[0];
        // this
        this.serverError = err.error.message;
        this.isSubmitting = false;
      }
    });
  }

  cancel() {
    this.router.navigate(['/doctor/prescription']);
  }
}
