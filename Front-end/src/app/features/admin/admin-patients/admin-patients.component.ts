import { Component } from '@angular/core';
import { PatientsService } from '../../../core/services/patients.service';
import { PatientResponseModel } from '../../../core/models/patient-response-model';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { PatientComponent } from "./patient/patient.component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-patients',
  imports: [CommonModule, Dialog, ButtonModule, PatientComponent],
  templateUrl: './admin-patients.component.html',
  styleUrl: './admin-patients.component.scss'
})
export class AdminPatientsComponent {

  patients: PatientResponseModel[] = [];
  selectedPatient!: PatientResponseModel;
  isPatientShown: boolean = false;

  constructor(private _patientService: PatientsService, private router: Router){}

  ngOnInit(): void {

    this._patientService.getAll().subscribe({
      next: data => {
        this.patients = data;
      },
      error: err => {
        console.log(err);
      }
    });

  }
  showPatient(patient: PatientResponseModel){
    this.isPatientShown = true;
    this.selectedPatient = patient;
  }
  navigateToEdit(id: number){
    this.router.navigate([`/admin/patients/${id}/edit`], {state: {id}})
  }
}
