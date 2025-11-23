import { Location } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DoctorResponseModel } from '../../../../core/models/doctor-response-model';
import { DoctorService } from '../../../../core/services/doctor.service';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
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
    FileUploadModule
  ],  
  templateUrl: './doctor-edit.component.html',
  styleUrl: './doctor-edit.component.scss'
})
export class DoctorEditComponent {

  doctor!: DoctorResponseModel;
  doctorForm!: FormGroup;
  serverError: string = '';
  selectedFile!: File;

  constructor(private location:Location, private formBuilder: FormBuilder, private _doctorService: DoctorService){}

  ngOnInit(){
    this.doctorForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      specialty: ['', Validators.required],
      licenseNumber : ['', Validators.required],
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
      error: err => {
        console.log(err);
        this.serverError = err
      }
    })
  }

  onFileSelected(event: any) {
    this.selectedFile = event.files[0];
  }

  buildForm(doctor: any) {
    this.doctorForm = this.formBuilder.group({
      firstName: [doctor.firstName, Validators.required],
      lastName: [doctor.lastName, Validators.required],
      specialty: [doctor.specialty, Validators.required],
      licenseNumber : [doctor.licenseNumber, Validators.required],
      qualifications: [doctor.qualifications],
      yearsOfExperience: [doctor.yearsOfExperience],
      availabilityNotes: [doctor.availabilityNotes],
      userId: [doctor.userId],
      departmentId: [doctor.departmentId],

    });
  }

  onSubmit() {
    this.serverError = '';
    if(!this.selectedFile) {
      this.serverError = "Please select a profile photo for the doctor";
    }
    if (this.doctorForm.invalid || !this.selectedFile) return;

    const formData = new FormData();
    Object.keys(this.doctorForm.value).forEach(key => {
      formData.append(key, this.doctorForm.value[key]);
    });

    if(this.selectedFile){
      formData.append("profilePhoto", this.selectedFile);
    }

    this._doctorService.updateDoctor(this.doctor.id, formData).subscribe({
      next: (res) => {
        console.log("Doctor updated successfully");
      },
      error: (err) => {
        console.error("Error updating doctor", err);
        this.serverError = err.error.message;
      }
    });
  }
}
