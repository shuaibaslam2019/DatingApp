import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  model: any = {};
  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
  }
  login() {
      this.authService.login(this.model).subscribe(next => {
          this.alertify.success('Logged in successfully');
        }, error => {
          this.alertify.error('Login falied');
      }, () => {
        this.router.navigate(['/members']);
      });
  }
  loggedIn() {
      // const token = localStorage.getItem('token');
      // return !!token; // if token has value then otherwise false

      // new method
      return this.authService.loggedIn();
  }

    logOut() {
      localStorage.removeItem('token');
      this.alertify.message('Logged out');
      this.router.navigate(['/home']);
    }

}
