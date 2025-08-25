<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="MilkWeb.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>
        <script type="text/javascript" src="keyboardNavigation.js"></script>

    <!-- Select2 initialization script -->
    <script type="text/javascript">
        $(document).ready(function () {
            // Initialize Select2 on the collector dropdown
            $("[id*=ddlCollector]").select2({
                placeholder: "Search and select a collector...",
                allowClear: true
            });

            // Initialize Select2 on the farmers dropdown
            $("[id*=ddlFarmers]").select2({
                placeholder: "Search and select a farmer...",
                allowClear: true
            });

            // Handle the change event for Select2 - Collector
            $("[id*=ddlCollector]").on('select2:select', function (e) {
                // Trigger the server-side postback manually
                __doPostBack(this.name, '');
            });

            // Handle the change event for Select2 - Farmers
            $("[id*=ddlFarmers]").on('select2:select', function (e) {
                // Trigger the server-side postback manually
                __doPostBack(this.name, '');
            });

            // Auto-sync logic - moved inside document ready to ensure elements exist
            initializeAutoSync();
        });

        // Validate float input to allow only one decimal place
        function validateFloat(input) {
            let value = input.value;
            if (value === "") return;

            const regex = /^\d+(\.\d{0,1})?$/;
            if (!regex.test(value)) {
                input.value = value.slice(0, -1);
            }
        }

        //validate Qty input
        function restrictToNumbers(input) {
            // Remove any non-numeric characters
            input.value = input.value.replace(/[^0-9]/g, '');
        }

        // Auto-sync logic function
        function initializeAutoSync() {
            // Independent sync flags for each pair
            let fatSyncEnabled = true;
            let snfSyncEnabled = true;

            const txtFATAll = document.getElementById('<%= txtFATAll.ClientID %>');
            const txtFAT = document.getElementById('<%= txtFAT.ClientID %>');
            const txtSNFAll = document.getElementById('<%= txtSNFAll.ClientID %>');
            const txtSNF = document.getElementById('<%= txtSNF.ClientID %>');

            if (txtFATAll && txtFAT && txtSNFAll && txtSNF) {
                // Sync FAT from All → Individual
                txtFATAll.addEventListener('input', function () {
                    if (fatSyncEnabled) {
                        txtFAT.value = this.value;
                    }
                });

                // Sync SNF from All → Individual
                txtSNFAll.addEventListener('input', function () {
                    if (snfSyncEnabled) {
                        txtSNF.value = this.value;
                    }
                });

                // If user edits FAT manually, stop syncing
                txtFAT.addEventListener('input', function () {
                    fatSyncEnabled = false;
                });

                // If user edits SNF manually, stop syncing
                txtSNF.addEventListener('input', function () {
                    snfSyncEnabled = false;
                });

                // If user clears FAT, re-enable sync
                txtFAT.addEventListener('blur', function () {
                    if (!this.value) {
                        fatSyncEnabled = true;
                        this.value = txtFATAll.value;
                    }
                });

                // If user clears SNF, re-enable sync
                txtSNF.addEventListener('blur', function () {
                    if (!this.value) {
                        snfSyncEnabled = true;
                        this.value = txtSNFAll.value;
                    }
                });
            }
        }
    </script>

    <main aria-labelledby="title">
        <div style="padding: 10px;">
            <%--Error Alert & view to previous collection records--%>
            <asp:Panel ID="Panel1" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="100%" Style="margin-bottom: 10px; padding: 0 5px 5px 5px;">
                 <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>

            <%-- Top Section: Center, Entity, Date, Mor/Eve, Collector, Collectors Farmer Code --%>
            <asp:Panel ID="PanelTopSection" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="100%" Style="margin-bottom: 10px; padding: 5px;">
                <table style="width: 100%; border-collapse: collapse;">
                    <tr>
                        <td style="width: 120px; padding: 2px;">
                            <asp:Label ID="lblCenter" runat="server" Text="Center"></asp:Label>
                        </td>
                        <td style="padding: 2px;">
                            <asp:Label ID="lblCenterOption" runat="server" Style="margin-left: 2px;"></asp:Label>
                        </td>
                        <td style="width: 80px; padding: 2px;">Date</td>
                        <td style="padding: 2px;">
                            <asp:TextBox ID="txtDate" runat="server" TextMode="Date"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; padding: 2px;">
                            <asp:Label ID="lblEntity" runat="server" Text="Entity"></asp:Label>
                        </td>
                        <td style="padding: 2px;">
                            <asp:RadioButtonList ID="rblEntity" runat="server" RepeatDirection="Horizontal"
                                AutoPostBack="true" OnSelectedIndexChanged="rblEntity_SelectedIndexChanged">
                                <asp:ListItem Text="Societies" Value="Societies" style="margin-left: 5px;"></asp:ListItem>
                                <asp:ListItem Text="Collectors" Value="Collectors" style="margin-left: 5px;"></asp:ListItem>
                                <asp:ListItem Text="Direct Farmers" Value="Direct Farmers" style="margin-left: 5px;"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td style="width: 80px; padding: 2px;">
                            <asp:Label ID="lblMorEve" runat="server" Text="Mor/Eve"></asp:Label>
                        </td>
                        <td style="padding: 2px;">
                            <asp:DropDownList ID="ddlMorEve" runat="server" Width="120px">
                                <asp:ListItem Text="Morning" Value="Morning" Selected="true" />
                                <asp:ListItem Text="Evening" Value="Evening" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; padding: 2px;">
                            <asp:Label ID="lblCollector" runat="server" Text="Collector"></asp:Label>
                        </td>
                        <td style="padding: 2px;">
                            <asp:DropDownList ID="ddlCollector" runat="server"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlCollector_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 80px; padding: 2px;"></td>
                        <td style="padding: 2px;"></td>
                    </tr>
                </table>
            </asp:Panel>

            <%-- Middle Section: If apply for all, Summary --%>
            <asp:Panel ID="PanelMiddleSection" runat="server" Width="100%" Style="display: flex; justify-content: space-between; margin-bottom: 10px;">
                <%-- If apply for all Panel --%>
                <asp:Panel ID="PanelApplyForAll" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="49%" Style="padding: 5px;">
                    <asp:Label ID="lblApplyForAll" runat="server" Text="If apply for all" Font-Bold="true"></asp:Label>
                    <div style="margin-top: 5px;">
                        <table style="width: 100%; border-collapse: collapse;">
                            <tr>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblFAT" runat="server" Text="FAT"></asp:Label>
                                </td>
                                <td style="padding: 2px;">
                                    <asp:TextBox
                                        ID="txtFATAll"
                                        runat="server"
                                        Width="70px"
                                        step="0.1"
                                        min="0.1"
                                        oninput="validateFloat(this)">
                                    </asp:TextBox>
                                </td>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblSNF" runat="server" Text="SNF"></asp:Label>
                                </td>
                                <td style="padding: 2px;">
                                    <asp:TextBox
                                        ID="txtSNFAll"
                                        runat="server"
                                        Width="70px"
                                        step="0.1"
                                        min="0.1"
                                        oninput="validateFloat(this)">
                                    </asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div style="margin-top: 10px;">
                        <table style="width: 100%; border-collapse: collapse;">
                            <tr>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblFarmer" runat="server" Text="Farmer"></asp:Label>
                                </td>
                                <td colspan="3" style="padding: 2px; width: 200px;">
                                    <asp:DropDownList ID="ddlFarmers" runat="server"
                                        Style="width: 200px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblFAT2" runat="server" Text="FAT"></asp:Label>
                                </td>
                                <td style="padding: 2px;">
                                    <asp:TextBox
                                        ID="txtFAT"
                                        runat="server"
                                        Width="70px"
                                        step="0.1"
                                        min="0.1"
                                        oninput="validateFloat(this)">
                                    </asp:TextBox>
                                </td>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblSNF2" runat="server" Text="SNF"></asp:Label>
                                </td>
                                <td style="padding: 2px;">
                                    <asp:TextBox
                                        ID="txtSNF"
                                        runat="server"
                                        Width="70px"
                                        step="0.1"
                                        min="0.1"
                                        oninput="validateFloat(this)">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblQuantity" runat="server" Text="Quantity (L)"></asp:Label>
                                </td>
                                <td style="padding: 2px;">
                                    <asp:TextBox ID="txtQuantity" runat="server" Width="70px" oninput="restrictToNumbers(this)"></asp:TextBox>
                                </td>
                                <td colspan="2" style="padding: 2px; text-align: right;">
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" Style="margin-left: 5px;" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>

                <%-- Summary Panel --%>
                <asp:Panel ID="PanelSummary" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="49%" Style="padding: 5px;">
                    <table style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td style="width: 120px; padding: 2px;">
                                Total Quantity :
                            </td>
                            <td style="padding: 2px;">
    <asp:Label ID="lblTotalQuantity" runat="server" Text="0" Font-Bold="True"></asp:Label>
                            </td>
                            <td style="width: 120px; padding: 2px;">
                               Record Count :                    
                            </td>
                            <td style="padding: 2px;">
                                <asp:Label ID="lblRecordCount" runat="server" Text="0" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 120px; padding: 2px;">
                               FAT Average :
                            </td>
                            <td style="padding: 2px;">
                                <asp:Label ID="lblFATAverage" runat="server" Text="0" Font-Bold="True"></asp:Label>
                            </td>
                            <td style="width: 120px; padding: 2px;">
                                SNF Average :
                            </td>
                            <td style="padding: 2px;">
                                <asp:Label ID="lblSNFAverage" runat="server" Text="0" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:Panel>

