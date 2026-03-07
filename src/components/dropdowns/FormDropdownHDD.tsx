import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { HDDItem } from "../../features/masterdata/types";

interface FormDropdownHDDProps {
  hdds: HDDItem[];
  hddId: number;
  onHDDChange: (value: number) => void;
}

export const FormDropdownHDD = ({
  hdds,
  hddId,
  onHDDChange,
}: FormDropdownHDDProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small">
      <InputLabel id="hdd-label">HDD</InputLabel>
      <Select
        labelId="hdd-label"
        label="HDD"
        value={hddId}
        onChange={(e) => onHDDChange(Number(e.target.value))}
      >
        {hdds
          .filter((hdd) => hdd.IsActive)
          .map((hdd) => (
            <MenuItem key={hdd.Id} value={hdd.Id}>
              {hdd.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
