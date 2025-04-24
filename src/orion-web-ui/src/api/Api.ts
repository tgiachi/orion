/* eslint-disable */
/* tslint:disable */
// @ts-nocheck
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface ChannelData {
    name: string;
    founder?: string;
    topic?: string;
    /** @format date-time */
    creation_time?: string;
    /** @format date-time */
    topic_set_time?: string;
    topic_set_by?: string;
    key?: string | null;
    members?: ChannelMembership[] | null;
    /** @format int32 */
    user_limit?: number | null;
    /** @format int32 */
    member_count?: number;
    invite_list?: string[] | null;
    is_secret?: boolean;
    is_private?: boolean;
    is_invite_only?: boolean;
    has_key?: boolean;
    has_user_limit?: boolean;
    is_moderated?: boolean;
    no_external_messages?: boolean;
    topic_protection?: boolean;
    have_topic?: boolean;
}

export interface ChannelEntry {
    name: string;
    /** @format int32 */
    users: number;
    topic: string;
}

export interface ChannelMembership {
    nick_name?: string;
    is_operator?: boolean;
    has_voice?: boolean;
    /** @format date-time */
    join_time?: string;
    prefix_nickname?: string | null;
}

export interface IrcUserSession {
    is_local?: boolean;
    remote_server_id?: string;
    session_id?: string;
    is_secure_connection?: boolean;
    endpoint?: string;
    remote_address?: string;
    /** @format int32 */
    remote_port?: number;
    nick_name?: string;
    user_name?: string;
    real_name?: string;
    host_name?: string;
    v_host_name?: string | null;
    full_address?: string | null;
    /** @format date-time */
    last_activity?: string;
    /** @format date-time */
    created?: string;
    /** @format date-time */
    last_ping_response?: string;
    is_user_sent?: boolean;
    is_nick_sent?: boolean;
    is_password_valid?: boolean;
    is_authenticated?: boolean;
    is_registered?: boolean;
    is_away?: boolean;
    away_message?: string;
    modes_string?: string | null;
    is_invisible?: boolean;
    is_operator?: boolean;
    receives_wallops?: boolean;
    is_registered_user?: boolean;
}

export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    is_success?: boolean;
    message?: string | null;
    access_token?: string | null;
    refresh_token?: string | null;
    /** @format date-time */
    access_token_expires_at?: string | null;
    /** @format date-time */
    refresh_token_expires_at?: string | null;
}

export interface VersionInfoData {
    app_name: string;
    code_name: string;
    version: string;
    git_hash: string;
    branch: string;
    build_date: string;
}

export namespace System {
    /**
     * @description Get the version information
     * @tags System
     * @name V1SystemVersionList
     * @request GET:/api/v1/system/version
     * @response `200` `VersionInfoData` OK
     */
    export namespace V1SystemVersionList {
        export type RequestParams = {};
        export type RequestQuery = {};
        export type RequestBody = never;
        export type RequestHeaders = {};
        export type ResponseBody = VersionInfoData;
    }

    /**
     * @description Get the sessions
     * @tags System
     * @name V1SystemSessionsList
     * @request GET:/api/v1/system/sessions
     * @response `200` `(IrcUserSession)[]` OK
     */
    export namespace V1SystemSessionsList {
        export type RequestParams = {};
        export type RequestQuery = {};
        export type RequestBody = never;
        export type RequestHeaders = {};
        export type ResponseBody = IrcUserSession[];
    }
}

export namespace Variables {
    /**
     * @description Get all variables
     * @tags Variables
     * @name V1VariablesList
     * @request GET:/api/v1/variables
     * @response `200` `object` OK
     */
    export namespace V1VariablesList {
        export type RequestParams = {};
        export type RequestQuery = {};
        export type RequestBody = never;
        export type RequestHeaders = {};
        export type ResponseBody = object;
    }
}

export namespace Auth {
    /**
     * @description Login to the server
     * @tags Auth
     * @name Login
     * @request POST:/api/v1/auth/login
     * @response `200` `LoginResponse` OK
     * @response `400` `void` Bad Request
     * @response `401` `void` Unauthorized
     */
    export namespace Login {
        export type RequestParams = {};
        export type RequestQuery = {};
        export type RequestBody = LoginRequest;
        export type RequestHeaders = {};
        export type ResponseBody = LoginResponse;
    }
}

export namespace Channels {
    /**
     * @description Get all channels
     * @tags Channels
     * @name V1ChannelsListList
     * @request GET:/api/v1/channels/list
     * @response `200` `(ChannelEntry)[]` OK
     */
    export namespace V1ChannelsListList {
        export type RequestParams = {};
        export type RequestQuery = {};
        export type RequestBody = never;
        export type RequestHeaders = {};
        export type ResponseBody = ChannelEntry[];
    }

