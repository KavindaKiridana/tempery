import {
  Grid,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from "@mui/material";
import { FormDropdownModel } from "@components/dropdowns/FormDropdownModel";
import { FormDropdownHDD } from "../../../components/dropdowns/FormDropdownHDD";
import { FormDropdownOS } from "../../../components/dropdowns/FormDropdownOS";
import { FormDropdownProcessor } from "../../../components/dropdowns/FormDropdownProcessor";
import { FormDropdownRAMSize } from "../../../components/dropdowns/FormDropdownRAMSize";
import { FormDropdownRAMType } from "../../../components/dropdowns/FormDropdownRAMType";
import { FormDropdownSSD } from "../../../components/dropdowns/FormDropdownSSD";
import type {
  HDDItem,
  ModelItem,
  OSItem,
  ProcessorItem,
  RAMSizeItem,
  RAMTypeItem,
  SSDItem,
} from "../../masterdata/types";

interface FormCommonProps {
  windowsLicenseKey: string;
  onWindowsLicenseKeyChange: (value: string) => void;
  oses: OSItem[];
  osId: number;
  onOSChange: (value: number) => void;
  make: string;
  onMakeChange: (value: string) => void;
  motherboard: string;
  onMotherboardChange: (value: string) => void;
  processors: ProcessorItem[];
  processorId: number;
  onProcessorChange: (value: number) => void;
  ramtypes: RAMTypeItem[];
  ramtypeId: number;
  onRAMTypeChange: (value: number) => void;
  ramsizes: RAMSizeItem[];
  ramsizeId: number;
  onRAMSizeChange: (value: number) => void;
  ssds: SSDItem[];
  ssdId: number;
  onSSDChange: (value: number) => void;
  hdds: HDDItem[];
  hddId: number;
  onHDDChange: (value: number) => void;
  powerSupply: boolean;
  onPowerSupplyChange: (value: boolean) => void;
  raidSupport: boolean;
  onRaidSupportChange: (value: boolean) => void;
  models: ModelItem[];
  modelId: number;
  onModelChange: (value: number) => void;
}

export const FormTabServer = ({
  windowsLicenseKey,
  onWindowsLicenseKeyChange,
  oses,
  osId,
  onOSChange,
  make,
  onMakeChange,
  motherboard,
  onMotherboardChange,
  processors,
  processorId,
  onProcessorChange,
  ramtypes,
  ramtypeId,
  onRAMTypeChange,
  ramsizes,
  ramsizeId,
  onRAMSizeChange,
  ssds,
  ssdId,
  onSSDChange,
  hdds,
  hddId,
  onHDDChange,
  powerSupply,
  onPowerSupplyChange,
  raidSupport,
  onRaidSupportChange,
  models,
  modelId,
  onModelChange,
}: FormCommonProps) => (
  <>
    {/* Windows License Key */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Windows License Key"
        type="text"
        size="small"
        fullWidth
        value={windowsLicenseKey}
        onChange={(e) => onWindowsLicenseKeyChange(e.target.value)}
      />
    </Grid>

    {/* OS dropdown */}
    <FormDropdownOS oses={oses} osId={osId} onOSChange={onOSChange} />

    {/* Make */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Make"
        type="text"
        size="small"
        fullWidth
        required
        value={make}
        onChange={(e) => onMakeChange(e.target.value)}
      />
    </Grid>

    {/* Motherboard */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Motherboard"
        type="text"
        size="small"
        fullWidth
        required
        value={motherboard}
        onChange={(e) => onMotherboardChange(e.target.value)}
      />
    </Grid>

    {/* Processor dropdown */}
    <FormDropdownProcessor
      processors={processors}
      processorId={processorId}
      onProcessorChange={onProcessorChange}
    />

    {/* RAM Type dropdown */}
    <FormDropdownRAMType
      ramtypes={ramtypes}
      ramtypeId={ramtypeId}
      onRAMTypeChange={onRAMTypeChange}
    />

    {/* RAM Size dropdown */}
    <FormDropdownRAMSize
      ramsizes={ramsizes}
      ramsizeId={ramsizeId}
      onRAMSizeChange={onRAMSizeChange}
    />

    {/* SSD dropdown */}
    <FormDropdownSSD ssds={ssds} ssdId={ssdId} onSSDChange={onSSDChange} />

    {/* HDD dropdown */}
    <FormDropdownHDD hdds={hdds} hddId={hddId} onHDDChange={onHDDChange} />

    {/* Power Supply — Yes/No select */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <FormControl fullWidth size="small" required>
        <InputLabel id="power-label">Power Supply</InputLabel>
        <Select
          labelId="power-label"
          label="Power Supply"
          value={powerSupply}
          onChange={(e) => onPowerSupplyChange(e.target.value === "true")}
        >
          <MenuItem value="true">Yes</MenuItem>
          <MenuItem value="false">No</MenuItem>
        </Select>
      </FormControl>
    </Grid>

    {/* RAID Support — Yes/No select */}
    <Grid size={{ xs: 12, sm: 6 }}>
      <FormControl fullWidth size="small" required>
        <InputLabel id="raid-label">Raid Support</InputLabel>
        <Select
          labelId="raid-label"
          label="Raid Support"
          value={raidSupport}
          onChange={(e) => onRaidSupportChange(e.target.value === "true")}
        >
          <MenuItem value="true">Yes</MenuItem>
          <MenuItem value="false">No</MenuItem>
        </Select>
      </FormControl>
    </Grid>

    {/* Model dropdown */}
    <FormDropdownModel
      models={models}
      modelId={modelId}
      onModelChange={onModelChange}
    />
  </>
);
