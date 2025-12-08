import { Component, OnInit } from '@angular/core';
import { DoctorService } from '../../../../../core/services/doctor.service';
import { DoctorResponseModel } from '../../../../../core/models/doctor-response-model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  AuthService,
  UserRole,
} from '../../../../../core/services/auth.service';

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

  // 👇 نفس فكرة الكومبوننت اللي قبلها
  currentRole: UserRole = null;

  get isPatient(): boolean {
    return this.currentRole === 'Patient';
  }

  get isReception(): boolean {
    return this.currentRole === 'Receptionist';
  }

  constructor(
    private doctorsService: DoctorService,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // نقرأ الـ Role من الـ userData في localStorage
    this.currentRole = this.authService.getCurrentUserRole();
    console.log('🧪 [ViewDoctors] Current Role = ', this.currentRole);

    // استلام departmentId من route params
    this.route.params.subscribe((params) => {
      this.departmentId = Number(params['departmentId']);
      this.loadDoctors();
    });
  }

  loadDoctors() {
    this.doctorsService.getAllDoctors().subscribe({
      next: (data) => {
        this.doctors = data;

        // فلترة الدكاترة حسب departmentId
        this.filteredDoctors = this.doctors.filter(
          (d) => Number(d.departmentId) === this.departmentId
        );

        this.loading = false;
      },
      error: () => {
        console.error('Error loading doctors');
        this.loading = false;
      },
    });
  }

  viewAvailability(doctor: DoctorResponseModel) {
    if (!doctor || !doctor.id) {
      console.error('Doctor id is invalid!');
      return;
    }

    // 👇 هنا الاختلاف حسب الـ Role
    if (this.isReception) {
      this.router.navigate([
        '/reception/new/appointments/available-days',
        doctor.id,
      ]);
    } else {
      // Patient أو في حالة عدم وجود role
      this.router.navigate(['/home/appointments/available-days', doctor.id]);
    }
  }
}
