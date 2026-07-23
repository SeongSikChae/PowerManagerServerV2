import { Dispatch } from "redux";
import { fetchWeatherForecast, WeatherForecast } from "@/lib/api";
import {
  FETCH_WEATHER_FAILURE,
  FETCH_WEATHER_REQUEST,
  FETCH_WEATHER_SUCCESS,
  WeatherAction,
} from "./types";

export function fetchWeatherRequest(): WeatherAction {
  return { type: FETCH_WEATHER_REQUEST };
}

export function fetchWeatherSuccess(items: WeatherForecast[]): WeatherAction {
  return { type: FETCH_WEATHER_SUCCESS, payload: items };
}

export function fetchWeatherFailure(error: string): WeatherAction {
  return { type: FETCH_WEATHER_FAILURE, payload: error };
}

export function loadWeatherForecast() {
  return (dispatch: Dispatch<WeatherAction>) => {
    dispatch(fetchWeatherRequest());

    return fetchWeatherForecast()
      .then((items) => {
        dispatch(fetchWeatherSuccess(items));
      })
      .catch((error: unknown) => {
        const message =
          error instanceof Error
            ? error.message
            : "알 수 없는 오류가 발생했습니다.";
        dispatch(fetchWeatherFailure(message));
      });
  };
}
