import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppointmentService } from '../../../core/services/appointment.service';
import { SafeUrlPipe } from '../../../core/pipes/safe-url.pipe';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-patient-payment',
  imports: [SafeUrlPipe, CommonModule],
  templateUrl: './patient-payment.component.html',
  styleUrl: './patient-payment.component.scss',
})
export class PatientPaymentComponent implements OnInit {
  paymentLink: string | null = null;
  appointmentId!: number;

  constructor(
    private route: ActivatedRoute,
    private appointmentService: AppointmentService
  ) {}
  ngOnInit(): void {
    this.appointmentId = Number(this.route.snapshot.paramMap.get('id'));

    this.appointmentService.confirm(this.appointmentId).subscribe({
      next: (res) => {
        if (res.success) {
          this.paymentLink = res.payment_link;
        }
      },
      error: (err) => {
        console.error('Payment error:', err);
      },
    });
  }
}
