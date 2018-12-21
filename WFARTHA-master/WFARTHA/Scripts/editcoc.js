$(document).ready(function () { });

//LEJGG 11/12/2018------------------------------------------------I
$('body').on('change', '#norden_compra', function (event, param1) {
    var eb = $(this).val();
    $.ajax({
        type: "POST",
        url: 'getEKKOInfo',
        dataType: "json",
        data: { "ebeln": $(this).val() },
        success: function (data) {
            var ekko = data.ekmo;
            var cuentas = data.res;
            var mtr = data.mtr;
            var brtwr = data.brtwr;
            llenarTablaOc(ekko, cuentas, mtr, brtwr);
        }
    });

    $.ajax({
        type: "POST",
        url: 'getEKBEInfo',
        dataType: "json",
        async: false,
        data: { "ebeln": $(this).val() },
        success: function (data) {
            llenarTablaOc2(data, eb);
        }
    });

    $.ajax({
        type: "POST",
        url: 'getEKPOInfo',
        dataType: "json",
        data: { "ebeln": $(this).val() },
        success: function (data) {
            armarTabla(data);
        }
    });
});

//LEJGG 21-12-2018
function mostrarTabla(ban) {
    if (ban === "SCO") {
        $("#div_sinPedido").addClass("hide");
        $("#div_conPedido").removeClass("hide");
        $("#div_garantia").removeClass("hide");
        $("#divTot").addClass("hide");
        $("#conOrden").val("X");
    } else {
        $("#div_conPedido").addClass("hide");
        $("#div_sinPedido").removeClass("hide");
        $("#div_garantia").addClass("hide");
        $("#divTot").removeClass("hide");
        $("#conOrden").val("");
    }
}

function llenarCOC() {
    var val3 = $("#tsol").val();
    val3 = "[" + val3 + "]";
    val3 = val3.replace("{", "{ \"");
    val3 = val3.replace("}", "\" }");
    val3 = val3.replace(/\,/g, "\" , \"");
    val3 = val3.replace(/\=/g, "\" : \"");
    val3 = val3.replace(/\ /g, "");
    var jsval = $.parseJSON(val3);

    //Recupero el valor de la sociedad
    var buk = $("#SOCIEDAD_ID").val();
    if (tsolid() === "SCO") {
        if ($("#PAYER_ID").val() != "") {
            llenaOrdenes($("#PAYER_ID").val(), buk);
        }
    }
}

function llenaOrdenes(lifnr, bukrs) {
    var tr = $(this).closest('tr'); //Obtener el row
    var t = $('#table_infoP').DataTable();

    var pedidosNum = [];
    $("#norden_compra").empty();

    $.ajax({
        type: "POST",
        url: '../getPedidos',
        dataType: "json",
        data: { "lifnr": lifnr.trim(), "bukrs": bukrs },
        success: function (data) {
            $("#norden_compra").empty();
            for (var i = 0; i < data.length; i++) {
                var ebeln = data[i];
                $("#norden_compra").append($("<option>").attr('value', ebeln).text(ebeln));
            }
            var eb = $("#eebeln").val();
            $('#norden_compra').find('option[value="' + eb + '"]').prop('selected', true);
            var elem = document.querySelectorAll("select");
            var instance = M.Select.init(elem, []);
            solData();
        }
    });
}

function solData() {
    var e = $('#norden_compra').val();
    $.ajax({
        type: "POST",
        url: '../getEKKOInfo',
        dataType: "json",
        data: { "ebeln": e },
        async: false,
        success: function (data) {
            var ekko = data.ekmo;
            var cuentas = data.res;
            var mtr = data.mtr;
            var brtwr = data.brtwr;
            llenarTablaOc(ekko, cuentas, mtr, brtwr);
        }
    });
    $('#norden_compra').trigger("change");
}

//LEJGG 11/12/2018------------------------------------------------I
$('body').on('change', '#norden_compra', function (event, param1) {
    var eb = $(this).val();
    $.ajax({
        type: "POST",
        url: 'getEKKOInfo',
        dataType: "json",
        data: { "ebeln": $(this).val() },
        success: function (data) {
            var ekko = data.ekmo;
            var cuentas = data.res;
            var mtr = data.mtr;
            var brtwr = data.brtwr;
            llenarTablaOc(ekko, cuentas, mtr, brtwr);
        }
    });

    $.ajax({
        type: "POST",
        url: 'getEKBEInfo',
        dataType: "json",
        async: false,
        data: { "ebeln": $(this).val() },
        success: function (data) {
            llenarTablaOc2(data, eb);
        }
    });

    $.ajax({
        type: "POST",
        url: 'getEKPOInfo',
        dataType: "json",
        data: { "ebeln": $(this).val() },
        success: function (data) {
            armarTabla(data);
        }
    });
});

