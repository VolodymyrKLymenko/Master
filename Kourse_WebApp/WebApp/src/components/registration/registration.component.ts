import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AccountService } from 'src/core/services/auth/account.service';
import { CommonConstants } from 'src/utilities/common-constants';
import { RegistrationModel } from 'src/core/models/auth/registration.model';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.styl']
})
export class RegistrationComponent implements OnInit {
  public registrationForm: FormGroup;
  public validationMsg: string;
  private model: RegistrationModel = new RegistrationModel();

  constructor(
    private router: Router,
    private accountService: AccountService) { }

  public ngOnInit(): void {
    this.createForm();
  }

  private createForm(): void {
    this.registrationForm = new FormGroup({
      userEmail: new FormControl('', [Validators.email, Validators.required]),
      userName: new FormControl('', [Validators.required]),
      userPassword: new FormControl('', [Validators.required])
    });
  }

  public onSubmit(): void {
    this.setValuesFromFormToModel();
    this.accountService.registerUser(this.model)
      .subscribe(() => {
        this.router.navigate(['/login']);
      });
  }

  private setValuesFromFormToModel(): void {
    const values = this.registrationForm.getRawValue();
    this.model.email = values.userEmail;
    this.model.userLogin = values.userName;
    this.model.password = values.userPassword;
  }
}
