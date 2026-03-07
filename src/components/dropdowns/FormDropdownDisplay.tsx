import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { DisplayItem } from "../../features/masterdata/types";

interface FormDropdownDisplayProps {
  displays: DisplayItem[];
  displayId: number;
  onDisplayChange: (value: number) => void;
}

export const FormDropdownDisplay = ({
  displays,
  displayId,
  onDisplayChange,
}: FormDropdownDisplayProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small">
      <InputLabel id=" display-label"> Display</InputLabel>
      <Select
        labelId=" display-label"
        label=" Display"
        value={displayId}
        onChange={(e) => onDisplayChange(Number(e.target.value))}
      >
        {displays
          .filter((display) => display.IsActive === true)
          .map((display) => (
            <MenuItem key={display.Id} value={display.Id}>
              {display.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
