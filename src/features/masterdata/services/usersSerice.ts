import { fetchWithAuth } from "../../../utils/apiClient";

import type { GetUsersParams, UsersList } from "../types";

const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const fetchDepartment = async (): Promise<any> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Department`);
  if (!res.ok) throw new Error("Failed to load Departments");
  return res.json();
};

export const fetchLocations = async (): Promise<any> => {
  const res = await fetchWithAuth(`${API_BASE}/api/Location`);
  if (!res.ok) throw new Error("Failed to load Locations");
  return res.json();
};

export const addUser = async (data: UsersList): Promise<any> => {
  const response = await fetchWithAuth(`${API_BASE}/api/Users`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });
  const result = await response.json();
  return result;
};

export const updateUser = async (user: UsersList): Promise<void> => {
  try {
    const response = await fetchWithAuth(`${API_BASE}/api/Users`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(user),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.message || "Failed to update user. Please try again.",
      );
    }
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unknown error";
    console.warn(message);
    throw error;
  }
};

export const getUsers = async (params: GetUsersParams): Promise<any> => {
  const queryParams = new URLSearchParams();
  queryParams.append("needEveryUsers", params.needEveryUsers.toString());
  if (params.requestedUserId !== undefined) {
    queryParams.append("requestedUserId", params.requestedUserId.toString());
  }
  if (params.search) {
    queryParams.append("search", params.search);
  }
  const response = await fetchWithAuth(
    `${API_BASE}/api/Users?${queryParams.toString()}`,
  );
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(
      errorData.message || "Failed to fetch users. Please try again.",
    );
  }
  if (params.needEveryUsers) {
    const data: UsersList[] = await response.json();
    return data;
  } else {
    const data = await response.json();
    return data;
  }
};
