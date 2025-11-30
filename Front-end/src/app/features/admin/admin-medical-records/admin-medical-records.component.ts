import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Dialog } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { MedicalRecordModel } from '../../../core/models/medical-record-model';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';
import { MedicalRecordService } from '../../../core/services/medical-record.service';
import { AppointmentService } from '../../../core/services/appointment.service';
import { RecordComponent } from "./record/record.component";

@Component({
  selector: 'app-admin-medical-records',
  imports: [CommonModule, Dialog, ButtonModule, RecordComponent],
  templateUrl: './admin-medical-records.component.html',
  styleUrl: './admin-medical-records.component.scss'
})
export class AdminMedicalRecordsComponent implements OnInit{
  records: MedicalRecordModel[] | any = [];
  appointments: AppointmentResponseModel[] = [];
  selectedRecord: MedicalRecordModel | null = null;
  visible: boolean = false;

  constructor(
    private medicalRecordService: MedicalRecordService,
    private appointmentService: AppointmentService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadRecordsWithAppointments();
  }

   loadRecordsWithAppointments() {
    forkJoin({
      appointments: this.appointmentService.getAll(),
      records: this.medicalRecordService.getAll(),
    }).subscribe({
      next: ({ appointments, records }) => {
        this.appointments = appointments;

          this.records = records.map((r) => {
            const appointment = this.appointments.find(
              (a) => a.id === r.appointmentId
            );

            return {
              ...r,
              patientName: r.patientName || appointment?.patientName || 'Unknown',
              appointmentDateTime: appointment?.appointmentDateTime,
            };
          });
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error fetching data:', err),
    });
  }
  viewRecord(record: MedicalRecordModel) {
    this.selectedRecord = record; 
    this.visible = true;
  }
}
