import { Component } from '@angular/core';
import { BillingResponseModel } from '../../../core/models/billing-response-model';
import { BillingService } from '../../../core/services/billing.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-all-billings',
  imports: [CommonModule],
  templateUrl: './all-billings.component.html',
  styleUrl: './all-billings.component.scss'
})
export class AllBillingsComponent {

  billings: BillingResponseModel[] = [];

  constructor(private _billingService: BillingService){}

  ngOnInit(){
    this._billingService.getAllBillings().subscribe({
      next: data => {
        this.billings = data;
      }
    })
  }
}
