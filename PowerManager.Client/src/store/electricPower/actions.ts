import { Dispatch } from "redux";
import { fetchElectricPowerList, ElectricPower } from "@/lib/api";
import {
  FETCH_ELECTRIC_POWER_FAILURE,
  FETCH_ELECTRIC_POWER_REQUEST,
  FETCH_ELECTRIC_POWER_SUCCESS,
  ElectricPowerAction,
} from "./types";

export function fetchElectricPowerRequest(): ElectricPowerAction {
  return { type: FETCH_ELECTRIC_POWER_REQUEST };
}

export function fetchElectricPowerSuccess(
  items: ElectricPower[]
): ElectricPowerAction {
  return { type: FETCH_ELECTRIC_POWER_SUCCESS, payload: items };
}

export function fetchElectricPowerFailure(error: string): ElectricPowerAction {
  return { type: FETCH_ELECTRIC_POWER_FAILURE, payload: error };
}

export function loadElectricPowers() {
  return (dispatch: Dispatch<ElectricPowerAction>) => {
    dispatch(fetchElectricPowerRequest());

    return fetchElectricPowerList()
      .then((items) => {
        dispatch(fetchElectricPowerSuccess(items));
      })
      .catch((error: unknown) => {
        const message =
          error instanceof Error
            ? error.message
            : "알 수 없는 오류가 발생했습니다.";
        dispatch(fetchElectricPowerFailure(message));
      });
  };
}
