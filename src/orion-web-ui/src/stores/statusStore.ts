import { makeAutoObservable } from "mobx";
import { RootStore } from "./rootStore";
import { VersionInfoData } from "../api/Api";
import { ApiStore } from "./apiStore";
import { LoadingStore } from "./loadingStore";

export class StatusStore {

  rootStore: RootStore
  isOnline: boolean = false
  apiStore: ApiStore;
  loadingStore: LoadingStore
  versionInfo: VersionInfoData | null = null

  constructor(rootStore: RootStore) {
    this.rootStore = rootStore
    this.apiStore = rootStore.apiStore
    this.loadingStore = rootStore.loadingStore

    makeAutoObservable(this)
    setInterval(() => this.checkStatus(), 1000)
  }

  checkStatus() {
    console.log('checking version')

    this.apiStore.get(`/system/version`, "")



  }

}
