import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Doctor } from '../models/doctor-response-model';
@Injectable({
  providedIn: 'root',
})
export class doctorService {
  private baseUrl = 'https://localhost:7086/api/Doctors';

  constructor(private http: HttpClient) {}
  getAllDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(this.baseUrl);
  }
}
