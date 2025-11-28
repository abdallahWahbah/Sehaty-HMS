import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { DoctorResponseModel } from '../models/doctor-response-model';
import { AvailableDayModel } from '../models/available-day.model';
import { AvailableSlotModel, Slot } from '../models/available-slot.model';
import { BookedAppointmentModel } from '../models/booked-appointment.model';

@Injectable({
  providedIn: 'root',
})
export class DoctorAvailabilityService {
  private baseUrl = 'https://localhost:7086/api/DoctorAvailabilitySlots';

  constructor(private http: HttpClient) {}

  // تجيب كل الأيام المتاحة لدكتور محدد
  getAvailableDaysForDoctor(doctorId: number): Observable<AvailableDayModel> {
    return this.http.get<AvailableDayModel>(
      `${this.baseUrl}/availableDaysForDoctor/${doctorId}`
    );
  }

  // تجيب كل الـ slots المتاحة لدكتور في يوم محدد
  getAvailableSlots(doctorId: number, date: string): Observable<Slot[]> {
    return this.http
      .get<AvailableSlotModel>(`${this.baseUrl}/AvailableSlots`, {
        params: { doctorId: doctorId.toString(), date },
      })
      .pipe(map((res) => res.slots ?? [])); // نرجع فقط مصفوفة slots
  }

  // حجز slot لمريض مع سبب الزيارة
  bookSlot(
    slotId: number,
    patientId: number,
    reasonForVisit: string
  ): Observable<BookedAppointmentModel> {
    console.log("00000000000000000", patientId);
    return this.http.post<BookedAppointmentModel>(`${this.baseUrl}/BookSlot`, {
      slotId,
      patientId,
      reasonForVisit,
    });
  }

  // اختياري: فلترة الدكاترة حسب القسم
  filterDoctorsByDepartment(
    doctors: DoctorResponseModel[],
    departmentId: number
  ): DoctorResponseModel[] {
    return doctors.filter((d) => d.departmentId === departmentId);
  }
}
