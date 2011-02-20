<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventListView.ascx.cs" Inherits="TfsDeployer.Web.Views.EventListView" %>
<%@ Import Namespace="System.Linq" %>
<table>
    <thead>
        <tr>
            <td>Build Number</td>
            <td>Original Quality</td>
            <td>New Quality</td>
            <td>Triggered At</td>
            <td>Triggered By</td>
            <td>Status</td>
        </tr>
    </thead>
    <tbody>
        <%
            foreach(var recentEvent in Model.RecentEvents)
            {
                %>
                    <tr class="expanding-row">
                        <td class=""><%= recentEvent.BuildNumber %></td>
                        <td class=""><%= recentEvent.OriginalQuality %></td>
                        <td class=""><%= recentEvent.NewQuality %></td>
                        <td class=""><%= recentEvent.TriggeredUtc.ToString() %></td>
                        <td class=""><%= recentEvent.TriggeredBy %></td>
                        <td class="" ><%
                                if (recentEvent.QueuedDeployments.Count(d => d.FinishedUtc == null && d.StartedUtc.HasValue) > 0)
                                {
                                    %><a href="<%= ResolveUrl("~/ShowDeployment.aspx?deploymentid=" + recentEvent.QueuedDeployments.Where(d=> !d.FinishedUtc.HasValue && d.StartedUtc.HasValue).Max(d => d.Id)) %>">In Progress</a><%
                                }
                                else if (recentEvent.QueuedDeployments.Count(d => d.FinishedUtc == null && d.StartedUtc == null) > 0)
                                {
                                    %><a href="<%= ResolveUrl("~/ShowDeployment.aspx?deploymentid=" + recentEvent.QueuedDeployments.Where(d => !d.FinishedUtc.HasValue && !d.StartedUtc.HasValue).Max(d => d.Id)) %>">No Action Required</a><%
                                }
                                else if (recentEvent.QueuedDeployments.Count(d => d.FinishedUtc.HasValue) > 0)
                                {
                                    %><a href="<%= ResolveUrl("~/ShowDeployment.aspx?deploymentid=" + recentEvent.QueuedDeployments.Where(d => d.FinishedUtc.HasValue).Max(d => d.Id))%>">Deployed</a><%
                                }%>                                 
                        </td>
                    </tr>
                    <tr id="<%=recentEvent.BuildNumber%>" class="reveal-row">
                        <td colspan="6">
                            <div> 
                                <% foreach(var deployment in recentEvent.QueuedDeployments)
                                {%>
                                    <p class="buildDetail">Deployed at <%=deployment.FinishedUtc %> using the script <%=deployment.Script%> </p>
                                  <%
                                }%>
                            </div>
                        </td>
                    </tr>
                <%
            }
        %>
    </tbody>
</table>
