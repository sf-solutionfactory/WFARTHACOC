$(document).ready(function () {
    var _miles = $("#miles").val();
    var _decimales = $("#dec").val();
    //Recuperamos los valores con jquery
    var _mdec = $("#monto_dis").val();
    //Para validar los decimales del costo unitario
    $('body').on('focusout', '#monto_dis', function () {
        var xx = $(this).val().replace("$", "");
        var _miles = $("#miles").val();
        var _decimales = $("#dec").val();
        if (xx != '') {
            if (_decimales === '.') {
                //Hace la conversion a 2 decimales
                var _xv = xx.replace(',', '');
                xx = _xv;
                $(this).val("$" + parseFloat(xx).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
            } else if (_decimales === ',') {
                var _xv = xx.replace('.', '');
                xx = _xv.replace(',', '.');
                var _xpf = parseFloat(xx.replace(',', '.')).toFixed(2);
                _xpf = _xpf.replace('.', ',');
                $(this).val("$" + _xpf.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
            }
        }
        else {
            $(this).val("$ 0.00");
        }
    });
    //Para que no ingrese letras ni signos extra
    $('body').on('keydown', '#monto_dis', function (e) {
        if (_decimales === ".") {
            if (e.keyCode == 110 || e.keyCode == 190) {
                if ($(this).val().indexOf('.') != -1) {
                    e.preventDefault();
                }
            }
            else {  // Allow: backspace, delete, tab, escape, enter and .
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    // Allow: Ctrl+A, Command+A
                    (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything

                    return;
                }

                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            }
        } else if (_decimales === ",") {
            if (e.keyCode == 188) {
                if ($(this).val().indexOf(',') != -1) {
                    e.preventDefault();
                }
            }
            else {  // Allow: backspace, delete, tab, escape, enter and ','
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 188]) !== -1 ||
                    // Allow: Ctrl+A, Command+A
                    (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything

                    return;
                }

                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            }
        }
    });

});