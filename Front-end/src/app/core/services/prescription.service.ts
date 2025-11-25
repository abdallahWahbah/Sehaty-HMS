import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Prescription } from '../models/prescription-response-model';

@Injectable({
  providedIn: 'root',
})
export class PrescriptionService {
  private baseUrl = 'https://localhost:7086/api/Prescriptions';

  constructor(private _http: HttpClient) {}

  // Get all prescriptions
  getAllPrescriptions(): Observable<Prescription[]> {
    return this._http.get<Prescription[]>(this.baseUrl);
  }

  // Get a single prescription by ID
  getPrescriptionById(id: number): Observable<Prescription> {
    return this._http.get<Prescription>(`${this.baseUrl}/${id}`);
  }

  // Get prescriptions by doctor ID
  getPrescriptionsByDoctor(doctorId: number): Observable<Prescription[]> {
    return this._http.get<Prescription[]>(
      `${this.baseUrl}?doctorId=${doctorId}`
    );
  }

  // Edit (update) a prescription
  editPrescription(
    id: number,
    updateData: Partial<Prescription>
  ): Observable<Prescription> {
    return this._http.put<Prescription>(`${this.baseUrl}/${id}`, updateData);
  }

  // Delete a prescription
  deletePrescription(id: number): Observable<void> {
    return this._http.delete<void>(`${this.baseUrl}/${id}`);
  }

  // Add a new prescription

  addPrescription(
    data: Partial<Prescription>,
    patientId: number,
    appointmentId: number
  ): Observable<Prescription> {
    const token = localStorage.getItem('token'); // أو من appConfig
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    return this._http.post<Prescription>(
      this.baseUrl,
      { ...data, appointmentId, patientId },
      { headers }
    );
  }
  getPatientPrescriptions() {
    const token = localStorage.getItem('token');
    const headers = token
      ? new HttpHeaders({ Authorization: `Bearer ${token}` })
      : undefined;

    return this._http
      .get<Prescription[]>(`${this.baseUrl}/patientprescriptions`, { headers })
      .pipe(
        map((list) =>
          list.map((p) => ({
            ...p,
            dateIssued: new Date(p.dateIssued),
          }))
        )
      );
  }
  downloadPrescription(id: number): Observable<Blob> {
    const url = `${this.baseUrl}/prescriptions/${id}/download`;
    return this._http.get(url, { responseType: 'blob' });
  }
}
