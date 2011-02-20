<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeploymentOutputView.ascx.cs" Inherits="TfsDeployer.Web.Views.DeploymentOutputView" %>
<div id="DeploymentOutput">
    <pre><%# Model.HtmlEncodedOutput %></pre>
    <asp:PlaceHolder runat="server" Visible="<%# Model.IsFinal %>">
        <p class="is-final">Deployment finished.</p>
    </asp:PlaceHolder>
</div>