import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { Department } from '../../../core/models/department-response.model';
import { DepartmentService } from '../../../core/services/department.service';
import { DoctorService } from '../../../core/services/doctor.service';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-departments',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './admin-departments.component.html',
  styleUrl: './admin-departments.component.scss',
})
export class AdminDepartmentsComponent implements OnInit {
  departments: Department[] = [];
  filteredDepartments: Department[] = []; // نسخة للفلترة
  searchTerm: string = ''; // نص البحث
  totalDepartments: number = 0;
  totalDoctors: number = 0;
  monthlyAppointments: string = '5000+';
  averageRating: string = '4.8';
  constructor(
    private deptService: DepartmentService,
    private doctorService: DoctorService,
    private cdr: ChangeDetectorRef
  ) {}
  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    forkJoin({
      departments: this.deptService.getAllDepartments(),
      doctors: this.doctorService.getAllDoctors(),
    }).subscribe({
      next: ({ departments, doctors }) => {
        this.departments = departments.map(dept => ({
          ...dept,
          doctors: doctors.filter((doc) => doc.departmentId === dept.id),
        }));
        this.filteredDepartments = this.departments

        this.totalDepartments = this.departments.length;
        this.totalDoctors = doctors.length;

        try {
          this.cdr.detectChanges();
        } catch (e) {
          console.warn('Change detection detectChanges() failed', e);
        }
      },
      error: (err) => console.error('ERROR loading data', err),
    });
  }
  searchDepartments() {
    if (!this.searchTerm) {
      this.filteredDepartments = [...this.departments];
    } else {
      console.log("111111111111111111", this.departments, this.searchTerm);
      this.filteredDepartments = this.departments.filter( dept => {
        return dept.en_Name.toLowerCase().includes(this.searchTerm.toLowerCase()) 
      }
      );
    }
  }
}
