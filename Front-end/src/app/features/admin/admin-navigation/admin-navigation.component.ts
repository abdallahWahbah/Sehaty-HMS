import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-navigation',
  imports: [RouterModule],
  templateUrl: './admin-navigation.component.html',
  styleUrl: './admin-navigation.component.scss'
})
export class AdminNavigationComponent {

  storedUser: any;
  constructor(private router:Router){}

  ngOnInit(){
    this.storedUser = localStorage.getItem('userData');
    this.storedUser = JSON.parse(this.storedUser);
  }
  logout(){
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('token')
    localStorage.removeItem('userData')
    this.router.navigate(['/login'])
  }
}
