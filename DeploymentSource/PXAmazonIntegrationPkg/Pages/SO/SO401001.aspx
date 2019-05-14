<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO401001.aspx.cs" Inherits="Page_SO401001" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="AMContSOClearActions" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter" TypeName="AmazonIntegration.SOImportClearLog">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand Name="ClearLog" Visible="true" CommitChanges="true" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="AMContSOClearProcess" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="Filter"
        Width="100%">
          <Template>
            <px:PXLayoutRule ID="AMPXLayoutRuleDates" runat="server" StartRow="True" LabelsWidth="XS" ControlSize="SM" Merge="true" />
            <px:PXDateTimeEdit CommitChanges="True" runat="server" DataField="StartDate" ID="AMPXDateTimeEditStartDate" />
            <px:PXDateTimeEdit CommitChanges="True" runat="server" DataField="Enddate" ID="AMPXDateTimeEditEndDate" />
            <px:PXLayoutRule runat="server" ID="AMPXLayoutRuleFilters" StartColumn="True" LabelsWidth="S" ControlSize="SM"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="AMCstPXSelectorIntegraionID" DataField="IntegrationID" CommitChanges="true"></px:PXSelector>
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
                    <px:PXGridColumn AllowUpdate="False" DataField="ParentProcessID" Width="100px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ProcessDate" Width="120px" DisplayFormat="d" />
                    <px:PXGridColumn AllowUpdate="False" DataField="TotalRecordstoImport" Width="140px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ImportedRecordsCount" Width="140px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="FailedRecordsCount"  Width="140px"/>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
       
    </px:PXGrid>
</asp:Content>
