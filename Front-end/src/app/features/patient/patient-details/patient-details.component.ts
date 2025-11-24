import { Component, OnInit } from '@angular/core';
import { PatientResponseModel } from '../../../core/models/patient-response-model';
import { PatientsService } from '../../../core/services/patients.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patient-details',
  imports: [CommonModule],
  templateUrl: './patient-details.component.html',
  styleUrl: './patient-details.component.scss',
})
export class PatientDetailsComponent implements OnInit {
  patient!: PatientResponseModel;

  constructor(
    private _patientService: PatientsService,
    private router: Router
  ) {}

  ngOnInit() {
    let storedUser: any = JSON.parse(localStorage.getItem('userData')!);

    this._patientService.getAll().subscribe({
      next: (data) => {
        this.patient = data.find((p) => p.userId === storedUser.userId)!;
      },
    });
  }
  goToEdit() {
    this.router.navigate(['/patient/edit', this.patient.id]);
  }
}
