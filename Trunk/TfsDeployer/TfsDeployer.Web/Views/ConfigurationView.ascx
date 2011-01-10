<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigurationView.ascx.cs" Inherits="TfsDeployer.Web.Views.ConfigurationView" %>

<form action="<%= Request.CurrentExecutionFilePath %>" method="post">
    <fieldset>
        <legend>Web Management Configuration</legend>
        <ol>
            <li>
                <label for="DeployerServiceUrl">TFS Deployer service endpoint:</label>
                <input type="text" name="DeployerServiceUrl" value="<%# Model.DeployerServiceUrl %>" />
            </li>
            <li>
                <input type="submit" name="SaveButton" value="Save" />
            </li>
        </ol>
    </fieldset>
</form>