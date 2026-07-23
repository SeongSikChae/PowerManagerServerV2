import {
  AuthAction,
  AuthState,
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

const initialState: AuthState = {
  certificate: null,
  loading: false,
  updating: false,
  escalating: false,
  error: null,
  operationError: null,
};

export function authReducer(
  state: AuthState = initialState,
  action: AuthAction
): AuthState {
  switch (action.type) {
    case FETCH_WHOAMI_REQUEST:
      return {
        ...state,
        loading: true,
        error: null,
      };
    case FETCH_WHOAMI_SUCCESS:
      return {
        ...state,
        loading: false,
        certificate: action.payload,
        error: null,
      };
    case FETCH_WHOAMI_FAILURE:
      return {
        ...state,
        loading: false,
        error: action.payload,
      };
    case UPDATE_CERTIFICATE_REQUEST:
      return {
        ...state,
        updating: true,
        operationError: null,
      };
    case UPDATE_CERTIFICATE_SUCCESS:
      return {
        ...state,
        updating: false,
        operationError: null,
      };
    case UPDATE_CERTIFICATE_FAILURE:
      return {
        ...state,
        updating: false,
        operationError: action.payload,
      };
    case ESCALATE_CERTIFICATE_REQUEST:
      return {
        ...state,
        escalating: true,
        operationError: null,
      };
    case ESCALATE_CERTIFICATE_SUCCESS:
      return {
        ...state,
        escalating: false,
        operationError: null,
      };
    case ESCALATE_CERTIFICATE_FAILURE:
      return {
        ...state,
        escalating: false,
        operationError: action.payload,
      };
    case CLEAR_OPERATION_ERROR:
      return {
        ...state,
        operationError: null,
      };
    default:
      return state;
  }
}
