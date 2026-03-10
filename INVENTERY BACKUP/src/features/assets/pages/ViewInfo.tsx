import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Card,
  CardContent,
  Grid,
  Alert,
  Container,
  TableContainer,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Paper,
  Button,
  Chip,
} from "@mui/material";
import { getTransaction } from "@features/transactions/services/tracsactionService";
import { getAssetInfo } from "../services/assetEditService";
import { getHistoryTransactions } from "../services/assetHistoryService";
import type { EditAssetView, TransactionResponse } from "../types";
import type { AvailableAssetItem } from "@features/transactions/types";
import { LoadingError } from "@components/layout/LoadingError";

// ─── Reusable label/value row used inside section cards ──────────────────────
function InfoRow({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "flex-start",
        py: 0.75,
        borderBottom: "1px solid",
        borderColor: "divider",
        gap: 1,
        "&:last-child": { borderBottom: "none" },
      }}
    >
      <Typography
        variant="body2"
        sx={{
          color: "text.secondary",
          fontWeight: 500,
          flexShrink: 0,
          minWidth: 140,
        }}
      >
        {label}
      </Typography>
      <Typography
        variant="body2"
        sx={{
          color: "text.primary",
          textAlign: "right",
          wordBreak: "break-word",
        }}
      >
        {value ?? "N/A"}
      </Typography>
    </Box>
  );
}

// ─── Titled bordered section card ─────────────────────────────────────────────
function SectionCard({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <Box
      sx={{
        border: "1px solid",
        borderColor: "divider",
        borderRadius: 2,
        overflow: "hidden",
        height: "100%",
      }}
    >
      <Box
        sx={{
          px: 2,
          py: 0.75,
          bgcolor: "action.hover",
          borderBottom: "1px solid",
          borderColor: "divider",
        }}
      >
        <Typography
          variant="caption"
          sx={{
            fontWeight: 700,
            letterSpacing: 0.8,
            textTransform: "uppercase",
            color: "text.secondary",
          }}
        >
          {title}
        </Typography>
      </Box>
      <Box sx={{ px: 2, py: 1 }}>{children}</Box>
    </Box>
  );
}

