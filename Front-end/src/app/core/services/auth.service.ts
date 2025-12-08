import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginResponseModel } from '../models/login-response-model';
import { RegisterPatientModel } from '../models/register-patient-model';
import { DoctorAddBodyModel } from '../models/doctor-add-body-model';
export type UserRole = 'Patient' | 'Receptionist' | null;

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl: string = 'https://localhost:7086/api/Auth/';

  constructor(private _http: HttpClient) {}

  login(userName: string, password: string) {
    return this._http.post<LoginResponseModel>(this.baseUrl + 'login', {
      userName,
      password,
      ipAddress: 'string',
    });
  }

  refreshToken() {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');

    return this._http.post<LoginResponseModel>(this.baseUrl + 'refresh-token', {
      token,
      refreshToken,
    });
  }

  register(newUser: RegisterPatientModel) {
    return this._http.post<any>(this.baseUrl + 'register', newUser);
  }

  registerDoctor(newDoctor: DoctorAddBodyModel) {
    return this._http.post<any>(this.baseUrl + 'register-doctor', newDoctor);
  }

  requestPasswordReset(email: string) {
    return this._http.post<any>(this.baseUrl + 'request-password-reset', {
      email,
    });
  }

  verifyOtp(email: string, otp: string) {
    return this._http.post<any>(this.baseUrl + 'verify-otp', {
      email,
      otp,
    });
  }

  setNewPassword(email: string, otp: string, password: string) {
    return this._http.post<any>(this.baseUrl + 'reset-password', {
      email,
      otp,
      newPassword: password,
    });
  }
  getCurrentUserRole(): UserRole {
    const userDataString = localStorage.getItem('userData');
    if (!userDataString) {
      console.warn('No userData in localStorage');
      return null;
    }

    try {
      const userData = JSON.parse(userDataString);

      const role: string | undefined = userData.role;

      // لو انت عندك Patient و Receptionist بس
      if (role === 'Patient' || role === 'Receptionist') {
        return role;
      }

      console.warn('Unknown role from userData:', role);
      return null;
    } catch (error) {
      console.error('Error parsing userData from localStorage', error);
      return null;
    }
  }
}
