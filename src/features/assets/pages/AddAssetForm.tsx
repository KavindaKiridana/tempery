import { useState } from "react";

import { useNavigate } from "react-router-dom";

import {
  Button,
  CircularProgress,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from "@mui/material";

import { FormLayout } from "@components/layout/FormLayout";
import { FormTabCommon } from "@features/assets/components/FormTabCommon";
import { useMasterData } from "@features/assets/services/useMasterData";

import { createCommonAsset } from "../services/assetCrudService";

import type { CreateCommonAssetRequest } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

export const AddAssetForm = () => {
  const { data, loading, error } = useMasterData();
  const navigate = useNavigate();

  // State for common form fields
  const [purchaseDate, setPurchaseDate] = useState("");
  const [financeAssetCode, setFinanceAssetCode] = useState("");
  const [warranty, setWarranty] = useState<string>("");
  const [type, setType] = useState("SparePart");
  const [companyId, setCompanyId] = useState<number>(0);
  const [locationId, setLocationId] = useState<number>(0);
  const [manufacturerSns, setManufacturerSns] = useState<string[]>([""]);
  const [brandnew, setBrandNew] = useState<boolean>(false);
  const [cost, setCost] = useState<string>("");
  const [name, setName] = useState("");
  const [supplierId, setSupplierId] = useState<number>(0);
  const [departmentId] = useState<number>(3); //initialize with IT Department
  const [ipAddress, setIpaddress] = useState("");
  const [qty, setQty] = useState<number>(1);
  const [note, setNote] = useState("");

  const onTypeChange = (value: string) => {
    setType(value);
  };

  const OnQtyChange = (value: number) => {
    setQty(value);
    // Reset or initialize manufacturerSns array based on new qty
    setManufacturerSns(Array(value).fill(""));
  };

  const OnManufactureSNChange = (index: number, value: string) => {
    const newSns = [...manufacturerSns];
    newSns[index] = value;
    setManufacturerSns(newSns);
  };
  const OnNoteChange = (value: string) => {
    setNote(value);
  };

  // State for submission status
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const validateForm = (): string | null => {
    if (qty <= 0) {
      return "Quantity must be at least 1";
    } // Check if qty is a float (not an integer)
    if (!Number.isInteger(qty)) {
      return "Quantity must be a whole number";
    }
    // Validate each Manufacturer Serial Number
    for (let i = 0; i < manufacturerSns.length; i++) {
      if (!manufacturerSns[i] || manufacturerSns[i].trim() === "") {
        return `Manufacturer Serial Number ${i + 1} is required`;
      }
    }

    if (!name) return "Name is required";

    // Validate warranty
    if (warranty == null || warranty === "") {
      setWarranty("0");
    }
    if (warranty !== null && warranty !== "" && isNaN(parseFloat(warranty))) {
      return "Please enter a valid warranty period (months)";
    }

    // To ensure that the error only appears when cost is a non-numeric string
    // (and not when it's null or empty)
    if (cost == null || cost === "") {
      setCost("0");
    }
    if (cost !== null && cost !== "" && isNaN(parseFloat(cost))) {
      return "Please enter a valid cost";
    }

    if (parseInt(warranty, 10) < 0) return "Warranty must be a positive number";
    // Validate IDs
    if (companyId === 0) return "Please select a company";
    if (locationId === 0) return "Please select a location";
    if (supplierId === 0) return "Please select a supplier";
    if (departmentId === 0) return "Please select a department";
    return null; // No errors
  };

  // Function to save common section data to DB
  const saveCommonData = async (): Promise<string[] | null> => {
    // Validate before submitting
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return null;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    try {
      const formData: CreateCommonAssetRequest = {
        Quantity: qty,
        DoP: purchaseDate,
        FinanceAssetCode: financeAssetCode,
        Warranty: parseInt(warranty, 10), //  Convert here
        Type: type,
        CompanyId: companyId,
        LocationId: locationId,
        //sent the list of ManufacturerSN as comma separated array string
        ManufactureSN: manufacturerSns
          .map((sn) => sn.trim())
          .filter((sn) => sn !== "")
          .join(", "),
        Brandnew: brandnew,
        Cost: parseFloat(cost), //  Convert here
        Name: name,
        SupplierId: supplierId,
        DepartmentId: departmentId,
        IPAddress: ipAddress,
        Note: note,
      };
      const assetIds = await createCommonAsset(formData);
      // Store the first assetId for navigation (or all of them if needed)
      if (assetIds && assetIds.length > 0) {
        sessionStorage.setItem("currentAssetId", assetIds[0]);
      }

      return assetIds; // Return the array
    } catch (error) {
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Failed to save asset. Please try again.";

      setSubmitError(errorMessage);
      return null;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleAddAsset = async () => {
    const assetId = await saveCommonData();
    if (assetId) {
      // Successfully saved spare part
      navigate("/viewAsset");
    } else {
      console.error("AssetId was missing");
    }
  };

  // Function to handle Next button click
  const handleNext = async () => {
    const result = await saveCommonData();
    // The API returns an object with AssetIds and Count, not just a string
    if (result) {
      // Assuming the first AssetId is the one you want to use
      const assetId = result;
      switch (type) {
        case "Laptop":
          navigate(`/AddLaptop/${assetId}`);
          break;
        case "Desktop":
          navigate(`/AddDesktop/${assetId}`);
          break;
        case "Server":
          navigate(`/AddServer/${assetId}`);
          break;
        default:
          break;
      }
    } else {
      console.error("Failed to save asset. Cannot proceed to next step.");
    }
  };

  return (
    <LoadingError loading={loading} error={error}>
      <FormLayout title="Asset Info">
        <Grid container spacing={3}>
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
          <Grid size={{ xs: 12, sm: 6 }}>
            <FormControl fullWidth size="small" required>
              <InputLabel id="type-label">Type</InputLabel>
              <Select
                labelId="type-label"
                label="Type"
                value={type}
                onChange={(e) => onTypeChange(e.target.value)}
              >
                <MenuItem value="Laptop">Laptop</MenuItem>
                <MenuItem value="Desktop">Desktop</MenuItem>
                <MenuItem value="Server">Server</MenuItem>
                <MenuItem value="SparePart">Spare Part</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          {/* Conditionally render the Quantity field */}
          {type === "SparePart" && (
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Quantity"
                type="number"
                size="small"
                fullWidth
                required
                value={qty}
                onChange={(e) => OnQtyChange(Number(e.target.value))}
              />
            </Grid>
          )}
          {Array.from({ length: qty }).map((_, index) => (
            <Grid key={index} size={{ xs: 12, sm: 6 }}>
              <TextField
                label={`Manufacturer Serial Number ${index + 1}`}
                type="text"
                size="small"
                fullWidth
                required
                value={manufacturerSns[index] || ""}
                onChange={(e) => OnManufactureSNChange(index, e.target.value)}
              />
            </Grid>
          ))}
          <Grid size={{ xs: 12 }}>
            <TextField
              label="Note"
              type="text"
              size="small"
              multiline
              rows={4}
              fullWidth
              value={note}
              onChange={(e) => OnNoteChange(e.target.value)}
            />
          </Grid>
          <Grid size={12}>
            {type === "SparePart" ? (
              <Button
                variant="contained"
                size="large"
                sx={{ minWidth: 200 }}
                onClick={handleAddAsset}
                disabled={isSubmitting}
              >
                {isSubmitting ? <CircularProgress size={24} /> : "Add Asset"}
              </Button>
            ) : (
              <Button
                variant="contained"
                size="large"
                sx={{ minWidth: 200 }}
                onClick={handleNext}
                disabled={isSubmitting}
              >
                {isSubmitting ? <CircularProgress size={24} /> : "Next"}
              </Button>
            )}
          </Grid>
          {/* Error Message Grid Item */}
          {submitError && (
            <Grid size={12}>
              <div
                style={{
                  color: "red",
                  textAlign: "center",
                  padding: "16px",
                  backgroundColor: "#ffebee",
                  borderRadius: "4px",
                }}
              >
                {submitError}
              </div>
            </Grid>
          )}
        </Grid>
      </FormLayout>
    </LoadingError>
  );
};
