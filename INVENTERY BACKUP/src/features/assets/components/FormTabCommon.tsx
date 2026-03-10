import dayjs from "dayjs";
import {
  Grid,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from "@mui/material";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { FormDropdownCompany } from "../../../components/dropdowns/FormDropdownCompany";
import { FormDropdownLocation } from "../../../components/dropdowns/FormDropdownLocation";
import { FormDropdownSupplier } from "../../../components/dropdowns/FormDropdownSupplier";
import type {
  CompanyItem,
  LocationItem,
  SupplierItem,
} from "../../masterdata/types";

interface FormCommonProps {
  purchaseDate: string;
  onPurchaseDateChange: (value: string) => void;
  financeAssetCode: string;
  onFinanceAssetCodeChange: (value: string) => void;
  warranty: string;
  onWarrantyChange: (value: string) => void;
  companies: CompanyItem[];
  companyId: number;
  onCompanyChange: (value: number) => void;
  locations: LocationItem[];
  locationId: number;
  onLocationChange: (value: number) => void;
  brandnew: boolean;
  onbrandnewChange: (value: boolean) => void;
  cost: string;
  onCostChange: (value: string) => void;
  name: string;
  onNameChange: (value: string) => void;
  suppliers: SupplierItem[];
  supplierId: number;
  onSupplierChange: (value: number) => void;
  ipAddress: string;
  onipAdressChange: (value: string) => void;
}

export const FormTabCommon = ({
  purchaseDate,
  onPurchaseDateChange,
  financeAssetCode,
  onFinanceAssetCodeChange,
  warranty,
  onWarrantyChange,
  companies,
  companyId,
  onCompanyChange,
  locations,
  locationId,
  onLocationChange,
  brandnew,
  onbrandnewChange,
  cost,
  onCostChange,
  name,
  onNameChange,
  suppliers,
  supplierId,
  onSupplierChange,
  ipAddress,
  onipAdressChange,
}: FormCommonProps) => (
  <>
    {/* Purchase Date */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <LocalizationProvider dateAdapter={AdapterDayjs}>
        <DatePicker
          label="Purchase Date"
          slotProps={{ textField: { size: "small", fullWidth: true } }}
          value={purchaseDate ? dayjs(purchaseDate) : null}
          onChange={(newValue) =>
            onPurchaseDateChange(newValue ? newValue.format("YYYY-MM-DD") : "")
          }
        />
      </LocalizationProvider>
    </Grid>

    {/* Finance Asset Code */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Finance Asset Code"
        type="text"
        size="small"
        fullWidth
        value={financeAssetCode}
        onChange={(e) => onFinanceAssetCodeChange(e.target.value)}
      />
    </Grid>

    {/* Warranty */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Warranty (months)"
        type="number"
        size="small"
        fullWidth
        value={warranty}
        onChange={(e) => onWarrantyChange(e.target.value)}
      />
    </Grid>

    {/* Company dropdown */}
    <FormDropdownCompany
      companies={companies}
      companyId={companyId}
      onCompanyChange={onCompanyChange}
    />

    {/* Location dropdown */}
    <FormDropdownLocation
      locations={locations}
      locationId={locationId}
      onLocationChange={onLocationChange}
    />

    {/* Brand New */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <FormControl fullWidth size="small" required>
        <InputLabel id="brandnew-label">Brand New</InputLabel>
        <Select
          labelId="brandnew-label"
          label="Brand New"
          value={brandnew}
          onChange={(e) => onbrandnewChange(e.target.value === "true")}
        >
          <MenuItem value="true">Yes</MenuItem>
          <MenuItem value="false">No</MenuItem>
        </Select>
      </FormControl>
    </Grid>

    {/* Cost */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Cost"
        type="number"
        size="small"
        fullWidth
        value={cost}
        onChange={(e) => onCostChange(e.target.value)}
      />
    </Grid>

    {/* Name */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Name"
        type="text"
        size="small"
        fullWidth
        required
        value={name}
        onChange={(e) => onNameChange(e.target.value)}
      />
    </Grid>

    {/* Supplier dropdown */}
    <FormDropdownSupplier
      suppliers={suppliers}
      supplierId={supplierId}
      onSupplierChange={onSupplierChange}
    />

    {/* IP Address */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="IP Address"
        type="text"
        size="small"
        fullWidth
        value={ipAddress}
        onChange={(e) => onipAdressChange(e.target.value)}
      />
    </Grid>
  </>
);