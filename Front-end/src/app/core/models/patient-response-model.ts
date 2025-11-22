import { PateintStatusEnum } from "../enums/patient-status-enum";

export interface PatientResponseModel  {
    id: number,
    patientId?: string,
    mrn: string,
    firstName: string,
    lastName: string,
    dateOfBirth: Date,
    gender: string,
    nationalId: string,
    bloodType: string,
    allergies: string,
    chrinicConditions: string,
    address: string,
    emergencyContactName: string,
    emergencyContactPhone: string,
    status: PateintStatusEnum,
    registrationDate: Date,
    userId: number,
    user: any
}