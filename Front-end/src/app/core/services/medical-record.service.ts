import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { MedicalRecordModel } from '../models/medical-record-model';
import { MedicalRecord } from '../models/medicalrecord-response.model';

@Injectable({
  providedIn: 'root',
})
export class MedicalRecordService {
  private baseUrl = 'https://localhost:7086/api/MedicalRecords/';
  token = localStorage.getItem('token');

  constructor(private http: HttpClient) {}

  getAll(): Observable<MedicalRecordModel[]> {
    return this.http.get<MedicalRecordModel[]>(this.baseUrl);
  }

  getForPatient() {
    return this.http.get<MedicalRecordModel>(
      this.baseUrl + 'GetMedicalRecordForPatient',
      {
        headers: { Authorization: `Bearer ${this.token}` },
      }
    );
  }
  getMedicalRecordByPatientId(patientId: number): Observable<MedicalRecord> {
    const token = localStorage.getItem('token');
    const headers = token
      ? new HttpHeaders({ Authorization: `Bearer ${token}` })
      : new HttpHeaders();

    const url = `${this.baseUrl}GetMedicalRecordByPatientId/${patientId}`;

    return this.http.get<MedicalRecord>(url, { headers }).pipe(
      map((record) => ({
        ...record,
        recordDate: new Date(record.recordDate),
        createdAt: new Date(record.createdAt),
      }))
    );
  }
}
