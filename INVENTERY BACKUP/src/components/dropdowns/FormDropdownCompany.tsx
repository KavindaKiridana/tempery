import { Grid, FormControl, InputLabel, Select, MenuItem } from "@mui/material";

import type { CompanyItem } from "../../features/masterdata/types";

interface FormDropdownCompanyProps {
  companies: CompanyItem[];
  companyId: number;
  onCompanyChange: (value: number) => void;
}

export const FormDropdownCompany = ({
  companies,
  companyId,
  onCompanyChange,
}: FormDropdownCompanyProps) => (
  <Grid size={{ xs: 12, sm: 6 }}>
    <FormControl fullWidth size="small" required>
      <InputLabel id="company-label">Company</InputLabel>
      <Select
        labelId="company-label"
        label="Company"
        value={companyId}
        onChange={(e) => onCompanyChange(Number(e.target.value))}
      >
        {companies
          .filter((company) => company.IsActive)
          .map((company) => (
            <MenuItem key={company.Id} value={company.Id}>
              {company.Name}
            </MenuItem>
          ))}
      </Select>
    </FormControl>
  </Grid>
);
