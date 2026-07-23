export type PageKey =
  | "dashboard"
  | "weather"
  | "devices"
  | "messenger"
  | "mqtt-connector";

export interface NavItem {
  key: PageKey;
  label: string;
}

export interface NavGroup {
  key: string;
  label: string;
  children: NavItem[];
}

export const PAGE_TITLES: Record<PageKey, string> = {
  dashboard: "대시보드",
  weather: "Weather Forecast",
  devices: "기기관리",
  messenger: "메신저 관리",
  "mqtt-connector": "MQTT Connector 관리",
};
