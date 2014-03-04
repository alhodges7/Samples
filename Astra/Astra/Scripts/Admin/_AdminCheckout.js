var AdminCheckoutScript = new function () {
    return {
        /* functions */
        initialize: function()
        {
            // Currently no initialization code
        },

        //Take Resource Id, Selected User Mid, and Send those parameters to CheckOut Action
        CheckOut: function (resourceId) {
            $("#selectedResource")[0].value = resourceId;

            var mid = $("#Users").val();
            $.ajax({
                type: "POST",
                url: '/CheckOuts/AdminCheckOut',
                data: { resourceId: resourceId, mId: mid },
                dataType: 'html',
                success: function (data, textStatus, jqxhr) {
                    window.parent.jQuery('.chkOut').dialog('close');
                    parent.location.reload();
                },
                error: function (jqxhr, textStatus, errorThrown) {
                    alert("Error");
                    $("#errorDiv").html(jqxhr.responseText);
                }
            })
        },
        //Take Resource Id, Selected User Mid, and Send those parameters to CheckIn Action
        CheckIn: function (resourceId) {
            $("#selectedResource")[0].value = resourceId;

            var mid = $("#Users").val();
            $.ajax({
                type: "POST",
                url: '/CheckOuts/AdminCheckIn',
                data: { resourceId: resourceId, mId: mid },
                dataType: 'html',
                success: function (data, textStatus, jqxhr) {
                    window.parent.jQuery('.chkIn').dialog('close');
                    parent.location.reload();
                },
                error: function (jqxhr, textStatus, errorThrown) {
                    alert("Error");
                }
            });
        }
    }
}();

$(document).ready(function () {
    AdminCheckoutScript.initialize();
});