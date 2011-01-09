<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventListView.ascx.cs" Inherits="TfsDeployer.Web.Views.EventListView" %>
<table>
    <thead>
        <tr>
            <td>Build Number</td>
            <td>Original Quality</td>
            <td>New Quality</td>
            <td>Triggered At</td>
            <td>Triggered By</td>
            <td>Queued Deployments</td>
        </tr>
    </thead>
    <tbody>
        <%
            foreach(var recentEvent in Model.RecentEvents)
            {
                %>
                    <tr>
                        <td><%= recentEvent.BuildNumber %></td>
                        <td><%= recentEvent.OriginalQuality %></td>
                        <td><%= recentEvent.NewQuality %></td>
                        <td><%= recentEvent.TriggeredUtc.ToString() %></td>
                        <td><%= recentEvent.TriggeredBy %></td>
                        <td><%
                            if (recentEvent.QueuedDeployments != null && recentEvent.QueuedDeployments.Length > 0)
                            {
                                foreach( var queuedDeployment in recentEvent.QueuedDeployments)
                                {
                                     %>Queued script <%= queuedDeployment.Script %> in queue <%= queuedDeployment.Queue %> at <%= queuedDeployment.QueuedUtc.ToString() %><br /><%
                                    if (queuedDeployment.StartedUtc.HasValue)
                                    {
                                        %>&gt; Started at <%= queuedDeployment.StartedUtc.Value.ToString() %><br /><%
                                    }
                                    if (queuedDeployment.FinishedUtc.HasValue)
                                    {
                                        %>&gt; Finished at <%= queuedDeployment.FinishedUtc.Value.ToString() %><br /><%
                                    }
                                    if (queuedDeployment.HasErrors)
                                    {
                                        %>&gt;&gt; Script returned errors! %><br /><%
                                    }
                                }
                            }
                        %></td>
                    </tr>
                <%
            }
        %>
    </tbody>
</table>