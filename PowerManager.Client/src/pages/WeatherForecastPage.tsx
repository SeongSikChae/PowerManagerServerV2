import * as React from "react";
import { connect } from "react-redux";
import {
  Cloud,
  CloudSun,
  RefreshCw,
  Snowflake,
  Sun,
  Thermometer,
} from "lucide-react";
import { RootState } from "@/store";
import { loadWeatherForecast } from "@/store/weather/actions";
import { WeatherForecast } from "@/lib/api";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

interface StateProps {
  items: WeatherForecast[];
  loading: boolean;
  error: string | null;
  lastFetchedAt: string | null;
}

interface DispatchProps {
  loadWeatherForecast: () => void;
}

type Props = StateProps & DispatchProps;

function formatDate(date: string): string {
  const parsed = new Date(date);
  if (Number.isNaN(parsed.getTime())) {
    return date;
  }
  return parsed.toLocaleDateString("ko-KR", {
    year: "numeric",
    month: "short",
    day: "numeric",
    weekday: "short",
  });
}

function summaryBadgeClass(summary: string | null): string {
  const value = (summary || "").toLowerCase();
  if (value.includes("freezing") || value.includes("bracing") || value.includes("chilly")) {
    return "bg-sb-info/15 text-sb-info";
  }
  if (value.includes("hot") || value.includes("sweltering") || value.includes("scorching")) {
    return "bg-sb-danger/15 text-sb-danger";
  }
  if (value.includes("warm") || value.includes("balmy")) {
    return "bg-sb-warning/20 text-amber-700";
  }
  return "bg-sb-primary/10 text-sb-primary";
}

function average(values: number[]): number | null {
  if (values.length === 0) {
    return null;
  }
  return Math.round(values.reduce((sum, value) => sum + value, 0) / values.length);
}

class WeatherForecastPage extends React.Component<Props> {
  componentDidMount() {
    this.props.loadWeatherForecast();
  }

  render() {
    const { items, loading, error, lastFetchedAt } = this.props;
    const tempsC = items.map((item) => item.temperatureC);
    const avgC = average(tempsC);
    const maxC = tempsC.length ? Math.max(...tempsC) : null;
    const minC = tempsC.length ? Math.min(...tempsC) : null;

    return (
      <div className="space-y-6">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-2xl font-extrabold text-sb-dark">Weather Forecast</h1>
            <p className="mt-1 text-sm text-muted-foreground">
              `/rest/weatherforecast` 응답을 SB Admin 2 스타일로 표시합니다.
            </p>
          </div>
          <Button
            onClick={() => this.props.loadWeatherForecast()}
            disabled={loading}
          >
            <RefreshCw className={cn("h-4 w-4", loading && "animate-spin")} />
            새로고침
          </Button>
        </div>

        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <StatCard
            title="예보 일수"
            value={loading ? "..." : String(items.length)}
            icon={<Cloud className="h-8 w-8" />}
            accent="border-l-sb-primary"
            iconColor="text-sb-primary"
          />
          <StatCard
            title="평균 온도 (°C)"
            value={loading || avgC === null ? "..." : `${avgC}°`}
            icon={<Thermometer className="h-8 w-8" />}
            accent="border-l-sb-success"
            iconColor="text-sb-success"
          />
          <StatCard
            title="최고 온도 (°C)"
            value={loading || maxC === null ? "..." : `${maxC}°`}
            icon={<Sun className="h-8 w-8" />}
            accent="border-l-sb-warning"
            iconColor="text-sb-warning"
          />
          <StatCard
            title="최저 온도 (°C)"
            value={loading || minC === null ? "..." : `${minC}°`}
            icon={<Snowflake className="h-8 w-8" />}
            accent="border-l-sb-info"
            iconColor="text-sb-info"
          />
        </div>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <CloudSun className="h-4 w-4" />
              Forecast Data
            </CardTitle>
            <span className="text-xs text-muted-foreground">
              {lastFetchedAt
                ? `마지막 갱신: ${new Date(lastFetchedAt).toLocaleString("ko-KR")}`
                : "아직 불러오지 않음"}
            </span>
          </CardHeader>
          <CardContent className="p-0">
            {error ? (
              <div className="px-5 py-8 text-center text-sm text-sb-danger">
                {error}
              </div>
            ) : loading && items.length === 0 ? (
              <div className="px-5 py-8 text-center text-sm text-muted-foreground">
                날씨 예보를 불러오는 중...
              </div>
            ) : items.length === 0 ? (
              <div className="px-5 py-8 text-center text-sm text-muted-foreground">
                표시할 예보가 없습니다.
              </div>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow className="bg-sb-light/60 hover:bg-sb-light/60">
                    <TableHead>날짜</TableHead>
                    <TableHead>°C</TableHead>
                    <TableHead>°F</TableHead>
                    <TableHead>요약</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {items.map((item) => (
                    <TableRow key={`${item.date}-${item.temperatureC}-${item.summary}`}>
                      <TableCell className="font-semibold">{formatDate(item.date)}</TableCell>
                      <TableCell>{item.temperatureC}</TableCell>
                      <TableCell>{item.temperatureF}</TableCell>
                      <TableCell>
                        <Badge className={summaryBadgeClass(item.summary)}>
                          {item.summary || "-"}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </CardContent>
        </Card>
      </div>
    );
  }
}

interface StatCardProps {
  title: string;
  value: string;
  icon: React.ReactNode;
  accent: string;
  iconColor: string;
}

function StatCard({ title, value, icon, accent, iconColor }: StatCardProps) {
  return (
    <Card className={cn("border-l-4", accent)}>
      <CardContent className="flex items-center justify-between py-4">
        <div>
          <div className="text-xs font-bold uppercase tracking-wide text-sb-primary">
            {title}
          </div>
          <div className="mt-1 text-2xl font-extrabold text-sb-dark">{value}</div>
        </div>
        <div className={cn("opacity-40", iconColor)}>{icon}</div>
      </CardContent>
    </Card>
  );
}

function mapStateToProps(state: RootState): StateProps {
  return {
    items: state.weather.items,
    loading: state.weather.loading,
    error: state.weather.error,
    lastFetchedAt: state.weather.lastFetchedAt,
  };
}

function mapDispatchToProps(dispatch: any): DispatchProps {
  return {
    loadWeatherForecast: () => dispatch(loadWeatherForecast()),
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(WeatherForecastPage);
