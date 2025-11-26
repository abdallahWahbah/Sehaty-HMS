import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { Checkbox } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-signup',
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
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent {
  serverError: string = '';

  signupForm = new FormGroup({
    firstName: new FormControl('ebrahim', [Validators.required]),
    lastName: new FormControl('fron ent', [Validators.required]),
    email: new FormControl('a@a.a', [Validators.required, Validators.email]),
    phoneNumber: new FormControl('+201092717902', [Validators.required]),
    userName: new FormControl('hankosh', [Validators.required]),
    password: new FormControl('asdASD!@#123', [
      Validators.required,
      Validators.minLength(6),
      Validators.pattern(/^(?=.*[a-z]).*$/),
      Validators.pattern(/^(?=.*[A-Z]).*$/),
      Validators.pattern(/^(?=.*\d).*$/),
      Validators.pattern(/^(?=.*[\W_]).*$/),
      Validators.pattern(/^\S+$/)
    ]),
    confirmPassword: new FormControl('asdASD!@#123', [Validators.required]),
    agreeTerms: new FormControl(true, [Validators.requiredTrue])
  });

  constructor(private _authService: AuthService, private router: Router) {}

  onSubmit() {
    this.serverError = '';
    if (this.signupForm.invalid) {
      this.signupForm.markAllAsTouched();
      return;
    }

    const userName = this.signupForm.get('userName')?.value as string;
    const email = this.signupForm.get('email')?.value as string;
    const phoneNumber = "+2" + (this.signupForm.get('phoneNumber')?.value as string).replace("+2", '');
    const firstName = this.signupForm.get('firstName')?.value as string;
    const lastName = this.signupForm.get('lastName')?.value as string;
    const password = this.signupForm.get('password')?.value as string;
    const confirmPassword = this.signupForm.get('confirmPassword')?.value as string;

    const newUser = {userName, email, phoneNumber, firstName, lastName, password, confirmPassword, languagePreference: 'Arabic'}

    this._authService.register(newUser).subscribe({
      next: data => {
        this.router.navigate(['login']);
      },
      error: err => this.serverError = err.error?.message || 'Registration failed'
    });
  }
}
