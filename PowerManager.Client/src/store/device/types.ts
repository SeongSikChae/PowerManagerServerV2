import { Device, Pagination } from "@/lib/api";

export const FETCH_DEVICE_REQUEST = "device/FETCH_REQUEST";
export const FETCH_DEVICE_SUCCESS = "device/FETCH_SUCCESS";
export const FETCH_DEVICE_FAILURE = "device/FETCH_FAILURE";
export const SET_DEVICE_QUERY = "device/SET_QUERY";
export const SET_DEVICE_PAGE = "device/SET_PAGE";
export const SET_DEVICE_ITEM_SIZE = "device/SET_ITEM_SIZE";

export const DEFAULT_DEVICE_ITEM_SIZE = 10;

export interface DeviceState {
  items: Device[];
  loading: boolean;
  error: string | null;
  query: string;
  currentPage: number;
  itemSize: number;
  totalCount: number;
}

export type DeviceAction =
  | { type: typeof FETCH_DEVICE_REQUEST }
  | {
      type: typeof FETCH_DEVICE_SUCCESS;
      payload: { items: Device[]; pagination: Pagination };
    }
  | { type: typeof FETCH_DEVICE_FAILURE; payload: string }
  | { type: typeof SET_DEVICE_QUERY; payload: string }
  | { type: typeof SET_DEVICE_PAGE; payload: number }
  | { type: typeof SET_DEVICE_ITEM_SIZE; payload: number };
