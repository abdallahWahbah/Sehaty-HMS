export interface BillingResponseModel{
    patientName: string,
    doctorName: string,
    appointmentDateTime: Date | string,
    billDate: Date | string,
    totalAmount: number,
    status: string,
    paymentMethod: any,
    paidAmount: number,
    paidAt: any
}