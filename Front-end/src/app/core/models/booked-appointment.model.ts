export interface BookedAppointmentModel {
  appointmentId: number;
  slotId: number;
  doctorId: number;
  doctorName: string;
  patientId: number;
  patientName: string;
  appointmentDateTime: string;
  date: string;
  startTime: string;
  endTime: string;
  status: string; // Pending, Confirmed, etc.
}
