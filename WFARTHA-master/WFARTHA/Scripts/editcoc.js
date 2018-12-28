$(document).ready(function () { });

//LEJGG 11/12/2018------------------------------------------------I
//$('body').on('change', '#norden_compra', function (event, param1) {
//    var eb = $(this).val();
//    $.ajax({
//        type: "POST",
//        url: '../getEKKOInfo',
//        dataType: "json",
//        data: { "ebeln": $(this).val() },
//        success: function (data) {
//            var ekko = data.ekmo;
//            var cuentas = data.res;
//            var mtr = data.mtr;
//            var brtwr = data.brtwr;
//            llenarTablaOc(ekko, cuentas, mtr, brtwr);
//        }
//    });

//    $.ajax({
//        type: "POST",
//        url: '../getEKBEInfo',
//        dataType: "json",
//        async: false,
//        data: { "ebeln": $(this).val() },
//        success: function (data) {
//            llenarTablaOc2(data, eb);
//        }
//    });

//    $.ajax({
//        type: "POST",
//        url: '../getEKPOInfo',
//        dataType: "json",
//        data: { "ebeln": $(this).val() },
//        success: function (data) {
//            armarTabla(data);
//        }
//    });
//});

//LEJGG 22-12-2018----------------------------------------I
function Tabla2oc() {
    var nd = $("#NUM_DOC").val();
    $.ajax({
        type: "POST",
        url: '../getAmorAntTable',
        dataType: "json",
        data: { "id": nd },
        async: false,
        success: function (data) {
            llenarTablaOc2(data);
            formatoOC2();
        }
    });
}

function tablaDet() {
    var nd = $("#NUM_DOC").val();
    $.ajax({
        type: "POST",
        url: '../getDocumentoPs',
        dataType: "json",
        data: { "id": nd },
        async: false,
        success: function (data1) {
            var d1 = data1;
            $.ajax({
                type: "POST",
                url: '../getDocumentoCOC',
                dataType: "json",
                data: { "id": nd },
                async: false,
                success: function (data2) {
                    var d2 = data2;
                    armarTablaDet(d1, d2);
                    updateFooterP();
                    formatoTcoc();
                }
            });
        }
    });
}

function llenarTablaOc2(val) {
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
        var mon = val[i].WAERS;
        var tant = val[i].TANT;
        var mt = val[i].ANTAMOR;
        var at = val[i].ANTTRANS;
        var axam = val[i].ANTXAMORT;
        tabl.row.add([
            ebelp,//POSC
            buzei,//POS
            belnr,//numdoc
            gjar,//EJERCICIO
            toShow(mt),//Anticipo amortizado
            toShow(tant),//total anticipo
            mon,//MONEDA,
            toShow(at),//anticipo en transito
            "<input class=\"ANTXAMORT\" style=\"font-size:12px;\" type=\"text\" id=\"antxamor\" name=\"\" value=\"" + toShow(axam) + "\">"
        ]).draw(false).node();
    }
    updateTotalesOC2();
}
//LEJGG 22-12-2018----------------------------------------T

