export interface DoctorAvailableSlotsModel{
  doctorId: number,
  days: string,
  startTime: Date,
  endTime: Date,
  isRecurring: boolean,
  date: Date | null
}