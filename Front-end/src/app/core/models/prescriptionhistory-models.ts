import { Medication } from "./medication-response-model";

export interface PrescriptionHistory {
  prescriptionId: number;
  dateIssued: Date;
  doctorName: string;
  specialInstructions: string;
  medications: Medication[];
  status: string;
  doctorNotes?: string,
}
