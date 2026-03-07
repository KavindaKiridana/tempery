import { fetchWithAuth } from "../../../utils/apiClient";

import type {
  CreateCommonAssetRequest,
  CreateAssetRequest,
  CreateAssetResponse,
  ReturnAssetId,
  AssetView,
} from "../types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const createCommonAsset = async (
  data: CreateCommonAssetRequest
): Promise<string[]> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Asset`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Please fill all required fields");
      }
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message || "Failed to create asset. Please try again."
      );
    }
    const result: CreateAssetResponse = await response.json();
    return result.AssetIds;
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    throw new Error(message);
  }
};

export const modifyAsset = async (
  AssetId: string,
  data: CreateAssetRequest
): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Asset/${AssetId}`, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message || "Failed to update asset. Please try again."
      );
    }
    const result: ReturnAssetId = await response.json();
    return result.AssetId;
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    throw new Error(message);
  }
};

export const getAssetInfo = async (assetId: string): Promise<AssetView> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Asset/${assetId}`);
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message || "Failed to fetch assets. Please try again."
      );
    }
    const data: AssetView = await response.json();
    return data;
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    console.warn("Failed to fetch assets:", message);
    throw new Error(message);
  }
};
