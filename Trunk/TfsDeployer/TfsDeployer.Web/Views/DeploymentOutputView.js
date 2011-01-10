window.setInterval(function () {
    $('#DeploymentOutput').load(applicationPath + 'Partials/DeploymentOutput.aspx?deploymentid=' + $.query.get('deploymentid') + ' #DeploymentOutput *');
}, 5000);