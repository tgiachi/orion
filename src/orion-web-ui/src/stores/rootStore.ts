import { createContext, useContext } from "react"

export class RootStore {

}

const rootStore = new RootStore()

const RootStoreContext = createContext<RootStore>(rootStore);


export const useStore = () => useContext(RootStoreContext)

export default rootStore
