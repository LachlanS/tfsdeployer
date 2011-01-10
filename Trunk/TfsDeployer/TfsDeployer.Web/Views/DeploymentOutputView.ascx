<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeploymentOutputView.ascx.cs" Inherits="TfsDeployer.Web.Views.DeploymentOutputView" %>
<script type="text/javascript" src="<%= ResolveUrl("~/Views/DeploymentOutputView.js") %>"></script>
<div id="DeploymentOutput">
    <pre><%# Model.HtmlEncodedOutput %></pre>
    <div class="IsFinal" runat="server" visible="<%# Model.IsFinal %>">
        <p class="IsFinal">Deployment finished.</p>
    </div>
</div>