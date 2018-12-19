$(document).ready(function () {
    $(".txtIDP").keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and 
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
            // Allow: Ctrl/cmd+A
            (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: Ctrl/cmd+C
            (e.keyCode == 67 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: Ctrl/cmd+X
            (e.keyCode == 88 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
    //Para validar los decimales del costo unitario
    $('body').on('focusout', '.pname', function () {
        var xx = $(this).val();

        if (xx !== '') {
            //Hace la conversion a 2 decimales
            $(this).val(xx.toUpperCase());
        }
        else {
            $(this).val('');
        }
    });
    //Para el campo id, que sean 10 digitos y se autocomplete con"0" de ser necesario
    $('body').on('focusout', '.txtIDP', function () {
        var xx = $(this).val();
        //si es diferente de vacio entra
        if (xx !== '') {
            //recuperamos la cantidad de numeros ingresados y de ser necesario añadir ceros0
            var ch = xx.split('');
            var nc = "";
            for (i = 0; i < (10 - ch.length); i++) {
                nc += "0";
            }
            nc = nc + xx;
            $(this).val(nc);
        }
        else {
            $(this).val('');
        }
    });
});