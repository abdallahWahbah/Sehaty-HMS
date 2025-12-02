import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models/user-response';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-users',
  imports: [FormsModule, CommonModule],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss',
})
export class AdminUsersComponent implements OnInit {
  users: User[] = [];
  isLoading = false;
  errorMessage = '';

  // رولات مع الـ IDs
  rolesMap: { [key: string]: number } = {
    Admin: 1,
    Doctor: 2,
    Patient: 3,
    Nurse: 4,
    Receptionist: 5,
  };
  roleNames: string[] = Object.keys(this.rolesMap);
  selectedRoles: { [userId: number]: string } = {};
  constructor(private userService: UserService) {}
  ngOnInit() {
    this.loadUsers();
  }
  loadUsers() {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        users.forEach((u) => {
          this.selectedRoles[u.id] = u.role;
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load users.';
        this.isLoading = false;
        console.error(err);
      },
    });
  }
  // changeRole(userId: number) {
  //   const selectedRoleName = this.selectedRoles[userId];
  //   const newRoleId = this.rolesMap[selectedRoleName];

  //   if (!newRoleId) {
  //     alert('Invalid role selected!');
  //     return;
  //   }

  //   this.userService.changeUserRole(userId, newRoleId).subscribe({
  //     next: () => {
  //       console.log(`Role updated to ${selectedRoleName} successfully.`);
  //     },
  //     error: (err) => {
  //       console.error(err);
  //     },
  //   });
  // }
}
