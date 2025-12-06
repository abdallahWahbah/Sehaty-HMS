import { Component } from '@angular/core';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { DoctorService } from '../../../core/services/doctor.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Dialog } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { DoctorComponent } from './doctor/doctor.component';
import { ConfirmationDialogComponent } from "../../../layout/confirmation-dialog/confirmation-dialog.component";

@Component({
  selector: 'app-admin-doctors',
  imports: [CommonModule, Dialog, ButtonModule, DoctorComponent, ConfirmationDialogComponent],
  templateUrl: './admin-doctors.component.html',
  styleUrl: './admin-doctors.component.scss',
})
export class AdminDoctorsComponent {
  doctors: DoctorResponseModel[] = [];
  selectedDoctor!: DoctorResponseModel;
  isDoctorShown: boolean = false;
  showDeleteDialog = false;

  constructor(private _doctorService: DoctorService, private router: Router) {}

  ngOnInit(): void {
    this._doctorService.getAllDoctors().subscribe({
      next: (data) => {
        this.doctors = data;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  showDoctor(doctor: DoctorResponseModel) {
    this.selectedDoctor = doctor;
    this.isDoctorShown = true;
  }

  navigateToEdit(id: number) {
    this.router.navigate([`/admin/doctors/${id}/edit`], { state: { id } });
  }
  navigateToAdd() {
    this.router.navigate(['/admin/doctors/add']);
  }
  openDeleteDialog(){
    this.showDeleteDialog = true;
  }
  closeDialog() {
    this.showDeleteDialog = false;
  }
  deleteDoctor(id: number){
    this._doctorService.deleteDoctor(id).subscribe({
      next: data => {
        window.location.reload();
      },
      error: err => {
        console.log("error deleting doctor");
      }
    }) 
  }
}
