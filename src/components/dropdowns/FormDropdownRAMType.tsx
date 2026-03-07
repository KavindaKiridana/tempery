import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { RAMTypeItem } from "../../features/masterdata/types";

interface FormDropdownRAMTypeProps {
  ramtypes: RAMTypeItem[];
  ramtypeId: number;
  onRAMTypeChange: (value: number) => void;
}

export const FormDropdownRAMType = ({
  ramtypes,
  ramtypeId,
  onRAMTypeChange,
}: FormDropdownRAMTypeProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small">
      <InputLabel id="ram-type-label">RAM Type</InputLabel>
      <Select
        labelId="ram-type-label"
        label="RAM Type"
        value={ramtypeId}
        onChange={(e) => onRAMTypeChange(Number(e.target.value))}
      >
        {ramtypes
          .filter((ramtype) => ramtype.IsActive === true)
          .map((ramtype) => (
            <MenuItem key={ramtype.Id} value={ramtype.Id}>
              {ramtype.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
