var SummaryScript = new function () {
    return {
        initialize: function() {
            $("#btn_checkout").click(function () {
                showSpinner();
                $.ajax({
                    type: "POST",
                    url: $(this).attr("data-actionurl"),
                    success: function (data, textStatus, jqxhr) {
                        hideSpinner();
                        parent.location.reload();
                    },
                    error: AstraSharedScript.ajaxError
                });
            });

            $("#btn_checkin").click(function () {
                showSpinner();
                $.ajax({
                    type: "POST",
                    url: $(this).attr("data-actionurl"),
                    success: function (data, textStatus, jqxhr) {
                        hideSpinner();
                        parent.location.reload();
                    },
                    error: AstraSharedScript.ajaxError
                });
            });

            $("#btn_reserve").click(function () {
                showSpinner();
                $.ajax({
                    type: "POST",
                    url: $(this).attr("data-actionurl"),
                    success: function (data, textStatus, jqxhr) {
                        hideSpinner();
                        parent.location.reload();
                    },
                    error: AstraSharedScript.ajaxError
                });
            });

            $("#btn_cancelReservation").click(function () {
                showSpinner();
                $.ajax({
                    type: "POST",
                    url: $(this).attr("data-actionurl"),
                    success: function (data, textStatus, jqxhr) {
                        hideSpinner();
                        parent.location.reload();
                    },
                    error: function (data, textStatus, jqxhr) {
                        alert("This reservation may no longer exists. The book may have been checked out to you automatically if a copy became available.")
                        parent.location.reload();
                    }
                });
            });

            $(".btn_addComment").click(function () {
                if (!SummaryPageVars.userIsAuthenticated) {
                    // NOTE: Unauthenticated user should never encounter an add comment box.
                    alert("You must be logged in to add a comment.");
                }
                else {
                    $(".addCommentBlock").show();
                }
            });

            $(".btn_editComment").click(function () {
                var buttonIdPrefix = "btn_editComment-";
                var buttonId = $(this)[0].id.substr(buttonIdPrefix.length);

                if (!SummaryPageVars.userIsAuthenticated) {
                    // NOTE: Unauthenticated user should never encounter an edit box.
                    alert("You must be logged in to edit a comment.");
                }
                else {
                    $("#editBlock-" + buttonId).show();
                    $(this).hide();
                }
            });

            $(".btn_removeRating").click(function () {
                showSpinner();
                var id = $(this)[0].id;
                $.ajax({
                    type: "POST",
                    url: $(this).attr("data-actionurl"),
                    success: function (data, textStatus, jqxhr) {
                        hideSpinner();
                        parent.location.reload();
                    },
                   
                    error: AstraSharedScript.ajaxError
                });
            });

            
            $(".addCommentValidation").click(function () {
                for (instance in CKEDITOR.instances) {
                    CKEDITOR.instances[instance].updateElement();
                }
                if (CKEDITOR.instances[instance].getData() == '')
                 {
                    alert("Emtpy review comments are not allowed.");
                    return false;
                }
                else
                    return true;
            });

            $(".commentCancel").click(function () {
                var buttonId = $(this)[0].id.substr(14);
                $(".addCommentBlock").hide();
                $("#editBlock-" + buttonId).hide();
                if (SummaryPageVars.editModeEnabled) {
                    $("#btn_editComment-" + buttonId).show();
                    var commentToEdit = $(".ckeditor")[0].value;
                    var decoded = $('<div/>').html(commentToEdit);
                    $(".ckeditor").val(decoded[0].innerText);
                }
                else {
                    for ( instance in CKEDITOR.instances ) {
                        CKEDITOR.instances[instance].updateElement();
                    }
                    CKEDITOR.instances[instance].setData('');
                }
            });

            $(".btn_removeComment").click(function () {
                var $this = $(this);

                var url = $this.attr('data-actionurl');
                showSpinner();
                $.ajax({
                    url: url,
                    success: function (data, textStatus, jqXHR) {
                        hideSpinner();
                        location.reload(true);
                    },
                    error: AstraSharedScript.ajaxError
                });
            });

            SummaryScript.processImageActions();
            //rating stuff
            initializeRatingControl();

            SummaryScript.initializeDialogs();
        },

        processImageActions: function() {
            var fancy = $(".fancybox");
            if (fancy.length > 0)
            {
                fancy.fancybox();
            }
        },

        initializeDialogs: function() {
        },
        
        addCommentComplete: function (data, textStatus, jqXHR) {
            location.reload(true);
        },

        submitCommentEdit: function (data, textStatus, jqXHR) {
            location.reload(true);
        }
    };
}();

var SummaryPageVars = new function () {
    return {
        userIsAuthenticated: false,
        numberOfUserComments: 0,
        editModeEnabled: false,
        reservationsAvailable: 0
    }
}();

$(document).ready(function () {
    SummaryScript.initialize();
});