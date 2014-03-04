$(document).ready(function () {
    $('.btnCheck').click(function () {
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
    $('#createNewResource').click(function () {
        $(".createType").css("display", "block");
    });
    $('#ResourceTypes').change(function () {
        $(this).se
        var createResourceType = $("#ResourceTypes option:selected").text();
        if (createResourceType != null) {

            document.getElementById("ResourceTypes").selectedIndex = -1;
            window.location.href = "/Admin/Create/?resourceType=" + createResourceType;

        }

    });



});
