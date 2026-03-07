import { Box, Container, Paper, Typography } from "@mui/material";

interface FormLayoutProps {
  title: string;
  children: React.ReactNode;
}

export const FormLayout = ({ title, children }: FormLayoutProps) => (
  <Box
    sx={{
      display: "flex",
      alignItems: "flex-start",
      justifyContent: "center",
      bgcolor: "background.default",
      p: { xs: 1, sm: 2 },
    }}
  >
    <Container maxWidth="md">
      <Paper
        elevation={3}
        sx={{
          p: { xs: 3, sm: 4 },
          borderRadius: 2,
        }}
      >
        <Typography
          variant="h5"
          component="h1"
          gutterBottom
          align="center"
          fontWeight="medium"
          color="primary.main"
        >
          {title}
        </Typography>
        {children}
      </Paper>
    </Container>
  </Box>
);
