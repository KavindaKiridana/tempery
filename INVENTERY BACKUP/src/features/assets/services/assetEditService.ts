import { fetchWithAuth } from "../../../utils/apiClient";

import type { CreateCommonAssetRequest, EditAssetView } from "../types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const updateAsset = async (
  data: CreateCommonAssetRequest,
): Promise<string | null> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/EditAsset`, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (response.ok) {
      return null;
    } else {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message;
    }
  } catch (error) {
    return error instanceof Error ? error.message : "Network error occurred";
  }
};

export const getAssetInfo = async (
  assetId: string,
): Promise<EditAssetView | null> => {
  try {
    const response = await fetchWithAuth(
      `${API_BASE}/api/ViewAsset/${assetId}`,
    );
    if (response.status === 404) {
      console.warn("No assets found (404). Maybe wrong AssetId?");
      return {} as EditAssetView;
    }
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message || "Failed to fetch assets. Please try again.",
      );
    }
    const data: EditAssetView = await response.json();
    return data;
  } catch (error) {
    const message = error!.toString();
    console.warn(message);
    return null;
  }
};