//LEJGG 21-12-2018
function mostrarTabla(ban) {
    if (ban === "SCO") {
        $("#div_sinPedido").addClass("hide");
        $("#div_conPedido").removeClass("hide");
        $("#div_garantia").removeClass("hide");
        $("#divTot").addClass("hide");
        $("#divTotCO").removeClass("hide");
        $("#conOrden").val("X");
    } else {
        $("#div_conPedido").addClass("hide");
        $("#div_sinPedido").removeClass("hide");
        $("#div_garantia").addClass("hide");
        $("#divTot").removeClass("hide");
        $("#divTotCO").addClass("hide");
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
    updateFooterP();//lejgg-18-12-2018
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
function addRowInfoP(t, POS, NumAnexo, NumAnexo2, NumAnexo3, NumAnexo4, NumAnexo5, MATERIAL, CA, FACTURA,
    TIPO_CONCEPTO, GRUPO, CUENTA, CUENTANOM, TIPOIMP, IMPUTACION, CCOSTO, MONTO, MONEDA, CANTIDAD, UNIDAD, IMPUESTO, IVA, TEXTO, TOTAL, PEP) { //MGC 03 - 10 - 2018 solicitud con orden de compra
    var r = addRowlP(
        t,
        "<input  class='NumAnexo' style='font-size:12px;width:21px' type='text' id='' name='' value='" + NumAnexo + "'>",
        "<input  class='NumAnexo2' style='font-size:12px;width:21px' type='text' id='' name='' value='" + NumAnexo2 + "'>",
        "<input  class='NumAnexo3' style='font-size:12px;width:21px' type='text' id='' name='' value='" + NumAnexo3 + "'>",
        "<input  class='NumAnexo4' style='font-size:12px;width:21px' type='text' id='' name='' value='" + NumAnexo4 + "'>",
        "<input  class='NumAnexo5' style='font-size:12px;width:21px' type='text' id='' name='' value='" + NumAnexo5 + "'>",
        POS,
        MATERIAL,
        TEXTO,
        CA,
        FACTURA,
        TIPO_CONCEPTO,
        GRUPO,
        CCOSTO,
        PEP,
        CUENTA,
        CUENTANOM,
        TIPOIMP,
        IMPUTACION,
        "<input  class='MONTOP' style='font-size:12px;width:90px;' type='text' id='' name='' value='" + toShow(MONTO) + "'>",
        MONEDA,
        "<input  class='CANTIDADP' style='font-size:12px;width:90px;' type='text' id='' name='' value='" + CANTIDAD + "'>",
        UNIDAD,
        IMPUESTO,
        IVA,
        toShow(TOTAL)
    );

    return r;
}

function addRowlP(t, nA, nA2, nA3, nA4, nA5, pos, mtr, txt, ca, factura, tc, grupo, ccentro, pep, cuenta, cuentanombre, tipoimpt, imput, monto, moneda, cantidad, unidad, impuesto, iva, total) {
    var colstoAdd = "";
    for (i = 0; i < extraCols; i++) {
        colstoAdd += '<td class=\"BaseImp' + tRet2[i] + '\"><input class=\"extrasCP BaseImp' + i + '\" style=\"font-size:12px;width:76px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td>';
        colstoAdd += '<td class=\"ImpRet' + tRet2[i] + '\"><input class=\"extrasC2P ImpRet' + i + '\" style=\"font-size:12px;width:76px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td>';
    }
    var table_rows = '<tr><td>' + pos + '</td><td><input class=\"NumAnexo\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td><input class=\"NumAnexo2\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td><input class=\"NumAnexo3\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td><input class=\"NumAnexo4\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td><input class=\"NumAnexo5\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td>' +
        '<td> ' + mtr + '</td><td> ' + txt + '</td><td>' + ca + '</td><td>' + factura + '</td><td>' + tc + '</td><td>' + grupo + '</td><td>' + ccentro + '</td><td>' + pep + '</td><td>' + cuenta + '</td><td>' + cuentanombre
        + '</td><td>' + tipoimpt + '</td><td>' + imput + '</td><td>' + monto + '</td><td>' + moneda + '</td><td>' + cantidad + '</td><td>' + unidad + '</td><td>' + impuesto + '</td><td></td><td>' + total + '</td>' + colstoAdd + '</tr>';
    //Lej 11.12.2018--------------------------------
    if (colstoAdd == "") {
        var r = t.row.add([
            pos,
            nA,
            nA2,
            nA3,
            nA4,
            nA5,
            mtr,//Material
            txt,//Texto
            ca,
            factura,//Factura
            tc,//
            grupo,//Grupo
            ccentro,//CECO
            pep,//PEP
            cuenta,
            cuentanombre,
            tipoimpt,
            imput,
            monto,//Monto
            moneda,//Moneda
            cantidad,//Cantidad
            unidad,//Unidad
            impuesto,//Impuesto
            "",//Iva        
            total//TOTAL
        ]).draw(false).node();
    }
    else {
        var r = t.row.add(
            $(table_rows)//Lej 19.12.2018
        ).draw(false).node();
    }
    return r;
}

function copiarTableInfoPControl() {

    //Tambien pasar los datos de la tabla "tableoc"
    var ebeln = $("#norden_compra").val();
    $("#tableOC tbody tr[role='row']").each(function () {
        var _t = $(this);
        var brtwr = _t.find("td.BRTWR").val();
        var retpc = _t.find("td.FondoGarantia").text();
        var porant = _t.find("td.PorAnt").text();
        //
        $('#EBELN').val(ebeln.toString());
        /*$('#AMOR_ANT').val(parseFloat(toNum(amor_ant)));
        $('#RETPC').val(parseFloat(toNum(retpc)));*/
    });
    var lengthT = $("table#table_infoP tbody tr[role='row']").length;
    var docsenviar = {};
    var docsenviar2 = {};
    var docsenviar3 = {};//lej01.10.2018
    var docsenviar4 = {};//lejgg 12.12-2018
    var docsenviar5 = {};//lejgg 15-12-2018
    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla oculta y agregarlos al json
        //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
        jsonObjDocs = [];
        jsonObjDocs2 = [];
        jsonObjDocs3 = [];//lej01.10.2018
        jsonObjDocs4 = [];//lejgg 12.12-2018
        jsonObjDocs5 = [];//lejgg 15-12-2018
        var i = 1;
        var t = $('#table_infoP').DataTable();
        var toc = $('#tableOC2').DataTable();
        //Lej 14.09.18---------------------
        //Aqui armo la tabla oculta de acuerdo a los valores y columnas ingresados de retencion
        var taInf = $("#table_inforeth");
        taInf.append($("<thead />"));
        taInf.append($("<tbody />"));
        var thead = $("#table_inforeth thead");
        thead.append($("<tr />"));
        $("#table_inforeth>thead>tr").append("<th>WITHT</th>");
        $("#table_inforeth>thead>tr").append("<th>WT_WITHCD</th>");
        $("#table_inforeth>thead>tr").append("<th>I. Ret.</th>");//Imp Ret
        $("#table_inforeth>thead>tr").append("<th>B.Imp.</th>");//Base imponible
        //Lej 14.09.18----------------
        $("#table_infoP > tbody  > tr[role='row']").each(function () {
            //Obtener el row para el plugin
            var tr = $(this);
            var indexopc = t.row(tr).index();
            var tconcepto = "";

            //LEJ 03-10-2018
            //MGC 11-10-2018 Obtener valor de columnas ocultas --------------------------->
            //Obtener la cuenta
            var cuenta = t.row(indexopc).data()[14];
            if (cuenta == "") {
                cuenta = "S/I";
            }
            //Obtener la imputación
            var imputacion = t.row(indexopc).data()[17];

            //MGC 11-10-2018 Obtener valor de columnas ocultas <---------------------------
            //Lej 14.08.2018-------------------------------------------------------------I
            var colsAdded = tRet2.length;//Las retenciones que se agregaron a la tabla
            var retTot = tRet.length;//Todas las retenciones
            //Lej 14.08.2018-------------------------------------------------------------T
            var pos = parseInt($(this).find("td.POS").text());
            var ca = t.row(indexopc).data()[8];//lejgg 09-10-2018 Conceptos
            if (ca == "") {
                ca = "D";
            }
            var factura = t.row(indexopc).data()[9];//lejgg 12-12-2018
            var grupo = $(this).find("td.GRUPO").text();
            tconcepto = grupo;//13-12-2018
            var cuentanom = t.row(indexopc).data()[15];
            if (cuentanom == "") {
                cuentanom = "S/I";
            }
            var tipoimp = t.row(indexopc).data()[16];
            var ccosto = $(this).find("td.CCOSTO").text(); //MGC 11-10-2018 Obtener valor de columnas oculta
            var impuesto = t.row(indexopc).data()[22];
            var pep = $(this).find("td.PEP").text(); //lejgg 12-12-2018
            var material = $(this).find("td.MATERIAL").text(); //lejgg 12-12-2018
            var moneda = $(this).find("td.MONEDA").text(); //lejgg 12-12-2018
            var unidad = $(this).find("td.UNIDAD").text(); //lejgg 12-12-2018
            var cantidad = $(this).find("td.CANTIDAD input").val();
            while (cantidad.indexOf(',') > -1) {
                cantidad = cantidad.replace('$', '').replace(',', '');
            }
            var monto1 = $(this).find("td.MONTO input").val().replace('$', '').replace(',', '');
            while (monto1.indexOf(',') > -1) {
                monto1 = monto1.replace('$', '').replace(',', '');
            }
            monto1 = monto1.replace(/\s/g, '');
            var monto = toNum(monto1);
            var iva1 = $(this).find("td.IVA select").val();
            iva1 = iva1.replace(/\s/g, '');
            var total1 = $(this).find("td.TOTAL").text().replace('$', '');
            var texto = $(this).find("td.TXTPOS").text();//LEJ 14.09.2018
            while (total1.indexOf(',') > -1) {
                total1 = total1.replace('$', '').replace(',', '');
            }
            var total = parseFloat(total1);
            //Para anexos
            //-----------------------

            var item3 = {};
            var an1 = $(this).find("td.NumAnexo input").val();
            var an2 = $(this).find("td.NumAnexo2 input").val();
            var an3 = $(this).find("td.NumAnexo3 input").val();
            var an4 = $(this).find("td.NumAnexo4 input").val();
            var an5 = $(this).find("td.NumAnexo5 input").val();
            item3["a1"] = an1;
            item3["a2"] = an2;
            item3["a3"] = an3;
            item3["a4"] = an4;
            item3["a5"] = an5;
            jsonObjDocs3.push(item3);
            item3 = "";

            //LEJGG-12-12-2018-----------------------
            var item4 = {};
            item4["POS"] = pos;
            item4["MATNR"] = material;
            item4["PS_PSP_PNR"] = pep;
            item4["WAERS"] = moneda;
            item4["MEINS"] = unidad;
            item4["MENGE_BIL"] = cantidad;
            item4["TOTAL"] = total;
            jsonObjDocs4.push(item4);
            item4 = "";
            //LEJGG-12-12-2018-----------------------
            //-----------------------
            for (j = 0; j < tRet2.length; j++) {
                //llenare mis documentorp's
                var item2 = {};
                item2["NUM_DOC"] = 0;
                item2["POS"] = pos;
                item2["WITHT"] = tRet2[j];
                item2["WT_WITHCD"] = "01";
                item2["BIMPONIBLE"] = parseFloat($(this).find("td.BaseImp" + tRet2[j] + " input").val().replace('$', '').replace(',', ''));
                item2["IMPORTE_RET"] = parseFloat($(this).find("td.ImpRet" + tRet2[j] + " input").val().replace('$', '').replace(',', ''));
                jsonObjDocs2.push(item2);
                item2 = "";
            }

            var item = {};

            item["NUM_DOC"] = 0;
            item["POS"] = "";
            item["ACCION"] = ca;
            item["FACTURA"] = factura;
            item["TCONCEPTO"] = tconcepto;
            item["GRUPO"] = grupo;
            item["CUENTA"] = cuenta;
            item["NOMCUENTA"] = cuentanom;
            item["TIPOIMP"] = tipoimp;
            item["IMPUTACION"] = pep;
            item["CCOSTO"] = ccosto;
            item["MONTO"] = monto;
            item["MWSKZ"] = iva1;
            item["IVA"] = "";//porcentaje del iva
            item["TEXTO"] = texto;
            item["TOTAL"] = total;

            jsonObjDocs.push(item);
            i++;
            item = "";

        });

        //LEJGG 15-12-2018
        $("#tableOC2 > tbody  > tr[role='row']").each(function () {
            //Obtener el row para el plugin
            var tr = $(this);
            var indexopc = toc.row(tr).index();

            var posc = $(this).find("td.POSC").text(); //MGC 11-10-2018 Obtener valor de columnas oculta
            //var pos = $(this).find("td.POS").text(); //lejgg 12-12-2018
            var pos = toc.row(indexopc).data()[1]; //lejgg 12-12-2018
            var ndoc = $(this).find("td.NDOC").text(); //lejgg 12-12-2018
            var ejercicio = $(this).find("td.EJERCICIO").text(); //lejgg 12-12-2018
            var antamor = $(this).find("td.ANTAMOR").text().replace('$', ''); //lejgg 12-12-2018
            while (antamor.indexOf(',') > -1) {
                antamor = antamor.replace('$', '').replace(',', '');
            }
            var toant = $(this).find("td.TOANT").text().replace('$', '');
            while (toant.indexOf(',') > -1) {
                toant = toant.replace('$', '').replace(',', '');
            }
            var moneda = tr.find("td.MONEDA").text();
            //var anttr = $(this).find("td.AntTr").text().replace('$', '');
            //while (anttr.indexOf(',') > -1) {
            //    anttr = anttr.replace('$', '').replace(',', '');
            //}
            var anttr = toc.row(indexopc).data()[7];
            anttr = anttr.toString().replace('$', '');
            while (anttr.indexOf(',') > -1) {
                anttr = anttr.replace('$', '').replace(',', '');
            }
            var antxamor = $(this).find("td.AntXAMOR input").val().replace('$', '');
            if (antxamor == "") {
                antxamor = "0";
            }
            while (antxamor.indexOf(',') > -1) {
                antxamor = antxamor.replace('$', '').replace(',', '');
            }
            //-----------------------

            var item5 = {};
            item5["EBELN"] = $('#norden_compra').val();
            item5["EBELP"] = posc;
            item5["BUZEI"] = parseFloat(pos);
            item5["BELNR"] = ndoc;
            item5["GJAHR"] = parseFloat(ejercicio);
            item5["ANTAMOR"] = parseFloat(antamor);
            item5["TANT"] = parseFloat(toant);
            item5["WAERS"] = moneda;
            item5["ANTTRANS"] = parseFloat(anttr);
            item5["ANTXAMORT"] = parseFloat(antxamor);
            jsonObjDocs5.push(item5);
            item5 = "";
        });

        docsenviar = JSON.stringify({ 'docs': jsonObjDocs });
        $.ajax({
            type: "POST",
            url: '../getPartialCon',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {
                if (data !== null || data !== "") {
                    $("table#table_infoh tbody").append(data);
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });

        docsenviar2 = JSON.stringify({ 'docs': jsonObjDocs2 });
        //Ajax para las retenciones en la tabla de info
        $.ajax({
            type: "POST",
            url: '../getPartialCon2',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar2,
            success: function (data) {
                if (data !== null || data !== "") {
                    $("table#table_inforeth tbody").append(data);
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });

        docsenviar3 = JSON.stringify({ 'docs': jsonObjDocs3 });
        $.ajax({
            type: "POST",
            url: '../getPartialCon3',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar3,
            success: function (data) {
                if (data !== null || data !== "") {
                    $("table#table_infoAnex tbody").append(data);
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });

        docsenviar4 = JSON.stringify({ 'docs': jsonObjDocs4 });
        $.ajax({
            type: "POST",
            url: '../getPartialCon5',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar4,
            success: function (data) {
                if (data !== null || data !== "") {
                    $("table#table_infoPh tbody").append(data);
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });

        //Tabla co2
        docsenviar5 = JSON.stringify({ 'docs': jsonObjDocs5 });
        $.ajax({
            type: "POST",
            url: '../getPartialConOC',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar5,
            success: function (data) {
                if (data !== null || data !== "") {
                    $("table#tableOC2H tbody").append(data);
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }
}

//26-12-2018
$('body').on('focusout', '.ANTXAMORT', function (e) {

    var t = $('#tableOC2').DataTable();
    var tr = $(this).closest('tr'); //Obtener el row 
    var amorant = $(this).val().replace('$', '');
    while (amorant.indexOf(',') > -1) {
        amorant = amorant.replace('$', '').replace(',', '');
    }
    if (amorant == "") {
        amorant = 0;
    }
    amorant = parseFloat(amorant);
    var famant = toShow(amorant);
    tr.find("td.AntXAMOR input").val();
    tr.find("td.AntXAMOR input").val(famant);
    updateTotalesOC2();
});

$('body').on('focusout', '.MONTOP', function (e) {
    var t = $('#table_infoP').DataTable();
    var tr = $(this).closest('tr'); //Obtener el row 
    var indexopc = t.row(tr).index();

    //recupero el iva para buscar su porcentaje y hacer la operacion
    var porc = "";
    var iva1 = tr.find("td.IVA select").val();
    iva1 = iva1.replace(/\s/g, '');
    $.ajax({
        type: "POST",
        url: '../getPorMult',
        data: { "id": iva1 },
        async: false,
        dataType: "json",
        success: function (data) {
            porc = data;
        }
    });


    //
    var monto = $(this).val().replace('$', '').replace(',', ''); //Obtener el row 
    if (monto != "") {
        while (monto.indexOf(',') > -1) {
            monto = monto.replace('$', '').replace(',', '');
        }
    }
    else {
        monto = "0";
    }
    monto = parseFloat(monto);
    //Agrego el nuevo total escondido
    var op = (monto * porc) / 100;
    var _tot = monto + op;
    t.cell(indexopc, 24).data("").draw();//Limpiar las celdas
    t.cell(indexopc, 24).data(toShow(_tot)).draw();//clavar el nuevo valor del total

    var _mt = toShow(monto);
    tr.find("td.MONTO input").val();
    tr.find("td.MONTO input").val(_mt);
    updateFooterP();
});

function updateTotalesOC2() {
    var t_antamor = 0;
    var t_toant = 0;
    var t_antxamor = 0;
    $("#tableOC2 > tbody > tr[role = 'row']").each(function (index) {
        var _this = $(this);
        var tr = $(this).closest('tr'); //Obtener el row 
        var antamor = tr.find("td.ANTAMOR").text().replace('$', '');//Anticipo amortizado
        while (antamor.indexOf(',') > -1) {
            antamor = antamor.replace('$', '').replace(',', '');
        }
        t_antamor = t_antamor + parseFloat(antamor);//realizo la sumatoria
        var toant = tr.find("td.TOANT").text().replace('$', '');//Anticipo total
        while (toant.indexOf(',') > -1) {
            toant = toant.replace('$', '').replace(',', '');
        }
        t_toant = t_toant + parseFloat(toant);//realizo la sumatoria
        var antxamor = tr.find("td.AntXAMOR input").val().replace('$', '');//Anticipo por amortizar
        if (antxamor == "") {//si esta vacia darle un 0 x default
            antxamor = "0";
        }
        while (antxamor.indexOf(',') > -1) {
            antxamor = antxamor.replace('$', '').replace(',', '');
        }
        t_antxamor = t_antxamor + parseFloat(antxamor);
    });
    //pinto los datos
    $("#totalAA").text(toShow(t_antamor));
    $("#totalAnt").text(toShow(t_toant));
    $("#totalAntxAm").text(toShow(t_antxamor));
}
//lejgg 26-12-2018

function updateFooterP() {
    var t = $('#table_infoP').DataTable();
    var total = 0;
    $("#table_infoP > tbody > tr[role = 'row']").each(function (index) {
        /*var mt = $(this).find("td.MONTO input").val().replace('$', '');
        while (mt.indexOf(',') > -1) {
            mt = mt.replace('$', '').replace(',', '');
        }*/
        //Saber si el renglón se va a sumar
        var tr = $(this);
        var indexopc = t.row(tr).index();
        var mt = $(this).find("td.TOTAL").text().replace('$', '');
        mt = mt.toString().replace('$', '');
        while (mt.indexOf(',') > -1) {
            mt = mt.replace('$', '').replace(',', '');
        }
        total = total + parseFloat(mt);
    });

    total = total.toFixed(2);
    $('#MONTO_DOC_MD').val(toShow(total));//Lej 12.12.2018
    $('#mtTot').val($('#MONTO_DOC_MD').val());//Lej 12.12.2018
    $('#total_infoP').text(toShow(total));//LEJGG 26-12-2018
}
function formatoOC2() {
    //--------
    //Para los titulos
    var tpo = $("#tableOC2>thead>tr").find('th.POSC');
    tpo.css("text-align", "left");
    var ts = $("#tableOC2>thead>tr").find('th.NDOC');
    ts.css("text-align", "left");
    var tn = $("#tableOC2>thead>tr").find('th.EJERCICIO');
    tn.css("text-align", "left");
    var mt = $("#tableOC2>thead>tr").find('th.ANTAMOR');
    mt.css("text-align", "left");
    var ct = $("#tableOC2>thead>tr").find('th.TOANT');
    ct.css("text-align", "left");
    var tt = $("#tableOC2>thead>tr").find('th.MONEDA');
    tt.css("text-align", "left");
    var tde = $("#tableOC2>thead>tr").find('th.AntXAMOR');
    tde.css("text-align", "left");
    //--------
    $("#tableOC2 > tbody > tr[role = 'row']").each(function (index) {
        //1
        var R1 = $(this).find("td.POSC");
        R1.css("text-align", "left");
        //2
        var R2 = $(this).find("td.NDOC");
        R2.css("text-align", "left");
        //3
        var R3 = $(this).find("td.EJERCICIO");
        R3.css("text-align", "left");
        //4
        var R4 = $(this).find("td.ANTAMOR");
        R4.css("text-align", "left");
        //5
        var R5 = $(this).find("td.TOANT");
        R5.css("text-align", "left");
        //6
        var R6 = $(this).find("td.MONEDA");
        R6.css("text-align", "left");
        //7
        var R7 = $(this).find("td.AntXAMOR");
        R7.css("text-align", "left");
    });
}

function formatoTcoc() {
    //--------
    //Para los titulos
    var tpo = $("#table_infoP>thead>tr").find('th.POS');
    tpo.css("text-align", "left");
    var ts = $("#table_infoP>thead>tr").find('th.TXTPOS');
    ts.css("text-align", "center");
    var tn = $("#table_infoP>thead>tr").find('th.GRUPO');
    tn.css("text-align", "left");
    var mt = $("#table_infoP>thead>tr").find('th.MONTO');
    mt.css("text-align", "left");
    var ct = $("#table_infoP>thead>tr").find('th.CANTIDAD');
    ct.css("text-align", "left");
    var tt = $("#table_infoP>thead>tr").find('th.PEP');
    tt.css("text-align", "center");
    var tde = $("#table_infoP>thead>tr").find('th.IVA');
    tde.css("text-align", "left");
    var tot = $("#table_infoP>thead>tr").find('th.TOTAL');
    tot.css("text-align", "left");
    //--------
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
}

function ocultarddl() {
    $("#table_infoP tbody tr[role='row']").each(function () {
        //1
        var R1 = $(this).find("td.IVA input");
        R1.css("display", "none");
    });
}