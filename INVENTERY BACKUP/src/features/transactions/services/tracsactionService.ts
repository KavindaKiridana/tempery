import { fetchWithAuth } from "../../../utils/apiClient";

import type { GetTransaction, PostTransaction } from "../types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

// httpget
export const getTransaction = async (data: GetTransaction): Promise<any> => {
  const params = new URLSearchParams();
  params.append("Type", data.Type);
  if (data.AssetId !== undefined) {
    params.append("AssetId", data.AssetId);
  }
  if (data.UserId !== undefined) {
    params.append("UserId", data.UserId.toString());
  }
  if (data.ComplainId !== undefined) {
    params.append("ComplainId", data.ComplainId.toString());
  }
  const res = await fetchWithAuth(
    `${API_BASE}/api/Transfer?${params.toString()}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    },
  );
  return res.json();
};

// httpost
export const postTransaction = async (
  data: PostTransaction,
): Promise<string> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Transfer`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    // Handle HTTP errors (4xx, 5xx)
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return errorData.Message || "Failed to transfer asset. Please try again.";
    }
    // Parse successful response
    const result = await response.json();
    return result; //  Return the whole object, not just the message
  } catch (error) {
    // Return structured error instead of throwing
    return error instanceof Error ? error.message : "Network error occurred";
  }
};

//httpget
//fetch all users for dropdowns
//currently used at TabAssigntoUser form
export const fetchUsers = async (): Promise<any> => {
  const res = await fetchWithAuth(`${API_BASE}/api/User`);
  if (!res.ok) throw new Error("Failed to load Users");
  return res.json();
};

//httpget
//fetch all suppliers for dropdowns
//currently used at TabSentToRepair form
export const fetchSuppliers = async (): Promise<any> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Supplier`);
  if (!res.ok) throw new Error("Failed to load suppliers");
  return res.json();
};
