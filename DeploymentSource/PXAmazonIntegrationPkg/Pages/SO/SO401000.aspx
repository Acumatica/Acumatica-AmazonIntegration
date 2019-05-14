<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO401000.aspx.cs" Inherits="Page_SO401000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="AMContSHClearActions" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter" TypeName="AmazonIntegration.SOSubmitClearLog" PageLoadBehavior="PopulateSavedValues">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="AMContSHClearProcess" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="Filter"
        Width="100%">
        <Template>
            <px:PXLayoutRule ID="AMPXLayoutRuleDates" runat="server" StartRow="True" LabelsWidth="XS" ControlSize="SM" Merge="true" />
            <px:PXDateTimeEdit CommitChanges="True" runat="server" DataField="StartDate" ID="AMPXDateTimeEditStartDate" />
            <px:PXDateTimeEdit CommitChanges="True" runat="server" DataField="Enddate" ID="AMPXDateTimeEditEndDate" />
            <px:PXLayoutRule runat="server" ID="AMPXLayoutRuleFilters" StartColumn="True" LabelsWidth="S" ControlSize="SM"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="AMCstPXSelectorIntegrationID" DataField="IntegrationID" CommitChanges="true"></px:PXSelector>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" SkinID="PrimaryInquire">
        <Levels>
            <px:PXGridLevel DataMember="Log">
                <Columns>
                    <px:PXGridColumn DataField="Selected" Type="CheckBox" Width="50px" CommitChanges="true" AllowCheckAll="true" TextAlign="Center" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ProcessID" Width="100px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ProcessDate" Width="130px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="TotalRecordstoProcess" Width="140px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="SubmitRecordsCount" Width="140px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="SubmitFailedRecordsCount" Width="140px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXGrid>
</asp:Content>
