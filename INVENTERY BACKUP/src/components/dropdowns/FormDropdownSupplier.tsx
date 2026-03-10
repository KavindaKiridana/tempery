import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { SupplierItem } from "../../features/masterdata/types";

interface FormDropdownSupplierProps {
  suppliers: SupplierItem[];
  supplierId: number;
  onSupplierChange: (value: number) => void;
}

export const FormDropdownSupplier = ({
  suppliers,
  supplierId,
  onSupplierChange,
}: FormDropdownSupplierProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small" required>
      <InputLabel id="supplier-label">Supplier</InputLabel>
      <Select
        labelId="supplier-label"
        label="Supplier"
        value={supplierId}
        onChange={(e) => onSupplierChange(Number(e.target.value))}
      >
        {suppliers
          .filter((supplier) => supplier.IsActive === true)
          .map((supplier) => (
            <MenuItem key={supplier.Id} value={supplier.Id}>
              {supplier.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
