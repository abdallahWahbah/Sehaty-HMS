import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppointmentResponseModel } from '../models/appointment-response-model';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { CreateAppointmentDto } from '../models/appointment-create-model';
import { RescheduleAppointmentDto } from '../models/appointment-updateDate-model';

@Injectable({
  providedIn: 'root',
})
export class AppointmentService {
  baseUrl: string = 'https://localhost:7086/api/Appointments/';

  constructor(private http: HttpClient) {}

  private toAppointment(
    raw: AppointmentResponseModel
  ): AppointmentResponseModel {
    return {
      ...raw,
      appointmentDateTime:
        raw.appointmentDateTime && new Date(raw.appointmentDateTime),
    };
  }

  private handleError(error: HttpErrorResponse) {
    console.error('Appointment API error:', error);
    return throwError(() => error);
  }

  getAll(): Observable<AppointmentResponseModel[]> {
    return this.http.get<AppointmentResponseModel[]>(this.baseUrl).pipe(
      map((list) => list.map((item) => this.toAppointment(item))),
      catchError(this.handleError)
    );
  }

  getById(id: number): Observable<AppointmentResponseModel> {
    return this.http.get<AppointmentResponseModel>(`${this.baseUrl}${id}`).pipe(
      map((raw) => this.toAppointment(raw)),
      catchError(this.handleError)
    );
  }

  create(dto: CreateAppointmentDto): Observable<AppointmentResponseModel> {
    const body = {
      ...dto,
      appointmentDateTime:
        dto.appointmentDateTime instanceof Date
          ? dto.appointmentDateTime.toISOString()
          : dto.appointmentDateTime,
    };
    return this.http.post<AppointmentResponseModel>(this.baseUrl, body).pipe(
      map((raw) => this.toAppointment(raw)),
      catchError(this.handleError)
    );
  }

  delete(id: number): Observable<void> {
    return this.http
      .delete<void>(`${this.baseUrl}${id}`)
      .pipe(catchError(this.handleError));
  }

  markNoShow(id: number): Observable<void> {
    return this.http
      .post<void>(`${this.baseUrl}NoShow/${id}`, {})
      .pipe(catchError(this.handleError));
  }

  checkIn(id: number): Observable<void> {
    return this.http
      .post<void>(`${this.baseUrl}CheckIn/${id}`, {})
      .pipe(catchError(this.handleError));
  }

  cancel(id: number): Observable<void> {
    return this.http
      .post<void>(`${this.baseUrl}CancelAppointment/${id}`, {})
      .pipe(catchError(this.handleError));
  }

  reschedule(id: number, dto: RescheduleAppointmentDto): Observable<void> {
    const body = {
      newAppointmentDateTime:
        dto.newAppointmentDateTime instanceof Date
          ? dto.newAppointmentDateTime.toISOString()
          : dto.newAppointmentDateTime,
    };
    return this.http
      .put<void>(`${this.baseUrl}RescheduleAppointment/${id}`, body)
      .pipe(catchError(this.handleError));
  }

  confirm(id: number): Observable<void> {
    return this.http
      .post<void>(`${this.baseUrl}ConfirmAppointment/${id}`, {})
      .pipe(catchError(this.handleError));
  }
}
