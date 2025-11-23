import { Component } from '@angular/core';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorService } from '../../../core/services/doctor.service';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-doctor-appointments',
  imports: [CommonModule],
  templateUrl: './doctor-appointments.component.html',
  styleUrl: './doctor-appointments.component.scss'
})
export class DoctorAppointmentsComponent {

  appointments: AppointmentResponseModel[] = [];
  currentDoctor!: DoctorResponseModel;

  constructor(private _appointmentsService: AppointmentService, private _doctorService: DoctorService){}

  ngOnInit(){
    let storedUser: any = localStorage.getItem("userData");
    storedUser = JSON.parse(storedUser);

    this._doctorService.getAllDoctors().subscribe({
      next: allDoctors => {
        this.currentDoctor = allDoctors.filter(doc => doc.userId === storedUser.userId)[0];
      }
    })

    this._appointmentsService.getAll().subscribe({
      next: data => {
        this.appointments = data.filter(app => app.doctorId === this.currentDoctor?.id);
      }
    });
  }
}
