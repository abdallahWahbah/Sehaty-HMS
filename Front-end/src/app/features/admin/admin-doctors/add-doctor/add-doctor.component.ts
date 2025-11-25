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

  constructor(
    private fb: FormBuilder,
    private doctorService: DoctorService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.doctorForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      specialty: ['', [Validators.required, Validators.maxLength(100)]],
      licenseNumber: ['', [Validators.required, Validators.maxLength(50)]],
      qualifications: [''],
      yearsOfExperience: [''],
      availabilityNotes: [''],
      userId: ['', Validators.required],
      departmentId: ['', Validators.required],
    });
  }

  onSubmit() {
    if (this.doctorForm.invalid) return;

    this.doctorService.addDoctor(this.doctorForm.value).subscribe({
      next: () => {
        this.router.navigate(['/admin/doctors']);
      },
      error: (err) => {
        this.serverError = err.error?.message || 'Something went wrong!';
      },
    });
  }
}
