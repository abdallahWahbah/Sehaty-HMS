import { Component } from '@angular/core';
import { PatientNavComponent } from '../../features/patient/patient-nav/patient-nav.component';
import { PatientFooterComponent } from '../../features/patient/patient-footer/patient-footer.component';

@Component({
  selector: 'app-landing-page',
  imports: [PatientNavComponent, PatientFooterComponent],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss'
})
export class LandingPageComponent {

}
