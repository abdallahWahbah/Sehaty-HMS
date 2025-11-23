import { Location } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DoctorService } from '../../../../core/services/doctor.service';
import { DoctorResponseModel } from '../../../../core/models/doctor-response-model';

@Component({
  selector: 'app-doctor-self-edit',
  imports: [],
  templateUrl: './doctor-self-edit.component.html',
  styleUrl: './doctor-self-edit.component.scss'
})
export class DoctorSelfEditComponent {

  doctor!: DoctorResponseModel;
  doctorForm!: FormGroup;
  serverError: string = '';
  constructor(private location: Location, private formBuilder: FormBuilder, private _doctorService: DoctorService){}

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

    // this._doctorService.getById(state.id).subscribe({
    //   next: (data: any) => {
    //     console.log(data);
    //     this.doctor = data;
    //     this.buildForm(data);
    //   },
    //   error: err => {
    //     console.log(err);
    //     this.serverError = err
    //   }
    // })
  }


}
