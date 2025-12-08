import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';

import { PatientsService } from '../../../core/services/patients.service';
import { AppointmentService } from '../../../core/services/appointment.service';
import { AppointmentResponseModel } from '../../../core/models/appointment-response-model';

import { DoctorService } from '../../../core/services/doctor.service';
import { DepartmentService } from '../../../core/services/department.service';
import { DoctorAvailabilityService } from '../../../core/services/doctoravilable.service';

import { forkJoin, of } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-admin-dashboard',
  // standalone: true, // لو شغال Standalone
  imports: [CommonModule, DatePipe],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss',
})
export class AdminDashboardComponent implements OnInit {
  // ====== Counts ======
  numOfPatients: number = 0;
  numOfDoctors: number = 0;
  numOfDepartments: number = 0;
  numOfTodaysAppointments: number = 0;
  availableDoctorsTodayCount: number = 0;

  // ====== Appointments list ======
  todaysAppointments: AppointmentResponseModel[] = [];

  constructor(
    private _patientService: PatientsService,
    private _appointmentService: AppointmentService,
    private _doctorService: DoctorService,
    private _departmentService: DepartmentService,
    private _doctorAvailabilityService: DoctorAvailabilityService
  ) {}

  ngOnInit(): void {
    this.loadPatientsCount();
    this.loadDoctorsCount();
    this.loadDepartmentsCount();
    this.loadTodaysAppointments();
    this.loadAvailableDoctorsToday();
  }

  // =========================
  //   LOAD COUNTS & DATA
  // =========================

  private loadPatientsCount(): void {
    this._patientService.getAll().subscribe({
      next: (data) => {
        this.numOfPatients = data.length;
      },
      error: () => {
        this.numOfPatients = 0;
      },
    });
  }

  private loadDoctorsCount(): void {
    this._doctorService.getAllDoctors().subscribe({
      next: (data) => {
        this.numOfDoctors = data.length;
      },
      error: () => {
        this.numOfDoctors = 0;
      },
    });
  }

  private loadDepartmentsCount(): void {
    this._departmentService.getAllDepartments().subscribe({
      next: (data) => {
        this.numOfDepartments = data.length;
      },
      error: () => {
        this.numOfDepartments = 0;
      },
    });
  }

  private loadTodaysAppointments(): void {
    this._appointmentService.getAll().subscribe({
      next: (data) => {
        const todayDateOnly = this.getLocalDateString(new Date());

        this.todaysAppointments = data.filter((item) => {
          const appointmentDate = new Date(item.appointmentDateTime);
          const appointmentDateOnly = this.getLocalDateString(appointmentDate);
          return appointmentDateOnly === todayDateOnly;
        });

        this.numOfTodaysAppointments = this.todaysAppointments.length;
      },
      error: () => {
        this.todaysAppointments = [];
        this.numOfTodaysAppointments = 0;
      },
    });
  }

  /**
   * حساب عدد الدكاترة اللي عندهم على الأقل Slot متاح النهاردة
   * بدون ما يطلع Errors في الكونسول لو الـ GET فشل لبعض الدكاترة
   */
  private loadAvailableDoctorsToday(): void {
    const today = new Date();
    const dateStr = this.getLocalDateString(today); // yyyy-MM-dd بالتوقيت المحلي

    this._doctorService
      .getAllDoctors()
      .pipe(
        switchMap((doctors) => {
          if (!doctors || doctors.length === 0) {
            return of(0);
          }

          const requests = doctors.map((doc) => {
            const doctorId =
              (doc as any).id ??
              (doc as any).doctorId ??
              (doc as any).doctorID ??
              null;

            // لو مفيش id أصلاً للدكتور نعتبره مش متاح
            if (!doctorId) {
              return of({ hasSlots: false });
            }

            return this._doctorAvailabilityService
              .getAvailableSlots(doctorId, dateStr)
              .pipe(
                map((slots) => ({
                  hasSlots: !!slots && slots.length > 0,
                })),
                // هنا بنبلع أي Error ونرجع false من غير ما نظهر حاجة في الكونسول
                catchError(() => {
                  return of({ hasSlots: false });
                })
              );
          });

          return forkJoin(requests).pipe(
            map((results) => results.filter((x) => x.hasSlots).length)
          );
        })
      )
      .subscribe({
        next: (count) => {
          this.availableDoctorsTodayCount = count;
        },
        error: () => {
          this.availableDoctorsTodayCount = 0;
        },
      });
  }

  // =========================
  //   HELPERS
  // =========================

  // دالة ترجع تاريخ محلي بصيغة yyyy-MM-dd
  private getLocalDateString(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private checkTheSameDay(appointmentDate: Date, today: Date): boolean {
    return (
      appointmentDate.getDate() === today.getDate() &&
      appointmentDate.getMonth() === today.getMonth() &&
      appointmentDate.getFullYear() === today.getFullYear()
    );
  }
}
