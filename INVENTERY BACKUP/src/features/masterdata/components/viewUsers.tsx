import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Typography,
  TextField,
  Button,
  CardContent,
  Box,
  Card,
} from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { getUsers } from "../services/usersSerice";
import type { UsersList } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

function ViewUsers() {
  const [rows, setRows] = useState<UsersList[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState<string>("");
  const navigate = useNavigate();

  const fetchUsers = async (searchQuery: string = "") => {
    try {
      setLoading(true);
      const params = { needEveryUsers: true, search: searchQuery };
      const usersOutput = await getUsers(params);
      if (usersOutput.Ok === false || usersOutput.ExceptionMessage) {
        setError(usersOutput.ExceptionMessage);
        console.warn("usersOutput", usersOutput);
        return;
      }
      setRows(usersOutput);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load users.");
      setRows([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      fetchUsers(searchQuery);
    }, 1000);
    return () => clearTimeout(timer);
  }, [searchQuery]);

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value);
  };

  const columns: GridColDef<UsersList>[] = [
    { field: "FullName", headerName: "Full Name", width: 120 },
    { field: "Email", headerName: "Email", width: 120 },
    { field: "Phone", headerName: "Phone", width: 80 },
    { field: "Designation", headerName: "Designation", width: 120 },
    { field: "DepartmentName", headerName: "Department", width: 100 },
    { field: "LocationName", headerName: "Location", width: 100 },
    { field: "isCapexUser", headerName: "Is Capex User", width: 80 },
    { field: "IsActive", headerName: "Is Active", width: 80 },
    { field: "AddedUserName", headerName: "Added By", width: 80 },
    { field: "AddedTime", headerName: "Added Time", width: 80 },
    {
      field: "actions",
      headerName: "Actions",
      width: 240,
      sortable: false,
      filterable: false,
      renderCell: (params) => {
        const id = params.row.Id;
        return (
          <Box sx={{ display: "flex", gap: 1 }}>
            {params.row.IsActive ? (
              <>
                <Button
                  onClick={() => navigate(`/EditUser/${id}`)}
                  size="small"
                  variant="outlined"
                >
                  Edit
                </Button>
                <Button
                  onClick={() => navigate(`/UserTransfer/${id}`)}
                  size="small"
                  variant="outlined"
                  disabled={!params.row.IsActive}
                >
                  Transfer
                </Button>
                <Button
                  onClick={() => navigate(`/ResignUser/${id}`)}
                  size="small"
                  variant="outlined"
                  disabled={!params.row.IsActive}
                >
                  Resign
                </Button>
              </>
            ) : null}
          </Box>
        );
      },
    },
  ];

  return (
    <LoadingError loading={loading} error={error}>
      <Card>
        <CardContent sx={{ p: 1 }}>
          <Box sx={{ mb: 1 }}>
            <Box sx={{ display: "flex", gap: 2, alignItems: "flex-start" }}>
              <TextField
                label="Search by FullName, Email, Phone, Designation or Department"
                variant="outlined"
                fullWidth
                value={searchQuery}
                onChange={handleSearchChange}
                placeholder="Type to search..."
              />
              <Button
                variant="contained"
                onClick={() => navigate("/AddUser")}
                sx={{ minWidth: "120px" }}
              >
                Add User
              </Button>
            </Box>
          </Box>
          {rows.length === 0 ? (
            <Typography>No users found</Typography>
          ) : (
            <Box sx={{ flexGrow: 1, width: "100%" }}>
              <DataGrid
                rows={rows}
                columns={columns}
                getRowId={(row) => row.Id}
                initialState={{
                  pagination: {
                    paginationModel: { pageSize: 8 },
                  },
                }}
                pageSizeOptions={[5, 8, 10, 25]}
                disableRowSelectionOnClick
                sx={{
                  height: "100%",
                  width: "100%",
                }}
              />
            </Box>
          )}
        </CardContent>
      </Card>
    </LoadingError>
  );
}

export default ViewUsers;
