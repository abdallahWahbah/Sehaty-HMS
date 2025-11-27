import { Component } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-reception-navigation',
  imports: [RouterModule],
  templateUrl: './reception-navigation.component.html',
  styleUrl: './reception-navigation.component.scss'
})
export class ReceptionNavigationComponent {

  constructor(private router: Router){}
  
  logout(){
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('token')
    localStorage.removeItem('userData')
    this.router.navigate(['/login'])
  }
}
