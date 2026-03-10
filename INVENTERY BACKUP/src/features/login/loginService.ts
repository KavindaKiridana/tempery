const API_BASE = import.meta.env.VITE_API_BASE_URL;

export interface LoginResponse {
  Token: string;
  UserId: string;
  FullName: string;
  LoginTime: string;
}

interface ApiError {
  message: string;
}

export const loginUser = async (
    username: string,
    password: string
  ): Promise<LoginResponse> => {
    try {
      const response = await fetch(`${API_BASE}/api/Login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ UserName: username, Password: password }),
      });
      if (!response.ok) {
        if (response.status === 401) {
          throw new Error("Invalid username or password.");
        }
        const contentType = response.headers.get("content-type");
        if (contentType && contentType.includes("application/json")) {
          const error: ApiError = await response.json();
          throw new Error(error.message);
        } else {
          const errorText = await response.text();
          console.error("Server error:", errorText);
          throw new Error(
            `Login failed: ${response.status} ${response.statusText}`
          );
        }
      }
      return response.json();
    } catch (error: any) {
      if (error instanceof TypeError) {
        throw new Error("Network error. Please check your connection and try again.");
      }
      throw error;
    }
  };
