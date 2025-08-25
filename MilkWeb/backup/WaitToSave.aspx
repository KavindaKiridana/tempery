<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true" CodeBehind="WaitToSave.aspx.cs"
Inherits="MilkWeb.WaitToSave2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <asp:Label ID="lblAlert" runat="server"></asp:Label>
  <%-- Summary Panel --%>
  <asp:Panel
    ID="PanelSummary"
    runat="server"
    BorderStyle="Solid"
    BorderWidth="1px"
    Width="49%"
    Style="padding: 5px;"
  >
    <table style="width: 100%; border-collapse: collapse">
      <tr>
        <td style="width: 120px; padding: 2px">Total Quantity :</td>
        <td style="padding: 2px">
          <asp:Label
            ID="lblTotalQuantity"
            runat="server"
            Text="0"
            Font-Bold="True"
          ></asp:Label>
        </td>
        <td style="width: 120px; padding: 2px">Record Count :</td>
        <td style="padding: 2px">
          <asp:Label
            ID="lblRecordCount"
            runat="server"
            Text="0"
            Font-Bold="True"
          ></asp:Label>
        </td>
      </tr>
      <tr>
        <td style="width: 120px; padding: 2px">FAT Average :</td>
        <td style="padding: 2px">
          <asp:Label
            ID="lblFATAverage"
            runat="server"
            Text="0"
            Font-Bold="True"
          ></asp:Label>
        </td>
        <td style="width: 120px; padding: 2px">SNF Average :</td>
        <td style="padding: 2px">
          <asp:Label
            ID="lblSNFAverage"
            runat="server"
            Text="0"
            Font-Bold="True"
          ></asp:Label>
        </td>
      </tr>
    </table>
  </asp:Panel>

  <asp:GridView
    ID="GridView1"
    runat="server"
    AllowPaging="True"
    AllowSorting="True"
    DataSourceID="SqlDataSource1"
    AutoGenerateColumns="False"
    DataKeyNames="ID"
  >
    <Columns>
      <asp:CommandField ShowDeleteButton="True" />
      <asp:BoundField
        DataField="ID"
        HeaderText="ID"
        InsertVisible="False"
        ReadOnly="True"
        SortExpression="ID"
      />
      <asp:BoundField
        DataField="CCode"
        HeaderText="CCode"
        SortExpression="CCode"
      />
      <asp:BoundField
        DataField="FarmerCode"
        HeaderText="FarmerCode"
        SortExpression="FarmerCode"
      />
      <asp:BoundField
        DataField="OldCode"
        HeaderText="OldCode"
        SortExpression="OldCode"
      />
      <asp:BoundField
        DataField="Parent"
        HeaderText="Parent"
        SortExpression="Parent"
      />
      <asp:BoundField
        DataField="DateOf"
        HeaderText="DateOf"
        SortExpression="DateOf"
        DataFormatString="{0:yyyy-MM-dd}"
      />
      <asp:BoundField
        DataField="TimeOf"
        HeaderText="TimeOf"
        SortExpression="TimeOf"
      />
      <asp:BoundField DataField="SNF" HeaderText="SNF" SortExpression="SNF" />
      <asp:BoundField DataField="FAT" HeaderText="FAT" SortExpression="FAT" />
      <asp:BoundField DataField="Qty" HeaderText="Qty" SortExpression="Qty" />
      <asp:BoundField
        DataField="SavedTime"
        HeaderText="SavedTime"
        SortExpression="SavedTime"
        DataFormatString="{0:yyyy-MM-dd}"
      />
    </Columns>
  </asp:GridView>
  <asp:SqlDataSource
    ID="SqlDataSource1"
    runat="server"
    ConflictDetection="CompareAllValues"
    ConnectionString="<%$ ConnectionStrings:MilkWeb %>"
    DeleteCommand="DELETE FROM [tblDWebTemp] WHERE [ID] = @original_ID AND [CCode] = @original_CCode AND [FarmerCode] = @original_FarmerCode AND [OldCode] = @original_OldCode AND [Parent] = @original_Parent AND [DateOf] = @original_DateOf AND [TimeOf] = @original_TimeOf AND [SNF] = @original_SNF AND [FAT] = @original_FAT AND [Qty] = @original_Qty AND [SavedTime] = @original_SavedTime"
    InsertCommand="INSERT INTO [tblDWebTemp] ([CCode], [FarmerCode], [OldCode], [Parent], [DateOf], [TimeOf], [SNF], [FAT], [Qty], [SavedTime]) VALUES (@CCode, @FarmerCode, @OldCode, @Parent, @DateOf, @TimeOf, @SNF, @FAT, @Qty, @SavedTime)"
    OldValuesParameterFormatString="original_{0}"
    ProviderName="System.Data.SqlClient"
    SelectCommand="SELECT [ID], [CCode], [FarmerCode], [OldCode], [Parent],  CONVERT(DATE, [DateOf]) AS DateOf, [TimeOf], [SNF], [FAT], [Qty], [SavedTime] FROM [tblDWebTemp] WHERE (([UserId] = @UserId) AND ([CCode] = @CCode) AND ([SavedTime] &lt; @SavedTime2))"
    UpdateCommand="UPDATE [tblDWebTemp] SET [CCode] = @CCode, [FarmerCode] = @FarmerCode, [OldCode] = @OldCode, [Parent] = @Parent, [DateOf] = @DateOf, [TimeOf] = @TimeOf, [SNF] = @SNF, [FAT] = @FAT, [Qty] = @Qty, [SavedTime] = @SavedTime WHERE [ID] = @original_ID AND [CCode] = @original_CCode AND [FarmerCode] = @original_FarmerCode AND [OldCode] = @original_OldCode AND [Parent] = @original_Parent AND [DateOf] = @original_DateOf AND [TimeOf] = @original_TimeOf AND [SNF] = @original_SNF AND [FAT] = @original_FAT AND [Qty] = @original_Qty AND [SavedTime] = @original_SavedTime"
  >
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
      <asp:Parameter DbType="Date" Name="original_SavedTime" />
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
      <asp:Parameter DbType="Date" Name="SavedTime" />
    </InsertParameters>
    <SelectParameters>
      <asp:SessionParameter Name="UserId" SessionField="userId" Type="Int32" />
      <asp:SessionParameter Name="CCode" SessionField="CCode" Type="Int32" />
      <asp:SessionParameter
        DbType="Date"
        Name="SavedTime2"
        SessionField="forWaitToSave"
      />
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
      <asp:Parameter DbType="Date" Name="SavedTime" />
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
      <asp:Parameter DbType="Date" Name="original_SavedTime" />
    </UpdateParameters>
  </asp:SqlDataSource>

  <table class="auto-style1">
    <tr>
      <td>
        <asp:Button
          ID="btnGoBack"
          runat="server"
          PostBackUrl="~/Contact.aspx"
          Text="Go Back"
        />
      </td>
      <td>
        <asp:Button
          ID="btnSave"
          runat="server"
          Text="Save All"
          OnClick="btnSave_Click"
        />
      </td>
    </tr>
  </table>
</asp:Content>
