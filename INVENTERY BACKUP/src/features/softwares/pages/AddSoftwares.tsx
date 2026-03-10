import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Box,
  Button,
  CircularProgress,
  Alert,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Checkbox,
  TablePagination,
  Typography,
  Chip,
} from "@mui/material";
import { FormLayout } from "@components/layout/FormLayout";
import {
  addInstalledSoftwares,
  fetchInstalledSoftwares,
  findAssetStatus,
  type InstallesSoftwares,
} from "@features/softwares/services/softwareService";
import { LoadingError } from "@components/layout/LoadingError";

export const AddSoftwares = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [rows, setRows] = useState<InstallesSoftwares[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [selected, setSelected] = useState<number[]>([]);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitSuccess, setSubmitSuccess] = useState<string | null>(null);
  const [isActive, setIsActive] = useState<boolean>();

  useEffect(() => {
    const loadSoftwares = async () => {
      try {
        setLoading(true);
        const assetStatusOutput = await findAssetStatus({
          Type: "is_asset_active",
          AssetId: id,
        });
        setIsActive(assetStatusOutput.IsActive);

        const softwares = await fetchInstalledSoftwares(id!);
        const mappedRows = softwares.map((sw) => ({
          InstalledSoftwareId: sw.InstalledSoftwareId,
          SoftwareName: sw.SoftwareName,
          InstalledStatus: sw.InstalledStatus,
        }));
        setRows(mappedRows);
        const initiallySelected = mappedRows
          .filter((row) => row.InstalledStatus)
          .map((row) => row.InstalledSoftwareId);
        setSelected(initiallySelected);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to load software.",
        );
      } finally {
        setLoading(false);
      }
    };
    loadSoftwares();
  }, []);

  const handleSelectAllClick = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.checked) {
      const newSelected = rows.map((n) => n.InstalledSoftwareId);
      setSelected(newSelected);
      return;
    }
    setSelected([]);
  };

  const handleClick = (id: number) => {
    const selectedIndex = selected.indexOf(id);
    let newSelected: number[] = [];

    if (selectedIndex === -1) {
      newSelected = newSelected.concat(selected, id);
    } else if (selectedIndex === 0) {
      newSelected = newSelected.concat(selected.slice(1));
    } else if (selectedIndex === selected.length - 1) {
      newSelected = newSelected.concat(selected.slice(0, -1));
    } else if (selectedIndex > 0) {
      newSelected = newSelected.concat(
        selected.slice(0, selectedIndex),
        selected.slice(selectedIndex + 1),
      );
    }

    setSelected(newSelected);
  };

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement>,
  ) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleAddAsset = async () => {
    if (!id) {
      setSubmitError("Asset ID is required.");
      return;
    }
    try {
      setIsSubmitting(true);
      setSubmitError(null);
      setSubmitSuccess(null);

      const payload: InstallesSoftwares[] = rows.map((row) => ({
        InstalledSoftwareId: row.InstalledSoftwareId,
        InstalledStatus: selected.includes(row.InstalledSoftwareId),
      }));

      const result = await addInstalledSoftwares(payload);
      setSubmitSuccess(result.Message);
    } catch (err) {
      setSubmitError(
        err instanceof Error
          ? err.message
          : "Failed to update software statuses.",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const goBack = () => {
    navigate("/viewAsset");
  };

  const isSelected = (id: number) => selected.indexOf(id) !== -1;

  const emptyRows =
    page > 0 ? Math.max(0, (1 + page) * rowsPerPage - rows.length) : 0;

  const visibleRows = rows.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage,
  );

  return (
    <LoadingError loading={loading} error={error}>
      <FormLayout title="Add Installed Software">
        <Box sx={{ width: "100%" }}>

          {/* Alerts */}
          {submitError && (
            <Alert
              severity="error"
              sx={{
                mb: 1.5,
                borderRadius: "8px",
                fontSize: "0.85rem",
                py: 0.5,
              }}
            >
              {submitError}
            </Alert>
          )}
          {submitSuccess && (
            <Alert
              severity="success"
              sx={{
                mb: 1.5,
                borderRadius: "8px",
                fontSize: "0.85rem",
                py: 0.5,
              }}
            >
              {submitSuccess}
            </Alert>
          )}

          {/* Table Card */}
          <Paper
            elevation={0}
            sx={{
              width: "100%",
              mb: 1.5,
              borderRadius: "12px",
              border: "1px solid",
              borderColor: "divider",
              overflow: "hidden",
            }}
          >
            {/* Table Header Bar */}
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                px: 2,
                py: 1.25,
                borderBottom: "1px solid",
                borderColor: "divider",
                bgcolor: "grey.50",
              }}
            >
              <Typography
                variant="subtitle2"
                sx={{
                  fontWeight: 600,
                  color: "text.primary",
                  fontSize: "0.875rem",
                  letterSpacing: "0.01em",
                }}
              >
                Software List
              </Typography>
              {selected.length > 0 && (
                <Chip
                  label={`${selected.length} selected`}
                  size="small"
                  color="primary"
                  sx={{
                    height: 22,
                    fontSize: "0.75rem",
                    fontWeight: 600,
                    borderRadius: "6px",
                  }}
                />
              )}
            </Box>

            <TableContainer>
              <Table
                sx={{ minWidth: 400 }}
                size="small"
                aria-label="software table"
              >
                <TableHead>
                  <TableRow
                    sx={{
                      "& th": {
                        bgcolor: "grey.50",
                        borderBottom: "2px solid",
                        borderColor: "divider",
                        py: 1,
                        fontSize: "0.78rem",
                        fontWeight: 700,
                        color: "text.secondary",
                        textTransform: "uppercase",
                        letterSpacing: "0.05em",
                      },
                    }}
                  >
                    <TableCell padding="checkbox" sx={{ width: 48, pl: 1.5 }}>
                      <Checkbox
                        color="primary"
                        size="small"
                        indeterminate={
                          selected.length > 0 && selected.length < rows.length
                        }
                        checked={
                          rows.length > 0 && selected.length === rows.length
                        }
                        onChange={handleSelectAllClick}
                        inputProps={{ "aria-label": "select all software" }}
                        sx={{ p: 0.5 }}
                      />
                    </TableCell>
                    <TableCell>Software Name</TableCell>
                    <TableCell align="right" sx={{ pr: 2 }}>
                      Status
                    </TableCell>
                  </TableRow>
                </TableHead>

                <TableBody>
                  {visibleRows.map((row, index) => {
                    const isItemSelected = isSelected(row.InstalledSoftwareId);
                    const labelId = `enhanced-table-checkbox-${row.InstalledSoftwareId}`;

                    return (
                      <TableRow
                        hover
                        onClick={() => handleClick(row.InstalledSoftwareId)}
                        role="checkbox"
                        aria-checked={isItemSelected}
                        tabIndex={-1}
                        key={row.InstalledSoftwareId}
                        selected={isItemSelected}
                        sx={{
                          cursor: "pointer",
                          transition: "background-color 0.15s ease",
                          bgcolor: isItemSelected
                            ? "primary.50"
                            : index % 2 === 0
                              ? "background.paper"
                              : "grey.50",
                          "&.Mui-selected": {
                            bgcolor: "primary.50",
                            "&:hover": { bgcolor: "primary.100" },
                          },
                          "& td": {
                            py: 0.75,
                            fontSize: "0.85rem",
                            borderBottom: "1px solid",
                            borderColor: "divider",
                          },
                          "&:last-child td": { borderBottom: 0 },
                        }}
                      >
                        <TableCell padding="checkbox" sx={{ pl: 1.5 }}>
                          <Checkbox
                            color="primary"
                            size="small"
                            checked={isItemSelected}
                            inputProps={{ "aria-labelledby": labelId }}
                            sx={{ p: 0.5 }}
                          />
                        </TableCell>
                        <TableCell
                          component="th"
                          id={labelId}
                          scope="row"
                          sx={{
                            fontWeight: isItemSelected ? 500 : 400,
                            color: isItemSelected
                              ? "primary.main"
                              : "text.primary",
                          }}
                        >
                          {row.SoftwareName}
                        </TableCell>
                        <TableCell align="right" sx={{ pr: 2 }}>
                          <Chip
                            label={isItemSelected ? "Installed" : "Not Installed"}
                            size="small"
                            sx={{
                              height: 20,
                              fontSize: "0.7rem",
                              fontWeight: 600,
                              borderRadius: "5px",
                              bgcolor: isItemSelected
                                ? "success.100"
                                : "grey.100",
                              color: isItemSelected
                                ? "success.800"
                                : "text.secondary",
                              border: "1px solid",
                              borderColor: isItemSelected
                                ? "success.300"
                                : "grey.300",
                            }}
                          />
                        </TableCell>
                      </TableRow>
                    );
                  })}

                  {emptyRows > 0 && (
                    <TableRow style={{ height: 40 * emptyRows }}>
                      <TableCell colSpan={3} />
                    </TableRow>
                  )}

                  {rows.length === 0 && !loading && (
                    <TableRow>
                      <TableCell colSpan={3} align="center" sx={{ py: 4 }}>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ fontSize: "0.85rem" }}
                        >
                          No software records found.
                        </Typography>
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </TableContainer>

            <TablePagination
              rowsPerPageOptions={[10, 25, 50]}
              component="div"
              count={rows.length}
              rowsPerPage={rowsPerPage}
              page={page}
              onPageChange={handleChangePage}
              onRowsPerPageChange={handleChangeRowsPerPage}
              sx={{
                borderTop: "1px solid",
                borderColor: "divider",
                "& .MuiTablePagination-toolbar": { minHeight: 44, px: 1.5 },
                "& .MuiTablePagination-selectLabel, & .MuiTablePagination-displayedRows":
                  { fontSize: "0.78rem" },
              }}
            />
          </Paper>

          {/* Action Buttons */}
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              gap: 1.5,
              flexWrap: "wrap",
            }}
          >
            {isActive ? (
              <Button
                variant="contained"
                size="medium"
                color="success"
                sx={{
                  minWidth: 160,
                  borderRadius: "8px",
                  fontWeight: 600,
                  fontSize: "0.85rem",
                  textTransform: "none",
                  py: 0.9,
                  boxShadow: "0 2px 6px rgba(76,175,80,0.3)",
                  "&:hover": {
                    boxShadow: "0 4px 10px rgba(76,175,80,0.4)",
                  },
                }}
                onClick={handleAddAsset}
                disabled={isSubmitting}
              >
                {isSubmitting ? (
                  <CircularProgress size={18} color="inherit" />
                ) : (
                  "Save Software"
                )}
              </Button>
            ) : null}

            <Button
              onClick={goBack}
              variant="contained"
              size="medium"
              color="warning"
              sx={{
                minWidth: 160,
                borderRadius: "8px",
                fontWeight: 600,
                fontSize: "0.85rem",
                textTransform: "none",
                py: 0.9,
                boxShadow: "0 2px 6px rgba(255,152,0,0.3)",
                "&:hover": {
                  boxShadow: "0 4px 10px rgba(255,152,0,0.4)",
                },
              }}
            >
              Go Back
            </Button>
          </Box>
        </Box>
      </FormLayout>
    </LoadingError>
  );
};