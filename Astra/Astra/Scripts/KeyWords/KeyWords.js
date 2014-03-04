

function confirmKeyWordDelete(keyWord, keyWordId) {

    if (!confirm("Are you sure want to delete the Key Word '" + keyWord + "' and all of its references?"))
        return;

    var deleteActionUrl = $("#DataIsland1").val() + "/" + keyWordId;
    location = deleteActionUrl;   

}


function MergeKeyWord(keyWordId) {

    var intoId = $("#MergeIntoKeyWordList").val();

    if (!intoId) {
        alert("Please select the Key Word to merge into.");
        return;
    }

    
    var mergeActionUrl = $("#DataIsland2").val() + "?Id=" + keyWordId + "&intoId=" + intoId;
    alert(mergeActionUrl);
    location = mergeActionUrl;


}