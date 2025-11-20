import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { SignupComponent } from './features/signup/signup.component';

export const routes: Routes = [
    {path: '', redirectTo: '/signup', pathMatch: 'full'},
    {path: 'login', component: LoginComponent},
    {path: 'signup', component: SignupComponent},
    {path: 'home', component: HomeComponent},
];
