import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-patient-nav',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './patient-nav.component.html',
  styleUrl: './patient-nav.component.scss'
})
export class PatientNavComponent {
  isMobileMenuOpen = false;

  toggleMobileMenu() {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }
}
