import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { OSItem } from "../../features/masterdata/types";

interface FormDropdownOSProps {
  oses: OSItem[];
  osId: number;
  onOSChange: (value: number) => void;
}

export const FormDropdownOS = ({
  oses,
  osId,
  onOSChange,
}: FormDropdownOSProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small">
      <InputLabel id="os-label">OS</InputLabel>
      <Select
        labelId="os-label"
        label="OS"
        value={osId}
        onChange={(e) => onOSChange(Number(e.target.value))}
      >
        {oses
          .filter((os) => os.IsActive)
          .map((os) => (
            <MenuItem key={os.Id} value={os.Id}>
              {os.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
