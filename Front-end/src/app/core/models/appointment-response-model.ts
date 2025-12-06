export interface AppointmentResponseModel {
  id: number;
  doctorId?: number;
  patientId?: number;
  appointmentDateTime: Date;
  reasonForVisit: string;
  durationMinutes?: number;
  status: string;
  doctorName: string;
  patientName: string;
  notes: string;
  cancellationReason? :string;
}
