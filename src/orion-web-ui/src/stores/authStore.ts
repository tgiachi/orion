import { RootStore } from "./rootStore";
import { makeAutoObservable, runInAction } from "mobx";

import { ApiStore } from "./apiStore";
import { LoginRequest, LoginResponse } from "../api/Api";

interface AuthLocalStorageData {
  jwtToken: string;
  refreshToken: string;
  tokenExpire: Date;
  refreshTokenExpire: Date;
  username: string;
}

export class AuthStore {
  rootStore: RootStore
  jwtToken: string | null = null
  refreshToken: string | null = null
  tokenExpire: Date | null = null
  refreshTokenExpire: Date | null = null;
  username: string | null = null;
  isAuthicated: boolean = false;
  apiStore: ApiStore;

  counter: number = 0;

  constructor(rootStore: RootStore) {

    makeAutoObservable(this)
    this.rootStore = rootStore;
    this.apiStore = rootStore.apiStore;


    runInAction(() => {
      this.loadAuthState();
    });
  }


  logout() {
    this.jwtToken = null;
    this.refreshToken = null;
    this.tokenExpire = null;
    this.refreshTokenExpire = null;
    this.username = null;
    this.isAuthicated = false;


    runInAction(() => {
      this.saveAuthState();
    });
  }

  async login(username: string, password: string) {

    const response = await this.apiStore.post<LoginResponse>(
      "/auth/login",
      { username, password } as LoginRequest,
      "login",
    );

    if (response) {
      console.log(`Logged is ${response.message}`)

      runInAction(() => {
        this.jwtToken = response.access_token!
        this.isAuthicated = true
        this.refreshToken = response.refresh_token!
        this.saveAuthState()
      })

      return true
    }

    return false
  }

  loadAuthState() {
    try {
      const authData = JSON.parse(
        localStorage.getItem("orion_auth_data") || "{}",
      ) as AuthLocalStorageData;

      if (authData.username) {
        this.isAuthicated = true;
      }
    } catch (ex) {
      console.error("Error during load Auth states" + ex);
    }
  }

  saveAuthState() {
    const authData = {
      jwtToken: this.jwtToken,
      refreshToken: this.refreshToken,
      refreshTokenExpire: this.refreshTokenExpire,
      tokenExpire: this.tokenExpire,
      username: this.username,
    } as AuthLocalStorageData;

    localStorage.setItem("orion_auth_data", JSON.stringify(authData));
  }
}
