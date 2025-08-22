<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="MilkWeb.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("[id*=cbCustomer]").select2();
        });
    </script>
    <div>
        <br />
        <br />
        <table>
            <tr>
                <td>Country:-
                </td>
                <td>
                    <asp:DropDownList ID="cbCustomer" runat="server" Width="300px" AutoPostBack="true">
                       <%-- <asp:ListItem Selected="True" Text="--Select--" Value="--Select--"></asp:ListItem>--%>
                        <asp:ListItem Text="Portugal-BL-CZ231" Value="Portugal-BL-CZ231"></asp:ListItem>
                        <asp:ListItem Text="GERMANY-AM-CX2918" Value="GERMANY-AM-CX2918"></asp:ListItem>
                        <asp:ListItem Text="USA-291-BC232X" Value="USA-291-BC232X"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
