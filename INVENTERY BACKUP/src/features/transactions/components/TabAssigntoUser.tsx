import { useEffect, useState } from "react";

import {
  Button,
  CircularProgress,
  FormControl,
  Grid,
  InputLabel,
  TextField,
  Select,
  MenuItem,
} from "@mui/material";

import {
  postTransaction,
  getTransaction,
} from "@features/transactions/services/tracsactionService";

import type { AvailableAssetItem, PostTransaction, UserItem } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

interface TabAssigntoUserProps {
  id?: string;
  onSuccess?: () => void;
}

function TabAssigntoUser({ id, onSuccess }: TabAssigntoUserProps) {
  const [userId, setUserId] = useState<number>(0);
  const [note, setNote] = useState("");
  const [assets, setAssets] = useState<AvailableAssetItem>();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [users, setUsers] = useState<UserItem[]>([]);
  const [sparePartStatus, setSparePartStatus] = useState<boolean>(false);

  const loadData = async () => {
    try {
      setLoading(true);
      const sentData1 = { Type: "is_active_spare_part", AssetId: id! };
      const fetchedSparePartStatus = await getTransaction(sentData1);
      // Check if the response indicates an error
      if (
        fetchedSparePartStatus.Ok === false ||
        fetchedSparePartStatus.ExceptionMessage
      ) {
        setError(fetchedSparePartStatus.ExceptionMessage);
        console.warn("fetchedSparePartStatus", fetchedSparePartStatus);
        return;
      }
      setSparePartStatus(fetchedSparePartStatus);
      if (!sparePartStatus) {
        const sentData2 = { Type: "assign_to_user", AssetId: id! };
        const fetchedAssets = await getTransaction(sentData2);
        // Check if the response indicates an error
        if (fetchedAssets.Ok === false || fetchedAssets.ExceptionMessage) {
          setError(fetchedAssets.ExceptionMessage);
          console.warn("fetchedAssets", fetchedAssets);
          return;
        }
        setAssets(fetchedAssets);
        const sentData3 = { Type: "get_related_users", AssetId: id! };
        const fetchedUsers = await getTransaction(sentData3);
        // Check if the response indicates an error
        if (fetchedUsers.Ok === false || fetchedUsers.ExceptionMessage) {
          setError(fetchedUsers.ExceptionMessage);
          return;
        }
        setUsers(fetchedUsers);
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

  const validateForm = (): string | null => {
    if (userId === 0) return "Please select a user";
    if (note.trim() === "") return "Please write a note";
    return null; // No errors
  };

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
        Type: "ASSET_ASSIGNED_TO_USER",
        AssetId: id!,
        Time: new Date().toISOString(),
        FromId: assets!.LocationId,
        ToId: userId,
        Note: note,
      };
      const output = await postTransaction(formData);
      if (typeof output === "string") {
        console.log("API Request Data:", formData);
        console.error("API Error:", output);
        setSubmitError(output);
      } else {
        // Handle successful response
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

  return (
    <LoadingError loading={loading} error={error}>
      {sparePartStatus ? (
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            height: "80vh",
          }}
        >
          <h3>
            Cannot assign this asset. This asset is an active spare part in a of
            another asset.
          </h3>
        </div>
      ) : (
        <Grid container spacing={1}>
          <Grid size={{ xs: 12 }}>
            <h3>Assign to User</h3>
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
                value={id + " : " + assets?.AssetName}
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
            <FormControl fullWidth size="small" required>
              <TextField
                label="Location"
                type="text"
                size="small"
                value={assets?.LocationName}
              />
            </FormControl>
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <FormControl fullWidth size="small" required>
              <InputLabel id="user-label">User</InputLabel>
              <Select
                labelId="user-label"
                label="User"
                value={userId}
                onChange={(e) => setUserId(Number(e.target.value))}
              >
                {users
                  .filter((user) => user.IsActive === true)
                  .map((user) => (
                    <MenuItem key={user.Id} value={user.Id}>
                      {user.FullName}
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
              {isSubmitting ? <CircularProgress size={24} /> : "Assign Asset"}
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
      )}
    </LoadingError>
  );
}
export { TabAssigntoUser };
