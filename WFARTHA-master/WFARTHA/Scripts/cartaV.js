$('body').on('click', '#btn_guardar', function (e) {

    guardarcarta("guardar_param");

});

$('body').on('click', '#btn_visualizar', function (e) {

    guardarcarta("");

});

$('body').on('focusout', '#ed_monto', function (e) {

    var total = 0;

    total = updateTotalRowp();

    //Obtener el monto original
    var monto = $('#monto').val();
    monto = parseFloat(monto);
    if (total > monto) {
        M.toast({ html: 'Monto de distribución es mayor al monto de la solicitud' });
    }
    var ed_monto = $('#ed_monto').val();
    $('#ed_monto').val(toShow(ed_monto))
});

//B20180801 MGC Textos
$('body').on('keydown', '.total', function (e) {

    var key = e.which;
    if (key == 13) {
        
        var tr = $(this).closest('tr'); //Obtener el row 

        //B20180720 MGC Formato a valores en la tabla
        //Moneda
        if ($(this).hasClass("mon")) {
            var val = toNum($(this).val());
            $(this).val(toShow(val));
        }
        //Numero
        if ($(this).hasClass("num")) {
            var val = toNum($(this).val());
            $(this).val(toShowNum(val));
        }
        //Porcentaje
        if ($(this).hasClass("porc")) {
            var val = toNum($(this).val());
            $(this).val(toShowPorc(val));
        }

        //Solo a cantidades
        if ($(this).hasClass("numberd")) {

            //Definir si es tipo m
            var tipo = "";
            if ($(this).hasClass("tipom")) {
                tipo = "m";
            } else if ($(this).hasClass("tipop")) {
                tipo = "p"
            }

            //Se dispara el evento desde el total
            if ($(this).hasClass("total") & !$(this).hasClass("cat")) {
                var total_val = toNum($(this).val());
                //Agregar los valores a 0 y agregar el total
                updateTotalRow(tr, "", "X", total_val, "", tipo);
                $(this).addClass("keyup");
            }
        }
    }

});

$('body').on('focusout', '.input_oper', function () {

    var tr = $(this).closest('tr'); //Obtener el row 

    //B20180720 MGC Formato a valores en la tabla
    //Moneda
    if ($(this).hasClass("mon")) {
        var val = toNum($(this).val());
        $(this).val(toShow(val));
    }
    //Numero
    if ($(this).hasClass("num")) {
        var val = toNum($(this).val());
        $(this).val(toShowNum(val));
    }
    //Porcentaje
    if ($(this).hasClass("porc")) {
        var val = toNum($(this).val());
        $(this).val(toShowPorc(val));
    }

    //Solo a cantidades
    if ($(this).hasClass("numberd")) {

        //Definir si es tipo m
        var tipo = "";
        if ($(this).hasClass("tipom")) {
            tipo = "m";
        } else if ($(this).hasClass("tipop")) {
            tipo = "p"
        }

        //Se dispara el evento desde el total
        if ($(this).hasClass("total") & !$(this).hasClass("cat")) {
            var total_val = toNum($(this).val());//B20180720 MGC Formato a valores en la tabla

            if (!$(this).hasClass("keyup")) {
                updateTotalRow(tr, "", "", 0, "", tipo);//B20180801 MGC Textos
            } else {
                //Agregar los valores a 0 y agregar el total
                updateTotalRow(tr, "", "X", total_val, "", tipo); //B20180801 MGC Textos
                $(this).removeClass("keyup");
            }
        
            //Agregar los valores a 0 y agregar el total
            //updateTotalRow(tr, "", "X", total_val, "", tipo);//B20180801 MGC Textos
            //alert("total" + total_val);
        } else if ($(this).hasClass("total") & $(this).hasClass("cat")) {
            var total_val = toNum($(this).val());//B20180720 MGC Formato a valores en la tabla
            updateTotalRow(tr, "", "X", total_val, "X", tipo);
        } else {
            updateTotalRow(tr, "", "", 0, "", tipo);
        }

    }
});

