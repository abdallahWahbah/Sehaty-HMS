import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const nurseGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  let userData: any = localStorage.getItem("userData");
  userData = JSON.parse(userData);

  if(userData.role === 'Nurse'){
    return true;
  }
  else{
    router.navigate(['/not-allowed']);
    return false;
  }
};
