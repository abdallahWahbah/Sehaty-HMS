import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { SignupComponent } from './features/signup/signup.component';
import { ForgetComponent } from './features/forget-password/forget/forget.component';
import { VerifyOtpComponent } from './features/forget-password/verify-otp/verify-otp.component';
import { SetPasswordComponent } from './features/forget-password/set-password/set-password.component';

export const routes: Routes = [
    {path: '', redirectTo: '/signup', pathMatch: 'full'},
    {path: 'login', component: LoginComponent},
    {path: 'signup', component: SignupComponent},
    {path: 'forgetPassword', component: ForgetComponent},
    {path: 'verifyOtp', component: VerifyOtpComponent},
    {path: 'setPassword', component: SetPasswordComponent},
    {path: 'home', component: HomeComponent},
];
