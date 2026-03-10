import { useEffect, useState } from "react";

import {
  Box,
  TextField,
  Button,
  Typography,
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Select,
  MenuItem,
  InputLabel,
  Card,
  CardContent,
  FormControl,
} from "@mui/material";

import { addType, getTypes } from "../services/typeService";

import type { TypeItem } from "@features/masterdata/types";

function AddType() {
  const [typeName, setTypeName] = useState("");
  const [categoryName, setCategoryName] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [types, setTypes] = useState<TypeItem[]>([]);

  // Fetch Types on component mount
  useEffect(() => {
    const fetchTypes = async () => {
      const data = await getTypes();
      setTypes(data);
    };
    fetchTypes();
  }, []);

  const handleSubmit = async () => {
    if (!typeName) {
      setSubmitError("Please enter a Type name.");
      return;
    }
    if (!categoryName) {
      setSubmitError("Please enter a Category name.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const TypeData: TypeItem = {
      AssetType: typeName,
      Category: categoryName,
      IsActive: true,
    };
    const result = await addType(TypeData);
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setTypeName("");
      setCategoryName("");
      const data = await getTypes();
      setTypes(data);
    }
  };

  return (
    <Card>
      <CardContent sx={{ p: 1 }}>
        <Box sx={{ mb: 1 }}>
          <Box sx={{ display: "flex", gap: 2, alignItems: "flex-start" }}>
            <TextField
              fullWidth
              label="Type"
              value={typeName}
              onChange={(e) => setTypeName(e.target.value)}
              error={!!submitError}
              helperText={submitError}
              required
            />
            <FormControl fullWidth size="small" required>
              <InputLabel id="category-label">Category</InputLabel>
              <Select
                labelId="category-label"
                label="Category"
                value={categoryName}
                onChange={(e) => setCategoryName(e.target.value)}
              >
                <MenuItem value="MainAsset">MainAsset</MenuItem>
                <MenuItem value="Consumables">Consumables</MenuItem>
                <MenuItem value="SparePart">Spare Part</MenuItem>
              </Select>
            </FormControl>
            <Button
              variant="contained"
              onClick={handleSubmit}
              disabled={isSubmitting}
              sx={{ minWidth: "120px" }}
            >
              {isSubmitting ? <CircularProgress size={24} /> : "Add"}
            </Button>
          </Box>
        </Box>
        <Typography variant="h6" gutterBottom>
          Existing Items ({types.length})
        </Typography>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Type</TableCell>
                <TableCell>Category</TableCell>
                <TableCell>Active</TableCell>
                <TableCell>Added By</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {types.length > 0 ? (
                types.map((type, index) => (
                  <TableRow>
                    <TableCell>{type.AssetType}</TableCell>
                    <TableCell>{type.Category}</TableCell>
                    <TableCell>{type.IsActive ? "Yes" : "No"}</TableCell>
                    <TableCell>{type.AddedUserName}</TableCell>
                    <TableCell>
                      {/* Add action buttons here if needed */}
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={5} align="center">
                    No items found.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </CardContent>
    </Card>
  );
}
export default AddType;
