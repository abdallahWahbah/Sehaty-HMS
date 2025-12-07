import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { AppointmentService } from '../../../core/services/appointment.service';
import { PatientsService } from '../../../core/services/patients.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingSpinnerComponent } from "../../../layout/loading-spinner/loading-spinner.component";
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ConfirmationDialogComponent } from "../../../layout/confirmation-dialog/confirmation-dialog.component";

@Component({
  selector: 'app-patient-appointments',
  imports: [CommonModule, LoadingSpinnerComponent, Toast, ConfirmationDialogComponent],
  templateUrl: './patient-appointments.component.html',
  styleUrls: ['./patient-appointments.component.scss'], // صححت styleUrls
  providers: [MessageService]
})
export class PatientAppointmentsComponent implements OnInit {
  userId!: number;
  patientId!: number;
  appointments: AppointmentResponseModel[] = [];
  loading: boolean = true;
  loadingCancel: boolean = false;
  showDialog: boolean = false;

  constructor(
    private appointmentService: AppointmentService,
    private patientsService: PatientsService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService
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

  navigateToAddAppointment() {
    this.router.navigate(['add'], { relativeTo: this.route });
  }
  goToPayment(appointmentId: number) {
    this.router.navigate(['/home/payment', appointmentId]);
  }
  handleCancel(appointment: AppointmentResponseModel){
    this.loadingCancel = true;
    if(this.checkLessThanDay(appointment.appointmentDateTime)) {
      this.showDialog = true;
    }
    else {
      this.cancelAppointment(appointment)
    }
  }
  cancelAppointment(appointment: AppointmentResponseModel){
    this.appointmentService.cancel(appointment.id).subscribe({
      next: data => {
        this.loadingCancel = false;
        window.location.reload();
      },
      error: err => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error.message });
        this.loadingCancel = false;
      }
    });
  }
  checkLessThanDay(date1: Date | string){
    const now = new Date();
    const diffMs = new Date(date1).getTime() - now.getTime();
    const diffHours = diffMs / (1000 * 60 * 60);
    this.loadingCancel = true;
    if (diffHours < 24 && diffHours > 0){
      return true
    }
    return false;
  }
  closeDialog(){
    this.showDialog = false;
  }
}
