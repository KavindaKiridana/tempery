import { fetchWithAuth } from "../../../utils/apiClient";

import type { AssetView } from "../types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

// https://localhost:44362/api/GetAllAssets?search=&isExport=false
export const getExcel = async (searchQuery: string = ""): Promise<void> => {
  try {
    const response = await fetchWithAuth(
      `${API_BASE}/api/GetAllAssets?search=${encodeURIComponent(
        searchQuery,
      )}&isExport=${true}`,
    );
    if (!response.ok) {
      throw new Error("Failed to export assets. Please try again.");
    }
    // Get the blob from response
    const blob = await response.blob();
    // Create a download link
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    // Get filename from Content-Disposition header or use default
    const contentDisposition = response.headers.get("Content-Disposition");
    let filename = `Assets_${new Date().toISOString().slice(0, 10)}.xlsx`;
    if (contentDisposition) {
      const filenameMatch = contentDisposition.match(/filename="?(.+)"?/i);
      if (filenameMatch) {
        filename = filenameMatch[1];
      }
    }
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    // Cleanup
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    console.error("Export error:", message);
    throw error;
  }
};

export const getAssets = async (
  searchQuery: string = "",
): Promise<AssetView[]> => {
  try {
    const response = await fetchWithAuth(
      `${API_BASE}/api/GetAllAssets?search=${encodeURIComponent(
        searchQuery,
      )}&isExport=${false}`,
    );
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message || "Failed to fetch assets. Please try again.",
      );
    }
    const data: AssetView[] = await response.json();
    return data;
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    console.warn(message);
    return [];
  }
};
