import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DoctorAvailableSlotsModel } from '../models/availability-slot-model';

@Injectable({
  providedIn: 'root'
})
export class DoctorAvailableSlotService {
  private baseUrl = 'https://localhost:7086/api/DoctorAvailabilitySlots/';

  constructor(private http: HttpClient) {}

  addAvailabilitySlot(body: DoctorAvailableSlotsModel){
    return this.http.post(this.baseUrl + 'AddAvailabilitySlot', body);
  }
  generateSlots(body: {doctorId: number, date: string}){
    return this.http.post(this.baseUrl + 'Generateslots', body);
  }
}
