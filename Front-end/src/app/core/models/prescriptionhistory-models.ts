export interface PrescriptionHistory {
  prescriptionId: number;
  dateIssued: Date;
  doctorName: string;
  specialInstructions: string;
  medicationNames: string[];
  status: string;
}
