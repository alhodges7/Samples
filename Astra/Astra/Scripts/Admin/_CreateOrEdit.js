
$(document).ready(function () {
    $("#imageUploadDiv").dialog({
        autoOpen: false,
        title: 'Image Uploader',
        width: 360,
        height: 170,
        resizable: false,
        modal: true
    });
    $('#date').datepicker({ changeYear: true, changeMonth: true, dateFormat: "mm/dd/yy" });
    processImageActions();

    $("#btn_coverImageUpload").click(
        function () {
            setDialogCoverImage();
            var $fileUploaderIFrame = $("#fileUploaderIFrame");
            $fileUploaderIFrame.attr('src', $fileUploaderIFrame.attr("data-coverImageSrc"));
            $("#imageUploadDiv").dialog("open");
        }
    );

    $("#btn_imageUpload").click(
        function () {
            setDialogAdditionalImages();
            var $fileUploaderIFrame = $("#fileUploaderIFrame");
            $fileUploaderIFrame.attr('src', $fileUploaderIFrame.attr("data-additionalImageSrc"));
            $("#imageUploadDiv").dialog("open");
        }
    );

    $(".AdminSectionToggleButton").click(
        function () {
            var imgTag = $(this);
            //alert(imgTag.attr("src"));
            var sectionId = imgTag.attr("id").split("_")[1];

            toggleAdminSection(sectionId);
        }
    );


});

function onFailure(response) {
    var x = 5;
}

function processFileUpload(result) {
    var x = 5;
}

function setDialogCoverImage() {
    $("#imageUploadDiv").dialog('option', 'title', 'Upload Cover Image');
}

function setDialogAdditionalImages() {
    $("#imageUploadDiv").dialog('option', 'title', 'Upload Additional Images');
}

function processImageUploadComplete(isCoverImage) {
    $("#imageUploadDiv").dialog("close");
    if (isCoverImage) {
        refreshCoverImage();
    }
    else {
        refreshImages();
    }
}

function refreshCoverImage() {
    GetCoverImage(resourceId);
}

function refreshImages() {
    GetImageList(resourceId);
}

function getImagesCallback() {
    processImageActions();
}

function getCoverImageCallback() {
    processImageActions();

    $.ajax({
        url: getCoverImageIdUrl,
        type: 'GET',
        data: { resourceId: resourceId },
        success: function (data, textStatus, jqXHR) {
            $("#CoverImageId").val(data.ID);
        },
        error: ajaxError
    });
}

function processImageActions() {
    $(".btn_imgDelete").off('click').on('click', function () {
        var $btn = $(this);

        var numberStartIndex = "imgDelete_".length;

        var strTargetImageId = $btn[0].id.substr(numberStartIndex, $btn[0].id.length - numberStartIndex);
        var targetImageId = Number(strTargetImageId);

        DeleteImage(resourceId, targetImageId);
    });

    $(".btn_coverImgDelete").off('click').on('click', function () {
        var $btn = $(this);

        DeleteCoverImage(resourceId);
    });

    var fancy = $(".fancybox");
    if (fancy.length > 0)
    {
        fancy.fancybox();
    }
}

function deleteCoverImageCompleteCallback() {
    refreshCoverImage();
}

function deleteImageCompleteCallback() {
    refreshImages();
}

function GetImageList(resourceId)
{
    $.ajax({
        url: getImagesUrl,
        data: { resourceId: resourceId },
        success: function(data, textStatus, jqXHR)
        {
            $("#imageList").html(data);
            getImagesCallback();
        },
        error: ajaxError
    });
}

function GetCoverImage(resourceId)
{
    $.ajax({
        url: getCoverImageUrl,
        data: { resourceId: resourceId },
        success: function (data, textStatus, jqXHR) {
            $("#coverImageDiv").html(data);
            getCoverImageCallback();
        },
        error: ajaxError
    });
}

function DeleteImage(resourceId, imageId)
{
    $.ajax({
        url: deleteImageUrl,
        data: { imageId: imageId },
        success: deleteImageCompleteCallback,
        error: ajaxError
    });
}

function DeleteCoverImage(resourceId)
{
    $.ajax({
        url: deleteCoverImageUrl,
        data: { resourceId: resourceId },
        success: deleteCoverImageCompleteCallback,
        error: ajaxError
    });
}

function ajaxError(jqXHR, textStatus, errorThrown) {
    alert("Ajax Error");
}

function toggleAdminSection(sectionId) {

    $("#" + sectionId).toggle();

    var expanderImg = $("#Toggle_" + sectionId);

    var src = expanderImg.attr("src");
    if (src.indexOf("plus") > -1)
        src = src.replace("plus", "minus");
    else
        src = src.replace("minus", "plus");

    expanderImg.attr("src", src);

}

