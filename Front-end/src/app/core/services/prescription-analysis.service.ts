import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PrescriptionAnalysis } from '../models/prescription-analysis.model';

@Injectable({
  providedIn: 'root',
})
export class PrescriptionAnalysisService {
  private readonly baseUrl = 'https://localhost:7086/api/OpenAI';

  constructor(private http: HttpClient) {}

  analyzePrescription(
    prescriptionId: number
  ): Observable<PrescriptionAnalysis> {
    const token = localStorage.getItem('token');
    const headers = token
      ? new HttpHeaders({ Authorization: `Bearer ${token}` })
      : new HttpHeaders();

    const url = `${this.baseUrl}/analyze/${prescriptionId}`;
    return this.http.get<PrescriptionAnalysis>(url, { headers });
  }
}
