import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { LocationItem } from "../../features/masterdata/types";

interface FormDropdownLocationProps {
  locations: LocationItem[];
  locationId: number;
  onLocationChange: (value: number) => void;
}

export const FormDropdownLocation = ({
  locations,
  locationId,
  onLocationChange,
}: FormDropdownLocationProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small" required>
      <InputLabel id="location-label">Asset Location</InputLabel>
      <Select
        labelId="location-label"
        label="Asset Location"
        value={locationId}
        onChange={(e) => onLocationChange(Number(e.target.value))}
      >
        {locations
          .filter(
            (location) =>
              location.IsActive === true && location.IsStockLocation === true,
          )
          .map((location) => (
            <MenuItem key={location.Id} value={location.Id}>
              {location.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
