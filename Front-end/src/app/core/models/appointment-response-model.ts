export interface AppointmentResponseModel {
    id: number,
    appointmentDateTime: Date,
    reasonForVisit: string,
    status: string,
    doctorName: string,
    patientName: string,
    notes: string
}