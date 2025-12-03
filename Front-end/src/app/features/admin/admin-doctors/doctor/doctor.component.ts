import { Component, Input } from '@angular/core';
import { DoctorResponseModel } from '../../../../core/models/doctor-response-model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-doctor',
  imports: [CommonModule],
  templateUrl: './doctor.component.html',
  styleUrl: './doctor.component.scss',
})
export class DoctorComponent {
  @Input() doctor!: DoctorResponseModel;
}
