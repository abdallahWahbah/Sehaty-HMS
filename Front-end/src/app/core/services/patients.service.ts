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
  getById(id: number, token: string){
    return this._http.get<PatientResponseModel>(this.baseUrl + id, { 
      headers: { Authorization: `Bearer ${token}` }
    });
  }
  editByStuff(id: number, patientToUpdate: any, token: string) {
    return this._http.put<any>(
      this.baseUrl + id, 
      patientToUpdate, 
      { 
        headers: { Authorization: `Bearer ${token}` }
      }
    )
  }
}