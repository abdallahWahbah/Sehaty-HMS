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
  serverError: string = '';

  constructor(
    private location: Location,
    private formBuilder: FormBuilder,
    private _doctorService: DoctorService,
    private router: Router
  ) {}

  ngOnInit() {
    this.doctorForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      specialty: ['', Validators.required],
      licenseNumber: ['', Validators.required],
      qualifications: [''],
      yearsOfExperience: [''],
      availabilityNotes: [''],
      userId: [''],
      departmentId: [''],
    });
    const state: any = this.location.getState();

    this._doctorService.getById(state.id).subscribe({
      next: (data: any) => {
        console.log(data);
        this.doctor = data;
        this.buildForm(data);
      },
      error: (err) => {
        console.log(err);
        this.serverError = err;
      },
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
      userId: [doctor.userId],
      departmentId: [doctor.departmentId],
    });
  }

  onSubmit() {
    this.serverError = '';
    if (this.doctorForm.invalid) return;

    this._doctorService
      .updateDoctor(this.doctor.id, this.doctorForm.value)
      .subscribe({
        next: (res) => {
          console.log('Doctor updated successfully');
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
      });
  }
  onDelete() {
    if (!confirm('Are you sure you want to delete this doctor?')) {
      return;
    }

    this._doctorService.deleteDoctor(this.doctor.id).subscribe({
      next: () => {
        console.log('Doctor deleted successfully');

        // توجه حسب الروت الحالي
        const url = this.router.url;
        const navigateTo = url.split('/')[1];
        switch (navigateTo) {
          case 'doctor':
            this.router.navigate(['/doctor/details']);
            break;
          case 'admin':
            this.router.navigate(['/admin/doctors']);
            break;
          default:
            this.router.navigate(['/']);
        }
      },
      error: (err) => {
        console.error('Error deleting doctor', err);
        this.serverError = err.error.message || 'Failed to delete doctor.';
      },
    });
  }
}
