import * as React from "react";

import theme from "./theme";

import { BrowserRouter, Routes, Route } from "react-router-dom";

import { Container, CssBaseline, Toolbar } from "@mui/material";
import { ThemeProvider } from "@mui/material/styles";

import { ProtectedRoute } from "@components/layout/ProtectedRoute";
import ResponsiveAppBar from "@features/ResponsiveAppBar";
import { AuthProvider, useAuth } from "@contexts/AuthContext";

import "@styles/App.css";
import { AddAssetForm } from "@features/assets/pages/AddAssetForm";
import { AddDesktop } from "@features/assets/pages/AddDesktop";
import { AddLaptop } from "@features/assets/pages/AddLaptop";
import { AddServer } from "@features/assets/pages/AddServer";
import EditAssetForm from "@features/assets/pages/EditAssetForm";
import EditComputerForm from "@features/assets/pages/EditComputerForm";
import ViewAsset from "@features/assets/pages/ViewAsset";
import ViewInfo from "@features/assets/pages/ViewInfo";
import LoginPage from "@features/login/LoginPage";
import AddMasterData from "@features/masterdata/pages/AddMasterData";
import AddUser from "@features/masterdata/pages/AddUser";
import EditUser from "@features/masterdata/pages/EditUser";
import { AddSoftwares } from "@features/softwares/pages/AddSoftwares";
import DestroyAsset from "@features/transactions/components/DestroyAsset";
import MoveLocation from "@features/transactions/components/MoveLocation";
import TransactionPage from "@features/transactions/pages/TransactionPage";
import UserTransfer from "@features/masterdata/pages/UserTransfer";
import UserResign from "@features/masterdata/pages/UserResign";

function AppContent() {
  const { isLoggedIn } = useAuth();

  return (
    <React.Fragment>
      <BrowserRouter>
        {/*wheather the user logged or not, the AppBar wouldn't be displayed on the login page*/}
        {isLoggedIn && location.pathname !== "/" && <ResponsiveAppBar />}

        <Toolbar />

        <Container maxWidth={false} sx={{ mt: 2 }}>
          <Routes>
            <Route path="/" element={<LoginPage />} />
            <Route
              path="/AddMasterData"
              element={
                <ProtectedRoute>
                  <AddMasterData />
                </ProtectedRoute>
              }
            />
            <Route
              path="/AddUser"
              element={
                <ProtectedRoute>
                  <AddUser />
                </ProtectedRoute>
              }
            />
            <Route
              path="/ResignUser/:id"
              element={
                <ProtectedRoute>
                  <UserResign />
                </ProtectedRoute>
              }
            />
            <Route
              path="/UserTransfer/:id"
              element={
                <ProtectedRoute>
                  <UserTransfer />
                </ProtectedRoute>
              }
            />
            <Route
              path="/EditUser/:id"
              element={
                <ProtectedRoute>
                  <EditUser />
                </ProtectedRoute>
              }
            />
            <Route
              path="/EditComputerForm/:id"
              element={
                <ProtectedRoute>
                  <EditComputerForm />
                </ProtectedRoute>
              }
            />
            <Route
              path="/EditAssetForm/:id"
              element={
                <ProtectedRoute>
                  <EditAssetForm />
                </ProtectedRoute>
              }
            />
            <Route
              path="/ViewInfo/:id"
              element={
                <ProtectedRoute>
                  <ViewInfo />
                </ProtectedRoute>
              }
            />
            <Route
              path="/viewAsset"
              element={
                <ProtectedRoute>
                  <ViewAsset />
                </ProtectedRoute>
              }
            />
            <Route
              path="/AddAssetForm"
              element={
                <ProtectedRoute>
                  <AddAssetForm />
                </ProtectedRoute>
              }
            />
            <Route
              path="/AddDesktop/:id"
              element={
                <ProtectedRoute>
                  <AddDesktop />
                </ProtectedRoute>
              }
            />
            <Route
              path="/AddLaptop/:id"
              element={
                <ProtectedRoute>
                  <AddLaptop />
                </ProtectedRoute>
              }
            />
            <Route
              path="/AddServer/:id"
              element={
                <ProtectedRoute>
                  <AddServer />
                </ProtectedRoute>
              }
            />
            <Route
              path="/AddSoftwares/:id"
              element={
                <ProtectedRoute>
                  <AddSoftwares />
                </ProtectedRoute>
              }
            />
            <Route
              path="/TransactionPage/:id"
              element={
                <ProtectedRoute>
                  <TransactionPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/assets/:id"
              element={
                <ProtectedRoute>
                  <ViewInfo />
                </ProtectedRoute>
              }
            />
            <Route
              path="/assets/:id/move"
              element={
                <ProtectedRoute>
                  <MoveLocation />
                </ProtectedRoute>
              }
            />
            <Route
              path="/assets/:id/destroy"
              element={
                <ProtectedRoute>
                  <DestroyAsset />
                </ProtectedRoute>
              }
            />
          </Routes>
        </Container>
      </BrowserRouter>
    </React.Fragment>
  );
}

function App() {
  return (
    <React.Fragment>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <AuthProvider>
          <AppContent />
        </AuthProvider>
      </ThemeProvider>
    </React.Fragment>
  );
}

export default App;
