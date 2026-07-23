import { ElectricPower, ElectricPowerType } from "@/lib/api";

export function resolvePowerTypeName(
  powerTypes: ElectricPower[],
  powerType: ElectricPowerType | null | undefined
): string {
  if (!powerType) {
    return "-";
  }

  const matched = powerTypes.find((item) => item.powerType === powerType);
  return matched?.powerTypeName || powerType;
}
