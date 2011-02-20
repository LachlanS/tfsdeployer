/// <reference path="jquery-1.4.4.js" />

$(function () {
    $('.expanding-row').click(function (e) {
        if (e.srcElement.nodeName === 'A') return;

        $(this).next().toggle();
    });
});

$(function () {
    $('#DeploymentOutput').each(function () {
        var output = $(this);
        var deploymentOutputLoad = function () {
            var isFinal = output.find('.is-final').length != 0;
            var url = applicationPath + 'Partials/DeploymentOutput.aspx?deploymentid=' + $.query.get('deploymentid');
            output.load(
                url + ' #DeploymentOutput *',
                null,
                function () {
                    if (!isFinal) {
                        window.setTimeout(deploymentOutputLoad, 5000);
                    }
                }
            );
        };

        deploymentOutputLoad();
    });
});

