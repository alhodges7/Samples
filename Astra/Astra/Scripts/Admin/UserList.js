
var targetMID = "";

$(document).ready(function () {
    $('.AdminAction').click(function () {
        var adminActionPrefix = "adminActionButton-";
        var id = $(this)[0].id.substr(adminActionPrefix.length);
        var selectedIndex = $('#actionSelect-' + id)[0].selectedIndex;
        var selection = $('#actionSelect-' + id)[0].value;
        var mid = $(this).attr("data-mid");
        if (selectedIndex == 0) {
            return;
        }
        switch (selection) {
            case "DeleteRatings":
                DeleteRatings(mid)
                return;
            case "ResetPassword":
                parent.location = "/Admin/ResetUserPassword/ProfileId/" + id;
                return;
            case "SendMessage":
                PopSendMessageDialog(mid);
                return;
            case "DeleteUser":
                $("#deletionDiv-" + id).show();
                return;
            case "DeleteReviews":
                DeleteReviews(mid);
                return;
            case "ToggleActive":
                ToggleActivationState(mid,id);
                return false;
            case "ToggleLibrarian":
                ToggleLibrarianState(mid,id);
                return;
            default:
        }
        $('#actionSelect-' + id)[0].selectedIndex = 0;
    })

    $('.confirmDeletion').click(function () {
        var mid = $(this).attr("data-mid");
        var confirmDeletionPrefix = "confirmDeletion-";
        var id = $(this)[0].id.substr(confirmDeletionPrefix.length);
        DeleteUser(mid,id);
    })

    $('#SMD_CancelMessage').click(function () {
        $("#sendMessageDialog").dialog("close");
    })

    $('#SMD_SendMessage').click(function () {
        sendSystemMessage();
    })
});

function PopSendMessageDialog(mid) {

    targetMID = mid;

    $("#sendMessageDialog").dialog();
    return;
}

function DeleteRatings(mid) {
    $.ajax({
        type: "POST",
        url: "Admin/DeleteAllRatingsForUser/",
        data: {usermid: mid },
        success: function (data, textStatus, jqxhr) {
            $(".message").text(data);
        },
        error: function (jqxhr, textStatus, errorThrown) {
            alert(jqxhr.statusText);
            $("#errorDiv").html(jqxhr.responseText);
        }
    })
    return false;
}
function DeleteReviews(mid) {
    $.ajax({
        type: "POST",
        url: "Admin/DeleteAllCommentsForUser/",
        data: { usermid: mid },
        success: function (data, textStatus, jqxhr) {
            $(".message").text(data);
        },
        error: function (jqxhr, textStatus, errorThrown) {
            alert(jqxhr.statusText);
            $("#errorDiv").html(jqxhr.responseText);
        }
    })
}
function ToggleActivationState(mid,id) {
    $.ajax({
        type: "POST",
        url: "Admin/ToggleUserActiveState/?usermid=" + mid,
        data: {},
        success: function (data, textStatus, jqxhr) {
            $(".message").text(data);
            var option = $("#actionSelect-" + id);
            var selectedIndex = $('#actionSelect-' + id)[0].selectedIndex;
            if (option[0][selectedIndex].text == "Activate") {
                option[0][selectedIndex].text = "Deactivate";
            }
            else {
                option[0][selectedIndex].text = "Activate";
            }
            $('#actionSelect-' + id)[0].selectedIndex = 0;
        },
        error: function (jqxhr, textStatus, errorThrown) {
            alert(jqxhr.statusText);
            $("#errorDiv").html(jqxhr.responseText);
        }
    })
}
function ToggleLibrarianState(mid,id) {
    $.ajax({
        type: "POST",
        url: "Admin/ToggleLibrarianPermission/",
        data: { usermid: mid },
        success: function (data, textStatus, jqxhr) {
            $(".message").text(data);
            var option = $("#actionSelect-" + id);
            var selectedIndex = $('#actionSelect-' + id)[0].selectedIndex;
            if (option[0][selectedIndex].text == "Promote") {
                option[0][selectedIndex].text = "Demote";
            }
            else {
                option[0][selectedIndex].text = "Promote";
            }
            $('#actionSelect-' + id)[0].selectedIndex = 0;
        },
        error: function (jqxhr, textStatus, errorThrown) {
            alert(jqxhr.statusText);
            $("#errorDiv").html(jqxhr.responseText);
        }
    })
}
function DeleteUser(mid,id) {
    $.ajax({
        type: "POST",
        url: "Admin/DeleteUser/",
        data: { usermid: mid },
        success: function (data, textStatus, jqxhr) {
            $(".message").text(data);
            $("#row-" + id).remove();
        },
        error: function (jqxhr, textStatus, errorThrown) {
            alert(jqxhr.statusText);
            $("#errorDiv").html(jqxhr.responseText);
        }
    })
}

function sendSystemMessage() {

    var messageText = $("#systemMessageText").val();

    if (!messageText) {
        alert("Please enter a valid message.");
        return;
    }

    $.ajax({
        type: "POST",
        url: "/Admin/SystemMessageCreateForUser/?mid=" + targetMID,
        data: messageText,
        success: function (data, textStatus, jqxhr) {
            if (data == "SUCCESS") {
                $("#sendMessageDialog").dialog("close");
                $("#systemMessageText").val("");
                alert("Your message has been sent.");
            }
            else
                alert(data);
        },
        error: function (jqxhr, textStatus, errorThrown) {
            alert(jqxhr.statusText);
            $("#errorDiv").html(jqxhr.responseText);
        }
    })

}


