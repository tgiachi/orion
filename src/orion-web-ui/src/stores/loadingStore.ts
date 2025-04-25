import { makeAutoObservable } from "mobx";
import { RootStore } from "./rootStore";

export class LoadingStore {
  isLoading: boolean = false;
  message: string | null = null;
  rootStore: RootStore

  constructor(rootStore: RootStore) {
    this.rootStore = rootStore
    makeAutoObservable(this)
  }

  setLoading(message: string) {
    this.isLoading = true;
    this.message = message
  }

  hideLoading() {
    this.isLoading = false
    this.message = null
  }

}
