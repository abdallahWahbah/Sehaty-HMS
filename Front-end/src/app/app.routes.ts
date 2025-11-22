import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { SignupComponent } from './features/signup/signup.component';
import { ForgetComponent } from './features/forget-password/forget/forget.component';
import { VerifyOtpComponent } from './features/forget-password/verify-otp/verify-otp.component';
import { SetPasswordComponent } from './features/forget-password/set-password/set-password.component';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard.component';
import { AdminPatientsComponent } from './features/admin/admin-patients/admin-patients.component';
import { AdminDoctorsComponent } from './features/admin/admin-doctors/admin-doctors.component';
import { AdminAppointmentsComponent } from './features/admin/admin-appointments/admin-appointments.component';
import { AdminMedicalRecordsComponent } from './features/admin/admin-medical-records/admin-medical-records.component';
import { AdminDepartmentsComponent } from './features/admin/admin-departments/admin-departments.component';
import { adminGuard } from './core/guards/admin.guard';
import { PatientHomeComponent } from './features/patient/patient-home/patient-home.component';
import { PatientMedicalRecordsComponent } from './features/patient/patient-medical-records/patient-medical-records.component';
import { PatientAppointmentsComponent } from './features/patient/patient-appointments/patient-appointments.component';
import { PatientDetailsComponent } from './features/patient/patient-details/patient-details.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { AdminNavigationComponent } from './features/admin/admin-navigation/admin-navigation.component';
import { AdminDepartmentDetailsComponent } from './features/admin/admin-department-details/admin-department-details.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'forgetPassword', component: ForgetComponent },
  { path: 'verifyOtp', component: VerifyOtpComponent },
  { path: 'setPassword', component: SetPasswordComponent },
  {
    path: 'admin',
    canActivate: [adminGuard],
    component: AdminNavigationComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboardComponent },
      { path: 'patients', component: AdminPatientsComponent },
      { path: 'doctors', component: AdminDoctorsComponent },
      { path: 'appointments', component: AdminAppointmentsComponent },
      { path: 'medicalRecords', component: AdminMedicalRecordsComponent },
      { path: 'departments', component: AdminDepartmentsComponent },
      { path: 'departments/:id', component: AdminDepartmentDetailsComponent },
    ],
  },
  {
    path: 'patient',
    // canActivate: [],
    children: [
      { path: '', component: PatientHomeComponent },
      { path: 'home', component: PatientHomeComponent },
      // {path: 'medicalRecords', component: PatientMedicalRecordsComponent},
      // {path: 'appointments', component: PatientAppointmentsComponent},
      // {path: 'details', component: PatientDetailsComponent},
    ],
  },
  { path: 'home', component: HomeComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', redirectTo: '/not-found' },
];
