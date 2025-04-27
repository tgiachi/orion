import { makeAutoObservable, runInAction } from "mobx";
import { RootStore } from "./rootStore";
import axios, { AxiosInstance, AxiosRequestConfig } from "axios";


export class ApiStore {
  rootStore: RootStore;
  api: AxiosInstance;
  errors: Record<string, string | null> = {};

  constructor(rootStore: RootStore) {

    this.rootStore = rootStore;

    this.api = axios.create({
      baseURL: "http://0.0.0.0:23021/api/v1",
      headers: {
        "Content-Type": "application/json",
      },
    });

    console.log('initialize api store')
    this.setupInterceptors()




  }

  /**
   * Setup axios interceptors for auth token and error handling
   */
  private setupInterceptors() {
    // Request interceptor for adding auth token
    this.api.interceptors.request.use(
      (config) => {
        const { authStore } = this.rootStore;

        // Add auth token if available
        if (authStore.jwtToken) {
          config.headers.Authorization = `Bearer ${authStore.jwtToken}`;
        }

        return config;
      },
      (error) => Promise.reject(error),
    );

    // Response interceptor for handling token expiration
    this.api.interceptors.response.use(
      (response) => response,
      async (error) => {
        const { authStore } = this.rootStore;
        const originalRequest = error.config;

        // Handle 401 errors (unauthorized)
        if (error.response?.status === 401 && !originalRequest._retry) {
          // Mark the request as retried to prevent infinite loops
          originalRequest._retry = true;

          // Try to refresh the token
          if (authStore.refreshToken) {
            try {
              //TODO: refresh token
              //await authStore.refreshAccessToken();

              // Retry the original request with the new token
              originalRequest.headers.Authorization = `Bearer ${authStore.jwtToken}`;
              return this.api(originalRequest);
            } catch (refreshError) {
              // If refresh fails, log the user out
              authStore.logout();
              return Promise.reject(refreshError);
            }
          } else {
            // No refresh token available, log the user out
            authStore.logout();
          }
        }

        return Promise.reject(error);
      },
    );
  }



  /**
   * Set error for a specific request key
   */
  setError(key: string, error: string | null) {
    runInAction(() => {
      this.errors[key] = error;
    });
  }

  /**
   * Get error for a specific request key
   */
  getError(key: string): string | null {
    return this.errors[key] || null;
  }

  /**
   * Reset error for a specific request key
   */
  resetError(key: string) {
    runInAction(() => {
      this.errors[key] = null;
    });
  }

  /**
    * Perform a GET request
    * @param url - API endpoint
    * @param requestKey - Unique key for tracking loading state
    * @param config - Axios request config
    */
  async get<T>(
    url: string,
    requestKey: string,
    config?: AxiosRequestConfig,
  ): Promise<T | null> {
    try {
      this.rootStore.loadingStore.setLoading(requestKey);
      this.resetError(requestKey);
      const response = await this.api.get<T>(url, config);
      return response.data;
    } catch (error) {
      this.handleRequestError(error, requestKey);
      return null;
    } finally {
      this.rootStore.loadingStore.hideLoading();
    }
  }


  /**
   * Perform a POST request
   * @param url - API endpoint
   * @param data - Request payload
   * @param requestKey - Unique key for tracking loading state
   * @param config - Axios request config
   */
  async post<T, D = unknown>(
    url: string,
    data: D,
    requestKey: string,
    config?: AxiosRequestConfig,
  ): Promise<T | null> {
    try {
      this.rootStore.loadingStore.setLoading(requestKey)
      this.resetError(requestKey);

      const response = await this.api.post<T>(url, data, config);
      return response.data;
    } catch (error) {
      this.handleRequestError(error, requestKey);
      return null;
    } finally {
      this.rootStore.loadingStore.setLoading(requestKey);
    }
  }

  /**
   * Perform a PUT request
   * @param url - API endpoint
   * @param data - Request payload
   * @param requestKey - Unique key for tracking loading state
   * @param config - Axios request config
   */
  async put<T, D = unknown>(
    url: string,
    data: D,
    requestKey: string,
    config?: AxiosRequestConfig,
  ): Promise<T | null> {
    try {
      this.rootStore.loadingStore.setLoading(requestKey);
      this.resetError(requestKey);

      const response = await this.api.put<T>(url, data, config);
      return response.data;
    } catch (error) {
      this.handleRequestError(error, requestKey);
      return null;
    } finally {
      this.rootStore.loadingStore.hideLoading()
    }
  }

  /**
   * Perform a DELETE request
   * @param url - API endpoint
   * @param requestKey - Unique key for tracking loading state
   * @param config - Axios request config
   */
  async delete<T>(
    url: string,
    requestKey: string,
    config?: AxiosRequestConfig,
  ): Promise<T | null> {
    try {
      this.rootStore.loadingStore.setLoading(requestKey);
      this.resetError(requestKey);

      const response = await this.api.delete<T>(url, config);
      return response.data;
    } catch (error) {
      this.handleRequestError(error, requestKey);
      return null;
    } finally {
      this.rootStore.loadingStore.hideLoading()
    }
  }

  /**
   * Perform a PATCH request
   * @param url - API endpoint
   * @param data - Request payload
   * @param requestKey - Unique key for tracking loading state
   * @param config - Axios request config
   */
  async patch<T, D = unknown>(
    url: string,
    data: D,
    requestKey: string,
    config?: AxiosRequestConfig,
  ): Promise<T | null> {
    try {
      this.rootStore.loadingStore.setLoading(requestKey)
      this.resetError(requestKey);

      const response = await this.api.patch<T>(url, data, config);
      return response.data;
    } catch (error) {
      this.handleRequestError(error, requestKey);
      return null;
    } finally {
      this.rootStore.loadingStore.hideLoading()
    }
  }

  /**
   * Handle API request errors
   * @param error - Error object
   * @param requestKey - Request key for error context
   */
  private handleRequestError(error: unknown, requestKey: string) {
    if (axios.isAxiosError(error)) {
      const errorMessage =
        error.response?.data?.message || error.message || "An error occurred";
      this.setError(requestKey, errorMessage);
      console.error(`API Error (${requestKey}):`, errorMessage);
    } else {
      this.setError(requestKey, "An unexpected error occurred");
      console.error(`Unexpected API Error (${requestKey}):`, error);
    }
  }
}