function llenarTablaOc2(val, eb) {
    var mt = "";
    var at = "";

    var tabl = $('#tableOC2').DataTable();
    //Limpio primero la tabla
    $("#tableOC2 tbody tr[role='row']").each(function () {
        var _t = $(this);
        tabl.row(_t).remove().draw(false);
    });
    //Ajax para llenar campos calculados
    for (var i = 0; i < val.length; i++) {
        var ebelp = val[i].EBELP;
        var buzei = val[i].BUZEI;
        var belnr = val[i].BELNR;
        var gjar = val[i].GJAHR;
        var wt = val[i].WRBTR;
        var mon = val[i].WAERS;
        //calculo de anticipo amortizado
        $.ajax({
            type: "POST",
            url: 'calculoAntAmor',
            dataType: "json",
            async: false,
            data: { "ebeln": eb, "belnr": belnr },
            success: function (data) {
                mt = data;
                //
                var at = 0;
                tabl.row.add([
                    ebelp,//POSC
                    buzei,//POS
                    belnr,//numdoc
                    gjar,//EJERCICIO
                    toShow(mt),//Anticipo amortizado
                    toShow(wt),//total anticipo
                    mon,//MONEDA,
                    toShow(at),//anticipo en transito
                    "<input class=\"ANTXAMORT\" style=\"font-size:12px;\" type=\"text\" id=\"antxamor\" name=\"\" value=\"\">"
                ]).draw(false).node();
                //calculo de anticipo en transito
                //$.ajax({
                //    type: "POST",
                //    url: 'calculoAntTr',
                //    dataType: "json",
                //    success: function (datax) {
                //        at = datax;
                //        //
                //        tabl.row.add([
                //            ebelp,//POSC
                //            buzei,//POS
                //            belnr,//numdoc
                //            gjar,//EJERCICIO
                //            toShow(mt),//Anticipo amortizado
                //            toShow(wt),//total anticipo
                //            mon,//MONEDA,
                //            toShow(at),//anticipo en transito
                //            "<input class=\"ANTXAMORT\" style=\"font-size:12px;\" type=\"text\" id=\"antxamor\" name=\"\" value=\"\">"
                //        ]).draw(false).node();
                //    }
                //});
            }
        });
    }
}

$('body').on('keydown', '.ANTXAMORT', function (e) {
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
});

$('body').on('keydown', '.MONTOP', function (e) {
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
});

function armarTabla(info) {
    var _t = $('#table_infoP').DataTable();
    var _numrow = _t.rows().count();
    _numrow++; //frt04122018
    //Limpio primero la tabla
    $("#table_infoP tbody tr[role='row']").each(function () {
        var t = $(this);
        _t.row(t).remove().draw(false);
    });
    var totl = 0;
    for (var i = 0; i < info.length; i++) {
        var porc = 0;
        $.ajax({
            type: "POST",
            url: 'getPorMult',
            data: { "id": info[i].MWSKZ },
            async: false,
            dataType: "json",
            success: function (data) {
                porc = data;
            }
        });
        var m = 0;
        var c = 0;
        m = parseFloat(info[i].NETPR_BIL) - parseFloat(info[i].NETPR_DEL);
        c = parseFloat(info[i].MENGE_BIL) - parseFloat(info[i].MENGE_DEL);
        var ebelp = info[i].EBELP;
        var mat = info[i].MATNR;
        var matkl = info[i].MATKL;
        var sakto = info[i].SAKTO;
        var knt = info[i].KNTTP;
        var kostl = info[i].KOSTL;
        var waers = info[i].WAERS;
        var meins = info[i].MEINS;
        var tx = info[i].TXZ01;
        var pep = info[i].PS_PSP_PNR;
        var iva = info[i].MWSKZ;
        var por = parseFloat(porc);
        var cal = (m * por) / 100;
        var tot = m + cal;
        //Ajax para calcular el iva
        var ari = addRowInfoP(_t, ebelp, "", "", "", "", "", mat, "D", "", "", matkl, sakto, "", knt, "", kostl, toShow(m), waers, c, meins, "", "", tx, tot, pep);
        //Obtener el select de impuestos en la cabecera
        var idselect = "infoSel" + _numrow;
        //totl = totl + parseFloat(m);
        //Obtener el valor 
        var imp = iva;
        //Crear el nuevo select con los valores de impuestos
        addSelectImpuestoP(ari, imp, idselect, "", "X");
    }
    updateFooterP();//lejgg-18-12-2018
    //$('#MONTO_DOC_MD').val(toShow(totl));
    //alinear a la izq
    $("#table_infoP tbody tr[role='row']").each(function () {
        //1
        var R1 = $(this).find("td.GRUPO");
        R1.css("text-align", "left");
        //2
        var R2 = $(this).find("td.MONEDA");
        R2.css("text-align", "left");
        var U = $(this).find("td.UNIDAD");
        U.css("text-align", "left");
    });
    $('#mtTot').val($('#MONTO_DOC_MD').val());//Lej 12.12.2018
}

