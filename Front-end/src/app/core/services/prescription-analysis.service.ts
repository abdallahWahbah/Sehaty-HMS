import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { PrescriptionAnalysisAlternative } from '../models/prescriptionAnalysisalternative-model';
import { PrescriptionAnalysis } from '../models/prescription-analysis.model';
import { PatientHistoryAnalysis } from '../models/PatientHistoryAnalysis.model';
import { ApiResponse } from '../models/api-prescription-response';
@Injectable({
  providedIn: 'root',
})
export class PrescriptionAnalysisService {
  private readonly baseUrl = 'https://localhost:7086/api/OpenAI';

  constructor(private http: HttpClient) {}

  // ========= 1) Prescription AI (تفكيك data فقط) =========
  analyzePrescription(
    prescriptionId: number
  ): Observable<PrescriptionAnalysis> {
    const headers = this.getAuthHeaders();
    const url = `${this.baseUrl}/analyze-prescription/${prescriptionId}`;

    return this.http
      .get<ApiResponse<PrescriptionAnalysis>>(url, { headers })
      .pipe(map((res) => res.data)); // 👈 تفكيك الـ data فقط
  }

  // ========= 2) Patient History AI (بدون pipe – يرجع الريسبونس كامل) =========
  analyzePatientHistory(
    patientId: number
  ): Observable<ApiResponse<PatientHistoryAnalysis>> {
    const headers = this.getAuthHeaders();
    const url = `${this.baseUrl}/analyze-patient-history/${patientId}`;

    return this.http.get<ApiResponse<PatientHistoryAnalysis>>(url, { headers });
    // 👆 كده بيرجع { data, isSuccess, error } كامل
  }
  findalternative(
    prescriptionId: number
  ): Observable<PrescriptionAnalysisAlternative> {
    const headers = this.getAuthHeaders();
    const url = `${this.baseUrl}/analyze-prescription-alternatives/${prescriptionId}`;

    return this.http
      .get<
        | ApiResponse<PrescriptionAnalysisAlternative>
        | PrescriptionAnalysisAlternative
      >(url, { headers })
      .pipe(
        map((res: any) => {
          // لو الريسبونس ملفوف { data, isSuccess, error }
          if (res && 'data' in res) {
            return res.data as PrescriptionAnalysisAlternative;
          }
          // لو بيرجع الداتا مباشرة
          return res as PrescriptionAnalysisAlternative;
        })
      );
  }

  // ========= Helper =========
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return token
      ? new HttpHeaders({ Authorization: `Bearer ${token}` })
      : new HttpHeaders();
  }
}
