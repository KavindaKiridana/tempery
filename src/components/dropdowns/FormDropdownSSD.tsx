import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { SSDItem } from "../../features/masterdata/types";

interface FormDropdownSSDProps {
  ssds: SSDItem[];
  ssdId: number;
  onSSDChange: (value: number) => void;
}

export const FormDropdownSSD = ({
  ssds,
  ssdId,
  onSSDChange,
}: FormDropdownSSDProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small">
      <InputLabel id="ssd-label">SSD</InputLabel>
      <Select
        labelId="ssd-label"
        label="SSD"
        value={ssdId}
        onChange={(e) => onSSDChange(Number(e.target.value))}
      >
        {ssds
          .filter((ssd) => ssd.IsActive === true)
          .map((ssd) => (
            <MenuItem key={ssd.Id} value={ssd.Id}>
              {ssd.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
