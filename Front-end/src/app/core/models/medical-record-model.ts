export interface MedicalRecordModel {
    id: number;
    appointmentId: number;
    recordDate: string;
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
    createdAt: string;
    isFinialize: boolean | null;
    patientName?: string;
    appointmentDateTime?: string;
    patientId?: number
}