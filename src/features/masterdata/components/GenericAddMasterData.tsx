import { useState } from "react";

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
  Card,
  CardContent,
  Checkbox,
} from "@mui/material";

interface GenericAddMasterDataProps {
  title: string;
  label: string;
  inputValue: string;
  onInputChange: (value: string) => void;
  onSubmit: () => void;
  isSubmitting: boolean;
  error: string | null;
  items: {
    Id?: number;
    Name: string;
    IsUsed?: boolean;
    IsActive?: boolean;
  }[];
  onUpdate: (item: {
    Id?: number;
    Name: string;
    IsUsed?: boolean;
    IsActive?: boolean;
  }) => void;
}

// Create a separate component for each row
function TableRowItem({
  item,
  onUpdate,
}: {
  item: {
    Id?: number;
    Name: string;
    IsUsed?: boolean;
    IsActive?: boolean;
  };
  onUpdate: (item: {
    Id?: number;
    Name: string;
    IsUsed?: boolean;
    IsActive?: boolean;
  }) => void;
}) {
  const [isEditing, setIsEditing] = useState(false);
  const [editName, setEditName] = useState(item.Name);
  const [editIsActive, setEditIsActive] = useState(item.IsActive || false);

  const handleSave = async () => {
    const updatedItem = {
      Id: item.Id,
      Name: editName,
      IsActive: editIsActive,
      IsUsed: item.IsUsed,
    };
    await onUpdate(updatedItem);
    setIsEditing(false);
  };

  return (
    <TableRow hover>
      <TableCell sx={{ padding: "0" }}>
        {isEditing && !item.IsUsed ? (
          <TextField
            fullWidth
            value={editName}
            onChange={(e) => setEditName(e.target.value)}
            disabled={item.IsUsed}
          />
        ) : (
          item.Name
        )}
      </TableCell>
      <TableCell sx={{ padding: "0" }}>
        <Checkbox
          checked={editIsActive}
          onChange={(e) => setEditIsActive(e.target.checked)}
          disabled={!isEditing}
        />
      </TableCell>
      <TableCell sx={{ padding: "0" }}>
        {isEditing ? (
          <Button onClick={handleSave}>Save</Button>
        ) : (
          <Button onClick={() => setIsEditing(true)}>Update</Button>
        )}
      </TableCell>
    </TableRow>
  );
}

export default function GenericAddMasterData({
  title,
  label,
  inputValue,
  onInputChange,
  onSubmit,
  isSubmitting,
  error,
  items,
  onUpdate,
}: GenericAddMasterDataProps) {
  return (
    <Card>
      <CardContent sx={{ p: 1 }}>
        {/* <Typography variant="h5" gutterBottom>
            {title}
          </Typography> */}
        <Box sx={{ mb: 1 }}>
          <Box sx={{ display: "flex", gap: 2, alignItems: "flex-start" }}>
            <TextField
              fullWidth
              label={label}
              value={inputValue}
              onChange={(e) => onInputChange(e.target.value)}
              error={!!error}
              helperText={error}
            />
            <Button
              variant="contained"
              onClick={onSubmit}
              disabled={isSubmitting}
              sx={{ minWidth: "120px" }}
            >
              {isSubmitting ? <CircularProgress size={24} /> : "Add"}
            </Button>
          </Box>
        </Box>

        <Typography variant="h6" gutterBottom>
          Existing Items ({items.length})
        </Typography>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Active</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {items.length > 0 ? (
                items.map((item, index) => (
                  <TableRowItem
                    key={item.Id || index}
                    item={item}
                    onUpdate={onUpdate}
                  />
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={3} align="center">
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
