import { Component } from '@angular/core';
import { DoctorResponseModel } from '../../../core/models/doctor-response-model';
import { DoctorService } from '../../../core/services/doctor.service';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DayOption } from '../../../core/models/days-options-model';
import { WeekDays } from '../../../core/enums/week-days-enum';
import { DoctorAvailableSlotService } from '../../../core/services/doctor-available-slot.service';
import { DoctorAvailableSlotsModel } from '../../../core/models/availability-slot-model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-doctor-available-slots',
  imports: [FormsModule, ReactiveFormsModule],
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

    this.generateSevenDaysExcludingFriday();

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

  generateSevenDaysExcludingFriday() {
    const todayIndex = new Date().getDay(); // Sunday=0, Monday=1, ..., Saturday=6
    const dayMap = [WeekDays.Sunday, WeekDays.Monday, WeekDays.Tuesday, WeekDays.Wednesday, WeekDays.Thursday, WeekDays.Friday, WeekDays.Saturday];
    const orderedDays = [ 'Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday' ];

    let result: DayOption[] = [];
    let count = 0;
    let current = todayIndex;

    while (count < 6) {
      const dayName = orderedDays[current];
      if (dayName !== 'Friday') {
        const option = this.dayOptions.find(d => d.name === dayName);
        if (option) {
          result.push(option);
          count++;
        }
      }
      current = (current + 1) % 7; // loop through the week
    }

    this.sevenDaysExclFriday = result;

  }

  getSelectedDates(): string[] {
    const today = new Date();
    const selectedDates: string[] = [];

    this.sevenDaysExclFriday.forEach(dayOption => {
    const isSelected = this.slotsForm.get('days')?.get(dayOption.controlName)?.value;
    if (isSelected) {
    // calculate next occurrence of the selected day starting from today
    let current = new Date(today);
    for (let i = 0; i < 7; i++) {
    const currentDayName = current.toLocaleDateString('en-US', { weekday: 'long' });
    if (currentDayName === dayOption.name) {
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
      return;
    }

    // validation 2: non-recurring but no date selected
    if (!formValue.isRecurring && !formValue.date) {
      this.dateError = 'Please select a date';
      return;
    }

    const payload: DoctorAvailableSlotsModel = {
      ...formValue,
      days: daysBitmask
    };

    console.log(payload, typeof payload);
    // console.log(this.getSelectedDates());

    let dates = this.getSelectedDates();

    // add availability slots
    this._doctorAvailabilitySlot.addAvailabilitySlot({
      doctorId: this.currentDoctor.id,
      days: payload.days.toString(),
      startTime: payload.startTime,
      endTime: payload.endTime,
      isRecurring: payload.isRecurring,
      date: payload.isRecurring ? null : payload.date
    }).subscribe({
      next: data => console.log(data),
      error: err => {
        console.log(err);
        this.serverError = err.error?.message;
      }
    })

    // generate slots
    if(formValue.isRecurring){
      dates.forEach(date => {
        this._doctorAvailabilitySlot.generateSlots({
          doctorId: this.currentDoctor.id,
          date: date,
        }).subscribe({
        next: data => {
          console.log(data);
          this.router.navigate(['/doctor/appointments']);
        },
        error: err => {
          console.log(err);
          this.serverError = err.error?.message
        }
      })
      })
    }
    else{
      this._doctorAvailabilitySlot.generateSlots({
        doctorId: this.currentDoctor.id,
        date: formValue.date,
      }).subscribe({
        next: data => {
          console.log(data);
          this.router.navigate(['/doctor/appointments']);
        },
        error: err => {
          console.log(err);
          this.serverError = err.error?.message
        }
      })
    }    
  }

}
