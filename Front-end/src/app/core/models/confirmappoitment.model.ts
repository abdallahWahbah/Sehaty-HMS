export interface ConfirmAppointmentResponse {
  success: boolean;
  payment_link: string;
  totalAmount: number;
  order_id: number;
  billingId: number;
}
