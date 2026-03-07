import { useEffect, useState } from "react";

import { useNavigate, useParams } from "react-router-dom";

import { Grid, CircularProgress, Button } from "@mui/material";

import { FormLayout } from "@components/layout/FormLayout";
import { FormTabDesktop } from "@features/assets/components/FormTabDesktop";
import { FormTabLaptop } from "@features/assets/components/FormTabLaptop";
import { FormTabServer } from "@features/assets/components/FormTabServer";
import { useMasterData } from "@features/assets/services/useMasterData";

import { getAssetInfo, updateAsset } from "../services/assetEditService";

import type {
  EditAssetView,
  CreateAssetRequest,
  CreateCommonAssetRequest,
} from "../types";
import { LoadingError } from "@components/layout/LoadingError";

function EditComputerForm() {
  const navigate = useNavigate();
  const { id } = useParams();

  const { data, loading, error: masterDataError } = useMasterData();

  const [formData, setFormData] = useState<EditAssetView | null>(null);

  // Selected ids / fields
  const [type, setType] = useState<string>("");
  const [osId, setOsId] = useState<number>(0);
  const [processorId, setProcessorId] = useState<number>(0);
  const [ramSizeId, setRamSizeId] = useState<number>(0);
  const [ramTypeId, setRamTypeId] = useState<number>(0);
  const [hddId, setHddId] = useState<number>(0);
  const [ssdId, setSsdId] = useState<number>(0);
  const [make, setMake] = useState<string>("");
  const [windowsLicenseKey, setWindowsLicenseKey] = useState<string>("");
  const [motherboard, setMotherboard] = useState<string>("");
  const [modelId, setModelId] = useState<number>(0);
  const [displayId, setDisplayId] = useState<number>(0);
  const [powerSupply, setPowerSupply] = useState<boolean>(false);
  const [raidSupport, setRaidSupport] = useState<boolean>(false);

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  useEffect(() => {
    const getPastData = async () => {
      const data = await getAssetInfo(id!);
      setFormData(data);
    };
    getPastData();
  }, [id]);

  useEffect(() => {
    if (formData) {
      setOsId(formData.OsId || 0);
      setProcessorId(formData.PId || 0);
      setRamSizeId(formData.RAMSId || 0);
      setType(formData.Type || "SparePart");
      setRamTypeId(formData.RAMTId || 0);
      setHddId(formData.HDDId || 0);
      setSsdId(formData.SSDId || 0);
      setMake(formData.Make || "");
      setWindowsLicenseKey(formData.WindowsKey || "");
      setMotherboard(formData.Motherboard || "");
      setModelId(formData.ModelId || 0);
      setDisplayId(formData.DisplayId || 0);
      setPowerSupply(formData.PowerSupply || false);
      setRaidSupport(formData.RAIDSupport || false);
    }
  }, [formData]);

  const validateForm = (): string | null => {
    if (!make.trim()) return "Make is required.";
    if (modelId === 0) return "Model is required.";
    return null;
  };

  const saveCommonData = async (): Promise<boolean> => {
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return false;
    }

    setIsSubmitting(true);
    setSubmitError(null);

    try {
      const updateData: CreateAssetRequest = {
        patchRequestType: "computer",
        AssetId: id!,
        Type: type,
        OsId: osId,
        PId: processorId,
        RAMSId: ramSizeId,
        RAMTId: ramTypeId,
        HDDId: hddId,
        SSDId: ssdId,
        Make: make,
        WindowsKey: windowsLicenseKey,
        Motherboard: motherboard,
        ModelId: modelId,
        DisplayId: displayId || undefined,
        PowerSupply: powerSupply,
        RAIDSupport: raidSupport,
      };
      const error = await updateAsset(
        updateData as unknown as CreateCommonAssetRequest,
      );
      if (error) {
        setSubmitError(error);
        return false;
      }
      return true;
    } catch (err) {
      console.error("Error during saveCommonData:", err);
      setSubmitError("An unexpected error occurred. Please try again.");
      return false;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleAddAsset = async () => {
    const ok = await saveCommonData();
    if (ok) {
      navigate(`/viewAsset`);
    }
  };

  const goBack = () => {
    navigate(`/viewAsset`);
  };

  return (
    <LoadingError loading={loading} error={masterDataError}>
      <FormLayout title={`Edit ${type} Info`}>
        <Grid container spacing={3}>
          {type === "Desktop" && (
            <FormTabDesktop
              windowsLicenseKey={windowsLicenseKey}
              onWindowsLicenseKeyChange={setWindowsLicenseKey}
              oses={data.oses}
              osId={osId}
              onOSChange={setOsId}
              make={make}
              onMakeChange={setMake}
              motherboard={motherboard}
              onMotherboardChange={setMotherboard}
              processors={data.processors}
              processorId={processorId}
              onProcessorChange={setProcessorId}
              ramtypes={data.ramTypes}
              ramtypeId={ramTypeId}
              onRAMTypeChange={setRamTypeId}
              ramsizes={data.ramSizes}
              ramsizeId={ramSizeId}
              onRAMSizeChange={setRamSizeId}
              ssds={data.ssds}
              ssdId={ssdId}
              onSSDChange={setSsdId}
              hdds={data.hdds}
              hddId={hddId}
              onHDDChange={setHddId}
              models={data.models}
              modelId={modelId}
              onModelChange={setModelId}
              displays={data.displays}
              displayId={displayId}
              onDisplayChange={setDisplayId}
            />
          )}
          {type === "Laptop" && (
            <FormTabLaptop
              windowsLicenseKey={windowsLicenseKey}
              onWindowsLicenseKeyChange={setWindowsLicenseKey}
              oses={data.oses}
              osId={osId}
              onOSChange={setOsId}
              make={make}
              onMakeChange={setMake}
              motherboard={motherboard}
              onMotherboardChange={setMotherboard}
              processors={data.processors}
              processorId={processorId}
              onProcessorChange={setProcessorId}
              ramtypes={data.ramTypes}
              ramtypeId={ramTypeId}
              onRAMTypeChange={setRamTypeId}
              ramsizes={data.ramSizes}
              ramsizeId={ramSizeId}
              onRAMSizeChange={setRamSizeId}
              ssds={data.ssds}
              ssdId={ssdId}
              onSSDChange={setSsdId}
              hdds={data.hdds}
              hddId={hddId}
              onHDDChange={setHddId}
              models={data.models}
              modelId={modelId}
              onModelChange={setModelId}
            />
          )}
          {type === "Server" && (
            <FormTabServer
              windowsLicenseKey={windowsLicenseKey}
              onWindowsLicenseKeyChange={setWindowsLicenseKey}
              oses={data.oses}
              osId={osId}
              onOSChange={setOsId}
              make={make}
              onMakeChange={setMake}
              motherboard={motherboard}
              onMotherboardChange={setMotherboard}
              processors={data.processors}
              processorId={processorId}
              onProcessorChange={setProcessorId}
              ramtypes={data.ramTypes}
              ramtypeId={ramTypeId}
              onRAMTypeChange={setRamTypeId}
              ramsizes={data.ramSizes}
              ramsizeId={ramSizeId}
              onRAMSizeChange={setRamSizeId}
              ssds={data.ssds}
              ssdId={ssdId}
              onSSDChange={setSsdId}
              hdds={data.hdds}
              hddId={hddId}
              onHDDChange={setHddId}
              models={data.models}
              modelId={modelId}
              onModelChange={setModelId}
              powerSupply={powerSupply}
              onPowerSupplyChange={setPowerSupply}
              raidSupport={raidSupport}
              onRaidSupportChange={setRaidSupport}
            />
          )}

          {/* Buttons Row */}
          <Grid size={12} container justifyContent="space-between">
            <Button
              onClick={goBack}
              variant="contained"
              size="large"
              color="warning"
              sx={{ minWidth: 200 }}
            >
              Go Back
            </Button>
            <Button
              variant="contained"
              size="large"
              color="success"
              sx={{ minWidth: 200 }}
              onClick={handleAddAsset}
            >
              {isSubmitting ? <CircularProgress size={24} /> : "Save"}
            </Button>
            {submitError && (
              <div
                style={{ color: "red", textAlign: "center", marginTop: "16px" }}
              >
                {submitError}
              </div>
            )}
          </Grid>
        </Grid>
      </FormLayout>{" "}
    </LoadingError>
  );
}

export default EditComputerForm;