<%-- GridView/Table Section --%>
<asp:Panel ID="Panel2" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="100%" Style="background-color: #f0f0f0; margin-bottom: 10px;">
    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" DataSourceID="SqlDataSource1" AllowPaging="True">
        <Columns>
            <asp:CommandField ShowDeleteButton="True" />
            <asp:BoundField DataField="CCode" HeaderText="CCode" SortExpression="CCode" />
            <asp:BoundField DataField="FarmerCode" HeaderText="FarmerCode" SortExpression="FarmerCode" />
            <asp:BoundField DataField="OldCode" HeaderText="OldCode" SortExpression="OldCode" />
            <asp:BoundField DataField="Parent" HeaderText="Parent" SortExpression="Parent" />
            <asp:BoundField DataField="DateOf" HeaderText="DateOf" SortExpression="DateOf" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="TimeOf" HeaderText="TimeOf" SortExpression="TimeOf" />
            <asp:BoundField DataField="SNF" HeaderText="SNF" SortExpression="SNF" />
            <asp:BoundField DataField="FAT" HeaderText="FAT" SortExpression="FAT" />
            <asp:BoundField DataField="Qty" HeaderText="Qty" SortExpression="Qty" />
            <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" Visible="False"/>
        </Columns>
    </asp:GridView>
