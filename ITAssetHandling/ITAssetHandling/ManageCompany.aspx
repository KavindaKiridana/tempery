<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageCompany.aspx.cs" Inherits="ITAssetHandling.ManageCompany" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <div class="container-fluid py-4">
        <div class="row">
            <div class="col-12">
                <!--  <h2 class="mb-4">Manage Companies</h2>-->
            </div>
        </div>

        <div class="row">
            <!-- Existing Companies Section -->
            <div class="col-12 col-lg-9 mb-4 mb-lg-0">
                <div class="card shadow-sm">
                    <div class="card-header bg-primary text-white">
                        <h5 class="mb-0"><i class="fa fa-list me-2"></i>Existing Companies</h5>
                    </div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:GridView ID="GridView1" runat="server"
                                DataSourceID="SqlDataSource1"
                                AutoGenerateColumns="False"
                                DataKeyNames="CompanyId"
                                AllowPaging="True"
                                OnRowUpdated="GridView1_RowUpdated"
                                CssClass="table  table-hover"
                                GridLines="None">
                                <Columns>
                                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="False"
                                        EditText="<i class='fa fa-edit'></i>"
                                        UpdateText="<i class='fa fa-refresh'></i>"
                                        CancelText="<i class='fa fa-times' style='font-size:19px'></i>"
                                        ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                        HeaderStyle-Width="120px" />

                                    <asp:BoundField DataField="CompanyId" HeaderText="CompanyId" InsertVisible="False" ReadOnly="True" SortExpression="CompanyId" Visible="False" />
                                    <asp:BoundField DataField="CName" HeaderText="Company Name" SortExpression="CName" ReadOnly="True" />
                                    <asp:BoundField DataField="Flag" HeaderText="Flag" SortExpression="Flag" ReadOnly="True" />
                                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" SortExpression="IsActive" />
                                </Columns>
                                <PagerStyle CssClass="table-pager" />

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Add Company Form Section -->
            <div class="col-12 col-lg-3">
                <div class="card shadow-sm" > 
                    <div class="card-header bg-success text-white">
                        <h5 class="mb-0"><i class="fa fa-plus me-2"></i>Add New Company</h5>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="lblAlert" runat="server" CssClass="mb-3 d-block"></asp:Label>

                        <div class="mb-3">
                            <label for="<%= txtCName.ClientID %>" class="form-label">Company Name</label>
                            <asp:TextBox ID="txtCName" runat="server" CssClass="form-control" placeholder="Enter company name" required />
                        </div>

                        <div class="mb-3">
                            <label for="<%= txtFlag.ClientID %>" class="form-label">Flag</label>
                            <asp:TextBox ID="txtFlag" runat="server" CssClass="form-control" placeholder="Enter flag" required />
                        </div>

                        <div class="d-grid mt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Add Company" CssClass="btn btn-success btn-lg" OnClick="btnSubmit_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server"
        ConnectionString="<%$ ConnectionStrings:ITAssetConn %>"
        SelectCommand="SELECT * FROM [Company] ORDER BY [CName]"
        UpdateCommand="UPDATE [Company] SET [IsActive] = @IsActive WHERE [CompanyId] = @CompanyId"
        InsertCommand="INSERT INTO [Company] ([CName], [Flag], [IsActive]) VALUES (@CName, @Flag, @IsActive)">

        <UpdateParameters>
            <asp:Parameter Name="CName" Type="String" />
            <asp:Parameter Name="Flag" Type="String" />
            <asp:Parameter Name="IsActive" Type="Boolean" />
            <asp:Parameter Name="CompanyId" Type="Int32" />
        </UpdateParameters>

        <InsertParameters>
            <asp:Parameter Name="CName" Type="String" />
            <asp:Parameter Name="Flag" Type="String" />
            <asp:Parameter Name="IsActive" Type="Boolean" />
        </InsertParameters>
    </asp:SqlDataSource>
</asp:Content>
