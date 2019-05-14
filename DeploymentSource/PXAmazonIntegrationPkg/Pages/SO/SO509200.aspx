<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO509200.aspx.cs" Inherits="Page_SO509200" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="AMContScheduleImportOrdersActions" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter" TypeName="AmazonIntegration.SOScheduleProcess">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="AMContScheduleImportOrders" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="Filter"
        Width="100%">
        <Template>
            <px:PXLayoutRule runat="server" ID="AMCstPXLayoutRuleColumnDates" StartColumn="True" LabelsWidth="XS" ControlSize="M"></px:PXLayoutRule>
            <px:PXDateTimeEdit CommitChanges="True" ID="AMPXDateTimeEditToDate" runat="server" DataField="ToDate" />
            <px:PXLayoutRule runat="server" ID="AMPXLayoutRuleFilters" StartColumn="True" LabelsWidth="S" ControlSize="M"></px:PXLayoutRule>
            <px:PXSelector runat="server" CommitChanges="True" ID="AMCstPXSelectoIntegrationID" DataField="IntegrationID"></px:PXSelector>
            <px:PXCheckBox ID="AMchkIsActiveProcessAllIntegration" CommitChanges="True" runat="server" DataField="ProcessAllTypes"></px:PXCheckBox>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="AmcontOrderDetails" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" SkinID="PrimaryInquire" AllowFilter="true" AllowPaging="true" AdjustPageSize="Auto">
        <Levels>
            <px:PXGridLevel DataMember="prepareAndImport">
                <Columns>
                    <px:PXGridColumn CommitChanges="True" Type="CheckBox" DataField="Selected" Width="50px" AllowCheckAll="true" TextAlign="Center"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="LastSyncDate" DisplayFormat="MM/dd/yyyy" Width="170px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ToDate" DisplayFormat="MM/dd/yyyy" Width="170px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="processID" Width="100px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="IntegrationID" Width="180px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />

    </px:PXGrid>
</asp:Content>
