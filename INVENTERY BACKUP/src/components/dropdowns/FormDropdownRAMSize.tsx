import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { RAMSizeItem } from "../../features/masterdata/types";

interface FormDropdownRAMSizeProps {
  ramsizes: RAMSizeItem[];
  ramsizeId: number;
  onRAMSizeChange: (value: number) => void;
}

export const FormDropdownRAMSize = ({
  ramsizes,
  ramsizeId,
  onRAMSizeChange,
}: FormDropdownRAMSizeProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small">
      <InputLabel id="ram-size-label">RAM Size</InputLabel>
      <Select
        labelId="ram-size-label"
        label="RAM Size"
        value={ramsizeId}
        onChange={(e) => onRAMSizeChange(Number(e.target.value))}
      >
        {ramsizes
          .filter((ramsize) => ramsize.IsActive === true)
          .map((ramsize) => (
            <MenuItem key={ramsize.Id} value={ramsize.Id}>
              {ramsize.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
