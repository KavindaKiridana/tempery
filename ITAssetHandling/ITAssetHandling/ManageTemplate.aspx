<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageTemplate.aspx.cs" Inherits="ITAssetHandling.ManageTemplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
        /* Existing styles unchanged */
        /*.template-grid {
            font-size: 0.875rem;
        }

            .template-grid th {
                font-size: 0.8rem;
                font-weight: 600;
                padding: 0.5rem 0.75rem;
                white-space: nowrap;
            }

            .template-grid td {
                padding: 0.5rem 0.75rem;
                vertical-align: middle;
            }

        .form-control-sm, .form-select-sm {
            font-size: 0.875rem;
            padding: 0.375rem 0.75rem;
            width: 100%;*/ /* Ensure consistent width */
        /*max-width: 100%;*/ /* Prevent exceeding container */
        /*}

        .btn-sm {
            font-size: 0.8rem;
            padding: 0.25rem 0.5rem;
            width: 100%;*/ /* Match width of form elements */
        /*max-width: 100%;*/ /* Prevent overflow */
        /*}

        .btn-custom {
            width: 100%;*/ /* Ensure buttons match form element width */
        /*max-width: 100%;*/ /* Prevent overflow */
        /*}

        .card-header h5 {
            font-size: 1.1rem;
        }

        .table-responsive {
            border-radius: 0.375rem;
        }

        @media (max-width: 991.98px) {
            .mobile-scroll {
                overflow-x: auto;
            }
        }

        .authorizer-grid {
            font-size: 0.8rem;
            width: 100%;*/ /* Match width of other form elements */
        /*max-width: 100%;*/ /* Prevent overflow */
        /*}

            .authorizer-grid th,
            .authorizer-grid td {
                padding: 0.4rem 0.5rem;
            }

        .authorizer-grid-container {
            width: 100%;*/ /* Ensure container matches form elements */
        /*max-width: 100%;*/ /* Prevent overflow */
        /*}*/

        /* Existing styles unchanged */
        .template-grid {
            font-size: 0.875rem;
        }

            .template-grid th {
                font-size: 0.8rem;
                font-weight: 600;
                padding: 0.5rem 0.75rem;
                white-space: nowrap;
            }

            .template-grid td {
                padding: 0.5rem 0.75rem;
                vertical-align: middle;
            }

        .form-control-sm, .form-select-sm {
            font-size: 0.875rem;
            padding: 0.375rem 0.75rem;
            width: 100%; /* Ensure consistent width */
            max-width: 100%; /* Prevent exceeding container */
        }

        .btn-sm {
            font-size: 0.8rem;
            padding: 0.25rem 0.5rem;
            width: 100%; /* Match width of form elements */
            max-width: 100%; /* Prevent overflow */
        }

        .btn-custom {
            font-size: 0.875rem; /* Match form-control-sm font size */
            padding: 0.375rem 0.75rem; /* Match form-control-sm padding */
            width: 100% !important; /* Ensure consistent width with form elements */
            max-width: 100%; /* Prevent overflow */
            box-sizing: border-box; /* Ensure padding is included in width */
        }

        .card-header h5 {
            font-size: 1.1rem;
        }

        .table-responsive {
            border-radius: 0.375rem;
        }

        @media (max-width: 991.98px) {
            .mobile-scroll {
                overflow-x: auto;
            }
        }

        .authorizer-grid {
            font-size: 0.8rem;
            width: 100%; /* Match width of other form elements */
            max-width: 100%; /* Prevent overflow */
        }

            .authorizer-grid th,
            .authorizer-grid td {
                padding: 0.4rem 0.5rem;
            }

        .authorizer-grid-container {
            width: 100%; /* Ensure container matches form elements */
            max-width: 100%; /* Prevent overflow */
        }
    </style>
    <div class="container-fluid py-4">
        <div class="row g-4">
            <!-- Existing Templates Section -->
            <div class="col-12">
                <div class="row g-4">
                    <div class="col-12 col-lg-9">
                        <div class="card shadow-sm">
                            <div class="card-header bg-primary text-white">
                                <h5 class="mb-0"><i class="fa fa-list me-2"></i>Existing Templates</h5>
                            </div>
                            <div class="card-body p-3">
                                <div class="table-responsive mobile-scroll">
                                    <asp:GridView ID="GridView1" runat="server"
                                        AllowPaging="True" AutoGenerateColumns="False"
                                        DataKeyNames="FlexibleTemplateId"
                                        OnRowEditing="GridView1_RowEditing"
                                        OnRowUpdating="GridView1_RowUpdating"
                                        OnRowCancelingEdit="GridView1_RowCancelingEdit"
                                        OnRowDataBound="GridView1_RowDataBound"
                                        CssClass="table table-hover table-sm template-grid"
                                        GridLines="None">
                                        <Columns>
                                            <asp:CommandField ShowEditButton="True"
                                                EditText="<i class='fa fa-edit'></i>"
                                                UpdateText="<i class='fa fa-refresh'></i>"
                                                CancelText="<i class='fa fa-times' style='font-size:16px'></i>"
                                                ControlStyle-CssClass="btn btn-sm btn-outline-primary me-1"
                                                HeaderStyle-Width="100px"
                                                HeaderText="Actions" />
                                            <asp:BoundField DataField="CompanyName" HeaderText="Company Name" ReadOnly="True"
                                                HeaderStyle-Width="60%" ItemStyle-Width="60%" />
                                            <asp:CheckBoxField DataField="IsActive" HeaderText="Status"
                                                HeaderStyle-Width="20%" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <PagerStyle CssClass="table-pager" />
                                        <HeaderStyle CssClass="table-dark" />
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Add New Template Section -->
                    <div class="col-12 col-lg-3">
                        <div class="card shadow-sm h-100">
                            <div class="card-header bg-success text-white">
                                <h5 class="mb-0"><i class="fa fa-plus me-2"></i>Add New Template</h5>
                            </div>
                            <div class="card-body p-3">
                                <asp:Label ID="lblMessage" runat="server" CssClass="mb-3 d-block small"></asp:Label>

                                <div class="mb-3">
                                    <label for="<%= ddlCompany.ClientID %>" class="form-label small fw-semibold">Select Company</label>
                                    <asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                                </div>

                                <div class="mb-3">
                                    <label for="<%= txtPosition.ClientID %>" class="form-label small fw-semibold">Position</label>
                                    <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control form-control-sm"
                                        placeholder="Enter position title"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <label for="<%= ddlUsers.ClientID %>" class="form-label small fw-semibold">Select User</label>
                                    <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-select form-select-sm">
                                        <asp:ListItem Text="--Select User--" Value="" />
                                    </asp:DropDownList>
                                </div>

                                <div class="d-grid mb-3">
                                    <asp:Button ID="btnAdd" runat="server" Text="Add Authorizer"
                                        CssClass="btn btn-outline-primary btn-sm" OnClick="btnAdd_Click" />
                                </div>

                                <!-- Authorizers List -->
                                <div class="mb-3">
                                    <label class="form-label small fw-semibold">Authorizers List</label>
                                    <div class="border rounded p-2 authorizer-grid-container" style="min-height: 120px; max-height: 200px; overflow-y: auto;">
                                        <asp:GridView
                                            ID="GridView3"
                                            runat="server"
                                            AutoGenerateColumns="false"
                                            CssClass="table table-sm table-borderless authorizer-grid"
                                            ShowHeader="true"
                                            EmptyDataText="No authorizers added yet">
                                            <Columns>
                                                <asp:BoundField DataField="Position" HeaderText="Position"
                                                    ItemStyle-Width="35%" HeaderStyle-Width="35%" />
                                                <asp:BoundField DataField="UserId" HeaderText="User ID" Visible="false" />
                                                <asp:BoundField DataField="UserName" HeaderText="User"
                                                    ItemStyle-Width="45%" HeaderStyle-Width="45%" />
                                                <asp:TemplateField HeaderText="Action"
                                                    ItemStyle-Width="20%" HeaderStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnRemove" runat="server" Text="×"
                                                            CssClass="btn btn-sm btn-outline-danger px-2 py-0"
                                                            CommandName="Remove" CommandArgument='<%# Container.DataItemIndex %>'
                                                            OnClick="btnRemove_Click"
                                                            ToolTip="Remove authorizer" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle CssClass="table-light small fw-semibold" />
                                            <EmptyDataRowStyle CssClass="text-muted small text-center" />
                                        </asp:GridView>
                                    </div>
                                </div>

                                <div class="d-grid mt-auto">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Add Template"
                                        CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
