import { CircularProgress } from "@mui/material";

interface LoadingErrorProps {
  loading: boolean;
  error: string | null;
  children: React.ReactNode;
}

export const LoadingError = ({
  loading,
  error,
  children,
}: LoadingErrorProps) => {
  if (loading) {
    return (
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "80vh",
        }}
      >
        <CircularProgress />
      </div>
    );
  }

  if (error) {
    return (
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "60vh",
        }}
      >
        <h3>{error}</h3>
      </div>
    );
  }

  return <>{children}</>;
};
