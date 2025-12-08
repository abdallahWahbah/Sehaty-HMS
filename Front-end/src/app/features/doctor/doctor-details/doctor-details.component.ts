import { Component } from '@angular/core';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { DoctorService } from '../../../core/services/doctor.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-doctor-details',
  imports: [],
  templateUrl: './doctor-details.component.html',
  styleUrl: './doctor-details.component.scss'
})
export class DoctorDetailsComponent {
  currentDoctor!: DoctorResponseModel;

  constructor(private _doctorService: DoctorService, private router: Router){}

  ngOnInit(){
    let storedUser: any = localStorage.getItem("userData");
    storedUser = JSON.parse(storedUser);

    this._doctorService.getAllDoctors().subscribe({
      next: allDoctors => {
        this.currentDoctor = allDoctors.filter(doc => doc.userId === storedUser.userId)[0];
      }
    })
  }
}
