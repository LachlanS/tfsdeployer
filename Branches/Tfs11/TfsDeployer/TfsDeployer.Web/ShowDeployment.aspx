<%@ Page Language="C#" MasterPageFile="~/NoForm.Master" AutoEventWireup="true" CodeBehind="ShowDeployment.aspx.cs" Inherits="TfsDeployer.Web.ShowDeployment" %>
<%@ Register TagName="DeploymentOutputView" TagPrefix="Views" Src="~/Views/DeploymentOutputView.ascx" %>
<asp:Content ContentPlaceHolderID="contentPlaceHolder" runat="server">
    <!-- more deployment data will go here -->
    <views:DeploymentOutputView runat="server" />
</asp:Content>
