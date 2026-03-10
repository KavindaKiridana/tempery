import { fetchWithAuth } from "../../../utils/apiClient";

import type { SSDItem } from "@features/masterdata/types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const updateSSD = async (
  data: SSDItem,
): Promise<{ success: boolean; message?: string }> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/SSD`, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return {
        success: false,
        message: errorData.Message || "Failed to update SSD.",
      };
    }
    return { success: true, message: "SSD updated successfully" };
  } catch (error) {
    return {
      success: false,
      message:
        error instanceof Error ? error.message : "Network error occurred",
    };
  }
};

// httpget
// used to get all SSDs
export const getSSDs = async (): Promise<SSDItem[]> => {
  const response = await fetchWithAuth(`${API_BASE}/api/SSD`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// httpost
// used to submit form inputs
export const addSSD = async (data: SSDItem): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/SSD`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to add SSD. Please try again.";
    }
    // Parse successful response
    const result = await response.json();
    return result; // Return the whole object, not just the message
  } catch (error) {
    // Return structured error instead of throwing
    return error instanceof Error ? error.message : "Network error occurred";
  }
};
