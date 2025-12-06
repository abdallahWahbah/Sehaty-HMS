import { Location } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { DoctorResponseModel } from '../../../../core/models/doctor-response-model';
import { DoctorService } from '../../../../core/services/doctor.service';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { Router, RouterModule } from '@angular/router';
import { FileUploadModule } from 'primeng/fileupload';
import { DepartmentService } from '../../../../core/services/department.service';
import { Department } from '../../../../core/models/department-response.model';

@Component({
  selector: 'app-doctor-edit',
  imports: [
    FloatLabelModule,
    InputTextModule,
    FormsModule,
    ButtonModule,
    ReactiveFormsModule,
    RouterModule,
    FileUploadModule,
  ],
  templateUrl: './doctor-edit.component.html',
  styleUrl: './doctor-edit.component.scss',
})
export class DoctorEditComponent {
  doctor!: DoctorResponseModel;
  doctorForm!: FormGroup;
  departments: Department[] = [];
  serverError: string = '';
  storedUser: any;

  constructor(
    private location: Location,
    private formBuilder: FormBuilder,
    private _doctorService: DoctorService,
    private _departmentService: DepartmentService,
    private router: Router
  ) {}

  ngOnInit() {
    this.storedUser = localStorage.getItem('userData');
    this.storedUser = JSON.parse(this.storedUser);

    this.doctorForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      specialty: ['', Validators.required],
      licenseNumber: ['', Validators.required],
      qualifications: [''],
      yearsOfExperience: [''],
      availabilityNotes: [''],
      detectionPrice: [''],
      userId: [''],
      departmentId: [''],
    });
    const state: any = this.location.getState();

    this._doctorService.getById(state.id).subscribe({
      next: (data: any) => {
        this.doctor = data;
        this.buildForm(data);
      },
      error: (err) => {
        this.serverError = err.error.message;
      },
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

  buildForm(doctor: any) {
    this.doctorForm = this.formBuilder.group({
      firstName: [doctor.firstName, Validators.required],
      lastName: [doctor.lastName, Validators.required],
      specialty: [doctor.specialty, Validators.required],
      licenseNumber: [doctor.licenseNumber, Validators.required],
      qualifications: [doctor.qualifications],
      yearsOfExperience: [doctor.yearsOfExperience],
      availabilityNotes: [doctor.availabilityNotes],
      detectionPrice: [doctor.detectionPrice],
      userId: [doctor.userId],
      departmentId: [doctor.departmentId],
    });
    this.doctorForm.get('userId')?.disable();
  }

  onSubmit() {
    this.serverError = '';
    if (this.doctorForm.invalid) return;
    this._doctorService
      .updateDoctor(this.doctor.id, {
        ...this.doctorForm.value, 
        departmentId: +this.doctorForm.value.departmentId,
        userId: this.doctor.userId, 
        price: 0
      })
      .subscribe({
        next: (res) => {
          const url = this.router.url;
          const navigateTo = url.split('/')[1];
          switch (navigateTo) {
            case 'doctor': {
              this.router.navigate(['/doctor/details']);
              break;
            }
            case 'admin': {
              this.router.navigate(['/admin/doctors']);
              break;
            }
          }
        },
        error: (err) => {
          console.error('Error updating doctor', err);
          this.serverError = err.error.message;
        },
      }
    );
  }
}