function guardarcarta(guardar) {

    //editmonto_texto
    var ed = $('#editmonto_texto').val();
    //Validar valores correctos en distribución
    var total = 0;
    if (ed == "false") {
        updateFooter(false);

        total = totalFooter();

        var texto = armarMonto(total);

    } else if (ed == "true") {
        total = updateTotalRowp();
    }

    total = parseFloat(total);
    //Obtener el monto original
    var monto = $('#monto').val();
    monto = parseFloat(monto);

    if (total > monto) {
        M.toast({ html: 'Monto de distribución es mayor al monto de la solicitud' });
    } else {
        copiarTableControl();
        $('#monto_enviar').val(total.toFixed(2));
        if (guardar == "guardar_param") {
            $('#guardar_param').val(guardar);
        }
        $('#btn_submit').click();
    }

}

function updateTotalRow(tr, tdp_apoyo, totals, total_val, cat, tipo) {

    //totals = X cuando nada más se agrega el total

    //Multiplicar costo unitario % por apoyo(dividirlo entre 100)
    //Columnas 8 * 9 res 10
    //Categoría es 7 * 8 = 9  --> -1
    //Material es 6 * 7 = 8   --> -2

    //Validar si las operaciones se hacen por renglón o solo agregar el valor del total
    if (totals != "X") {
        var col3 = tr.find("td:eq(" + (3) + ") input").val();
        col3 = toNum(col3)//B20180720 MGC Formato a valores en la tabla
        var col4 = 0;

        if (tipo == "m") {
            col4 = tr.find("td:eq(" + (4) + ") input").val();
        } else if (tipo == "p") {
            col4 = tr.find("td:eq(" + (4) + ")").text();
        }
        col4 = toNum(col4)//B20180720 MGC Formato a valores en la tabla
        col4 = convertP(col4);

        if ($.isNumeric(col4)) {
            col4 = col4 / 100;
        }

        var col5 = col3 * col4;
        //Apoyo por pieza
        //Modificar el input
        //tr.find("td:eq(" + (5) + ")").text(col5.toFixed(2));//B20180720 MGC Formato a valores en la tabla
        tr.find("td:eq(" + (5) + ")").text(toShow(col5));//B20180720 MGC Formato a valores en la tabla

        //Costo con apoyo
        var col6 = col3 - col5;
        //tr.find("td:eq(" + (6) + ")").text(col6.toFixed(2));//B20180720 MGC Formato a valores en la tabla
        tr.find("td:eq(" + (6) + ")").text(toShow(col6));//B20180720 MGC Formato a valores en la tabla

        //Estimado apoyo
        var col8 = tr.find("td:eq(" + (8) + ") input").val();//B20180720 MGC Formato a valores en la tabla
        col8 = toNum(col8)//B20180720 MGC Formato a valores en la tabla
        var col9 = col5 * col8;
        //col14 = col14.toFixed(2);
        if (tipo == "m") {
            //tr.find("td:eq(" + (9) + ") input").val(col9.toFixed(2));//B20180720 MGC Formato a valores en la tabla
            tr.find("td:eq(" + (9) + ") input").val(toShow(col9));//B20180720 MGC Formato a valores en la tabla
        } else if (tipo == "p") {
            //tr.find("td:eq(" + (9) + ")").text(col9.toFixed(2));//B20180720 MGC Formato a valores en la tabla
            tr.find("td:eq(" + (9) + ")").text(toShow(col9));//B20180720 MGC Formato a valores en la tabla
        }


        //Agregar nada más el total
    } else {
        total_val = parseFloat(total_val);
        var col9 = total_val;
        if (cat == "") {
            //tr.find("td:eq(" + (3) + ") input").val("0.00");//B20180720 MGC Formato a valores en la tabla
            tr.find("td:eq(" + (3) + ") input").val(toShow("0"));//B20180720 MGC Formato a valores en la tabla
            if (tdp_apoyo != "X") {
                if (tipo == "m") {
                    //tr.find("td:eq(" + (4) + ") input").val("0.00");//B20180720 MGC Formato a valores en la tabla
                    tr.find("td:eq(" + (4) + ") input").val(toShowPorc("0"));//B20180720 MGC Formato a valores en la tabla
                } else if (tipo == "p") {
                    //tr.find("td:eq(" + (4) + ")").val("0.00");//B20180720 MGC Formato a valores en la tabla
                    tr.find("td:eq(" + (4) + ")").val(toShowPorc("0"));//B20180720 MGC Formato a valores en la tabla
                }
            }
            //tr.find("td:eq(" + (5) + ")").text("0.00");//B20180720 MGC Formato a valores en la tabla
            tr.find("td:eq(" + (5) + ")").text(toShow("0"));//B20180720 MGC Formato a valores en la tabla
            //tr.find("td:eq(" + (6) + ")").text("0.00");//B20180720 MGC Formato a valores en la tabla
            tr.find("td:eq(" + (6) + ")").text(toShow("0"));//B20180720 MGC Formato a valores en la tabla
            //tr.find("td:eq(" + (7) + ") input").val("0.00");//B20180720 MGC Formato a valores en la tabla
            tr.find("td:eq(" + (7) + ") input").val(toShow("0"));//B20180720 MGC Formato a valores en la tabla
            //tr.find("td:eq(" + (8) + ") input").val("0.00");//B20180720 MGC Formato a valores en la tabla
            tr.find("td:eq(" + (8) + ") input").val(toShowNum("0"));//B20180720 MGC Formato a valores en la tabla
            if (tipo == "m") {
                //tr.find("td:eq(" + (9) + ") input").val(col9.toFixed(2));//B20180720 MGC Formato a valores en la tabla
                tr.find("td:eq(" + (9) + ") input").val(toShow(col9));//B20180720 MGC Formato a valores en la tabla
            } else if (tipo == "p") {
                //tr.find("td:eq(" + (9) + ")").text(col9.toFixed(2));//B20180720 MGC Formato a valores en la tabla
                tr.find("td:eq(" + (9) + ")").text(toShow(col9));//B20180720 MGC Formato a valores en la tabla
            }

        } else if (cat == "X") {
            tr.find("td:eq(" + (3) + ")").text("");
            if (tdp_apoyo != "X") {

                tr.find("td:eq(" + (4) + ")").text("");
            }
            tr.find("td:eq(" + (5) + ")").text("");
            tr.find("td:eq(" + (6) + ")").text("");
            tr.find("td:eq(" + (7) + ")").text("");
            tr.find("td:eq(" + (8) + ")").text("");
            tr.find("td:eq(" + (9) + ") input").val(toShow(col9));//B20180720 MGC Formato a valores en la tabla
        }
    }

    updateFooter(true);
}

