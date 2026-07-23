import {
  FETCH_ELECTRIC_POWER_FAILURE,
  FETCH_ELECTRIC_POWER_REQUEST,
  FETCH_ELECTRIC_POWER_SUCCESS,
  ElectricPowerAction,
  ElectricPowerState,
} from "./types";

const initialState: ElectricPowerState = {
  items: [],
  loading: false,
  error: null,
};

export function electricPowerReducer(
  state: ElectricPowerState = initialState,
  action: ElectricPowerAction
): ElectricPowerState {
  switch (action.type) {
    case FETCH_ELECTRIC_POWER_REQUEST:
      return {
        ...state,
        loading: true,
        error: null,
      };
    case FETCH_ELECTRIC_POWER_SUCCESS:
      return {
        ...state,
        loading: false,
        items: action.payload,
        error: null,
      };
    case FETCH_ELECTRIC_POWER_FAILURE:
      return {
        ...state,
        loading: false,
        error: action.payload,
      };
    default:
      return state;
  }
}
