import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DepartmentService } from '../../../../core/services/department.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

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
  loading: boolean = true; // <--- أضفنا هنا

  constructor(
    private departmentsService: DepartmentService,
    private router: Router
  ) {}

  ngOnInit(): void {
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
    this.router.navigate(['/patient/appointments/doctors', departmentId]);
  }
}
