import { createContext, useContext } from "react"
import { AuthStore } from "./authStore";
import { ApiStore } from "./apiStore";
import { LoadingStore } from "./loadingStore";
import { StatusStore } from "./statusStore";


export class RootStore {
  apiStore: ApiStore
  authStore: AuthStore
  loadingStore: LoadingStore
  statusStore: StatusStore

  constructor() {
    this.loadingStore = new LoadingStore(this)
    this.apiStore = new ApiStore(this)
    this.authStore = new AuthStore(this)
    this.statusStore = new StatusStore(this)
  }
}

const rootStore = new RootStore()

const RootStoreContext = createContext<RootStore>(rootStore);


export const useStore = () => useContext(RootStoreContext)

export default rootStore
