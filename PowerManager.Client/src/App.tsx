import * as React from "react";
import { Footer, Sidebar, Topbar } from "@/components/layout/AdminLayout";
import WeatherForecastPage from "@/pages/WeatherForecastPage";
import DevicesPage from "@/pages/DevicesPage";
import PlaceholderPage from "@/pages/PlaceholderPage";
import { PageKey } from "@/navigation/types";

interface AppState {
  sidebarOpen: boolean;
  activePage: PageKey;
}

export default class App extends React.Component<{}, AppState> {
  state: AppState = {
    sidebarOpen: false,
    activePage: "dashboard",
  };

  toggleSidebar = () => {
    this.setState((prev) => ({ sidebarOpen: !prev.sidebarOpen }));
  };

  closeSidebar = () => {
    this.setState({ sidebarOpen: false });
  };

  navigate = (page: PageKey) => {
    this.setState({ activePage: page });
  };

  renderPage() {
    const { activePage } = this.state;

    switch (activePage) {
      case "weather":
        return <WeatherForecastPage />;
      case "dashboard":
        return (
          <PlaceholderPage
            title="대시보드"
            description="기기 상태를 모니터링 하고 제어할 수 있습니다."
          />
        );
      case "devices":
        return <DevicesPage />;
      case "messenger":
        return (
          <PlaceholderPage
            title="메신저 관리"
            description="텔레그램 및 WebPush 알림을 관리합니다."
          />
        );
      case "mqtt-connector":
        return (
          <PlaceholderPage
            title="MQTT Connector 관리"
            description="Forward 용 MQTT Connector를 관리합니다."
          />
        );
      default:
        return <PlaceholderPage title="페이지 없음" />;
    }
  }

  render() {
    const { sidebarOpen, activePage } = this.state;

    return (
      <div className="flex min-h-full">
        <Sidebar
          open={sidebarOpen}
          activePage={activePage}
          onClose={this.closeSidebar}
          onNavigate={this.navigate}
        />
        <div className="flex min-h-full min-w-0 flex-1 flex-col bg-sb-light">
          <Topbar onMenuClick={this.toggleSidebar} />
          <main className="flex-1 p-4 sm:p-6">{this.renderPage()}</main>
          <Footer />
        </div>
      </div>
    );
  }
}
