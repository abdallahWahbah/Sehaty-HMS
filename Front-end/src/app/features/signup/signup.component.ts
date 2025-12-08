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
  step = 1;
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
        firstName: ['', [Validators.required]],
        lastName: ['', [Validators.required]],
        email: ['', [Validators.required, Validators.email]],
        phoneNumber: ['', [Validators.required, Validators.pattern(/^(\+2)?(010|011|012)\d{8}$/)]],
        userName: ['', [Validators.required]],
        password: ['', [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(/^(?=.*[a-z]).*$/),
          Validators.pattern(/^(?=.*[A-Z]).*$/),
          Validators.pattern(/^(?=.*\d).*$/),
          Validators.pattern(/^(?=.*[\W_]).*$/),
          Validators.pattern(/^\S+$/)
        ]],
        confirmPassword: ['', Validators.required],
        agreeTerms: [false, Validators.requiredTrue]
      }),

      // FORM 2 — PATIENT FORM
      patient: this.fb.group({
        dateOfBirth: ['', Validators.required],
        gender: [this.genderOptions[0].value, Validators.required],
        nationalId: ['', Validators.required],
        bloodType: ['', Validators.required],
        allergies: ['', Validators.required],
        chrinicConditions: ['', Validators.required],
        address: ['', Validators.required],
        emergencyContactName: ['', Validators.required],
        emergencyContactPhone: ['', [Validators.required, Validators.pattern(/^(\+2)?(010|011|012)\d{8}$/)]],
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
      this.loading = false;
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
      emergencyContactPhone: "+2" + patientData.emergencyContactPhone.replace("+2", ''),
    };
    console.log(newUser.phoneNumber);
    setTimeout(()=>{
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
    }, 500)
  }
  get accountForm(): FormGroup {
    return this.signupForm.get('account') as FormGroup;
  }
  get patientForm(): FormGroup {
    return this.signupForm.get('patient') as FormGroup;
  }
}
