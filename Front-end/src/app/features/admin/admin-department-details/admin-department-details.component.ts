import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DepartmentService } from '../../../core/services/department.service';
import { Department } from '../../../core/models/department-response.model';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-department-details',
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-department-details.component.html',
  styleUrls: ['./admin-department-details.component.scss'],
})
export class AdminDepartmentDetailsComponent implements OnInit {
  department?: Department;

  constructor(
    private route: ActivatedRoute,
    private deptService: DepartmentService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.deptService.getDepartmentById(id).subscribe({
      next: (data) => {
        this.department = data;
      },
      error: (err) => console.error(err),
    });
  }
  deleteDepartment() {
    if (confirm('Are you sure you want to delete this department?')) {
      if (!this.department) return; // للتأكد إن البيانات موجودة
      this.deptService.deleteDepartment(this.department.id).subscribe({
        next: () => {
          window.location.href = '/admin/departments';
        },
        error: (err) => console.error('Delete error:', err),
      });
    }
  }
}
