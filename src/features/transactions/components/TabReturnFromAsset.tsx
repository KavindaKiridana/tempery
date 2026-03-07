import { useEffect, useState } from "react";

import {
  Button,
  CircularProgress,
  FormControl,
  Grid,
  InputLabel,
  TextField,
  MenuItem,
  Select,
} from "@mui/material";

import {
  postTransaction,
  getTransaction,
} from "../services/tracsactionService";

import type {
  AvailableAssetItem,
  PostTransaction,
  RemoveSpareParts,
} from "../types";
import { LoadingError } from "@components/layout/LoadingError";

interface ReturnFromAssetProps {
  id?: string;
  onSuccess?: () => void;
}

function TabReturnFromAsset({ id, onSuccess }: ReturnFromAssetProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [note, setNote] = useState("");
  const [removePartData, setRemovePartData] = useState<RemoveSpareParts>();
  const [selectedSparePart, setSparePart] = useState<AvailableAssetItem>();

  const loadData = async () => {
    try {
      setLoading(true);
      const sentData = {
        Type: "remove_spare_parts",
        AssetId: id!,
      };
      const comingData = await getTransaction(sentData);
      // Check if the response indicates an error
      if (comingData.Ok === false || comingData.ExceptionMessage) {
        setError(comingData.ExceptionMessage);
        return;
      }
      setRemovePartData(comingData);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (id) loadData();
  }, [id]);

  const handleSubmit = async () => {
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return null;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    // send to backend
    try {
      const formData: PostTransaction = {
        Type: "ASSET_RETURNED_FROM_ASSET_PART",
        AssetId: selectedSparePart!.AssetId!,
        Time: new Date().toISOString(),
        RelatedAssetId: id!,
        ToId: selectedSparePart!.LocationId,
        Note: note,
      };
      const output = await postTransaction(formData);
      if (typeof output === "string") {
        console.log("API Request Data:", formData);
        console.error("API Error:", output);
        setSubmitError(output);
      } else {
        window.alert("Asset returned successfully.");
        await loadData();
        onSuccess?.(); // Call the callback
      }
    } catch (error) {
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Failed to return asset. Please try again.";
      setSubmitError(errorMessage);
      return null;
    } finally {
      setIsSubmitting(false);
    }
  };

  const validateForm = (): string | null => {
    if (!selectedSparePart?.AssetId)
      return " Please select a spare part to return.";
    if (note.trim() === "") return "Please write a note";
    return null;
  };

  return (
    <LoadingError loading={loading} error={error}>
      <Grid container spacing={1}>
        <Grid size={{ xs: 12 }}>
          <h3>Return from Asset</h3>
        </Grid>
        {/* <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Assignment Date & Time"
            type="text"
            size="small"
            fullWidth
            required
            value={new Date().toISOString()}
            disabled
            InputProps={{
              readOnly: true,
            }}
          />
        </Grid> */}
        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControl fullWidth size="small" required>
            <TextField
              label="Asset"
              type="text"
              size="small"
              value={id + " : " + removePartData?.AssetName}
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
          </FormControl>
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Main Asset Location"
            type="text"
            size="small"
            fullWidth
            value={removePartData?.LocationName}
            InputProps={{
              readOnly: true,
            }}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          {removePartData?.currentSparePartsList?.length ? (
            <FormControl fullWidth size="small" required>
              <InputLabel id="spare-part-label">Select Spare Part</InputLabel>
              <Select
                labelId="spare-part-label"
                label="Select Spare Part"
                value={selectedSparePart?.AssetId || ""}
                onChange={(e) => {
                  const selectedId = e.target.value;
                  const selected = removePartData?.currentSparePartsList?.find(
                    (asset) => asset.AssetId === selectedId,
                  );
                  setSparePart(selected);
                }}
              >
                {removePartData?.currentSparePartsList?.map((asset) => (
                  <MenuItem key={asset.AssetId} value={asset.AssetId}>
                    (Id: {asset.AssetId}) - (Name: {asset.AssetName})
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : (
            <> No spare parts available.</>
          )}
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Spare Part Location"
            type="text"
            size="small"
            fullWidth
            value={selectedSparePart?.LocationName || ""}
            InputProps={{
              readOnly: true,
            }}
          />
        </Grid>
        <Grid size={{ xs: 12 }}>
          <TextField
            label="Note"
            type="text"
            size="small"
            multiline
            rows={4}
            fullWidth
            value={note}
            onChange={(e) => setNote(e.target.value)}
          />
        </Grid>
        <Grid
          size={12}
          sx={{ display: "flex", justifyContent: "center", mt: 0 }}
        >
          <Button
            variant="contained"
            size="large"
            onClick={handleSubmit}
            sx={{ minWidth: 120 }}
          >
            {isSubmitting ? (
              <CircularProgress size={24} />
            ) : (
              "Return from Asset"
            )}{" "}
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
    </LoadingError>
  );
}
export default TabReturnFromAsset;
