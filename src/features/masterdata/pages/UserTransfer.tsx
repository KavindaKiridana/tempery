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

import { LoadingError } from "@components/layout/LoadingError";
import { useNavigate, useParams } from "react-router-dom";
import { fetchLocations } from "../services/usersSerice";
import type { LocationItem } from "../types";

function UserTransfer() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [currentLocationId, setCurrentLocationId] = useState<number>(0);
  const [nextLocationId, setNextLocationId] = useState<number>(0);
  const [nextLocations, setNextLocations] = useState<LocationItem[]>([]);
  const [note, setNote] = useState("");
  const [locationName, setLocationName] = useState("");
  const [userName, setUserName] = useState("");

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const load = async () => {
    try {
      setLoading(true);
      const payload2 = {
        Type: "user_transfer_page_data",
        UserId: parseInt(id!),
      };
      const res1 = await getTransaction(payload2);
      if (res1.Ok === false || res1.ExceptionMessage) {
        setError(res1.ExceptionMessage);
        console.warn("User transfer page data", res1);
        return;
      } else {
        setCurrentLocationId(res1.LocationId);
        setLocationName(res1.LocationName);
        setUserName(res1.UserName);
        const res2 = await fetchLocations();
        if (res2.Ok === false || res2.ExceptionMessage) {
          setError(res2.ExceptionMessage);
          console.warn("Locations data", res2);
          return;
        }
        setNextLocations(res2);
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
      Type: "USER_LOCATION_CHANGED",
      Time: new Date().toISOString(),
      FromId: currentLocationId!, // current location
      ToId: nextLocationId, // next location
      Note: note,
      UserId: parseInt(id!), // user id
    };
    const result = await postTransaction(postData);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      alert("User moved successfully!");
      setNextLocationId(0);
      setNote("");
      load();
    }
    setIsSubmitting(false);
  };

  const validateForm = (): string | null => {
    if (nextLocationId === 0) return "Please select the next location";
    if (note.trim() === "") return "Please write a note";
    return null; // No errors
  };

  return (
    <LoadingError loading={loading} error={error}>
      <Grid container spacing={1}>
        <Grid size={{ xs: 12 }}>
          <h3>Move User to Another Location</h3>
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControl fullWidth size="small" required>
            <TextField
              label="User Name"
              type="text"
              size="small"
              value={userName}
              InputProps={{
                readOnly: true,
              }}
            />
          </FormControl>
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
              value={nextLocationId}
              label="Select Next Location"
              onChange={(e) => setNextLocationId(e.target.value as number)}
            >
              {nextLocations.map((loc) => (
                <MenuItem key={loc.Id} value={loc.Id}>
                  {loc.Name}
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
        <Grid
          size={12}
          sx={{ mt: 2, display: "flex", justifyContent: "center" }}
        ></Grid>
        <Grid size={{ xs: 12 }} container spacing={2} justifyContent="flex-end">
          <Grid>
            <Button
              variant="outlined"
              color="secondary"
              onClick={() => navigate("/AddMasterData")}
              size="large"
              sx={{ minWidth: 120 }}
            >
              Cancel
            </Button>
          </Grid>
          <Grid>
            <Button
              variant="contained"
              color="primary"
              onClick={handleSubmit}
              size="large"
              sx={{ minWidth: 120 }}
            >
              {isSubmitting ? <CircularProgress size={24} /> : "Move User"}
            </Button>
          </Grid>
        </Grid>
        {submitError && (
          <div style={{ color: "red", marginTop: 16 }}>{submitError}</div>
        )}
      </Grid>
    </LoadingError>
  );
}
export default UserTransfer;
