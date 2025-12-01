import { PatientHistoryRecord } from './PatientHistoryRecord.model';

export interface PatientHistoryAnalysis {
  patientId: number;
  patientName: string;
  totalPrescriptions: number;
  aiSummary: string;
  records: PatientHistoryRecord[];
}

export interface PatientHistoryResponse {
  data: PatientHistoryAnalysis;
  isSuccess: boolean;
  error: string | null;
  errorType: string;
}
