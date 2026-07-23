import * as React from "react";
import { connect } from "react-redux";
import {
  ChevronDown,
  CloudSun,
  Cpu,
  LayoutDashboard,
  Menu,
  MessageSquare,
  Radio,
  Settings,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { PageKey } from "@/navigation/types";
import { RootState } from "@/store";
import UserMenu from "@/components/layout/UserMenu";

interface SidebarOwnProps {
  open: boolean;
  activePage: PageKey;
  onClose: () => void;
  onNavigate: (page: PageKey) => void;
}

interface SidebarStateProps {
  authenticated: boolean;
}

type SidebarProps = SidebarOwnProps & SidebarStateProps;

interface SidebarState {
  settingsOpen: boolean;
}

class SidebarView extends React.Component<SidebarProps, SidebarState> {
  state: SidebarState = {
    settingsOpen: true,
  };

  componentDidUpdate() {
    const settingsPages: PageKey[] = ["devices", "messenger", "mqtt-connector"];
    if (
      !this.props.authenticated &&
      settingsPages.indexOf(this.props.activePage) !== -1
    ) {
      this.props.onNavigate("dashboard");
    }
  }

  toggleSettings = () => {
    this.setState((prev) => ({ settingsOpen: !prev.settingsOpen }));
  };

  handleNavigate = (page: PageKey) => {
    this.props.onNavigate(page);
    this.props.onClose();
  };

  render() {
    const { open, activePage, authenticated } = this.props;
    const { settingsOpen } = this.state;
    const settingsActive =
      activePage === "devices" ||
      activePage === "messenger" ||
      activePage === "mqtt-connector";

    return (
      <>
        <div
          className={cn(
            "fixed inset-0 z-30 bg-black/40 transition-opacity lg:hidden",
            open ? "opacity-100" : "pointer-events-none opacity-0"
          )}
          onClick={this.props.onClose}
        />
        <aside
          className={cn(
            "z-40 flex shrink-0 flex-col bg-gradient-to-b from-sb-primary to-sb-sidebar text-white shadow-lg transition-all duration-200",
            "fixed inset-y-0 left-0 w-[16.8rem]",
            "lg:static lg:h-auto",
            open
              ? "translate-x-0"
              : "-translate-x-full lg:w-0 lg:overflow-hidden lg:translate-x-0 lg:border-0 lg:shadow-none"
          )}
        >
          <div
            className={cn(
              "flex h-16 items-center px-4",
              !open && "lg:invisible lg:opacity-0"
            )}
          >
            <div className="flex items-center gap-2 font-extrabold tracking-wide">
              <CloudSun className="h-5 w-5 shrink-0" />
              <span className="whitespace-nowrap">PowerManager</span>
            </div>
          </div>

          <hr
            className={cn(
              "mx-4 border-white/20",
              !open && "lg:invisible lg:opacity-0"
            )}
          />

          <nav
            className={cn(
              "mt-3 flex-1 space-y-4 overflow-y-auto px-3 pb-4",
              !open && "lg:invisible lg:opacity-0"
            )}
          >
            <div>
              <div className="mb-2.5 px-2 text-[0.78rem] font-bold uppercase tracking-widest text-white/60">
                Monitoring
              </div>
              <NavButton
                active={activePage === "dashboard"}
                icon={<LayoutDashboard className="h-5 w-5 shrink-0" />}
                label="대시보드"
                onClick={() => this.handleNavigate("dashboard")}
              />
            </div>

            {authenticated && (
              <div>
                <div className="mb-2.5 px-2 text-[0.78rem] font-bold uppercase tracking-widest text-white/60">
                  Manager
                </div>
                <button
                  type="button"
                  className={cn(
                    "flex w-full items-center gap-2.5 rounded-md px-3.5 py-3 text-[1.05rem] font-semibold whitespace-nowrap transition-colors",
                    settingsActive ? "bg-white/15" : "hover:bg-white/10"
                  )}
                  onClick={this.toggleSettings}
                >
                  <Settings className="h-5 w-5 shrink-0" />
                  <span className="flex-1 text-left">설정</span>
                  <ChevronDown
                    className={cn(
                      "h-5 w-5 shrink-0 transition-transform",
                      settingsOpen && "rotate-180"
                    )}
                  />
                </button>
                {settingsOpen && (
                  <div className="mt-1.5 space-y-1 border-l border-white/20 ml-4 pl-2">
                    <NavButton
                      active={activePage === "devices"}
                      icon={<Cpu className="h-5 w-5 shrink-0" />}
                      label="기기관리"
                      nested
                      onClick={() => this.handleNavigate("devices")}
                    />
                    <NavButton
                      active={activePage === "messenger"}
                      icon={<MessageSquare className="h-5 w-5 shrink-0" />}
                      label="메신저 관리"
                      nested
                      onClick={() => this.handleNavigate("messenger")}
                    />
                    <NavButton
                      active={activePage === "mqtt-connector"}
                      icon={<Radio className="h-5 w-5 shrink-0" />}
                      label="MQTT Connector 관리"
                      nested
                      onClick={() => this.handleNavigate("mqtt-connector")}
                    />
                  </div>
                )}
              </div>
            )}

            <div>
              <div className="mb-2.5 px-2 text-[0.78rem] font-bold uppercase tracking-widest text-white/60">
                ETC
              </div>
              <NavButton
                active={activePage === "weather"}
                icon={<CloudSun className="h-5 w-5 shrink-0" />}
                label="Weather Forecast"
                onClick={() => this.handleNavigate("weather")}
              />
            </div>
          </nav>

          <div
            className={cn(
              "border-t border-white/15 p-4 text-xs text-white/70 whitespace-nowrap",
              !open && "lg:invisible lg:opacity-0"
            )}
          >
            SB Admin 2 Style
          </div>
        </aside>
      </>
    );
  }
}

function mapSidebarState(state: RootState): SidebarStateProps {
  return {
    authenticated: Boolean(state.auth.certificate?.email?.trim()),
  };
}

export const Sidebar = connect(mapSidebarState)(SidebarView);

interface NavButtonProps {
  active: boolean;
  icon: React.ReactNode;
  label: string;
  nested?: boolean;
  onClick: () => void;
}

function NavButton({ active, icon, label, nested, onClick }: NavButtonProps) {
  return (
    <button
      type="button"
      className={cn(
        "flex w-full items-center gap-2.5 rounded-md text-left text-[1.05rem] font-semibold whitespace-nowrap transition-colors",
        nested ? "px-3 py-2.5" : "px-3.5 py-3",
        active ? "bg-white/20" : "hover:bg-white/10"
      )}
      onClick={onClick}
    >
      {icon}
      <span className={cn(nested && "text-[0.975rem]")}>{label}</span>
    </button>
  );
}

interface TopbarProps {
  onMenuClick: () => void;
}

export function Topbar({ onMenuClick }: TopbarProps) {
  return (
    <header className="sticky top-0 z-20 flex h-16 items-center justify-between border-b border-sb-border bg-card px-4 shadow-[0_0.15rem_1.75rem_0_rgba(58,59,69,0.15)]">
      <button
        type="button"
        className="rounded-md p-2 text-sb-secondary hover:bg-sb-light"
        onClick={onMenuClick}
        aria-label="사이드바 토글"
      >
        <Menu className="h-5 w-5" />
      </button>
      <div className="ml-auto flex items-center">
        <UserMenu />
      </div>
    </header>
  );
}

export function Footer() {
  return (
    <footer className="mt-auto border-t border-sb-border bg-card py-4 text-center text-sm text-muted-foreground">
      Copyright &copy; PowerManager {new Date().getFullYear()}
    </footer>
  );
}
