import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { ProcessorItem } from "../../features/masterdata/types";

interface FormDropdownProcessorProps {
  processors: ProcessorItem[];
  processorId: number;
  onProcessorChange: (value: number) => void;
}

export const FormDropdownProcessor = ({
  processors,
  processorId,
  onProcessorChange,
}: FormDropdownProcessorProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small">
      <InputLabel id="Processor-label">Processor</InputLabel>
      <Select
        labelId="Processor-label"
        label="Processor"
        value={processorId}
        onChange={(e) => onProcessorChange(Number(e.target.value))}
      >
        {processors
          .filter((processor) => processor.IsActive === true)
          .map((processor) => (
            <MenuItem key={processor.Id} value={processor.Id}>
              {processor.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
