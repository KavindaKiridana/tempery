import { fetchWithAuth } from "../../../utils/apiClient";

import type { TransactionResponse } from "../types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const getHistoryTransactions = async (
  assetId: string,
): Promise<TransactionResponse[] | null> => {
  try {
    const response = await fetchWithAuth(
      `${API_BASE}/api/LogHistory/${assetId}`,
    );
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message || "Failed to fetch assets. Please try again.",
      );
    }
    const data: TransactionResponse[] = await response.json();
    return data;
  } catch (error) {
    const message = error!.toString();
    console.warn(message);
    return null;
  }
};
