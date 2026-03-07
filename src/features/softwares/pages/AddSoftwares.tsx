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
        // Set the initial selected state based on InstalledStatus
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

      // Include all items, set InstalledStatus based on selection
      const payload: InstallesSoftwares[] = rows.map((row) => ({
        InstalledSoftwareId: row.InstalledSoftwareId,
        InstalledStatus: selected.includes(row.InstalledSoftwareId), // true if selected, false otherwise
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

  // Avoid a layout jump when reaching the last page with empty rows.
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
          {submitError && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {submitError}
            </Alert>
          )}
          {submitSuccess && (
            <Alert severity="success" sx={{ mb: 2 }}>
              {submitSuccess}
            </Alert>
          )}

          <Paper sx={{ width: "100%", mb: 2 }}>
            <TableContainer>
              <Table sx={{ minWidth: 750 }} aria-label="software table">
                <TableHead>
                  <TableRow>
                    <TableCell padding="checkbox">
                      <Checkbox
                        color="primary"
                        indeterminate={
                          selected.length > 0 && selected.length < rows.length
                        }
                        checked={
                          rows.length > 0 && selected.length === rows.length
                        }
                        onChange={handleSelectAllClick}
                        inputProps={{
                          "aria-label": "select all software",
                        }}
                      />
                    </TableCell>
                    <TableCell>
                      <strong>Software Name</strong>
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {visibleRows.map((row) => {
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
                        sx={{ cursor: "pointer" }}
                      >
                        <TableCell padding="checkbox">
                          <Checkbox
                            color="primary"
                            checked={isItemSelected}
                            inputProps={{
                              "aria-labelledby": labelId,
                            }}
                          />
                        </TableCell>
                        <TableCell component="th" id={labelId} scope="row">
                          {row.SoftwareName}
                        </TableCell>
                      </TableRow>
                    );
                  })}
                  {emptyRows > 0 && (
                    <TableRow
                      style={{
                        height: 53 * emptyRows,
                      }}
                    >
                      <TableCell colSpan={2} />
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
            />
          </Paper>

          <Box sx={{ display: "flex", justifyContent: "space-between", mt: 2 }}>
            {isActive ? (
              <Button
                variant="contained"
                size="large"
                color="success"
                sx={{ minWidth: 200 }}
                onClick={handleAddAsset}
                disabled={isSubmitting}
              >
                {isSubmitting ? <CircularProgress size={24} /> : "Add Software"}
              </Button>
            ) : null}
            <Button
              onClick={goBack}
              variant="contained"
              size="large"
              color="warning"
              sx={{ minWidth: 200 }}
            >
              Go Back
            </Button>
          </Box>
        </Box>
      </FormLayout>
    </LoadingError>
  );
};
