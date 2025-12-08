import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { FeedbackDto } from '../models/feedback.request';
import { FeedbackResponseModel } from '../models/feedback.response';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class FeedbackService {
  private baseUrl: string = 'https://localhost:7086/api/Feedbacks/';
  constructor(private http: HttpClient) {}
  add(feedback: FeedbackDto): Observable<FeedbackResponseModel> {
    return this.http
      .post<FeedbackResponseModel>(this.baseUrl, feedback)
      .pipe(catchError(this.handleError.bind(this)));
  }

  // جلب كل الفيدباكات (GET)
  getAll(): Observable<FeedbackResponseModel[]> {
    return this.http.get<FeedbackResponseModel[]>(this.baseUrl).pipe(
      map((list) =>
        list.map((item) => ({
          ...item,
          submittedAt: new Date(item.submittedAt),
          appointmentDateTime: new Date(item.appointmentDateTime),
        }))
      ),
      catchError(this.handleError.bind(this))
    );
  }

  // جلب فيدباك محدد (GET by id)
  getById(id: number): Observable<FeedbackResponseModel> {
    return this.http.get<FeedbackResponseModel>(`${this.baseUrl}${id}`).pipe(
      map((item) => ({
        ...item,
        submittedAt: new Date(item.submittedAt),
        appointmentDateTime: new Date(item.appointmentDateTime),
      })),
      catchError(this.handleError.bind(this))
    );
  }

  // دالة معالجة الأخطاء
  private handleError(error: HttpErrorResponse) {
    console.error('Feedback API error:', error);
    return throwError(() => error);
  }
  // feedback.service.ts (أضف هذا)
  getByAppointmentId(appointmentId: number) {
    // عدّل المسار لو endpoint بتاعك اسمه مختلف
    return this.http
      .get<FeedbackResponseModel>(
        `${this.baseUrl}GetByAppointmentId/${appointmentId}`
      )
      .pipe(
        map((item) => ({
          ...item,
          submittedAt: new Date(item.submittedAt),
          appointmentDateTime: new Date(item.appointmentDateTime),
        })),
        catchError(this.handleError.bind(this))
      );
  }
}
