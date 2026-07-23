import { ElectricPower } from "@/lib/api";

export const FETCH_ELECTRIC_POWER_REQUEST = "electricPower/FETCH_REQUEST";
export const FETCH_ELECTRIC_POWER_SUCCESS = "electricPower/FETCH_SUCCESS";
export const FETCH_ELECTRIC_POWER_FAILURE = "electricPower/FETCH_FAILURE";

export interface ElectricPowerState {
  items: ElectricPower[];
  loading: boolean;
  error: string | null;
}

export type ElectricPowerAction =
  | { type: typeof FETCH_ELECTRIC_POWER_REQUEST }
  | { type: typeof FETCH_ELECTRIC_POWER_SUCCESS; payload: ElectricPower[] }
  | { type: typeof FETCH_ELECTRIC_POWER_FAILURE; payload: string };
