import { Component, OnInit } from '@angular/core';
import { Prescription } from '../../../core/models/prescription-response-model';
import { PrescriptionService } from '../../../core/services/prescription.service';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-doctor-prescriptions',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './doctor-prescriptions.component.html',
  styleUrls: ['./doctor-prescriptions.component.scss'],
})
export class DoctorPrescriptionsComponent implements OnInit {
  prescriptions: Prescription[] = [];
  isLoading = true;
  errorMessage = '';
  filteredPrescriptions: Prescription[] = [];
  searchPatientName: string = "";

  constructor(
    private prescriptionService: PrescriptionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPrescriptions();
  }

  // Get all prescriptions
  private loadPrescriptions(): void {
    this.prescriptionService.getDoctorPrecriptions().subscribe({
      next: (data) => {
        this.prescriptions = data;
        this.filteredPrescriptions = [...this.prescriptions]; // initialize filtered list
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load prescriptions';
        console.error(err);
        this.isLoading = false;
      },
    });
  }

  filterByPatientName() {
    if (!this.searchPatientName) {
      this.filteredPrescriptions = [...this.prescriptions];
      return;
    }

    // this.filteredPrescriptions = this.prescriptions.filter(
    //   (p) => p.patiantName === this.searchPatientName
    // );
    this.filteredPrescriptions = this.prescriptions.filter(p => p.patiantName.toLowerCase().includes(this.searchPatientName.toLowerCase()));
  }

  // Navigate to edit prescription page
  editPrescription(prescription: Prescription) {
    if (!prescription.id) {
      return;
    }
    // Navigate to the edit page
    this.router.navigate(['/doctor/prescriptions/edit', prescription.id]);
  }

  // Delete a prescription
  deletePrescription(prescription: Prescription) {
    if (
      confirm(
        `Are you sure you want to delete prescription ${prescription.id}?`
      )
    ) {
      this.prescriptionService.deletePrescription(prescription.id).subscribe({
        next: () => {
          window.location.reload();
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Failed to delete prescription', err)
        }
      });
    }
  }
}
