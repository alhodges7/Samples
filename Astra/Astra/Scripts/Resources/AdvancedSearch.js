$(document).ready(function () {

    $('#selectResource').change(function (e) {

        var resourcetypeID = $(this).find('option:selected').val();
        if (resourcetypeID == "") {
            $("#searchfields").html("");
        }
        else {
            showSpinner();
            $.ajax({
                url: $('#searchfields').attr('data-actionurl'),
                data: { resourcetypeID: resourcetypeID },
                type: 'GET',
                success: function (data) {
                    hideSpinner();
                    $("#searchfields").html(data);
                },
                error: AstraSharedScript.ajaxError
            });
        }

    })
});