import { MedicalRecordType } from "../enums/medical-record-enum";

export interface MedicalRecordModel {
  patientId: number,
  symptoms: string,
  diagnosis: string,
  treatmentPlan: string,
  bpSystolic: number,
  bpDiastolic: number,
  temperature: number,
  heartRate: number,
  weight: number,
  vitalBp: string,
  notes: string,
  recordType: MedicalRecordType
}