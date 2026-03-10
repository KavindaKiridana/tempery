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
  Box,
} from "@mui/material";

import {
  postTransaction,
  getTransaction,
} from "@features/transactions/services/tracsactionService";

import type {
  MoveAssetToLocationItem,
  NextLocationsItem,
  TransactionPageData,
} from "../types";
import { LoadingError } from "@components/layout/LoadingError";

interface ReturnFromUserProps {
  id?: string;
  onSuccess?: () => void;
}

function MoveLocation({ id, onSuccess }: ReturnFromUserProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [companyName, setCompanyName] = useState("");
  const [locationName, setLocationName] = useState("");
  const [currentLocationId, setCurrentLocationId] = useState<number | null>(
    null,
  );

  const [nextLocations, setNextLocations] = useState<NextLocationsItem[]>([]);
  const [selectedNextLocation, setSelectedNextLocation] = useState<number>(0);

  const [note, setNote] = useState("");

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [comingData, setComingData] = useState<MoveAssetToLocationItem>();
  const [isAssetMovable, setIsAssetMovable] = useState<boolean>(true);
  const [transactionPageData, setTransactionPageData] =
    useState<TransactionPageData>();

  const load = async () => {
    try {
      setLoading(true);
      const payload2 = {
        Type: "is_eligible_to_location_transfer",
        AssetId: id,
      };
      const res1 = await getTransaction(payload2);
      // Check if the response indicates an error
      if (res1.Ok === false || res1.ExceptionMessage) {
        setError(res1.ExceptionMessage);
        console.warn("Eligility to transfer", res1);
        return;
      }
      setTransactionPageData(res1);
      setIsAssetMovable(!res1.HasExistingUser && !res1.IsActiveSparePart); // Update movability status
      if (isAssetMovable) {
        const payload1 = {
          Type: "move_asset_to_location",
          AssetId: id,
        };
        const res2 = await getTransaction(payload1);
        // Check if the response indicates an error
        if (res2.Ok === false || res2.ExceptionMessage) {
          setError(res2.ExceptionMessage);
          console.warn("fetchMoveLocationItem", res2);
          return;
        }
        setComingData(res2);
        setCompanyName(res2.ExistingCompanyName);
        setLocationName(res2.ExistingLocationName);
        setCurrentLocationId(res2.ExistingLocationId);
        setNextLocations(res2.NextLocations);
      }
    } catch (e: any) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, [id]);

  const handleSubmit = async () => {
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return null;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const postData = {
      Type: "ASSET_LOCATION_CHANGED",
      AssetId: id!,
      Time: new Date().toISOString(),
      FromId: currentLocationId!, // current location
      ToId: selectedNextLocation, // next location
      Note: note,
    };
    const result = await postTransaction(postData);
    if (typeof result === "string") {
      console.log("Transaction submission info:", postData);
      console.error("Transaction submission error:", result);
      setSubmitError(result);
    } else {
      alert("Asset moved successfully!");
      load();
      onSuccess?.(); // Call the callback
    }
    setIsSubmitting(false);
  };

  const validateForm = (): string | null => {
    if (!selectedNextLocation) return "Please select the next location";
    if (note.trim() === "") return "Please write a note";
    return null; // No errors
  };

  return (
    <LoadingError loading={loading} error={error}>
      {!isAssetMovable ? (
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          style={{ minHeight: "50vh" }}
        >
          <Box textAlign="center">
            <h3>
              {transactionPageData?.HasExistingUser
                ? "Cannot move asset while it is assigned to a user."
                : "Cannot move asset while it is an active spare part."}
            </h3>
          </Box>
        </Grid>
      ) : (
        <Grid container spacing={1}>
          <Grid size={{ xs: 12 }}>
            <h3>Move Asset to Another Location</h3>
          </Grid>
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
                    backgroundColor: (theme) =>
                      `${theme.palette.primary.main}14`, // 14 = ~8% hex opacity
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
              label="Current Company"
              value={companyName}
              disabled
              fullWidth
              size="small"
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Current Location"
              value={locationName}
              disabled
              fullWidth
              size="small"
            />
          </Grid>

          {/* Select next location */}
          <Grid size={{ xs: 12, sm: 6 }}>
            <FormControl fullWidth size="small" required>
              <InputLabel>Select Next Location</InputLabel>
              <Select
                value={selectedNextLocation}
                label="Select Next Location"
                onChange={(e) =>
                  setSelectedNextLocation(e.target.value as number)
                }
              >
                {nextLocations.map((loc) => (
                  <MenuItem key={loc.LocationId} value={loc.LocationId}>
                    {loc.LocationName}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          {/* Note */}
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

          {/* Submit */}
          <Grid
            size={12}
            sx={{ mt: 2, display: "flex", justifyContent: "center" }}
          >
            <Button variant="contained" size="large" onClick={handleSubmit}>
              {isSubmitting ? <CircularProgress size={24} /> : "Move Asset"}
            </Button>
          </Grid>

          {submitError && (
            <div style={{ color: "red", marginTop: 16 }}>{submitError}</div>
          )}
        </Grid>
      )}
    </LoadingError>
  );
}

export default MoveLocation;
