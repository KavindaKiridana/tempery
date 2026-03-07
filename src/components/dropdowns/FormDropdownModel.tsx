import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { ModelItem } from "../../features/masterdata/types";

interface FormDropdownModelProps {
  models: ModelItem[];
  modelId: number;
  onModelChange: (value: number) => void;
}

export const FormDropdownModel = ({
  models,
  modelId,
  onModelChange,
}: FormDropdownModelProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small">
      <InputLabel id="model-label">Model</InputLabel>
      <Select
        labelId="model-label"
        label="Model"
        value={modelId}
        onChange={(e) => onModelChange(Number(e.target.value))}
      >
        {models
          .filter((model) => model.IsActive === true)
          .map((model) => (
            <MenuItem key={model.Id} value={model.Id}>
              {model.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
