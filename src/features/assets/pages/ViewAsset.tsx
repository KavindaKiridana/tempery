import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  CircularProgress,
  Typography,
  TextField,
  Button,
  Container,
  Card,
  CardContent,
  Box,
} from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { getAssets, getExcel } from "../services/assetListService";
import type { AssetView } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

function ViewAsset() {
  const [rows, setRows] = useState<AssetView[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [isExporting, setIsExporting] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState<string>("");
  const navigate = useNavigate();

  const fetchAssets = async (searchQuery: string = "") => {
    try {
      setLoading(true);
      const assets = await getAssets(searchQuery);
      setRows(assets);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load assets.");
      setRows([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      fetchAssets(searchQuery);
    }, 1000);
    return () => clearTimeout(timer);
  }, [searchQuery]);

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value);
  };

  const onExport = async () => {
    try {
      setIsExporting(true);
      setError(null);
      await getExcel(searchQuery);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to export excel file.",
      );
    } finally {
      setIsExporting(false);
    }
  };

  const columns: GridColDef<AssetView>[] = [
    { field: "AssetId", headerName: "Asset ID", width: 90 },
    { field: "Name", headerName: "Name", width: 100 },
    { field: "CompanyName", headerName: "Company", width: 100 },
    { field: "LocationName", headerName: "Location", width: 100 },
    { field: "ManufactureSN", headerName: "Serial Number", width: 100 },
    { field: "CurrentUser", headerName: "User", width: 100 },
    { field: "Type", headerName: "Type", width: 80 },
    { field: "IsAvailable", headerName: "Available", width: 80 },
    { field: "IsActive", headerName: "IsActive", width: 80 },
    {
      field: "actions",
      headerName: "Actions",
      width: 300,
      sortable: false,
      filterable: false,
      renderCell: (params) => {
        const id = params.row.AssetId;
        return (
          <Box sx={{ display: "flex", gap: 1 }}>
            {params.row.IsActive ? (
              <Button
                onClick={() => navigate(`/TransactionPage/${id}`)}
                size="small"
                variant="outlined"
                sx={{ minWidth: "60px" }}
              >
                Transfer
              </Button>
            ) : null}
            <Button
              onClick={() => navigate(`/ViewInfo/${id}`)}
              size="small"
              variant="outlined"
              sx={{ minWidth: "60px" }}
            >
              View
            </Button>
            {params.row.IsActive ? (
              <Button
                onClick={() => navigate(`/EditAssetForm/${id}`)}
                size="small"
                variant="outlined"
                sx={{ minWidth: "60px" }}
              >
                Edit
              </Button>
            ) : null}
            {params.row.Type !== "SparePart" && (
              <Button
                onClick={() => navigate(`/AddSoftwares/${id}`)}
                size="small"
                variant="outlined"
                sx={{ minWidth: "60px" }}
              >
                Softwares
              </Button>
            )}
          </Box>
        );
      },
    },
  ];

  return (
    <LoadingError loading={loading} error={error}>
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Card>
          <CardContent sx={{ p: 1 }}>
            <Box sx={{ mb: 1 }}>
              <Box sx={{ display: "flex", gap: 2, alignItems: "flex-start" }}>
                <TextField
                  label="Search by Name, Company, Location, Type, Availability, Serial Number or User"
                  variant="outlined"
                  fullWidth
                  value={searchQuery}
                  onChange={handleSearchChange}
                  placeholder="Type to search..."
                />
                <Button
                  variant="contained"
                  onClick={onExport}
                  disabled={isExporting}
                  sx={{ minWidth: "120px", alignSelf: "flex-start" }}
                >
                  {isExporting ? (
                    <CircularProgress size={24} color="inherit" />
                  ) : (
                    "Export"
                  )}
                </Button>
              </Box>
            </Box>

            {rows.length === 0 ? (
              <Typography textAlign="center" py={4}>
                No assets found
              </Typography>
            ) : (
              <Box sx={{ height: "auto", width: "100%" }}>
                <DataGrid
                  rows={rows}
                  columns={columns}
                  getRowId={(row) => row.AssetId}
                  initialState={{
                    pagination: {
                      paginationModel: { pageSize: 8 },
                    },
                  }}
                  pageSizeOptions={[5, 8, 10, 25]}
                  disableRowSelectionOnClick
                  autoHeight
                  sx={{
                    "& .MuiDataGrid-virtualScroller": {
                      overflow: "auto",
                    },
                  }}
                />
              </Box>
            )}
          </CardContent>
        </Card>
      </Container>
    </LoadingError>
  );
}

export default ViewAsset;
