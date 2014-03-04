$(document).ready(function () {
    $('.btnCheckIn').click(function () {
        $.ajax({
            type: "POST",
            url: $.ajax({
                type: "POST",
                url: $(this).attr('data-actionurl'),
                success: function (data, textStatus, jqxhr) {
                    parent.location.reload();
                },
                error: function (jqxhr, textStatus, errorThrown) {
                    alert("Error");
                    $("#errorDiv").html(jqxhr.responseText);
                }
            })
        })
    });

    $('.btnCheckOut').click(function () {
        var ctrlId = $(this)[0].id.substr(11);
        var selectedResourceId = $("#ctrlResources-" + ctrlId)[0].value;
        $.ajax({
            type: "POST",
            url: $.ajax({
                type: "POST",
                url: $(this).attr('data-actionurl') + "&resourceId=" + selectedResourceId,
                success: function (data, textStatus, jqxhr) {
                    parent.location.reload();
                },
                error: function (jqxhr, textStatus, errorThrown) {
                    alert("Error");
                    $("#errorDiv").html(jqxhr.responseText);
                }
            })
        })
    });
    $(".AvailableResources").change(function () {
        var ctrlPrefix = "ctrlResources-";
        var id = $(this)[0].id.substr(ctrlPrefix.length);
        if ($(this)[0].value > 0)
            $("#ctrlChkOut-" + id).removeAttr("disabled", "disabled");
        else
            $("#ctrlChkOut-" + id).attr("disabled", "disabled");
    });

    $('.cancelDeletion').click(function () {
        var anchorPrefix = "cancelDeletion-";
        var id = $(this)[0].id.substr(anchorPrefix.length);
        $("#deletionDiv-" + id).hide();
        var actionDDL = $("#actionSelect-" + id);
        $(actionDDL)[0].selectedIndex = 0;
    });
});
