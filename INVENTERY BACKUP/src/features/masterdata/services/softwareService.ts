import { fetchWithAuth } from "../../../utils/apiClient";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export interface SoftwareData {
  Id?: number;
  Name: string;
  IsActive?: boolean;
  IsUsed?: boolean;
}

export const updateSoftware = async (
  data: SoftwareData,
): Promise<{ success: boolean; message?: string }> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Software`, {
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
        message: errorData.Message || "Failed to update software.",
      };
    }
    return { success: true, message: "Software updated successfully" };
  } catch (error) {
    return {
      success: false,
      message:
        error instanceof Error ? error.message : "Network error occurred",
    };
  }
};

// httpget
// used to get all Software
export const getSoftwares = async (): Promise<SoftwareData[]> => {
  const response = await fetchWithAuth(`${API_BASE}/api/Software`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// httpost
// used to submit form inputs
export const addSoftware = async (data: SoftwareData): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Software`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to add Software. Please try again.";
    }
    // Parse successful response
    const result = await response.json();
    return result; // Return the whole object, not just the message
  } catch (error) {
    // Return structured error instead of throwing
    return error instanceof Error ? error.message : "Network error occurred";
  }
};
