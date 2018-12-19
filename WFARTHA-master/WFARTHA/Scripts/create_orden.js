var extraCols = 0;
//Variables globales
$(document).ready(function () {
    $('#table_infoP').DataTable({

        language: {
            //"url": "../Scripts/lang/@Session["spras"].ToString()" + ".json"
            "url": "../Scripts/lang/ES.json"
        },
        "paging": false,
        "info": false,
        "ordering": false,
        "searching": false,
        columnDefs: [
            {
                targets: [0, 6, 7, 11, 19, 21],
                className: 'mdl-data-table__cell--non-numeric'
            }
        ],
        "columns": [
            {
                "name": 'Fila',
                "className": 'POS',
                "orderable": false
            },
            {
                "name": 'A1',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo',
                "orderable": false
            },
            {
                "name": 'A2',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo2',
                "orderable": false
            },
            {
                "name": 'A3',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo3',
                "orderable": false
            },
            {
                "name": 'A4',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo4',
                "orderable": false
            },
            {
                "name": 'A5',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo5',
                "orderable": false
            },
            {
                "name": 'MATERIAL',
                "className": 'MATERIAL',
                "orderable": false
            },
            {
                "name": 'TXTPOS',
                "className": 'TXTPOS',
                "orderable": false
            },
            {
                "name": 'CA',
                "className": 'CA',
                "orderable": false,
                "visible": false
            },
            {
                "name": 'FACTURA',
                "className": 'FACTURA',
                "orderable": false,
                "visible": false
            },
            {
                "name": 'TCONCEPTO',
                "className": 'TCONCEPTO',
                "orderable": false,
                "visible": false//MGC 22-10-2018 Etiquetas
            },
            {
                "name": 'CONCEPTO',
                "className": 'GRUPO',
                "orderable": false
            },
            {
                "name": 'CCOSTO',
                "className": 'CCOSTO',
                "orderable": false
            },
            {
                "name": 'PEP',
                "className": 'PEP',
                "orderable": false
            },
            {
                "name": 'CUENTA',
                "className": 'CUENTA',
                "orderable": false,
                "visible": false//lej 11.09.2018
            },
            {
                "name": 'CUENTANOM',
                "className": 'CUENTANOM',
                "orderable": false,
                "visible": false//lej 11.09.2018
            },
            {
                "name": 'TIPOIMP',
                "className": 'TIPOIMP',
                "orderable": false,
                "visible": false//MGC 22-10-2018 Etiquetas
            },
            {
                "name": 'IMPUTACION',
                "className": 'IMPUTACION',
                "orderable": false,
                "visible": false//lej 11.09.2018
            },
            {
                "name": 'MONTO',
                "className": 'MONTO',
                "orderable": false
            },
            {
                "name": 'MONEDA',
                "className": 'MONEDA',
                "orderable": false

            },
            {
                "name": 'CANTIDAD',
                "className": 'CANTIDAD',
                "orderable": false
            },
            {
                "name": 'UNIDAD',
                "className": 'UNIDAD',
                "orderable": false
            },
            {
                "name": 'IMPUESTOP',
                "className": 'IMPUESTOP',
                "orderable": false,
                "visible": false
            },
            {
                "name": 'IVA',
                "className": 'IVA',
                "orderable": false
            },
            {
                "name": 'TOTAL',
                "className": 'TOTAL',
                "orderable": false,
                "visible": false //
            }
        ]
    });

    formatoTcoc();//lejgg 12-12-2018

    $('#tableOC').DataTable({
        language: {
            "url": "../Scripts/lang/ES.json"
        },
        "paging": false,
        "info": false,
        "ordering": false,
        "searching": false,
        "columns": [
            {
                "className": 'BRTWR',
                "defaultContent": '',
                "orderable": false
            },
            {
                "className": 'FondoGarantia',
                "defaultContent": '',
                "orderable": false
            },
            {
                "name": 'PorAnt',
                "className": 'PorAnt',
                "orderable": false,
                "visible": true
            }//,
            //{
            //    "name": 'AntSol',
            //    "className": 'AntSol',
            //    "orderable": false,
            //    "visible": true
            //},
            //{
            //    "name": 'MontoAntT',
            //    "className": 'MontoAntT',
            //    "orderable": false,
            //    "visible": true
            //},
            //{
            //    "name": 'AntAmort',
            //    "className": 'AntAmort',
            //    "orderable": false,
            //    "visible": true
            //},
            //{
            //    "name": 'AntTr',
            //    "className": 'AntTr',
            //    "orderable": false,
            //    "visible": true
            //},
            //{
            //    "name": 'AmortAnt',
            //    "className": 'AmortAnt',
            //    "orderable": false,
            //    "visible": true
            //}
        ]
    });

    $('#tableOC2').DataTable({
        language: {
            "url": "../Scripts/lang/ES.json"
        },
        "paging": false,
        "info": false,
        "ordering": false,
        "searching": false,
        "columns": [
            {
                "className": 'POSC',
                "defaultContent": '',
                "orderable": false
            },
            {
                "className": 'POS',
                "defaultContent": '',
                "orderable": false,
                "visible": false
            },
            {
                "className": 'NDOC',
                "defaultContent": '',
                "orderable": false
            },
            {
                "className": 'EJERCICIO',
                "orderable": false,
                "visible": true
            },
            {
                "className": 'ANTAMOR',
                "orderable": false,
                "visible": true
            },
            {
                "name": 'TOANT',
                "className": 'TOANT',
                "orderable": false,
                "visible": true
            },
            {
                "className": 'MONEDA',
                "defaultContent": '',
                "orderable": false
            },
            {
                "name": 'AntTr',
                "className": 'AntTr',
                "orderable": false,
                "visible": true
            },
            {
                "className": 'AntXAMOR',
                "orderable": false,
                "visible": true
            }
        ]
    });

    $('#table_infoP tbody').on('click', 'td.select_row', function () {
        $(tr).toggleClass('selected');
    });

    $('body').on('focusout', '.OPERP', function (e) {

        var t = $('#table_infop').DataTable();
        var tr = $(this).closest('tr'); //Obtener el row 

        //Obtener el valor del impuesto
        var imp = tr.find("td.IMPUESTOP").find("select").val();

        //Calcular impuesto y subtotal
        var impimp = impuestoVal(imp);
        impimp = parseFloat(impimp);

        //Desde el total
        if ($(this).hasClass("TOTAL")) {

            var total = $(this).val();
            total = parseFloat(total);

            var impv = (total * impimp) / 100;
            impv = parseFloat(impv);
            var sub = total - impv;

            impv = toShow(impv);
            sub = toShow(sub);
            total = toShow(total);

            //Enviar los valores a la tabla
            //Subtotal
            tr.find("td.MONTO_F input").val();
            tr.find("td.MONTO_F input").val(sub);

            //IVA
            tr.find("td.IVA input").val();
            tr.find("td.IVA input").val(impv);

            //Total
            tr.find("td.TOTAL input").val();
            tr.find("td.TOTAL input").val(total);


        }
        else if ($(this).hasClass("MONTO_F")) {

            //Desde el subtotal
            var sub = $(this).val().replace('$', '').replace(',', '');
            sub = parseFloat(sub);

            //Lleno los campos de Base Imponible con el valor del monto
            for (x = 0; x < tRet2.length; x++) {
                var _xvalue = tr.find("td.BaseImp" + tRet2[x] + " input").val();
                if (_xvalue === "") {
                    tr.find("td.BaseImp" + tRet2[x] + " input").val(toShow(sub));
                    //Ejecutamos un ajax para llenar el valor de importe de retencion
                    var _res = porcentajeImpRet(tRet2[x]);
                    _res = (sub * _res) / 100;//Saco el porcentaje
                    tr.find("td.ImpRet" + tRet2[x] + " input").val(toShow(_res));
                }
            }
            //Ejecutamos el metodo para sumarizar las columnas
            var colTotal = sumarColumnasExtras(tr);

            // rimpimp = 100 - impimp;

            var impv = (sub * impimp) / 100;
            impv = parseFloat(impv);
            var total = sub + impv;
            total = parseFloat(total);

            sub = total - impv;

            //-------------------------------------------------------
            var por = $("#por_ant").text();
            por = toNum(por);
            por = parseFloat(por);
            tr.find("td.ANT_EST input").val(toShow(sub * (por / 100)));
            //-------------------------------------------------------


            impv = toShow(impv);
            sub = toShow(sub);
            total = toShow(total);

            //Enviar los valores a la tabla
            //Subtotal
            tr.find("td.MONTO_F input").val();
            tr.find("td.MONTO_F input").val(sub);

            //IVA
            tr.find("td.IVA input").val();
            tr.find("td.IVA input").val(impv);

            //Total
            tr.find("td.TOTAL input").val();
            if (colTotal > 0) {
                var sumt = parseFloat(total.replace('$', '').replace(',', '')) - parseFloat(colTotal);
                tr.find("td.TOTAL input").val(toShow(sumt));
            }
            else {
                tr.find("td.TOTAL input").val(total);
            }
        }
        else if ($(this).hasClass("ANT_EST")) {
            $(this).val(toShow($(this).val()));
        }
        else {
            $(this).val(toShowNum($(this).val()));
        }
        updateFooterP();
        //$(".extrasPC").trigger("focusout"); //lej18102018
    });

    //LEJGG 10/12/2018---------------I
    $('body').on('focusout', '#PAYER_ID', function (e) {
        llenarCOC();
    });
    //LEJGG 10/12/2018---------------T
});

