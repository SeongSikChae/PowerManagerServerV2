import * as React from "react";
import { connect } from "react-redux";
import { ChevronDown, FileKey, RefreshCw, User } from "lucide-react";
import { RootState } from "@/store";
import {
  clearOperationError,
  loadWhoami,
  renewCertificate,
  transferExpiredCertificate,
} from "@/store/auth/actions";
import { CertificateInfo } from "@/lib/api";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  Dialog,
  DialogBody,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { CertificateInfoModal } from "@/components/layout/CertificateInfoModal";

interface StateProps {
  certificate: CertificateInfo | null;
  loading: boolean;
  updating: boolean;
  escalating: boolean;
  operationError: string | null;
}

interface DispatchProps {
  loadWhoami: () => void;
  renewCertificate: (file: File) => void;
  transferExpiredCertificate: (file: File) => void;
  clearOperationError: () => void;
}

type Props = StateProps & DispatchProps;

interface UserMenuState {
  profileOpen: boolean;
}

function displayEmail(certificate: CertificateInfo | null, loading: boolean): string {
  if (loading && !certificate) {
    return "불러오는 중...";
  }
  const email = certificate?.email?.trim();
  return email ? email : "미인증";
}

function avatarLabel(email: string): string {
  if (!email || email === "미인증" || email === "불러오는 중...") {
    return "PM";
  }
  return email.charAt(0).toUpperCase();
}

class UserMenu extends React.Component<Props, UserMenuState> {
  renewFileInputRef = React.createRef<HTMLInputElement>();
  escalateFileInputRef = React.createRef<HTMLInputElement>();

  state: UserMenuState = {
    profileOpen: false,
  };

  componentDidMount() {
    this.props.loadWhoami();
  }

  handleProfile = (event: Event) => {
    event.preventDefault();
    this.setState({ profileOpen: true });
  };

  handleProfileOpenChange = (open: boolean) => {
    this.setState({ profileOpen: open });
  };

  handleRenewCertificate = () => {
    this.renewFileInputRef.current?.click();
  };

  handleRenewCertificateFileChange = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = event.target.files?.[0];
    event.target.value = "";

    if (!file) {
      return;
    }

    this.props.renewCertificate(file);
  };

  handleTransferExpiredCertificate = () => {
    this.escalateFileInputRef.current?.click();
  };

  handleEscalateCertificateFileChange = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = event.target.files?.[0];
    event.target.value = "";

    if (!file) {
      return;
    }

    this.props.transferExpiredCertificate(file);
  };

  handleOperationErrorOpenChange = (open: boolean) => {
    if (!open) {
      this.props.clearOperationError();
    }
  };

  render() {
    const { certificate, loading, updating, escalating, operationError } =
      this.props;
    const { profileOpen } = this.state;
    const email = displayEmail(certificate, loading);
    const authenticated = Boolean(certificate?.email?.trim());

    return (
      <>
        <input
          ref={this.renewFileInputRef}
          type="file"
          accept=".p12,.pfx,.crt,.cer,.pem,.zip"
          className="hidden"
          onChange={this.handleRenewCertificateFileChange}
        />
        <input
          ref={this.escalateFileInputRef}
          type="file"
          accept=".p12,.pfx,.crt,.cer,.pem,.zip"
          className="hidden"
          onChange={this.handleEscalateCertificateFileChange}
        />

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <button
              type="button"
              className="flex items-center gap-2 rounded-md px-2 py-1.5 text-sm font-semibold text-sb-secondary outline-none hover:bg-sb-light focus-visible:ring-2 focus-visible:ring-ring"
            >
              <span className="hidden max-w-[16rem] truncate sm:inline">{email}</span>
              <div className="flex h-8 w-8 items-center justify-center rounded-full bg-sb-primary text-xs font-bold text-white">
                {avatarLabel(email)}
              </div>
              <ChevronDown className="h-4 w-4 opacity-60" />
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuLabel>계정</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem onSelect={this.handleProfile}>
              <User className="mr-2 h-4 w-4" />
              Profile
            </DropdownMenuItem>
            {authenticated && (
              <>
                <DropdownMenuItem
                  disabled={updating || escalating}
                  onSelect={this.handleRenewCertificate}
                >
                  <RefreshCw className="mr-2 h-4 w-4" />
                  {updating ? "인증서 갱신 중..." : "인증서 갱신"}
                </DropdownMenuItem>
                <DropdownMenuItem
                  disabled={updating || escalating}
                  onSelect={this.handleTransferExpiredCertificate}
                >
                  <FileKey className="mr-2 h-4 w-4" />
                  {escalating ? "이관 중..." : "만료 인증서 이관"}
                </DropdownMenuItem>
              </>
            )}
          </DropdownMenuContent>
        </DropdownMenu>

        <CertificateInfoModal
          open={profileOpen}
          certificate={certificate}
          onOpenChange={this.handleProfileOpenChange}
        />

        <Dialog
          open={Boolean(operationError)}
          onOpenChange={this.handleOperationErrorOpenChange}
        >
          <DialogContent>
            <DialogHeader>
              <DialogTitle>오류</DialogTitle>
            </DialogHeader>
            <DialogBody className="space-y-5">
              <p className="text-sm font-semibold text-sb-danger">
                {operationError}
              </p>
              <div className="flex justify-end">
                <Button
                  type="button"
                  onClick={() => this.props.clearOperationError()}
                >
                  확인
                </Button>
              </div>
            </DialogBody>
          </DialogContent>
        </Dialog>
      </>
    );
  }
}

function mapStateToProps(state: RootState): StateProps {
  return {
    certificate: state.auth.certificate,
    loading: state.auth.loading,
    updating: state.auth.updating,
    escalating: state.auth.escalating,
    operationError: state.auth.operationError,
  };
}

function mapDispatchToProps(dispatch: any): DispatchProps {
  return {
    loadWhoami: () => dispatch(loadWhoami()),
    renewCertificate: (file: File) => dispatch(renewCertificate(file)),
    transferExpiredCertificate: (file: File) =>
      dispatch(transferExpiredCertificate(file)),
    clearOperationError: () => dispatch(clearOperationError()),
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(UserMenu);
