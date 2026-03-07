import { useEffect, useState } from "react";

import { useNavigate } from "react-router-dom";

import {
  Grid,
  TextField,
  Button,
  CircularProgress,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
} from "@mui/material";

import { FormDropdownDepartment } from "@components/dropdowns/FormDropdownDepartment";

import type { DepartmentItem, LocationItem, UsersList } from "../types";
import {
  addUser,
  fetchDepartment,
  fetchLocations,
} from "../services/usersSerice";
import { LoadingError } from "@components/layout/LoadingError";

function AddUser() {
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [departments, setDepartments] = useState<DepartmentItem[]>([]);
  const [isCapexUser, setIsCapexUser] = useState<boolean>(false);
  const [designations, setDesignations] = useState<string>("");
  const [fullName, setFullName] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [phone, setPhone] = useState<string>("");
  const [departmentId, setDepartmentId] = useState<number>(0);
  const [locations, setLocations] = useState<LocationItem[]>([]);
  const [locationId, setLocationId] = useState<number>(0);

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await fetchDepartment();
      if (response.Ok === false || response.ExceptionMessage) {
        setError(response.ExceptionMessage);
        return;
      }
      setDepartments(response);
      const locationsResponse = await fetchLocations();
      if (
        locationsResponse.Ok === false ||
        locationsResponse.ExceptionMessage
      ) {
        setError(locationsResponse.ExceptionMessage);
        return;
      }
      setLocations(locationsResponse);
    } catch (error: any) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleSave = async () => {
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return null;
    }
    try {
      setIsSubmitting(true);
      const user: UsersList = {
        Id: 0,
        FullName: fullName,
        Email: email,
        Phone: phone,
        Designation: designations,
        DepartmentId: departmentId,
        LocationId: locationId,
        isCapexUser: isCapexUser,
      };
      const result = await addUser(user);
      if (result.Ok === false || result.ExceptionMessage) {
        window.alert(result.ExceptionMessage);
        console.log(result);
        return;
      }
      window.alert("User added successfully");
      navigate("/AddMasterData");
    } catch (error: any) {
      setError(error.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const validateForm = (): string | null => {
    if (!fullName) return "Name is required";
    if (!email) return "Email is required";
    if (!designations) return "Designation is required";
    if (departmentId === 0) return "Department is required";
    if (locationId === 0) return "Location is required";
    return null;
  };

  return (
    <LoadingError loading={loading} error={error}>
      <Grid container spacing={1}>
        <Grid size={{ xs: 12 }}>
          <h2>Add User</h2>
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Full Name"
            fullWidth
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Email"
            fullWidth
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Phone"
            fullWidth
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Designation"
            fullWidth
            value={designations}
            onChange={(e) => setDesignations(e.target.value)}
          />
        </Grid>
        <FormDropdownDepartment
          departments={departments}
          departmentId={departmentId}
          onDepartmentChange={(value) => setDepartmentId(value)}
        />
        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControl fullWidth size="small" required>
            <InputLabel>Select Next Location</InputLabel>
            <Select
              value={locationId}
              label="Select Next Location"
              onChange={(e) => setLocationId(e.target.value as number)}
            >
              {locations.map((loc) => (
                <MenuItem key={loc.Id} value={loc.Id}>
                  {loc.Name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControl fullWidth size="small">
            <InputLabel id="is-active-label">This user is</InputLabel>
            <Select
              labelId="is-active-label"
              id="is-active-select"
              label="Status"
              value={isCapexUser ? "true" : "false"}
              onChange={(e) => setIsCapexUser(e.target.value === "true")}
            >
              <MenuItem value="false">only related this inventery</MenuItem>
              <MenuItem value="true">related for both applications</MenuItem>
            </Select>
          </FormControl>
        </Grid>
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
              size="large"
              sx={{ minWidth: 120 }}
              onClick={handleSave}
            >
              {isSubmitting ? <CircularProgress size={24} /> : "Save"}
            </Button>
          </Grid>
          {submitError && (
            <Grid size={12}>
              <div
                style={{
                  color: "red",
                  textAlign: "center",
                  padding: "16px",
                  backgroundColor: "#ffebee",
                  borderRadius: "4px",
                }}
              >
                {submitError}
              </div>
            </Grid>
          )}
        </Grid>
      </Grid>
    </LoadingError>
  );
}

export default AddUser;
