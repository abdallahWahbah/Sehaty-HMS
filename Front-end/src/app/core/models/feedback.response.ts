export interface FeedbackResponseModel {
  id: number;
  rating: number;
  comments: string;
  isAnonymous: boolean;
  submittedAt: Date; // بعد التحويل
  appointmentId: number;
  appointmentDateTime: Date; // بعد التحويل
}
