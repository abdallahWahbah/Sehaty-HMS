import { Location } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-allowed',
  imports: [],
  templateUrl: './not-allowed.component.html',
  styleUrl: './not-allowed.component.scss'
})
export class NotAllowedComponent {
  
  constructor(private router: Router, private location: Location) {}

  goHome() {
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("token");
    localStorage.removeItem("userData");
    this.router.navigate(['/login']);
  }
  
  goBack() {
    this.location.back();
  }
}
