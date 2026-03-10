import { useEffect, useState, useCallback } from "react";

import {
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Typography,
  TextField,
  FormControlLabel,
  Checkbox,
  CircularProgress,
  Button,
} from "@mui/material";

import {
  postTransaction,
  getTransaction,
} from "../services/tracsactionService";

import type { PostTransaction } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

interface ReturnFromUserProps {
  id?: string;
  onSuccess?: () => void;
}

function TabReturnFromRepair({ id, onSuccess }: ReturnFromUserProps) {
  const [doRepairComplete, SetRepairComplete] = useState<boolean>(false);
  const [loading, setLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [note, setNote] = useState("");
  const [assetName, setAssetName] = useState("");
  const [cost, setCost] = useState<number | null>(null);
  const [isTempAssigned, setIsTempAssigned] = useState(false);
  const [supplierName, setSupplierName] = useState("");
  const [supplierId, setSupplierId] = useState<number | null>(null);

  // Memoize loadData to prevent unnecessary re-renders
  const loadData = useCallback(async () => {
    if (!id) {
      setLoadError("Asset ID is required");
      return;
    }

    try {
      setLoading(true);
      setLoadError(null); // Clear previous errors
      const sentData = { Type: "returned_from_repair", AssetId: id };
      const comingData = await getTransaction(sentData);
      // Check if the response indicates an error
      if (comingData.Ok === false || comingData.ExceptionMessage) {
        setLoadError(comingData.ExceptionMessage);
        return;
      }
      // Update all form fields with fetched data
      setAssetName(comingData.AssetName ?? "");
      setCost(comingData.Cost ?? null);
      setIsTempAssigned(comingData.IsTempAssigned ?? false);
      setSupplierName(comingData.SupplierName ?? "");
      setSupplierId(comingData.SupplierId ?? null);
      // Reset form state
      setNote("");
      SetRepairComplete(false);
      setSubmitError(null);
    } catch (err: any) {
      console.error("Error loading repair data:", err);
      setLoadError(err.message || "Failed to load repair information");
    } finally {
      setLoading(false);
    }
  }, [id]); // Only depend on id

  useEffect(() => {
    loadData();
  }, [loadData]); // Depend on the memoized function

  const handleSubmit = async () => {
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    try {
      const formData: PostTransaction = {
        Type: doRepairComplete ? "RETURNED_FROM_REPAIR" : "STILL_IN_REPAIR",
        AssetId: id!,
        Time: new Date().toISOString(),
        Note: note,
        IsTempAssigned: isTempAssigned,
        RepairCost: cost,
        FromId: doRepairComplete ? supplierId! : 0,
        ToId: doRepairComplete ? 0 : supplierId!,
      };
      console.log("Submitting form data TabReturnFromRepair:", formData);
      const output = await postTransaction(formData);
      if (typeof output === "string") {
        console.log("API Request Data:", formData);
        console.error("API Error:", output);
        setSubmitError(output);
      } else {
        // Success - show alert and reload data
        window.alert(
          doRepairComplete
            ? "Asset returned from repair successfully."
            : "Repair status updated successfully.",
        );

        // Reload the form data to show updated values
        await loadData();

        // Call the onSuccess callback if provided
        onSuccess?.();
      }
    } catch (error) {
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Failed to process repair. Please try again.";
      console.error("Submit error:", error);
      setSubmitError(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  const validateForm = (): string | null => {
    if (doRepairComplete && (cost === null || cost === 0))
      return "Please enter a valid cost.";
    if (note.trim() === "") return "Please write a note";
    return null;
  };

  return (
    <LoadingError loading={loading} error={loadError}>
      <Grid container spacing={1}>
      <Grid size={{ xs: 12 }}>
          <h3>
             {doRepairComplete ? "Return from Repair" : "Still In Repair"}
          </h3>
      </Grid>
      <Grid size={{ xs: 12, sm: 6 }}>
        <TextField
        fullWidth
                      label="Asset"
                      type="text"
                      size="small"
                      value={id + " : " + assetName}
                      color="primary"
                      focused // keeps the primary color border always visible
                      sx={{
                        "& .MuiOutlinedInput-root": {
                          backgroundColor: (theme) => `${theme.palette.primary.main}14`, // 14 = ~8% hex opacity
                        },
                      }}
                      InputProps={{
                        readOnly: true,
                        sx: { fontWeight: 600 },
                      }}
                    />
      </Grid>
      <Grid size={{ xs: 12, sm: 6 }}>
        <TextField
          label="Supplier Name"
          value={supplierName}
          disabled
          fullWidth
          size="small"
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 6 }}>
        <FormControl fullWidth size="small" required>
          <InputLabel id="does-repair-completed-label">
            Does Repair Completed
          </InputLabel>
          <Select
            labelId="does-repair-completed-label"
            label="Does Repair Completed"
            value={doRepairComplete}
            onChange={(e) => SetRepairComplete(e.target.value === "true")}
          >
            <MenuItem value="true">Yes</MenuItem>
            <MenuItem value="false">No</MenuItem>
          </Select>
        </FormControl>
      </Grid>
      <Grid size={{ xs: 12, sm: 6 }}>
        <FormControlLabel
          control={
            <Checkbox
              name="isTemporaryAssetAssigned"
              checked={isTempAssigned}
              onChange={(e) => setIsTempAssigned(e.target.checked)}
            />
          }
          label="Is Temporary Asset Assigned?"
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 6 }}>
        <TextField
          label="Cost"
          name="cost"
          type="number"
          fullWidth
          value={cost ?? ""}
          onChange={(e) =>
            setCost(e.target.value === "" ? null : Number(e.target.value))
          }
          margin="normal"
        />
      </Grid>
      <Grid size={12}>
        <TextField
          label="Note"
          value={note}
          multiline
          rows={3}
          fullWidth
          size="small"
          onChange={(e) => setNote(e.target.value)}
        />
      </Grid>
      <Grid size={12}>
        <Button
          variant="contained"
          size="large"
          onClick={handleSubmit}
          disabled={isSubmitting}
          sx={{ minWidth: 200 }}
        >
          {isSubmitting ? (
            <CircularProgress size={24} color="inherit" />
          ) : doRepairComplete ? (
            "Return from Repair"
          ) : (
            "Update Status"
          )}
        </Button>
        {submitError && (
          <Typography color="error" sx={{ textAlign: "center", mt: 2 }}>
            {submitError}
          </Typography>
        )}
      </Grid>
      </Grid>
    </LoadingError>
  );
}

export default TabReturnFromRepair;
