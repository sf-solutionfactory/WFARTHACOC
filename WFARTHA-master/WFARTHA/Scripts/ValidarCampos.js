$(document).ready(function () {
    $(".fechaVal").keydown(function (e) {
        if (e.keyCode == 109 || e.keyCode == 189) {
            if ($(this).val().indexOf('-') != -1) {
                e.preventDefault();
            }
        }
        else { // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190, 189, 109]) !== -1 ||
                // Allow: Ctrl+A, Command+A
                (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40)) {
                // No pasa nada
                return;
            }


            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        }
    });

    //Para que no ingrese letras ni simbolos
    $(".valmon").keydown(function (e) {
        if (e.keyCode == 110 || e.keyCode == 190) {
            if ($(this).val().indexOf('.') != -1) {
                e.preventDefault();
            }
        }
        else { // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                // Allow: Ctrl+A, Command+A
                (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40)) {
                // No pasa nada
                return;
            }


            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        }
    });

    //Para validar los decimales del costo unitario
    $('body').on('focusout', '.valmon', function () {
        var xx = $(this).val().replace("$", "");
        var _miles = $("#miles").val();
        var _decimales = $("#dec").val();
        if (xx != '') {
            //Hace la conversion a 2 decimales
            var _xv = xx.replace(',', '');
            xx = _xv;
            $(this).val("$" + parseFloat(xx).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        }
        else {
            $(this).val("$0.00");
        }
    });


});