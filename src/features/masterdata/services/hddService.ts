import { fetchWithAuth } from "../../../utils/apiClient";

import type { HDDItem } from "@features/masterdata/types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const updateHDD = async (
  data: HDDItem,
): Promise<{ success: boolean; message?: string }> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/HDD`, {
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
        message: errorData.Message || "Failed to update HDD.",
      };
    }
    return { success: true, message: "HDD updated successfully" };
  } catch (error) {
    return {
      success: false,
      message:
        error instanceof Error ? error.message : "Network error occurred",
    };
  }
};

// httpget
// used at HDDManagement to get all HDDs
export const getHDDs = async (): Promise<HDDItem[]> => {
  const response = await fetchWithAuth(`${API_BASE}/api/HDD`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// httpost
// used at AddHDD for submit form inputs
export const addHDD = async (data: HDDItem): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/HDD`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to add HDD. Please try again.";
    }
    // Parse successful response
    const result = await response.json();
    return result; //  Return the whole object, not just the message
  } catch (error) {
    // Return structured error instead of throwing
    return error instanceof Error ? error.message : "Network error occurred";
  }
};
