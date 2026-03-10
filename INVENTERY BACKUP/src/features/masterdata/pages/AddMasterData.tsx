import { useState } from "react";

import { Box, Paper, Tab, Tabs } from "@mui/material";

import AddDisplay from "../components/AddDisplay";
import AddHDD from "../components/AddHDD";
import AddModel from "../components/AddModel";
import AddOS from "../components/AddOS";
import AddProcessor from "../components/AddProcessor";
import AddRAMSize from "../components/AddRAMSize";
import AddRAMType from "../components/AddRAMType";
import AddSoftware from "../components/AddSoftware";
import AddSSD from "../components/AddSSD";
import ViewUsers from "../components/viewUsers";
import AddType from "../components/AddType";

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
      style={{ height: "100%", overflow: "hidden" }}
    >
      {value === index && <Box sx={{ height: "100%", p: 0 }}>{children}</Box>}
    </div>
  );
}

function a11yProps(index: number) {
  return {
    id: `simple-tab-${index}`,
    "aria-controls": `simple-tabpanel-${index}`,
  };
}

function AddMasterData() {
  const [value, setValue] = useState(0);

  const handleChange = (_event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  return (
    <Paper
      elevation={0}
      sx={{
        height: "100%",
        display: "flex",
        flexDirection: "column",
        bgcolor: "background.default",
      }}
    >
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
          variant="scrollable"
          scrollButtons="auto"
          allowScrollButtonsMobile
        >
          <Tab label="User" {...a11yProps(0)} />
          <Tab label="Type" {...a11yProps(1)} />
          <Tab label="OS" {...a11yProps(2)} />
          <Tab label="Processor" {...a11yProps(3)} />
          <Tab label="RAM Size" {...a11yProps(4)} />
          <Tab label="RAM Type" {...a11yProps(5)} />
          <Tab label="Software" {...a11yProps(6)} />
          <Tab label="SSD" {...a11yProps(7)} />
          <Tab label="Model" {...a11yProps(8)} />
          <Tab label="Display" {...a11yProps(9)} />
          <Tab label="HDD" {...a11yProps(10)} />
        </Tabs>
      </Box>
      <Box sx={{ flexGrow: 1, overflow: "hidden", pt: 1 }}>
        <CustomTabPanel value={value} index={0}>
          <ViewUsers />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={1}>
          <AddType />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={2}>
          <AddOS />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={3}>
          <AddProcessor />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={4}>
          <AddRAMSize />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={5}>
          <AddRAMType />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={6}>
          <AddSoftware />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={7}>
          <AddSSD />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={8}>
          <AddModel />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={9}>
          <AddDisplay />
        </CustomTabPanel>
        <CustomTabPanel value={value} index={10}>
          <AddHDD />
        </CustomTabPanel>
      </Box>
    </Paper>
  );
}

export default AddMasterData;
