import {
  FETCH_WEATHER_FAILURE,
  FETCH_WEATHER_REQUEST,
  FETCH_WEATHER_SUCCESS,
  WeatherAction,
  WeatherState,
} from "./types";

const initialState: WeatherState = {
  items: [],
  loading: false,
  error: null,
  lastFetchedAt: null,
};

export function weatherReducer(
  state: WeatherState = initialState,
  action: WeatherAction
): WeatherState {
  switch (action.type) {
    case FETCH_WEATHER_REQUEST:
      return {
        ...state,
        loading: true,
        error: null,
      };
    case FETCH_WEATHER_SUCCESS:
      return {
        ...state,
        loading: false,
        items: action.payload,
        error: null,
        lastFetchedAt: new Date().toISOString(),
      };
    case FETCH_WEATHER_FAILURE:
      return {
        ...state,
        loading: false,
        error: action.payload,
      };
    default:
      return state;
  }
}
