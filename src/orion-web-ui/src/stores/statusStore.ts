import { makeAutoObservable, runInAction } from "mobx";
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

    setInterval(() => this.checkStatus(), 5000)
  }

  async checkStatus() {
    console.log('checking version')
    const result = await this.apiStore.get<VersionInfoData>(`/system/version`, "")

    if (result) {
      runInAction(() => {
        this.isOnline = true;
        this.versionInfo = result

      })
    }


  }

}
