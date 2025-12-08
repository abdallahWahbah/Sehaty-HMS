import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { SignupComponent } from './features/signup/signup.component';
import { ForgetComponent } from './features/forget-password/forget/forget.component';
import { VerifyOtpComponent } from './features/forget-password/verify-otp/verify-otp.component';
import { SetPasswordComponent } from './features/forget-password/set-password/set-password.component';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard.component';
import { AdminPatientsComponent } from './features/admin/admin-patients/admin-patients.component';
import { AdminDoctorsComponent } from './features/admin/admin-doctors/admin-doctors.component';
import { AdminAppointmentsComponent } from './features/admin/admin-appointments/admin-appointments.component';
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
import { DoctorAppointmentsComponent } from './features/doctor/doctor-appointments/doctor-appointments.component';
import { DoctorDetailsComponent } from './features/doctor/doctor-details/doctor-details.component';
import { DoctorPrescriptionsComponent } from './features/doctor/doctor-prescriptions/doctor-prescriptions.component';
import { DoctorAvailableSlotsComponent } from './features/doctor/doctor-available-slots/doctor-available-slots.component';
import { AdminAppointmentDetailsComponent } from './features/admin/admin-appointments/admin-appointment-details/admin-appointment-details.component';
import { AdminUpdateScheduleComponent } from './features/admin/admin-appointments/admin-update-schedule/admin-update-schedule.component';
import { patientGuard } from './core/guards/patient.guard';
import { PatientPaymentComponent } from './features/patient/patient-payment/patient-payment.component';
import { PatientNavigationComponent } from './features/patient/patient-navigation/patient-navigation.component';
import { PatientUpdateComponent } from './features/patient/patient-update/patient-update.component';
import { DoctorEditPrescriptionsComponent } from './features/doctor/doctor-prescriptions/doctor-edit-prescriptions.component/doctor-edit-prescriptions.component';
import { DoctorAddPrescriptionComponent } from './features/doctor/doctor-prescriptions/doctor-add-prescription/doctor-add-prescription.component';
import { AddDoctorComponent } from './features/admin/admin-doctors/add-doctor/add-doctor.component';
import { PatientPrescriptionComponent } from './features/patient/patient-prescription/patient-prescription.component';
import { PatientFeedbackComponent } from './features/patient/patient-feedback/patient-feedback.component';
import { PatientAddFeedbackComponent } from './features/patient/patient-feedback/patient-add-feedback/patient-add-feedback.component';
import { AdminUsersComponent } from './features/admin/admin-users/admin-users.component';
import { ViewDepartmentsComponent } from './features/patient/patient-appointments/view-departments/view-departments.component';
import { ViewDoctorsComponent } from './features/patient/patient-appointments/view-departments/view-doctors/view-doctors.component';
import { DoctorAvailabilityComponent } from './features/patient/patient-appointments/view-departments/view-doctors/doctor-avilabilty/doctor-avilabilty.component';
import { AvailableSlotsComponent } from './features/patient/patient-appointments/view-departments/view-doctors/doctor-avilabilty/avilable-slots/avilable-slots.component';
import { receiptionistGuard } from './core/guards/receiptionist.guard';
import { ReceptionAppointmentsComponent } from './features/reception/reception-appointments/reception-appointments.component';
import { patientReceptionGuard } from './core/guards/patient-reception.guard';
import { DoctorMedicalrecordandprescriptionComponent } from './features/doctor/doctor-medicalrecordandprescription/doctor-medicalrecordandprescription.component';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { landingPageGuard } from './core/guards/landingPage.guard';
import { PatientHomeComponent } from './pages/patient-home/patient-home.component';
import { AllBillingsComponent } from './features/admin/all-billings/all-billings.component';
import { adminDoctorGuard } from './core/guards/admin-doctor.guard';
import { adminReceptionGuard } from './core/guards/admin-reception.guard';
import { PatientBillingComponent } from './features/patient/patient-billing/patient-billing.component';
import { DoctorMedicalRecordEditComponent } from './features/doctor/doctor-medical-record-edit/doctor-medical-record-edit.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'forgetPassword', component: ForgetComponent },
  { path: 'verifyOtp', component: VerifyOtpComponent },
  { path: 'setPassword', component: SetPasswordComponent },
  {
    path: 'admin',
    component: AdminNavigationComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboardComponent, canActivate: [adminGuard] },
      { path: 'patients', component: AdminPatientsComponent, canActivate: [adminGuard] },
      { path: 'patients/:id/edit', component: PatientEditComponent, canActivate: [adminGuard] },
      { path: 'doctors', component: AdminDoctorsComponent, canActivate: [adminGuard] },
      { path: 'doctors/add', component: AddDoctorComponent, canActivate: [adminGuard] },
      { path: 'doctors/:id/edit', component: DoctorEditComponent, canActivate: [adminGuard] },
      { path: 'appointments', component: AdminAppointmentsComponent, canActivate: [adminGuard] },
      { path: 'appointments/:id', component: AdminAppointmentDetailsComponent, canActivate: [adminGuard] },
      { path: 'appointments/update/:id', component: AdminUpdateScheduleComponent, canActivate: [adminGuard] },
      { path: 'doctors', component: AdminDoctorsComponent, canActivate: [adminGuard] },
      // { path: 'medicalRecords', component: AdminMedicalRecordsComponent, canActivate: [adminGuard] },
      { path: 'departments', component: AdminDepartmentsComponent, canActivate: [adminGuard] },
      { path: 'departments/add', component: AdminAddDepartmentComponent, canActivate: [adminGuard] },
      { path: 'departments/:id', component: AdminDepartmentDetailsComponent, canActivate: [adminGuard] },
      { path: 'departments/update/:id', component: AdminUpdateDepartmentComponent, canActivate: [adminGuard] },
      { path: 'allBillings', component: AllBillingsComponent, canActivate: [adminReceptionGuard] },
      { path: 'users', component: AdminUsersComponent, canActivate: [adminGuard] },
    ],
  },
  {
    path: 'doctor',
    component: AdminNavigationComponent,
    children: [
      { path: '', redirectTo: 'details', pathMatch: 'full' },
      { path: 'details', component: DoctorDetailsComponent, canActivate: [doctorGuard] },
      { path: 'appointments', component: DoctorAppointmentsComponent, canActivate: [doctorGuard] },
      { path: ':id/edit', component: DoctorEditComponent, canActivate: [doctorGuard] },
      { path: 'prescription', component: DoctorPrescriptionsComponent, canActivate: [doctorGuard] },
      { path: 'prescriptions/edit/:id', component: DoctorEditPrescriptionsComponent, canActivate: [doctorGuard] },
      { path: 'prescriptions/add', component: DoctorAddPrescriptionComponent, canActivate: [doctorGuard] },
      { path: 'availableSlots', component: DoctorAvailableSlotsComponent, canActivate: [doctorGuard] },
      { path: 'patient/details/:id', component: DoctorMedicalrecordandprescriptionComponent, canActivate: [adminDoctorGuard] },
      { path: 'patient/medicalRecord/edit/:id', component: DoctorMedicalRecordEditComponent, canActivate: [doctorGuard] },
    ],
  },
  {
    path: 'patient',
    component: PatientNavigationComponent,
    children: [
      // protected by patientGuard
      { path: '', redirectTo: 'medicalRecords', pathMatch: 'full' },
      {
        path: 'medicalRecords',
        canActivate: [patientGuard],
        component: PatientMedicalRecordsComponent,
      },
      {
        path: 'appointments',
        canActivate: [patientGuard],
        component: PatientAppointmentsComponent,
      },
      {
        path: 'details',
        canActivate: [patientGuard],
        component: PatientDetailsComponent,
      },
      {
        path: 'edit/:id',
        canActivate: [patientGuard],
        component: PatientUpdateComponent,
      },
      { path: 'payment/:id', component: PatientPaymentComponent },
      { path: 'prescription', component: PatientPrescriptionComponent },
      { path: 'feedback', component: PatientFeedbackComponent },
      { path: 'feedback/add/:id', component: PatientAddFeedbackComponent },

      // protected by patientReceptionGuard
      {
        path: 'appointments/add',
        canActivate: [patientReceptionGuard],
        component: ViewDepartmentsComponent,
      },
      {
        path: 'appointments/doctors/:departmentId',
        canActivate: [patientReceptionGuard],
        component: ViewDoctorsComponent,
      },
      {
        path: 'appointments/available-days/:doctorId',
        canActivate: [patientReceptionGuard],
        component: DoctorAvailabilityComponent,
      },
      {
        path: 'appointments/available-slots/:doctorId/:date',
        canActivate: [patientReceptionGuard],
        component: AvailableSlotsComponent,
      },
    ],
  },
  {
    path: 'reception',
    canActivate: [receiptionistGuard],
    component: AdminNavigationComponent,
    children: [
      { path: '', redirectTo: 'appointments', pathMatch: 'full' },
      { path: 'appointments', component: ReceptionAppointmentsComponent },
      { path: 'new/appointment', component: ViewDepartmentsComponent },
      {
        path: 'new/appointments/doctors/:departmentId',
        component: ViewDoctorsComponent,
      },
      {
        path: 'new/appointments/available-days/:doctorId',
        component: DoctorAvailabilityComponent,
      },
      {
        path: 'new/appointments/available-slots/:doctorId/:date',
        component: AvailableSlotsComponent,
      },
    ],
  },
  {
    path: 'home',
    component: PatientHomeComponent,
    // مهم: شيل canActivate من هنا علشان مايتطبّقش على كل الأطفال
    // وسيب الجاردز على الأطفال نفسهم
    children: [
      // 🔹 Landing page جوّه /home
      {
        path: '',
        component: LandingPageComponent,
        canActivate: [landingPageGuard], // لو عايز تمنع ناس معينة من اللاندنج
      },

      // 🔹 Routes المريض (كانت تحت /patient وبقت تحت /home)
      {
        path: 'medicalRecords',
        canActivate: [patientGuard],
        component: PatientMedicalRecordsComponent,
      },
      {
        path: 'appointments',
        canActivate: [patientGuard],
        component: PatientAppointmentsComponent,
      },
      {
        path: 'billings',
        canActivate: [patientGuard],
        component: PatientBillingComponent,
      },
      {
        path: 'details',
        canActivate: [patientGuard],
        component: PatientDetailsComponent,
      },
      {
        path: 'edit/:id',
        canActivate: [patientGuard],
        component: PatientUpdateComponent,
      },
      { path: 'payment/:id', component: PatientPaymentComponent, canActivate: [patientGuard] },
      { path: 'prescription', component: PatientPrescriptionComponent, canActivate: [patientGuard] },
      { path: 'feedback', component: PatientFeedbackComponent, canActivate: [patientGuard] },
      { path: 'feedback/add/:id', component: PatientAddFeedbackComponent, canActivate: [patientGuard] },

      // 🔹 اللي كانوا patientReceptionGuard
      {
        path: 'appointments/add',
        canActivate: [patientReceptionGuard],
        component: ViewDepartmentsComponent,
      },
      {
        path: 'appointments/doctors/:departmentId',
        canActivate: [patientReceptionGuard],
        component: ViewDoctorsComponent,
      },
      {
        path: 'appointments/available-days/:doctorId',
        canActivate: [patientReceptionGuard],
        component: DoctorAvailabilityComponent,
      },
      {
        path: 'appointments/available-slots/:doctorId/:date',
        canActivate: [patientReceptionGuard],
        component: AvailableSlotsComponent,
      },
    ],
  },

  { path: 'patient-home', component: PatientHomeComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'not-allowed', component: NotAllowedComponent },
  { path: '**', redirectTo: '/not-found' },
];