// ─── Main component ───────────────────────────────────────────────────────────
function ViewInfo() {
  const { id } = useParams<{ id: string }>();

  // ─── State ─────────────────────────────────────────────────────────────────
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [asset, setAsset] = useState<EditAssetView | null>(null);
  const [history, setHistory] = useState<TransactionResponse[] | null>(null);
  const [sparePartList, setSparePartList] = useState<
    AvailableAssetItem[] | null
  >(null);
  const [softwareList, setSoftwareList] = useState<string[]>();

  // ─── Data Fetching ──────────────────────────────────────────────────────────

  /** Fetches asset details and transaction history independently. */
  const assetFeature = async () => {
    try {
      setLoading(true);
      const assetData = await getAssetInfo(id!);
      setAsset(assetData);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load assets.");
    }
    try {
      setLoading(true);
      const historyData = await getHistoryTransactions(id!);
      setHistory(historyData);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load history.");
    } finally {
      setLoading(false);
    }
  };

  /** Fetches spare parts list and installed software list. */
  const transactionFeature = async () => {
    try {
      setLoading(true);

      // Spare parts
      const comingData1 = await getTransaction({
        Type: "get_list_of_parts",
        AssetId: id!,
      });
      if (Array.isArray(comingData1) && comingData1.length > 0) {
        setSparePartList(comingData1);
      }

      // Installed software
      const comingData2 = await getTransaction({
        Type: "get_list_of_softwares",
        AssetId: id!,
      });
      if (Array.isArray(comingData2) && comingData2.length > 0) {
        setSoftwareList(comingData2);
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Trigger all fetches when asset id changes
  useEffect(() => {
    assetFeature();
    if (id) transactionFeature();
  }, [id]);

  // ─── Guard ──────────────────────────────────────────────────────────────────
  if (!asset) {
    return (
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Alert severity="warning" sx={{ mt: 2 }}>
          No asset found.
        </Alert>
      </Container>
    );
  }

  const isSparePart = asset.Type === "SparePart";

  // ─── Render ─────────────────────────────────────────────────────────────────
  return (
    <LoadingError loading={loading} error={error}>
      <Container maxWidth="xl" sx={{ py: 2.5 }}>
        {/* ── Page Header ── */}
        <Box
          sx={{
            display: "flex",
            alignItems: "flex-start",
            justifyContent: "space-between",
            mb: 2,
            flexWrap: "wrap",
            gap: 1,
          }}
        >
          <Box>
            <Typography variant="h5" sx={{ fontWeight: 700, lineHeight: 1.3 }}>
              {asset.Name || "Asset Information"}
            </Typography>
            <Box sx={{ display: "flex", gap: 0.75, mt: 0.5, flexWrap: "wrap" }}>
              <Chip
                label={asset.AssetId}
                size="small"
                variant="outlined"
                sx={{ fontFamily: "monospace", fontSize: "0.7rem" }}
              />
              <Chip label={asset.Type} size="small" color="primary" />
            </Box>
          </Box>
          {/* Edit button moved to header for quick access */}
          <Button
            onClick={() =>
              window.open(`/EditAssetForm/${id}`, "_blank", "noreferrer")
            }
            variant="contained"
            size="small"
            sx={{ minWidth: 100, alignSelf: "center" }}
          >
            Edit Info
          </Button>
        </Box>

        {/* ── Asset Detail Card ── */}
        <Card variant="outlined" sx={{ mb: 2.5 }}>
          <CardContent sx={{ p: 2, "&:last-child": { pb: 2 } }}>
            <Grid container spacing={2}>
              {/* Basic Info */}
              <Grid size={{ xs: 12, sm: 6, md: isSparePart ? 4 : 3 }}>
                <SectionCard title="Basic Information">
                  <InfoRow label="Date of Purchase" value={asset.DoP} />
                  <InfoRow
                    label="Finance Asset Code"
                    value={asset.FinanceAssetCode}
                  />
                  <InfoRow label="Warranty (Months)" value={asset.Warranty} />
                  <InfoRow
                    label="Manufacturer SN"
                    value={asset.ManufactureSN}
                  />
                  <InfoRow
                    label="Brand New"
                    value={asset.Brandnew ? "Yes" : "No"}
                  />
                  <InfoRow label="Cost" value={asset.Cost} />
                </SectionCard>
              </Grid>

              {/* Company & Location */}
              <Grid size={{ xs: 12, sm: 6, md: isSparePart ? 4 : 3 }}>
                <SectionCard title="Company & Location">
                  <InfoRow label="Company" value={asset.CName} />
                  <InfoRow label="Location" value={asset.LName} />
                </SectionCard>
              </Grid>

              {/* Technical Specs — non-SparePart only */}
              {!isSparePart && (
                <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                  <SectionCard title="Technical Specifications">
                    <InfoRow label="OS" value={asset.OS} />
                    <InfoRow label="Processor" value={asset.Processor} />
                    <InfoRow label="RAM Size" value={asset.RAMSize} />
                    <InfoRow label="RAM Type" value={asset.RAMType} />
                    <InfoRow label="Make" value={asset.Make} />
                    <InfoRow label="Model" value={asset.Model} />
                  </SectionCard>
                </Grid>
              )}

              {/* Storage & Display — non-SparePart only */}
              {!isSparePart && (
                <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                  <SectionCard title="Storage & Display">
                    <InfoRow label="HDD" value={asset.HDD} />
                    <InfoRow label="SSD" value={asset.SSD} />
                    <InfoRow label="Display" value={asset.Display} />
                  </SectionCard>
                </Grid>
              )}

              {/* Additional Info */}
              <Grid size={{ xs: 12, sm: 6, md: isSparePart ? 4 : 6 }}>
                <SectionCard title="Additional Information">
                  <InfoRow label="IP Address" value={asset.IPAddress} />
                  <InfoRow label="Note" value={asset.Note} />
                </SectionCard>
              </Grid>

              {/* Installed Software — non-SparePart only, chips layout */}
              {!isSparePart && softwareList && softwareList.length > 0 && (
                <Grid size={{ xs: 12, sm: 6, md: 6 }}>
                  <SectionCard title="Installed Software">
                    <Box
                      sx={{
                        display: "flex",
                        flexWrap: "wrap",
                        gap: 0.75,
                        pt: 0.5,
                      }}
                    >
                      {softwareList.map((software, index) => (
                        <Chip
                          key={index}
                          label={software}
                          size="small"
                          variant="outlined"
                        />
                      ))}
                    </Box>
                  </SectionCard>
                </Grid>
              )}
            </Grid>
          </CardContent>
        </Card>

        {/* ── Transaction History ── */}
        <Box sx={{ mb: 2.5 }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              mb: 1,
            }}
          >
            <Typography variant="h6" sx={{ fontWeight: 600 }}>
              Transaction History
            </Typography>
            {/* Transfer button placed next to section title */}
            <Button
              onClick={() =>
                window.open(`/TransactionPage/${id}`, "_blank", "noreferrer")
              }
              variant="contained"
              size="small"
              sx={{ minWidth: 100 }}
            >
              Transfer
            </Button>
          </Box>
          <Card variant="outlined">
            <CardContent sx={{ p: 0, "&:last-child": { pb: 0 } }}>
              {history && history.length > 0 ? (
                <TableContainer
                  component={Paper}
                  elevation={0}
                  sx={{ maxHeight: 360 }}
                >
                  <Table stickyHeader size="small">
                    <TableHead>
                      <TableRow>
                        {[
                          "Date",
                          "Edited By",
                          "Type",
                          "From",
                          "To",
                          "Associated Asset",
                          "Note",
                        ].map((h) => (
                          <TableCell
                            key={h}
                            sx={{
                              fontWeight: 700,
                              fontSize: "0.75rem",
                              bgcolor: "action.hover",
                              py: 1,
                            }}
                          >
                            {h}
                          </TableCell>
                        ))}
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {history.map((transaction, index) => (
                        <TableRow
                          key={index}
                          hover
                          sx={{ "&:last-child td": { border: 0 } }}
                        >
                          <TableCell
                            sx={{ fontSize: "0.8rem", whiteSpace: "nowrap" }}
                          >
                            {transaction.Time
                              ? new Date(transaction.Time).toLocaleString()
                              : "N/A"}
                          </TableCell>
                          <TableCell sx={{ fontSize: "0.8rem" }}>
                            {transaction.EditedUserFullName}
                          </TableCell>
                          <TableCell sx={{ fontSize: "0.8rem" }}>
                            <Chip
                              label={transaction.Type}
                              size="small"
                              variant="outlined"
                              sx={{ fontSize: "0.7rem" }}
                            />
                          </TableCell>
                          <TableCell sx={{ fontSize: "0.8rem" }}>
                            {transaction.FromName || "N/A"}
                          </TableCell>
                          <TableCell sx={{ fontSize: "0.8rem" }}>
                            {transaction.ToName || "N/A"}
                          </TableCell>
                          <TableCell sx={{ fontSize: "0.8rem" }}>
                            {transaction.RelatedAssetName || "N/A"}
                          </TableCell>
                          <TableCell
                            sx={{
                              fontSize: "0.8rem",
                              maxWidth: 160,
                              overflow: "hidden",
                              textOverflow: "ellipsis",
                              whiteSpace: "nowrap",
                            }}
                          >
                            {transaction.Note || "N/A"}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              ) : (
                <Box sx={{ p: 2 }}>
                  <Alert severity="info">No transaction history found.</Alert>
                </Box>
              )}
            </CardContent>
          </Card>
        </Box>

        {/* ── Spare Parts List ── */}
        <Box>
          <Typography variant="h6" sx={{ fontWeight: 600, mb: 1 }}>
            Spare Parts List
          </Typography>
          <Card variant="outlined">
            <CardContent sx={{ p: 0, "&:last-child": { pb: 0 } }}>
              {sparePartList && sparePartList.length > 0 ? (
                <TableContainer
                  component={Paper}
                  elevation={0}
                  sx={{ maxHeight: 360 }}
                >
                  <Table stickyHeader size="small">
                    <TableHead>
                      <TableRow>
                        {["Spare Part ID", "Spare Part Name", "Actions"].map(
                          (h) => (
                            <TableCell
                              key={h}
                              sx={{
                                fontWeight: 700,
                                fontSize: "0.75rem",
                                bgcolor: "action.hover",
                                py: 1,
                              }}
                            >
                              {h}
                            </TableCell>
                          ),
                        )}
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {sparePartList.map((sparePart, index) => (
                        <TableRow
                          key={index}
                          hover
                          sx={{ "&:last-child td": { border: 0 } }}
                        >
                          <TableCell
                            sx={{ fontSize: "0.8rem", fontFamily: "monospace" }}
                          >
                            {sparePart.AssetId}
                          </TableCell>
                          <TableCell sx={{ fontSize: "0.8rem" }}>
                            {sparePart.AssetName}
                          </TableCell>
                          <TableCell>
                            <Box sx={{ display: "flex", gap: 0.75 }}>
                              {/* View — opens spare part detail in new tab */}
                              <Button
                                onClick={() =>
                                  window.open(
                                    `/ViewInfo/${sparePart.AssetId}`,
                                    "_blank",
                                    "noreferrer",
                                  )
                                }
                                size="small"
                                variant="outlined"
                                sx={{ minWidth: 60, py: 0.25 }}
                              >
                                View
                              </Button>
                              {/* Transfer — opens transfer form for spare part in new tab */}
                              <Button
                                onClick={() =>
                                  window.open(
                                    `/TransactionPage/${sparePart.AssetId}`,
                                    "_blank",
                                    "noreferrer",
                                  )
                                }
                                size="small"
                                variant="outlined"
                                sx={{ minWidth: 70, py: 0.25 }}
                              >
                                Transfer
                              </Button>
                            </Box>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              ) : (
                <Box sx={{ p: 2 }}>
                  <Alert severity="info">
                    No spare parts found for this asset.
                  </Alert>
                </Box>
              )}
            </CardContent>
          </Card>
        </Box>
      </Container>
    </LoadingError>
  );
}

export default ViewInfo;
