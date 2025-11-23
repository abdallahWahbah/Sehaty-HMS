import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, NgForm, ReactiveFormsModule, Validators } from '@angular/forms';
import { FloatLabelModule } from "primeng/floatlabel"
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { Checkbox } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../core/services/auth.service';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [
    FloatLabelModule, 
    InputTextModule, 
    FormsModule, 
    PasswordModule, 
    Checkbox, 
    ButtonModule, 
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  username: string= '';
  password: string = '';
  rememberMe: boolean = false;
  serverError: string = '';

  loginForm = new FormGroup({
    
    username: new FormControl('Doctor1', [ // Admin Doctor1
      Validators.required,
    ]),
    password: new FormControl('P@ssw0rd', [ 
      Validators.required,
      Validators.minLength(6),
      Validators.pattern(/^(?=.*[a-z]).*$/),     // at least 1 lowercase
      Validators.pattern(/^(?=.*[A-Z]).*$/),     // at least 1 uppercase
      Validators.pattern(/^(?=.*\d).*$/),        // at least 1 number
      Validators.pattern(/^(?=.*[\W_]).*$/),     // at least 1 special character
      Validators.pattern(/^\S+$/)                // no spaces allowed
    ]),
    rememberMe: new FormControl(false)
  });

  constructor(private _authService: AuthService, private router: Router){}

  onSubmit() {
    this.serverError = '';
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    const username = this.loginForm.get('username')?.value as string;
    const password = this.loginForm.get('password')?.value as string;
    this._authService.login(username, password).subscribe({
      next: (data: any) => {
        localStorage.setItem("userData", JSON.stringify(data));
        localStorage.setItem("token", data['token']);
        localStorage.setItem("refreshToken", data['refreshToken']);
        switch(data['role']){
          case "Admin":{
            this.router.navigate(['admin']);
            break;
          }
          case "Doctor":{
            this.router.navigate(['doctor']);
            break;
          }
          case "Patient":{
            this.router.navigate(['patient/home']);
            break;
          }
        }
      },
      error: err => this.serverError = err.error?.message || 'Invalid username or password'
    });
  }
}
