

$(document).ready(function () {

    var repTags = $("[id^=reportLink_]");

    repTags.each(function (i, repTag) {
        var fullId = $(repTag).attr("id");
        var reportId = fullId.split("_")[1];
        $(repTag).click(function (e) { runReport(e, reportId); });
    });

});

function runReport(e, reportId) {
    e.preventDefault();
    var url = "/ASPForms/ReportViewer.aspx?ReportId=" + reportId;
    window.open(url, "AstraReport");
    return true;
}