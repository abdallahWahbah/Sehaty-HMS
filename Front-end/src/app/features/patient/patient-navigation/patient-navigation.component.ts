import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-patient-navigation',
  imports: [RouterModule],
  templateUrl: './patient-navigation.component.html',
  styleUrl: './patient-navigation.component.scss'
})
export class PatientNavigationComponent {
  constructor(private router:Router){}

  logout(){
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('token')
    localStorage.removeItem('userData')
    this.router.navigate(['/login'])
  }
}
