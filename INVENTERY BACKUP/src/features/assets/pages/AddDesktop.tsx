import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Alert, Button, CircularProgress, Grid } from "@mui/material";
import { FormLayout } from "@components/layout/FormLayout";
import { FormTabDesktop } from "@features/assets/components/FormTabDesktop";
import { useMasterData } from "@features/assets/services/useMasterData";
import { modifyAsset } from "../services/assetCrudService";
import type { CreateAssetRequest } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

export const AddDesktop = () => {
  const { id } = useParams();
  const { data, loading, error } = useMasterData();
  const navigate = useNavigate();

  // ─── Form State ────────────────────────────────────────────────────────────
  const [osId, setOsId] = useState<number>(0);
  const [processorId, setProcessorId] = useState<number>(0);
  const [ramSizeId, setRamSizeId] = useState<number>(0);
  const [ramTypeId, setRamTypeId] = useState<number>(0);
  const [hddId, setHddId] = useState<number>(0);
  const [ssdId, setSsdId] = useState<number>(0);
  const [make, setMake] = useState("");
  const [windowsLicenseKey, setWindowsLicenseKey] = useState("");
  const [motherboard, setMotherboard] = useState("");
  const [modelId, setModelId] = useState<number>(0);
  const [displayId, setDisplayId] = useState<number>(0);

  // ─── Submission State ──────────────────────────────────────────────────────
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  // ─── Save Desktop Data ─────────────────────────────────────────────────────
  const saveDesktopData = async (): Promise<string | null> => {
    setIsSubmitting(true);
    setSubmitError(null);
    try {
      const formData: CreateAssetRequest = {
        Type: "desktop",
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
        DisplayId: displayId,
      };
      const assetIdcoming = await modifyAsset(id!, formData);
      sessionStorage.setItem("currentAssetId", assetIdcoming);
      return assetIdcoming;
    } catch (error) {
      setSubmitError(
        error instanceof Error
          ? error.message
          : "Failed to save asset. Please try again.",
      );
      return null;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleAddAsset = async () => {
    if (id) {
      const assetId = await saveDesktopData();
      if (!assetId) {
        console.error("Failed to save desktop info. Cannot proceed.");
        return;
      }
      navigate("/viewAsset");
    } else {
      setSubmitError(
        "Common asset ID is missing. Please go back and complete the previous step.",
      );
    }
  };

  const goBack = () => navigate("/AddAssetForm");

  // ─── Render ────────────────────────────────────────────────────────────────
  return (
    <LoadingError loading={loading} error={error}>
      <FormLayout title="Desktop Info">
        <Grid container spacing={2}>

          {/* Desktop-specific fields */}
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

          {/* Action buttons row */}
          <Grid size={12} container justifyContent="space-between" alignItems="center">
            <Button
              onClick={goBack}
              variant="contained"
              size="large"
              color="warning"
              sx={{ minWidth: 160 }}
            >
              Go Back
            </Button>
            <Button
              variant="contained"
              size="large"
              color="success"
              sx={{ minWidth: 160 }}
              onClick={handleAddAsset}
              disabled={isSubmitting}
            >
              {isSubmitting ? <CircularProgress size={22} /> : "Add Asset"}
            </Button>
          </Grid>

          {/* Submission error */}
          {submitError && (
            <Grid size={12}>
              <Alert severity="error" sx={{ borderRadius: 1 }}>
                {submitError}
              </Alert>
            </Grid>
          )}

        </Grid>
      </FormLayout>
    </LoadingError>
  );
};