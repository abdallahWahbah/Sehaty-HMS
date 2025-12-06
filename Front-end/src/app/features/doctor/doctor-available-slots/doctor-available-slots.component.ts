import { Component } from '@angular/core';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { DoctorService } from '../../../core/services/doctor.service';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DayOption } from '../../../core/models/days-options-model';
import { WeekDays } from '../../../core/enums/week-days-enum';
import { DoctorAvailableSlotService } from '../../../core/services/doctor-available-slot.service';
import { DoctorAvailableSlotsModel } from '../../../core/models/availability-slot-model';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { LoadingSpinnerComponent } from "../../../layout/loading-spinner/loading-spinner.component";

@Component({
  selector: 'app-doctor-available-slots',
  imports: [FormsModule, ReactiveFormsModule, LoadingSpinnerComponent],
  templateUrl: './doctor-available-slots.component.html',
  styleUrl: './doctor-available-slots.component.scss'
})
export class DoctorAvailableSlotsComponent {
  currentDoctor!: DoctorResponseModel;
  isLoading: boolean = true;
  slotsForm!: FormGroup;
  serverError: string = '';
  sevenDaysExclFriday: DayOption[] = [];
  selectedDaysError: string = '';
  dateError: string = '';
  dayOptions: DayOption[] = [
    { name: 'Saturday', value: WeekDays.Saturday, controlName: 'Saturday' },
    { name: 'Sunday',   value: WeekDays.Sunday,   controlName: 'Sunday' },
    { name: 'Monday',   value: WeekDays.Monday,   controlName: 'Monday' },
    { name: 'Tuesday',  value: WeekDays.Tuesday,  controlName: 'Tuesday' },
    { name: 'Wednesday',value: WeekDays.Wednesday,controlName: 'Wednesday' },
    { name: 'Thursday', value: WeekDays.Thursday, controlName: 'Thursday' },
    { name: 'Friday',   value: WeekDays.Friday,   controlName: 'Friday' },
  ];

  constructor(
    private _doctorService: DoctorService,
    private fb: FormBuilder,
    private _doctorAvailabilitySlot: DoctorAvailableSlotService,
    private router: Router
  ) {}

  ngOnInit() {
    // stored data
    let storedUser: any = localStorage.getItem('userData');
    storedUser = JSON.parse(storedUser);

    // current doctor
    this._doctorService.getAllDoctors().subscribe({
      next: (allDoctors) => {
        this.currentDoctor = allDoctors.find(doc => doc.userId === storedUser.userId)!;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      },
    });

    this.slotsForm = this.fb.group({
      startTime: [''],
      endTime: [''],
      isRecurring: [true],
      date: [''],
      days: this.buildDaysGroup()
    });

    // toggle date field based on isRecurring
    this.slotsForm.get('isRecurring')?.valueChanges.subscribe(value => {
      this.dateError = this.selectedDaysError = '';
      const dateControl = this.slotsForm.get('date');
      if (!value) {
        dateControl?.setValidators([/* any validators like required */]);
      } else {
        dateControl?.clearValidators();
        dateControl?.setValue('');
      }
      dateControl?.updateValueAndValidity();
    });

    // populate form from API
    const response = {
      doctorId: 2,
      days: '0',
      startTime: '09:00:00',
      endTime: '17:00:00',
      isRecurring: true,
      date: null
    };
    this.patchForm(response);
    this.isLoading = false;
  }

  getSelectedDates(): string[] {
    const now = new Date();
    const selectedDates: string[] = [];

    this.dayOptions.forEach(dayOption => {
      const isSelected = this.slotsForm.get('days')?.get(dayOption.controlName)?.value;
      if (isSelected) {
        let current = new Date(now);
        for (let i = 0; i < 14; i++) { // look ahead up to 2 weeks
          const currentDayName = current.toLocaleDateString('en-US', { weekday: 'long' });

          if (currentDayName === dayOption.name) {
            // // If today is the selected day, only allow it if before 12 PM
            // if (
            //   current.getDate() === now.getDate() &&
            //   current.getMonth() === now.getMonth() &&
            //   current.getFullYear() === now.getFullYear() &&
            //   now.getHours() >= 12
            // ) {
            //   current.setDate(current.getDate() + 7); // move to next week's same day
            // }

            selectedDates.push(this.formatDate(current));
            break;
          }

          current.setDate(current.getDate() + 1);
        }
      }
    });

    return selectedDates;
  }


