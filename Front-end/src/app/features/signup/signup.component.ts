import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators, ReactiveFormsModule, FormsModule, FormBuilder } from '@angular/forms';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { Checkbox } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { PatientsService } from '../../core/services/patients.service';
import { PateintStatusEnum } from '../../core/enums/patient-status-enum';
import { DropdownModule } from 'primeng/dropdown';
import { LoadingSpinnerComponent } from "../../layout/loading-spinner/loading-spinner.component";

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
    RouterModule,
    DropdownModule,
    LoadingSpinnerComponent
],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent {
  serverError: string = '';
  loading: boolean = false;
  step = 2;
  signupForm!: FormGroup;
  genderOptions = [
    { label: 'Male', value: 'Male' },
    { label: 'Female', value: 'Female' },
  ];

  constructor(
    private fb: FormBuilder,
    private _authService: AuthService, 
    private router: Router,
    private _patientServie: PatientsService
  ) {}

  ngOnInit(){
    this.signupForm = this.fb.group({
      // FORM 1 — ACCOUNT FORM
      account: this.fb.group({
        firstName: ['ebrahim', [Validators.required]],
        lastName: ['front end', [Validators.required]],
        email: ['a@a.a', [Validators.required, Validators.email]],
        phoneNumber: ['+201092717902', [Validators.required]],
        userName: ['hankosh', [Validators.required]],
        password: ['P@ssw0rd', [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(/^(?=.*[a-z]).*$/),
          Validators.pattern(/^(?=.*[A-Z]).*$/),
          Validators.pattern(/^(?=.*\d).*$/),
          Validators.pattern(/^(?=.*[\W_]).*$/),
          Validators.pattern(/^\S+$/)
        ]],
        confirmPassword: ['P@ssw0rd', Validators.required],
        agreeTerms: [true, Validators.requiredTrue]
      }),

      // FORM 2 — PATIENT FORM
      patient: this.fb.group({
        dateOfBirth: ['2025-12-01', Validators.required],
        gender: [this.genderOptions[0].value, Validators.required],
        nationalId: ['25687419354716', Validators.required],
        bloodType: ['A+', Validators.required],
        allergies: ['None', Validators.required],
        chrinicConditions: ['None', Validators.required],
        address: ['Mit Ghamr', Validators.required],
        emergencyContactName: [''],
        emergencyContactPhone: [''],
      })
    });
  }

  nextStep() {
    const account = this.signupForm.get('account');
    if (account?.invalid) {
      account.markAllAsTouched();
      return;
    }
    this.step = 2;
    this.serverError = '';
  }

  prevStep() {
    this.step = 1;
    this.serverError = '';
  }
  
  onSubmit() {
    this.loading = true;
    const patient = this.signupForm.get('patient');
    if (patient?.invalid) {
      patient.markAllAsTouched();
      return;
    }

    this.serverError = '';

    const accountData = this.signupForm.get('account')!.value;
    const patientData = this.signupForm.get('patient')!.value;

    const newUser = {
      userName: accountData.userName,
      email: accountData.email,
      phoneNumber: "+2" + accountData.phoneNumber.replace("+2", ''),
      firstName: accountData.firstName,
      lastName: accountData.lastName,
      password: accountData.password,
      confirmPassword: accountData.confirmPassword,
      languagePreference: 'Arabic',
      dateOfBirth: new Date(patientData.dateOfBirth).toISOString(),
      gender: patientData.gender,
      nationalId: patientData.nationalId,
      bloodType: patientData.bloodType,
      allergies: patientData.allergies,
      chrinicConditions: patientData.chrinicConditions,
      address: patientData.address,
      emergencyContactName: patientData.emergencyContactName,
      emergencyContactPhone: patientData.emergencyContactPhone,
    };
    console.log(newUser.phoneNumber);
    this._authService.register(newUser).subscribe({
      next: data => {
        this.router.navigate(['login']);
        this.loading = false;
      },
      error: err => {
        let concatenatedError = '';
        if(err.error.errors)
          for(let i = 0; i < err.error.errors.length; i++) concatenatedError += err.error.errors[i]
        this.serverError = err.error?.errors?.length > 0 ? concatenatedError : err.error?.message
        this.loading = false;
      }
    })
  }
  get accountForm(): FormGroup {
    return this.signupForm.get('account') as FormGroup;
  }
  get patientForm(): FormGroup {
    return this.signupForm.get('patient') as FormGroup;
  }
}
