import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
  
export const landingPageGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  let userData: any = localStorage.getItem("userData");
  userData = JSON.parse(userData);

  if(userData?.role === "Doctor"){
    router.navigate(['/doctor']);
    return false;
  }
    else if(userData?.role === "Receptionist"){
    router.navigate(['/reception']);
    return false;
  }
  else if(userData?.role === "Admin"){
    router.navigate(['/admin']);
    return false;
  }
  else {
    return true;
  }
};
