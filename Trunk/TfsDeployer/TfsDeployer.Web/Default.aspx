﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TfsDeployer.Web._Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <p>TFS Deployer has been running for <%= UptimeText %></p>
    <table>
    <thead>
    <tr>
    <td>Build Number</td>
    <td>Original Quality</td>
    <td>New Quality</td>
    <td>Triggered At</td>
    <td>Triggered By</td>
    </tr>
    </thead>
    <tbody>
    <%
    foreach(var recentEvent in RecentEvents)
    {
        %>
        <tr>
            <td><%= recentEvent.BuildNumber %></td>
            <td><%= recentEvent.OriginalQuality %></td>
            <td><%= recentEvent.NewQuality %></td>
            <td><%= recentEvent.Triggered.ToString() %></td>
            <td><%= recentEvent.TriggeredBy %></td>
        </tr>
        <%
    }
    %>
    </tbody>
    </table>
</body>
</html>
