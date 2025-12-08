import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-patient-navigation',
  imports: [RouterModule, CommonModule],
  templateUrl: './patient-navigation.component.html',
  styleUrl: './patient-navigation.component.scss'
})
export class PatientNavigationComponent {

  userData: any = localStorage.getItem('userData');
  constructor(private router:Router){
    this.userData = JSON.parse(this.userData);
  }

  logout(){
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('token')
    localStorage.removeItem('userData')
    this.router.navigate(['/login'])
  }
}
