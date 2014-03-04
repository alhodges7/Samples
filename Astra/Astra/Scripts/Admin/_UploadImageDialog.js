
$(document).ready(function () {

    $("#uploadForm").submit(
        function () {
            var $this = jQuery(this);

            var uploadControl = $("#FileUpload");

            var targetFilename = uploadControl.val().toLowerCase();

            if (targetFilename.length < 4) {
                alert("Please selected a valid image file.");
                return false;
            }
            if (targetFilename.substr(targetFilename.length - 4, 4) != ".jpg"
                    && targetFilename.substr(targetFilename.length - 4, 4) != ".jpeg"
                    && targetFilename.substr(targetFilename.length - 4, 4) != ".gif"
                    && targetFilename.substr(targetFilename.length - 4, 4) != ".png") {
                alert("Only the following image types are supported: PNG, JPG, GIF");
                return false;
            }
            else {
                return true;
            }
        }
    );
});
