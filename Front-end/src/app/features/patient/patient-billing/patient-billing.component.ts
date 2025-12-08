import { Component } from '@angular/core';
import { PatientResponseModel } from '../../../core/models/patient-response-model';
import { PatientsService } from '../../../core/services/patients.service';
import { LoadingSpinnerComponent } from "../../../layout/loading-spinner/loading-spinner.component";
import { BillingService } from '../../../core/services/billing.service';
import { BillingResponseModel } from '../../../core/models/billing-response-model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-patient-billing',
  imports: [LoadingSpinnerComponent, CommonModule],
  templateUrl: './patient-billing.component.html',
  styleUrl: './patient-billing.component.scss'
})
export class PatientBillingComponent {

  patient!: PatientResponseModel;
  loading: boolean = true;
  serverError: string = '';
  billings: BillingResponseModel[] = [];


  constructor(private _patientService: PatientsService, private _billingService: BillingService){}

  ngOnInit(){
    let storedUser: any = localStorage.getItem('userData');
    storedUser = JSON.parse(storedUser);

    this._patientService?.getAll().subscribe({
      next: data => {
        const tempPatient = data.filter(patient => patient.userId === storedUser.userId)[0];
        this.patient = tempPatient;

        this._billingService.getBillingForPatient(tempPatient.id).subscribe({
          next: patientBillings => {
            this.billings = patientBillings;
          },
          error: err => {
            this.serverError = err.error.message;
            // this.billings = [
            //   {
            //     "patientName": "Youssef Hany",
            //     "doctorName": "Ibrahim Youssef",
            //     "appointmentDateTime": "2025-12-06T09:00:00",
            //     "billDate": "2025-12-05T19:38:09.113",
            //     "totalAmount": 250,
            //     "status": "Paid",
            //     "paymentMethod": "CreditCard",
            //     "paidAmount": 250,
            //     "paidAt": "2025-12-05T19:39:53.173"
            //   },
            //   {
            //     "patientName": "Youssef Hany",
            //     "doctorName": "Ibrahim Youssef",
            //     "appointmentDateTime": "2025-12-06T09:30:00",
            //     "billDate": "2025-12-05T19:41:30.093",
            //     "totalAmount": 250,
            //     "status": "Paid",
            //     "paymentMethod": "CreditCard",
            //     "paidAmount": 250,
            //     "paidAt": "2025-12-05T19:42:09.45"
            //   },
            //   {
            //     "patientName": "Youssef Hany",
            //     "doctorName": "Ibrahim Youssef",
            //     "appointmentDateTime": "2025-12-06T10:00:00",
            //     "billDate": "2025-12-05T19:43:18.587",
            //     "totalAmount": 250,
            //     "status": "Paid",
            //     "paymentMethod": "CreditCard",
            //     "paidAmount": 250,
            //     "paidAt": "2025-12-05T19:43:50.58"
            //   },
            //   {
            //     "patientName": "Youssef Hany",
            //     "doctorName": "Ibrahim Youssef",
            //     "appointmentDateTime": "2025-12-06T10:30:00",
            //     "billDate": "2025-12-05T19:50:33.363",
            //     "totalAmount": 250,
            //     "status": "Paid",
            //     "paymentMethod": "CreditCard",
            //     "paidAmount": 250,
            //     "paidAt": "2025-12-05T19:51:09.44"
            //   },
            //   {
            //     "patientName": "Youssef Hany",
            //     "doctorName": "Ibrahim Youssef",
            //     "appointmentDateTime": "2025-12-06T11:00:00",
            //     "billDate": "2025-12-05T19:52:50.533",
            //     "totalAmount": 250,
            //     "status": "Partially",
            //     "paymentMethod": "CreditCard",
            //     "paidAmount": 100,
            //     "paidAt": "2025-12-05T19:53:26.733"
            //   },
            //   {
            //     "patientName": "Youssef Hany",
            //     "doctorName": "Ibrahim Youssef",
            //     "appointmentDateTime": "2025-12-06T11:00:00",
            //     "billDate": "2025-12-05T19:54:09.68",
            //     "totalAmount": 250,
            //     "status": "Refunded",
            //     "paymentMethod": "CreditCard",
            //     "paidAmount": 0,
            //     "paidAt": "2025-12-05T19:54:40.643"
            //   },
            //   {
            //     "patientName": "Youssef Hany",
            //     "doctorName": "Ibrahim Youssef",
            //     "appointmentDateTime": "2025-12-06T11:30:00",
            //     "billDate": "2025-12-05T19:56:34.48",
            //     "totalAmount": 250,
            //     "status": "Refunded",
            //     "paymentMethod": "CreditCard",
            //     "paidAmount": 0,
            //     "paidAt": "2025-12-05T19:57:19.14"
            //   }
            // ]
          }
        })
        this.loading = false;
      }
    });

    if(this.patient){
    }
  }
}
