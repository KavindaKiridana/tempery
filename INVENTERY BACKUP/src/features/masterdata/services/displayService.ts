import { fetchWithAuth } from "@utils/apiClient";

import type { DisplayItem } from "@features/masterdata/types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

// displayService.ts
export const updateDisplay = async (
  data: DisplayItem,
): Promise<{ success: boolean; message?: string }> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Display`, {
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
        message: errorData.Message || "Failed to update display.",
      };
    }
    return { success: true, message: "Display updated successfully" };
  } catch (error) {
    return {
      success: false,
      message:
        error instanceof Error ? error.message : "Network error occurred",
    };
  }
};

// httpget
// used at DisplayManagement to get all displays
export const getDisplays = async (): Promise<DisplayItem[]> => {
  const response = await fetchWithAuth(`${API_BASE}/api/Display`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// httpost
// used at AddDisplay for submit form inputs
export const addDisplay = async (data: DisplayItem): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Display`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to add display. Please try again.";
    }
    // Parse successful response
    const result = await response.json();
    return result; //  Return the whole object, not just the message
  } catch (error) {
    // Return structured error instead of throwing
    return error instanceof Error ? error.message : "Network error occurred";
  }
};