  formatDate(date: any): string {
    const dd = String(date.getDate()).padStart(2, '0');
    const mm = String(date.getMonth() + 1).padStart(2, '0'); // months are 0-indexed
    const yyyy = date.getFullYear();
    return `${yyyy}-${mm}-${dd}`;
  }

  buildDaysGroup() {
    const group: any = {};
    this.dayOptions.forEach(day => {
      group[day.controlName] = new FormControl(false);
    });
    return this.fb.group(group);
  }

  patchForm(data: any) {
    this.slotsForm.patchValue({
      startTime: data.startTime,
      endTime: data.endTime,
      isRecurring: data.isRecurring,
      date: data.date
    });

    // handle days bitmask
    const daysNumber = Number(data.days);
    this.dayOptions.forEach(day => {
      const selected = (daysNumber & day.value) === day.value;
      this.slotsForm.get('days')?.get(day.controlName)?.setValue(selected);
    });
  }

  onSubmit() {
    this.isLoading = true;
    this.serverError = '';
    const formValue = this.slotsForm.value;

    // convert days back to bitmask
    let daysBitmask = 0;
    Object.keys(formValue.days).forEach(key => {
      if (formValue.days[key]) {
        const day = this.dayOptions.find(d => d.controlName === key);
        if (day) daysBitmask |= day.value;
      }
    });

    // reset errors
    this.selectedDaysError = '';
    this.dateError = '';

    // validation 1: no day selected
    if (daysBitmask === 0 && formValue.isRecurring) {
      this.selectedDaysError = 'Please select at least one day.';
      this.isLoading = false;
      return;
    }

    // validation 2: non-recurring but no date selected
    if (!formValue.isRecurring && !formValue.date) {
      this.dateError = 'Please select a date';
      this.isLoading = false;
      return;
    }

    const payload: DoctorAvailableSlotsModel = {
      ...formValue,
      days: daysBitmask
    };

    let dates = this.getSelectedDates();

    // // add availability slots
    this._doctorAvailabilitySlot.addAvailabilitySlot({
      doctorId: this.currentDoctor.id,
      days: payload.days.toString(),
      startTime: payload.startTime,
      endTime: payload.endTime,
      isRecurring: payload.isRecurring,
      date: payload.isRecurring ? null : payload.date
    }).subscribe({
      next: data => {
        console.log("days generated");
        if (formValue.isRecurring) {
          const requests = dates.map(date =>
            this._doctorAvailabilitySlot.generateSlots({
              doctorId: this.currentDoctor.id,
              date: date
            })
          );
          forkJoin(requests).subscribe({
            next: results => {
              console.log("slots generated");
              this.isLoading = true;
              setTimeout(() => {
                this.router.navigate(['/doctor/appointments']);
              }, 1000)
            },
            error: err => {
              this.isLoading = false;
              console.log("error generating slots");
              this.serverError = err.error?.message;
            }
          });
        }
        else{
          this._doctorAvailabilitySlot.generateSlots({
            doctorId: this.currentDoctor.id,
            date: formValue.date,
          }).subscribe({
            next: data => {
              console.log("slots for single day generated");
              this.router.navigate(['/doctor/appointments']);
            },
            error: err => {
              this.isLoading = false;
              console.log("error generating single day slots");
              console.log(err);
              this.serverError = err.error?.message
            }
          })
        }
        this.isLoading = false;
      },
      error: err => {
        this.serverError = err.error?.message;
        this.isLoading = false;
      }
    })
    this.isLoading = false;
  }
}
