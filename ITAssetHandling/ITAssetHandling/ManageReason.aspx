<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageReason.aspx.cs" Inherits="ITAssetHandling.ManageSupplier" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <div class="container-fluid py-4">
        <div class="row">
            <!-- Existing Reasons Section -->
            <div class="col-12 col-lg-9 mb-4 mb-lg-0">
                <div class="card shadow-sm">
                    <div class="card-header bg-primary text-white">
                        <h5 class="mb-0"><i class="fa fa-list me-2"></i>Existing Reasons</h5>
                    </div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:GridView ID="GridView1" runat="server"
                                AllowPaging="True" AutoGenerateColumns="False"
                                DataKeyNames="ReasonId" DataSourceID="SqlDataSource1"
                                CssClass="table table-hover"
                                GridLines="None">
                                <Columns>
                                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="False"
                                        EditText="<i class='fa fa-edit'></i>"
                                        UpdateText="<i class='fa fa-refresh'></i>"
                                        CancelText="<i class='fa fa-times' style='font-size:19px'></i>"
                                        ControlStyle-CssClass="btn btn-sm btn-outline-primary"
                                        HeaderStyle-Width="120px" />
                                    <asp:BoundField DataField="RName" HeaderText="Reason Name" SortExpression="RName" ReadOnly="True" />
                                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" SortExpression="IsActive" />
                                </Columns>
                                <PagerStyle CssClass="table-pager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Add Reason Form Section -->
            <div class="col-12 col-lg-3">
                <div class="card shadow-sm">
                    <div class="card-header bg-success text-white">
                        <h5 class="mb-0"><i class="fa fa-plus me-2"></i>Add New Reason</h5>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="lblAlert" runat="server" CssClass=" mb-3 d-block"></asp:Label>
                       
                        <div class="mb-3">
                            <label for="<%= txtRName.ClientID %>" class="form-label">Reason Name</label>
                            <asp:TextBox ID="txtRName" runat="server" CssClass="form-control" placeholder="Enter reason name" required="true" />
                        </div>

                        <div class="d-grid mt-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Add Reason" CssClass="btn btn-success btn-lg" OnClick="btnSubmit_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConflictDetection="CompareAllValues"
        ConnectionString="<%$ ConnectionStrings:ITAssetConn %>"
        InsertCommand="INSERT INTO [Reason] ([RName],[IsActive]) VALUES (@RName,@IsActive)"
        OldValuesParameterFormatString="original_{0}" ProviderName="System.Data.SqlClient"
        SelectCommand="SELECT * FROM [Reason] ORDER BY [RName]"
        UpdateCommand="UPDATE [Reason] SET [IsActive]=@IsActive WHERE [ReasonId] = @original_ReasonId">
        <InsertParameters>
            <asp:Parameter Name="RName" Type="String" />
            <asp:Parameter Name="IsActive" Type="Boolean" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="RName" Type="String" />
            <asp:Parameter Name="original_ReasonId" Type="Int32" />
            <asp:Parameter Name="original_RName" Type="String" />
            <asp:Parameter Name="IsActive" Type="Boolean" />
        </UpdateParameters>
    </asp:SqlDataSource>
</asp:Content>