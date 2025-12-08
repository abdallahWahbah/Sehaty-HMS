import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Pipe({
  name: 'safeUrl',
  standalone: true,
})
export class SafeUrlPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}

  transform(url: string | null): SafeResourceUrl {
    // لو null أو empty string ارجع لينك فارغ آمن
    return this.sanitizer.bypassSecurityTrustResourceUrl(url ?? 'about:blank');
  }
}
