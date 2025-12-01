import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, Observable } from 'rxjs';

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
    const url = `${this.baseUrl}/analyze/${prescriptionId}`;

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

  // ========= Helper =========
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return token
      ? new HttpHeaders({ Authorization: `Bearer ${token}` })
      : new HttpHeaders();
  }
}
