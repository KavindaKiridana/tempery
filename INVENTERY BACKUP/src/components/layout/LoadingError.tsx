import { Box, CircularProgress, Typography } from "@mui/material";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";

interface LoadingErrorProps {
  loading: boolean;
  error: string | null;
  children: React.ReactNode;
}

export const LoadingError = ({ loading, error, children }: LoadingErrorProps) => {
  // ── Loading state ──
  if (loading) {
    return (
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "center",
          gap: 1.5,
          height: "80vh",
        }}
      >
        <CircularProgress size={36} />
        <Typography variant="body2" color="text.secondary">
          Loading…
        </Typography>
      </Box>
    );
  }

  // ── Error state ──
  if (error) {
    return (
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "center",
          gap: 1,
          height: "60vh",
          color: "error.main",
        }}
      >
        <ErrorOutlineIcon sx={{ fontSize: 40 }} />
        <Typography variant="body1" fontWeight={600}>
          {error}
        </Typography>
      </Box>
    );
  }

  return <>{children}</>;
};