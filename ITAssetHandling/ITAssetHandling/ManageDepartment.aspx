<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageDepartment.aspx.cs" Inherits="ITAssetHandling.ManageDepartment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <div class="container-fluid py-4">
        <div class="row">
            <!-- Existing Departments Section -->
            <div class="col-12 col-lg-9 mb-4 mb-lg-0">
                <div class="card shadow-sm">
                    <div class="card-header bg-primary text-white">
                        <h5 class="mb-0"><i class="fa fa-list me-2"></i>Existing Departments</h5>
                    </div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:GridView ID="GridView1" runat="server"
                                AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                DataKeyNames="DepartmentId" DataSourceID="SqlDataSource1"
                                CssClass="table table-hover"
                                GridLines="None">
                                <Columns>
                                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="False"
                                        EditText="<i class='fa fa-edit'></i>"
                                        UpdateText="<i class='fa fa-refresh'></i>"
                                        CancelText="<i class='fa fa-times' style='font-size:19px'></i>"
                                        ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                        HeaderStyle-Width="120px" />
                                    <asp:BoundField DataField="DName" HeaderText="Department Name" SortExpression="DName" ReadOnly="True" />
                                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" SortExpression="IsActive" />
                                </Columns>
                                <PagerStyle CssClass="table-pager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Add Department Form Section -->
            <div class="col-12 col-lg-3">
                <div class="card shadow-sm ">
                    <div class="card-header bg-success text-white">
                        <h5 class="mb-0"><i class="fa fa-plus me-2"></i>Add New Department</h5>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="lblAlert" runat="server" CssClass="mb-3 d-block"></asp:Label>

                        <div class="mb-3">
                            <label for="<%= txtDName.ClientID %>" class="form-label">Department Name</label>
                            <asp:TextBox ID="txtDName" runat="server" CssClass="form-control" placeholder="Enter department name" required />
                        </div>

                        <div class="d-grid mt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Add Department" CssClass="btn btn-success btn-lg" OnClick="btnSubmit_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server"
        ConnectionString="<%$ ConnectionStrings:ITAssetConn %>"
        SelectCommand="SELECT [DepartmentId], [DName], [IsActive] FROM [Department] ORDER BY [DName]"
        UpdateCommand="UPDATE [Department] SET [IsActive] = @IsActive WHERE [DepartmentId] = @DepartmentId"
        InsertCommand="INSERT INTO [Department] ([DName], [IsActive]) VALUES (@DName, @IsActive)">
        <UpdateParameters>
            <asp:Parameter Name="DName" Type="String" />
            <asp:Parameter Name="IsActive" Type="Boolean" />
            <asp:Parameter Name="DepartmentId" Type="Int32" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="DName" Type="String" />
            <asp:Parameter Name="IsActive" Type="Boolean" />
        </InsertParameters>
    </asp:SqlDataSource>
</asp:Content>