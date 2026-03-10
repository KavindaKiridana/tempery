import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Grid,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress,
  Button,
  Alert,
} from "@mui/material";
import { FormLayout } from "@components/layout/FormLayout";
import { FormTabCommon } from "@features/assets/components/FormTabCommon";
import { useMasterData } from "@features/assets/services/useMasterData";
import { getAssetInfo, updateAsset } from "../services/assetEditService";
import type { EditAssetView, CreateCommonAssetRequest } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

function EditAssetForm() {
  const navigate = useNavigate();
  const { id } = useParams();
  const { data, loading, error: masterDataError } = useMasterData();

  // ─── Form State ────────────────────────────────────────────────────────────
  const [formData, setFormData] = useState<EditAssetView | null>(null);
  const [purchaseDate, setPurchaseDate] = useState("");
  const [financeAssetCode, setFinanceAssetCode] = useState("");
  const [warranty, setWarranty] = useState("");
  const [type, setType] = useState("SparePart");
  const [companyId, setCompanyId] = useState(0);
  const [locationId, setLocationId] = useState(0);
  const [manufacturerSN, setManufacturerSN] = useState("");
  const [brandnew, setBrandNew] = useState(false);
  const [cost, setCost] = useState("");
  const [name, setName] = useState("");
  const [supplierId, setSupplierId] = useState(0);
  const [ipAddress, setIpaddress] = useState("");
  const [note, setNote] = useState("");

  // ─── Submission State ──────────────────────────────────────────────────────
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  // ─── Load existing asset data ──────────────────────────────────────────────
  useEffect(() => {
    const getPastData = async () => {
      const data = await getAssetInfo(id!);
      setFormData(data);
    };
    getPastData();
  }, [id]);

  // Populate form fields once asset data is loaded
  useEffect(() => {
    if (formData) {
      setPurchaseDate(formData.DoP || "");
      setFinanceAssetCode(formData.FinanceAssetCode || "");
      setWarranty(formData.Warranty?.toString() || "");
      setType(formData.Type || "SparePart");
      setCompanyId(formData.CompanyId || 0);
      setLocationId(formData.LocationId || 0);
      setBrandNew(formData.Brandnew || false);
      setCost(formData.Cost?.toString() || "");
      setName(formData.Name || "");
      setSupplierId(formData.SupplierId || 0);
      setIpaddress(formData.IPAddress || "");
      setNote(formData.Note || "");
      setManufacturerSN(formData.ManufactureSN || "");
    }
  }, [formData]);

  // ─── Validation ────────────────────────────────────────────────────────────
  const validateForm = (): string | null => {
    if (manufacturerSN == null || manufacturerSN === "") setWarranty("0");
    if (!name) return "Name is required";
    if (warranty == null || warranty === "") setWarranty("0");
    if (warranty !== null && warranty !== "" && isNaN(parseFloat(warranty)))
      return "Please enter a valid warranty period (months)";
    if (cost == null || cost === "") setCost("0");
    if (cost !== null && cost !== "" && isNaN(parseFloat(cost)))
      return "Please enter a valid cost";
    if (companyId === 0) return "Please select a company";
    if (locationId === 0) return "Please select a location";
    if (supplierId === 0) return "Please select a supplier";
    return null;
  };

  // ─── Save ──────────────────────────────────────────────────────────────────
  const saveCommonData = async (): Promise<boolean> => {
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return false;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    try {
      const updateData: CreateCommonAssetRequest = {
        patchRequestType: "common",
        AssetId: id,
        Type: type,
        Quantity: 1,
        DoP: purchaseDate,
        FinanceAssetCode: financeAssetCode,
        Warranty: warranty ? parseFloat(warranty) : 0,
        CompanyId: companyId,
        LocationId: locationId,
        ManufactureSN: manufacturerSN,
        Brandnew: brandnew,
        Cost: cost ? parseFloat(cost) : 0,
        Name: name,
        SupplierId: supplierId,
        DepartmentId: 0,
        IPAddress: ipAddress,
        Note: note,
      };
      const error = await updateAsset(updateData);
      if (error) {
        setSubmitError(error);
        return false;
      }
      return true;
    } catch (error) {
      console.error("Error during saveCommonData:", error);
      setSubmitError("An unexpected error occurred. Please try again.");
      return false;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleAddAsset = async () => {
    const result = await saveCommonData();
    if (result) navigate("/viewAsset");
    else console.error("Error saving asset, can't proceed to next step");
  };

  const handleNext = async () => {
    const result = await saveCommonData();
    if (result) navigate(`/EditComputerForm/${id}`);
    else console.error("Failed to save asset. Cannot proceed to next step.");
  };

  // ─── Render ────────────────────────────────────────────────────────────────
  return (
    <LoadingError loading={loading} error={masterDataError}>
      <FormLayout title="Edit Asset Info">
        <Grid container spacing={2}>

          {/* Shared common fields */}
          <FormTabCommon
            purchaseDate={purchaseDate}
            onPurchaseDateChange={setPurchaseDate}
            financeAssetCode={financeAssetCode}
            onFinanceAssetCodeChange={setFinanceAssetCode}
            warranty={warranty}
            onWarrantyChange={setWarranty}
            companies={data.companies}
            companyId={companyId}
            onCompanyChange={setCompanyId}
            locations={data.locations}
            locationId={locationId}
            onLocationChange={setLocationId}
            brandnew={brandnew}
            onbrandnewChange={setBrandNew}
            cost={cost}
            onCostChange={setCost}
            name={name}
            onNameChange={setName}
            suppliers={data.suppliers}
            supplierId={supplierId}
            onSupplierChange={setSupplierId}
            ipAddress={ipAddress}
            onipAdressChange={setIpaddress}
          />

          {/* Asset Type selector */}
          <Grid size={{ xs: 12, sm: 6 }}>
            <FormControl fullWidth size="small" required>
              <InputLabel id="type-label">Type</InputLabel>
              <Select
                labelId="type-label"
                label="Type"
                value={type}
                onChange={(e) => setType(e.target.value)}
              >
                <MenuItem value="Laptop">Laptop</MenuItem>
                <MenuItem value="Desktop">Desktop</MenuItem>
                <MenuItem value="Server">Server</MenuItem>
                <MenuItem value="SparePart">Spare Part</MenuItem>
              </Select>
            </FormControl>
          </Grid>

          {/* Manufacturer Serial Number */}
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Manufacturer Serial Number"
              type="text"
              size="small"
              fullWidth
              required
              value={manufacturerSN}
              onChange={(e) => setManufacturerSN(e.target.value)}
            />
          </Grid>

          {/* Note */}
          <Grid size={{ xs: 12 }}>
            <TextField
              label="Note"
              type="text"
              size="small"
              multiline
              rows={3}
              fullWidth
              value={note}
              onChange={(e) => setNote(e.target.value)}
            />
          </Grid>

          {/* Submit button — "Save Asset" for SparePart, "Next" for others */}
          <Grid size={12}>
            {type === "SparePart" ? (
              <Button
                variant="contained"
                size="large"
                sx={{ minWidth: 160 }}
                onClick={handleAddAsset}
                disabled={isSubmitting}
              >
                {isSubmitting ? <CircularProgress size={22} /> : "Save Asset"}
              </Button>
            ) : (
              <Button
                variant="contained"
                size="large"
                sx={{ minWidth: 160 }}
                onClick={handleNext}
                disabled={isSubmitting}
              >
                {isSubmitting ? <CircularProgress size={22} /> : "Next"}
              </Button>
            )}
          </Grid>

          {/* Validation / submission error */}
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
}

export default EditAssetForm;