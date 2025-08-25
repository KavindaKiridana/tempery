<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MilkWeb._Default"
%>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  <main>
    <div>
      <h2>Login</h2>
      <div>
        <label for="txtUserName">User Name:</label>
        <input id="txtUserName" type="text" runat="server" />
      </div>
      <div>
        <label for="txtPassword">Password:</label>
        <input id="txtPassword" type="password" runat="server" />
      </div>
      <div>
        <asp:Button
          ID="btnLogin"
          runat="server"
          Text="Login"
          OnClick="btnLogin_Click"
        />
      </div>
      <div>
        <asp:Label
          ID="lblError"
          runat="server"
          ForeColor="Red"
          Visible="false"
        ></asp:Label>
      </div>
    </div>
  </main>
</asp:Content>
