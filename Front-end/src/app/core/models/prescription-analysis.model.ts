export interface MedicationAnalysis {
  medicationName: string;
  dosage: string;
  frequency: string;
  duration: string;
}

export interface PrescriptionAnalysis {
  prescriptionId: number;
  patientName: string;
  doctorName: string;
  dateIssued: Date; // أو Date لو هتعمل لها تحويل
  analysisResult: string;
  medicationsAnalysis: MedicationAnalysis[];
  generalInstructions: string;
  analyzedAt: Date; // أو Date
}
