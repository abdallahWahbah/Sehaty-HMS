import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MedicalRecordModel } from '../../../core/models/medical-record-model';
import { PatientResponseModel } from '../../../core/models/patient-response-model';
import { MedicalRecordService } from '../../../core/services/medical-record.service';
import { PatientsService } from '../../../core/services/patients.service';
import { CommonModule } from '@angular/common';
import { LoadingSpinnerComponent } from "../../../layout/loading-spinner/loading-spinner.component";

@Component({
  selector: 'app-patient-medical-records',
  imports: [CommonModule, LoadingSpinnerComponent],
  templateUrl: './patient-medical-records.component.html',
  styleUrl: './patient-medical-records.component.scss'
})
export class PatientMedicalRecordsComponent implements OnInit{

  medicalRecord!: MedicalRecordModel;
  patient!: PatientResponseModel;
  loading: boolean = true;

  constructor(private _medicalRecordService: MedicalRecordService, private _patientService: PatientsService){
  }

  ngOnInit(){
    let storedUser: any = localStorage.getItem('userData');
    storedUser = JSON.parse(storedUser);

    this._patientService?.getAll().subscribe({
      next: data => {
        this.patient = data.filter(patient => patient.userId === storedUser.userId)[0];
        this.loading = false;
      }
    });

    this._medicalRecordService.getForPatient().subscribe({
      next: (data: MedicalRecordModel) => {
        this.medicalRecord = data;
        this.loading = false;
      }
    })
  }
}