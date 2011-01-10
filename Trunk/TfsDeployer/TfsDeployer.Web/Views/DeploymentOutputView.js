var deploymentOutputLoadIntervalId = 0;

var deploymentOutputLoad = function () {
    var isFinal = $('#DeploymentOutput .IsFinal').length != 0;
    console.log(isFinal);
    if (isFinal) {
        window.clearInterval(deploymentOutputLoadIntervalId);
        return;
    }

    var url = applicationPath + 'Partials/DeploymentOutput.aspx?deploymentid=' + $.query.get('deploymentid');
    $('#DeploymentOutput').load(url + ' #DeploymentOutput *');
}

deploymentOutputLoadIntervalId = window.setInterval(deploymentOutputLoad, 5000);