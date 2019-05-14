<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO509100.aspx.cs" Inherits="Page_SO509100" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="AMContImportOrdersActions" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="AmazonIntegration.SOImportProcess" PrimaryView="Filter">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="ImportRecords" Visible="true" CommitChanges="true" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="AMContImportOrders" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="Filter"
        Width="100%">
        <Template>
            <px:PXLayoutRule runat="server" ID="AMCstPXLayoutRuleColumnDates" StartColumn="True" LabelsWidth="XS" ControlSize="M"></px:PXLayoutRule>
            <px:PXDateTimeEdit CommitChanges="True" ID="AMPXDateTimeEditLastSyncDate" runat="server" DataField="LastSyncDate" />
            <px:PXDateTimeEdit CommitChanges="True" ID="AMPXDateTimeEditToDate" runat="server" DataField="ToDate" />
            <px:PXLayoutRule runat="server" ID="AMPXLayoutRuleFilters" StartColumn="True" LabelsWidth="S" ControlSize="M"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="AMCstPXSelectorIntegrationID" DataField="IntegrationID" CommitChanges="true"></px:PXSelector>
            <px:PXCheckBox ID="AMchkIsActiveProcessAllIntegration" CommitChanges="True" runat="server" DataField="ProcessAllTypes"></px:PXCheckBox>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" SkinID="PrimaryInquire" AllowPaging="true" AdjustPageSize="Auto" MatrixMode="true" SyncPosition="true">
        <Levels>
            <px:PXGridLevel DataMember="ImportOrderList">
                <Columns>
                    <px:PXGridColumn CommitChanges="True" Type="CheckBox" DataField="Selected" Width="50px" AllowCheckAll="true" TextAlign="Center"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="SynDatetime" Width="150px" DisplayFormat="MM/dd/yyyy hh:mm:ss tt" />
                    <px:PXGridColumn AllowUpdate="False" DataField="IntegrationID" CommitChanges="true" RenderEditorText="True" Width="110px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonOrderID" RenderEditorText="True" Width="140px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ProcessID" CommitChanges="true" RenderEditorText="True" Width="90px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonStatus" Width="100px"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonOrderDate" Width="150px" DisplayFormat="MM/dd/yyyy hh:mm:ss tt"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonOrderLastUpdated" Width="120px" DisplayFormat="MM/dd/yyyy hh:mm:ss tt"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="BuyerEmailID" Width="120px"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="OrderAmount" Width="90px"></px:PXGridColumn>                    
                </Columns>
                <RowTemplate>
                    <px:PXSelector CommitChanges="True" ID="IntegrationID" runat="server" DataField="IntegrationID" AutoRefresh="True" />
                </RowTemplate>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXGrid>
</asp:Content>
