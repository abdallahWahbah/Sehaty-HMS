import { Component } from '@angular/core';
import { PatientNavComponent } from '../patient-nav/patient-nav.component';
import { PatientFooterComponent } from '../patient-footer/patient-footer.component';

@Component({
  selector: 'app-patient-home',
  imports: [PatientNavComponent, PatientFooterComponent],
  templateUrl: './patient-home.component.html',
  styleUrl: './patient-home.component.scss'
})
export class PatientHomeComponent {

}
