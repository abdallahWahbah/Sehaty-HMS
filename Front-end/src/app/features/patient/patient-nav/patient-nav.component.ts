import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-patient-nav',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './patient-nav.component.html',
  styleUrl: './patient-nav.component.scss'
})
export class PatientNavComponent {
  isMobileMenuOpen = false;
  storedData: any = localStorage.getItem('userData');

  constructor(private router:Router){}

  ngOnInit(){
    this.storedData = JSON.parse(this.storedData);
  }
  toggleMobileMenu(isLoggingout = false) {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    if(isLoggingout){
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('token')
      localStorage.removeItem('userData')
      this.router.navigate(['/login'])
    }
  }
}
