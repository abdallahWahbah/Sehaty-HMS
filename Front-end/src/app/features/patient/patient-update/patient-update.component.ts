import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PatientsService } from '../../../core/services/patients.service';
import { PatientResponseModel } from '../../../core/models/patient-response-model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
@Component({
  selector: 'app-patient-update',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './patient-update.component.html',
  styleUrl: './patient-update.component.scss',
})
export class PatientUpdateComponent {
  patient!: PatientResponseModel;
  patientForm!: FormGroup;
  constructor(
    private fb: FormBuilder,
    private patientService: PatientsService,
    private router: Router
  ) {}
  // جلب بيانات المريض
  ngOnInit() {
    this.patientForm = this.fb.group({
      firstName: [''],
      lastName: [''],
      dateOfBirth: [''],
      gender: [''],
      address: [''],
      emergencyContactName: [''],
      emergencyContactPhone: [''],
    });
    // جلب بيانات المستخدم من localStorage
    const storedUser: any = JSON.parse(localStorage.getItem('userData')!);

    // جلب بيانات المريض
    this.patientService.getAll().subscribe({
      next: (data) => {
        this.patient = data.find((p) => p.userId === storedUser.userId)!;
        this.initForm();
      },
    });
  }
  initForm() {
    let formattedDob = this.patient.dateOfBirth
      ? new Date(this.patient.dateOfBirth).toISOString().split('T')[0]
      : '';

    this.patientForm = this.fb.group({
      firstName: [
        this.patient.firstName,
        [Validators.required, Validators.maxLength(20)],
      ],
      lastName: [
        this.patient.lastName,
        [Validators.required, Validators.maxLength(20)],
      ],
      dateOfBirth: [formattedDob, Validators.required],
      gender: [
        this.patient.gender,
        [Validators.required, Validators.maxLength(10)],
      ],
      address: [
        this.patient.address, 
        [Validators.required, Validators.maxLength(500)]
      ],
      emergencyContactName: [
        this.patient.emergencyContactName,
        [Validators.required, Validators.maxLength(100)]
      ],
      emergencyContactPhone: [
        this.patient.emergencyContactPhone,
        [Validators.required, Validators.pattern(/^(\+2)?(010|011|012)\d{8}$/)],
      ],
    });
  }
  saveChanges() {
    const token = localStorage.getItem('token')!;
    let body = this.patientForm.value;
    body = {...body, emergencyContactPhone: "+2" + body.emergencyContactPhone.replace("+2", "")}
    this.patientService.editByPatient(this.patient.id, body, token).subscribe({
      next: () => {
        Object.assign(this.patient, body);
        this.router.navigate(['/home/details']);
      },
      error: (err) => console.error(err),
    });
  }
}
