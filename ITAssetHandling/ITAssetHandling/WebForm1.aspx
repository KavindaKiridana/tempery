<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="ITAssetHandling.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="table-responsive">
                <table class="table table-bordered table-hover">
                    <thead>
                        <tr>
                            <th>Supplier</th>
                            <th>Description</th>
                            <th>Quantity</th>
                            <th>Unit Price</th>
                            <th>Total - USD</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtSupply1" runat="server" AutoPostBack="True" /></td>
                            <!--supplier1-->
                            <td>
                                <asp:TextBox ID="txtDetail1" runat="server" AutoPostBack="True" /></td>
                            <!--description1-->
                            <td>
                                <asp:TextBox ID="txtQty1" runat="server" AutoPostBack="True" OnTextChanged="record1_TextChanged" /></td>
                            <!--qty1-->
                            <td>
                                <asp:TextBox ID="txtPrice1" runat="server" AutoPostBack="True" OnTextChanged="record1_TextChanged" /></td>
                            <!--unitprice1-->
                            <td>
                                <asp:Label ID="lblTotal1" runat="server" /></td>
                            <!--total1-->
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtSupply2" runat="server" AutoPostBack="True" /></td>
                            <!--supplier2-->
                            <td><asp:TextBox ID="txtDetail2" runat="server" AutoPostBack="True" /></td>
                            <!--description2-->
                            <td><asp:TextBox ID="txtQty2" runat="server" AutoPostBack="True" OnTextChanged="record2_TextChanged" /></td>
                            <!--qty2-->
                            <td><asp:TextBox ID="txtPrice2" runat="server" AutoPostBack="True" OnTextChanged="record2_TextChanged" /></td>
                            <!--unitprice2-->
                            <td><asp:Label ID="lblTotal2" runat="server" /></td>
                            <!--total2-->
                        </tr>
                        <tr>
                            <td> <asp:TextBox ID="txtSupply3" runat="server" AutoPostBack="True" /></td>
                            <!--supplier3-->
                            <td><asp:TextBox ID="txtDetail3" runat="server" AutoPostBack="True" /></td>
                            <!--description3-->
                            <td><asp:TextBox ID="txtQty3" runat="server" AutoPostBack="True" OnTextChanged="record3_TextChanged" /></td>
                            <!--qty3-->
                            <td><asp:TextBox ID="txtPrice3" runat="server" AutoPostBack="True" OnTextChanged="record3_TextChanged" /></td>
                            <!--unitprice3-->
                            <td><asp:Label ID="lblTotal3" runat="server" /></td>
                            <!--total3-->
                        </tr>
                        <tr>
                            <td> <asp:TextBox ID="txtSupply4" runat="server" AutoPostBack="True" /></td>
                            <!--supplier4-->
                            <td><asp:TextBox ID="txtDetail4" runat="server" AutoPostBack="True" /></td>
                            <!--description4-->
                            <td><asp:TextBox ID="txtQty4" runat="server" AutoPostBack="True" OnTextChanged="record4_TextChanged" /></td>
                            <!--qty4-->
                            <td><asp:TextBox ID="TextBox4" runat="server" AutoPostBack="True" OnTextChanged="record4_TextChanged" /></td>
                            <!--unitprice4-->
                            <td><asp:Label ID="lblTotal4" runat="server" /></td>
                            <!--total4-->
                        </tr>
                        <tr>
                            <td colspan="4">Total Cost - USD</td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>


        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
