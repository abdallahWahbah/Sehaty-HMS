export interface MedicalRecord {
  id: number;
  recordDate: Date;
  symptoms: string;
  diagnosis: string;
  treatmentPlan: string;
  bpSystolic: number;
  bpDiastolic: number;
  temperature: number;
  heartRate: number;
  weight: number;
  vitalBp: string;
  notes: string;
  recordType: string;
  createdAt: Date;
  isFinialize: boolean | null;
  patientId: number;
  patientName: string;
}
