export interface CreateAppointmentDto {
  patientId: number;
  doctorId: number;
  appointmentDateTime: string | Date;
  reasonForVisit: string;
}
