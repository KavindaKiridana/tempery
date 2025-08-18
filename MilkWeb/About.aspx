<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="MilkWeb.About" %>

<%-- view collection table --%>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <div style="padding: 10px;">
            <%-- Filter Records Section --%>
            <asp:Panel ID="PanelFilterRecords" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="100%" Style="margin-bottom: 10px; padding: 5px;">
                <asp:Label ID="lblFilterRecords" runat="server" Text="Filter Records" Font-Bold="True"></asp:Label>
                <asp:Label ID="lblError" runat="server"></asp:Label>
                <div style="margin-top: 5px;">
                    <asp:RadioButtonList
                        ID="rblFilterType"
                        runat="server"
                        RepeatDirection="Horizontal"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="rblFilterType_SelectedIndexChanged">

                        <asp:ListItem Text="Center" Value="Center" style="margin-left: 5px;" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Society" Value="Society" style="margin-left: 5px;"></asp:ListItem>
                        <asp:ListItem Text="Collector" Value="Collector" style="margin-left: 5px;"></asp:ListItem>
                        <asp:ListItem Text="Direct Farmer" Value="Direct Farmer" style="margin-left: 5px;"></asp:ListItem>
                    </asp:RadioButtonList>

                </div>
                <div style="margin-top: 5px;">
                    <table style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td style="width: 70px; padding: 2px;">
                                <asp:Label ID="lblCenter" runat="server" Text="Center"></asp:Label></td>
                            <td style="padding: 2px;">
                                <asp:Label ID="lblCenterOption" runat="server" ></asp:Label>
                            </td>
                            <td style="width: 50px; padding: 2px;">From</td>
                            <td style="padding: 2px;">
                                <asp:TextBox TextMode="Date" ID="txtFromDate" runat="server"></asp:TextBox>
                            </td>
                            <td style="width: 60px; padding: 2px; text-align: right;" rowspan="2">
                                <asp:Button ID="btnView" runat="server" Text="View" OnClick="btnView_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 70px; padding: 2px;">
                                <asp:Label ID="lblSociety" runat="server" Text="Society"></asp:Label></td>
                            <td style="padding: 2px;">
                                <asp:DropDownList ID="ddlSociety" runat="server" Style="margin-left: 2px;">
                                    <asp:ListItem Text="-- Select --" Value="null" Selected="true" />
                                </asp:DropDownList>
                            </td>
                            <td style="width: 50px; padding: 2px;">To</td>
                            <td style="padding: 2px;">
                                <asp:TextBox TextMode="Date" ID="txtToDate" runat="server" ></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>

           <%-- GridView/Table Section --%>
<div borderstyle="Solid" borderwidth="1px" width="100%" height="300px" style="background-color: #f0f0f0; margin-bottom: 10px;">
    <asp:GridView ID="GridView1" runat="server" 
        AllowPaging="True" 
        AllowSorting="True" 
        AutoGenerateColumns="False" 
        CellPadding="2" 
        CellSpacing="1"
        OnRowDeleting="GridView1_RowDeleting"
        OnRowDataBound="GridView1_RowDataBound"
        DataKeyNames="ID">
        <Columns>
            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:LinkButton ID="btnDelete" runat="server" 
                        CommandName="Delete" 
                        Text="Delete" 
                        CssClass="btn btn-danger btn-sm"
                        OnClientClick="return confirm('Are you sure you want to delete this record?');"
                        Visible='<%# Eval("Accept").ToString() == "0" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" />
            <asp:BoundField DataField="CCode" HeaderText="CCode" SortExpression="CCode" />
            <asp:BoundField DataField="FarmerCode" HeaderText="FarmerCode" SortExpression="FarmerCode" />
            <asp:BoundField DataField="OldCode" HeaderText="OldCode" SortExpression="OldCode" />
            <asp:BoundField DataField="Parent" HeaderText="Parent" SortExpression="Parent" />
            <asp:BoundField DataField="DateOf" HeaderText="DateOf" SortExpression="DateOf" />
            <asp:BoundField DataField="TimeOf" HeaderText="TimeOf" SortExpression="TimeOf" />
            <asp:BoundField DataField="SNF" HeaderText="SNF" SortExpression="SNF" />
            <asp:BoundField DataField="FAT" HeaderText="FAT" SortExpression="FAT" />
            <asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" />
            <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" />
            <asp:BoundField DataField="Accept" HeaderText="Accept" SortExpression="Accept" />
        </Columns>
    </asp:GridView>

            <%-- Bottom Buttons (Save and Cancel) --%>
            <div style="text-align: right;">
              <!--  <asp:Button ID="btnSave" runat="server" Text="Save" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" Style="margin-left: 5px;" />-->
            </div>
        </div>
    </main>
</div>
</asp:Content>
