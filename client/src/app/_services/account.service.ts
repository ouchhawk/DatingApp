// injectable dekoratörü bu servisin başka bir servise ya da component'e inject edilebileceğini gösterir
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';

import { map } from 'rxjs/operators';

import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
// iki nokta üst üste type definition, eşittir= set value
export class AccountService {
  baseUrl='https://localhost:5001/api/';

  // replaysubject is kind of like a buffer object number is how many versions of the current user we store
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  /// model is what the api sent to us
  // localstora browser'in storage'i
  login(model: any){
    return this.http.post(this.baseUrl + 'account/login', model ).pipe(
      map((response: User) => {
        const user= response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    )
  }

  register(model: any){
    return this.http.post(this.baseUrl+'account/register', model).pipe(
      map((user: User) => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    )
  }

  setCurrentUser(user: User){
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}