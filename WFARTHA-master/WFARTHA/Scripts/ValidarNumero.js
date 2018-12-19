$(document).ready(function () {
    var ban = 0;
    var _mRef = 0;
    var _decimales = $("#dec").val();
    var _miles = $("#miles").val();
    $('body').on('keydown', '.input_dc', function (e) {
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
        }
        else if (_decimales === ",") {
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

    //Para validar los decimales del costo unitario
    $('body').on('focusout', '.input_dc', function () {
        var xx = $(this).val().replace("$",'');
        if (_decimales == ".") {
            var _xx = xx.replace(',', '');
            xx = _xx;
        }
        if (_decimales == ",") {
            if (xx.indexOf(",") >= 0) {
                var _xx = xx.replace('.', '');
                _xx = _xx.replace(',', '.');
                xx = _xx;
            }
        }
        if (xx != '') {
            //Hace la conversion a 2 decimales
            var _re = parseFloat(xx).toFixed(2);
            //if para decimales es una coma es el siguiente proceso
            if (_decimales == ",") {
                if (_re.indexOf(".") >= 0) {
                    var _rex = _re;
                    _rex = _rex.replace('.', ',');
                    _re = _rex;
                }
            }
            $(this).val("$" + _re.toString().replace(/\B(?=(\d{3})+(?!\d))/g, _miles));
        }
        else {
            $(this).val($(this).val());
        }
    });

    // Para que no ingresen letras en la fecha
    $('body').on('keydown', '.input_fe', function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        //Para slash
        if (e.keyCode === 111 || (e.shiftKey & e.keyCode === 55)) {
            return;
        }

        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    //Para validar las fechas
    $('body').on('focusout', '.fv', function () {
        var xx = $(this).val();
        if (xx != "") {
            if (isDate(xx) === true) {
                //alert("true");
            } else {
                alert("Fecha Erronea");
                //$(this).val("");
            }
        } else {
            return;
        }
    });

    //Para validar las fechas
    $('body').on('focusout', '.input_fe', function () {
        var xx = $(this).val();
        if (xx != "") {
            if (isDate(xx) === true) {
                //alert("true");
            } else {
                //alert("false");
                $(this).val("");
            }
        } else {
            return;
        }
    });

    function isDate(xx) {
        var currVal = xx;
        if (currVal == '')
            return false;

        var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/; //Declare Regex
        var dtArray = currVal.match(rxDatePattern); // is format OK?

        if (dtArray == null)
            return false;

        //Checks for mm/dd/yyyy format.
        dtMonth = dtArray[3];
        dtDay = dtArray[1];
        dtYear = dtArray[5];

        if (dtMonth < 1 || dtMonth > 12) return false;

        else if (dtDay < 1 || dtDay > 31) return false;
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31) return false;
        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap)) return false;
        }
        return true;
    }
});