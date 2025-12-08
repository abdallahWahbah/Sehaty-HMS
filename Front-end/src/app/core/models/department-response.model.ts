import { DoctorResponseModel } from './doctor-response-model';
export interface Department {
  id: number;
  en_Name: string;
  ar_Name: string;
  description: string;
  doctors?: DoctorResponseModel[];
}
