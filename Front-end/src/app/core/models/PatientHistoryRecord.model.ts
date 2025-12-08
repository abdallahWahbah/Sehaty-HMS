export interface PatientHistoryRecord {
  recordId: number;
  recordDate: string;
  recordType: string;
  diagnosis: string;
  symptoms: string;
  treatmentPlan: string;
  medications: string[];
}
