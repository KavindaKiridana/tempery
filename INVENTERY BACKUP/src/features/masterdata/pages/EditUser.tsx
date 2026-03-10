import { useEffect, useState } from "react";

import { useParams, useNavigate } from "react-router-dom";

import {
  Grid,
  TextField,
  Button,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
} from "@mui/material";

import { FormDropdownDepartment } from "../../../components/dropdowns/FormDropdownDepartment";
import { fetchDepartment, getUsers, updateUser } from "../services/usersSerice";

import type { GetUsersParams, UsersList, DepartmentItem } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

function EditUser() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [departments, setDepartments] = useState<DepartmentItem[]>([]);
  const [isCapexUser, setIsCapexUser] = useState<boolean>(false);
  const [isUsed, setIsUsed] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [designations, setDesignations] = useState<string>("");
  const [fullName, setFullName] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [phone, setPhone] = useState<string>("");
  const [departmentId, setDepartmentId] = useState<number>(0);
  const [isActive, setIsActive] = useState<boolean>(false);
  const [LocationName, setLocationName] = useState<string>("");

  const loadData = async () => {
    try {
      setLoading(true);
      const params: GetUsersParams = {
        needEveryUsers: false,
        requestedUserId: Number(id),
      };
      const usersOutput = await getUsers(params);
      if (usersOutput.Ok === false || usersOutput.ExceptionMessage) {
        setError(usersOutput.ExceptionMessage);
        return;
      }
      setIsCapexUser(usersOutput.isCapexUser);
      setIsUsed(usersOutput.IsUsed || false);
      setFullName(usersOutput.FullName || "");
      setEmail(usersOutput.Email || "");
      setPhone(usersOutput.Phone || "");
      setDesignations(usersOutput.Designation || "");
      setDepartmentId(usersOutput.DepartmentId || 0);
      setIsActive(usersOutput.IsActive || false);
      setLocationName(usersOutput.LocationName || "");
      const response = await fetchDepartment();
      if (response.Ok === false || response.ExceptionMessage) {
        setError(usersOutput.ExceptionMessage);
        return;
      }
      setDepartments(response);
    } catch (error: any) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [id]);

  const handleChange = (field: string, value: any) => {
    switch (field) {
      case "FullName":
        setFullName(value);
        break;
      case "Email":
        setEmail(value);
        break;
      case "Phone":
        setPhone(value);
        break;
      case "Designation":
        setDesignations(value);
        break;
      case "DepartmentId":
        setDepartmentId(value);
        break;
      case "IsActive":
        setIsActive(value);
        break;
      default:
        break;
    }
  };

  const handleSave = async () => {
    try {
      setLoading(true);
      const user: UsersList = {
        Id: Number(id),
        FullName: fullName,
        Email: email,
        Phone: phone,
        Designation: designations,
        DepartmentId: departmentId,
        isCapexUser,
        IsActive: isActive,
        IsUsed: isUsed,
      };
      await updateUser(user);
      navigate("/AddMasterData");
    } catch (error: any) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <LoadingError loading={loading} error={error}>
      <Grid container spacing={1}>
        <Grid size={{ xs: 12 }}>
          <h3>Edit User</h3>
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Full Name"
            fullWidth
            value={fullName}
            onChange={(e) => handleChange("FullName", e.target.value)}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Email"
            fullWidth
            value={email}
            onChange={(e) => handleChange("Email", e.target.value)}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Phone"
            fullWidth
            value={phone}
            onChange={(e) => handleChange("Phone", e.target.value)}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Current Location"
            value={LocationName}
            disabled
            fullWidth
            size="small"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Designation"
            fullWidth
            value={designations}
            onChange={(e) => handleChange("Designation", e.target.value)}
          />
        </Grid>
        <FormDropdownDepartment
          departments={departments}
          departmentId={departmentId}
          onDepartmentChange={(value) => handleChange("DepartmentId", value)}
        />
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Is Capex User"
            fullWidth
            value={isCapexUser ? "Yes" : "No"}
            InputProps={{ readOnly: true }}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <FormControl fullWidth size="small">
            <InputLabel id="is-active-label">Status</InputLabel>
            <Select
              labelId="is-active-label"
              id="is-active-select"
              value={isActive ? "Active" : "Inactive"}
              label="Status"
              onChange={(e) =>
                handleChange("IsActive", e.target.value === "Active")
              }
            >
              <MenuItem value="Active">Active</MenuItem>
              <MenuItem value="Inactive">Inactive</MenuItem>
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
              onClick={handleSave}
              size="large"
              sx={{ minWidth: 120 }}
              disabled={isUsed}
              title={isUsed ? "This user is in use and cannot be edited" : ""}
            >
              Save
            </Button>
          </Grid>
        </Grid>
      </Grid>
    </LoadingError>
  );
}

export default EditUser;
