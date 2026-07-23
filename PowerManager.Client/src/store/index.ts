import { applyMiddleware, combineReducers, createStore } from "redux";
import thunk from "redux-thunk";
import { authReducer } from "./auth/reducer";
import { AuthState } from "./auth/types";
import { deviceReducer } from "./device/reducer";
import { DeviceState } from "./device/types";
import { electricPowerReducer } from "./electricPower/reducer";
import { ElectricPowerState } from "./electricPower/types";
import { weatherReducer } from "./weather/reducer";
import { WeatherState } from "./weather/types";

export interface RootState {
  auth: AuthState;
  device: DeviceState;
  electricPower: ElectricPowerState;
  weather: WeatherState;
}

const rootReducer = combineReducers<RootState>({
  auth: authReducer,
  device: deviceReducer,
  electricPower: electricPowerReducer,
  weather: weatherReducer,
});

export const store = createStore(rootReducer, applyMiddleware(thunk));

export type AppStore = typeof store;
