import * as React from "react";
import { CertificateInfo } from "@/lib/api";
import {
  Dialog,
  DialogBody,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

interface CertificateInfoModalProps {
  open: boolean;
  certificate: CertificateInfo | null;
  onOpenChange: (open: boolean) => void;
}

function pad2(value: number): string {
  return value < 10 ? `0${value}` : String(value);
}

function formatDateTime(ms: number | null | undefined): string {
  if (ms == null || Number.isNaN(ms) || ms === 0) {
    return "-";
  }

  const date = new Date(ms);
  if (Number.isNaN(date.getTime())) {
    return "-";
  }

  return (
    `${date.getFullYear()}-${pad2(date.getMonth() + 1)}-${pad2(date.getDate())} ` +
    `${pad2(date.getHours())}:${pad2(date.getMinutes())}:${pad2(date.getSeconds())}`
  );
}

function displayValue(value: string | null | undefined): string {
  const trimmed = value?.trim();
  return trimmed ? trimmed : "-";
}

export function CertificateInfoModal({
  open,
  certificate,
  onOpenChange,
}: CertificateInfoModalProps) {
  const rows = [
    { label: "CommonName", value: displayValue(certificate?.commonName) },
    { label: "Email", value: displayValue(certificate?.email) },
    {
      label: "유효기간 시작",
      value: formatDateTime(certificate?.before),
    },
    {
      label: "유효기간 종료",
      value: formatDateTime(certificate?.after),
    },
    { label: "지문", value: displayValue(certificate?.thumbprint) },
  ];

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>클라이언트 인증서 정보</DialogTitle>
        </DialogHeader>
        <DialogBody>
          <dl className="space-y-3">
            {rows.map((row) => (
              <div key={row.label} className="grid grid-cols-[8.5rem_1fr] gap-3 text-sm">
                <dt className="font-bold text-sb-secondary">{row.label}</dt>
                <dd className="break-all font-semibold text-sb-dark">{row.value}</dd>
              </div>
            ))}
          </dl>
        </DialogBody>
      </DialogContent>
    </Dialog>
  );
}
