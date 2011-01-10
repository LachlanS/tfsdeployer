<%@ Page Language="C#" MasterPageFile="~/NoForm.Master" AutoEventWireup="true" CodeBehind="Configuration.aspx.cs" Inherits="TfsDeployer.Web.Configuration" %>
<%@ Register TagPrefix="views" TagName="ConfigurationView" Src="~/Views/ConfigurationView.ascx" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
    <views:ConfigurationView runat="server" />
</asp:Content>
