<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO509400.aspx.cs" Inherits="Page_SO509400" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="AmazonIntegration.SOGetFBATrackingNumberProcess" PrimaryView="Filter">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" DataMember="Filter">
        <Template>
            <px:PXLayoutRule runat="server" ID="AMCstPXLayoutRuleColumnDates" StartColumn="True" LabelsWidth="XS" ControlSize="M"></px:PXLayoutRule>
            <px:PXDateTimeEdit CommitChanges="True" ID="AMPXDateTimeEditFromDate" runat="server" DataField="FromDate" />
            <px:PXDateTimeEdit CommitChanges="True" ID="AMPXDateTimeEditToDate" runat="server" DataField="ToDate" />
             <px:PXCheckBox ID="PXAmazonCurrentDay" CommitChanges="True" runat="server" DataField="CurrentDay"></px:PXCheckBox>
            <px:PXLayoutRule runat="server" ID="AMPXLayoutRuleFilters" StartColumn="True" LabelsWidth="S" ControlSize="M"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="AMCstPXSelectorIntegrationID" DataField="IntegrationID" CommitChanges="true"></px:PXSelector>
            <px:PXCheckBox ID="AMchkIsActiveProcessAllIntegration" CommitChanges="True" runat="server" DataField="ProcessAllTypes"></px:PXCheckBox>
           
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" SkinID="PrimaryInquire" AllowPaging="true" AdjustPageSize="Auto" SyncPosition="true">
        <Levels>
            <px:PXGridLevel DataMember="TrackingNumberOrdersList">
                <Columns>
                    <px:PXGridColumn CommitChanges="True" Type="CheckBox" DataField="Selected" Width="50px" AllowCheckAll="true" TextAlign="Center"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="IntegrationID" CommitChanges="true" RenderEditorText="True" Width="120px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonOrderID" RenderEditorText="True" Width="160px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonStatus" Width="100px"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonOrderDate" Width="170px" DisplayFormat="MM/dd/yyyy hh:mm:ss tt"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonOrderLastUpdated" Width="170px" DisplayFormat="MM/dd/yyyy hh:mm:ss tt"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="AmazonTrackingNumber" Width="140px"></px:PXGridColumn>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" />
    </px:PXGrid>
</asp:Content>
