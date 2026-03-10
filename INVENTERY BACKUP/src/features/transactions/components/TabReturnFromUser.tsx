import { useEffect, useState } from "react";

import {
  Button,
  CircularProgress,
  FormControl,
  Grid,
  TextField,
} from "@mui/material";

import {
  postTransaction,
  getTransaction,
} from "@features/transactions/services/tracsactionService";

import type { AvailableAssetItem, PostTransaction } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

interface ReturnFromUserProps {
  id?: string;
  onSuccess?: () => void;
}

function ReturnFromUser({ id, onSuccess }: ReturnFromUserProps) {
  const [note, setNote] = useState("");
  const [assets, setAssets] = useState<AvailableAssetItem>();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const loadData = async () => {
    try {
      setLoading(true);
      const sentData = {
        Type: "return_from_user",
        AssetId: id!,
      };
      const fetchedAssets = await getTransaction(sentData);
      // Check if the response indicates an error
      if (fetchedAssets.Ok === false || fetchedAssets.ExceptionMessage) {
        setError(fetchedAssets.ExceptionMessage);
        return;
      }
      setAssets(fetchedAssets);
    } catch (err: any) {
      setError(err.message);
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
        Type: "ASSET_REMOVE_FROM_USER",
        AssetId: id!,
        Time: new Date().toISOString(),
        FromId: assets!.UserId,
        ToId: assets!.LocationId,
        Note: note,
      };
      const output = await postTransaction(formData);
      if (typeof output === "string") {
        console.log("API Request Data:", formData);
        console.error("API Error:", output);
        setSubmitError(output);
      } else {
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
    if (!assets?.UserName) return "Invalid User"; // Check if UserName exists
    if (!assets?.LocationId) return "Invalid Location";
    if (!id) return "Asset ID is missing. Please try again.";
    if (note.trim() === "") return "Please write a note";
    return null;
  };

  return (
    <LoadingError loading={loading} error={error}>
      <h3>Return from User</h3>
      <Grid container spacing={1}>
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
              value={id + " : " + assets?.AssetName}
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
            label="Location"
            type="text"
            size="small"
            fullWidth
            value={assets?.LocationName || ""}
            InputProps={{
              readOnly: true,
            }}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="User"
            type="text"
            size="small"
            fullWidth
            value={assets?.UserName || ""}
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
            {isSubmitting ? <CircularProgress size={24} /> : "Return from User"}
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
export default ReturnFromUser;
