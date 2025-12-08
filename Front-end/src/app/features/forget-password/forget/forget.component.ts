import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { FloatLabelModule } from "primeng/floatlabel"
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-forget',
  imports: [
    FloatLabelModule, 
    InputTextModule, 
    FormsModule, 
    PasswordModule, 
    ButtonModule, 
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './forget.component.html',
  styleUrl: './forget.component.scss'
})
export class ForgetComponent {
  email: string= '';
  serverError: string = '';

  loginForm = new FormGroup({
    email: new FormControl('abdowahbah@gmail.com', [
      Validators.required,
      Validators.email
    ])
  });
  
  constructor(private _authService: AuthService, private router: Router){}

  onSubmit() {
    this.serverError = '';
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    const email = this.loginForm.get('email')?.value as string;

    this._authService.requestPasswordReset(email).subscribe({
      next: data => {
        this.router.navigate(['verifyOtp'], {state: {email}});
      },
      error: err => this.serverError = err.statusText
    })
  }
}
