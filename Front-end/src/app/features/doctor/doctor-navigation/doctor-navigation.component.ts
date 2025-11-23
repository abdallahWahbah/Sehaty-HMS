import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-doctor-navigation',
  imports: [RouterModule],
  templateUrl: './doctor-navigation.component.html',
  styleUrl: './doctor-navigation.component.scss'
})
export class DoctorNavigationComponent {

  constructor(private router:Router){}

  logout(){
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('token')
    localStorage.removeItem('userData')
    this.router.navigate(['/login'])
  }
}
