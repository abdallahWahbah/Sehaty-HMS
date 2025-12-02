import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { DoctorService } from '../../../../core/services/doctor.service';
import { Router } from '@angular/router';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { DepartmentService } from '../../../../core/services/department.service';
import { Department } from '../../../../core/models/department-response.model';

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
    private doctorService: DoctorService,
    private router: Router,
    private _departmentService: DepartmentService,
  
  ) {}

  ngOnInit(): void {
    this.doctorForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      userName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      password: ['P@ssw0rd', [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(/^(?=.*[a-z]).*$/),
          Validators.pattern(/^(?=.*[A-Z]).*$/),
          Validators.pattern(/^(?=.*\d).*$/),
          Validators.pattern(/^(?=.*[\W_]).*$/),
          Validators.pattern(/^\S+$/)
      ]],
      confirmPassword: ['P@ssw0rd', Validators.required],
      specialty: ['', [Validators.required, Validators.maxLength(100)]],
      licenseNumber: ['', [Validators.required, Validators.maxLength(50)]],
      detectionPrice: [0, Validators.required],
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

    console.log("111111111111", this.doctorForm.value);

    // this.doctorService.addDoctor(this.doctorForm.value).subscribe({
    //   next: () => {
    //     this.router.navigate(['/admin/doctors']);
    //   },
    //   error: (err) => {
    //     this.serverError = err.error?.message || 'Something went wrong!';
    //   },
    // });
  }
}
