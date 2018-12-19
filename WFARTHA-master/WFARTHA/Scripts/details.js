//Variables globales
var firmaVal = "";

$(document).ready(function () {

    //Inicializar las tabs
    $('#tabs').tabs();

    $('#Contable_cont').change(function () {
        tamanosRenglones();
    });

    //Lejgg 21-11-2018---------------------->
    //Nombre del autorizador ya establecido
    //MGC 22-11-2018.2 Cadena de autorización----------------------------------------------------------------------->
    //var nDoc = $('#refHd').val();
    //datosCadena(nDoc);
    //Lejgg 21-11-2018----------------------<
    //MGC 22-11-2018.2 Cadena de autorización-----------------------------------------------------------------------<
    //Iniciar todos los selects
    var elem = document.querySelectorAll('select');
    var instance = M.Select.init(elem, []);
    formatoTabla();
    $('#table_sop').DataTable({

        language: {
            //"url": "../Scripts/lang/@Session["spras"].ToString()" + ".json"
            "url": "../../Scripts/lang/ES.json"
        },
        "paging": false,
        "info": false,
        "searching": false,
        "columns": [
            {
                "className": 'select_row',
                "data": null,
                "defaultContent": '',
                "orderable": false
            },
            {
                "name": 'OPC',
                "className": 'OPC',
                "orderable": false,
            },
            {
                "name": 'POS',
                "className": 'POS',
                "orderable": false,
            },
            {
                "name": 'RFC',
                "className": 'RFC',
                "orderable": false
            },
            {
                "name": 'FACTURA',
                "className": 'FACTURA',
                "orderable": false
            },
            {
                "name": 'FECHA',
                "className": 'FECHA',
                "orderable": false
            },

            {
                "name": 'MONTO',
                "className": 'MONTO',
                "orderable": false
            }
            ,
            {
                "name": 'IVA',
                "className": 'IVA',
                "orderable": false
            },
            {
                "name": 'TOTAL',
                "className": 'TOTAL',
                "orderable": false
            }
            ,
            {
                "name": 'ARCHIVO',
                "className": 'ARCHIVO',
                "orderable": false
            }
        ]
    });

    //Contabilizar
    $('#btn_cont').on("click", function () {
        document.getElementById("loader").style.display = "initial";//RSG 26.04.2018
        var num = $('#num_doc_send').val();

        $.ajax({
            type: "POST",
            url: '../../Flujos/Procesa',
            //dataType: "json",
            data: { "id": num },

            success: function (data) {

                if (data != null || data != "") {
                    M.toast({ html: data });
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {

            },
            async: false
        });

        document.getElementById("loader").style.display = "none";//RSG 26.04.2018

    });

    //Tabla de Retenciones
    $('#table_ret').DataTable({
        language: {
            "url": "../../Scripts/lang/ES.json"
        },
        "paging": false,
        "info": false,
        "searching": false,
        "columns": [
            {
                "name": 'SOCRET',
                "className": 'SOCRET',
                "orderable": false,
                "visible": false
            },
            {
                "name": 'PROVRET',
                "className": 'PROVRET',
                "orderable": false,
                "visible": false
            },
            {
                "name": 'TRET',
                "className": 'TRET',
                "orderable": false
            },
            {
                "name": 'DESCRET',
                "className": 'DESCTRET',
                "orderable": false
            },
            {
                "name": 'INDRET',
                "className": 'INDRET',
                "orderable": false
            },
            {
                "name": 'BIMPONIBLE',
                "className": 'BIMPONIBLE',
                "orderable": false
            },
            {
                "name": 'IMPRET',
                "className": 'IMPRET',
                "orderable": false
            }
        ],
        columnDefs: [
            {
                targets: [0, 1, 2, 3, 4, 5, 6],
                className: 'mdl-data-table__cell--non-numeric'
            }
        ]
    });

    $('#div-menu').on('click', function () {
        $(window).resize();
    });

    $('#cerrar-menu').on('click', function () {
        $(window).resize();
    });

    //se ocupa un ciclo for con minimo 2 veces que se ejecute el metodo para que lo haga,ó ue e le de 2 clicks a tab_con para que lo haga.
    //Lejgg 27-11-2018
    $('#tab_con').on('click', function () {
        for (var i = 0; i < 2; i++) {
            $(window).resize();
        }
    });
    formatoMon();

    var val3 = $('#tsol').val();
    showHide(val3);

});

$(window).on('load', function () {

    $('.materialize-textarea').css("height", "0px");
    //Lejgg28/11/2018
    alinearIzq();
    //---
    var _fd = $('#D_FECHAD').val().split(' ');
    $('#D_FECHAD').val(_fd[0]);
    //---
});
function alinearIzq() {
    $("#table_ret > tbody  > tr[role='row']").each(function () {
        //1
        var R1 = $(this).find("td.TRET");
        R1.css("text-align", "left");
        //2
        var R2 = $(this).find("td.DESCTRET");
        R2.css("text-align", "left");
        //3
        var R3 = $(this).find("td.INDRET");
        R3.css("text-align", "left");
        //4
        var R4 = $(this).find("td.BIMPONIBLE");
        R4.css("text-align", "left");
        //5
        var R5 = $(this).find("td.IMPRET");
        R5.css("text-align", "left");
    });
}

function formatoTabla() {
    var colsArray = [
        {//MGC 30-10-2018 Tipo de presupuesto
            "className": 'select_row',
            "data": null,
            "defaultContent": '',
            "orderable": false,
            "visible": false //MGC 30-10-2018 Tipo de presupuesto
        },//MGC 30-10-2018 Tipo de presupuesto
        {
            "name": 'Fila',
            "className": 'POS',
            "orderable": false
        },
        {
            "name": 'A1',
            "className": 'NumAnexo',
            "orderable": false
        },
        {
            "name": 'A2',
            "className": 'NumAnexo2',
            "orderable": false
        },
        {
            "name": 'A3',
            "className": 'NumAnexo3',
            "orderable": false
        },
        {
            "name": 'A4',
            "className": 'NumAnexo4',
            "orderable": false
        },
        {
            "name": 'A5',
            "className": 'NumAnexo5',
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
            "orderable": false
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
            "name": 'CCOSTO',
            "className": 'CCOSTO',
            "orderable": false

        },
        {
            "name": 'MONTO',
            "className": 'MONTO',
            "orderable": false
        },
        {
            "name": 'IVA',
            "className": 'IVA',
            "orderable": false
        }
    ];
    //Variable para saber cuantos tipos de impuestos tiene
    tRet = [];
    tRet2 = [];
    var docsenviar = {};
    var jsonObjDocs = [];
    var sociedad_id = $("#SOCIEDAD_ID").val();
    var proveedor = $("#pid").val();//lej 05.09.2018
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
        var bimponible = toNum(bimponible);
        var imret = toNum(imret);

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

    docsenviar = JSON.stringify({ 'items': jsonObjDocs, "bukrs": sociedad_id, "lifnr": proveedor });
    $.ajax({
        type: "POST",
        url: '../getRetenciones',
        contentType: "application/json; charset=UTF-8",
        data: docsenviar,
        success: function (data) {
            if (data !== null || data !== "") {
                $.each(data, function (i, dataj) {
                    tRet.push(dataj.WITHT);//Lej 15.11.18
                }); //Fin de for
            }

        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });

    tRet2 = tRet;
    for (i = 0; i < tRet.length; i++) {//Revisare las retenciones que tienes ligadas
        $.ajax({
            type: "POST",
            url: '../getRetLigadas',
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
    //Se hara un push al arreglo de columnas original
    for (i = 0; i < tRet2.length; i++) {
        colsArray.push({
            "name": tRet2[i] + " B.Imp.",
            "orderable": false
        }, {
                "name": tRet2[i] + " I. Ret.",
                "orderable": false
            });
    }
    //Para agregar el texto
    colsArray.push({
        "name": 'TOTAL',
        "className": 'TOTAL',
        "orderable": false
    });
    //Tabla de Información
    $('#table_info').DataTable({
        language: {
            //"url": "../Scripts/lang/@Session["spras"].ToString()" + ".json"
            "url": "../../Scripts/lang/ES.json"
        },
        scrollX: "50vh",
        scrollY: "50vh",
        scrollCollapse: true,
        columnDefs: [
            {
                targets: [0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16],
                className: 'mdl-data-table__cell--non-numeric'
            }
        ],
        "paging": false,
        "info": false,
        "searching": false,
        "columns": colsArray
    });
    tamanosRenglones();
}

function showHide(tsol) {
    var val3 = tsol;
    val3 = "[" + val3 + "]";
    val3 = val3.replace("{", "{ \"");
    val3 = val3.replace("}", "\" }");
    val3 = val3.replace(/\,/g, "\" , \"");
    val3 = val3.replace(/\=/g, "\" : \"");
    val3 = val3.replace(/\ /g, "");


    var jsval = "";
    try {
        jsval = $.parseJSON(val3);
    } catch (err) {
        jsval = "";
    }

    try {

        $.each(jsval, function (i, dataj) {
            ocultarCampos(dataj.EDITDET, param1);
        });
    } catch (err) {
        var jj = "";
    }

}

function formatoMon() {
    var table = $('#table_info').DataTable();
    // $("#table_info > tbody > tr[role = 'row']").each(function (index) {
    //var col11 = $(this).find("td.TOTAL input").val();
    //var col11 = $(this).find("td.TOTAL input").val();


    //col11 = col11.replace(/\s/g, '');
    //var val = toNum(col11);
    //val = convertI(val);
    //if ($.isNumeric(val)) {
    //    total += val;
    //}
    //  });
}

function updateFooter() {
    resetFooter();

    var t = $('#table_info').DataTable();
    var total = 0;

    $("#table_info > tbody > tr[role = 'row']").each(function (index) {
        //var col11 = $(this).find("td.TOTAL input").val();
        var col11 = $(this).find("td.TOTAL input").val();


        col11 = col11.replace(/\s/g, '');
        var val = toNum(col11);
        val = convertI(val);
        if ($.isNumeric(val)) {
            total += val;
        }
    });

    total = total.toFixed(2);

    $('#total_info').text(toShow(total));


}

function convertI(i) {
    return typeof i === 'string' ?
        i.replace(/[\$,]/g, '') * 1 :
        typeof i === 'number' ?
            i : 0;
};

function resetFooter() {
    $('#total_info').text("$0");
}

function resetTabs() {

    var ell = document.getElementById("tabs");
    var instances = M.Tabs.getInstance(ell);

    var active = $('.tabs').find('.active').attr('href');
    active = active.replace("#", "");
    instances.select(active);
    //instances.updateTabIndicator
}

function ocultarCampos(opc, load) {
    //respuesta en minúscula
    opc = opc.toLowerCase();

    //Si load = "load" solo se ocultan o muestran campos

    if (opc === "true") {

        //Solicitud sin orden de compra
        $("#div_norden_compra").css("display", "none");

        if (load === "load") {
            //
        } else {
            $("#norden_compra").val("");
        }


    } else {
        //Solicitud con orden de compra
        $("#div_norden_compra").css("display", "inherit");

        if (load === "load") {
            //
        } else {
            $("#norden_compra").val("");
        }
    }

    //Deshabilitar campos de la tabla
    //ocultarColumnas(opc);
}


//MGC 18-10-2018 Firma del usuario -------------------------------------------------->
//Validar la firma del usuario

function valF(frmValues) {

    firmaVal = "";
    firmaVallocal = "";
    $.ajax({
        type: "POST",
        url: '../ValF',
        //dataType: "json",
        data: { "pws": frmValues },

        success: function (data) {

            asigF(data);

        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            var a = xhr;
        },
        async: false
    });


    firmaVallocal = firmaVal;
    return firmaVallocal;
}

function asigF(fir) {
    firmaVal = fir;
}


//MGC 18-10-2018 Firma del usuario --------------------------------------------------<

//LEJGG 13/11/2018
function tamanosRenglones() {
    //TEXTO
    var t_ret = $("#table_info>thead>tr").find('th.TXTPOS');
    t_ret.css("text-align", "center");
    //Monto
    var t_mt = $("#table_info>thead>tr").find('th.CCOSTO');
    t_mt.css("text-align", "center");
    //total
    var t_fac = $("#table_info>thead>tr").find('th.FACTURA');
    t_fac.css("text-align", "left");
    //grupo
    var tg = $("#table_info>thead>tr").find('th.GRUPO');
    tg.css("text-align", "center");
    //IVA
    var t_iva = $("#table_info>thead>tr").find('th.IVA');
    t_iva.css("text-align", "center");
    //SELECT IMPUESTO
    var t_imps = $("#table_info>thead>tr").find('th.IMPUESTO_SELECT');
    t_imps.css("text-align", "left");
    //reviso si tiene retenciones para agregarles nueva medidas
    if (tRet2.length > 0) {
        for (var i = 0; i < tRet2.length; i++) {
            var _cex = $("#table_info>thead>tr").find("th.bi" + tRet2[i]);
            _cex.css("text-align", "left");
            var _cex2 = $("#table_info>thead>tr").find("th.ir" + tRet2[i]);
            _cex2.css("text-align", "left");
        }
    }
    //total
    var t_tot = $("#table_info>thead>tr").find('th.TOTAL');
    t_tot.css("text-align", "center");
    //FILA
    var tpos = $("#table_info>tbody>tr").find('td.POS');
    tpos.css("text-align", "left");
    tpos.css("font-size", "15px");
    if (tRet2.length == 0) {
        //TEXTO
        $('.materialize-textarea').css("width", "100%");
        //FILA
        var an = $("#table_info>tbody>tr").find('td.NumAnexo');
        an.css("text-align", "left");
        //FILA
        var an2 = $("#table_info>tbody>tr").find('td.NumAnexo2');
        an2.css("text-align", "left");
        //FILA
        var an3 = $("#table_info>tbody>tr").find('td.NumAnexo3');
        an3.css("text-align", "left");
        //FILA
        var an4 = $("#table_info>tbody>tr").find('td.NumAnexo4');
        an4.css("text-align", "left");
        //FILA
        var an5 = $("#table_info>tbody>tr").find('td.NumAnexo5');
        an5.css("text-align", "left");
        //total
        var TTH = $("#table_info>thead>tr").find('th.TOTAL');
        TTH.css("text-align", "left");
        var TT = $("#table_info>tbody>tr").find('td.TOTAL');
        TT.css("text-align", "left");
        //grupo
        var tg1 = $("#table_info>thead>tr").find('th.GRUPO');
        tg1.css("text-align", "left");
        //Monto
        var t_mt1 = $("#table_info>thead>tr").find('th.CCOSTO');
        t_mt1.css("text-align", "left");
        //IVA
        var t_iva1 = $("#table_info>thead>tr").find('th.IVA');
        t_iva1.css("text-align", "left");
        //IMPUESTO
        var t_im = $("#table_info>thead>tr").find('th.MONTO');
        t_im.css("text-align", "left");
    }
}
//LEJGG 21-11-2018 Cadena de autorización----------------------------------------------------------------------------->
//Al seleccionar un solicitante, obtener la cadena para mostrar

function obtenerCadena(version, usuarioc, id_ruta, usuarioa, monto, sociedad) {

    try {
        monto = parseFloat(monto) || 0.0;
    } catch (err) {
        monto = 0.0;
    }

    //Eliminar Registros
    $("#tableAutorizadores > tbody > tr").remove();

    $.ajax({
        type: "POST",
        url: '../getCadena',
        data: { 'version': version, 'usuarioc': usuarioc, 'id_ruta': id_ruta, 'usuarioa': usuarioa, 'monto': monto, 'bukrs': sociedad },
        dataType: "json",
        success: function (data) {
            if (data !== null || data !== "") {

                $.each(data, function (i, dataj) {
                    var fase = dataj.fase;
                    var autorizador = dataj.autorizador;

                    //Agregar los valores de las cadenas a las tablas
                    $('#tableAutorizadores').append('<tr><td>' + fase + '</td><td>' + autorizador + '</td></tr>');

                }); //Fin de for


            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });

}

//eliminar registros de tabla

//LEJGG 21-11-2018 Cadena de autorización-----------------------------------------------------------------------------<

function datosCadena(nDoc) {
    $.ajax({
        type: "POST",
        url: '../getCadAut',
        data: { 'nd': nDoc },
        dataType: "json",
        success: function (data) {
            if (data !== null || data !== "") {
                obtenerCadena(data[5], data[4], data[2], data[3], data[1], data[0]);
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
}