function addSelectImpuestoP(addedRowInfo, imp, idselect, disabled, clase) {

    //Obtener la celda del row agregado
    var ar = $(addedRowInfo).find("td.IVA");

    var sel = $("<select style=\"width:60px;\" class = \"IMPUESTO_SELECT browser-default\" id = \"" + idselect + "\"> ").appendTo(ar);
    $("#IMPUESTO option").each(function () {
        var _valor = $(this).val();//lej 19.09.2018
        var _texto = $(this).text();//lej 19.09.2018
        sel.append($("<option>").attr('value', _valor).text(_texto));//lej 19.09.2018
    });

    //Seleccionar el valor
    $("#" + idselect + "").val(imp);
    $("#" + idselect + "").siblings(".select-dropdown").css("font-size", "12px");
    if (disabled == "X") {

        $("#" + idselect + "").prop('disabled', 'disabled');
    }

    //Iniciar el select agregado
    //  $(".IMPUESTO_SELECT").trigger("change");
    $('.IMPUESTO_SELECT option[value="' + imp.trim() + '"]').attr("selected", true);
}

function llenarTablaOc(a, b, mtr, brtwr) {
    var tabl = $('#tableOC').DataTable();
    //Limpio primero la tabla
    $("#tableOC tbody tr[role='row']").each(function () {
        var _t = $(this);
        tabl.row(_t).remove().draw(false);
    });
    var c = b.split('?');
    //Añado los datos
    tabl.row.add([
        toShow(brtwr),
        a.RETPC,  //%ret             
        a.DPPCT//,//%deposito
    ]).draw(false).node();
    alinearTOC();
}

function alinearTOC() {
    //--------
    //Para los titulos
    var t0 = $("#tableOC>thead>tr").find('th.BRTWR ');
    t0.css("text-align", "left");
    var t1 = $("#tableOC>thead>tr").find('th.FondoGarantia ');
    t1.css("text-align", "left");
    var t2 = $("#tableOC>thead>tr").find('th.AmortAnt');
    t2.css("text-align", "left");
    var t3 = $("#tableOC>thead>tr").find('th.MontoAntT');
    t3.css("text-align", "left");
    var t4 = $("#tableOC>thead>tr").find('th.AntAmort');
    t4.css("text-align", "left");
    var t5 = $("#tableOC>thead>tr").find('th.PorAnt');
    t5.css("text-align", "left");
    var t6 = $("#tableOC>thead>tr").find('th.AntSol');
    t6.css("text-align", "left");
    var t7 = $("#tableOC>thead>tr").find('th.AntTr');
    t7.css("text-align", "left");
    //--------
    $("#tableOC > tbody  > tr[role='row']").each(function () {
        //0
        var R0 = $(this).find("td.BRTWR");
        R0.css("text-align", "left");
        //1
        var R1 = $(this).find("td.FondoGarantia");
        R1.css("text-align", "left");
        //2
        var R2 = $(this).find("td.AmortAnt");
        R2.css("text-align", "left");
        //3
        var R3 = $(this).find("td.MontoAntT");
        R3.css("text-align", "left");
        //4
        var R4 = $(this).find("td.AntAmort");
        R4.css("text-align", "left");
        //5
        var R5 = $(this).find("td.PorAnt");
        R5.css("text-align", "left");
        //6
        var R6 = $(this).find("td.AntSol");
        R6.css("text-align", "left");
        //7
        var R7 = $(this).find("td.AntTr");
        R7.css("text-align", "left");
    });
}
//LEJGG 11/12/2018------------------------------------------------T