function convertP(i) {
    return typeof i === 'string' ?
        i.replace(/[\$,]/g, '') * 1 :
        typeof i === 'number' ?
            i : 0;
};

function convertI(i) {
    return typeof i === 'string' ?
        i.replace(/[\$,]/g, '') * 1 :
        typeof i === 'number' ?
            i : 0;
};

function updateFooter(flag) {
    resetFooter();
    var total = 0;

    total = totalFooter();

    var texto = armarMonto(toShow(total.toFixed(2)));//RSG 01.08.2018

    //Obtener el monto original
    if (flag) {
        var monto = $('#monto').val();
        var montof = parseFloat(monto);
        var totalf = parseFloat(total);
        if (totalf > montof) {
            M.toast({ html: 'Monto de distribución es mayor al monto de la solicitud' });
        }
    }

    $('#lbl_monto').text(texto);
}

function totalFooter() {
    coltotal = (9);
    var total = 0;

    //Obtener las tablas
    var tables = $('.table_mat');

    try {
        for (var i = 0; i < tables.length; i++) {
            var tabname = "#" + tables[i].id;
            $(tabname).find("tr[role='row']").each(function (index) {
                var col9 = 0;
                if ($(this).hasClass("total")) {
                    col9 = $(this).find("td:eq(" + coltotal + ") input").val();
                } else {
                    col9 = $(this).find("td:eq(" + (coltotal) + ")").text();
                }
                col9 = toNum(col9);//B20180720 MGC Formato a valores en la tabla
                col9 = convertI(col9);

                if ($.isNumeric(col9)) {
                    total += col9;
                }

            });
        }
    } catch (error) {

    }

    total = total.toFixed(2);
    total = parseFloat(total)

    return total;
}