    /**
     * @description Get a channel by name
     * @tags Channels
     * @name V1ChannelsDetail
     * @request GET:/api/v1/channels/{channel}
     * @response `200` `ChannelData` OK
     * @response `404` `void` Not Found
     */
    export namespace V1ChannelsDetail {
        export type RequestParams = {
            channel: string;
        };
        export type RequestQuery = {};
        export type RequestBody = never;
        export type RequestHeaders = {};
        export type ResponseBody = ChannelData;
    }
}

export namespace Status {
    /**
     * @description Get the health status
     * @tags Status
     * @name V1StatusHealthList
     * @request GET:/api/v1/status/health
     * @response `200` `void` OK
     */
    export namespace V1StatusHealthList {
        export type RequestParams = {};
        export type RequestQuery = {};
        export type RequestBody = never;
        export type RequestHeaders = {};
        export type ResponseBody = void;
    }
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
    /** set parameter to `true` for call `securityWorker` for this request */
    secure?: boolean;
    /** request path */
    path: string;
    /** content type of request body */
    type?: ContentType;
    /** query params */
    query?: QueryParamsType;
    /** format of response (i.e. response.json() -> format: "json") */
    format?: ResponseFormat;
    /** request body */
    body?: unknown;
    /** base url */
    baseUrl?: string;
    /** request cancellation token */
    cancelToken?: CancelToken;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> {
    baseUrl?: string;
    baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
    securityWorker?: (securityData: SecurityDataType | null) => Promise<RequestParams | void> | RequestParams | void;
    customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown> extends Response {
    data: D;
    error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
    Json = "application/json",
    FormData = "multipart/form-data",
    UrlEncoded = "application/x-www-form-urlencoded",
    Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
    public baseUrl: string = "http://127.0.0.1:23021/";
    private securityData: SecurityDataType | null = null;
    private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
    private abortControllers = new Map<CancelToken, AbortController>();
    private customFetch = (...fetchParams: Parameters<typeof fetch>) => fetch(...fetchParams);

    private baseApiParams: RequestParams = {
        credentials: "same-origin",
        headers: {},
        redirect: "follow",
        referrerPolicy: "no-referrer",
    };

    constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
        Object.assign(this, apiConfig);
    }

    public setSecurityData = (data: SecurityDataType | null) => {
        this.securityData = data;
    };

    protected encodeQueryParam(key: string, value: any) {
        const encodedKey = encodeURIComponent(key);
        return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
    }

    protected addQueryParam(query: QueryParamsType, key: string) {
        return this.encodeQueryParam(key, query[key]);
    }

    protected addArrayQueryParam(query: QueryParamsType, key: string) {
        const value = query[key];
        return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
    }

    protected toQueryString(rawQuery?: QueryParamsType): string {
        const query = rawQuery || {};
        const keys = Object.keys(query).filter((key) => "undefined" !== typeof query[key]);
        return keys
            .map((key) => (Array.isArray(query[key]) ? this.addArrayQueryParam(query, key) : this.addQueryParam(query, key)))
            .join("&");
    }

    protected addQueryParams(rawQuery?: QueryParamsType): string {
        const queryString = this.toQueryString(rawQuery);
        return queryString ? `?${queryString}` : "";
    }

    private contentFormatters: Record<ContentType, (input: any) => any> = {
        [ContentType.Json]: (input: any) =>
            input !== null && (typeof input === "object" || typeof input === "string") ? JSON.stringify(input) : input,
        [ContentType.Text]: (input: any) => (input !== null && typeof input !== "string" ? JSON.stringify(input) : input),
        [ContentType.FormData]: (input: any) =>
            Object.keys(input || {}).reduce((formData, key) => {
                const property = input[key];
                formData.append(
                    key,
                    property instanceof Blob
                        ? property
                        : typeof property === "object" && property !== null
                          ? JSON.stringify(property)
                          : `${property}`,
                );
                return formData;
            }, new FormData()),
        [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
    };

    protected mergeRequestParams(params1: RequestParams, params2?: RequestParams): RequestParams {
        return {
            ...this.baseApiParams,
            ...params1,
            ...(params2 || {}),
            headers: {
                ...(this.baseApiParams.headers || {}),
                ...(params1.headers || {}),
                ...((params2 && params2.headers) || {}),
            },
        };
    }

    protected createAbortSignal = (cancelToken: CancelToken): AbortSignal | undefined => {
        if (this.abortControllers.has(cancelToken)) {
            const abortController = this.abortControllers.get(cancelToken);
            if (abortController) {
                return abortController.signal;
            }
            return void 0;
        }

        const abortController = new AbortController();
        this.abortControllers.set(cancelToken, abortController);
        return abortController.signal;
    };

    public abortRequest = (cancelToken: CancelToken) => {
        const abortController = this.abortControllers.get(cancelToken);

        if (abortController) {
            abortController.abort();
            this.abortControllers.delete(cancelToken);
        }
    };

    public request = async <T = any, E = any>({
        body,
        secure,
        path,
        type,
        query,
        format,
        baseUrl,
        cancelToken,
        ...params
    }: FullRequestParams): Promise<T> => {
        const secureParams =
            ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
                this.securityWorker &&
                (await this.securityWorker(this.securityData))) ||
            {};
        const requestParams = this.mergeRequestParams(params, secureParams);
        const queryString = query && this.toQueryString(query);
        const payloadFormatter = this.contentFormatters[type || ContentType.Json];
        const responseFormat = format || requestParams.format;

        return this.customFetch(`${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`, {
            ...requestParams,
            headers: {
                ...(requestParams.headers || {}),
                ...(type && type !== ContentType.FormData ? { "Content-Type": type } : {}),
            },
            signal: (cancelToken ? this.createAbortSignal(cancelToken) : requestParams.signal) || null,
            body: typeof body === "undefined" || body === null ? null : payloadFormatter(body),
        }).then(async (response) => {
            const r = response.clone() as HttpResponse<T, E>;
            r.data = null as unknown as T;
            r.error = null as unknown as E;

            const data = !responseFormat
                ? r
                : await response[responseFormat]()
                      .then((data) => {
                          if (r.ok) {
                              r.data = data;
                          } else {
                              r.error = data;
                          }
                          return r;
                      })
                      .catch((e) => {
                          r.error = e;
                          return r;
                      });

            if (cancelToken) {
                this.abortControllers.delete(cancelToken);
            }

            if (!response.ok) throw data;
            return data.data;
        });
    };
}

/**
 * @title Orion.Server | v1
 * @version 1.0.0
 * @baseUrl http://127.0.0.1:23021/
 */
export class Api<SecurityDataType extends unknown> {
    http: HttpClient<SecurityDataType>;

