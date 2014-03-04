$(document).ready(function () {

    $('.DeleteAction').click(function () {

        var position = $("#confirmDeleteDiv").position();
        scroll(0, position.top); 

        $("#confirmDeleteDiv").show();
        return;
    })

    $('.CancelAction').click(function () {
        $("#confirmDeleteDiv").hide();
        return;
    })

    $('.SelectAllAction').click(function () {
        $('.MessageCheckbox').each(function (i, obj) {
            $(this).prop('checked', true);
        });
        return;
    })

    $('.DeSelectAllAction').click(function () {
        $('.MessageCheckbox').each(function (i, obj) {
            $(this).prop('checked', false);
        });
        return;
    })

    $('.ConfirmDeleteAction').click(function () {
        checkedMessageIDListString = "";

        $('.MessageCheckbox').each(function (i, obj) {
            if ($(this).is(':checked')) {
                checkedMessageIDListString += $(this).context.name + ",";
            }
        });

        //remove last ","
        checkedMessageIDListString = checkedMessageIDListString.substring(0, checkedMessageIDListString.length - 1);

        showSpinner();
        $.ajax({
            type: "POST",
            url: '/UserMessages/DeleteMessages',
            data: { messageIDs: checkedMessageIDListString },
            dataType: 'html',
            success: function (data, textStatus, jqxhr) {
                hideSpinner();
                parent.location.reload();
            },
            error: function (jqxhr, textStatus, errorThrown) {
                hideSpinner();
                alert("Message Deletion Error");
            }
        })

        return;
    })

    

});