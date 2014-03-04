$(document).ready(function () {
    $(".Approved").css("background-color", "#bdFFbd");
    $(".Pending").css("background-color", "#FFFFA0");
    $(".Rejected").css("background-color", "#FF8282");

    //this function makes the suggestion grid an AJAX grid,
    //but if javascript is turned off, this won't fire 
    //and the standard server paging will continue to function
    function updateGrid(e) {
        e.preventDefault();
        // find the containing element's id then reload the grid
        var url = $(this).attr('href');
        var grid = $(this).parents('.ajaxGrid'); // get the grid
        var id = grid.attr('id');
        grid.load(url + ' #' + id);
    };

    $(document).on("click", ".ajaxGrid table thead tr a", updateGrid); // hook up ajax refresh for sorting links
    $(document).on("click", ".ajaxGrid table tfoot tr a", updateGrid); // hook up ajax refresh for paging links (note: this doesn't handle the separate Pager() call!)


    $('.MoreDetailsAction').click(function () {
        var editActionPrefix = "moreDetailsActionButton-";
        var id = $(this)[0].id.substr(editActionPrefix.length);
        $("#shortDisplayDiv-" + id).hide();
        $("#displayDiv-" + id).show();
        $("#editDiv-" + id).hide();
        $("#leftButtonPanelDiv-" + id).show();
        $("#rightButtonPanelDiv-" + id).show();

        var position = $("#displayDiv-" + id).position();
        scroll(0, position.top);

        $("#deleteDiv-" + id).hide();
        return;
    })

    $('.LessdetailsAction').click(function () {
        var editActionPrefix = "lessdetailsActionButton-";
        var id = $(this)[0].id.substr(editActionPrefix.length);
        $("#shortDisplayDiv-" + id).show();
        $("#displayDiv-" + id).hide();
        $("#editDiv-" + id).hide();

        var position = $("#displayDiv-" + id).position();
        scroll(0, position.top);

        $("#deleteDiv-" + id).hide();
        return;
    })


    $('.EditAction').click(function () {
        var id = $(this).attr("data-id");
        $("#shortDisplayDiv-" + id).hide();
        $("#displayDiv-" + id).hide();
        $("#editDiv-" + id).show();

        var position = $("#editDiv-" + id).position();
        scroll(0, position.top);

        $("#deleteDiv-" + id).hide();
        return;
    })


    $('.QuickDeleteAction').click(function () {
        var deleteActionPrefix = "quickDeleteActionButton-";
        var id = $(this)[0].id.substr(deleteActionPrefix.length);
        $("#shortDisplayDiv-" + id).hide();
        $("#displayDiv-" + id).show();
        $("#leftButtonPanelDiv-" + id).hide();
        $("#rightButtonPanelDiv-" + id).hide();

        var position = $("#displayDiv-" + id).position();
        scroll(0, position.top);

        $("#editDiv-" + id).hide();
        $("#deleteDiv-" + id).show();
        return;
    })


    $('.DeleteAction').click(function () {
        var deleteActionPrefix = "deleteActionButton-";
        var id = $(this)[0].id.substr(deleteActionPrefix.length);
        $("#shortDisplayDiv-" + id).hide();
        $("#displayDiv-" + id).show();

        var position = $("#displayDiv-" + id).position();
        scroll(0, position.top);

        $("#editDiv-" + id).hide();
        $("#deleteDiv-" + id).show();
        return;
    })

    $('.CancelAction').click(function () {
        var id = $(this).attr("data-id");
        $("#shortDisplayDiv-" + id).show();
        $("#displayDiv-" + id).hide();
        $("#editDiv-" + id).hide();
        $("#deleteDiv-" + id).hide();
        return;
    })

    $('.GridEditCancelAction').click(function () {
        var id = $(this).attr("data-id");
        $("#displayDiv-" + id).show();
        $("#editDiv-" + id).hide();
        $("#deleteDiv-" + id).hide();
        return;
    })
    
    $('.UserDeleteAction').click(function () {
        var userDeleteActionPrefix = "userDeleteActionButton-";
        var id = $(this)[0].id.substr(userDeleteActionPrefix.length);
        $("#userDeleteDiv-" + id).show();

        var position = $("#userDisplayDiv-" + id).position();
        scroll(0, position.top);

        return;
    })

    $('.UserCancelAction').click(function () {
        var userCancelActionPrefix = "userCancelActionButton-";
        var id = $(this)[0].id.substr(userCancelActionPrefix.length);
        $("#userDeleteDiv-" + id).hide();
        return;
    })


    $('.ApproveAction').click(function () {
        showSpinner();
        $.ajax({
            type: "POST",
            url: $(this).attr("data-actionurl"),
            success: function (data, textStatus, jqxhr) {
                hideSpinner();
                parent.location.reload();
            },
            error: function (jqxhr, textStatus, errorThrown) {
                hideSpinner();
                alert("Suggestion Approval Error");
            }
        });
    })

    $('.RejectAction').click(function () {
        showSpinner();
        $.ajax({
            type: "POST",
            url: $(this).attr("data-actionurl"),
            success: function (data, textStatus, jqxhr) {
                hideSpinner();
                parent.location.reload();
            },
            error: function (jqxhr, textStatus, errorThrown) {
                hideSpinner();
                alert("Suggestion Rejection Error");
            }
        });
    })

    //hook up the back to index button unobtrusively
    $(".backToSuggestionIndexButton").click(function () {
        var $this = $(this);

        location.href = $this.attr("data-link");
    });
});