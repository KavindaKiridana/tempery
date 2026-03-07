import * as React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Container,
  Paper,
  Typography,
  TextField,
  Button,
  CssBaseline,
  CircularProgress,
  Avatar,
} from "@mui/material";
import { LockOutlined as LockOutlinedIcon } from "@mui/icons-material";
import { useAuth } from "../../contexts/AuthContext";
import { loginUser } from "./loginService";

function LoginPage() {
  const [loading, setLoading] = useState(false);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, logout } = useAuth();
  const navigate = useNavigate();

  // Redirect if already authenticated
  React.useEffect(() => {
    logout();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await loginUser(username, password);
      login(response);
      navigate("/viewAsset");
    } catch (error: any) {
      alert(error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <React.Fragment>
      <CssBaseline />
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "75vh",
          bgcolor: "background.default",
          p: 2,
        }}
      >
        <Container maxWidth="xs">
          <Paper
            elevation={3}
            sx={{
              p: 4,
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              borderRadius: 4,
              border: "1px solid",
              borderColor: "divider",
              boxShadow: "0px 8px 32px rgba(0, 0, 0, 0.08)",
              transition: "transform 0.3s ease, box-shadow 0.3s ease",
              "&:hover": {
                transform: "translateY(-4px)",
                boxShadow: "0px 12px 40px rgba(0, 0, 0, 0.12)",
              },
            }}
          >
            <Avatar
              sx={{
                m: 1,
                bgcolor: "primary.main",
                width: 56,
                height: 56,
                mb: 2,
              }}
            >
              <LockOutlinedIcon fontSize="large" />
            </Avatar>
            <Typography
              variant="h4"
              component="h1"
              gutterBottom
              fontWeight="700"
              color="primary.main"
              sx={{ mb: 1, letterSpacing: "-0.5px" }}
            >
              Welcome Back
            </Typography>
            <Typography
              variant="body2"
              color="text.secondary"
              sx={{ textAlign: "center" }}
            >
              Sign in to CAPEX Hub to continue
            </Typography>
            <Box
              component="form"
              onSubmit={handleSubmit}
              sx={{ width: "100%", mt: 1 }}
            >
              <TextField
                label="Username"
                type="text"
                fullWidth
                required
                margin="normal"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                sx={{ mb: 2 }}
                variant="outlined"
                size="small"
              />
              <TextField
                label="Password"
                type="password"
                fullWidth
                required
                margin="normal"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                sx={{ mb: 3 }}
                variant="outlined"
                size="small"
              />
              <Button
                variant="contained"
                size="small"
                type="submit"
                fullWidth
                disabled={loading}
                sx={{
                  mb: 2,
                  fontSize: "1rem",
                  textTransform: "none",
                  borderRadius: 2,
                  bgcolor: "primary.main",
                  "&:hover": {
                    bgcolor: "primary.dark",
                  },
                }}
              >
                {loading ? (
                  <CircularProgress size={24} color="inherit" />
                ) : (
                  "Sign In"
                )}
              </Button>
            </Box>
            <Typography
              variant="body2"
              color="text.secondary"
              sx={{ textAlign: "center" }}
            >
              Renuks Group IT © {new Date().getFullYear()}
            </Typography>
          </Paper>
        </Container>
      </Box>
    </React.Fragment>
  );
}

export default LoginPage;
