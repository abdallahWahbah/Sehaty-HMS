import { Component, Input } from '@angular/core';
import { DoctorResponseModel } from '../../../../core/models/doctor-response-model';

@Component({
  selector: 'app-doctor',
  imports: [],
  templateUrl: './doctor.component.html',
  styleUrl: './doctor.component.scss'
})
export class DoctorComponent {
  @Input() doctor!: DoctorResponseModel;
}
