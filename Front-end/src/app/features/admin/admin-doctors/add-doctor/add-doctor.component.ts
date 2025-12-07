import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router } from '@angular/router';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { DepartmentService } from '../../../../core/services/department.service';
import { Department } from '../../../../core/models/department-response.model';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-add-doctor',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    FloatLabelModule,
    ButtonModule,
    InputTextModule,
  ],
  templateUrl: './add-doctor.component.html',
  styleUrl: './add-doctor.component.scss',
})
export class AddDoctorComponent {
  doctorForm!: FormGroup;
  serverError: string = '';
  departments: Department[] = [];

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private _departmentService: DepartmentService,
    private _authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.doctorForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      userName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^(\+2)?(010|011|012)\d{8}$/)]],
      password: ['', [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(/^(?=.*[a-z]).*$/),
          Validators.pattern(/^(?=.*[A-Z]).*$/),
          Validators.pattern(/^(?=.*\d).*$/),
          Validators.pattern(/^(?=.*[\W_]).*$/),
          Validators.pattern(/^\S+$/)
      ]],
      confirmPassword: ['', Validators.required],
      specialty: ['', [Validators.required, Validators.maxLength(100)]],
      licenseNumber: ['', [Validators.required, Validators.maxLength(50)]],
      detectionPrice: [150, Validators.required],
      qualifications: [''],
      yearsOfExperience: [''],
      availabilityNotes: [''],
      departmentId: ['', [Validators.required]],
    });

    this._departmentService.getAllDepartments().subscribe({
      next: (departments) => {
        this.departments = departments;
      },
      error: (err) => {
        this.serverError = err.error.message
      }
    });
  }

  onSubmit() {
    this.serverError = '';
    if (this.doctorForm.invalid) {
      this.doctorForm.markAllAsTouched();
      return;
    }

    const formValue = this.doctorForm.value;
    const newDoctorData = {
      ...formValue,
      departmentId: +formValue.departmentId,
      languagePreference: "Arabic",
    }

    this._authService.registerDoctor(newDoctorData).subscribe({
      next: () => {
        this.router.navigate(['/admin/doctors']);
      },
      error: (err) => {
        let concatenatedError = '';
        if(err.error.errors)
          for(let i = 0; i < err.error.errors.length; i++) concatenatedError += err.error.errors[i]
        this.serverError = err.error?.errors?.length > 0 ? concatenatedError : err.error?.message
      },
    });
  }
}
