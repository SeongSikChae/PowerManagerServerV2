import { WeatherForecast } from "@/lib/api";

export const FETCH_WEATHER_REQUEST = "weather/FETCH_REQUEST";
export const FETCH_WEATHER_SUCCESS = "weather/FETCH_SUCCESS";
export const FETCH_WEATHER_FAILURE = "weather/FETCH_FAILURE";

export interface WeatherState {
  items: WeatherForecast[];
  loading: boolean;
  error: string | null;
  lastFetchedAt: string | null;
}

export type WeatherAction =
  | { type: typeof FETCH_WEATHER_REQUEST }
  | { type: typeof FETCH_WEATHER_SUCCESS; payload: WeatherForecast[] }
  | { type: typeof FETCH_WEATHER_FAILURE; payload: string };
