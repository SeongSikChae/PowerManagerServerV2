import { Dispatch } from "redux";
import { fetchDeviceList, Device, Pagination } from "@/lib/api";
import {
  FETCH_DEVICE_FAILURE,
  FETCH_DEVICE_REQUEST,
  FETCH_DEVICE_SUCCESS,
  SET_DEVICE_ITEM_SIZE,
  SET_DEVICE_PAGE,
  SET_DEVICE_QUERY,
  DeviceAction,
  DeviceState,
} from "./types";

type GetDeviceState = () => { device: DeviceState };

export function fetchDeviceRequest(): DeviceAction {
  return { type: FETCH_DEVICE_REQUEST };
}

export function fetchDeviceSuccess(
  items: Device[],
  pagination: Pagination
): DeviceAction {
  return { type: FETCH_DEVICE_SUCCESS, payload: { items, pagination } };
}

export function fetchDeviceFailure(error: string): DeviceAction {
  return { type: FETCH_DEVICE_FAILURE, payload: error };
}

export function setDeviceQuery(query: string): DeviceAction {
  return { type: SET_DEVICE_QUERY, payload: query };
}

export function setDevicePage(page: number): DeviceAction {
  return { type: SET_DEVICE_PAGE, payload: page };
}

export function setDeviceItemSize(itemSize: number): DeviceAction {
  return { type: SET_DEVICE_ITEM_SIZE, payload: itemSize };
}

function fetchDevices(
  dispatch: Dispatch<DeviceAction>,
  getState: GetDeviceState
) {
  const { query, currentPage, itemSize } = getState().device;
  dispatch(fetchDeviceRequest());

  return fetchDeviceList({
    query: query.trim() || null,
    pagination: {
      currentPage,
      itemSize,
      totalCount: 0,
    },
  })
    .then((result) => {
      dispatch(fetchDeviceSuccess(result.items, result.pagination));
    })
    .catch((error: unknown) => {
      const message =
        error instanceof Error
          ? error.message
          : "알 수 없는 오류가 발생했습니다.";
      dispatch(fetchDeviceFailure(message));
    });
}

export function loadDevices() {
  return (dispatch: Dispatch<DeviceAction>, getState: GetDeviceState) => {
    return fetchDevices(dispatch, getState);
  };
}

export function searchDevices(query: string) {
  return (dispatch: Dispatch<DeviceAction>, getState: GetDeviceState) => {
    dispatch(setDeviceQuery(query));
    return fetchDevices(dispatch, getState);
  };
}

export function changeDevicePage(page: number) {
  return (dispatch: Dispatch<DeviceAction>, getState: GetDeviceState) => {
    dispatch(setDevicePage(page));
    return fetchDevices(dispatch, getState);
  };
}

export function changeDeviceItemSize(itemSize: number) {
  return (dispatch: Dispatch<DeviceAction>, getState: GetDeviceState) => {
    dispatch(setDeviceItemSize(itemSize));
    return fetchDevices(dispatch, getState);
  };
}