var pedidosSel = [];

//$('body').on('keydown.autocomplete', '#norden_compra', function () {
//    var tr = $(this).closest('tr'); //Obtener el row
//    var t = $('#table_infoP').DataTable();

//    //Obtener el id de la sociedad
//    var prov = $("#PAYER_ID").val();
//    var pedidosNum = [];
//    //if (prov.trim() !== "") {
//    //    pedidosNum = ["4000000001", "4000000002", "4000000003", "4000000004", "4000000005"];
//    //}
//    auto(this).autocomplete({
//        source: function (request, response) {
//            auto.ajax({
//                type: "POST",
//                url: 'getPedidos',
//                dataType: "json",
//                data: { "Prefix": request.term, "lifnr": prov },
//                success: function (data) {
//                    response(auto.map(data, function (item) {
//                        //return { label: trimStart('0', item.LIFNR) + " - " + item.NAME1, value: trimStart('0', item.LIFNR) };
//                        return { label: trimStart('0', item.EBELN), value: trimStart('0', item.EBELN) };
//                    }))
//                }
//            })
//            //pedidosNum
//        }
//        ,
//        messages: {
//            noResults: '',
//            results: function (resultsCount) { }
//        },
//        change: function (e, ui) {
//            if (!(ui.item)) {
//                e.target.value = "";
//                t.rows().remove().draw(false);
//            }
//        },
//        select: function (event, ui) {
//            pedidosSel = [];
//            var label = ui.item.label;
//            var value = ui.item.value;
//            //for (var i = 0; i < pedidos.length; i++) {
//            //    if (pedidos[i].NUM_PED == value)
//            //        pedidosSel.push(pedidos[i]);
//            //}
//            ////alert(pedidosSel);
//            addPedido(value);
//        }
//    });
//});

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
            data: { "ebeln": eb, "belnr": belnr },
            success: function (data) {
                mt = data;
                //
                //calculo de anticipo en transito
                $.ajax({
                    type: "POST",
                    url: 'calculoAntTr',
                    dataType: "json",
                    success: function (datax) {
                        at = datax;
                        //
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
                    }
                });
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
//LEJGG 11/12/2018------------------------------------------------T

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

function llenaOrdenes(lifnr, bukrs) {
    var tr = $(this).closest('tr'); //Obtener el row
    var t = $('#table_infoP').DataTable();

    var pedidosNum = [];
    $("#norden_compra").empty();

    $.ajax({
        type: "POST",
        url: 'getPedidos',
        dataType: "json",
        data: { "lifnr": lifnr.trim(), "bukrs": bukrs },
        success: function (data) {
            $("#norden_compra").empty();
            for (var i = 0; i < data.length; i++) {
                var ebeln = data[i];
                $("#norden_compra").append($("<option>").attr('value', ebeln).text(ebeln));
            }

            var elem = document.querySelectorAll("select");
            var instance = M.Select.init(elem, []);
        }
    });
}

function mostrarTabla(ban) {
    if (ban === "False") {
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

function addPedido(ebeln) {
    var t = $('#table_infoP').DataTable();
    var ti = $('#table_info').DataTable();

    t.rows().remove().draw(false);
    ti.rows().remove().draw(false);

    document.getElementById("loader").style.display = "initial";
    auto.ajax({
        type: "POST",
        url: 'getFondos',
        dataType: "json",
        data: { ebeln: ebeln },
        success: function (data) {
            if (data.length > 0) {
                //$("#fondo_g").val(toShow(data[0].FONDOG));
                //$("#Rfondo_g").val(toShow(data[0].RET_FONDOG));
                //$("#Resfondo_g").val(toShow(data[0].RES_FONDOG));
                //$("label[for='fondo_g']").addClass("active");
                //$("label[for='Rfondo_g']").addClass("active");
                //$("label[for='Resfondo_g']").addClass("active");

                $("#fondo_g").text(toShow(data[0].FONDOG));
                $("#Rfondo_g").text(toShow(data[0].RET_FONDOG));
                $("#Resfondo_g").text(toShow(data[0].RES_FONDOG));
                $("#totPed").text(toShow(data[0].TOTAL));
                $("#por_ant").text(toShowPorc(data[0].POR_ANTICIPO));
                $("#por_fondo").text(toShowPorc(data[0].POR_FONDO));
            }
            document.getElementById("loader").style.display = "none";
        },
        error: function (x) {
            alert(x);
            document.getElementById("loader").style.display = "none";
        },
        sync: false
    });


    document.getElementById("loader").style.display = "initial";
    auto.ajax({
        type: "POST",
        url: 'getPedidosPos',
        dataType: "json",
        data: { ebeln: ebeln },
        success: function (data) {
            var P = data;

            var posinfo = 0;
            for (var i = 0; i < P.length; i++) {
                var addedRowInfo = addRowInfoP(t, P[i].EBELP, "", "", "", "", "", "D", "", "", "", "", "", "", "", "", P[i].MENGE, "", "", "", "", "", "", P[i]);//Lej 13.09.2018 //MGC 03-10-2018 solicitud con orden de compra
                posinfo = i + 1;

                //Obtener el select de impuestos en la cabecera
                var idselect = "infoSel" + posinfo;

                //Obtener el valor 
                var imp = $('#IMPUESTO').val();
                addSelectImpuestoP(addedRowInfo, imp, idselect, "", "X");
                updateFooterP();
            }
            document.getElementById("loader").style.display = "none";
        },
        error: function (x) {
            alert(x);
            document.getElementById("loader").style.display = "none";
        },
        sync: false
    });

    llenarRetencionesIRetP();
    llenarRetencionesBImp();
}

$('#tab_enc').on("click", function (e) {
    if (!conOrden()) {
        llenarRetencionesIRet();
        llenarRetencionesBImp();
    } else {
        llenarRetencionesIRetP();
        llenarRetencionesBImpP();
    }
});

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
        "<input  class='MONTOP' style='font-size:12px;width:90px;' type='text' id='' name='' value='" + MONTO + "'>",
        MONEDA,
        "<input  class='CANTIDADP' style='font-size:12px;width:90px;' type='text' id='' name='' value='" + CANTIDAD + "'>",
        UNIDAD,
        IMPUESTO,
        IVA,
        TOTAL
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
        var mt = t.row(indexopc).data()[24];
        mt = mt.toString().replace('$', '');
        while (mt.indexOf(',') > -1) {
            mt = mt.replace('$', '').replace(',', '');
        }
        total = total + parseFloat(mt);
    });

    total = total.toFixed(2);
    $('#MONTO_DOC_MD').val(toShow(total));//Lej 12.12.2018
    $('#mtTot').val($('#MONTO_DOC_MD').val());//Lej 12.12.2018
}

