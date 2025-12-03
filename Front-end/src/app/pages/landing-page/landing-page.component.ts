import { Component } from '@angular/core';
import { PatientNavComponent } from '../../features/patient/patient-nav/patient-nav.component';
import { PatientFooterComponent } from '../../features/patient/patient-footer/patient-footer.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  imports: [PatientNavComponent, PatientFooterComponent],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss'
})
export class LandingPageComponent {
  storedData: any = localStorage.getItem('userData');

  constructor(private router:Router){}

  ngOnInit(){
    this.storedData = JSON.parse(this.storedData);
    console.log(this.storedData);
  }

  navigateToAppointments(){
    this.router.navigate(["/patient/appointments"]);
  }
}
