import { Component, OnInit } from '@angular/core';
import { PatientsService } from '../../../core/services/patients.service';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-dashboard',
  imports: [DatePipe, CommonModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit{

  numOfPatients!: number;
  todaysAppointments: AppointmentResponseModel[] = [];
  numOfTodaysAppointments!: number;

  constructor(private _patientService: PatientsService, private _appointmentService: AppointmentService){}

  ngOnInit(): void {

    // number of patients
    this._patientService.getAll().subscribe({
      next: data => {
        this.numOfPatients = data.length;
      },
      error: err => {
        this.numOfPatients = 0;
        console.log(err);
      }
    });

    // num of today's appointments
    this._appointmentService.getAll().subscribe({
      next: data => {
        this.todaysAppointments = data;
        this.numOfTodaysAppointments = data.filter(item => {
          const appointmentDate = new Date(item.appointmentDateTime);
          const today = new Date();

          return (
            appointmentDate.getDate() === today.getDate() &&
            appointmentDate.getMonth() === today.getMonth() &&
            appointmentDate.getFullYear() === today.getFullYear()
          );
        }).length
      },
      error: err => {
        console.log(err);
      }
    })
  }
}