</asp:Panel>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                ConflictDetection="CompareAllValues" 
                ConnectionString="<%$ ConnectionStrings:MilkWeb %>" 
                DeleteCommand="DELETE FROM [tblDWebTemp] WHERE [ID] = @original_ID AND [CCode] = @original_CCode AND [FarmerCode] = @original_FarmerCode AND [OldCode] = @original_OldCode AND [Parent] = @original_Parent AND [DateOf] = @original_DateOf AND [TimeOf] = @original_TimeOf AND [SNF] = @original_SNF AND [FAT] = @original_FAT AND [Qty] = @original_Qty" 
                InsertCommand="INSERT INTO [tblDWebTemp] ([CCode], [FarmerCode], [OldCode], [Parent], [DateOf], [TimeOf], [SNF], [FAT], [Qty]) VALUES (@CCode, @FarmerCode, @OldCode, @Parent, @DateOf, @TimeOf, @SNF, @FAT, @Qty)" 
                OldValuesParameterFormatString="original_{0}" 
                SelectCommand="SELECT [CCode], [FarmerCode], [OldCode], [Parent],  CONVERT(DATE, [DateOf]) AS DateOf, [TimeOf], [SNF], [FAT], [Qty], [ID] FROM [tblDWebTemp] WHERE (([UserId] = @UserId) AND ([SavedTime] = @SavedTime)) ORDER BY [ID] DESC" 
                UpdateCommand="UPDATE [tblDWebTemp] SET [CCode] = @CCode, [FarmerCode] = @FarmerCode, [OldCode] = @OldCode, [Parent] = @Parent, [DateOf] = @DateOf, [TimeOf] = @TimeOf, [SNF] = @SNF, [FAT] = @FAT, [Qty] = @Qty WHERE [ID] = @original_ID AND [CCode] = @original_CCode AND [FarmerCode] = @original_FarmerCode AND [OldCode] = @original_OldCode AND [Parent] = @original_Parent AND [DateOf] = @original_DateOf AND [TimeOf] = @original_TimeOf AND [SNF] = @original_SNF AND [FAT] = @original_FAT AND [Qty] = @original_Qty">
                <DeleteParameters>
                    <asp:Parameter Name="original_ID" Type="Int32" />
                    <asp:Parameter Name="original_CCode" Type="Int32" />
                    <asp:Parameter Name="original_FarmerCode" Type="String" />
                    <asp:Parameter Name="original_OldCode" Type="String" />
                    <asp:Parameter Name="original_Parent" Type="String" />
                    <asp:Parameter Name="original_DateOf" Type="String" />
                    <asp:Parameter Name="original_TimeOf" Type="String" />
                    <asp:Parameter Name="original_SNF" Type="Decimal" />
                    <asp:Parameter Name="original_FAT" Type="Decimal" />
                    <asp:Parameter Name="original_Qty" Type="Int32" />
                </DeleteParameters>
                <InsertParameters>
                    <asp:Parameter Name="CCode" Type="Int32" />
                    <asp:Parameter Name="FarmerCode" Type="String" />
                    <asp:Parameter Name="OldCode" Type="String" />
                    <asp:Parameter Name="Parent" Type="String" />
                    <asp:Parameter Name="DateOf" Type="String" />
                    <asp:Parameter Name="TimeOf" Type="String" />
                    <asp:Parameter Name="SNF" Type="Decimal" />
                    <asp:Parameter Name="FAT" Type="Decimal" />
                    <asp:Parameter Name="Qty" Type="Int32" />
                </InsertParameters>
                <SelectParameters>
                    <asp:SessionParameter Name="UserId" SessionField="userId" Type="Int32" />
                    <asp:SessionParameter DbType="Date" Name="SavedTime" SessionField="today" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="CCode" Type="Int32" />
                    <asp:Parameter Name="FarmerCode" Type="String" />
                    <asp:Parameter Name="OldCode" Type="String" />
                    <asp:Parameter Name="Parent" Type="String" />
                    <asp:Parameter Name="DateOf" Type="String" />
                    <asp:Parameter Name="TimeOf" Type="String" />
                    <asp:Parameter Name="SNF" Type="Decimal" />
                    <asp:Parameter Name="FAT" Type="Decimal" />
                    <asp:Parameter Name="Qty" Type="Int32" />
                    <asp:Parameter Name="original_ID" Type="Int32" />
                    <asp:Parameter Name="original_CCode" Type="Int32" />
                    <asp:Parameter Name="original_FarmerCode" Type="String" />
                    <asp:Parameter Name="original_OldCode" Type="String" />
                    <asp:Parameter Name="original_Parent" Type="String" />
                    <asp:Parameter Name="original_DateOf" Type="String" />
                    <asp:Parameter Name="original_TimeOf" Type="String" />
                    <asp:Parameter Name="original_SNF" Type="Decimal" />
                    <asp:Parameter Name="original_FAT" Type="Decimal" />
                    <asp:Parameter Name="original_Qty" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>

            <%-- Bottom Buttons --%>
            <div style="text-align: right;">
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
            </div>
        </div>
    </main>
</asp:Content>
