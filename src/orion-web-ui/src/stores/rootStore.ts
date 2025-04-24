import { createContext, useContext } from "react"
import { AuthStore } from "./authStore";
import { ApiStore } from "./apiStore";


export class RootStore {
  apiStore: ApiStore
  authStore: AuthStore

  constructor() {
    this.apiStore = new ApiStore(this)
    this.authStore = new AuthStore(this)
  }
}

const rootStore = new RootStore()

const RootStoreContext = createContext<RootStore>(rootStore);


export const useStore = () => useContext(RootStoreContext)

export default rootStore
