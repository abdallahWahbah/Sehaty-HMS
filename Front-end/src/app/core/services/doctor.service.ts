import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DoctorResponseModel } from '../models/doctor-response-model';
@Injectable({
  providedIn: 'root',
})
export class DoctorService {
  private baseUrl = 'https://localhost:7086/api/Doctors/';

  constructor(private http: HttpClient) {}
  
  getAllDoctors(): Observable<DoctorResponseModel[]> {
    return this.http.get<DoctorResponseModel[]>(this.baseUrl);
  }
  getById(id: number){
    return this.http.get(this.baseUrl + id);
  }
  updateDoctor(id: number, formData: FormData): Observable<any> {
    return this.http.put(this.baseUrl + id, formData);
  }
}
