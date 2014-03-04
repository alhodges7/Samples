
function initializeRatingControl() {
    //tooltip
    $('.rateit').on('over', function (event, value) { $(this).attr('title', value); });

    //Rated Event
    $(".rateit").bind('rated',
    function (e) {
        var ri = $(this);
        var value = ri.rateit('value');
       
        $.ajax({
            url: $(this).attr('data-actionurl'),
            data: { userRating: value, resourceId: $(this).attr('data-resourceId') },
            type: 'POST',
            success: function (data) {
                $("#avgRate").rateit('value', data);
                $(ri).rateit('resetable', true);
            },
            error: AstraSharedScript.ajaxError
        });
    });
    //Reset Event
    $(".rateit").bind('reset',
    function (e) {
        var ri = $(this);
        var value = ri.rateit('value');
        
        $.ajax({
            url: $(this).attr('data-actionurl'),
            data: { userRating: -1, resourceId: $(this).attr('data-resourceId') },
            type: 'POST',
            success: function (data) {
                $("#avgRate").rateit('value', 0);
                $(ri).rateit('resetable', false);
            },
            error: AstraSharedScript.ajaxError
        });
    });
}