function updateTotalRowp() {
    var coltotal = (9);
    var colpor = (4)
    var total = 0;

    //Obtener la cantidad asignada por el usuario como total
    var ed_monto = $('#ed_monto').val();
    ed_monto = toNum(ed_monto);//B20180801 MGC Formato
    ed_monto = parseFloat(ed_monto);

    // ed_monto -- 100%
    //coltotal  -- colpor
    //Obtener las tablas
    var tables = $('.table_mat');

    try {
        for (var i = 0; i < tables.length; i++) {
            var tabname = "#" + tables[i].id;
            $(tabname).find("tr[role='row']").each(function (index) {
                var col4 = 0;
                col4 = $(this).find("td:eq(" + (colpor) + ")").text();
                col4 = toNum(col4);//B20180720 MGC Formato a valores en la tabla
                col4 = parseFloat(col4);

                colt = (col4 * ed_monto) / 100;

                //$(this).find("td:eq(" + (coltotal) + ")").text(colt.toFixed(2));//B20180720 MGC Formato a valores en la tabla
                $(this).find("td:eq(" + (coltotal) + ")").text(toShow(colt));//B20180720 MGC Formato a valores en la tabla //B20180720P MGC
                total += colt;

            });
        }
    } catch (error) {

    }

    total = total.toFixed(2);
    total = parseFloat(total)

    return total;
}

function resetFooter() {
    var texto = armarMonto("0.00");
    $('#lbl_monto').text(texto);
}

function armarMonto(monto) {
    var texto = $('#monto_texto').val();
    //var monto = $('#monto').val();
    var moneda = $('#moneda').val();

    return texto + " " + monto + " " + moneda
}

