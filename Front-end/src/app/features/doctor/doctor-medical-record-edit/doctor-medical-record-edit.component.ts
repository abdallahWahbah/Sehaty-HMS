import { Component } from '@angular/core';
import { MedicalRecord } from '../../../core/models/medicalrecord-response.model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { MedicalRecordType } from '../../../core/enums/medical-record-enum';
import { MedicalRecordService } from '../../../core/services/medical-record.service';
import { LoadingSpinnerComponent } from "../../../layout/loading-spinner/loading-spinner.component";

@Component({
  selector: 'app-doctor-medical-record-edit',
  imports: [
    ReactiveFormsModule,
    FloatLabelModule,
    InputTextModule,
    DropdownModule,
    ButtonModule,
    RouterModule,
    LoadingSpinnerComponent
],  templateUrl: './doctor-medical-record-edit.component.html',
  styleUrl: './doctor-medical-record-edit.component.scss'
})
export class DoctorMedicalRecordEditComponent {
  medicalRecord!: MedicalRecord;
  recordForm!: FormGroup;
  serverError: string = '';
  isLoading: boolean = false;

  recordTypeOptions = [
    { label: 'Diagnosis', value: MedicalRecordType.Diagnosis },
    { label: 'Lab Result', value: MedicalRecordType.LabResult },
    { label: 'Imaging', value: MedicalRecordType.Imaging },
    { label: 'Follow Up', value: MedicalRecordType.FollowUp },
    { label: 'Procedure', value: MedicalRecordType.Procedure },
  ];

  constructor(
    private fb: FormBuilder,
    private router: Router, 
    private _medicalRecordService: MedicalRecordService,
    private route: ActivatedRoute,
  ) {
    console.log("LOADED MedicalRecordService VERSION X");
  }

  ngOnInit(): void {
    this.medicalRecord = history.state.medicalRecord;
    // this.route.params.subscribe((params) => {
    //   // console.log("11111111111111", params["id"]);
    // });

    this.recordForm = this.fb.group({
      symptoms: [this.medicalRecord.symptoms, Validators.required],
      diagnosis: [this.medicalRecord.diagnosis, Validators.required],
      treatmentPlan: [this.medicalRecord.treatmentPlan, Validators.required],
      bpSystolic: [this.medicalRecord.bpSystolic, Validators.required],
      bpDiastolic: [this.medicalRecord.bpDiastolic, Validators.required],
      temperature: [this.medicalRecord.temperature, Validators.required],
      heartRate: [this.medicalRecord.heartRate, Validators.required],
      weight: [this.medicalRecord.weight, Validators.required],
      recordType: [this.medicalRecord.recordType, Validators.required],
      notes: [this.medicalRecord.notes, Validators.required]
    });
  }
  onSubmit(): void {
    this.serverError = '';
    this.isLoading = true;
    if (this.recordForm.invalid) return;
    setTimeout(()=>{
      const updatedMedicalRecord = this.recordForm.value;
  
      this._medicalRecordService.updateByDoctor(this.medicalRecord.id, updatedMedicalRecord)
        .subscribe({
          next: () => {
            this.router.navigate(['/doctor/appointments'])
          },
          error: err => {
            this.isLoading = false;
            this.serverError = err.error?.message || 'An error occurred';
          }
      });
    }, 500)

  }
}
