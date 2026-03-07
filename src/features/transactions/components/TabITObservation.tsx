import { useEffect, useState } from "react";

import {
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress,
  TextField,
  Button,
  Box,
} from "@mui/material";

import {
  postTransaction,
  getTransaction,
} from "../services/tracsactionService";

import type { AvailableAssetItem, Complains, PostTransaction } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

interface ReturnFromUserProps {
  id?: string;
  onSuccess?: () => void;
}

function ITObservation({ id, onSuccess }: ReturnFromUserProps) {
  const [note, setNote] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [hasActiveComplaints, setHasActiveComplaints] =
    useState<boolean>(false);
  const [complains, setComplains] = useState<Complains[]>([]);
  const [complainId, setComplainId] = useState<number>(0);
  const [isAssetsListVisible, setIsAssetsListVisible] = useState(false);
  const [assetsUnderComplain, setAssetsUnderComplain] = useState<
    AvailableAssetItem[]
  >([]);
  const [selectedAssetId, setSelectedAssetId] = useState<string>(id!);
  const[assetName, setAssetName] = useState<string| null>(null);

  const loadData = async () => {
    try {
      setLoading(true);
      const hasActiveComplaints = await getTransaction({
        Type: "are_their_any_active_complaints",
        AssetId: id,
      });
      if (
        hasActiveComplaints.Ok === false ||
        hasActiveComplaints.ExceptionMessage
      ) {
        console.warn("hasActiveComplaints", hasActiveComplaints);
        setError(hasActiveComplaints.ExceptionMessage);
        return;
      }
      setHasActiveComplaints(hasActiveComplaints);
      if (hasActiveComplaints) {
        const fetcheListOfComplains = await getTransaction({
          Type: "get_list_of_active_complaints",
          AssetId: id,
        });
        if (
          fetcheListOfComplains.Ok === false ||
          fetcheListOfComplains.ExceptionMessage
        ) {
          console.warn("fetcheListOfComplains", fetcheListOfComplains);
          setError(fetcheListOfComplains.ExceptionMessage);
          return;
        }
        setComplains(fetcheListOfComplains);
        const getAssetName = await getTransaction({
          Type: "get_asset_name_by_id",
          AssetId: id,
        });
        if (getAssetName.Ok === false || getAssetName.ExceptionMessage) {
          console.warn("getAssetName", getAssetName);
          console.error(getAssetName.ExceptionMessage);
        } else {
          setAssetName(getAssetName.AssetName);  
        } 
        console.log(getAssetName);
        if (fetcheListOfComplains.length === 1) {
          setComplainId(fetcheListOfComplains[0].ComplainId);
          getAssetsList(fetcheListOfComplains[0].ComplainId);
          setIsAssetsListVisible(true);
        }
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const getAssetsList = async (complainId: number) => {
    const fetcheListOfAssets = await getTransaction({
      Type: "get_list_of_assets_under_complain",
      ComplainId: complainId,
    });
    if (
      fetcheListOfAssets.Ok === false ||
      fetcheListOfAssets.ExceptionMessage
    ) {
      console.warn("fetcheListOfAssets", fetcheListOfAssets);
      setError(fetcheListOfAssets.ExceptionMessage);
      return;
    }
    setAssetsUnderComplain(fetcheListOfAssets);
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
    try {
      const formData: PostTransaction = {
        Type: "ITOBSERVATION",
        AssetId: selectedAssetId,
        Time: new Date().toISOString(),
        ToId: 0,
        Note: note,
        ComplainId: complainId,
      };
      const output = await postTransaction(formData);
      if (typeof output === "string") {
        console.log("API Request Data:", formData);
        console.error("API Error:", output);
        setSubmitError(output);
      } else {
        // Handle successful response
        window.alert("ITObservation submitted successfully!");
        loadData();
        onSuccess?.();
      }
    } catch (error) {
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Failed to submit ITObservation. Please try again.";
      setSubmitError(errorMessage);
      return null;
    } finally {
      setIsSubmitting(false);
    }
  };

  const validateForm = (): string | null => {
    if (note.trim() === "") return "Please write a note";
    if (complainId === 0) return "Please select a complain";
    return null;
  };

  return (
    <LoadingError loading={loading} error={error}>
      <Grid container spacing={1}>
        {!hasActiveComplaints ? (
          <Grid
            container
            justifyContent="center"
            alignItems="center"
            style={{ minHeight: "50vh" }}
          >
            <Box textAlign="center">
              <h3>
                This asset has no open complains. If there are no open
                complains, the user is unable to submit ITObservation records.
              </h3>
            </Box>
          </Grid>
        ) : (
          <>
            <Grid size={{ xs: 12 }}>
              <h3>IT Observation</h3>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <FormControl fullWidth size="small" required>
                <TextField
                  label="Asset"
                  type="text"
                  size="small"
                 value={assetName ? `${id} : ${assetName}` : id ?? ""}
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
            {complains.length === 1 ? (
              <>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <FormControl fullWidth size="small" required>
                    <TextField
                      label="Complained by"
                      type="text"
                      size="small"
                      value={
                        complains[0].UserName +
                        " - " +
                        new Date(complains[0].CreatedAt).toLocaleString()
                      }
                      InputProps={{
                        readOnly: true,
                      }}
                    />
                  </FormControl>
                </Grid>
              </>
            ) : (
              <>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <FormControl fullWidth size="small" required>
                    <InputLabel>Select Complain</InputLabel>
                    <Select
                      value={complainId}
                      label="Select Complain"
                      onChange={(e) => {
                        const selectedComplainId = Number(e.target.value);
                        setComplainId(selectedComplainId);
                        getAssetsList(selectedComplainId); // Call getAssetsList with the selected complainId
                        setIsAssetsListVisible(true); // make the asset list dropdown visible when a complain is selected
                      }}
                    >
                      {complains.map((x) => (
                        <MenuItem key={x.ComplainId} value={x.ComplainId}>
                          {x.Note.length > 20
                            ? x.Note.substring(0, 20) + "..."
                            : x.Note}
                          {" - " +
                            x.UserName +
                            " - " +
                            new Date(x.CreatedAt).toLocaleString()}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </Grid>
              </>
            )}
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Complain"
                type="text"
                size="small"
                multiline
                rows={4}
                fullWidth
                value={
                  complains.find((c) => c.ComplainId === complainId)?.Note || ""
                }
              />
            </Grid>
            {isAssetsListVisible ? (
              <Grid size={{ xs: 12, sm: 6 }}>
                <FormControl fullWidth size="small" required>
                  <InputLabel>Select Asset</InputLabel>
                  <Select
                    label="Select Asset"
                    value={selectedAssetId}
                    onChange={(e) => {
                      setSelectedAssetId(e.target.value as string);
                    }}
                  >
                    {/* Default option: Main Asset */}
                    <MenuItem value={id}>{id} (Current asset)</MenuItem>
                    {/* List of assets under the selected complain, excluding the main asset */}
                    {assetsUnderComplain
                      .filter((asset) => asset.AssetId !== id)
                      .map((asset) => (
                        <MenuItem key={asset.AssetId} value={asset.AssetId}>
                          (Id: {asset.AssetId}) - (Name: {asset.AssetName})
                        </MenuItem>
                      ))}
                  </Select>
                </FormControl>
              </Grid>
            ) : null}
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
                {isSubmitting ? <CircularProgress size={24} /> : "Assign Asset"}
              </Button>
              {submitError && (
                <div
                  style={{
                    color: "red",
                    textAlign: "center",
                    marginTop: "16px",
                  }}
                >
                  {submitError}
                </div>
              )}
            </Grid>
          </>
        )}
      </Grid>
    </LoadingError>
  );
}
export default ITObservation;
