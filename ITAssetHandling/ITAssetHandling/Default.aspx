<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ITAssetHandling._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .vertical-center {
            min-height: 100vh;
            display: flex;
            align-items: center;
        }
    </style>
    <main>
        <div class="container-fluid d-flex align-items-center justify-content-center min-vh-100">
            <div class="container py-5">
                <!-- <div class="container py-5">  -->
                <div class="row justify-content-center">
                    <div class="col-12 col-md-6 col-lg-4">
                        <div class="card shadow-sm border-light">
                            <!-- Header -->
                            <div class="card-header bg-light text-primary p-3">
                                <h5 class="mb-0" style="text-align: center;">IT Asset Request - Login</h5>
                            </div>

                            <!-- Body -->
                            <div class="card-body p-4">
                                <asp:Label ID="lblAlert" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

                                <div class="mb-3">
                                    <label for="<%=txtUserName.ClientID %>" class="form-label fw-semibold">User Name:</label>
                                    <asp:TextBox runat="server" ID="txtUserName" CssClass="form-control rounded-2" placeholder="Enter your username" />
                                </div>

                                <div class="mb-3">
                                    <label for="<%=txtPassword.ClientID %>" class="form-label fw-semibold">Password:</label>
                                    <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control rounded-2" placeholder="Enter your password" />
                                </div>

                                <div class="d-flex justify-content-end mt-3">
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary rounded-2" Text="Login" OnClick="btnSubmit_Click" />
                                </div>
                            </div>

                            <!-- Footer -->
                            <div class="card-footer bg-light text-center py-2">
                                <small class="text-muted">Renuks Group IT © 2025</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    </main>
</asp:Content>
