import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BillingResponseModel } from '../models/billing-response-model';

@Injectable({
  providedIn: 'root'
})
export class BillingService {

  baseUrl: string = 'https://localhost:7086/api/Billing/';
  token = localStorage.getItem('token')!;

  constructor(private http: HttpClient) { }

  getAllBillings(){
    return this.http.get<BillingResponseModel[]>(this.baseUrl, {
      headers: { 
        Authorization: `Bearer ${this.token}` 
      },
    });
  }
  getBillingForPatient(id: number){
    return this.http.get<BillingResponseModel[]>(this.baseUrl + "GetAllForPatient/" + id, {
      headers: { 
        Authorization: `Bearer ${this.token}` 
      },
    });
  }
}
