export interface AvailableSlotModel {
  doctorId: number;
  doctorName: string;
  date: string;
  dayOfWeek: string;
  totalSlots: number;
  availableSlots: number;
  bookedSlots: number;
  slots: Slot[];
}

export interface Slot {
  slotId: number;
  date: string;
  startTime: string;
  endTime: string;
  timeRange: string;
  isBooked: boolean;
}
