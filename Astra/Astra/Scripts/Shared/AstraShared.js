var AstraSharedScript = new function () {
    var spinnerVisible = false;

    return {
        showLoader: function () {
            if (!spinnerVisible) {
                $("div#spinner").fadeIn("fast");
                spinnerVisible = true;
            }
        },

        hideLoader: function() {
            if (spinnerVisible) {
                var spinner = $("div#spinner");
                spinner.stop();
                spinner.fadeOut("fast");
                spinnerVisible = false;
            }
        },
        openDialog: function (id, width, height, title) {
            var $dialog = $("#" + id);

            $dialog.dialog({
                width: width,
                height: height,
                title: title
            });
            $dialog.dialog("open");
        },
        ajaxError: function (jqXHR, textStatus, errorThrown) {
            alert("Ajax Error");
        }
    }
}();