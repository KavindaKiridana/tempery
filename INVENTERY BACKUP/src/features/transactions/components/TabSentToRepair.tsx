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
  fetchSuppliers,
  getTransaction,
} from "../services/tracsactionService";

import type { ItObservationTracker, PostTransaction } from "../types";
import type { SupplierItem } from "@features/masterdata/types";
import { LoadingError } from "@components/layout/LoadingError";

interface ReturnFromUserProps {
  id?: string;
  onSuccess?: () => void;
}

function TabSentToRepair({ id, onSuccess }: ReturnFromUserProps) {
  const [supplierId, setSupplierId] = useState<number>(0);
  const [suppliers, setSuppliers] = useState<SupplierItem[]>([]);
  const [note, setNote] = useState("");
  const [loading, setLoading] = useState(false);
  const [IsTempAssigned, SetTempAssigned] = useState<boolean>(false);
  const [error, setError] = useState("");
  const [cost, setCost] = useState<string>("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [itObservationData, setItObservationData] =
    useState<ItObservationTracker>();
  const [observationId, setObservationId] = useState<number>(0);
  const[assetName, setAssetName] = useState<string| null>(null);

  const loadData = async () => {
    try {
      setLoading(true);
      const output = await getTransaction({
        Type: "are_their_any_active_itobservations",
        AssetId: id,
      });
      if (output.Ok === false || output.ExceptionMessage) {
        console.warn("ItObservationTracker", output);
        setError(output.ExceptionMessage);
        return;
      }
      setItObservationData(output);
      if (output.ActiveItObservations.length === 1) {
        setObservationId(output.ActiveItObservations[0].ObservationId);
      }
      if (output.HasActiveItObservations) {
        const fetchedSuppliers = await fetchSuppliers();
        if (
          fetchedSuppliers.Ok === false ||
          fetchedSuppliers.ExceptionMessage
        ) {
          console.warn("fetchedSuppliers", fetchedSuppliers);
          setError(fetchedSuppliers.ExceptionMessage);
          return;
        }
        setSuppliers(fetchedSuppliers);
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
      }
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
    try {
      const formData: PostTransaction = {
        Type: "GIVEN_TO_REAPAIR",
        AssetId: id!,
        Time: new Date().toISOString(),
        ToId: supplierId,
        Note: note,
        RepairCost: parseFloat(cost) || 0,
        IsTempAssigned: IsTempAssigned,
        ObservationId: observationId,
      };
      const output = await postTransaction(formData);
      if (typeof output === "string") {
        console.error("Form Data: ", formData);
        console.error("API Error:", output);
        setSubmitError(output);
      } else {
        // Handle successful response
        window.alert("Asset sent to repair successfully!");
        loadData();
        onSuccess?.();
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
    if (supplierId === 0) return "Please select a supplier";
    if (note.trim() === "") return "Please write a note";
    return null;
  };

  return (
    <LoadingError loading={loading} error={error}>
      <Grid container spacing={1}>
        {!itObservationData?.HasActiveItObservations ? (
          <Grid
            container
            justifyContent="center"
            alignItems="center"
            style={{ minHeight: "50vh" }}
          >
            <Box textAlign="center">
              <h3>
                This asset has no open ITObservation. If there are no open
                ITObservation, the user is unable to submit an asset for repair.
              </h3>
            </Box>
          </Grid>
        ) : (
          <>
            <Grid size={{ xs: 12 }}>
              <h3>Sent To Repair</h3>
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
            {itObservationData.ActiveItObservations.length === 1 ? (
              <>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <FormControl fullWidth size="small" required>
                    <TextField
                      label="Main Asset"
                      type="text"
                      size="small"
                      value={
                        itObservationData.ActiveItObservations[0]
                          .ObservedByName +
                        " - " +
                        new Date(
                          itObservationData.ActiveItObservations[0]
                            .ObservationTime,
                        ).toLocaleString()
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
                    <InputLabel>Select Observation</InputLabel>
                    <Select
                      value={observationId}
                      label="Select Observation"
                      onChange={(e) => setObservationId(Number(e.target.value))}
                    >
                      {itObservationData.ActiveItObservations.map((x) => (
                        <MenuItem key={x.ObservationId} value={x.ObservationId}>
                          {x.ObservationNote.length > 20
                            ? x.ObservationNote.substring(0, 20) + "..."
                            : x.ObservationNote}
                          {" - " +
                            x.ObservedByName +
                            " - " +
                            new Date(x.ObservationTime).toLocaleString()}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </Grid>
              </>
            )}
            <Grid size={{ xs: 12 }}>
              <TextField
                label="ITObservation"
                type="text"
                size="small"
                multiline
                rows={4}
                fullWidth
                value={
                  itObservationData.ActiveItObservations.find(
                    (c) => c.ObservationId === observationId,
                  )?.ObservationNote || ""
                }
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <FormControl fullWidth size="small" required>
                <InputLabel id="supplier-label">Supplier</InputLabel>
                <Select
                  labelId="supplier-label"
                  label="Supplier"
                  value={supplierId}
                  onChange={(e) => setSupplierId(Number(e.target.value))}
                >
                  {suppliers
                    .filter((supplier) => supplier.IsActive === true)
                    .map((supplier) => (
                      <MenuItem key={supplier.Id} value={supplier.Id}>
                        {supplier.Name}
                      </MenuItem>
                    ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Cost"
                type="number"
                size="small"
                fullWidth
                required
                value={cost}
                onChange={(e) => setCost(e.target.value)}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <FormControl fullWidth size="small" required>
                <InputLabel id="does-temp-assigned-label">
                  Does Tempery Asset Assigned to User
                </InputLabel>
                <Select
                  labelId="does-temp-assigned-label"
                  label="Does Tempery Asset Assigned to User"
                  value={IsTempAssigned}
                  onChange={(e) => SetTempAssigned(e.target.value === "true")}
                >
                  <MenuItem value="true">Yes</MenuItem>
                  <MenuItem value="false">No</MenuItem>
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
export default TabSentToRepair;
