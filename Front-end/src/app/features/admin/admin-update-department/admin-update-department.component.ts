import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DepartmentService } from '../../../core/services/department.service';
import { Department } from '../../../core/models/department-response.model';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-update-department',
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './admin-update-department.component.html',
  styleUrl: './admin-update-department.component.scss',
})
export class AdminUpdateDepartmentComponent {
  departmentId!: number;
  department!: Department;
  updateForm!: FormGroup;
  loading: boolean = false;
  serverError = ""; 

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private departmentService: DepartmentService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.departmentId = Number(this.route.snapshot.paramMap.get('id'));
    this.getDepartment(this.departmentId);

    this.updateForm = this.fb.group({
      en_Name: ['', Validators.required],
      ar_Name: ['', Validators.required],
      description: ['', Validators.required],
    });
  }

  getDepartment(id: number) {
    this.departmentService.getDepartmentById(id).subscribe({
      next: (res) => {
        this.department = res;
        this.updateForm.patchValue({
          en_Name: res.en_Name,
          ar_Name: res.ar_Name,
          description: res.description,
        });
      },
      error: (err) => console.error('Failed to load department:', err),
    });
  }

  updateDepartment() {
    this.serverError = "";
    if (this.updateForm.invalid) return;

    this.loading = true;

    const updatedData = this.updateForm.value as Department;

    this.departmentService
      .updateDepartment(this.departmentId, updatedData)
      .subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/admin/departments']);
        },
        error: (err) => {
          
          this.loading = false;
          console.error('Update failed:', err.error.errors[0]);
          this.serverError = err.error.errors[0];
        },
      });
  }
}
