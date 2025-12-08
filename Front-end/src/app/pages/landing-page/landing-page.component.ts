import { Component } from '@angular/core';
import { PatientNavComponent } from '../../features/patient/patient-nav/patient-nav.component';
import { PatientFooterComponent } from '../../features/patient/patient-footer/patient-footer.component';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { PatientPrescriptionComponent } from '../../features/patient/patient-prescription/patient-prescription.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  imports: [PatientNavComponent, PatientFooterComponent, RouterLink, RouterModule],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss',
})
export class LandingPageComponent {
  storedData: any = localStorage.getItem('userData');

  constructor(private router: Router) {}

  ngOnInit() {
    this.storedData = JSON.parse(this.storedData);
  }

  navigateToAppointments() {
    this.router.navigate(['/home/appointments/add']);
  }

    navigateToFeature() {
    this.router.navigate([]);
  }

}
