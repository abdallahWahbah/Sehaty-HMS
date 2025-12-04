import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DepartmentService } from '../../../../core/services/department.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService, UserRole } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-view-departments',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './view-departments.component.html',
  styleUrls: ['./view-departments.component.scss'],
})
export class ViewDepartmentsComponent implements OnInit {
  departments: any[] = [];
  filteredDepartments: any[] = [];
  searchTerm: string = '';
  loading: boolean = true;

  currentRole: UserRole = null;

  get isPatient(): boolean {
    return this.currentRole === 'Patient';
  }

  get isReception(): boolean {
    return this.currentRole === 'Receptionist'; // 👈 خلي بالك من الاسم هنا
  }

  constructor(
    private departmentsService: DepartmentService,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentRole = this.authService.getCurrentUserRole();

    console.log('🧪 Current Role = ', this.currentRole); // 👈 جرّب تشوفها في الكونسل

    this.loadDepartments();
  }

  loadDepartments() {
    this.departmentsService.getAllDepartments().subscribe({
      next: (data) => {
        this.departments = data;
        this.filteredDepartments = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading departments', err);
        this.loading = false;
      },
    });
  }

  searchDepartments() {
    const term = this.searchTerm.toLowerCase();
    this.filteredDepartments = this.departments.filter(
      (d) =>
        d.en_Name.toLowerCase().includes(term) ||
        (d.description && d.description.toLowerCase().includes(term))
    );
  }

  viewDoctors(departmentId: number) {
    if (this.isReception) {
      // مسار الريسيبشن
      this.router.navigate([
        '/reception/new/appointments/doctors',
        departmentId,
      ]);
    } else {
      // Patient أو لو الـ role مش معروف
      this.router.navigate(['/home/appointments/doctors', departmentId]);
    }
  }
}
