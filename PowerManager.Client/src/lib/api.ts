export interface CertificateInfo {
  commonName: string;
  email: string;
  before: number;
  after: number;
  thumbprint: string;
}

export interface RestResult {
  error: boolean;
  code: string | null;
  errorMessage: string | null;
}

export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string | null;
}

export type ElectricPowerType = string;

export interface ElectricPower {
  powerType: ElectricPowerType;
  powerTypeName: string;
}

export interface Device {
  id: string;
  deviceName: string;
  model: string;
  topic: string;
  password: string | null;
  powerType: ElectricPowerType;
  voltCalibration: number | null;
  forwardConnector: string | null;
}

export interface Pagination {
  currentPage: number;
  itemSize: number;
  totalCount: number;
}

export interface SearchQuery {
  query?: string | null;
  pagination: Pagination;
}

export interface PagedResult<T> {
  items: T[];
  pagination: Pagination;
}

const CERTIFICATE_ERROR_MESSAGES: Record<string, string> = {
  TOO_BIG_CERTIFICATE: "인증서 파일 크기가 너무 큽니다. (최대 128KB)",
  NOT_IMPLEMENTED: "아직 구현되지 않은 기능입니다.",
};

function formatCertificateError(result: RestResult, fallback: string): string {
  if (result.code && CERTIFICATE_ERROR_MESSAGES[result.code]) {
    return CERTIFICATE_ERROR_MESSAGES[result.code];
  }

  const message = result.errorMessage?.trim();
  if (message) {
    return message;
  }

  if (result.code) {
    return `요청에 실패했습니다. (${result.code})`;
  }

  return fallback;
}

async function ensureRestSuccess(
  response: Response,
  fallback: string
): Promise<void> {
  let result: RestResult | null = null;

  try {
    result = (await response.json()) as RestResult;
  } catch {
    result = null;
  }

  if (result?.error) {
    throw new Error(formatCertificateError(result, fallback));
  }

  if (!response.ok) {
    throw new Error(fallback);
  }
}

export async function fetchWhoami(): Promise<CertificateInfo> {
  const response = await fetch("/rest/auth/whoami");

  if (!response.ok) {
    throw new Error(`whoami 요청 실패 (${response.status})`);
  }

  return response.json();
}

export async function updateCertificate(file: File): Promise<void> {
  const formData = new FormData();
  formData.append("file", file);

  const response = await fetch("/rest/auth/certificateUpdate", {
    method: "POST",
    body: formData,
  });

  await ensureRestSuccess(
    response,
    `certificateUpdate 요청 실패 (${response.status})`
  );
}

export async function escalateCertificate(file: File): Promise<void> {
  const formData = new FormData();
  formData.append("file", file);

  const response = await fetch("/rest/auth/certificateEscalation", {
    method: "POST",
    body: formData,
  });

  await ensureRestSuccess(
    response,
    `certificateEscalation 요청 실패 (${response.status})`
  );
}

export async function fetchWeatherForecast(): Promise<WeatherForecast[]> {
  const response = await fetch("/rest/weatherforecast");

  if (!response.ok) {
    throw new Error(`WeatherForecast 요청 실패 (${response.status})`);
  }

  return response.json();
}

export async function fetchDeviceList(
  searchQuery: SearchQuery = {
    query: null,
    pagination: { currentPage: 1, itemSize: 10, totalCount: 0 },
  }
): Promise<PagedResult<Device>> {
  const response = await fetch("/rest/device/list", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(searchQuery),
  });

  if (!response.ok) {
    throw new Error(`기기 목록 요청 실패 (${response.status})`);
  }

  return response.json();
}

export async function fetchElectricPowerList(): Promise<ElectricPower[]> {
  const response = await fetch("/rest/electricpower/list");

  if (!response.ok) {
    throw new Error(`전력 유형 목록 요청 실패 (${response.status})`);
  }

  return response.json();
}

export async function createDevice(device: Device): Promise<void> {
  const response = await fetch("/rest/device/create", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(device),
  });

  await ensureRestSuccess(
    response,
    `기기 등록 요청 실패 (${response.status})`
  );
}

export async function updateDevice(device: Device): Promise<void> {
  const response = await fetch("/rest/device/update", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(device),
  });

  await ensureRestSuccess(
    response,
    `기기 수정 요청 실패 (${response.status})`
  );
}

export async function deleteDevice(id: string): Promise<void> {
  const response = await fetch(
    `/rest/device/delete/${encodeURIComponent(id)}`,
    {
      method: "DELETE",
    }
  );

  await ensureRestSuccess(
    response,
    `기기 삭제 요청 실패 (${response.status})`
  );
}
