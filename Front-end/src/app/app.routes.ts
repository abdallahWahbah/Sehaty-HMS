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
import { PatientMedicalRecordsComponent } from './features/patient/patient-medical-records/patient-medical-records.component';
import { PatientAppointmentsComponent } from './features/patient/patient-appointments/patient-appointments.component';
import { PatientDetailsComponent } from './features/patient/patient-details/patient-details.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { AdminNavigationComponent } from './features/admin/admin-navigation/admin-navigation.component';
import { AdminDepartmentDetailsComponent } from './features/admin/admin-department-details/admin-department-details.component';
import { PatientEditComponent } from './features/admin/admin-patients/patient-edit/patient-edit.component';
import { DoctorEditComponent } from './features/admin/admin-doctors/doctor-edit/doctor-edit.component';
import { AdminUpdateDepartmentComponent } from './features/admin/admin-update-department/admin-update-department.component';
import { AdminAddDepartmentComponent } from './features/admin/admin-add-department/admin-add-department.component';
import { doctorGuard } from './core/guards/doctor.guard';
import { NotAllowedComponent } from './pages/not-allowed/not-allowed.component';
import { DoctorNavigationComponent } from './features/doctor/doctor-navigation/doctor-navigation.component';
import { DoctorAppointmentsComponent } from './features/doctor/doctor-appointments/doctor-appointments.component';
import { DoctorDetailsComponent } from './features/doctor/doctor-details/doctor-details.component';
import { DoctorPrescriptionsComponent } from './features/doctor/doctor-prescriptions/doctor-prescriptions.component';
import { DoctorAvailableSlotsComponent } from './features/doctor/doctor-available-slots/doctor-available-slots.component';
import { AdminAppointmentDetailsComponent } from './features/admin/admin-appointments/admin-appointment-details/admin-appointment-details.component';
import { AdminUpdateScheduleComponent } from './features/admin/admin-appointments/admin-update-schedule/admin-update-schedule.component';
import { patientGuard } from './core/guards/patient.guard';
import { PatientPaymentComponent } from './features/patient/patient-payment/patient-payment.component';

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
      { path: 'patients/:id/edit', component: PatientEditComponent },
      { path: 'doctors', component: AdminDoctorsComponent },
      { path: 'doctors/:id/edit', component: DoctorEditComponent },
      { path: 'appointments', component: AdminAppointmentsComponent },
      { path: 'appointments/:id', component: AdminAppointmentDetailsComponent },
      {
        path: 'appointments/update/:id',
        component: AdminUpdateScheduleComponent,
      },
      { path: 'medicalRecords', component: AdminMedicalRecordsComponent },
      { path: 'doctors', component: AdminDoctorsComponent },
      { path: 'medicalRecords', component: AdminMedicalRecordsComponent },
      { path: 'departments', component: AdminDepartmentsComponent },
      { path: 'departments/add', component: AdminAddDepartmentComponent },
      { path: 'departments/:id', component: AdminDepartmentDetailsComponent },
      {
        path: 'departments/update/:id',
        component: AdminUpdateDepartmentComponent,
      },
    ],
  },
  {
    path: 'doctor',
    canActivate: [doctorGuard],
    component: DoctorNavigationComponent,
    children: [
      { path: '', redirectTo: 'appointments', pathMatch: 'full' },
      { path: 'appointments', component: DoctorAppointmentsComponent },
      { path: 'details', component: DoctorDetailsComponent },
      { path: ':id/edit', component: DoctorEditComponent },
      { path: 'prescription', component: DoctorPrescriptionsComponent },
      { path: 'availableSlots', component: DoctorAvailableSlotsComponent },
    ],
  },
  {
    path: 'patient',
    canActivate: [patientGuard],
    children: [
      { path: '', redirectTo: 'medicalRecords', pathMatch: 'full' },
      {path: 'medicalRecords', component: PatientMedicalRecordsComponent},
      {path: 'appointments', component: PatientAppointmentsComponent},
      {path: 'details', component: PatientDetailsComponent},
      {path: 'payment', component: PatientPaymentComponent},
    ],
  },
  { path: 'home', component: HomeComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'not-allowed', component: NotAllowedComponent },
  { path: '**', redirectTo: '/not-found' },
];
