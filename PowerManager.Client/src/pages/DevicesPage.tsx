import * as React from "react";
import { connect } from "react-redux";
import { ChevronLeft, ChevronRight, Cpu, Plus, Search } from "lucide-react";
import { RootState } from "@/store";
import {
  changeDeviceItemSize,
  changeDevicePage,
  loadDevices,
  searchDevices,
} from "@/store/device/actions";
import { loadElectricPowers } from "@/store/electricPower/actions";
import { Device, ElectricPower } from "@/lib/api";
import { resolvePowerTypeName } from "@/lib/electricPower";
import {
  DeviceFormModal,
  DeviceFormMode,
} from "@/components/layout/DeviceFormModal";
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

const ITEM_SIZE_OPTIONS = [10, 20, 50];

interface StateProps {
  items: Device[];
  loading: boolean;
  error: string | null;
  query: string;
  currentPage: number;
  itemSize: number;
  totalCount: number;
  powerTypes: ElectricPower[];
}

interface DispatchProps {
  loadDevices: () => void;
  loadElectricPowers: () => void;
  searchDevices: (query: string) => void;
  changeDevicePage: (page: number) => void;
  changeDeviceItemSize: (itemSize: number) => void;
}

type Props = StateProps & DispatchProps;

interface LocalState {
  searchInput: string;
  selectedDevice: Device | null;
  formOpen: boolean;
  formMode: DeviceFormMode;
}

function buildPageNumbers(currentPage: number, totalPages: number): number[] {
  if (totalPages <= 0) {
    return [];
  }

  const windowSize = 5;
  let start = Math.max(1, currentPage - Math.floor(windowSize / 2));
  let end = Math.min(totalPages, start + windowSize - 1);
  start = Math.max(1, end - windowSize + 1);

  const pages: number[] = [];
  for (let page = start; page <= end; page += 1) {
    pages.push(page);
  }
  return pages;
}

class DevicesPage extends React.Component<Props, LocalState> {
  state: LocalState = {
    searchInput: this.props.query,
    selectedDevice: null,
    formOpen: false,
    formMode: "create",
  };

  componentDidMount() {
    this.props.loadDevices();
    this.props.loadElectricPowers();
  }

  handleSearchSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    this.props.searchDevices(this.state.searchInput);
  };

  handleItemSizeChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    this.props.changeDeviceItemSize(Number(event.target.value));
  };

  openCreate = () => {
    this.setState({
      formOpen: true,
      formMode: "create",
      selectedDevice: null,
    });
  };

  openEdit = (device: Device) => {
    this.setState({
      formOpen: true,
      formMode: "edit",
      selectedDevice: device,
    });
  };

  handleFormOpenChange = (open: boolean) => {
    this.setState({
      formOpen: open,
      selectedDevice: open ? this.state.selectedDevice : null,
    });
  };

  handleSaved = () => {
    this.props.loadDevices();
  };

  render() {
    const {
      items,
      loading,
      error,
      currentPage,
      itemSize,
      totalCount,
      powerTypes,
    } = this.props;
    const totalPages = Math.max(1, Math.ceil(totalCount / itemSize) || 1);
    const pageNumbers = buildPageNumbers(currentPage, totalPages);
    const from = totalCount === 0 ? 0 : (currentPage - 1) * itemSize + 1;
    const to = Math.min(currentPage * itemSize, totalCount);

    return (
      <div className="space-y-6">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-2xl font-extrabold text-sb-dark">기기관리</h1>
            <p className="mt-1 text-sm text-muted-foreground">
              플러그 및 멀티탭 기기를 관리합니다.
            </p>
          </div>
          <Button type="button" onClick={this.openCreate}>
            <Plus className="h-4 w-4" />
            기기 등록
          </Button>
        </div>

        <Card>
          <CardContent className="py-4">
            <form
              onSubmit={this.handleSearchSubmit}
              className="flex flex-col gap-3 sm:flex-row sm:items-end"
            >
              <div className="min-w-0 flex-1 space-y-1.5">
                <label
                  htmlFor="device-search"
                  className="text-xs font-bold uppercase tracking-wide text-sb-primary"
                >
                  검색
                </label>
                <input
                  id="device-search"
                  type="text"
                  value={this.state.searchInput}
                  onChange={(event) =>
                    this.setState({ searchInput: event.target.value })
                  }
                  placeholder="ID 또는 기기명"
                  className="flex h-9 w-full rounded-md border border-input bg-card px-3 text-sm outline-none ring-offset-background placeholder:text-muted-foreground focus-visible:ring-2 focus-visible:ring-ring"
                />
              </div>
              <div className="space-y-1.5 sm:w-36">
                <label
                  htmlFor="device-item-size"
                  className="text-xs font-bold uppercase tracking-wide text-sb-primary"
                >
                  페이지 크기
                </label>
                <select
                  id="device-item-size"
                  value={itemSize}
                  onChange={this.handleItemSizeChange}
                  className="flex h-9 w-full rounded-md border border-input bg-card px-3 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring"
                >
                  {ITEM_SIZE_OPTIONS.map((size) => (
                    <option key={size} value={size}>
                      {size}개
                    </option>
                  ))}
                </select>
              </div>
              <Button type="submit" disabled={loading}>
                <Search className="h-4 w-4" />
                검색
              </Button>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Cpu className="h-4 w-4" />
              기기 목록
            </CardTitle>
            <span className="text-xs text-muted-foreground">
              총 {totalCount.toLocaleString("ko-KR")}건
            </span>
          </CardHeader>
          <CardContent className="p-0">
            {error ? (
              <div className="px-5 py-8 text-center text-sm text-sb-danger">
                {error}
              </div>
            ) : loading && items.length === 0 ? (
              <div className="px-5 py-8 text-center text-sm text-muted-foreground">
                기기 목록을 불러오는 중...
              </div>
            ) : items.length === 0 ? (
              <div className="px-5 py-8 text-center text-sm text-muted-foreground">
                검색 조건에 맞는 기기가 없습니다.
              </div>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow className="bg-sb-light/60 hover:bg-sb-light/60">
                    <TableHead>ID</TableHead>
                    <TableHead>장치명</TableHead>
                    <TableHead>모델</TableHead>
                    <TableHead>Topic</TableHead>
                    <TableHead>전력 유형</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {items.map((item) => (
                    <TableRow
                      key={item.id}
                      className="cursor-pointer"
                      onClick={() => this.openEdit(item)}
                    >
                      <TableCell className="font-semibold font-mono text-xs">
                        {item.id}
                      </TableCell>
                      <TableCell>{item.deviceName}</TableCell>
                      <TableCell>{item.model}</TableCell>
                      <TableCell className="font-mono text-xs">
                        {item.topic}
                      </TableCell>
                      <TableCell>
                        <Badge className="bg-sb-primary/10 text-sb-primary">
                          {resolvePowerTypeName(powerTypes, item.powerType)}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}

            <div className="flex flex-col gap-3 border-t border-sb-border px-5 py-4 sm:flex-row sm:items-center sm:justify-between">
              <div className="text-xs text-muted-foreground">
                {totalCount === 0
                  ? "표시할 항목이 없습니다."
                  : `${from.toLocaleString("ko-KR")}-${to.toLocaleString("ko-KR")} / ${totalCount.toLocaleString("ko-KR")}건`}
              </div>
              <div className="flex items-center gap-1">
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  disabled={loading || currentPage <= 1}
                  onClick={() => this.props.changeDevicePage(currentPage - 1)}
                >
                  <ChevronLeft className="h-4 w-4" />
                  이전
                </Button>
                {pageNumbers.map((page) => (
                  <Button
                    key={page}
                    type="button"
                    variant={page === currentPage ? "default" : "outline"}
                    size="sm"
                    disabled={loading}
                    className={cn("min-w-8 px-2")}
                    onClick={() => this.props.changeDevicePage(page)}
                  >
                    {page}
                  </Button>
                ))}
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  disabled={loading || currentPage >= totalPages || totalCount === 0}
                  onClick={() => this.props.changeDevicePage(currentPage + 1)}
                >
                  다음
                  <ChevronRight className="h-4 w-4" />
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>

        <DeviceFormModal
          open={this.state.formOpen}
          mode={this.state.formMode}
          device={this.state.selectedDevice}
          powerTypes={powerTypes}
          onOpenChange={this.handleFormOpenChange}
          onSaved={this.handleSaved}
        />
      </div>
    );
  }
}

function mapStateToProps(state: RootState): StateProps {
  return {
    items: state.device.items,
    loading: state.device.loading,
    error: state.device.error,
    query: state.device.query,
    currentPage: state.device.currentPage,
    itemSize: state.device.itemSize,
    totalCount: state.device.totalCount,
    powerTypes: state.electricPower.items,
  };
}

function mapDispatchToProps(dispatch: any): DispatchProps {
  return {
    loadDevices: () => dispatch(loadDevices()),
    loadElectricPowers: () => dispatch(loadElectricPowers()),
    searchDevices: (query: string) => dispatch(searchDevices(query)),
    changeDevicePage: (page: number) => dispatch(changeDevicePage(page)),
    changeDeviceItemSize: (itemSize: number) =>
      dispatch(changeDeviceItemSize(itemSize)),
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(DevicesPage);
