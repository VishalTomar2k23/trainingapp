import { Injectable } from "@angular/core";
import { JwtHelper } from "../helper/jwt-helper";
import { LoggedInUser } from "../models/loggedin-user";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    constructor(private jwtHelper: JwtHelper) {

    }
    login(token, callback) {
        localStorage.setItem('isLoggedin', 'true');
        localStorage.setItem('USERTOKEN', token);
        if (callback) {
            callback();
        }

    }
    logout(callback) {
        localStorage.removeItem('isLoggedin');
        localStorage.removeItem('USERTOKEN');
        if (callback) {
            callback();
        }
    }

    getLoggedInUserInfo() {
        let token = localStorage.getItem('USERTOKEN');
        var user: LoggedInUser = this.jwtHelper.decodeToken(token);
        return user;
    }
}
