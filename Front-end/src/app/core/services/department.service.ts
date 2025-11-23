import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department } from '../models/department-response.model';
import { DepartmentCreate } from '../models/department.request.model';
@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
  private baseUrl = 'https://localhost:7086/api/Departments';

  constructor(private http: HttpClient) {}

  getAllDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(this.baseUrl);
  }
  getDepartmentById(id: number): Observable<Department> {
    return this.http.get<Department>(`${this.baseUrl}/${id}`);
  }
  deleteDepartment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
  updateDepartment(id: number, department: Department): Observable<Department> {
    return this.http.put<Department>(`${this.baseUrl}/${id}`, department);
  }
  addDepartment(department: DepartmentCreate): Observable<DepartmentCreate> {
    return this.http.post<DepartmentCreate>(this.baseUrl, department);
  }
}
