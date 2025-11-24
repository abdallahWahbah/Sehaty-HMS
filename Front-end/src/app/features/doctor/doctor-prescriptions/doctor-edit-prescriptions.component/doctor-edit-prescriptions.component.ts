import { Component, OnInit } from '@angular/core';
import { Prescription } from '../../../../core/models/prescription-response-model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PrescriptionService } from '../../../../core/services/prescription.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-doctor-edit-prescriptions',
  imports: [CommonModule, FormsModule,RouterModule],
  templateUrl: './doctor-edit-prescriptions.component.html',
  styleUrl: './doctor-edit-prescriptions.component.scss'
})
export class DoctorEditPrescriptionsComponent implements OnInit {
  prescription!: Prescription;
  isLoading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private prescriptionService: PrescriptionService
  ) { }

  ngOnInit(): void {
    const prescriptionId = Number(this.route.snapshot.paramMap.get('id'));

    if (!prescriptionId) {
      this.errorMessage = 'Invalid prescription ID';
      return;
    }

    this.loadPrescription(prescriptionId);
  }
  removeMedication(index: number) {
    if (this.prescription.medications && this.prescription.medications.length > 1) {
      this.prescription.medications.splice(index, 1);
    }
  }
  addMedication() {
    this.prescription.medications?.push({ medicationName: '', dosage: '', frequency: '', duration: '' });
  }
  private loadPrescription(id: number) {
    this.prescriptionService.getPrescriptionById(id).subscribe({
      next: (found) => {
        if (!found) {
          this.errorMessage = 'Prescription not found';
          this.isLoading = false;
          return;
        }

        this.prescription = {
          ...found,
          dateIssued: new Date(found.dateIssued)
        };
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load prescription';
        this.isLoading = false;
      }
    });
  }


  saveChanges() {
    this.errorMessage = '';
    this.prescriptionService.editPrescription(this.prescription.id, this.prescription).subscribe({
      next: () => {
        alert('Prescription updated successfully');
        this.router.navigate(['/doctor/prescription']);
      },
      error: (err) => {
        this.errorMessage = err.error.errors[0];
        console.error('Update failed', err)
      }
    });
  }

  cancel() {
    this.router.navigate(['/doctor/prescription']);
  }
}
