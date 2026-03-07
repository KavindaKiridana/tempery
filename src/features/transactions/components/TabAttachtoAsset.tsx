import { useEffect, useState } from "react";

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

import {
  postTransaction,
  getTransaction,
} from "@features/transactions/services/tracsactionService";

import type { AssignToAsset, PostTransaction } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

interface ReturnFromUserProps {
  id?: string;
  onSuccess?: () => void;
}

function TabAttachtoAsset({ id, onSuccess }: ReturnFromUserProps) {
  const [sparePartId, setSparePartId] = useState<string>("");
  const [note, setNote] = useState("");
  const [Data, setData] = useState<AssignToAsset>();
  const [loading, setLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const loadData = async () => {
    try {
      setLoading(true);
      const sentData = { Type: "assign_to_asset", AssetId: id! };
      const comingData = await getTransaction(sentData);
      // Check if the response indicates an error
      if (comingData.Ok === false || comingData.ExceptionMessage) {
        setLoadError(comingData.ExceptionMessage);
        return;
      }
      setData(comingData);
    } catch (err: any) {
      setLoadError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

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
        Type: "ASSET_ASSIGNED_TO_ASSET_PART",
        AssetId: sparePartId,
        Time: new Date().toISOString(),
        FromId: Data!.LocationId,
        RelatedAssetId: Data!.AssetId,
        Note: note,
      };
      const output = await postTransaction(formData);
      if (typeof output === "string") {
        console.log("API Request Data:", formData);
        console.error("API Error:", output);
        setSubmitError(output);
      } else {
        window.alert("Asset assigned successfully!");
        loadData();
        onSuccess?.(); // Call the callback
      }
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

  const validateForm = (): string | null => {
    if (!sparePartId) return "Please select a spare part to assign.";
    if (note.trim() === "") return "Please write a note";
    return null;
  };

  return (
    <LoadingError loading={loading} error={loadError}>
      <Grid container spacing={1}>
        <Grid size={{ xs: 12 }}>
          <h3>Attach Spare Part</h3>
        </Grid>
        {/* <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Assignment Date & Time"
            type="text"
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
          <TextField
            fullWidth
            label="Asset"
            type="text"
            size="small"
            value={id + " : " + Data?.AssetName}
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
            label="Location"
            type="text"
            fullWidth
            required
            value={Data?.LocationName || ""}
            InputProps={{
              readOnly: true,
            }}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControl fullWidth size="small" required>
            <InputLabel id="spare-part-label">Select Spare Part</InputLabel>
            <Select
              labelId="spare-part-label"
              label="Select Spare Part"
              value={sparePartId}
              onChange={(e) => setSparePartId(e.target.value)}
            >
              {Data?.AvailableAssets?.map((asset) => (
                <MenuItem key={asset.AssetId} value={asset.AssetId}>
                  (Id: {asset.AssetId}) - (Name: {asset.AssetName})
                </MenuItem>
              ))}
            </Select>
          </FormControl>
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
            {isSubmitting ? <CircularProgress size={24} /> : " Attach Asset"}
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
export default TabAttachtoAsset;
