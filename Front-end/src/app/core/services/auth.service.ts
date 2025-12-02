import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginResponseModel } from '../models/login-response-model';
import { RegisterPatientModel } from '../models/register-patient-model';
import { DoctorAddBodyModel } from '../models/doctor-add-body-model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseUrl: string = "https://localhost:7086/api/Auth/"

  constructor(private _http: HttpClient) { }

  login(userName: string, password: string){
    return this._http.post<LoginResponseModel>(this.baseUrl + 'login', {
      userName,
      password,
      ipAddress: "string"
    });
  }

  refreshToken() {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem("refreshToken");

    return this._http.post<LoginResponseModel>(
      this.baseUrl + 'refresh-token',
      {
        token,
        refreshToken
      }
    );
  }
  
  register(newUser: RegisterPatientModel) {
    return this._http.post<any>(this.baseUrl + 'register', newUser);
  }
  
  registerDoctor(newDoctor: DoctorAddBodyModel){
    return this._http.post<any>(this.baseUrl + 'register-doctor', newDoctor);
  }

  requestPasswordReset(email: string){
    return this._http.post<any>(this.baseUrl + 'request-password-reset', {
      email
    });
  }

  verifyOtp(email: string, otp: string){
    return this._http.post<any>(this.baseUrl + 'verify-otp', {
      email, 
      otp
    })
  }

  setNewPassword(email: string, otp: string, password: string){
    return this._http.post<any>(this.baseUrl + 'reset-password', {
      email,
      otp,
      newPassword: password
    })
  }
}
