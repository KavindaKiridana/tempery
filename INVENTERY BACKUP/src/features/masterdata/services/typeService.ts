import { fetchWithAuth } from "../../../utils/apiClient";

import type { TypeItem } from "@features/masterdata/types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const updateType = async (
  data: TypeItem,
): Promise<{ success: boolean; message?: string }> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Type`, {
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
        message: errorData.Message || "Failed to update Type.",
      };
    }
    return { success: true, message: "Type updated successfully" };
  } catch (error) {
    return {
      success: false,
      message:
        error instanceof Error ? error.message : "Network error occurred",
    };
  }
};

// httpget
export const getTypes = async (): Promise<TypeItem[]> => {
  const response = await fetchWithAuth(`${API_BASE}/api/Type`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// httpost
export const addType = async (data: TypeItem): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Type`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to add Type. Please try again.";
    }
    // Parse successful response
    const result = await response.json();
    return result; //  Return the whole object, not just the message
  } catch (error) {
    // Return structured error instead of throwing
    return error instanceof Error ? error.message : "Network error occurred";
  }
};
