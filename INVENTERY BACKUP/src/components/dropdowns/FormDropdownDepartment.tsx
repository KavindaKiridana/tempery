import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { DepartmentItem } from "../../features/masterdata/types";

interface FormDropdownDepartmentProps {
  departments: DepartmentItem[];
  departmentId: number;
  onDepartmentChange: (value: number) => void;
}

export const FormDropdownDepartment = ({
  departments,
  departmentId,
  onDepartmentChange,
}: FormDropdownDepartmentProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small" required>
      <InputLabel id="department-label">Department</InputLabel>
      <Select
        labelId="department-label"
        label="Department"
        value={departmentId}
        onChange={(e) => onDepartmentChange(Number(e.target.value))}
      >
        {departments
          .filter((department) => department.IsActive === true)
          .map((department) => (
            <MenuItem key={department.Id} value={department.Id}>
              {department.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
