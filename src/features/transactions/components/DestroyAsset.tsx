import React, { useState } from "react";

import { useNavigate } from "react-router-dom";

import {
  Button,
  CircularProgress,
  Grid,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from "@mui/material";

import {
  postTransaction,
  getTransaction,
} from "@features/transactions/services/tracsactionService";

import type { PostTransaction, TransactionPageData } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

interface ReturnFromUserProps {
  id?: string;
}

function DestroyAsset({ id }: ReturnFromUserProps) {
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [comingData, setComingData] = useState<TransactionPageData>();
  const [note, setNote] = useState("");
  const [isDestroy, setIsDestroy] = useState(true);

  React.useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        const sentData = {
          Type: "asset_destroyed_lost_stolen",
          AssetId: id!,
        };
        const result = await getTransaction(sentData);
        // Check if the response indicates an error
        if (result.Ok === false || result.ExceptionMessage) {
          setError(result.ExceptionMessage);
          console.warn("Distroy Asset PageData", comingData);
          return;
        }
        setComingData(result);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, [id]);

  const handleSubmit = async (id: string) => {
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return null;
    }
    const isConfirmed = window.confirm(
      "Are you sure you? This action cannot be undone.",
    );
    setIsSubmitting(true);
    setSubmitError(null);
    if (isConfirmed) {
      let transactionType: string;
      if (isDestroy) {
        if (comingData?.HasExistingUser) {
          transactionType = "ASSET_DESTROYED_FROM_USER";
        } else {
          if (comingData?.IsActiveSparePart) {
            transactionType = "SPAREPART_DESTROYED";
          } else {
            transactionType = "ASSET_DESTROYED_FROM_STOCK";
          }
        }
      } else {
        if (comingData?.HasExistingUser) {
          transactionType = "ASSET_LOST_STOLEN_FROM_USER";
        } else {
          if (comingData?.IsActiveSparePart) {
            transactionType = "SPAREPART_LOST_STOLEN";
          } else {
            transactionType = "ASSET_LOST_STOLEN_FROM_STOCK";
          }
        }
      }
      const formData: PostTransaction = {
        Type: transactionType,
        AssetId: id,
        Time: new Date().toISOString(),
        FromId: comingData?.FromId,
        Note: note,
        RelatedAssetId: comingData?.AssociateAssetId,
      };
      const result = await postTransaction(formData);
      if (typeof result === "string") {
        console.log("Transaction submission info:", formData);
        console.error("Transaction submission error:", result);
        setSubmitError(result);
      } else {
        alert("Submitted successfully!");
        navigate("/viewAsset");
      }
      setIsSubmitting(false);
    }
  };

  const validateForm = (): string | null => {
    if (note.trim() === "") return "Please write a note";
    return null; // No errors
  };

  return (
    <LoadingError loading={loading} error={error}>
      <Grid container spacing={1}>
        {isDestroy ? (
          <Grid size={{ xs: 12 }}>
            <h3>Destroy Asset</h3>
          </Grid>
        ) : (
          <Grid size={{ xs: 12 }}>
            <h3>Lost or Stolen</h3>
          </Grid>
        )}
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
              value={id + " : " + comingData?.AssetName}
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
          {comingData?.HasExistingUser ? (
            <TextField
              label="Current User"
              value={comingData?.FromName}
              disabled
              fullWidth
              size="small"
            />
          ) : comingData?.IsActiveSparePart ? (
            <TextField
              label="Part Of Asset"
              value={comingData?.AssociateAssetId ?? ""}
              disabled
              fullWidth
              size="small"
            />
          ) : (
            <TextField
              label="Current Location"
              value={comingData?.FromName ?? ""}
              disabled
              fullWidth
              size="small"
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControl fullWidth size="small" required>
            <InputLabel id="Type-label">Type</InputLabel>
            <Select
              labelId="Type-label"
              label="Type"
              value={isDestroy}
              onChange={(e) => setIsDestroy(e.target.value === "true")}
            >
              <MenuItem value="true">Destroy Asset</MenuItem>
              <MenuItem value="false">Asset Lost or Stolen</MenuItem>
            </Select>
          </FormControl>
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
        <Grid
          size={12}
          sx={{ mt: 2, display: "flex", justifyContent: "center" }}
        >
          <Button
            variant="contained"
            color="primary"
            size="large"
            sx={{ minWidth: 120 }}
            onClick={() => handleSubmit(id!)}
          >
            {isSubmitting ? <CircularProgress size={24} /> : "Submit"}
          </Button>
        </Grid>
        {submitError && (
          <div style={{ color: "red", marginTop: 16 }}>{submitError}</div>
        )}
      </Grid>
    </LoadingError>
  );
}
export default DestroyAsset;
