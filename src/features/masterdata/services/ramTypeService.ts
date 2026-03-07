import { fetchWithAuth } from "../../../utils/apiClient";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export interface RAMTypeData {
  Type: string;
}

// httpget
// used to get all RAM Types
export const getRAMTypes = async (): Promise<RAMTypeData[]> => {
  const response = await fetchWithAuth(`${API_BASE}/api/RAMType`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
  return response.json();
};

// httpost
// used to submit form inputs
export const addRAMType = async (data: RAMTypeData): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/RAMType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to add RAM Type. Please try again.";
    }
    // Parse successful response
    const result = await response.json();
    return result; // Return the whole object, not just the message
  } catch (error) {
    // Return structured error instead of throwing
    return error instanceof Error ? error.message : "Network error occurred";
  }
};
