import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { PatientsService } from '../../../core/services/patients.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-patient-appointments',
  imports: [CommonModule],
  templateUrl: './patient-appointments.component.html',
  styleUrls: ['./patient-appointments.component.scss'], // صححت styleUrls
})
export class PatientAppointmentsComponent implements OnInit {
  userId!: number;
  patientId!: number;
  appointments: AppointmentResponseModel[] = [];
  loading: boolean = true;

  constructor(
    private appointmentService: AppointmentService,
    private patientsService: PatientsService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // قراءة userData من localStorage
    const userDataString = localStorage.getItem('userData');
    if (userDataString) {
      try {
        const userData = JSON.parse(userDataString);
        this.userId = userData.userId;
      } catch (error) {
        console.error('Error parsing userData from localStorage', error);
        this.loading = false;
        return;
      }
    } else {
      console.error('No userData found in localStorage');
      this.loading = false;
      return;
    }

    // جلب بيانات المرضى والفلترة حسب userId
    this.patientsService.getAll().subscribe({
      next: (patients) => {
        const patient = patients.find((p) => p.userId === this.userId);

        if (patient) {
          this.patientId = patient.id;
          this.loadAppointments();
        } else {
          console.error('Patient not found for this user');
          this.loading = false;
        }
      },
      error: () => {
        console.error('Error loading patients');
        this.loading = false;
      },
    });
  }

  loadAppointments() {
    this.appointmentService.getByPatientId(this.patientId).subscribe({
      next: (data) => {
        this.appointments = data;
        this.loading = false;
      },
      error: () => {
        console.error('Error loading appointments');
        this.loading = false;
      },
    });
  }

  viewDetails(appointment: AppointmentResponseModel) {
    console.log('Appointment Details:', appointment);
    // هنا ممكن تفتح Dialog أو تروح لصفحة التفاصيل
  }

  trackById(index: number, item: AppointmentResponseModel) {
    return item.id;
  }
  navigateToAddAppointment() {
    this.router.navigate(['add'], { relativeTo: this.route });
  }
  goToPayment(appointmentId: number) {
    this.router.navigate(['/patient/payment', appointmentId]);
  }
}
