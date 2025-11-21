import {
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Box,
  CardActions,
  CardContent,
  Typography,
  Card,
} from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

function HomePage() {
  const navigate = useNavigate();

  function handleTypeChange(event: any) {
    const value = Number(event.target.value); // Convert string → number.

    // Navigate immediately
    if (value === 1) navigate("/ActivityOne");
    if (value === 2) navigate("/ActivityTwo");
  }
  return (
    <Box
      component="span"
      sx={{ display: "inline-block", mx: "2px", transform: "scale(0.8)" }}
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "80vh",
      }}
    >
      <Card sx={{ minWidth: 275 }}>
        <CardContent>
          <Typography
            gutterBottom
            sx={{ color: "text.secondary", fontSize: 14 }}
          >
            TS Practices
          </Typography>
        </CardContent>
        <CardActions>
          <FormControl fullWidth size="small">
            <InputLabel id="type-label">View More</InputLabel>
            <Select
              labelId="type-label"
              label="Type"
              onChange={handleTypeChange}
            >
              <MenuItem value={1}>one</MenuItem>
              <MenuItem value={2}>two</MenuItem>
            </Select>
          </FormControl>
        </CardActions>
      </Card>
    </Box>
  );
}

export default HomePage;
