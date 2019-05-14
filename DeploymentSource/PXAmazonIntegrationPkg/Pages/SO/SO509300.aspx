<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO509300.aspx.cs" Inherits="Page_SO509300" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="AMContSubmitFeedActions" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="AmazonIntegration.SOSubmitFeedProcess"
        PrimaryView="Filter">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="SubmitRecords" Visible="true" CommitChanges="true" />
            
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="AMContSumbitFeed" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Style="z-index: 100"
        Width="100%">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True" ID="AMPXLayoutRuleDates" LabelsWidth="XS" ControlSize="S"></px:PXLayoutRule>
            <px:PXDateTimeEdit CommitChanges="True" ID="AMPXDateTimeEditToDate" runat="server" DataField="ToDate" />
            <px:PXLayoutRule runat="server" ID="AMPXLayoutRuleFilters" StartColumn="True" LabelsWidth="S" ControlSize="M"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="AMCstPXSelectorIntegrationID" DataField="IntegrationID" CommitChanges="true"></px:PXSelector>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="AMContSubmitFeedOrders" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" AllowPaging="true" AdjustPageSize="Auto" SkinID="PrimaryInquire" AllowFilter="true" NoteIndicator="false" AutoAdjustColumns="true" AllowSearch="true" SyncPosition="True">
        <Levels>
            <px:PXGridLevel DataMember="SubmitFeedList">

                <Columns>
                    <px:PXGridColumn CommitChanges="True" Type="CheckBox" DataField="Selected" Width="50px" AllowCheckAll="true" TextAlign="Center"></px:PXGridColumn>
                    <px:PXGridColumn AllowUpdate="False" DataField="ShipmentNbr" Width="90px" LinkCommand="ViewShipment" />
                    <px:PXGridColumn DataField="AmazonOrderID" Width="140px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="Status" RenderEditorText="True" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ShipDate" Width="90px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="CustomerID" Width="120px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="SiteID" Width="90px" />
                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="ShipmentQty" TextAlign="Right" Width="90px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ShipVia" DisplayFormat="&gt;aaaaaaaaaaaaaaa" />
                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="ShipmentWeight" TextAlign="Right" Width="100px" />
                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="ShipmentVolume" TextAlign="Right" Width="100px" />
                </Columns>
                <RowTemplate></RowTemplate>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXGrid>
</asp:Content>
