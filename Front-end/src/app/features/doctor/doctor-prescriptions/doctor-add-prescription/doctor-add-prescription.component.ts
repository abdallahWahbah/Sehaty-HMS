import { Component, OnInit } from '@angular/core';
import { Prescription } from '../../../../core/models/prescription-response-model';
import { Medication } from '../../../../core/models/medication-response-model';
import { PrescriptionService } from '../../../../core/services/prescription.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Location } from '@angular/common';
import { stat } from 'fs';


@Component({
  selector: 'app-doctor-add-prescription',
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor-add-prescription.component.html',
  styleUrl: './doctor-add-prescription.component.scss'
})
export class DoctorAddPrescriptionComponent {

  isSubmitting = false;
  errorMessage = '';
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
    console.log(state);
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
    // تحقق من الحقول المطلوبة
    if (
      !this.prescription.status ||
      !this.prescription.medications?.length ||
      !this.prescription.digitalSignature ||
      !this.prescription.specialInstructions

    ) {
      this.errorMessage = 'Please fill all required fields';
      return;
    }


    this.isSubmitting = true;
    // this.prescriptionService.addPrescription(this.prescription).subscribe({
    //   next: () => {
    //     alert('Prescription added successfully');
    //     this.router.navigate(['/doctor/prescription']);
    //   },
    //   error: (err) => {
    //     console.error(err);
    //     this.errorMessage = 'Failed to add prescription';
    //     this.isSubmitting = false;
    //   }
    // });
    console.log(this.prescription);
    console.log(this.appointmentId,this.patientId);


  }

  cancel() {
    this.router.navigate(['/doctor/prescription']);
  }
}
