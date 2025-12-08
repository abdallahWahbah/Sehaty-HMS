export interface DoctorResponseModel {
  id: number;
  firstName: string;
  lastName: string;
  specialty: string;
  licenseNumber: string;
  qualifications: string;
  yearsOfExperience: number;
  profilePhotoUrl?: string;
  availabilityNotes: string;
  userId: number;
  user?: string;
  departmentId: number;
  department?: string;
}
