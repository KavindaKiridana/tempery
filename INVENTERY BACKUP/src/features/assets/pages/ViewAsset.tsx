import { useEffect, useState } from "react";
import {
  Button,
  Container,
  Card,
  CardContent,
  Box,
  Typography,
  Chip,
} from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { getAssets } from "../services/assetListService";
import type { AssetView } from "../types";
import { LoadingError } from "@components/layout/LoadingError";

function ViewAsset() {
  const [rows, setRows] = useState<AssetView[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery] = useState<string>("");

  // ─── Data Fetching ──────────────────────────────────────────────────────────
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
    fetchAssets(searchQuery);
  }, [searchQuery]);

  // ─── Column Definitions ─────────────────────────────────────────────────────
  const columns: GridColDef<AssetView>[] = [
    {
      field: "AssetId",
      headerName: "Asset ID",
      width: 80,
      renderCell: (params) => (
        <Typography
          variant="body2"
          sx={{
            fontFamily: "monospace",
            fontSize: "0.78rem",
            color: "text.secondary",
          }}
        >
          {params.value}
        </Typography>
      ),
    },
    { field: "Name", headerName: "Name", width: 90 },
    { field: "CompanyName", headerName: "Company", width: 100 },
    { field: "LocationName", headerName: "Location", width: 100 },
    { field: "ManufactureSN", headerName: "Serial Number", width: 100 },
    { field: "CurrentUser", headerName: "Using By", width: 100 },
    {
      field: "Type",
      headerName: "Type",
      width: 90,
      renderCell: (params) => (
        <Chip
          label={params.value}
          size="small"
          variant="outlined"
          sx={{ fontSize: "0.7rem" }}
        />
      ),
    },
    {
      field: "IsAvailable",
      headerName: "Available",
      width: 75,
      renderCell: (params) => (
        <Chip
          label={params.value ? "Yes" : "No"}
          size="small"
          color={params.value ? "success" : "default"}
          sx={{ fontSize: "0.7rem" }}
        />
      ),
    },
    {
      field: "IsActive",
      headerName: "Active",
      width: 60,
      renderCell: (params) => (
        <Chip
          label={params.value ? "Yes" : "No"}
          size="small"
          color={params.value ? "primary" : "default"}
          sx={{ fontSize: "0.7rem" }}
        />
      ),
    },
    {
      field: "actions",
      headerName: "Actions",
      width: 280,
      sortable: false,
      filterable: false,
      renderCell: (params) => {
        const id = params.row.AssetId;
        return (
          <Box
            sx={{
              display: "flex",
              gap: 0.75,
              alignItems: "center",
              height: "100%",
            }}
          >
            {/* Transfer — only for active assets */}
            {params.row.IsActive && (
              <Button
                onClick={() =>
                  window.open(`/TransactionPage/${id}`, "_blank", "noreferrer")
                }
                size="small"
                variant="outlined"
                sx={{ minWidth: 70, py: 0.25, fontSize: "0.75rem" }}
              >
                Transfer
              </Button>
            )}
            {/* View — always shown */}
            <Button
              onClick={() =>
                window.open(`/ViewInfo/${id}`, "_blank", "noreferrer")
              }
              size="small"
              variant="outlined"
              sx={{ minWidth: 55, py: 0.25, fontSize: "0.75rem" }}
            >
              View
            </Button>
            {/* Edit — only for active assets */}
            {params.row.IsActive && (
              <Button
                onClick={() =>
                  window.open(`/EditAssetForm/${id}`, "_blank", "noreferrer")
                }
                size="small"
                variant="outlined"
                sx={{ minWidth: 50, py: 0.25, fontSize: "0.75rem" }}
              >
                Edit
              </Button>
            )}
            {/* Softwares — hidden for SparePart type */}
            {params.row.Type !== "SparePart" && (
              <Button
                onClick={() =>
                  window.open(`/AddSoftwares/${id}`, "_blank", "noreferrer")
                }
                size="small"
                variant="outlined"
                sx={{ minWidth: 80, py: 0.25, fontSize: "0.75rem" }}
              >
                Softwares
              </Button>
            )}
          </Box>
        );
      },
    },
  ];

  // ─── Render ─────────────────────────────────────────────────────────────────
  return (
    <LoadingError loading={loading} error={error}>
      <Container maxWidth="xl" sx={{ py: 2 }}>
        {/* ── Page Header ── */}
        <Box sx={{ mb: 1.5 }}>
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            Asset List
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {rows.length} asset{rows.length !== 1 ? "s" : ""} found
          </Typography>
        </Box>

        {/* ── Data Table ── */}
        <Card variant="outlined">
          <CardContent sx={{ p: 1, "&:last-child": { pb: 1 } }}>
            <Box sx={{ width: "100%" }}>
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
                showToolbar
                slotProps={{
                  toolbar: {
                    showQuickFilter: true,
                    csvOptions: { allColumns: false },
                    printOptions: { disableToolbarButton: true },
                  },
                }}
                localeText={{
                  noRowsLabel: "No assets found",
                }}
                sx={{
                  border: "none",
                  // Tighter row height
                  "& .MuiDataGrid-row": {
                    minHeight: "40px !important",
                    maxHeight: "44px !important",
                  },
                  "& .MuiDataGrid-cell": {
                    py: 0.5,
                    display: "flex",
                    alignItems: "center",
                  },
                  // Header styling
                  "& .MuiDataGrid-columnHeader": {
                    bgcolor: "action.hover",
                    fontWeight: 700,
                    fontSize: "0.78rem",
                  },
                  "& .MuiDataGrid-columnHeaderTitle": {
                    fontWeight: 700,
                  },
                  // Subtle row hover
                  "& .MuiDataGrid-row:hover": {
                    bgcolor: "action.hover",
                  },
                  // Remove outer border
                  "& .MuiDataGrid-virtualScroller": {
                    overflow: "auto",
                  },
                  // Toolbar spacing
                  "& .MuiDataGrid-toolbarContainer": {
                    px: 1,
                    py: 0.5,
                    borderBottom: "1px solid",
                    borderColor: "divider",
                  },
                }}
              />
            </Box>
          </CardContent>
        </Card>
      </Container>
    </LoadingError>
  );
}

export default ViewAsset;