$('body').on('change', '.IMPUESTOP_SELECT', function (event, param1) {

    if (param1 != "tr") {
        //Modificación del sub, iva y total

        var t = $('#table_infop').DataTable();
        var tr = $(this).closest('tr'); //Obtener el row 

        //Obtener el valor del impuesto
        var imp = tr.find("td.IMPUESTOP").find("select").val();

        //Calcular impuesto y subtotal
        var impimp = impuestoVal(imp);
        impimp = parseFloat(impimp);
        var colTotal = sumarColumnasExtras(tr);//lej 19.08.18

        var sub = tr.find("td.MONTO_F input").val().replace('$', '').replace(',', '');
        sub = parseFloat(sub);

        //rimpimp = 100 - impimp;//lej 19.08.18

        var impv = (sub * impimp) / 100;
        impv = parseFloat(impv);
        var total = sub + impv;
        total = parseFloat(total);

        impv = toShow(impv);
        sub = toShow(sub);
        total = toShow(total);

        //Enviar los valores a la tabla
        //Subtotal
        tr.find("td.MONTO_F input").val();
        tr.find("td.MONTO_F input").val(sub);

        //IVA
        tr.find("td.IVA input").val();
        tr.find("td.IVA input").val(impv);

        //Total
        tr.find("td.TOTAL input").val();
        if (colTotal > 0) {
            var sumt = parseFloat(total.replace('$', '').replace(',', '')) + parseFloat(colTotal);
            tr.find("td.TOTAL input").val(toShow(sumt));
        }
        else {
            tr.find("td.TOTAL input").val(total);
        }
        updateFooterP();
    }

});

