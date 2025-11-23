import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-navigation',
  imports: [RouterModule],
  templateUrl: './admin-navigation.component.html',
  styleUrl: './admin-navigation.component.scss'
})
export class AdminNavigationComponent {

  constructor(private router:Router){}

  logout(){
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('token')
    localStorage.removeItem('userData')
    this.router.navigate(['/login'])
  }
}
