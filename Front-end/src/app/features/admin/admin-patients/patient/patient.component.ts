import { Component, Input } from '@angular/core';
import { PatientResponseModel } from '../../../../core/models/patient-response-model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-patient',
  imports: [CommonModule],
  templateUrl: './patient.component.html',
  styleUrl: './patient.component.scss'
})
export class PatientComponent {
  @Input() patient!: PatientResponseModel;
}
