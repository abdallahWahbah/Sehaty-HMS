import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppointmentResponseModel } from '../models/appointment-response-model';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {

  baseUrl: string = "https://localhost:7086/api/Appointments/"

  constructor(private _http: HttpClient) { }

  getAll(){
    return this._http.get<AppointmentResponseModel[]>(this.baseUrl);
  }
}