import { runInAction } from "mobx";


type LoadingState = {
  [key: string]: boolean;
};


export class BaseStore {
  loadingState: LoadingState = {}
  /**
   * Set loading state for a specific request key
   */
  setLoading(key: string, isLoading: boolean) {
    runInAction(() => {
      this.loadingState[key] = isLoading;
    });
  }

  /**
   * Check if a specific request is loading
   */
  isLoading(key: string): boolean {
    return !!this.loadingState[key];
  }
}
