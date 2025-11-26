import { Component, OnInit } from '@angular/core';
import { DoctorService } from '../../../../../core/services/doctor.service';
import { DoctorResponseModel } from '../../../../../core/models/doctor-response-model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-view-doctors',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './view-doctors.component.html',
  styleUrls: ['./view-doctors.component.scss'],
})
export class ViewDoctorsComponent implements OnInit {
  departmentId!: number;
  doctors: DoctorResponseModel[] = [];
  filteredDoctors: DoctorResponseModel[] = [];
  loading = true;

  constructor(
    private doctorsService: DoctorService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    // استلام departmentId من route params
    this.route.params.subscribe((params) => {
      this.departmentId = Number(params['departmentId']);
      console.log('Route departmentId:', this.departmentId); // <-- تتبع

      this.loadDoctors();
    });
  }

  loadDoctors() {
    this.doctorsService.getAllDoctors().subscribe({
      next: (data) => {
        this.doctors = data;
        console.log('All doctors:', this.doctors); // <-- تتبع

        // فلترة الدكاترة حسب departmentId
        this.filteredDoctors = this.doctors.filter(
          (d) => Number(d.departmentId) === this.departmentId
        );
        console.log('Filtered doctors:', this.filteredDoctors); // <-- تتبع

        this.loading = false;
      },
      error: () => {
        console.error('Error loading doctors');
        this.loading = false;
      },
    });
  }

  viewAvailability(doctor: DoctorResponseModel) {
    console.log('Navigating with doctor id:', doctor.id);
    console.log('Doctor object:', doctor);

    if (!doctor || !doctor.id) {
      console.error('Doctor id is invalid!');
      return;
    }

    // Navigate لصفحة الأيام المتاحة للدكتور
    this.router.navigate(['/patient/appointments/available-days', doctor.id]);
  }
}
