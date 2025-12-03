import { Component } from '@angular/core';
import { PatientNavComponent } from '../../features/patient/patient-nav/patient-nav.component';
import { PatientFooterComponent } from '../../features/patient/patient-footer/patient-footer.component';
import { Router } from '@angular/router';
import { PatientPrescriptionComponent } from '../../features/patient/patient-prescription/patient-prescription.component';
import { RouterOutlet } from '@angular/router';
import { LandingPageComponent } from '../landing-page/landing-page.component';

@Component({
  selector: 'app-patient-home',
  imports: [
    PatientNavComponent,
    PatientFooterComponent,
    PatientPrescriptionComponent,
    LandingPageComponent,
    RouterOutlet,
  ],
  templateUrl: './patient-home.component.html',
  styleUrl: './patient-home.component.scss',
})
export class PatientHomeComponent {}
