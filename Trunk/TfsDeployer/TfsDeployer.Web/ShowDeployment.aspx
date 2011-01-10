<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowDeployment.aspx.cs" Inherits="TfsDeployer.Web.ShowDeployment" %>
<%@ Register TagName="DeploymentOutputView" TagPrefix="Views" Src="~/Views/DeploymentOutputView.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.4.4.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.query-2.1.7.js") %>"></script>
    <script type="text/javascript">
        var applicationPath = '<%= ResolveUrl("~/") %>';
    </script>
</head>
<body>
    <!-- more deployment data will go here -->
    <views:DeploymentOutputView runat="server" />
</body>
</html>
