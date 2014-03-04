$(document).ready(function () {
    $(".chkOut").dialog({
        autoOpen: false,
        title: 'Select Employee',
        width: 350,
        height: 300,
        resizable: false,
        modal: true
    });
    $(".chkIn").dialog({
        autoOpen: false,
        title: 'Select Employee',
        width: 350,
        height: 300,
        resizable: false,
        modal: true
    });

    $(".admCheckOut").click(
        function () {
            var resourceId = this.id.substring(this.id.lastIndexOf('-') + 1)
            var $fileUploaderIFrame = $("#adminCheckOutIframe-"+resourceId);
            
            $fileUploaderIFrame.attr('src', $fileUploaderIFrame.attr("data-src"));

            AstraSharedScript.openDialog("chkOut-" + resourceId, 450, 250, "Check Out:");
        }
    );

    $(".admCheckIn").click(
        function () {
            var resourceId = this.id.substring(this.id.lastIndexOf('-') + 1)
            var $fileUploaderIFrame = $("#adminCheckInIframe-" + resourceId);

            $fileUploaderIFrame.attr('src', $fileUploaderIFrame.attr("data-src"));
            AstraSharedScript.openDialog("chkIn-" + resourceId, 450, 250, "Check In:");
        }
    );
});