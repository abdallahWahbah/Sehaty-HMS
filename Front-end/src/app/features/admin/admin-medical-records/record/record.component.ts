import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { MedicalRecordModel } from '../../../../core/models/medical-record-model';
import { Dialog } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-record',
  imports: [Dialog, ButtonModule, CommonModule],
  templateUrl: './record.component.html',
  styleUrl: './record.component.scss'
})
export class RecordComponent implements OnChanges {

  @Input() record: MedicalRecordModel | null = null;

  ngOnChanges(changes: SimpleChanges): void {    
  }

}