    constructor(http: HttpClient<SecurityDataType>) {
        this.http = http;
    }

    system = {
        /**
         * @description Get the version information
         *
         * @tags System
         * @name V1SystemVersionList
         * @request GET:/api/v1/system/version
         * @response `200` `VersionInfoData` OK
         */
        v1SystemVersionList: (params: RequestParams = {}) =>
            this.http.request<VersionInfoData, any>({
                path: `/api/v1/system/version`,
                method: "GET",
                format: "json",
                ...params,
            }),

        /**
         * @description Get the sessions
         *
         * @tags System
         * @name V1SystemSessionsList
         * @request GET:/api/v1/system/sessions
         * @response `200` `(IrcUserSession)[]` OK
         */
        v1SystemSessionsList: (params: RequestParams = {}) =>
            this.http.request<IrcUserSession[], any>({
                path: `/api/v1/system/sessions`,
                method: "GET",
                format: "json",
                ...params,
            }),
    };
    variables = {
        /**
         * @description Get all variables
         *
         * @tags Variables
         * @name V1VariablesList
         * @request GET:/api/v1/variables
         * @response `200` `object` OK
         */
        v1VariablesList: (params: RequestParams = {}) =>
            this.http.request<object, any>({
                path: `/api/v1/variables`,
                method: "GET",
                format: "json",
                ...params,
            }),
    };
    auth = {
        /**
         * @description Login to the server
         *
         * @tags Auth
         * @name Login
         * @request POST:/api/v1/auth/login
         * @response `200` `LoginResponse` OK
         * @response `400` `void` Bad Request
         * @response `401` `void` Unauthorized
         */
        login: (data: LoginRequest, params: RequestParams = {}) =>
            this.http.request<LoginResponse, void>({
                path: `/api/v1/auth/login`,
                method: "POST",
                body: data,
                type: ContentType.Json,
                format: "json",
                ...params,
            }),
    };
    channels = {
        /**
         * @description Get all channels
         *
         * @tags Channels
         * @name V1ChannelsListList
         * @request GET:/api/v1/channels/list
         * @response `200` `(ChannelEntry)[]` OK
         */
        v1ChannelsListList: (params: RequestParams = {}) =>
            this.http.request<ChannelEntry[], any>({
                path: `/api/v1/channels/list`,
                method: "GET",
                format: "json",
                ...params,
            }),

        /**
         * @description Get a channel by name
         *
         * @tags Channels
         * @name V1ChannelsDetail
         * @request GET:/api/v1/channels/{channel}
         * @response `200` `ChannelData` OK
         * @response `404` `void` Not Found
         */
        v1ChannelsDetail: (channel: string, params: RequestParams = {}) =>
            this.http.request<ChannelData, void>({
                path: `/api/v1/channels/${channel}`,
                method: "GET",
                format: "json",
                ...params,
            }),
    };
    status = {
        /**
         * @description Get the health status
         *
         * @tags Status
         * @name V1StatusHealthList
         * @request GET:/api/v1/status/health
         * @response `200` `void` OK
         */
        v1StatusHealthList: (params: RequestParams = {}) =>
            this.http.request<void, any>({
                path: `/api/v1/status/health`,
                method: "GET",
                ...params,
            }),
    };
}
