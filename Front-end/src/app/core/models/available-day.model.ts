export interface AvailableDayModel {
  doctorId: number;
  doctorName: string;
  recurringSchedule: RecurringSchedule[];
  specificDates: SpecificDate[];
  availableDaysString: string;
}

export interface RecurringSchedule {
  day: string;
  date: string;
  startTime: string | null;
  endTime: string | null;
  available: 'Available' | 'Not Available';
}

export interface SpecificDate {
  date: string;
  startTime: string | null;
  endTime: string | null;
  available: 'Available' | 'Not Available';
}
