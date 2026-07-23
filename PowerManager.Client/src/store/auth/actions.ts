import { Dispatch } from "redux";
import {
  CertificateInfo,
  escalateCertificate,
  fetchWhoami,
  updateCertificate,
} from "@/lib/api";
import {
  AuthAction,
  CLEAR_OPERATION_ERROR,
  ESCALATE_CERTIFICATE_FAILURE,
  ESCALATE_CERTIFICATE_REQUEST,
  ESCALATE_CERTIFICATE_SUCCESS,
  FETCH_WHOAMI_FAILURE,
  FETCH_WHOAMI_REQUEST,
  FETCH_WHOAMI_SUCCESS,
  UPDATE_CERTIFICATE_FAILURE,
  UPDATE_CERTIFICATE_REQUEST,
  UPDATE_CERTIFICATE_SUCCESS,
} from "./types";

export function fetchWhoamiRequest(): AuthAction {
  return { type: FETCH_WHOAMI_REQUEST };
}

export function fetchWhoamiSuccess(certificate: CertificateInfo): AuthAction {
  return { type: FETCH_WHOAMI_SUCCESS, payload: certificate };
}

export function fetchWhoamiFailure(error: string): AuthAction {
  return { type: FETCH_WHOAMI_FAILURE, payload: error };
}

export function updateCertificateRequest(): AuthAction {
  return { type: UPDATE_CERTIFICATE_REQUEST };
}

export function updateCertificateSuccess(): AuthAction {
  return { type: UPDATE_CERTIFICATE_SUCCESS };
}

export function updateCertificateFailure(error: string): AuthAction {
  return { type: UPDATE_CERTIFICATE_FAILURE, payload: error };
}

export function escalateCertificateRequest(): AuthAction {
  return { type: ESCALATE_CERTIFICATE_REQUEST };
}

export function escalateCertificateSuccess(): AuthAction {
  return { type: ESCALATE_CERTIFICATE_SUCCESS };
}

export function escalateCertificateFailure(error: string): AuthAction {
  return { type: ESCALATE_CERTIFICATE_FAILURE, payload: error };
}

export function clearOperationError(): AuthAction {
  return { type: CLEAR_OPERATION_ERROR };
}

export function loadWhoami() {
  return (dispatch: Dispatch<AuthAction>) => {
    dispatch(fetchWhoamiRequest());

    return fetchWhoami()
      .then((certificate) => {
        dispatch(fetchWhoamiSuccess(certificate));
      })
      .catch((error: unknown) => {
        const message =
          error instanceof Error
            ? error.message
            : "알 수 없는 오류가 발생했습니다.";
        dispatch(fetchWhoamiFailure(message));
      });
  };
}

export function renewCertificate(file: File) {
  return (dispatch: Dispatch<AuthAction>) => {
    dispatch(updateCertificateRequest());

    return updateCertificate(file)
      .then(() => {
        dispatch(updateCertificateSuccess());
      })
      .catch((error: unknown) => {
        const message =
          error instanceof Error
            ? error.message
            : "알 수 없는 오류가 발생했습니다.";
        dispatch(updateCertificateFailure(message));
      });
  };
}

export function transferExpiredCertificate(file: File) {
  return (dispatch: Dispatch<AuthAction>) => {
    dispatch(escalateCertificateRequest());

    return escalateCertificate(file)
      .then(() => {
        dispatch(escalateCertificateSuccess());
      })
      .catch((error: unknown) => {
        const message =
          error instanceof Error
            ? error.message
            : "알 수 없는 오류가 발생했습니다.";
        dispatch(escalateCertificateFailure(message));
      });
  };
}
