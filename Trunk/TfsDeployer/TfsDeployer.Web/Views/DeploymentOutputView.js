window.setInterval(function () {
    $('#DeploymentOutput').load('Partials/DeploymentOutput.aspx?deploymentid=' + $.query.get('deploymentid') + ' #DeploymentOutput *');
}, 5000);