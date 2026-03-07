// AuthContext.tsx
import * as React from "react";

interface User {
  UserId: string;
  FullName: string;
  LoginTime: string;
  Token: string;
}

interface AuthContextType {
  isLoggedIn: boolean;
  user: User | null;
  token: string | null;
  login: (userData: User) => void;
  logout: () => void;
}

const AuthContext = React.createContext<AuthContextType>({
  isLoggedIn: false,
  user: null,
  token: null,
  login: () => {},
  logout: () => {},
});

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = React.useState<User | null>(null);
  const [loading, setLoading] = React.useState(true);

  const login = (userData: User) => {
    setUser(userData);
    localStorage.setItem("user", JSON.stringify(userData));
    localStorage.setItem("token", userData.Token);
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem("user");
    localStorage.removeItem("token");
  };

  React.useEffect(() => {
    const storedUser = localStorage.getItem("user");
    const storedToken = localStorage.getItem("token");
    if (storedUser && storedToken) {
      const parsedUser = JSON.parse(storedUser);
      parsedUser.Token = storedToken;
      setUser(parsedUser);
    }
    setLoading(false); // done loading
  }, []);

  if (loading) return null; // prevents redirect before auth loads

  return (
    <AuthContext.Provider
      value={{
        isLoggedIn: !!user,
        user,
        token: user?.Token || null,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => React.useContext(AuthContext);
