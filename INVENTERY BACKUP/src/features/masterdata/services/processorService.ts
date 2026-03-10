import { fetchWithAuth } from "../../../utils/apiClient";

import type { ProcessorItem } from "@features/masterdata/types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const updateProcessor = async (
  data: ProcessorItem,
): Promise<{ success: boolean; message?: string }> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Processor`, {
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
        message: errorData.Message || "Failed to update processor.",
      };
    }
    return { success: true, message: "Processor updated successfully" };
  } catch (error) {
    return {
      success: false,
      message:
        error instanceof Error ? error.message : "Network error occurred",
    };
  }
};

// httpget
// used to get all Processors
export const getProcessors = async (): Promise<ProcessorItem[]> => {
  const response = await fetchWithAuth(`${API_BASE}/api/Processor`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// httpost
// used to submit form inputs
export const addProcessor = async (data: ProcessorItem): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Processor`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to add processor. Please try again.";
    }
    // Parse successful response
    const result = await response.json();
    return result; // Return the whole object, not just the message
  } catch (error) {
    // Return structured error instead of throwing
    return error instanceof Error ? error.message : "Network error occurred";
  }
};
