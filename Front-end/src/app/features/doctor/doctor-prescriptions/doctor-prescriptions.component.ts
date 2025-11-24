import { Component, OnInit } from '@angular/core';
import { Prescription } from '../../../core/models/prescription-response-model';
import { PrescriptionService } from '../../../core/services/prescription.service';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-doctor-prescriptions',
  imports: [CommonModule, RouterModule],
  templateUrl: './doctor-prescriptions.component.html',
  styleUrls: ['./doctor-prescriptions.component.scss']
})
export class DoctorPrescriptionsComponent implements OnInit {

  prescriptions: Prescription[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(
    private prescriptionService: PrescriptionService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadPrescriptions();
  }

  // Get all prescriptions
  private loadPrescriptions(): void {
    this.prescriptionService.getAllPrescriptions().subscribe({
      next: (data) => {
        this.prescriptions = data.map(p => ({
          ...p,
          dateIssued: new Date(p.dateIssued)
        }));
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load prescriptions';
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  // Navigate to edit prescription page
  editPrescription(prescription: Prescription) {
    if (!prescription.id) {
      alert('Prescription ID is missing!');
      return;
    }
    // Navigate to the edit page
    this.router.navigate(['/doctor/prescriptions/edit', prescription.id]);
  }




  // Delete a prescription
  deletePrescription(prescription: Prescription) {
    if (confirm(`Are you sure you want to delete prescription ${prescription.id}?`)) {
      this.prescriptionService.deletePrescription(prescription.id).subscribe({
        next: () => {
          this.prescriptions = this.prescriptions.filter(p => p.id !== prescription.id);
          console.log('Prescription deleted');
        },
        error: (err) => console.error('Failed to delete prescription', err)
      });
    }
  }

}
