$(document).ready(function () {
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

    //Para que el campo de id solo sea posible ingresar numeros
    $('body').on('keydown', '.txtIDP', function (e) {
        // Allow: backspace, delete, tab, escape, enter.
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
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
    });

    //Para el texto del checkbox editable
    $('#mEdit').change(function () {
        if ($(this).is(":checked")) {
            $('#spEd').text("No");
        } else {
            $('#spEd').text("Si");
        }
    });

    //Para el texto del checkbox Obligatorio
    $('#mOb').change(function () {
        if ($(this).is(":checked")) {
            $('#spOb').text("Si");
        } else {
            $('#spOb').text("No");
        }
    });
});