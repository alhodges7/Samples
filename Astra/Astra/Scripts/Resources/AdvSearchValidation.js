$(document).ready(function () {

    $("#btn_advancedSearchBookSubmit").click(function () {
        if ($("#Title").val() == ""
            && $("#Author").val() == ""
            && $("#ISBN").val() == ""
            && $("#skillLevelList").val() == undefined
            && $("#keywordsList").val() == null) {
            alert("At least select a keyword to do an advanced search on books.");
            return false;
        }
        else {
            return true;
        }
    });
    $("#btn_advancedSearchDVDSubmit").click(function () {
        if ($("#Title").val() == ""
            && $("#Directors").val() == ""
            && $("#Language").val() == ""
            && $("#Studio").val() == ""
            && $("#keywordsList").val() == null) {
            alert("At least select a keyword to do an advanced search on DVDs.");
            return false;
        }
        else {
            return true;
        }
    });
    $("#btn_advancedSearchEBookSubmit").click(function () {
        if ($("#Title").val() == ""
            && $("#Author").val() == ""
            && $("#ISBN").val() == ""
            && $("#keywordsList").val() == null) {
            alert("At least select a keyword to do an advanced search on a EBook.");
            return false;
        }
        else {
            return true;
        }
    });
    $("#btn_advancedSearchHardwareSubmit").click(function () {
        if ($("#Title").val() == ""
            && $("#Modelno").val() == ""
            && $("#Description").val() == ""
            && $("#keywordsList").val() == null) {
            alert("At least select a keyword to do an advanced search on Hardware.");
            return false;
        }
        else {
            return true;
        }
    });
    $("#btn_advancedSearchSoftwareSubmit").click(function () {
        if ($("#Title").val() == ""
            && $("#Description").val() == ""
            && $("#keywordsList").val() == null) {
            alert("At least select a keyword to do an advanced search on Software.");
            return false;
        }
        else {
            return true;
        }
    });
    $("#btn_advancedSearchWhitePaperSubmit").click(function () {
        if ($("#Title").val() == ""
            && $("#Description").val() == ""
            && $("#keywordsList").val() == null) {
            alert("At least select a keyword to do an advanced search on White Papers.");
            return false;
        }
        else {
            return true;
        }
    });

});
