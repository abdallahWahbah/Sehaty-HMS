import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DepartmentService } from '../../../core/services/department.service';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-admin-add-department',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './admin-add-department.component.html',
  styleUrls: ['./admin-add-department.component.scss'],
})
export class AdminAddDepartmentComponent implements OnInit {
  addForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private departmentService: DepartmentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.addForm = this.fb.group({
      en_Name: ['', Validators.required],
      ar_Name: ['', Validators.required],
      description: ['', Validators.required],
    });
  }

  onSubmit() {
    if (this.addForm.invalid) return;

    this.departmentService.addDepartment(this.addForm.value).subscribe({
      next: () => {
        this.router.navigate(['/admin/departments']);
      },
      error: (err) => console.error('Add error:', err),
    });
  }
}
