import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MedicalRecordModel } from '../models/medical-record-model';

@Injectable({
  providedIn: 'root'
})
export class MedicalRecordService {

  private baseUrl = 'https://localhost:7086/api/MedicalRecords/';
  token = localStorage.getItem('token');

  constructor(private http: HttpClient) {}

  getAll(): Observable<MedicalRecordModel[]> {
    return this.http.get<MedicalRecordModel[]>(this.baseUrl);
  }
  
  getAllForPatient(){
    return this.http.get<MedicalRecordModel>(this.baseUrl + 'GetMedicalRecordForPatient', {
      headers: { Authorization: `Bearer ${this.token}` }
    })
  }
}
