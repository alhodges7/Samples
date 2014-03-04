$(document).ready(function () {
    $('#hdnResultsPerPage').val($("#ddlResultsPerPage").val());
    $('#hdnSortBy').val($("#ddlSortBy").val());
    IndexPageScript.initialize();

    $("#ddlResultsPerPage").change(function () {
        var NumResultsPerPage = $(this).val();        
        var sort = $('#hdnSortBy').val();
        var nextPageUrl = "<a href=\"/Resources?page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort + "\">»</a>";

        if (hdnBrowsingFrom.val == "KeywordSearch")
        {
            nextPageUrl = "<a href=\"/Resources/QuickSearch?keyWords=" + IndexPageVars.keywordSearchText + "&page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort + "\">»</a>";
            window.location = "/Resources/QuickSearch?keyWords=" + IndexPageVars.keywordSearchText +"&page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort;
        }
        else if (hdnBrowsingFrom.val == "AdvancedSearch") {
            nextPageUrl = "<a href=\"/Resources/AdvSearchUsePrevious?" + "page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort + "\">»</a>";
            window.location = "/Resources/AdvSearchUsePrevious?page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort;
        }
        else {            
                window.location = "/Resources?page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort;
            }
        
    });

    $("#ddlSortBy").change(function () {
        var sort = $(this).val();
        var NumResultsPerPage = $('#hdnResultsPerPage').val();
        var nextPageUrl = "<a href=\"/Resources?page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort + "\">»</a>";

        if (hdnBrowsingFrom.val == "KeywordSearch") {
            nextPageUrl = "<a href=\"/Resources/QuickSearch?keyWords=" + IndexPageVars.keywordSearchText + "&page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort + "\">»</a>";
            window.location = "/Resources/QuickSearch?keyWords=" + IndexPageVars.keywordSearchText + "&page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort;
        }
        else if (hdnBrowsingFrom.val == "AdvancedSearch") {
            nextPageUrl = "<a href=\"/Resources/AdvSearchUsePrevious?" + "page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort + "\">»</a>";
            window.location = "/Resources/AdvSearchUsePrevious?page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort;
        }
        else {
            window.location = "/Resources?page=1&NumResultsPerPage=" + NumResultsPerPage + "&sortBy=" + sort;
        }

    });
});


function jumpToSummary(e, resourceId) {
    e.preventDefault();
    location = "/Resources/Summary?resourceId=" + resourceId;
    return true;
}

var IndexPageScript = new function () {
    return {
        initialize: function() {
            var imgTags = $("[id^=resourceImage_]");

            imgTags.each(function (i, imgTag) {
                var fullId = $(imgTag).attr("id");
                var resourceId = fullId.split("_")[1];
                $(imgTag).click(function (e) { jumpToSummary(e, resourceId); });
            });

            if (IndexPageVars.keywordSearchText !== "") {
                $("#KWB_KeyWordsToSearch").val(IndexPageVars.keywordSearchText);
            }
        },
        jumpToSummary: function(e, resourceId) {
            e.preventDefault();
            location = "/Resources/Summary?resourceId=" + resourceId;
            return true;
        }
    }
}();

var IndexPageVars = new function () {
    return {
        keywordSearchText: "",
        CurrentlyBrowsingFrom: ""
    }
}();