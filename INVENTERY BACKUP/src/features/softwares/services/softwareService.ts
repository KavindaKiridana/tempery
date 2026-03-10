import { fetchWithAuth } from "../../../utils/apiClient";

import type { GetTransaction } from "@features/transactions/types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export interface SoftwareItem {
  softwareId: number;
  softwareName: string;
  isActive: boolean;
}

export interface InstallesSoftwares {
  InstalledSoftwareId: number;
  SoftwareName?: string;
  InstalledStatus: boolean;
}

export interface AssetStatus {
  IsActive: boolean;
}

//get request to fetch the asset's active status by asset id
export const findAssetStatus = async (
  data: GetTransaction,
): Promise<AssetStatus> => {
  const params = new URLSearchParams();
  params.append("Type", data.Type);
  if (data.UserId !== undefined) {
    params.append("UserId", data.UserId.toString());
  }
  if (data.AssetId !== undefined) {
    params.append("AssetId", data.AssetId);
  }
  const res = await fetchWithAuth(
    `${API_BASE}/api/Transfer?${params.toString()}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    },
  );
  console.log(res);
  return res.json();
};

// PATCH request to update installed software statuses
export async function addInstalledSoftwares(
  data: InstallesSoftwares[],
): Promise<{ Message: string }> {
  try {
    const response = await fetchWithAuth(
      `${API_BASE}/api/InstallSoftware/InstallSoftwares`,
      {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      },
    );
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.Message ||
          "Failed to update software statuses. Please try again.",
      );
    }
    return await response.json();
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    throw new Error(message);
  }
}

// httpget
// Fetch the list of installed softwares for a specific asset.
export async function fetchInstalledSoftwares(
  assetId: string,
): Promise<InstallesSoftwares[]> {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Software/${assetId}`);
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message ||
          "Failed to fetch installed softwares. Please try again.",
      );
    }
    return await response.json();
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    throw new Error(message);
  }
}

// httpget
// Fetch the list of softwares from the DB weather the software is active or not.
export async function fetchSoftwares(): Promise<SoftwareItem[]> {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Software`);
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message || "Failed to fetch software. Please try again.",
      );
    }
    return await response.json();
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    throw new Error(message);
  }
}
