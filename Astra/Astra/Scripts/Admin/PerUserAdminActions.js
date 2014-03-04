$(document).ready(function () {
 
    //Generic Button
    $('.genButton').click(function () {
        
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
    //Set Tabs
    $(function () {
       
        //Create Tabs      
        $(".ResourceInfo").tabs({
        });
        //Disable empty tabs and set selected
        $('.ResourceInfo').each(function (i, obj) {
            //Select Tab
            $(this).tabs('option', 'active', $(this).attr("data-selectedIndex"));
            //Disable Tabs
            var array = $(this).attr("data-tabsToDisable").split(',');
            for (var i = 0; i < array.length; i++) { array[i] = parseInt(array[i]); }
            $(this).tabs("option", "disabled", array);
        });
         
    });

});
