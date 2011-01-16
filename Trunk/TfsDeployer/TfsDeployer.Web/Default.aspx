<%@ Page Language="C#" MasterPageFile="~/NoForm.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TfsDeployer.Web._Default" %>
<%@ Register TagPrefix="views" TagName="UptimeView" Src="~/Views/UptimeView.ascx" %>
<%@ Register TagPrefix="views" TagName="EventListView" Src="~/Views/EventListView.ascx" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="contentPlaceHolder" runat="server">
    <views:UptimeView runat="server" />
    <views:EventListView runat="server" />
</asp:Content>

