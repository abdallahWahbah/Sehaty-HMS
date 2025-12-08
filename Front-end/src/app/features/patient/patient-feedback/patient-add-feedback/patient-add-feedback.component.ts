import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { FeedbackService } from '../../../../core/services/feedback.service';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-patient-add-feedback',
  imports: [ReactiveFormsModule],
  templateUrl: './patient-add-feedback.component.html',
  styleUrl: './patient-add-feedback.component.scss',
})
export class PatientAddFeedbackComponent implements OnInit {
  feedbackForm!: FormGroup;
  appointmentId!: number;

  constructor(
    private fb: FormBuilder,
    private feedbackService: FeedbackService,
    private route: ActivatedRoute,
    private router: Router
  ) {}
  ngOnInit(): void {
    // الحصول على الـ appointmentId من الرابط (URL)
    this.appointmentId = Number(this.route.snapshot.paramMap.get('id'));

    // تهيئة الفورم
    this.feedbackForm = this.fb.group({
      rating: [
        null,
        [Validators.required, Validators.min(1), Validators.max(5)],
      ],
      comments: ['', Validators.required],
      isAnonymous: [false],
    });
  }
  onSubmit() {
    if (this.feedbackForm.invalid) {
      return;
    }

    const feedbackData = {
      ...this.feedbackForm.value,
      appointmentId: this.appointmentId,
    };

    this.feedbackService.add(feedbackData).subscribe({
      next: () => {
        this.router.navigate(['/home/feedback']); // أو أي صفحة تحب ترجع لها
      },
      error: (err) => {
        console.error('Error submitting feedback:', err);
      },
    });
  }
}