function resetFooterP() {
    $('#total_infoP').text("$0");
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
            var monto1 = $(this).find("td.MONTO input").val();
            while (monto1.indexOf(',') > -1) {
                monto1 = monto1.replace('$', '').replace(',', '');
            }
            monto1 = monto1.replace(/\s/g, '');
            var monto = toNum(monto1);
            var iva1 = $(this).find("td.IVA select").val();
            iva1 = iva1.replace(/\s/g, '');
            var total1 = t.row(indexopc).data()[24].toString().replace('$', '');
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
            var moneda = $(this).find("td.MONEDA").text();
            var anttr = $(this).find("td.AntTr").text().replace('$', '');
            while (anttr.indexOf(',') > -1) {
                anttr = anttr.replace('$', '').replace(',', '');
            }
            var antxamor = $(this).find("td.AntXAMOR input").val().replace('$', '');
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
            url: 'getPartialCon',
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
            url: 'getPartialCon2',
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
            url: 'getPartialCon3',
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
            url: 'getPartialCon5',
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
            url: 'getPartialConOC',
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

function conOrden() {
    if ($("#conOrden").val() === "X")
        return true;
    else
        return false;
}

function obtenerRetencionesP(flag) {
    //Obtener la sociedad
    var sociedad_id = $("#SOCIEDAD_ID").val();
    var proveedor = $("#PAYER_ID").val();//lej 05.09.2018

    //Validar que los campos tengan valores
    if ((sociedad_id !== "" & sociedad_id !== null) & (proveedor !== "" & proveedor !== null)) {
        //Enviar los valores actuales de la tabla de retenciones
        var lengthT = $("table#table_ret tbody tr[role='row']").length;
        var docsenviar = {};
        var jsonObjDocs = [];
        if (lengthT > 0) {
            //Obtener los valores de la tabla para agregarlos a la tabla oculta y agregarlos al json
            //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin

            var i = 1;
            var t = $('#table_ret').DataTable();

            $("#table_ret > tbody  > tr[role='row']").each(function () {

                //Obtener el row para el plugin
                var tr = $(this);
                var indexopc = t.row(tr).index();

                //Obtener la sociedad oculta
                var soc = t.row(indexopc).data()[0];

                //Obtener el proveedor oculto
                var prov = t.row(indexopc).data()[1];

                //Obtener valores visibles en la tabla
                var tret = toNum($(this).find("td.TRET").text());
                var indret = toNum($(this).find("td.INDRET").text());
                var bimponible = $(this).find("td.BIMPONIBLE").text();
                var imret = $(this).find("td.IMPRET").text();

                //Quitar espacios
                bimponible = bimponible.replace(/\s/g, '');
                imret = imret.replace(/\s/g, '');

                //Conversión a número
                bimponible = toNum(bimponible);
                imret = toNum(imret);

                var item = {};

                //Agregar los valores para enviarlos al modelo
                item["LIFNR"] = prov;
                item["BUKRS"] = soc;
                item["WITHT"] = tret;
                item["WT_WITHCD"] = indret;
                item["POS"] = i;
                item["BIMPONIBLE"] = bimponible;
                item["IMPORTE_RET"] = imret;

                jsonObjDocs.push(item);
                i++;
                item = "";
            });
        }

        //Variable para saber cuantos tipos de impuestos tiene
        tRet = [];
        tRet2 = [];
        docsenviar = JSON.stringify({ 'items': jsonObjDocs, "bukrs": sociedad_id, "lifnr": proveedor });
        t = $('#table_ret').DataTable();
        t.rows().remove().draw(false);
        var agRowRet = [];//19-12-2018
        $.ajax({
            type: "POST",
            url: 'getRetenciones',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {
                if (data !== null || data !== "") {
                    $.each(data, function (i, dataj) {
                        var bimp = toShow(dataj.BIMPONIBLE);
                        var imp = toShow(dataj.IMPORTE_RET);
                        tRet.push(dataj.WITHT);//Lej 12.09.18
                        agRowRet.push({ t, dataj });
                    }); //Fin de for
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
        for (var i = 0; i < agRowRet.length; i++) {
            $.ajax({
                type: "POST",
                url: 'getRetLigadas',
                data: { 'id': agRowRet[i].dataj.WITHT },
                dataType: "json",
                success: function (data) {
                    if (data !== null || data !== "") {
                        if (data != "Null") {
                            for (var a = 0; a < agRowRet.length; a++) {
                                if (agRowRet[a].dataj.WITHT == data) {
                                    agRowRet.splice($.inArray(agRowRet[a], agRowRet), 1);
                                }
                            }
                        }
                        else {
                        }
                    }
                },
                error: function (xhr, httpStatusMessage, customErrorMessage) {
                    M.toast({ html: httpStatusMessage });
                },
                async: false
            });
        }//19-12-2018
        //Para agregar los renglones de retenciones
        for (var i = 0; i < agRowRet.length; i++) {//19-12-2018
            var bimp = toShow(agRowRet[i].dataj.BIMPONIBLE);
            var imp = toShow(agRowRet[i].dataj.IMPORTE_RET);
            addRowRet(t, agRowRet[i].dataj.BUKRS, agRowRet[i].dataj.LIFNR, agRowRet[i].dataj.WITHT, agRowRet[i].dataj.DESC, agRowRet[i].dataj.WT_WITHCD, bimp, imp);
        }
        //Lej 19.12.18-------------------------------------------------------
        //Aqui se agregaran las columnas extras a la tabla de detalle
        $('#table_infoP').DataTable().destroy();
        $('#table_infoP').empty();
        //LEJGG 11-12-2018-----------------------I
        var arrCols = [
            {
                "name": 'Fila',
                "className": 'POS',
                "orderable": false
            },
            {
                "name": 'A1',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo',
                "orderable": false
            },
            {
                "name": 'A2',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo2',
                "orderable": false
            },
            {
                "name": 'A3',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo3',
                "orderable": false
            },
            {
                "name": 'A4',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo4',
                "orderable": false
            },
            {
                "name": 'A5',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo5',
                "orderable": false
            },
            {
                "name": 'MATERIAL',
                "className": 'MATERIAL',
                "orderable": false
            },
            {
                "name": 'TXTPOS',
                "className": 'TXTPOS',
                "orderable": false
            },
            {
                "name": 'CA',
                "className": 'CA',
                "orderable": false,
                "visible": false
            },
            {
                "name": 'FACTURA',
                "className": 'FACTURA',
                "orderable": false,
                "visible": false
            },
            {
                "name": 'TCONCEPTO',
                "className": 'TCONCEPTO',
                "orderable": false,
                "visible": false//MGC 22-10-2018 Etiquetas
            },
            {
                "name": 'CONCEPTO',
                "className": 'GRUPO',
                "orderable": false
            },
            {
                "name": 'CCOSTO',
                "className": 'CCOSTO',
                "orderable": false
            },
            {
                "name": 'PEP',
                "className": 'PEP',
                "orderable": false
            },
            {
                "name": 'CUENTA',
                "className": 'CUENTA',
                "orderable": false,
                "visible": false//lej 11.09.2018
            },
            {
                "name": 'CUENTANOM',
                "className": 'CUENTANOM',
                "orderable": false,
                "visible": false//lej 11.09.2018
            },
            {
                "name": 'TIPOIMP',
                "className": 'TIPOIMP',
                "orderable": false,
                "visible": false//MGC 22-10-2018 Etiquetas
            },
            {
                "name": 'IMPUTACION',
                "className": 'IMPUTACION',
                "orderable": false,
                "visible": false//lej 11.09.2018
            },
            {
                "name": 'MONTO',
                "className": 'MONTO',
                "orderable": false
            },
            {
                "name": 'MONEDA',
                "className": 'MONEDA',
                "orderable": false

            },
            {
                "name": 'CANTIDAD',
                "className": 'CANTIDAD',
                "orderable": false
            },
            {
                "name": 'UNIDAD',
                "className": 'UNIDAD',
                "orderable": false
            },
            {
                "name": 'IMPUESTOP',
                "className": 'IMPUESTOP',
                "orderable": false,
                "visible": false//lej 11.12.2018
            },
            {
                "name": 'IVA',
                "className": 'IVA',
                "orderable": false
            },
            {
                "name": 'TOTAL',
                "className": 'TOTAL',
                "orderable": false,
                "visible": false//lej 11.12.2018
            }
        ];
        //Se rearmara la tabla en HTML
        var taInf = $("#table_infoP");
        taInf.append($("<thead />"));
        taInf.append($("<tbody />"));
        taInf.append($("<tfoot />"));
        var thead = $("#table_infoP thead");
        thead.append($("<tr />"));
        //Theads        
        $("#table_infoP>thead>tr").append("<th class=\"lbl_pos\">Fila</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_NmAnexo\">A1</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_NmAnexo\">A2</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_NmAnexo\">A3</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_NmAnexo\">A4</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_NmAnexo\">A5</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_mat\">Material</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_Texto\">Texto</th>");//FRT08112018
        $("#table_infoP>thead>tr").append("<th class=\"lbl_cargoAbono\">D/H</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_factura\">Factura</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_tconcepto\">TIPO CONCEPTO</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_grupo\">Grupo</th>");//FRT08112018
        $("#table_infoP>thead>tr").append("<th class=\"lbl_ccosto\">Centro de Costo</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_pep\">PEP</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_cuenta\">Cuenta</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_cuentaNom\">Nombre de cuenta</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_tipoimp\">Tipo Imp.</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_imputacion\">Imputación</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_monto\">Monto</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_moneda\">Moneda</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lblcantidad\">Cantidad</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_Unidad\">Unidad</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_impuesto\">Impuesto  </th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_iva\">IVA</th>");
        $("#table_infoP>thead>tr").append("<th class=\"lbl_total\">Total</th>");
        //LEJGG 19-12-2018-------------------------------------------------------------------->
        tRet2 = tRet;
        for (i = 0; i < tRet.length; i++) {//Revisare las retenciones que tienes ligadas
            $.ajax({
                type: "POST",
                url: 'getRetLigadas',
                data: { 'id': tRet[i] },
                dataType: "json",
                success: function (data) {
                    if (data !== null || data !== "") {
                        if (data != "Null") {
                            tRet2 = jQuery.grep(tRet2, function (value) {
                                return value != data;
                            });
                        }
                        else {
                        }
                    }
                },
                error: function (xhr, httpStatusMessage, customErrorMessage) {
                    M.toast({ html: httpStatusMessage });
                },
                async: false
            });
        }
        for (i = 0; i < tRet2.length; i++) {//Agregare las columnas extras
            $("#table_infoP>thead>tr").append("<th class=\"bi" + tRet2[i] + "\">" + tRet2[i] + "B. I.</th>");
            $("#table_infoP>thead>tr").append("<th class=\"ir" + tRet2[i] + "\">" + tRet2[i] + "I. R.</th>");
        }
        for (i = 0; i < tRet2.length; i++) {
            arrCols.push(
                {
                    "name": tRet2[i] + " B.Imp.",
                    "orderable": false
                },
                {
                    "name": tRet2[i] + " I. Ret.",
                    "orderable": false
                });
        }
        //LEJGG 19-12-2018--------------------------------------------------------------------<
        extraCols = tRet2.length;//lejgg 18-12-2018
        $('#table_infoP').DataTable({
            language: {
                "url": "../Scripts/lang/ES.json"
            },
            "destroy": true,
            "paging": false,
            "ordering": false,
            "info": false,
            "searching": false,
            "columns": arrCols,
            columnDefs: [
                {
                    targets: [0, 6, 7, 11, 19, 21],
                    className: 'mdl-data-table__cell--non-numeric'
                }
            ]
        });
        //LEJGG 11-12-2018-----------------------T
        //darle formato a la tabla
        formatoTcoc();
    } else {
        //Enviar mensaje de error true
    }
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
    //--------
}
$('body').on('focusout', '.extrasPC', function (e) {
    //var y = parseFloat(num);
    var total = 0;
    var _t = $('#table_ret').DataTable();
    var _this = $(this);
    var tr = $(this).closest('tr'); //Obtener el row 
    //sumarizarTodoRow(_this);

    var _v2 = "";
    //Convertir a formato monetario y numerico
    var _nnm = $(this).val().replace("$", "");
    if (_nnm === "") {
        //si esta vacio le agrego un valor de 0.0
        _nnm = parseFloat("0.0");
    } else {
        _nnm = parseFloat(_nnm.replace(',', ''));
    }

    if (_nnm !== "") {
        var cl = _this.attr('class');
        var arrcl = cl.split('p');
        var _res = porcentajeImpRet(tRet2[arrcl[1]]);
        _res = (_nnm * _res) / 100;//Saco el porcentaje
        tr.find("td.ImpRet" + tRet2[arrcl[1]] + " input").val(toShow(_res));
        //--------------------------------------LEJ18102018---------------------->
        //hare la operacion para actualizar el total del renglon
        var _mnt = tr.find("td.MONTO input").val().replace('$', '');
        if (_mnt === "") {
            //si esta vacio le agrego un valor de 0.0
            _mnt = parseFloat("0.0");
        }
        else {
            _mnt = parseFloat(_mnt.replace(',', ''));
        }
        var _iva = tr.find("td.IVA input").val().replace('$', '');
        if (_iva === "") {
            //si esta vacio le agrego un valor de 0.0
            _iva = parseFloat("0.0");
        } else {
            _iva = parseFloat(_iva.replace(',', ''));
        }
        var _ttal = (_mnt + _iva) - sumarColumnasExtras(tr);
        //actualizar el total
        tr.find("td.TOTAL input").val(toShow(_ttal));
        //--------------------------------------LEJ18102018----------------------<
    }
    $(this).val("$" + _nnm.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    updateFooter();
    llenarRetencionesBImpP();
    llenarRetencionesIRetP();
});

$('body').on('focusout', '.extrasPC2', function (e) {
    //var y = parseFloat(num);
    var total = 0;
    var _t = $('#table_ret').DataTable();
    var centi = 999;
    var _this = $(this);

    sumarizarTodoRowP(_this);
    var _v2 = "";
    //Convertir a formato monetario y numerico
    var _nnm = $(this).val().replace("$", "");
    if (_nnm === "") {
        //si esta vacio le agrego un valor de 0.0
        _nnm = parseFloat("0.0");
    } else {
        _nnm = parseFloat(_nnm.replace(',', ''));
    }
    $(this).val("$" + _nnm.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#table_infoP > tbody > tr[role = 'row']").each(function (index) {
        for (x = 0; x < tRet2.length; x++) {
            var _var = "ImpRet" + x;
            _v2 = "ImpRetF" + (x + 1);
            if (_this.hasClass(_var)) {
                centi = x;
                break;
            }
        }
        var colex = $(this).find("td." + _v2 + " input").val().replace("$", "").replace(',', '');
        //de esta manera saco el renglon y la celad en especifico
        var er = $('#table_ret tbody tr').eq(x).find('td').eq(3).text().replace('$', '');
        var txbi = $.trim(colex);
        var sum = parseFloat(txbi);
        // sum = parseFloat(sum + y).toFixed(2);
        total += sum;

    });
    if (centi !== 9999) {
        $('#table_ret tbody tr').eq(centi).find('td').eq(4).text('$' + total.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $('#table_ret tbody tr').eq(centi + 2).find('td').eq(4).text('$' + total.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    }
    llenarRetencionesBImpP();
    llenarRetencionesIRetP();
});

//LEJGG 12-12-2018----------------------------------I
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
        url: 'getPorMult',
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
        monto = 0;
    }
    monto = parseFloat(monto);
    //Agrego el nuevo total escondido
    var op = (monto * porc) / 100;
    var _tot = monto + op;
    t.cell(indexopc, 24).data("").draw();//Limpiar las celdas
    t.cell(indexopc, 24).data(_tot).draw();//clavar el nuevo valor del total

    var _mt = toShow(monto);
    tr.find("td.MONTO input").val();
    tr.find("td.MONTO input").val(_mt);
    updateFooterP();
});
//LEJGG 12-12-2018----------------------------------T
function sumarizarTodoRowP(_this) {
    //Inicio codio sumarizar
    //Ejecutamos el metodo para sumarizar las columnas
    //var t = $('#table_info').DataTable();
    var tr = _this.closest('tr'); //Obtener el row 
    //Obtener el valor del impuesto
    var imp = tr.find("td.IMPUESTOP select").val();
    //Calcular impuesto y subtotal
    var impimp = impuestoVal(imp);
    impimp = parseFloat(impimp);
    var colTotal = sumarColumnasExtras(tr);

    //Desde el subtotal
    var sub = tr.find("td.MONTO_F input").val().replace('$', '').replace(',', '');
    sub = parseFloat(sub);

    //rimpimp = 100 - impimp;

    var impv = (sub * impimp) / 100;
    impv = parseFloat(impv);

    var total = sub + impv;
    total = parseFloat(total);
    sub = total - impv;

    impv = toShow(impv);
    sub = toShow(sub);
    total = toShow(total);

    //Enviar los valores a la tabla
    //Subtotal
    tr.find("td.MONTO_F input").val();
    tr.find("td.MONTO_F input").val(sub);

    //IVA
    tr.find("td.IVA input").val();
    tr.find("td.IVA input").val(impv);

    //Total
    tr.find("td.TOTAL input").val();
    if (colTotal > 0) {
        var sumt = parseFloat(total.replace('$', '').replace(',', '')) - parseFloat(colTotal);
        tr.find("td.TOTAL input").val(toShow(sumt));
    }
    else {
        tr.find("td.TOTAL input").val(total);
    }
    //Fin de codigo que sumariza
    updateFooterP();
}

//lejgg 23/10/18
function llenarRetencionesIRetP() {
    var _t = [];
    var centi = 9999;
    for (x = 0; x < tRet2.length; x++) {
        _t.push("0");
    }
    $("#table_infoP > tbody > tr[role = 'row']").each(function (index) {
        for (x = 0; x < tRet2.length; x++) {
            var _var = "ImpRet" + x;
            _v2 = "ImpRet" + tRet2[x];
            if ($(this).find("td." + _v2 + " input").hasClass(_var)) {
                centi = x;
                var colex = $(this).find("td." + _v2 + " input").val().replace("$", "").replace(',', '');
                while (colex.indexOf(',') > -1) {
                    colex = colex.replace('$', '').replace(',', '');
                }
                var txbi = $.trim(colex);
                var sum = parseFloat(txbi);
                _t[x] = parseFloat(_t[x]) + sum;
                //break;
            }
        }
        /* var colex = $(this).find("td." + _v2 + " input").val().replace("$", "").replace(',', '');
         //de esta manera saco el renglon y la celad en especifico
         //var er = $('#table_ret tbody tr').eq(x).find('td').eq(3).text().replace('$', '');
         var txbi = $.trim(colex);
         var sum = parseFloat(txbi);
         // sum = parseFloat(sum + y).toFixed(2);
         _t += sum;*/
    });
    for (x = 0; x < tRet2.length; x++) {
        $('#table_ret tbody tr').eq(x).find('td').eq(4).text('$' + parseFloat(_t[x]).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    }
    //$('#table_ret tbody tr').eq(0).find('td').eq(4).text('$' + _t[].toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    // $('#table_ret tbody tr').eq(1).find('td').eq(4).text('$' + _t.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

}

function llenarRetencionesBImpP() {
    var _t = [];
    var centi = 0;
    for (x = 0; x < tRet2.length; x++) {
        _t.push("0");
    }
    $("#table_infoP > tbody > tr[role = 'row']").each(function (index) {
        for (x = 0; x < tRet2.length; x++) {
            var _var = "BaseImp" + x;
            _v2 = "BaseImp" + tRet2[x];
            if ($(this).find("td." + _v2 + " input").hasClass(_var)) {
                var colex = $(this).find("td." + _v2 + " input").val().replace("$", "").replace(',', '');
                var txbi = $.trim(colex);
                var sum = parseFloat(txbi);
                _t[x] = parseFloat(_t[x]) + sum;
            }
        }
    });
    for (x = 0; x < tRet2.length; x++) {
        $('#table_ret tbody tr').eq(x).find('td').eq(3).text('$' + parseFloat(_t[x]).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    }

}
