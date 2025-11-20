import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginResponseModel } from '../models/login-response-model';

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
  
  register(
    userName: string, 
    email: string, 
    phoneNumber: string, 
    firstName: string, 
    lastName: string, 
    password: string,
    confirmPassword: string,
    ) {
    return this._http.post<any>(this.baseUrl + 'register', {
      userName,
      email,
      phoneNumber,
      firstName,
      lastName,
      password,
      confirmPassword,
      languagePreference: 'Arabic'
    });
  }
}
