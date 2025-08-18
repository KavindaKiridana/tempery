<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="MilkWeb.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <div style="padding: 10px;">
            <%-- Top Section: Center, Entity, Date, Mor/Eve, Collector, Collectors Farmer Code --%>
            <asp:Panel ID="PanelTopSection" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="100%" Style="margin-bottom: 10px; padding: 5px;">
                <table style="width: 100%; border-collapse: collapse;">
                    <tr>
                        <td style="width: 120px; padding: 2px;">
                            <asp:Label ID="lblCenter" runat="server" Text="Center"></asp:Label></td>
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
                            <asp:Label ID="lblEntity" runat="server" Text="Entity"></asp:Label></td>
                        <td style="padding: 2px;">
                            <asp:RadioButtonList ID="rblEntity" runat="server" RepeatDirection="Horizontal"
                                AutoPostBack="true" OnSelectedIndexChanged="rblEntity_SelectedIndexChanged">
                                <asp:ListItem Text="Societies" Value="Societies" style="margin-left: 5px;"></asp:ListItem>
                                <asp:ListItem Text="Collectors" Value="Collectors" style="margin-left: 5px;"></asp:ListItem>
                                <asp:ListItem Text="Direct Farmers" Value="Direct Farmers" style="margin-left: 5px;"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td style="width: 80px; padding: 2px;">
                            <asp:Label ID="lblMorEve" runat="server" Text="Mor/Eve"></asp:Label></td>
                        <td style="padding: 2px;">
                            <asp:DropDownList ID="ddlMorEve" runat="server" Width="120px">
                                <asp:ListItem Text="Morning" Value="Morning" Selected="true" />
                                <asp:ListItem Text="Evening" Value="Evening" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; padding: 2px;">
                            <asp:Label ID="lblCollector" runat="server" Text="Collector"></asp:Label></td>
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
                                    <asp:Label ID="lblFAT" runat="server" Text="FAT"></asp:Label></td>
                                <td style="padding: 2px;">
                                    <asp:TextBox ID="txtFAT" runat="server" Text="--.--" Width="70px"></asp:TextBox></td>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblSNF" runat="server" Text="SNF"></asp:Label></td>
                                <td style="padding: 2px;">
                                    <asp:TextBox ID="txtSNF" runat="server" Text="--.--" Width="70px"></asp:TextBox></td>
                            </tr>
                        </table>
                    </div>
                    <div style="margin-top: 10px;">
                        <table style="width: 100%; border-collapse: collapse;">
                            <tr>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblFarmer" runat="server" Text="Farmer"></asp:Label></td>
                                <td colspan="3" style="padding: 2px;">
                                    <asp:TextBox ID="txtFarmer" runat="server" Width="100%"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblFAT2" runat="server" Text="FAT"></asp:Label></td>
                                <td style="padding: 2px;">
                                    <asp:TextBox ID="txtFAT2" runat="server" Text="--.--" Width="70px"></asp:TextBox></td>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblSNF2" runat="server" Text="SNF"></asp:Label></td>
                                <td style="padding: 2px;">
                                    <asp:TextBox ID="txtSNF2" runat="server" Text="--.--" Width="70px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td style="width: 60px; padding: 2px;">
                                    <asp:Label ID="lblQuantity" runat="server" Text="Quantity (Kg)"></asp:Label></td>
                                <td style="padding: 2px;">
                                    <asp:TextBox ID="txtQuantity" runat="server" Width="70px"></asp:TextBox></td>
                                <td colspan="2" style="padding: 2px; text-align: right;">
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Style="margin-left: 5px;" />
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
                                <asp:Label ID="lblTotalQuantity" runat="server" Text="Total Quantity :"></asp:Label></td>
                            <td style="padding: 2px;">
                                <asp:Label ID="lblTotalQuantityValue" runat="server" Text="0" Font-Bold="True"></asp:Label></td>
                            <td style="width: 120px; padding: 2px;">
                                <asp:Label ID="lblTotalPayment" runat="server" Text="Total Payment :"></asp:Label></td>
                            <td style="padding: 2px;">
                                <asp:Label ID="lblTotalPaymentValue" runat="server" Text="0" Font-Bold="True"></asp:Label></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; padding: 2px;">
                                <asp:Label ID="lblFATAverage" runat="server" Text="FAT Average :"></asp:Label></td>
                            <td style="padding: 2px;">
                                <asp:Label ID="lblFATAverageValue" runat="server" Text="0" Font-Bold="True"></asp:Label></td>
                            <td style="width: 120px; padding: 2px;">
                                <asp:Label ID="lblSNFAverage" runat="server" Text="SNF Average :"></asp:Label></td>
                            <td style="padding: 2px;">
                                <asp:Label ID="lblSNFAverageValue" runat="server" Text="0" Font-Bold="True"></asp:Label></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; padding: 2px;">
                                <asp:Label ID="lblRecordCount" runat="server" Text="Record Count :"></asp:Label></td>
                            <td style="padding: 2px;" colspan="3">
                                <asp:Label ID="lblRecordCountValue" runat="server" Text="0" Font-Bold="True"></asp:Label></td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:Panel>

            <%-- GridView/Table Section --%>
            <asp:Panel ID="PanelGridSection" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="100%" Height="200px" Style="background-color: #f0f0f0; margin-bottom: 10px;">
                <asp:Label ID="lblGridPlaceholder" runat="server" Text="[Data GridView Placeholder]" Style="display: block; text-align: center; margin-top: 80px; color: #888;"></asp:Label>
            </asp:Panel>

            <%-- Bottom Buttons --%>
            <div style="text-align: right;">
                <asp:Button ID="btnSave" runat="server" Text="Save" />
                <asp:Button ID="btnCancelBottom" runat="server" Text="Cancel" Style="margin-left: 5px;" />
            </div>
        </div>
    </main>
</asp:Content>
