(function ($) {
    $.validator.addMethod('verifymid', function (value, element, other) {

        if (value[0] != 'm' && value[0] != 'M') {
            return false;
        }
        if (value.length >= 9) {
            return false
        }
        if (value.length < 8) {
            return false;
        }
        else {
            for (var i = 1; i < value.length; i++) {
                if (!$.isNumeric(value[i]))
                    return false;
            }
        }
        return true;
    });
    $.validator.unobtrusive.adapters.addBool('verifymid');
})(jQuery)
