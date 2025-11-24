import { Medication } from "./medication-response-model";

export interface Prescription {
    id: number;
    status: string;
    digitalSignature: string;
    specialInstructions: string;
    dateIssued: Date;
    appointmentId: number;
    medicalRecordId: number;
    medicalRecordNotes: string;
    patientId: number;
    patiantName: string; 
    mrn: string | null;
    doctorId: number;
    doctorName: string;
    licenseNumber: string;
    medications: Medication[];
}