import * as React from "react";
import { Eye, EyeOff } from "lucide-react";
import {
  createDevice,
  deleteDevice,
  updateDevice,
  Device,
  ElectricPower,
  ElectricPowerType,
} from "@/lib/api";
import {
  Dialog,
  DialogBody,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Table,
  TableBody,
  TableCell,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

export type DeviceFormMode = "create" | "edit";

interface DeviceFormModalProps {
  open: boolean;
  mode: DeviceFormMode;
  device: Device | null;
  powerTypes: ElectricPower[];
  onOpenChange: (open: boolean) => void;
  onSaved: () => void;
}

interface FormState {
  id: string;
  deviceName: string;
  model: string;
  topic: string;
  password: string;
  powerType: ElectricPowerType;
  voltCalibration: string;
  forwardConnector: string;
}

function createEmptyForm(powerTypes: ElectricPower[]): FormState {
  return {
    id: "",
    deviceName: "",
    model: "",
    topic: "",
    password: "",
    powerType: powerTypes[0]?.powerType || "",
    voltCalibration: "",
    forwardConnector: "",
  };
}

function createFormFromDevice(
  device: Device,
  powerTypes: ElectricPower[]
): FormState {
  return {
    id: device.id,
    deviceName: device.deviceName,
    model: device.model,
    topic: device.topic,
    password: device.password || "",
    powerType: device.powerType || powerTypes[0]?.powerType || "",
    voltCalibration:
      device.voltCalibration != null ? String(device.voltCalibration) : "",
    forwardConnector: device.forwardConnector || "",
  };
}

const inputClassName =
  "flex h-8 w-full rounded-md border border-input bg-card px-2 text-sm outline-none placeholder:text-muted-foreground focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:bg-sb-light disabled:opacity-80";

interface DeviceFormModalState {
  form: FormState;
  submitting: boolean;
  deleting: boolean;
  error: string | null;
  showPassword: boolean;
}

export class DeviceFormModal extends React.Component<
  DeviceFormModalProps,
  DeviceFormModalState
> {
  state: DeviceFormModalState = {
    form: createEmptyForm(this.props.powerTypes),
    submitting: false,
    deleting: false,
    error: null,
    showPassword: false,
  };

  resetForm(props: DeviceFormModalProps = this.props) {
    const form =
      props.mode === "edit" && props.device
        ? createFormFromDevice(props.device, props.powerTypes)
        : createEmptyForm(props.powerTypes);

    this.setState({
      form,
      submitting: false,
      deleting: false,
      error: null,
      showPassword: false,
    });
  }

  componentDidUpdate(prevProps: DeviceFormModalProps) {
    if (!prevProps.open && this.props.open) {
      this.resetForm(this.props);
      return;
    }

    if (
      this.props.open &&
      prevProps.device !== this.props.device &&
      this.props.mode === "edit" &&
      this.props.device
    ) {
      this.resetForm(this.props);
      return;
    }

    if (
      this.props.open &&
      prevProps.powerTypes !== this.props.powerTypes &&
      this.props.mode === "create" &&
      !this.state.form.powerType &&
      this.props.powerTypes.length > 0
    ) {
      this.setState((prev) => ({
        form: {
          ...prev.form,
          powerType: this.props.powerTypes[0].powerType,
        },
      }));
    }
  }

  updateField = <K extends keyof FormState>(key: K, value: FormState[K]) => {
    this.setState((prev) => ({
      form: { ...prev.form, [key]: value },
      error: null,
    }));
  };

  buildPayload = (): Device | null => {
    const { form } = this.state;
    const id = form.id.trim();
    const deviceName = form.deviceName.trim();
    const model = form.model.trim();
    const topic = form.topic.trim();
    const password = form.password.trim();
    const forwardConnector = form.forwardConnector.trim();
    const voltCalibrationRaw = form.voltCalibration.trim();

    if (!id || !deviceName || !model || !topic || !password || !form.powerType) {
      this.setState({
        error: "ID, 장치명, 모델, Topic, 비밀번호, 전력 유형은 필수입니다.",
      });
      return null;
    }

    let voltCalibration: number | null = null;
    if (voltCalibrationRaw) {
      const parsed = Number(voltCalibrationRaw);
      if (Number.isNaN(parsed)) {
        this.setState({ error: "전압 보정은 숫자여야 합니다." });
        return null;
      }
      voltCalibration = parsed;
    }

    return {
      id,
      deviceName,
      model,
      topic,
      password,
      powerType: form.powerType,
      voltCalibration,
      forwardConnector: forwardConnector || null,
    };
  };

  handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    if (this.state.submitting || this.state.deleting) {
      return;
    }

    const payload = this.buildPayload();
    if (!payload) {
      return;
    }

    const isEdit = this.props.mode === "edit";
    this.setState({ submitting: true, error: null });

    const request = isEdit ? updateDevice(payload) : createDevice(payload);

    request
      .then(() => {
        this.props.onOpenChange(false);
        this.props.onSaved();
      })
      .catch((error: unknown) => {
        const fallback = isEdit
          ? "기기 수정 중 오류가 발생했습니다."
          : "기기 등록 중 오류가 발생했습니다.";
        const message = error instanceof Error ? error.message : fallback;
        this.setState({ submitting: false, error: message });
      });
  };

  handleDelete = () => {
    if (this.state.submitting || this.state.deleting || this.props.mode !== "edit") {
      return;
    }

    const id = this.state.form.id.trim() || this.props.device?.id;
    if (!id) {
      this.setState({ error: "삭제할 기기 ID를 확인할 수 없습니다." });
      return;
    }

    const confirmed = window.confirm(
      `기기 "${id}"을(를) 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다.`
    );
    if (!confirmed) {
      return;
    }

    this.setState({ deleting: true, error: null });

    deleteDevice(id)
      .then(() => {
        this.props.onOpenChange(false);
        this.props.onSaved();
      })
      .catch((error: unknown) => {
        const message =
          error instanceof Error
            ? error.message
            : "기기 삭제 중 오류가 발생했습니다.";
        this.setState({ deleting: false, error: message });
      });
  };

  renderFieldRow(
    label: string,
    required: boolean,
    control: React.ReactNode
  ) {
    return (
      <TableRow className="hover:bg-transparent">
        <TableCell className="w-40 bg-sb-light/60 font-bold text-sb-secondary">
          {label}
          {required ? <span className="ml-1 text-sb-danger">*</span> : null}
        </TableCell>
        <TableCell>{control}</TableCell>
      </TableRow>
    );
  }

  render() {
    const { open, onOpenChange, powerTypes, mode } = this.props;
    const { form, submitting, deleting, error, showPassword } = this.state;
    const isEdit = mode === "edit";
    const busy = submitting || deleting;

    return (
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="max-w-xl">
          <form onSubmit={this.handleSubmit}>
            <DialogHeader>
              <DialogTitle>{isEdit ? "기기 수정" : "기기 등록"}</DialogTitle>
            </DialogHeader>
            <DialogBody className="p-0">
              <Table>
                <TableBody>
                  {this.renderFieldRow(
                    "ID",
                    true,
                    <input
                      className={inputClassName}
                      value={form.id}
                      maxLength={12}
                      required
                      disabled={busy || isEdit}
                      onChange={(event) =>
                        this.updateField("id", event.target.value)
                      }
                    />
                  )}
                  {this.renderFieldRow(
                    "장치명",
                    true,
                    <input
                      className={inputClassName}
                      value={form.deviceName}
                      maxLength={20}
                      required
                      disabled={busy}
                      onChange={(event) =>
                        this.updateField("deviceName", event.target.value)
                      }
                    />
                  )}
                  {this.renderFieldRow(
                    "모델",
                    true,
                    <input
                      className={inputClassName}
                      value={form.model}
                      maxLength={20}
                      required
                      disabled={busy}
                      onChange={(event) =>
                        this.updateField("model", event.target.value)
                      }
                    />
                  )}
                  {this.renderFieldRow(
                    "Topic",
                    true,
                    <input
                      className={inputClassName}
                      value={form.topic}
                      maxLength={20}
                      required
                      disabled={busy}
                      onChange={(event) =>
                        this.updateField("topic", event.target.value)
                      }
                    />
                  )}
                  {this.renderFieldRow(
                    "비밀번호",
                    true,
                    <div className="relative">
                      <input
                        type={showPassword ? "text" : "password"}
                        className={cn(
                          inputClassName,
                          "pr-9 [&::-ms-reveal]:hidden"
                        )}
                        value={form.password}
                        maxLength={50}
                        required
                        autoComplete="new-password"
                        disabled={busy}
                        onChange={(event) =>
                          this.updateField("password", event.target.value)
                        }
                      />
                      <button
                        type="button"
                        className="absolute right-1.5 top-1/2 -translate-y-1/2 rounded p-1 text-sb-secondary outline-none hover:bg-sb-light hover:text-sb-dark focus-visible:ring-2 focus-visible:ring-ring disabled:opacity-50"
                        aria-label={
                          showPassword ? "비밀번호 숨기기" : "비밀번호 보기"
                        }
                        disabled={busy}
                        onClick={() =>
                          this.setState((prev) => ({
                            showPassword: !prev.showPassword,
                          }))
                        }
                      >
                        {showPassword ? (
                          <EyeOff className="h-4 w-4" />
                        ) : (
                          <Eye className="h-4 w-4" />
                        )}
                      </button>
                    </div>
                  )}
                  {this.renderFieldRow(
                    "전력 유형",
                    true,
                    <select
                      className={inputClassName}
                      value={form.powerType}
                      required
                      disabled={busy || powerTypes.length === 0}
                      onChange={(event) =>
                        this.updateField("powerType", event.target.value)
                      }
                    >
                      {powerTypes.length === 0 ? (
                        <option value="">불러오는 중...</option>
                      ) : (
                        powerTypes.map((item) => (
                          <option key={item.powerType} value={item.powerType}>
                            {item.powerTypeName}
                          </option>
                        ))
                      )}
                    </select>
                  )}
                  {this.renderFieldRow(
                    "전압 보정",
                    false,
                    <input
                      type="number"
                      step="any"
                      className={inputClassName}
                      value={form.voltCalibration}
                      disabled={busy}
                      onChange={(event) =>
                        this.updateField("voltCalibration", event.target.value)
                      }
                    />
                  )}
                  {this.renderFieldRow(
                    "Forward Connector",
                    false,
                    <select
                      className={inputClassName}
                      value={form.forwardConnector}
                      disabled={busy}
                      onChange={(event) =>
                        this.updateField(
                          "forwardConnector",
                          event.target.value
                        )
                      }
                    >
                      <option value="">선택 안 함</option>
                    </select>
                  )}
                </TableBody>
              </Table>
              {error ? (
                <div className="border-t border-sb-border px-4 py-3 text-sm text-sb-danger">
                  {error}
                </div>
              ) : null}
            </DialogBody>
            <DialogFooter
              className={cn(isEdit && "sm:justify-between")}
            >
              {isEdit ? (
                <Button
                  type="button"
                  variant="destructive"
                  disabled={busy}
                  onClick={this.handleDelete}
                >
                  {deleting ? "삭제 중..." : "삭제"}
                </Button>
              ) : null}
              <div className="flex flex-col-reverse gap-2 sm:flex-row sm:justify-end">
                <Button
                  type="button"
                  variant="outline"
                  disabled={busy}
                  onClick={() => onOpenChange(false)}
                >
                  취소
                </Button>
                <Button
                  type="submit"
                  disabled={busy || powerTypes.length === 0}
                >
                  {submitting
                    ? isEdit
                      ? "저장 중..."
                      : "등록 중..."
                    : isEdit
                      ? "저장"
                      : "등록"}
                </Button>
              </div>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>
    );
  }
}
