import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PatientResponseModel } from '../models/patient-response-model';

@Injectable({
  providedIn: 'root'
})
export class PatientsService {
  baseUrl: string = "https://localhost:7086/api/Patients/"

  constructor(private _http: HttpClient) { }

  getAll(){
    return this._http.get<PatientResponseModel[]>(this.baseUrl);
  }
}
