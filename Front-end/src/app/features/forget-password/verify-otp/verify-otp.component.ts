import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-verify-otp',
  imports: [
    FloatLabelModule, 
    InputTextModule, 
    FormsModule, 
    PasswordModule, 
    ButtonModule, 
    ReactiveFormsModule,
    RouterModule
  ],  
  templateUrl: './verify-otp.component.html',
  styleUrl: './verify-otp.component.scss'
})
export class VerifyOtpComponent {
  otp: string= '';
  serverError: string = '';

  loginForm = new FormGroup({
    otp: new FormControl('', [
      Validators.required,
    ])
  });
  
  constructor(
    private _authService: AuthService, 
    private router: Router, 
    private location: Location
  ){}

  onSubmit() {
    this.serverError = '';
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const otp = this.loginForm.get('otp')?.value as string;
    const state: any = this.location.getState();
    const email = state.email;

    
    this._authService.verifyOtp(email, otp).subscribe({
      next: data => {
        if(data.isValid){
          this.router.navigate(['setPassword'], {state: {email, otp}});
        }
        else{
          this.serverError = "Invalid OTP sent to your mobile";
        }
      },
    })
  }
}
