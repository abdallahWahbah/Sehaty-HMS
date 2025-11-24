import { Component } from '@angular/core';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorService } from '../../../core/services/doctor.service';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-doctor-appointments',
  imports: [CommonModule],
  templateUrl: './doctor-appointments.component.html',
  styleUrl: './doctor-appointments.component.scss'
})
export class DoctorAppointmentsComponent {

 appointments: AppointmentResponseModel[] = [];
  currentDoctor!: DoctorResponseModel;
  isLoading: boolean = true;

  constructor(
    private _appointmentsService: AppointmentService,
    private _doctorService: DoctorService,
    private router: Router
  ) {}

  ngOnInit() {
    let storedUser: any = localStorage.getItem("userData");
    storedUser = JSON.parse(storedUser);

    // احضر بيانات الدكتور الحالي
    this._doctorService.getAllDoctors().subscribe({
      next: allDoctors => {
        this.currentDoctor = allDoctors.find(doc => doc.userId === storedUser.userId)!;
        this.loadAppointments();
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  private loadAppointments() {
    this._appointmentsService.getAll().subscribe({
      next: data => {
        this.appointments = data
          .filter(app => app.doctorId === this.currentDoctor?.id)
          .sort((a, b) => {
            const dateA = new Date(a.appointmentDateTime);
            const dateB = new Date(b.appointmentDateTime);

            const dayA = new Date(dateA.getFullYear(), dateA.getMonth(), dateA.getDate()).getTime();
            const dayB = new Date(dateB.getFullYear(), dateB.getMonth(), dateB.getDate()).getTime();

            if (dayA !== dayB) return dayB - dayA;

            return dateA.getTime() - dateB.getTime();
          });
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  // TrackBy function لتحسين أداء ngFor
  trackById(index: number, item: AppointmentResponseModel) {
    return item.id;
  }

  // زر Add Prescription
  addPrescription(appointment: AppointmentResponseModel) {
    this.router.navigate(['/doctor/prescriptions/add'], {
      state: {
        patientId: appointment.patientId,
        appointmentId: appointment.id,
      }
    });
    // this.router.navigate(['doctor/prescriptions/add'], {state: {email: 'lol', otp:'ya ngem'}});
  }
}