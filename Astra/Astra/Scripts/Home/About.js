
$(document).ready(function() {

    var helpLink = $("#launchAstraHelp");

    helpLink.click(function () {
        openAstraWiki();
    });
    
});


function openAstraWiki() {
    var helpLink = $("#launchAstraHelp");
    var wikiUrl = helpLink.attr("data-targetUrl");
    window.open(wikiUrl, "AstraWiki");
}