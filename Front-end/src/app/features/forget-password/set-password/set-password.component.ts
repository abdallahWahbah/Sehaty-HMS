import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { Checkbox } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../../core/services/auth.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-set-password',
  imports: [
    FloatLabelModule,
    InputTextModule,
    FormsModule,
    PasswordModule,
    ButtonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './set-password.component.html',
  styleUrl: './set-password.component.scss'
})
export class SetPasswordComponent {
  serverError: string = '';

  resetForm = new FormGroup({
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
      Validators.pattern(/^(?=.*[a-z]).*$/),
      Validators.pattern(/^(?=.*[A-Z]).*$/),
      Validators.pattern(/^(?=.*\d).*$/),
      Validators.pattern(/^(?=.*[\W_]).*$/),
      Validators.pattern(/^\S+$/)
    ]),
    confirmPassword: new FormControl('', [Validators.required]),
  });

  constructor(
    private _authService: AuthService, 
    private router: Router, 
    private location: Location
  ){}
  onSubmit() {
    this.serverError = '';
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    const password = this.resetForm.get('password')?.value as string;
    const confirmPassword = this.resetForm.get('confirmPassword')?.value as string;

    if(password !== confirmPassword) {
      this.serverError = "Passwords don't match";
      return;
    }

    const state: any = this.location.getState();
    const email = state.email;
    const otp = state.otp;
    
    this._authService.setNewPassword(email, otp, password).subscribe({
      next: data => {
        this.router.navigate(['login']);
      },
      error: err => this.serverError = err.error?.message
    });
  }

}
