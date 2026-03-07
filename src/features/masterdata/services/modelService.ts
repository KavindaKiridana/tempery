import { fetchWithAuth } from "@utils/apiClient";

import type { ModelItem } from "@features/masterdata/types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const updateModel = async (
  data: ModelItem,
): Promise<{ success: boolean; message?: string }> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Model`, {
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
        message: errorData.Message || "Failed to update model.",
      };
    }
    return { success: true, message: "Model updated successfully" };
  } catch (error) {
    return {
      success: false,
      message:
        error instanceof Error ? error.message : "Network error occurred",
    };
  }
};

// GET: Fetch all models
export const getModels = async (): Promise<ModelItem[]> => {
  const response = await fetchWithAuth(`${API_BASE}/api/Model`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// POST: Add a new model
export const addModel = async (
  data: ModelItem,
): Promise<string | ModelItem> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Model`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to add model. Please try again.";
    }
    const result = await response.json();
    return result;
  } catch (error) {
    return error instanceof Error ? error.message : "Network error occurred";
  }
};
