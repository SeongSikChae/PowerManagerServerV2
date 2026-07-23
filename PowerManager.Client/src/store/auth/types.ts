import { CertificateInfo } from "@/lib/api";

export const FETCH_WHOAMI_REQUEST = "auth/FETCH_WHOAMI_REQUEST";
export const FETCH_WHOAMI_SUCCESS = "auth/FETCH_WHOAMI_SUCCESS";
export const FETCH_WHOAMI_FAILURE = "auth/FETCH_WHOAMI_FAILURE";

export const UPDATE_CERTIFICATE_REQUEST = "auth/UPDATE_CERTIFICATE_REQUEST";
export const UPDATE_CERTIFICATE_SUCCESS = "auth/UPDATE_CERTIFICATE_SUCCESS";
export const UPDATE_CERTIFICATE_FAILURE = "auth/UPDATE_CERTIFICATE_FAILURE";

export const ESCALATE_CERTIFICATE_REQUEST = "auth/ESCALATE_CERTIFICATE_REQUEST";
export const ESCALATE_CERTIFICATE_SUCCESS = "auth/ESCALATE_CERTIFICATE_SUCCESS";
export const ESCALATE_CERTIFICATE_FAILURE = "auth/ESCALATE_CERTIFICATE_FAILURE";

export const CLEAR_OPERATION_ERROR = "auth/CLEAR_OPERATION_ERROR";

export interface AuthState {
  certificate: CertificateInfo | null;
  loading: boolean;
  updating: boolean;
  escalating: boolean;
  error: string | null;
  operationError: string | null;
}

export type AuthAction =
  | { type: typeof FETCH_WHOAMI_REQUEST }
  | { type: typeof FETCH_WHOAMI_SUCCESS; payload: CertificateInfo }
  | { type: typeof FETCH_WHOAMI_FAILURE; payload: string }
  | { type: typeof UPDATE_CERTIFICATE_REQUEST }
  | { type: typeof UPDATE_CERTIFICATE_SUCCESS }
  | { type: typeof UPDATE_CERTIFICATE_FAILURE; payload: string }
  | { type: typeof ESCALATE_CERTIFICATE_REQUEST }
  | { type: typeof ESCALATE_CERTIFICATE_SUCCESS }
  | { type: typeof ESCALATE_CERTIFICATE_FAILURE; payload: string }
  | { type: typeof CLEAR_OPERATION_ERROR };
