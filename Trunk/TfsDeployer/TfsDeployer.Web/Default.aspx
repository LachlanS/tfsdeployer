﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TfsDeployer.Web._Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <p>TFS Deployer has been running for <%= UptimeText %></p>
    <table>
    <%
    foreach(var recentEvent in RecentEvents)
    {
        %>
        <tr>
            <td><%= recentEvent.BuildNumber %></td>
            <td><%= recentEvent.Triggered.ToString() %></td>
        </tr>
        <%
    }
    %>
    </table>
</body>
</html>
