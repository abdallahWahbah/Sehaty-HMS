import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const adminGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  let userData: any = localStorage.getItem("userData");
  userData = JSON.parse(userData);

  if(userData?.role === 'Admin'){
    return true;
  }
  else if(!userData){
    router.navigate(['/login']);
    return false;
  }
  else{
    router.navigate(['/not-allowed']);
    return false;
  }
};
