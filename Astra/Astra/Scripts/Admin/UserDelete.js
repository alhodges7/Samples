$(document).ready(function () {
    $('#deleteButton').click(function () {
        var UserDeleteBoxText = $('#UserDeleteBox')[0].value;
        $.ajax({
            type: "POST",
            url: $(this).attr('data-actionurl') + '/?deletionString=' + UserDeleteBoxText,
            success: function (data, textStatus, jqxhr) {
                if (data.ResultSuccessful == true) {
                    alert("User sucessfully deleted.");
                    parent.location.reload();
                }
                else {
                    alert("Warning: user was not successfully deleted");
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                alert(jqxhr.statusText);
                $("#errorDiv").html(jqxhr.responseText);
            }
        })
    })
});

