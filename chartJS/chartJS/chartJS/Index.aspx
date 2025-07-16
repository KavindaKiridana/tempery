<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="chartJS.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <!-- Include Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</head>
<body style="height: 270px">
        <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
            <h3>Chart.js Canvas</h3>
            <div style="width: 400px; height: 300px;">
                <canvas id="myChart"></canvas> <!-- Add a <canvas> Element -->
            </div>
        </div>
        </form>
</body>
</html>
