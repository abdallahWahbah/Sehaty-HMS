import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MedicalRecordModel } from '../../../core/models/medical-record-model';
import { PatientResponseModel } from '../../../core/models/patient-response-model';
import { MedicalRecordService } from '../../../core/services/medical-record.service';
import { PatientsService } from '../../../core/services/patients.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-patient-medical-records',
  imports: [CommonModule],
  templateUrl: './patient-medical-records.component.html',
  styleUrl: './patient-medical-records.component.scss'
})
export class PatientMedicalRecordsComponent implements OnInit{

  medicalRecord!: MedicalRecordModel;
  patient!: PatientResponseModel;

  constructor(private _medicalRecordService: MedicalRecordService, private _patientService: PatientsService){
  }

  ngOnInit(){
    let storedUser: any = localStorage.getItem('userData');
    storedUser = JSON.parse(storedUser);

    this._patientService?.getAll().subscribe({
      next: data => {
        this.patient = data.filter(patient => patient.userId === storedUser.userId)[0];
      }
    });

    this._medicalRecordService.getAllForPatient().subscribe({
      next: (data: MedicalRecordModel) => {
        this.medicalRecord = data;
        console.log("aaaaaaaaa", this.medicalRecord);
      }
    })

  }

}