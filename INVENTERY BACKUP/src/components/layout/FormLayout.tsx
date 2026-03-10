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
          p: { xs: 2, sm: 3 }, // reduced from xs:3/sm:4 to cut excess whitespace
          borderRadius: 2,
        }}
      >
        <Typography
          variant="h5"
          component="h1"
          align="center"
          fontWeight="medium"
          color="primary.main"
          sx={{ mb: 1.5 }} // tighter gap between title and form fields
        >
          {title}
        </Typography>
        {children}
      </Paper>
    </Container>
  </Box>
);