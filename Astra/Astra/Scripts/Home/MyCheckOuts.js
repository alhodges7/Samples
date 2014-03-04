var MyCheckOutsScript = new function () {
    return {
        initialize: function () {
            $(".btn_checkin").click(function () {
                $.ajax({
                    type: "POST",
                    url: $(this).attr("data-actionurl"),
                    success: function (data, textStatus, jqxhr) {
                        parent.location.reload();
                    },
                    error: AstraSharedScript.ajaxError
                });
            });

            $(".btn_cancelReservation").click(function () {
                $.ajax({
                    type: "POST",
                    url: $(this).attr("data-actionurl"),
                    success: function (data, textStatus, jqxhr) {
                        parent.location.reload();
                    },
                    error: AstraSharedScript.ajaxError
                });
            });
        }
    }
}();

$(document).ready(function () {
    MyCheckOutsScript.initialize();
});