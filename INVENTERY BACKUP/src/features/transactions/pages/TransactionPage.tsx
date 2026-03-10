import * as React from "react";
import { useCallback, useState } from "react";

import { useParams } from "react-router-dom";

import { Box, CircularProgress, Tab, Tabs } from "@mui/material";

import MoveLocation from "@features/transactions/components/MoveLocation";
import TabAddComplain from "@features/transactions/components/TabAddComplain";
import { TabAssigntoUser } from "@features/transactions/components/TabAssigntoUser";
import TabAttachtoAsset from "@features/transactions/components/TabAttachtoAsset";
import TabReturnFromRepair from "@features/transactions/components/TabReturnFromRepair";
import TabReturnFromUser from "@features/transactions/components/TabReturnFromUser";
import TabSentToRepair from "@features/transactions/components/TabSentToRepair";

import DestroyAsset from "../components/DestroyAsset";
import TabReturnFromAsset from "../components/TabReturnFromAsset";
import { getTransaction } from "../services/tracsactionService";

import type { TransactionPageData } from "../types";
import ITObservation from "../components/TabITObservation";

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function CustomTabPanel(props: TabPanelProps) {
  const { children, value, index } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
    >
      {value === index && <Box sx={{ p: { xs: 0, sm: 0 } }}>{children}</Box>}
    </div>
  );
}

function a11yProps(index: number) {
  return {
    id: `simple-tab-${index}`,
    "aria-controls": `simple-tabpanel-${index}`,
  };
}

function TransactionPage() {
  const [value, setValue] = useState(0);
  const { id } = useParams();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [transactionPageData, setTransactionPageData] =
    useState<TransactionPageData>();

  const loadData = async () => {
    try {
      setLoading(true);
      const sentData = {
        Type: "transaction_page",
        AssetId: id!,
      };
      const comingData = await getTransaction(sentData);
      // Check if the response indicates an error
      if (comingData.Ok === false || comingData.ExceptionMessage) {
        setError(comingData.ExceptionMessage);
        return;
      }
      setTransactionPageData(comingData);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => {
    loadData();
  }, [id]);

  const refreshData = useCallback(async () => {
    await loadData();
  }, []);

  const handleChange = (_: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  return (
    <Box
      sx={{
        height: "100%",
        display: "flex",
        flexDirection: "column",
        bgcolor: "background.default",
      }}
    >
      {loading ? (
        <CircularProgress size={24} />
      ) : error ? (
        <div>Error: {error}</div>
      ) : !transactionPageData?.IsActiveAsset ? (
        <Box sx={{ p: 3, textAlign: "center" }}>
          <div>
            <h3>This asset is inactive and cannot perform transactions.</h3>
          </div>
        </Box>
      ) : (
        <>
          <Box
            sx={{
              borderBottom: 1,
              borderColor: "divider",
              bgcolor: "background.paper",
            }}
          >
            <Tabs
              value={value}
              onChange={handleChange}
              aria-label="transaction tabs"
              variant="scrollable"
              scrollButtons="auto"
              allowScrollButtonsMobile
            >
              <Tab label="Assign to User" {...a11yProps(0)} />
              <Tab label="Attach Spare Part" {...a11yProps(1)} />
              <Tab label="Remove Spare Part" {...a11yProps(2)} />
              <Tab label="Location Transfer" {...a11yProps(3)} />
              <Tab label="Add Complain" {...a11yProps(4)} />
              <Tab label="Add ITObservation" {...a11yProps(5)} />
              {transactionPageData?.HasOngoingRepair ? (
                <Tab label="Return from Repair" {...a11yProps(6)} />
              ) : (
                <Tab label="Sent to Repair" {...a11yProps(6)} />
              )}
              <Tab label="Destroy Asset" {...a11yProps(7)} />
            </Tabs>
          </Box>
          <CustomTabPanel value={value} index={0}>
            {transactionPageData?.HasExistingUser ? (
              <TabReturnFromUser id={id} onSuccess={refreshData} />
            ) : (
              <TabAssigntoUser id={id} onSuccess={refreshData} />
            )}
          </CustomTabPanel>
          <CustomTabPanel value={value} index={1}>
            <TabAttachtoAsset id={id} onSuccess={refreshData} />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={2}>
            <TabReturnFromAsset id={id} onSuccess={refreshData} />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={3}>
            <MoveLocation id={id} onSuccess={refreshData} />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={4}>
            <TabAddComplain id={id} />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={5}>
            <ITObservation id={id} onSuccess={refreshData} />
          </CustomTabPanel>
          {transactionPageData.HasOngoingRepair ? (
            <CustomTabPanel value={value} index={6}>
              <TabReturnFromRepair id={id} onSuccess={refreshData} />
            </CustomTabPanel>
          ) : (
            <CustomTabPanel value={value} index={6}>
              <TabSentToRepair id={id} onSuccess={refreshData} />
            </CustomTabPanel>
          )}
          <CustomTabPanel value={value} index={7}>
            <DestroyAsset id={id} />
          </CustomTabPanel>
        </>
      )}
    </Box>
  );
}

export default TransactionPage;
