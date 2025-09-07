<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageSupplier.aspx.cs" Inherits="ITAssetHandling.ManageSupplier1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <div class="container-fluid py-4">
        <div class="row">
            <!-- Existing Suppliers Section -->
            <div class="col-12 col-lg-9 mb-4 mb-lg-0">
                <div class="card shadow-sm">
                    <div class="card-header bg-primary text-white">
                        <h5 class="mb-0"><i class="fa fa-list me-2"></i>Existing Suppliers</h5>
                    </div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:GridView ID="GridView1" runat="server"
                                AllowPaging="True" AutoGenerateColumns="False"
                                DataKeyNames="SupplierId" DataSourceID="SqlDataSource1"
                                CssClass="table table-hover"
                                GridLines="None" >
                                <Columns>
                                    <asp:CommandField ShowDeleteButton="False" ShowEditButton="True"
                                        EditText="<i class='fa fa-edit'></i>"
                                        UpdateText="<i class='fa fa-refresh'></i>"
                                        CancelText="<i class='fa fa-times' style='font-size:19px'></i>"
                                        ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                        HeaderStyle-Width="120px" />

                                    <asp:BoundField DataField="SupplierId" HeaderText="SupplierId" InsertVisible="False" ReadOnly="True" SortExpression="SupplierId" Visible="False" />
                                    <asp:BoundField DataField="SName" HeaderText="Supplier Name" SortExpression="SName" ReadOnly="True" />
                                    <asp:TemplateField HeaderText="Currency" SortExpression="Currency">
                                        <ItemTemplate>
                                            <%# Eval("Currency") %>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <%# Eval("Currency") %>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" SortExpression="IsActive" />
                                </Columns>
                                <PagerStyle CssClass="table-pager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Add Supplier Form Section -->
            <div class="col-12 col-lg-3">
                <div class="card shadow-sm" >
                    <div class="card-header bg-success text-white">
                        <h5 class="mb-0"><i class="fa fa-plus me-2"></i>Add New Supplier</h5>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="lblAlert" runat="server" ></asp:Label><br />

                        <div class="mb-3">
                            <label for="<%= txtSName.ClientID %>" class="form-label">Supplier Name</label>
                            <asp:TextBox ID="txtSName" runat="server" CssClass="form-control" placeholder="Enter supplier name" required />
                        </div>

                        <div class="mb-3">
                            <label for="<%= ddlCurrency.ClientID %>" class="form-label">Currency</label>
                            <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-select" required>
                                <asp:ListItem Text="Select Currency" Value="" />
                                <asp:ListItem Text="LKR" Value="LKR" />
                                <asp:ListItem Text="USD" Value="USD" />
                            </asp:DropDownList>
                        </div>

                        <div class="d-grid mt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Add Supplier" CssClass="btn btn-success btn-lg" OnClick="btnSubmit_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server"
            ConnectionString="<%$ ConnectionStrings:ITAssetConn %>"
            SelectCommand="SELECT [SupplierId], [SName], [Currency], [IsActive] FROM [Supplier] ORDER BY [SName]"
            UpdateCommand="UPDATE [Supplier] SET [IsActive] = @IsActive WHERE [SupplierId] = @SupplierId">
            <UpdateParameters>
                <asp:Parameter Name="IsActive" Type="Boolean" />
                <asp:Parameter Name="SupplierId" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>