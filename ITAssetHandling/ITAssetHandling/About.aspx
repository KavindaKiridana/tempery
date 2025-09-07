<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="ITAssetHandling.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <div class="container-fluid py-4">
            <div class="row justify-content-center">
                <div class="col-12 col-lg-12">
                    <div class="card shadow-sm">
                        <div class="card-header bg-primary text-white">
                            <h2 class="mb-0 h4">CAPEX Form</h2>
                        </div>
                        <div class="card-body">
                            <!-- Header Section -->
                            <div class="row mb-4">
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Date</label>
                                    <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Requested By</label>
                                    <asp:TextBox ID="txtRequestedBy" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                            </div>

                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <!-- Company and Department -->
                                    <div class="row mb-4">
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Invoice Company</label>
                                            <asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-select">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Allocation Department</label>
                                            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <!-- Reason and Division Head -->
                                    <div class="row mb-4">
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Reason</label>
                                            <asp:DropDownList ID="ddlReason" runat="server" CssClass="form-select">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Division Head</label>
                                            <asp:DropDownList ID="ddlHead" runat="server" CssClass="form-select">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <!-- Requisition Details -->
                                    <div class="row mb-4">
                                        <div class="col-12 mb-3">
                                            <h5 class="border-bottom pb-2">Requisition Details</h5>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Used by / To whom</label>
                                            <asp:DropDownList ID="ddlUsedByToWhom" runat="server" CssClass="form-select">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Budgeted</label>
                                            <asp:DropDownList ID="ddlBudgeted" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="Yes" Value="true" />
                                                <asp:ListItem Text="No" Value="false" Selected="true" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <!-- Existing Item Details -->
                                    <div class="row mb-4">
                                        <div class="col-12 mb-3">
                                            <h5 class="border-bottom pb-2">Existing Item Details (If the item is not a new/ new project)</h5>
                                        </div>
                                        <div class="col-md-6 col-lg-3 mb-3">
                                            <label class="form-label">Date of Purchase</label>
                                            <asp:TextBox ID="txtDateOfPurchase" runat="server" TextMode="Date" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6 col-lg-3 mb-3">
                                            <label class="form-label">Warranty</label>
                                            <asp:TextBox ID="txtWarranty" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6 col-lg-3 mb-3">
                                            <label class="form-label">Make</label>
                                            <asp:TextBox ID="txtMake" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6 col-lg-3 mb-3">
                                            <label class="form-label">Model</label>
                                            <asp:TextBox ID="txtModel" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">Serial Number</label>
                                            <asp:TextBox ID="txtSerialNo" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>

                                    <!-- Costing & Configuration -->
                                    <div class="row mb-4">
                                        <div class="col-12 mb-3">
                                            <h5 class="border-bottom pb-2">Costing & Configuration (If repair only quotation will be attached)</h5>
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Quotation</label>
                                            <asp:DropDownList ID="ddlQuatation" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="-- Select --" Value="null" Selected="true" />
                                                <asp:ListItem Text="Yes" Value="true" />
                                                <asp:ListItem Text="No" Value="false" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Configuration Evaluation</label>
                                            <asp:DropDownList ID="ddlConfigurationEvalation" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="-- Select --" Value="null" Selected="true" />
                                                <asp:ListItem Text="Yes" Value="true" />
                                                <asp:ListItem Text="No" Value="false" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Cost Breakdown</label>
                                            <asp:DropDownList ID="ddlCostBreakdown" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="-- Select --" Value="null" Selected="true" />
                                                <asp:ListItem Text="Yes" Value="true" />
                                                <asp:ListItem Text="No" Value="false" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <!-- Cost Summary Table -->
                                    <div class="row mb-4">
                                        <!-- start -->
                                        <div class="col-12 mb-3">
                                            <h5 class="border-bottom pb-2">Cost Summary & Recommended Supplier</h5>
                                        </div>
                                        <asp:Label ID="lblPaymnetRecord" runat="server"></asp:Label>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Supplier</label>
                                            <asp:DropDownList ID="ddlSupply" runat="server" CssClass="form-select">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Quantity</label>
                                            <asp:TextBox ID="txtQty" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Unit Price</label>
                                            <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Currency</label>
                                            <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="USD" Value="USD" Selected="true" />
                                                <asp:ListItem Text="LKR" Value="LKR" />
                                                <asp:ListItem Text="INR" Value="INR" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Description</label>
                                            <asp:TextBox ID="txtDetail" runat="server" CssClass="form-control" />
                                        </div>
                                        <!-- end -->
                                    </div>
                                    <div class="row">
                                        <div class="d-flex flex-column flex-md-row justify-content-md-end gap-2">
                                            <asp:Button ID="btnAddPayment" runat="server" Text="Add Payment Record" CssClass="btn btn-primary" OnClick="btnAddPayment_Click" />
                                        </div>
                                    </div>

                                    <!--gridview start here-->
                                    <div class="row mb-4">
                                        <!-- Updated ASPX GridView section -->
                                        <div class="row mb-4">
                                            <asp:GridView ID="GridView" runat="server" OnRowCommand="GridView_RowCommand" AutoGenerateColumns="false" CssClass="table table-striped">
                                                <Columns>
                                                    <asp:BoundField DataField="SupplierName" HeaderText="Supplier Name" />
                                                    <asp:BoundField DataField="Qty" HeaderText="Quantity" DataFormatString="{0:N2}" />
                                                    <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:N2}" />
                                                    <asp:BoundField DataField="Currency" HeaderText="Currency" />
                                                    <asp:BoundField DataField="Detail" HeaderText="Description" />
                                                    <asp:BoundField DataField="TotalPrice" HeaderText="Total Price" DataFormatString="{0:N2}" />
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>
                                                            <asp:Button ID="btnDelete" runat="server"
                                                                Text="Delete"
                                                                CssClass="btn btn-danger btn-sm"
                                                                CommandName="DeleteRow"
                                                                CommandArgument="<%# Container.DataItemIndex %>"
                                                                OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div class="text-center p-3">
                                                        <p class="mb-0">No payment records found.</p>
                                                    </div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <!--gridview end here-->

                                    <asp:Label ID="lbltotalINR" runat="server"></asp:Label>
                                    <asp:Label ID="lbltotalUSD" runat="server"></asp:Label>
                                    <asp:Label ID="lbltotalLKR" runat="server"></asp:Label>

                                    <!-- Confirmation -->
                                    <div class="row mb-4">
                                        <div class="col-md-8 offset-md-2">
                                            <div class="row align-items-center">
                                                <div class="col-md-6 mb-3">
                                                    <label class="form-label">Costing, Configuration & recommendation confirmed by</label>
                                                </div>
                                                <div class="col-md-6 mb-3">
                                                    <asp:DropDownList ID="ddlConfirmedBy" runat="server" CssClass="form-select">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Authorizing Template -->
                                    <div class="row mb-4">
                                        <div class="col-md-8 offset-md-2">
                                            <div class="row align-items-center">
                                                <div class="col-md-6 mb-3">
                                                    <label class="form-label">Authorizing Template</label>
                                                </div>
                                                <div class="col-md-6 mb-3">
                                                    <asp:DropDownList ID="ddlTemplate" runat="server" CssClass="form-select" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <!-- IT Division Comments -->
                            <div class="row mb-4">
                                <div class="col-12 mb-3">
                                    <h5 class="border-bottom pb-2">IT Division Comments</h5>
                                    <asp:TextBox ID="txtITComments" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control w-100" Style="min-width: 100%"/>
                                </div>
                            </div>

                            <!-- IT Division Recommendation -->
                            <div class="row mb-4">
                                <div class="col-12 mb-3">
                                    <h5 class="border-bottom pb-2">IT Division Recommendation (with justification)</h5>
                                    <asp:TextBox ID="txtITDivRecommend" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control w-100" Style="min-width: 100%" />
                                </div>
                            </div>

                            <!-- Remarks -->
                            <div class="row mb-4">
                                <div class="col-12 mb-3">
                                    <h5 class="border-bottom pb-2">Remarks</h5>
                                    <asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" Style="min-width: 100%" />
                                </div>
                            </div>

                            <!-- Action Buttons -->
                            <div class="row">
                                <div class="col-12">
                                    <div class="d-flex flex-column flex-md-row justify-content-md-end gap-2">
                                        <asp:Button ID="btnSubmit" runat="server" Text="Print" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                                        <asp:Button ID="btnReset" CssClass="btn btn-secondary" runat="server" Text="Reset" OnClick="btnReset_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
        <script>

            //numbering validation function for user can anter only 0-9 values
            function allowOnlyNumbers(inputId) {
                const input = document.getElementById(inputId);

                if (input) {
                    input.addEventListener('keypress', function (event) {
                        const key = event.key;

                        // Allow only digits (0-9)
                        if (!/^\d$/.test(key)) {
                            event.preventDefault();
                        }
                    });

                    input.addEventListener('paste', function (event) {
                        // Prevent pasting non-numeric values
                        const pasteData = (event.clipboardData || window.clipboardData).getData('text');
                        if (!/^\d+$/.test(pasteData)) {
                            event.preventDefault();
                        }
                    });
                }
            }
            //to valitate Unit price
            function allowOnlyDecimal(inputId) {
                const input = document.getElementById(inputId);
                if (input) {
                    input.addEventListener('keypress', function (event) {
                        const key = event.key;
                        const currentValue = input.value;

                        // Allow digits (0-9)
                        if (/^\d$/.test(key)) {
                            return; // Allow the digit
                        }

                        // Allow decimal point only if there isn't already one
                        if (key === '.' && !currentValue.includes('.')) {
                            return; // Allow the decimal point
                        }

                        // Prevent all other characters
                        event.preventDefault();
                    });

                    input.addEventListener('paste', function (event) {
                        // Get the pasted data
                        const pasteData = (event.clipboardData || window.clipboardData).getData('text');

                        // Allow only valid float format (digits with optional single decimal point)
                        if (!/^\d*\.?\d*$/.test(pasteData) || (pasteData.match(/\./g) || []).length > 1) {
                            event.preventDefault();
                        }
                    });
                }
            }

            function pageLoad() {
                allowOnlyNumbers('<%= txtQty.ClientID %>');
                allowOnlyDecimal('<%= txtUnitPrice.ClientID %>');
            }
        </script>
    </main>
</asp:Content>
