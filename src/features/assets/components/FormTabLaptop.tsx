import { Grid, TextField } from "@mui/material";

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
  models: ModelItem[];
  modelId: number;
  onModelChange: (value: number) => void;
}

export const FormTabLaptop = ({
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
  models,
  modelId,
  onModelChange,
}: FormCommonProps) => (
  <>
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
    <FormDropdownOS oses={oses} osId={osId} onOSChange={onOSChange} />
    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Make"
        type="text"
        size="small"
        fullWidth
        value={make}
        onChange={(e) => onMakeChange(e.target.value)}
      />
    </Grid>

    <Grid size={{ xs: 12, sm: 6 }}>
      <TextField
        label="Motherboard"
        type="text"
        size="small"
        fullWidth
        value={motherboard}
        onChange={(e) => onMotherboardChange(e.target.value)}
      />
    </Grid>
    <FormDropdownProcessor
      processors={processors}
      processorId={processorId}
      onProcessorChange={onProcessorChange}
    />
    <FormDropdownRAMType
      ramtypes={ramtypes}
      ramtypeId={ramtypeId}
      onRAMTypeChange={onRAMTypeChange}
    />
    <FormDropdownRAMSize
      ramsizes={ramsizes}
      ramsizeId={ramsizeId}
      onRAMSizeChange={onRAMSizeChange}
    />
    <FormDropdownSSD ssds={ssds} ssdId={ssdId} onSSDChange={onSSDChange} />
    <FormDropdownHDD hdds={hdds} hddId={hddId} onHDDChange={onHDDChange} />
    <FormDropdownModel
      models={models}
      modelId={modelId}
      onModelChange={onModelChange}
    />
  </>
);
