import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { PatientsService } from '../../../../core/services/patients.service';
import { PatientResponseModel } from '../../../../core/models/patient-response-model';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { Router, RouterModule } from '@angular/router';
import { PateintStatusEnum } from '../../../../core/enums/patient-status-enum';

@Component({
  selector: 'app-patient-edit',
  imports: [
    FloatLabelModule, 
    InputTextModule, 
    FormsModule, 
    ButtonModule, 
    ReactiveFormsModule,
    RouterModule,
    DropdownModule,
  ],  
  templateUrl: './patient-edit.component.html',
  styleUrl: './patient-edit.component.scss'
})
export class PatientEditComponent implements OnInit{

  patient!: PatientResponseModel;
  patientForm!: FormGroup;
  statusEnum = PateintStatusEnum;
  token: any = '';
  serverError: string = '';

  constructor(
    private location:Location, 
    private _patientService: PatientsService, 
    private formBuilder: FormBuilder,
    private router: Router
  ){}

  ngOnInit(){
    this.patientForm = this.formBuilder.group({
      patientId: [''],
      firstName: [''],
      lastName: [''],
      dateOfBirth: [''],
      gender: [''],
      nationalId: [''],
      bloodType: ['', Validators.required],
      allergies: ['', Validators.required],
      chrinicConditions: ['', Validators.required],
      address: [''],
      emergencyContactName: [''],
      emergencyContactPhone: [''],
      status: [null, Validators.required],
      registrationDate: [''],
      userId: ['']
    });
    this.token = localStorage.getItem("token");
    const state: any = this.location.getState();

    this._patientService.getById(state.id, this.token).subscribe({
      next: data => {
        this.patient = data;
        this.buildForm(data);
      },
      error: err => console.log(err)
    })
  }

  buildForm(patient: PatientResponseModel) {
    this.patientForm = this.formBuilder.group({
      patientId: [patient.patientId],
      firstName: [patient.firstName],
      lastName: [patient.lastName],
      dateOfBirth: [patient.dateOfBirth],
      gender: [patient.gender],
      nationalId: [patient.nationalId],
      bloodType: [patient.bloodType, Validators.required],
      allergies: [patient.allergies, Validators.required],
      chrinicConditions: [patient.chrinicConditions, Validators.required],
      address: [patient.address],
      emergencyContactName: [patient.emergencyContactName],
      emergencyContactPhone: [patient.emergencyContactPhone],
      status: [patient.status as PateintStatusEnum, Validators.required],
      registrationDate: [patient.registrationDate],
      userId: [patient.userId]
    });
  }
  onSubmit() {
    this.serverError = '';
    if (this.patientForm.invalid) return;
    const patientToUpdate = {
      bloodType: this.patientForm.value.bloodType,
      allergies: this.patientForm.value.allergies,
      chrinicConditions: this.patientForm.value.chrinicConditions,
      status: this.patientForm.value.status,
    }

    this._patientService.editByStuff(this.patient.id, patientToUpdate, this.token).subscribe({
      next: data => {
        this.router?.navigate(['admin/patients']);
      },
      error: err => {
        this.serverError = err.error?.message || 'Invalid username or password'
      }
    })
  }
}
