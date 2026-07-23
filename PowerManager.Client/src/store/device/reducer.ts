import {
  FETCH_DEVICE_FAILURE,
  FETCH_DEVICE_REQUEST,
  FETCH_DEVICE_SUCCESS,
  SET_DEVICE_ITEM_SIZE,
  SET_DEVICE_PAGE,
  SET_DEVICE_QUERY,
  DEFAULT_DEVICE_ITEM_SIZE,
  DeviceAction,
  DeviceState,
} from "./types";

const initialState: DeviceState = {
  items: [],
  loading: false,
  error: null,
  query: "",
  currentPage: 1,
  itemSize: DEFAULT_DEVICE_ITEM_SIZE,
  totalCount: 0,
};

export function deviceReducer(
  state: DeviceState = initialState,
  action: DeviceAction
): DeviceState {
  switch (action.type) {
    case FETCH_DEVICE_REQUEST:
      return {
        ...state,
        loading: true,
        error: null,
      };
    case FETCH_DEVICE_SUCCESS:
      return {
        ...state,
        loading: false,
        items: action.payload.items,
        currentPage: action.payload.pagination.currentPage,
        itemSize: action.payload.pagination.itemSize,
        totalCount: action.payload.pagination.totalCount,
        error: null,
      };
    case FETCH_DEVICE_FAILURE:
      return {
        ...state,
        loading: false,
        error: action.payload,
      };
    case SET_DEVICE_QUERY:
      return {
        ...state,
        query: action.payload,
        currentPage: 1,
      };
    case SET_DEVICE_PAGE:
      return {
        ...state,
        currentPage: action.payload,
      };
    case SET_DEVICE_ITEM_SIZE:
      return {
        ...state,
        itemSize: action.payload,
        currentPage: 1,
      };
    default:
      return state;
  }
}
