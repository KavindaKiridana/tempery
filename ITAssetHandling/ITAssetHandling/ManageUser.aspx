<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUser.aspx.cs" Inherits="ITAssetHandling.ManageUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <div class="container-fluid py-4">
        <div class="row">
            <!-- Existing Users Section -->
            <div class="col-12 col-lg-9">
                <div class="card shadow-sm">
                    <div class="card-header bg-primary text-white">
                        <h5 class="mb-0"><i class="fa fa-list me-2"></i>Existing Users</h5>
                    </div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:GridView ID="GridView1" runat="server"
                                AllowPaging="True" AutoGenerateColumns="False"
                                DataKeyNames="UsersId" DataSourceID="SqlDataSource1"
                                CssClass="table table-hover"
                                GridLines="None">
                                <Columns>
                                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="False"
                                        EditText="<i class='fa fa-edit'></i>"
                                        UpdateText="<i class='fa fa-refresh'></i>"
                                        CancelText="<i class='fa fa-times' style='font-size:19px'></i>"
                                        ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                        HeaderStyle-Width="120px" />
                                    <asp:BoundField DataField="UserName" HeaderText="User Name" SortExpression="UserName" ReadOnly="True" />
                                    <asp:BoundField DataField="FullName" HeaderText="Full Name" SortExpression="FullName" ReadOnly="True" />
                                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" SortExpression="IsActive" />
                                    <asp:CheckBoxField DataField="IsHeadOrNot" HeaderText="Is Head" SortExpression="IsHeadOrNot" ReadOnly="True" />
                                    <asp:BoundField DataField="IsAuthorizer" HeaderText="User Type" SortExpression="IsAuthorizer" ReadOnly="True" />
                                </Columns>
                                <PagerStyle CssClass="table-pager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Add New User Section -->
            <div class="col-12 col-lg-3 mb-4 mb-lg-0">
                <div class="card shadow-sm">
                    <div class="card-header bg-success text-white">
                        <h5 class="mb-0"><i class="fa fa-plus me-2"></i>Add New User</h5>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="lblAlert" runat="server" CssClass=" d-block"></asp:Label>

                        <div class="mb-3">
                            <label for="<%= txtUserName.ClientID %>" class="form-label">User Name</label>
                            <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="User Name" required />
                        </div>

                        <div class="mb-3">
                            <label for="<%= txtFullName.ClientID %>" class="form-label">Full Name</label>
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Full Name" required />
                        </div>

                        <div class="mb-3">
                            <label for="<%= ddlIsHead.ClientID %>" class="form-label">Is Head User?</label>
                            <asp:DropDownList ID="ddlIsHead" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Yes" Value="true" />
                                <asp:ListItem Text="No" Value="false" />
                            </asp:DropDownList>
                        </div>
                        <div class="mb-3">
                            <label for="<%= ddlAutherizer.ClientID %>" class="form-label">User Type</label>
                            <asp:DropDownList ID="ddlAutherizer" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Editor" Value="Editor" />
                                <asp:ListItem Text="Unauthorizer" Value="Unauthorizer" />
                                <asp:ListItem Text="IT Manager" Value="IT Manager" />
                                <asp:ListItem Text="CEO" Value="CEO" />
                                <asp:ListItem Text="MD" Value="MD" />
                            </asp:DropDownList>
                        </div>

                        <div class="mb-3">
                            <label for="<%= txtPassword.ClientID %>" class="form-label">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Password" required />
                        </div>

                        <div class="d-grid mt-4">
                            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" runat="server" Text="Add User" CssClass="btn btn-success btn-lg" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConflictDetection="CompareAllValues"
        ConnectionString="<%$ ConnectionStrings:ITAssetConn %>"
        OldValuesParameterFormatString="original_{0}" ProviderName="System.Data.SqlClient"
        SelectCommand="SELECT [UsersId], [Password], [IsActive], [UserName], [FullName], [IsHeadOrNot], [IsAuthorizer] FROM [Users] ORDER BY [UserName]"
        UpdateCommand="UPDATE [Users] SET [IsActive] = @IsActive  WHERE [UsersId] = @original_UsersId">

        <UpdateParameters>
            <asp:Parameter Name="IsActive" Type="Boolean" />
            <asp:Parameter Name="original_UsersId" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
</asp:Content>