function copiarTableControl() {

    //var lengthT = $("table#table_dis tbody tr[role='row']").length;
    var tables = $('.table_mat');

    if (tables.length > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla oculta y agregarlos al json

        jsonObjDocs = [];
        var j = 1;
        var vol = "";
        var mostrar = true;
        mostrar = isFactura();


        if (mostrar) {
            vol = "real";
        } else {
            vol = "estimado";
        }

        //Obtener las tablas
        var tables = $('.table_mat');
        try {
            for (var i = 0; i < tables.length; i++) {
                var tabdedate = "#aldate_" + tables[i].id;
                var tabaldate = "#dedate_" + tables[i].id;
                var tabname = "#" + tables[i].id + " > tbody  > tr[role='row']";

                $(tabname).each(function () {

                    var vigencia_de = $(tabdedate).val();//$(this).find("td:eq(" + (3) + ") input").val();
                    var vigencia_al = $(tabaldate).val();//$(this).find("td:eq(" + (4) + ") input").val();

                    var matnr = "";
                    matnr = $(this).find("td:eq(" + (0) + ")").text();
                    var matkl = $(this).find("td:eq(" + (1) + ")").text();

                    //Obtener el id de la categoría            

                    var matkl_id = matkl;

                    //B20180720 MGC Formato a valores en la tabla
                    ////Definir si es tipo m
                    //var tipo = "";
                    //if ($(this).hasClass("tipom")) {
                    //    tipo = "m";
                    //} else if ($(this).hasClass("tipop")) {
                    //    tipo = "p"
                    //}

                    //Saber si los valores se tienen como texto del td o input
                    var costo_unitario = 0;
                    if ($(this).find("td:eq(" + (3) + ")").hasClass("ni")) {
                        costo_unitario = $(this).find("td:eq(" + (3) + ")").text();
                    } else {
                        costo_unitario = $(this).find("td:eq(" + (3) + ") input").val();
                    }
                    costo_unitario = toNum(costo_unitario);//B20180720 MGC Formato a valores en la tabla

                    var porc_apoyo = 0;
                    if ($(this).find("td:eq(" + (4) + ")").hasClass("ni")) {
                        porc_apoyo = $(this).find("td:eq(" + (4) + ")").text();
                    } else {
                        porc_apoyo = $(this).find("td:eq(" + (4) + ") input").val();
                    }
                    porc_apoyo = toNum(porc_apoyo);//B20180720 MGC Formato a valores en la tabla
                    //var porc_apoyo = $(this).find("td:eq(" + (4) + ") input").val();
                    var monto_apoyo = 0;
                    if ($(this).find("td:eq(" + (5) + ")").hasClass("ni")) {
                        monto_apoyo = $(this).find("td:eq(" + (5) + ")").text();
                    } else {
                        monto_apoyo = $(this).find("td:eq(" + (5) + ") input").val();
                    }
                    monto_apoyo = toNum(monto_apoyo);//B20180720 MGC Formato a valores en la tabla
                    //var monto_apoyo = $(this).find("td:eq(" + (5) + ") input").val();
                    var precio_sug = 0;
                    if ($(this).find("td:eq(" + (7) + ")").hasClass("ni")) {
                        precio_sug = $(this).find("td:eq(" + (7) + ")").text();
                    } else {
                        precio_sug = $(this).find("td:eq(" + (7) + ") input").val();
                    }
                    precio_sug = toNum(precio_sug);//B20180720 MGC Formato a valores en la tabla
                    //var precio_sug = $(this).find("td:eq(" + (7) + ") input").val();
                    var volumen_est = 0;
                    if ($(this).find("td:eq(" + (8) + ")").hasClass("ni")) {
                        volumen_est = $(this).find("td:eq(" + (8) + ")").text();
                    } else {
                        volumen_est = $(this).find("td:eq(" + (8) + ") input").val();
                    }
                    volumen_est = toNum(volumen_est);//B20180720 MGC Formato a valores en la tabla
                    //var volumen_est = $(this).find("td:eq(" + (8) + ") input").val();
                    var total = 0;
                    if ($(this).find("td:eq(" + (9) + ")").hasClass("ni")) {
                        total = $(this).find("td:eq(" + (9) + ")").text();
                    } else {
                        total = $(this).find("td:eq(" + (9) + ") input").val();
                    }
                    total = toNum(total);//B20180720 MGC Formato a valores en la tabla
                    //var total = $(this).find("td:eq(" + (9) + ") input").val();

                    var item = {};

                    item["NUM_DOC"] = 0;
                    item["POS"] = j;
                    item["MATNR"] = matnr || "";
                    item["MATKL"] = matkl;
                    item["MATKL_ID"] = matkl_id;
                    item["DESC"] = "";
                    item["CANTIDAD"] = 0; //Siempre 0
                    item["MONTO"] = costo_unitario || 0;
                    item["PORC_APOYO"] = porc_apoyo || 0;
                    item["MONTO_APOYO"] = monto_apoyo || 0;
                    item["VIGENCIA_DE"] = vigencia_de + " 12:00:00 p. m.";
                    item["VIGENCIA_AL"] = vigencia_al + " 12:00:00 p. m.";
                    item["PRECIO_SUG"] = precio_sug || 0;
                    volumen_est = volumen_est || 0;
                    total = parseFloat(total);
                    total = total || 0;
                    if (vol == "estimado") {
                        item["VOLUMEN_EST"] = volumen_est;
                        item["VOLUMEN_REAL"] = 0;
                        item["APOYO_REAL"] = 0;
                        item["APOYO_EST"] = total;
                    } else {
                        item["VOLUMEN_EST"] = 0;
                        item["VOLUMEN_REAL"] = volumen_est;
                        item["APOYO_REAL"] = total;
                        item["APOYO_EST"] = 0;

                    }

                    jsonObjDocs.push(item);
                    j++;
                    item = "";

                });

            }
        } catch (error) {

        }



        docsenviar = JSON.stringify({ 'docs': jsonObjDocs });

        $.ajax({
            type: "POST",
            url: '../../Listas/getPartialMat',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {

                if (data !== null || data !== "") {

                    $("table#table_dish tbody").append(data);

                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }

}

function isFactura() {

    var res = false;


    var fact = $('#isfactura').val();

    try {
        fact = (fact == 'true');
    } catch (error) {
        fact = false;
    }
    res = fact;
    return res;
}