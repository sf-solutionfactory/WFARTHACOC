//Variables globales
var posinfo = 0;
var posrows = 0;  //FRT08112018
var inicio = 0;   //FRT08112018  
var statSend = false;//FRT02122018 PARA PODER MANDAR ALERTA DE QUE EL FORMULARIO SE ESTA ENVIANDO


var tRet = [];//Agrego a un array los tipos de retenciones
var tRet2 = [];
$(document).ready(function () {
    var elem = document.querySelectorAll('select');
    var instance = M.Select.init(elem, []);

    //---
    var _fd = $('#FECHAD').val().split(' ');
    $('#FECHAD').val(_fd[0]);
    //---
    $("#list_detaa").trigger("change");
    //Formato a campo tipo_cambio
    var _Tc = $('#TIPO_CAMBIO').val();
    $('#TIPO_CAMBIO').val(toShow(_Tc));
    //Inicializar las tabs
    $('#tabs').tabs();

    $('#div-menu').on('click', function () {
        tamanosRenglones();
    });

    $('#cerrar-menu').on('click', function () {
        tamanosRenglones();
    });

    $('#tab_enc').on('click', function () {
        tamanosRenglones();
    });

    $('#tab_con').on("click", function (e) {
        var proveedor = $("#PAYER_ID").val();
        if (proveedor == "" | proveedor == null) {
            M.toast({ html: "Falta ingresar proveedor" });
            return false;
        } else {
            //$(this).tab('show')

        }
    });

    $('#tab_sop').on("click", function (e) {
        var proveedor = $("#PAYER_ID").val();
        if (proveedor == "" | proveedor == null) {
            M.toast({ html: "Falta ingresar proveedor" });
            return false;
        } else {
            //$(this).tab('show')

        }
    });

    $('#tab_sap').on("click", function (e) {
        var proveedor = $("#PAYER_ID").val();
        if (proveedor == "" | proveedor == null) {
            M.toast({ html: "Falta ingresar proveedor" });
            return false;
        } else {
            //$(this).tab('show')

        }
    });

    $('.tabDet').on('click', function () {
        tamanosRenglones();
    });

    ////FRT02122018 Para no dejar avanzar si no existe proveedor
    //$('.tabs a').click(function () {


    //    var proveedor = $("#PAYER_ID").val();
    //    if (proveedor == "" | proveedor == null) {
    //        alert("Falta ingresar proveedor");
    //        return false;
    //    } else {
    //        $(this).tab('show')

    //    }

    //})
    //    //ENDFRT02122018 Para no dejar avanzar si no existe proveedor

    //FRT21112018.3 Se añade para obtener el Tipo de Cambio
    $("#MONEDA_ID").change(function () {
        var moneda = $('#MONEDA_ID Option:Selected').val();
        var fecha = $('#FECHAD').val();
        //var fecha = "12/08/2018";
        if (moneda.substring(0, 3) == "USD") {
            tipocambio = getTipoCambio(moneda, fecha);
            $("#TIPO_CAMBIO").val(tipocambio);
        } else {
            $('#TIPO_CAMBIO').val(0);
        }

    });

    //Tabla de Anexos
    $('#table_anexa').DataTable({
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
                "name": 'Fila',
                "className": 'POS',
                "orderable": false
            },
            {
                "name": 'STAT',
                "className": 'STAT',
                "orderable": false
            },
            {
                "name": 'NAME',
                "className": 'NAME',
                "orderable": false
            },
            {
                "name": 'TYPE',
                "className": 'TYPE',
                "orderable": false
            },
            {
                "name": 'DESC',
                "className": 'DESC',
                "orderable": false
            },
            {
                "name": '',
                "className": '',
                "orderable": false
            }
        ]
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

    //Establecer fechas
    $("#FECHADO").val($("#FECHAD").val());//lejgg 22/11/2018

    solicitarDatos();
    insertarCondicion();


    $('#btn_cancelar').on("click", function (e) {
        if (statSend) {
            alert("Favor de esperar. Se esta generando Solicitud...");
        }

    });

    $('#btn_guardarh').on("click", function (e) {
        if (!statSend) {
            statSend = true;
            //LEJGG 06-11-18
            //Aqui verificare si es invoice o factura
            var val3 = $('#tsol').val();
            val3 = "[" + val3 + "]";
            val3 = val3.replace("{", "{ \"");
            val3 = val3.replace("}", "\" }");
            val3 = val3.replace(/\,/g, "\" , \"");
            val3 = val3.replace(/\=/g, "\" : \"");
            val3 = val3.replace(/\ /g, "");
            var jsval = $.parseJSON(val3);
            if (jsval[0].ID === "SSO") {
                var res = validarFacs();//Lejgg 23-10-2018
                if (res) {//si es true signfica que si hay factura
                    //Fechade la factura
                    var _fdo = $("#FECHADO").val();
                } else {
                    //si es false signfica que es invoice(fecha de la creacion)
                    var fdo = $("#FECHADO").val();
                }

            }
            //CODIGO
            //dar formato al monto
            var enca_monto = $("#MONTO_DOC_MD").val();
            enca_monto = enca_monto.replace(/\s/g, '');
            //enca_monto = toNum(enca_monto);
            //enca_monto = parseFloat(enca_monto);
            $("#MONTO_DOC_MD").val(enca_monto);
            var _b = false;
            var _m = true;
            var _g = true;
            var _f = true;
            

            var _anull = true //FRT06122018
            var _asnull = true //FRT06122018
            _aduplicados = true; //FRT07122018 
            var _vs = [];
            var msgerror = "";
            var _rni = 0;
            //Validar que los anexos existan
            $("#table_anexa > tbody  > tr[role='row']").each(function () {
                var pos = $(this).find("td.POS").text().trim();
                _vs.push(pos);
            });

            //LEJ 11.09.2018
            //dar formato al T CAMBIO
            var tcambio = $("#TIPO_CAMBIO").val();
            tcambio = tcambio.replace(/\s/g, '');
            tcambio = toNum(tcambio);
            tcambio = parseFloat(tcambio);
            $("#TIPO_CAMBIO").val(tcambio);
            var t = $('#table_info').DataTable();
            var tabble = "table_info";
            var borrador = $("#borr").val();
            if ($("table#table_info tbody tr[role='row']").length === 0) { tabble = "table_infoP"; }
            $("#" + tabble + " > tbody  > tr[role='row']").each(function () {

                var _anexos = []; //FRT07122018
                _rni++;
                //Obtener valores visibles en la tabla
                var na1 = $(this).find("td.NumAnexo input").val();
                _anexos.push(na1);
                var na2 = $(this).find("td.NumAnexo2 input").val();
                _anexos.push(na2);
                var na3 = $(this).find("td.NumAnexo3 input").val();
                _anexos.push(na3);
                var na4 = $(this).find("td.NumAnexo4 input").val();
                _anexos.push(na4);
                var na5 = $(this).find("td.NumAnexo5 input").val();
                _anexos.push(na5);

                var _as = true; //FRT05122018 Para anexos asociados en detalles
                //frt05112018 validacion de CECOS vacion en Tipo Imp. "K"
                var ceco = $(this).find("td.CCOSTO input").val();
                var tr = $(this);
                var indexopc = t.row(tr).index();

                //FRT20112018 iNGRESAR VALIDACION DE CONCEPTO
                //var concepto = t.row(indexopc).data()[13]; 
                var concepto = $(this).find("td.GRUPO input").val(); //FRT21112018

                if (concepto == "" | concepto == "") {
                    msgerror = "Fila " + _rni + ": Falta ingresar Concepto";
                    statSend = false;
                    _g = false;
                    _b = false;
                } else {
                    _b = true;
                }
                if (_b === false) {
                    return false;
                }
                //ENDFRT20112018 iNGRESAR VALIDACION DE CONCEPTO


                //FRT21112018.3 Se realizara validación del monto > 0

                var monto = $(this).find("td.MONTO input").val().replace('$', '').replace(',', '');
                while (monto.indexOf(',') > -1) {
                    monto = monto.replace('$', '').replace(',', '');
                }

                if (borrador != "B") {
                    if (monto == " 0.00" | monto == null | monto == "") { //MGC 07-11-2018 Validación en el monto
                        statSend = false;
                        msgerror = "Fila " + _rni + ": El monto debe ser mayor a cero";
                        _b = false;
                    } else {
                        _b = true;
                    }
                    if (_b === false) {
                        return false;
                    }
                }


                //END FRT06112018.3

                //FRT02122018 PARA VALIDAR QUE LA FACTURA NO ESTE VACIA

                if (borrador != "B") {
                    var factura = $(this).find("td.FACTURA input").val().trim();


                    if (factura == "" | factura == null) {
                        statSend = false;
                        msgerror = "Fila " + _rni + ": Falta ingresar numero de factura";
                        _b = false;
                    } else {
                        _b = true;
                    }
                    if (_b === false) {
                        return false;
                    }
                }

                //ENDFRT02122018 PAA VALIDAR QUE LA FACTURA NO ESTE VACIA


                //LEJGG 24112018 Para validar el Monto contra las F
                if (tRet2[0] != null) {
                    monto = parseFloat(monto);
                    var lengthT1 = $("table#table_ret tbody tr[role='row']").length;
                    $("#table_info tbody tr[role='row']").each(function () {
                        var _t = $(this);
                        var findexopc = t.row(_t).index();
                        findexopc++;
                        if (findexopc == _rni) {
                            for (var x = 0; x < tRet2.length; x++) {
                                var _montobase = _t.find("td.BaseImp" + tRet2[x] + " input").val().replace('$', '').replace(',', '');
                                while (_montobase.indexOf(',') > -1) {
                                    _montobase = _montobase.replace('$', '').replace(',', '');
                                }
                                var montobase = parseFloat(_montobase);

                                if (monto < montobase) {
                                    statSend = false;
                                    msgerror = "Fila " + _rni + ": Monto base de retencion (" + montobase + ") no debe ser mayor al monto antes de IVA (" + monto + ")";
                                    _m = false;
                                    break
                                } else {
                                    _m = true;
                                }
                                if (_m === false) {
                                    return false;
                                }
                            }
                        }
                    });
                } else {
                    _m = true;
                }


                if (_m === false) {
                    return false;
                }
                //LEJGG 24112018


                ////FRT2311208 PARA VALIDACION DE 50 CARACTERES  FRT06122018 se queita validacion en detales

                if (borrador != "B") {
                    var texto = $(this).find("td.TEXTO textarea").val().trim();
                    var ct = texto.length;
                    ct = parseFloat(ct)
                    if (ct == 0) {
                        _b = false;
                        statSend = false;
                        msgerror = "Fila " + _rni + ": Falta explicación en la columna de Texto";
                    } else {
                        _b = true;
                    }
                    if (_b === false) {
                        return false;
                    }
                }

                ////END FRT2311208 PARA VALIDACION DE 50 CARACTERES

                if (borrador != "B") {
                    var tipoimp = t.row(indexopc).data()[14];

                    if (tipoimp == "K" & (ceco == "" | ceco == null)) {
                        msgerror = "Fila " + _rni + ": Falta ingresar Centro de Costo";
                        statSend = false;
                        _b = false;
                    } else {
                        _b = true;
                    }
                    if (_b === false) {
                        return false;
                    }
                }


                //FRT05122018 Para validar que si tiene anexos debemos tener al menos uno asociado por detalle
                if (borrador != "B") {
                    if (_vs.length > 0) {
                        if (na1 === "") {
                            _as = false;
                            statSend = false;
                            _b = false;
                            msgerror = "Fila " + _rni + " :  Falta asociar numero de anexo en la columna A1";
                            return false;
                        }
                    }
                    if (_b === false) {
                        return false;
                    }

                }
                //ENDFRT05122018 Para validar que si tiene anexos debemos tener al menos uno asociado por detalle


                //frt07122018 para validar que no metan dos veces el anexo asociado en la misma fila

                for (var k = 0; k < 5; k++) {
                    var duplicado = false;
                    for (var z = 0; z < k; z++) {
                        if (_anexos[k] != "") {
                            if (_anexos[z] == _anexos[k]) {
                                duplicado = true;
                                break;
                            }
                        }

                    }
                    if (duplicado) {
                        _b = false;
                        _aduplicados = false;
                        statSend = false;
                        msgerror = "Fila " + _rni + " : No es posible duplicar anexo asociado " + _anexos[z];
                        break;
                    }
                }
                if (_b === false) {
                    return false;
                }

                //endfrt07122018

                //FRT06122018 Para validar que si no hay anexos no se pueda asociar
                if (_vs.length == 0) {
                    if (na1 != "" || na2 != "" || na3 != "" || na4 != "" || na5 != "") {
                        _b = false;
                        _anull = false;
                        msgerror = "Fila " + _rni + " : No existe numero de anexo " + na1 + " por asociar ";

                    }
                }

                if (_anull === false) {
                    return false;
                }
                //ENDFRT06122018 Para validar que si no hay anexos no se pueda asociar


                if (_vs.length > 0) {
                    for (var i = 0; i < _vs.length; i++) {
                        if (na1 === _vs[i] || na1 === "") {
                            _b = true;
                            _asnull = true;
                            break;
                        } else {
                            _b = false;
                            _asnull = false;
                            //msgerror = "Error en el renglon " + _rni + " valor: " + na1 + " Columna 2";
                            statSend = false;
                            msgerror = "Fila " + _rni + " : El valor ingresado en la columna A1 no es un Anexo";
                        }
                    }
                    if (_b === false) {
                        return false;
                    }
                    if (_asnull === false) {
                        return false;
                    }
                    for (var i2 = 0; i2 < _vs.length; i2++) {
                        if (na2 === _vs[i2] || na2 === "") {
                            _b = true;
                            _asnull = true;
                            break;
                        } else {
                            _b = false;
                            _asnull = false;
                            //msgerror = "Error en el renglon " + _rni + " valor: " + na2 + " Columna 3";
                            statSend = false;
                            msgerror = "Fila " + _rni + " : El valor ingresado en la columna A2 no es un Anexo";
                        }
                    }
                    if (_b === false) {
                        return false;
                    }
                    if (_asnull === false) {
                        return false;
                    }
                    for (var i3 = 0; i3 < _vs.length; i3++) {
                        if (na3 === _vs[i3] || na3 === "") {
                            _b = true;
                            _asnull = true;
                            break;
                        } else {
                            _b = false;
                            _asnull = false;
                            //msgerror = "Error en el renglon " + _rni + " valor: " + na3 + " Columna 4";
                            statSend = false;
                            msgerror = "Fila " + _rni + " : El valor ingresado en la columna A3 no es un Anexo";
                        }
                    }
                    if (_b === false) {
                        return false;
                    }
                    if (_asnull === false) {
                        return false;
                    }
                    for (var i4 = 0; i4 < _vs.length; i4++) {
                        if (na4 === _vs[i4] || na4 === "") {
                            _b = true;
                            _asnull = true;
                            break;
                        } else {
                            _b = false;
                            _asnull = false;
                            //msgerror = "Error en el renglon " + _rni + " valor: " + na4 + " Columna 5";
                            statSend = false;
                            msgerror = "Fila " + _rni + " : El valor ingresado en la columna A4 no es un Anexo";
                        }
                    }
                    if (_b === false) {
                        return false;
                    }
                    if (_asnull === false) {
                        return false;
                    }
                    for (var i5 = 0; i5 < _vs.length; i5++) {
                        if (na5 === _vs[i5] || na5 === "") {
                            _b = true;
                            _asnull = true;
                            break;
                        } else {
                            _b = false;
                            _asnull = false;
                            //msgerror = "Error en el renglon " + _rni + " valor: " + na5 + " Columna 6";
                            statSend = false;
                            msgerror = "Fila " + _rni + " : El valor ingresado en la columna A5 no es un Anexo";
                        }
                    }
                    if (_b === false) {
                        return false;
                    }
                    if (_asnull === false) {
                        return false;
                    }
                } else {
                    _b = true;
                }


            });
            var rn = $("table#table_info tbody tr[role='row']").length;
            if (rn == 0) {
                statSend = false;
                _f = false;
                msgerror = "No hay filas con egresos";
            } else {
                _f = true;
            }

            //FRT21112018 Para validar cantidad de anexos solamente al enviar

            var borra = $("#borr").val(); //FRT03122018 para calidar anexos solamente en Enviar

            var lengthT = $("table#table_anexa tbody tr[role='row']").length;
            _a = true;
            if (borra != "B") {
                if (lengthT == 0) {
                    msgerror = "Es necesario agregar por lo menos 1 Anexo";
                    statSend = false;
                    _a = false;
                } else {
                    _a = true;
                }
            }





            //ENDFRT21112018

            //FRT2311208 PARA VALIDACION DE 50 CARACTERES

            if (borrador != "B") {
                var texto1 = $("#CONCEPTO").val();
                var ct1 = texto1.length;
                ct1 = parseFloat(ct1);
                if (ct1 < 50) {
                    _ct = false;
                    statSend = false;
                    msgerror = "Falta explicación en cabecera";
                } else {
                    _ct = true;
                }
            }

            //END FRT2311208 PARA VALIDACION DE 50 CARACTERES

            //FRT02122018 para validar solamente en borrador

            if (_m) {
                if (_g) {
                    if (_f) {
                        if (_anull) {
                            if (_asnull) {
                                if (_aduplicados) {
                                    if (borra != "B") {
                                        if (_b) {
                                            if (_a) {
                                                if (_ct) {
                                                    //Guardar los valores de la tabla en el modelo para enviarlos al controlador
                                                    copiarTableInfoControl(); //copiarTableInfoPControl();
                                                    //copiarTableSopControl();
                                                    copiarTableRet();
                                                    $('#btn_guardar').trigger("click");
                                                } else {
                                                    statSend = false
                                                    M.toast({ html: msgerror });
                                                }
                                            } else {
                                                statSend = false
                                                M.toast({ html: msgerror });
                                            }
                                        } else {
                                            statSend = false
                                            M.toast({ html: msgerror });
                                        }
                                    } else {
                                        //Guardar los valores de la tabla en el modelo para enviarlos al controlador
                                        copiarTableInfoControl(); //copiarTableInfoPControl();
                                        //copiarTableSopControl();
                                        copiarTableRet();
                                        $('#btn_guardar').trigger("click");
                                    }
                                }
                                else {
                                    statSend = false
                                    M.toast({ html: msgerror });
                                }



                            } else {
                                statSend = false
                                M.toast({ html: msgerror });
                            }

                        } else {
                            statSend = false
                            M.toast({ html: msgerror });
                        }
                    } else {
                        statSend = false
                        M.toast({ html: msgerror });
                    }
                    

                }
                else {

                    statSend = false
                    M.toast({ html: msgerror });
                }



            } else {
                statSend = false
                M.toast({ html: msgerror });

            }



            $("#borr").val(''); //FRT05122018 para  saber cuando es borrador y cuando envio




            //ENDFRT02122018 para validar solamente eb el borrador


            //Termina provisional
            // $('#btn_guardar').click();

        } else {
            alert("Favor de esperar. Se esta generando Solicitud...");

        }

    });


    //FRT03122018 para poder validar el guardado de borrador
    $('#btn_borradorh').on("click", function (e) {
        document.getElementById("loader").style.display = "initial";
        guardarBorrador(false);
        document.getElementById("loader").style.display = "none";
    });


    function guardarBorrador(asyncv) {
        $("#borr").val('B');
        $('#btn_guardarh').trigger("click");
    }

    //ENDFRT03122018 Para poder guardar el borrador

    $('#addRowInfo').on('click', function () {

        var t = $('#table_info').DataTable();
        var _numrow = t.rows().count();
        _numrow++; //frt04122018


        var addedRowInfo = addRowInfo(t, _numrow, "", "", "", "", "", "D", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");//Lej 13.09.2018

        //posinfo++;  frt04122018

        //FRT08112018 AGREGAR Y QUE SE MIREN LAS 
        posinfot = _numrow + _numrow++;

        //Obtener el select de impuestos en la cabecera
        var idselect = "infoSel" + posinfot;

        //Obtener el select de impuestos en la cabecera
        //var idselect = "infoSel" + posinfo;


        //Obtener el valor 
        var imp = $('#IMPUESTO').val();

        //MGC 04092018 Conceptos
        //Crear el nuevo select con los valores de impuestos
        addSelectImpuesto(addedRowInfo, imp, idselect, "", "X");

        updateFooter();
        event.returnValue = false;
        event.cancel = true;
        tamanosRenglones();
    });

    $('.btnD').on("click", function (e) {
        var val = $(this).val();
        $('#archivo').val(val);
        $('#btnDownload').trigger("click");
    });

    $('#file_sopAnexar').on("click", function (e) {
        $("#cargando").css("display", "inline");
    });

    $('#file_sopAnexar').change(function () {
        ////FRT 13112018 PARA PODER SUBIR LOS ARCHIVOS A CAREPETA TEMPORAL
        ////var lengthtemp = $(this).get(0).files.length;

        ////for (var t = 0; t < lengthtemp; t++) {
        ////    var filetemp = $(this).get(0).files[t];
        ////    var filetemp = $(this).get(0).files[t];
        ////    var ft = filetemp.name.substr(filetemp.name.lastIndexOf('.') + 1);
        ////    if (ft.toLowerCase() == "jpg" || ft.toLowerCase() == "png" || ft.toLowerCase() == "jpeg" || ft.toLowerCase() == "doc" || ft.toLowerCase() == "xls" || ft.toLowerCase() == "ppt" ||
        ////        ft.toLowerCase() == "xml" || ft.toLowerCase() == "pdf" || ft.toLowerCase() == "txt" || ft.toLowerCase() == "docx" || ft.toLowerCase() == "xlsx" || ft.toLowerCase() == "pptx") {
        ////        var datatemp = new FormData();
        ////        datatemp.append('file', filetemp);
        ////        $.ajax({
        ////            type: "POST",
        ////            url: '../subirTemporalEditar',
        ////            data: datatemp,
        ////            dataType: "json",
        ////            cache: false,
        ////            contentType: false,
        ////            processData: false,
        ////            success: function (datatemp) {
        ////                if (datatemp !== null || datatemp !== "") {

        ////                }
        ////            },
        ////            error: function (xhr, httpStatusMessage, customErrorMessage) {
        ////                M.toast({ html: httpStatusMessage });
        ////            },
        ////            async: false
        ////        });
        ////    }
        ////}

        ////END FRT13112018

        //Validacion para saber si es sin orden de compra o reembolso
        var val3 = $('#tsol').val();
        val3 = "[" + val3 + "]";
        val3 = val3.replace("{", "{ \"");
        val3 = val3.replace("}", "\" }");
        val3 = val3.replace(/\,/g, "\" , \"");
        val3 = val3.replace(/\=/g, "\" : \"");
        val3 = val3.replace(/\ /g, "");
        var jsval = $.parseJSON(val3);
        if (jsval[0].ID === "SSO") {
            var length = $(this).get(0).files.length;
            var tdata = "";
            var _tab = $('#table_anexa').DataTable();
            for (var i = 0; i < length; i++) {
                var nr = _tab.rows().count();
                //Si nr es 0 significa que la tabla esta vacia
                if (nr === 0) {
                    var file = $(this).get(0).files[i];
                    var fileName = file.name;
                    var fileNameExt = fileName.substr(fileName.lastIndexOf('.') + 1);
                    if (fileNameExt.toLowerCase() == "jpg" || fileNameExt.toLowerCase() == "png" || fileNameExt.toLowerCase() == "jpeg" || fileNameExt.toLowerCase() == "doc" || fileNameExt.toLowerCase() == "xls" || fileNameExt.toLowerCase() == "ppt" ||
                        fileNameExt.toLowerCase() == "xml" || fileNameExt.toLowerCase() == "pdf" || fileNameExt.toLowerCase() == "txt" || fileNameExt.toLowerCase() == "docx" || fileNameExt.toLowerCase() == "xlsx" || fileNameExt.toLowerCase() == "pptx") {
                        tdata = "<tr><td></td><td style=\"text - align: left\">" + (1) + "</td><td style=\"text - align: left\">OK</td><td style=\"text - align: left\">" + file.name + "</td><td style=\"text - align: left\">" + fileNameExt.toLowerCase() + "</td><td style=\"text - align: left\"><input name=\"labels_desc\" class=\"Descripcion\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td></td></tr>";

                        //Lejgg 22-10-2018
                        if (fileNameExt.toLowerCase() === "xml") {
                            var data = new FormData();
                            var _fbool = false;
                            var _resVu = false;
                            data.append('file', file);
                            $.ajax({
                                type: "POST",
                                url: '../procesarXML',
                                data: data,
                                dataType: "json",
                                cache: false,
                                contentType: false,
                                processData: false,
                                success: function (data) {
                                    //FRT20112018 Para validar los RFC
                                    if (data[0] == "1") {
                                        _bcorrecto = true;
                                        _resVu = validarUuid(data[5]);
                                        if (!_resVu) {
                                            _bemisor = validarRFCEmisor(data[4]);
                                            _breceptor = validarRFCReceptor(data[3], data[7]);
                                            if (_bemisor & _breceptor) {
                                                $('#Uuid').val(data[5]);
                                                $('#FECHAD').val(data[1]);
                                                $('#FECHADO').val(data[1]);
                                                $("#FECHAD").trigger("change");
                                                data[2];//Monto Total
                                                //FRT14112018.3 Para Tipo de Cambio en XML
                                                if (data[6] != "MXN") {
                                                    tipo = data[8];
                                                    $('#TIPO_CAMBIO').val(tipo);
                                                    $('#TIPO_CAMBIO').trigger("change");
                                                    var objSelect = document.getElementById("MONEDA_ID");
                                                    objSelect.options[1].selected = true;
                                                }
                                            }

                                        }

                                    } else {
                                        _bcorrecto = false;
                                    }
                                },
                                error: function (xhr, httpStatusMessage, customErrorMessage) {
                                    //
                                },
                                async: false
                            });
                        }
                        if (fileNameExt.toLowerCase() === "xml") {
                            if (_resVu) {
                                //Alert no se metio porque ya hay un xml en la tabla
                                M.toast({ html: "UUID existente en BD" });
                                //document.getElementById('file_sopAnexar').value = '';
                            }
                            else {
                                //quiere decir que es true y que el rfc coincide, por lo tanto hace el pintado de datos en la tabla
                                //FRT20112018 Para saber de donde sale el error
                                if (_bcorrecto) {
                                    if (!_bemisor & !_breceptor) {
                                        //Alert no se metio porque ya hay un xml en la tabla
                                        M.toast({ html: "El RFC de Receptor y Emisor no coinciden" });
                                        //document.getElementById('file_sopAnexar').value = '';
                                    } else {
                                        if (_bemisor) {
                                            if (_breceptor) {
                                                _tab.row.add(
                                                    $(tdata)
                                                ).draw(false).node();
                                            }
                                            else {
                                                //Alert no se metio porque ya hay un xml en la tabla
                                                M.toast({ html: "El RFC de Receptor no coincide" });
                                                //document.getElementById('file_sopAnexar').value = '';
                                            }

                                        } else {
                                            //Alert no se metio porque ya hay un xml en la tabla
                                            M.toast({ html: "El RFC de Emisor no coincide" });
                                            //document.getElementById('file_sopAnexar').value = '';
                                        }
                                    }
                                } else {
                                    //Alert no se metio porque ya hay un xml en la tabla
                                    M.toast({ html: "El XML no tiene formato correcto" });
                                    //document.getElementById('file_sopAnexar').value = '';
                                }
                            }
                        }
                        else {
                            _tab.row.add(
                                $(tdata)
                            ).draw(false).node();
                        }
                    }
                    else {
                        M.toast({ html: "Tipo de archivo no valido: " + fileName });
                    }
                } else {
                    var file = $(this).get(0).files[i];
                    var fileName = file.name;
                    var fileNameExt = fileName.substr(fileName.lastIndexOf('.') + 1);
                    if (fileNameExt.toLowerCase() == "jpg" || fileNameExt.toLowerCase() == "png" || fileNameExt.toLowerCase() == "jpeg" || fileNameExt.toLowerCase() == "doc" || fileNameExt.toLowerCase() == "xls" || fileNameExt.toLowerCase() == "ppt" ||
                        fileNameExt.toLowerCase() == "xml" || fileNameExt.toLowerCase() == "pdf" || fileNameExt.toLowerCase() == "txt" || fileNameExt.toLowerCase() == "docx" || fileNameExt.toLowerCase() == "xlsx" || fileNameExt.toLowerCase() == "pptx") {
                        var _ban = false;
                        var _dupli = false;
                        //Lejgg 22-10-2018------------------------------------------------>
                        $("#table_anexa > tbody  > tr[role='row']").each(function () {
                            var t = $("#table_anexa").DataTable();
                            //Obtener el row para el plugin
                            var tr = $(this);
                            var indexopc = t.row(tr).index();

                            //Obtener valores visibles en la tabla
                            var _tipoAr = $(this).find("td.TYPE").text();
                            if (fileNameExt.toLowerCase() === _tipoAr.trim()) {
                                _ban = true;
                            }
                            if (_ban)
                                return;
                        });

                        //FRT09122018 para no permitir anexar dos veces el archivo
                        $("#table_anexa > tbody  > tr[role='row']").each(function () {
                            var t = $("#table_anexa").DataTable();
                            //Obtener el row para el plugin
                            var tr = $(this);
                            var indexopc = t.row(tr).index();
                            //Verificar si existe el archivo en la tabla FRT09122018
                            var _namefile = $(this).find("td.NAME").text();
                            if (fileName == _namefile) {
                                _dupli = true;
                            }
                            if (_dupli)
                                return;
                        });

                        if (!_dupli) {
                            //Si el archivo es xml entra
                            //LEJGG23/10/18---------------------------------------------------->
                            if (fileNameExt.toLowerCase() === "xml") {
                                var _fbool = false;
                                //Si ban es false, no hay ningun otro archivo xml, entonces metere el registro
                                if (!_ban) {
                                    tdata = "<tr><td></td><td style=\"text - align: left\">" + (nr + 1) + "</td><td style=\"text - align: left\">OK</td><td style=\"text - align: left\">" + file.name + "</td><td style=\"text - align: left\">" + fileNameExt.toLowerCase() + "</td><td style=\"text - align: left\"><input name=\"labels_desc\" class=\"Descripcion\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td></td></tr>";
                                    var data = new FormData();
                                    var _resVu = false;
                                    data.append('file', file);
                                    $.ajax({
                                        type: "POST",
                                        url: '../procesarXML',
                                        data: data,
                                        dataType: "json",
                                        cache: false,
                                        contentType: false,
                                        processData: false,
                                        success: function (data) {
                                            //FRT20112018 Para validar los RFC
                                            if (data[0] == "1") {
                                                _bcorrecto = true;
                                                _resVu = validarUuid(data[5]);
                                                if (!_resVu) {
                                                    _bemisor = validarRFCEmisor(data[4]);
                                                    _breceptor = validarRFCReceptor(data[3], data[7]);
                                                    if (_bemisor & _breceptor) {
                                                        $('#Uuid').val(data[5]);
                                                        $('#FECHAD').val(data[1]);
                                                        $('#FECHADO').val(data[1]);
                                                        $("#FECHAD").trigger("change");
                                                        data[2];//Monto Total
                                                        //FRT14112018.3 Para Tipo de Cambio en XML
                                                        if (data[6] != "MXN") {
                                                            tipo = data[8];
                                                            $('#TIPO_CAMBIO').val(tipo);
                                                            $('#TIPO_CAMBIO').trigger("change");
                                                            var objSelect = document.getElementById("MONEDA_ID");
                                                            objSelect.options[1].selected = true;
                                                        }
                                                    }
                                                }
                                            } else {
                                                _bcorrecto = false;
                                            }
                                        },
                                        error: function (xhr, httpStatusMessage, customErrorMessage) {
                                            //
                                        },
                                        async: false
                                    });
                                    if (_resVu) {
                                        //Alert no se metio porque ya hay un xml en la tabla
                                        M.toast({ html: "UUID existente en BD" });
                                        //document.getElementById('file_sopAnexar').value = '';
                                    }
                                    else {
                                        //quiere decir que es true y que el rfc coincide, por lo tanto hace el pintado de datos en la tabla
                                        //FRT20112018 Para saber de donde sale el error
                                        if (_bcorrecto) {
                                            if (!_bemisor & !_breceptor) {
                                                //Alert no se metio porque ya hay un xml en la tabla
                                                M.toast({ html: "El RFC de Receptor y Emisor no coinciden" });
                                                //document.getElementById('file_sopAnexar').value = '';
                                            } else {
                                                if (_bemisor) {
                                                    if (_breceptor) {
                                                        _tab.row.add(
                                                            $(tdata)
                                                        ).draw(false).node();
                                                    }
                                                    else {
                                                        //Alert no se metio porque ya hay un xml en la tabla
                                                        M.toast({ html: "El RFC de Receptor no coincide" });
                                                        document.getElementById('file_so/*p*/Anexar').value = '';
                                                    }

                                                } else {
                                                    //Alert no se metio porque ya hay un xml en la tabla
                                                    M.toast({ html: "El RFC de Emisor no coincide" });
                                                    //document.getElementById('file_sopAnexar').value = '';
                                                }
                                            }
                                        } else {
                                            //Alert no se metio porque ya hay un xml en la tabla
                                            M.toast({ html: "El XML no tiene formato correcto" });
                                            //document.getElementById('file_sopAnexar').value = '';
                                        }
                                    }
                                }
                                else {
                                    //Alert no se metio porque ya hay un xml en la tabla
                                    M.toast({ html: "Ya existe una factura" });
                                    //document.getElementById('file_sopAnexar').value = '';
                                }
                            }
                            //LEJGG23/10/18----------------------------------------------------<
                            else {
                                tdata = "<tr><td></td><td style=\"text - align: left\">" + (nr + 1) + "</td><td style=\"text - align: left\">OK</td><td style=\"text - align: left\">" + file.name + "</td><td style=\"text - align: left\">" + fileNameExt.toLowerCase() + "</td><td style=\"text - align: left\"><input name=\"labels_desc\" class=\"Descripcion\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td></td></tr>";
                                _tab.row.add(
                                    $(tdata)
                                ).draw(false).node();
                            }
                        //Lejgg 22-10-2018------------------------------------------------>
                        } else {
                            M.toast({ html: "No se pueden repetir el anexo: " + fileName });
                        }
                        
                    }
                    else {
                        M.toast({ html: "Tipo de archivo no valido: " + fileName });
                    }
                }
            }
            //document.getElementById('file_sopAnexar').value = '';
        }
        if (jsval[0].ID === "SRE") {
            var _length = $(this).get(0).files.length;
            var _tab2 = $('#table_anexa').DataTable();
            for (var i = 0; i < _length; i++) {
                var nr = _tab2.rows().count();
                if (nr === 0) {
                    var file = $(this).get(0).files[i];
                    var fileName = file.name;
                    var fileNameExt = fileName.substr(fileName.lastIndexOf('.') + 1);
                    if (fileNameExt.toLowerCase() == "jpg" || fileNameExt.toLowerCase() == "png" || fileNameExt.toLowerCase() == "jpeg" || fileNameExt.toLowerCase() == "doc" || fileNameExt.toLowerCase() == "xls" || fileNameExt.toLowerCase() == "ppt" ||
                        fileNameExt.toLowerCase() == "xml" || fileNameExt.toLowerCase() == "pdf" || fileNameExt.toLowerCase() == "txt" || fileNameExt.toLowerCase() == "docx" || fileNameExt.toLowerCase() == "xlsx" || fileNameExt.toLowerCase() == "pptx") {
                        var nr = _tab2.rows().count();
                        var file = $(this).get(0).files[i];
                        var _data = new FormData();
                        _data.append('file', file);
                        if (fileNameExt.toLowerCase() === "xml") {
                            //Se saca el UUID
                            $.ajax({
                                type: "POST",
                                url: '../procesarXML',
                                data: _data,
                                dataType: "json",
                                cache: false,
                                contentType: false,
                                processData: false,
                                success: function (data) {
                                    if (data !== null || data !== "") {
                                        data[4];//UUID
                                    }
                                },
                                error: function (xhr, httpStatusMessage, customErrorMessage) {
                                    //
                                },
                                async: false
                            });
                        }
                    }
                    else {
                        M.toast({ html: "Tipo de archivo no valido: " + fileName });
                    }
                }
            }
        }
        alinearEstilo();
        //FRT 13112018 PARA PODER SUBIR LOS ARCHIVOS A CAREPETA TEMPORAL
        var lengthtemp = $(this).get(0).files.length;

        for (var t = 0; t < lengthtemp; t++) {
            var filetemp = $(this).get(0).files[t];
            var filetemp = $(this).get(0).files[t];
            var ft = filetemp.name.substr(filetemp.name.lastIndexOf('.') + 1);
            if (ft.toLowerCase() == "jpg" || ft.toLowerCase() == "png" || ft.toLowerCase() == "jpeg" || ft.toLowerCase() == "doc" || ft.toLowerCase() == "xls" || ft.toLowerCase() == "ppt" ||
                ft.toLowerCase() == "xml" || ft.toLowerCase() == "pdf" || ft.toLowerCase() == "txt" || ft.toLowerCase() == "docx" || ft.toLowerCase() == "xlsx" || ft.toLowerCase() == "pptx") {
                var datatemp = new FormData();
                datatemp.append('file', filetemp);
                $.ajax({
                    type: "POST",
                    url: '../subirTemporalEditar',
                    data: datatemp,
                    dataType: "json",
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (datatemp) {
                        if (datatemp !== null || datatemp !== "") {

                        }
                    },
                    error: function (xhr, httpStatusMessage, customErrorMessage) {
                        M.toast({ html: httpStatusMessage });
                    },
                    async: false
                });
            }
        }

        $("#cargando").css("display", "none");
        document.getElementById('file_sopAnexar').value = '';
            // END FRT13112018
    });

    $('#table_anexa tbody').on('click', 'td.select_row', function () {
        //var t = $('#table_anexa').DataTable();
        var tr = $(this).closest('tr');

        $(tr).toggleClass('selected');
        $(tr).css("background-color:#c4f0ff;");
    });

    $('#table_info tbody').on('click', 'td.select_row', function () {
        //var t = $('#table_anexa').DataTable();
        var tr = $(this).closest('tr');

        $(tr).toggleClass('selected');
        $(tr).css("background-color:#c4f0ff;");
    });
    //En esta parte me encargare de bloquear o desbloquear ciertos campos
    var pacc = $('#pacc').val();
    ocultarCamposEdicion(pacc);

    $('#delRowInfo').click(function (e) {
        var t = $('#table_info').DataTable();
        t.rows('.selected').remove().draw(false);
        updateFooter();
        event.returnValue = false;
        event.cancel = true;

        if (tRet2.length > 0) {
            updateTableRet();
        }

        //FRT04122018
        var _num = t.rows().count();
        for (i = 1; i < _num + 1; i++) {
            document.getElementById("table_info").rows[i].cells[1].innerHTML = i;
        }

    });

    $('#delRowAnex').click(function (e) {
        var t = $('#table_anexa').DataTable();

        //FRT27112018 ----------------->
        var tr = $(this);
        var indexopc = t.row(tr).index();
        var num_doc = $('#NUM_DOC').val();
        var type = t.row(indexopc).data()[4];

        if (type == "xml") {
            $.ajax({
                type: "POST",
                url: '../deleteUuid',
                data: { "num_doc": num_doc },
                success: function () {

                },
                error: function (xhr, httpStatusMessage, customErrorMessage) {
                    M.toast({ html: httpStatusMessage });
                },
                async: false
            });
        }
        //FRT27112018------------------------------<
        t.rows('.selected').remove().draw(false);
        event.returnValue = false;
        event.cancel = true;//FRT 12112018  Para recorrer los numero borrados de los anexos

        var _num = t.rows().count();
        for (i = 1; i < _num + 1; i++) {
            document.getElementById("table_anexa").rows[i].cells[1].innerHTML = i;
        }
    });

    tamanoTextArea();
    alinearEstilo();
});

//MGC 03-11-2018 Cadena de autorización
//Obtener los datos de la cadena seleccionada
//Cuando se termina de cargar la página
$(window).on('load', function () {

    //$("#list_detaa").change();

    //var val3 = $(this).val();
    var val3 = $('#list_detaa').val();
    val3 = "[" + val3 + "]";
    val3 = val3.replace("{", "{ \"");
    val3 = val3.replace("}", "\" }");
    val3 = val3.replace(/\,/g, "\" , \"");
    val3 = val3.replace(/\=/g, "\" : \"");
    val3 = val3.replace(/\ /g, "");
    var jsval = $.parseJSON(val3);

    $.each(jsval, function (i, dataj) {
        $("#DETTA_VERSION").val(dataj.VERSION);
        $("#DETTA_USUARIOC_ID").val(dataj.USUARIOC_ID);
        $("#DETTA_ID_RUTA_AGENTE").val(dataj.ID_RUTA_AGENTE);
        $("#DETTA_USUARIOA_ID").val(dataj.USUARIOA_ID);
    });

    $('.materialize-textarea').css("height", "0px");
    tamanosRenglones();
});

//Cadena de autorización
//$('#list_detaa').change(function () {
$('body').on('change', '#list_detaa', function (event, param1) {
    var val3 = $(this).val();
    val3 = "[" + val3 + "]";
    val3 = val3.replace("{", "{ \"");
    val3 = val3.replace("}", "\" }");
    val3 = val3.replace(/\,/g, "\" , \"");
    val3 = val3.replace(/\=/g, "\" : \"");
    val3 = val3.replace(/\ /g, "");
    var jsval = $.parseJSON(val3);

    //LEJGG 21-11-2018 Cadena de autorización----------------------------------------------------------------------------->
    //Obtener los datos de la cadena
    var version = "";
    var usuarioc = "";
    var id_ruta = "";
    var usuarioa = "";
    //lejgg 21-11-2018 Cadena de autorización-----------------------------------------------------------------------------<

    $.each(jsval, function (i, dataj) {
        $("#DETTA_VERSION").val(dataj.VERSION);
        $("#DETTA_USUARIOC_ID").val(dataj.USUARIOC_ID);
        $("#DETTA_ID_RUTA_AGENTE").val(dataj.ID_RUTA_AGENTE);
        $("#DETTA_USUARIOA_ID").val(dataj.USUARIOA_ID);
        //LEJGG 21-11-2018 Cadena de autorización----------------------------------------------------------------------------->
        //Obtener los datos de la cadena
        version = dataj.VERSION;
        usuarioc = dataj.USUARIOC_ID;
        id_ruta = dataj.ID_RUTA_AGENTE;
        usuarioa = dataj.USUARIOA_ID;
        //LEJGG 21-11-2018 Cadena de autorización-----------------------------------------------------------------------------<
    });

    //LEJGG 21-11-2018 Cadena de autorización----------------------------------------------------------------------------->
    //Obtener el monto
    var monto = $('#MONTO_DOC_MD').val();
    //Obtener la sociedad
    var sociedad = $('#SOCIEDAD_ID').val();

    //Al seleccionar un solicitante, obtener la cadena para mostrar

    obtenerCadena(version, usuarioc, id_ruta, usuarioa, monto, sociedad, 0, 0);//MGC 11-12-2018 Agregar Contabilizador 0
    //LEJGG 21-11-2018 Cadena de autorización-----------------------------------------------------------------------------<

});

$('body').on('change', '.IMPUESTO_SELECT', function (event, param1) {

    if (param1 != "tr") {
        //Modificación del sub, iva y total

        var t = $('#table_info').DataTable();
        var tr = $(this).closest('tr'); //Obtener el row 

        //Obtener el valor del impuesto
        var imp = tr.find("td.IMPUESTO option:selected").text();

        //Calcular impuesto y subtotal
        var impimp = impuestoVal(imp);
        impimp = parseFloat(impimp);
        var colTotal = sumarColumnasExtras(tr);//lej 19.08.18

        var sub = tr.find("td.MONTO input").val().replace('$', '').replace(',', '');
        while (sub.indexOf(',') > -1) {
            sub = sub.replace('$', '').replace(',', '');
        }
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
        tr.find("td.MONTO input").val();
        tr.find("td.MONTO input").val(sub);

        //IVA
        tr.find("td.IVA input").val();
        tr.find("td.IVA input").val(impv);

        //Total
        tr.find("td.TOTAL input").val();
        if (colTotal > 0) {
            var _tot = total.replace('$', '').replace(',', '');
            while (_tot.indexOf(',') > -1) {
                _tot = _tot.replace('$', '').replace(',', '');
            }
            var sumt = parseFloat(_tot) - parseFloat(colTotal);
            tr.find("td.TOTAL input").val(toShow(sumt));
        }
        else {
            tr.find("td.TOTAL input").val(total);
        }
        updateFooter();
    }
});

$('body').on('focusout', '.OPER', function (e) {

    var t = $('#table_info').DataTable();
    var tr = $(this).closest('tr'); //Obtener el row 

    //Obtener el valor del impuesto
    var imp = tr.find("td.IMPUESTO option:selected").text();

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
        tr.find("td.MONTO input").val();
        tr.find("td.MONTO input").val(sub);

        //IVA
        tr.find("td.IVA input").val();
        tr.find("td.IVA input").val(impv);

        //Total
        tr.find("td.TOTAL input").val();
        tr.find("td.TOTAL input").val(total);


    }
    else if ($(this).hasClass("MONTO")) {
        if ($(this).hasClass("Cambio")) {//LEJGG 23-11-2018
            //Si tiene la clase,ya no hace nada
        }
        else {
            //Desde el subtotal
            var sub = $(this).val().replace('$', '').replace(',', '');
            //While para que elimine las comas //LEJGG21-11-2018
            while (sub.indexOf(',') > -1) {
                sub = sub.replace('$', '').replace(',', '');
            }
            sub = parseFloat(sub);

            //Lleno los campos de Base Imponible con el valor del monto
            for (x = 0; x < tRet2.length; x++) {
                var _xvalue = tr.find("td.BaseImp" + tRet2[x] + " input").val().replace('$', '').replace(',', '');

                // if (_xvalue === "") {
                //AJAX
                var indret = 0;
                $("#table_ret > tbody  > tr[role='row']").each(function () {
                    var t_ret = $(this).find("td.TRET").text().trim();
                    if (t_ret === tRet2[x]) {
                        indret = $(this).find("td.INDRET").text().trim();
                    }
                });
                var campo = "";
                $.ajax({
                    type: "POST",
                    url: '../getCampoMult',
                    dataType: "json",
                    data: { 'witht': tRet2[x], 'ir': indret },
                    success: function (data) {
                        if (data !== null || data !== "") {
                            campo = data;
                        }
                    },
                    error: function (xhr, httpStatusMessage, customErrorMessage) {
                        M.toast({ html: httpStatusMessage });
                    },
                    async: false
                });
                if (campo == "MONTO") {
                    tr.find("td.BaseImp" + tRet2[x] + " input").val(toShow(sub));
                    //Ejecutamos un ajax para llenar el valor de importe de retencion
                    var _res = porcentajeImpRet(tRet2[x]);
                    _res = (sub * _res) / 100;//Saco el porcentaje
                    tr.find("td.ImpRet" + tRet2[x] + " input").val(toShow(_res));
                }
                if (campo == "IVA") {
                    var xiva = (sub * impimp) / 100;
                    var _iva_ = parseFloat(xiva);
                    tr.find("td.BaseImp" + tRet2[x] + " input").val(toShow(_iva_));
                    var _resx = porcentajeImpRet(tRet2[x]);
                    var _resIva = _iva_ * _resx;
                    //Ejecutamos un ajax para llenar el valor de importe de retencion
                    tr.find("td.ImpRet" + tRet2[x] + " input").val(toShow(_resIva));
                }
                //}
            }
            //Ejecutamos el metodo para sumarizar las columnas
            var colTotal = sumarColumnasExtras(tr);

            // rimpimp = 100 - impimp;

            var impv = (sub * impimp) / 100;
            impv = parseFloat(impv);
            var total = sub + impv;
            total = parseFloat(total);

            var sub = total - impv;

            impv = toShow(impv);
            sub = toShow(sub);
            total = toShow(total);

            //Enviar los valores a la tabla
            //Subtotal
            tr.find("td.MONTO input").val();
            tr.find("td.MONTO input").val(sub);

            //IVA
            tr.find("td.IVA input").val();
            tr.find("td.IVA input").val(impv);

            //Total
            tr.find("td.TOTAL input").val();
            if (colTotal > 0) {
                var _tot = total.replace('$', '').replace(',', '');
                while (_tot.indexOf(',') > -1) {
                    _tot = _tot.replace('$', '').replace(',', '');
                }
                var sumt = parseFloat(_tot) - parseFloat(colTotal);
                tr.find("td.TOTAL input").val(toShow(sumt));
            }
            else {
                tr.find("td.TOTAL input").val(total);
            }
            $(this).addClass("Cambio");
        }
    }
    updateFooter();
    llenarRetencionesIRet();
    llenarRetencionesBImp();
});

$('body').on('keydown', '.OPER', function (e) {
    if ($(this).hasClass("Cambio")) {
        $(this).removeClass("Cambio");
    }
    else {
        //No hagas nada
    }
});

$('body').on('keydown.autocomplete', '.GRUPO_INPUT', function () {
    var tr = $(this).closest('tr'); //Obtener el row

    //Obtener el id de la sociedad
    var soc = $("#SOCIEDAD_ID").val();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: '../getConceptoI',
                dataType: "json",
                data: { "Prefix": request.term, bukrs: soc },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.TIPO_CONCEPTO + "" + item.ID_CONCEPTO + " - " + item.DESC_CONCEPTO, value: item.TIPO_CONCEPTO + "-" + item.ID_CONCEPTO };
                    }))
                }
            })
        },
        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },
        change: function (e, ui) {
            if (!(ui.item)) {
                e.target.value = "";
            }
        },
        select: function (event, ui) {

            var label = ui.item.label;
            var value = ui.item.value;

            //Quitar espacios
            value = value.replace(/\s/g, '');

            //Obtener el despliegue de la llave
            var cadena = value.split("-");
            var tipo = cadena[0];
            var val = cadena[1];

            val = val.replace(/\s/g, '');

            ui.item.value = tipo + "" + val;//MGC 22-10-2018 Etiquetas


            selectConcepto(val, tr, tipo);
        }
    });
});
//LEJGG 06-11-18------------------------------------------------------------>
$('body').on('keydown.autocomplete', '.CCOSTO', function () {
    var tr = $(this).closest('tr'); //Obtener el row

    //Obtener el id de la sociedad
    var soc = $("#SOCIEDAD_ID").val();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: '../getCcosto',
                dataType: "json",
                data: { "Prefix": request.term, "bukrs": soc },
                success: function (data) {
                    response(auto.map(data, function (item) {

                        //return { label: trimStart('0', item.LIFNR) + " - " + item.NAME1, value: trimStart('0', item.LIFNR) };
                        //return { label: trimStart('0', item.CECO1) + " - " + item.TEXT, value: item.CECO1 };
                        return { label: (item.CECO1).toString().trim() + " - " + item.TEXT, value: item.CECO1 };
                    }))
                }
            })
        },
        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },
        change: function (e, ui) {
            if (!(ui.item)) {
                e.target.value = "";
            }
        },
        select: function (event, ui) {

            var label = ui.item.label;
            var value = ui.item.value;
            selectCeco(value, tr);
        }
    });
});

function selectCeco(val, tr) {

    var t = $('#table_info').DataTable();

    //Obtener el row para el plugin //MGC 19-10-2018 
    var trp = $(tr);
    var indexopc = t.row(trp).index();

    tr.find("td.CCOSTO input").val();
    if (val != null & val != "") {
        //Asignar número de ceco a la columna

        tr.find("td.CCOSTO input").val(val);

    }
}

//LEJGG 05-12-2018--------------------------------------------------------------------I
$('body').on('keydown.autocomplete', '#PAYER_ID', function () {
    var tr = $(this).closest('tr'); //Obtener el row

    //Obtener el id de la sociedad
    var soc = $("#SOCIEDAD_ID").val();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: '../getProveedor',
                dataType: "json",
                data: { "Prefix": request.term, bukrs: soc },
                success: function (data) {
                    response(auto.map(data, function (item) {

                        //return { label: trimStart('0', item.LIFNR) + " - " + item.NAME1, value: trimStart('0', item.LIFNR) };
                        return { label: trimStart('0', item.LIFNR) + " - " + item.NAME1, value: item.LIFNR };
                    }));
                }
            });
        },
        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },
        change: function (e, ui) {
            if (!(ui.item)) {
                e.target.value = "";
            }
        },
        select: function (event, ui) {
            var label = ui.item.label;
            var value = ui.item.value;
            selectProveedor(value);
        }
    });
});

function selectProveedor(val) {

    //Obtener las sociedad//MGC 19-10-2018 Condiciones
    var soc = $("#SOCIEDAD_ID").val();//MGC 19-10-2018 Condiciones

    //Add MGC Validar que los proveedores no existan duplicados en la tabla
    var prov = getProveedorC(val, "", soc);//MGC 19-10-2018 Condiciones

    $('#rfc_proveedor').val("");
    $('#nom_proveedor').val("");
    $('#condiciones_prov').val("");//MGC 19-10-2018.2 Condiciones
    $('#condiciones_provt').val("");//MGC 30-10-2018 Condiciones
    $('#MONTO_DOC_MD').val("");//MGC 07-11-2018 Condiciones

    if (prov != null & prov != "") {

        //MGC 30-10-2018 Quitar los espacios de las condiciones
        var cond = "";
        if (prov.COND_PAGO != null) {
            cond = $.trim(prov.COND_PAGO);
        }


        //Asignar valores
        $('#PAYER_ID').val(prov.LIFNR);
        $('#rfc_proveedor').val(prov.STCD1);
        $('#nom_proveedor').val(prov.NAME1);
        //$('#condiciones_prov').val(prov.COND_PAGO);//MGC 19-10-2018.2 Condiciones//MGC 30-10-2018 Quitar los espacios de las condiciones
        $('#condiciones_prov').val(cond);//MGC 19-10-2018.2 Condiciones//MGC 30-10-2018 Quitar los espacios de las condiciones

        //MGC 30-10-2018 Condiciones ---------------------------------------------------->
        //Obtener la descpción de las retenciones

        if (cond != null & cond != "") {
            selectCondicionP(cond);
        }

        if (!conOrden())
            obtenerRetenciones(false);//LEJ 05.09.2018
        else
            obtenerRetencionesP(false);//LEJ 05.09.2018
    }
}

function trimStart(character, string) {
    var startIndex = 0;

    while (string[startIndex] === character) {
        startIndex++;
    }

    return string.substr(startIndex);
}

function getProveedorC(prov, message, soc) {//MGC 19-10-2018 Condiciones
    proveedorValC = "";
    var localval = "";
    if (prov != "") {
        $.ajax({
            type: "POST",
            url: '../getProveedorD',
            dataType: "json",
            data: { "lifnr": prov, "soc": soc },//MGC 19-10-2018 Condiciones

            success: function (data) {

                if (data !== null || data !== "") {
                    asignarValProvC(data);
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (message == "X") {
                    M.toast({ html: "Valor no encontrado" });
                }
            },
            async: false
        });
    }

    localval = proveedorValC;
    return localval;
}

function asignarValProvC(val) {
    proveedorValC = val;
}

function conOrden() {
    if ($("#conOrden").val() === "X")
        return true;
    else
        return false;
}

function obtenerRetenciones(flag) {
    //Obtener la sociedad
    var sociedad_id = $("#SOCIEDAD_ID").val();
    var proveedor = $("#PAYER_ID").val();//lej 05.09.2018

    //Validar que los campos tengan valores
    if ((sociedad_id != "" & sociedad_id != null) & (proveedor != "" & proveedor != null)) {
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
        }

        //Variable para saber cuantos tipos de impuestos tiene
        tRet = [];
        tRet2 = [];
        docsenviar = JSON.stringify({ 'items': jsonObjDocs, "bukrs": sociedad_id, "lifnr": proveedor });
        var t = $('#table_ret').DataTable();
        t.rows().remove().draw(false);
        var agRowRet = [];
        $.ajax({
            type: "POST",
            url: '../getRetenciones',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {
                if (data !== null || data !== "") {
                    $.each(data, function (i, dataj) {
                        var bimp = toShow(dataj.BIMPONIBLE);
                        var imp = toShow(dataj.IMPORTE_RET);
                        tRet.push(dataj.WITHT);//Lej 12.09.18
                        //agRowRet.push(addRowRet(t, dataj.BUKRS, dataj.LIFNR, dataj.WITHT, dataj.DESC, dataj.WT_WITHCD, bimp, imp));
                        agRowRet.push({ t, dataj });
                    }); //Fin de for
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
        //Lej 26-11-18---------------------------------------------------->
        //Para quitar las ligadas
        for (var i = 0; i < agRowRet.length; i++) {
            $.ajax({
                type: "POST",
                url: '../getRetLigadas',
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
        }
        //Para agregar los renglones de retenciones
        for (var i = 0; i < agRowRet.length; i++) {
            var bimp = toShow(agRowRet[i].dataj.BIMPONIBLE);
            var imp = toShow(agRowRet[i].dataj.IMPORTE_RET);
            addRowRet(t, agRowRet[i].dataj.BUKRS, agRowRet[i].dataj.LIFNR, agRowRet[i].dataj.WITHT, agRowRet[i].dataj.DESC, agRowRet[i].dataj.WT_WITHCD, bimp, imp);
        }
        //Mandar llamar un metodo para que realinie los renglones a la izquierda

        //Lej 26-11-18----------------------------------------------------<
        //Lej 12.09.18-------------------------------------------------------
        //Aqui se agregaran las columnas extras a la tabla de detalle
        //$('#table_info').DataTable().clear().draw();//Reinicio la tabla
        if (parseInt(flag) !== 99) {
            $('#table_info').DataTable().destroy();
            $('#table_info').empty();
        }
        var arrCols = [
            {
                "className": 'select_row',
                "data": null,
                "defaultContent": '',
                "orderable": false
            },
            {
                "name": 'Fila',
                "className": 'POS',
                "orderable": false,
                "visible": true //MGC 04092018 Conceptos FRT 041223018
            },
            {
                "name": 'A1',//MGC 22-10-2018 Etiquetas
                "className": 'NumAnexo',
                "orderable": false,
                "width": "1px"
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
                "name": 'TEXTO',
                "className": 'TEXTO',
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
                "name": 'IMPUESTO',
                "className": 'IMPUESTO',
                "orderable": false
            },
            {
                "name": 'IVA',
                "className": 'IVA',
                "orderable": false
            }
        ];
        //Se rearmara la tabla en HTML
        var taInf = $("#table_info");
        taInf.append($("<thead />"));
        taInf.append($("<tbody />"));
        taInf.append($("<tfoot />"));
        var thead = $("#table_info thead");
        thead.append($("<tr />"));
        //Theads
        $("#table_info>thead>tr").append("<th></th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_pos\">Fila</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A1</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A2</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A3</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A4</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A5</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_Texto\">Texto</th>");//FRT08112018
        $("#table_info>thead>tr").append("<th class=\"lbl_cargoAbono\">D/H</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_factura\">Factura</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_tconcepto\">TIPO CONCEPTO</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_grupo\">Concepto</th>");//FRT08112018
        $("#table_info>thead>tr").append("<th class=\"lbl_cuenta\">Cuenta</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_cuentaNom\">Nombre de cuenta</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_tipoimp\">Tipo Imp.</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_imputacion\">Imputación</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_ccosto\">Centro de Costo</th>"); //FRT08112018
        $("#table_info>thead>tr").append("<th class=\"lbl_monto\">Monto</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_impuesto\">Impuesto  </th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_iva\">IVA</th>");
        var colspan = 20;
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
        for (i = 0; i < tRet2.length; i++) {//Agregare las columnas extras
            $("#table_info>thead>tr").append("<th class=\"bi" + tRet2[i] + "\">" + tRet2[i] + "B. I.</th>");
            $("#table_info>thead>tr").append("<th class=\"ir" + tRet2[i] + "\">" + tRet2[i] + "I. R.</th>");
            colspan++;
            colspan++;
        }
        $("#table_info>thead>tr").append("<th class=\"lbl_total\">Total</th>");
        $("#table_info>thead>tr").append("<th class=\"lbl_check\">Check</th>");
        //Tfoot       
        var tfoot = $("#table_info tfoot");
        tfoot.append($("<tr />"));
        //$("#table_info>tfoot>tr").append("<th colspan=\"" + colspan + "\" style=\"text-align:right\"></th>");FRT22112018 para quitar el footer
        //$("#table_info>tfoot>tr").append("<th id=\"total_info\"></th>");FRT22112018 para quitar el footer
        //Se hara un push al arreglo de columnas original
        for (i = 0; i < tRet2.length; i++) {
            arrCols.push({
                "name": tRet2[i] + " B.Imp.",
                "orderable": false
            }, {
                    "name": tRet2[i] + " I. Ret.",
                    "orderable": false
                });
        }
        //Lej 17.09.18
        //Para agregar columna texto al final
        arrCols.push({
            "name": 'TOTAL',
            "className": 'TOTAL',
            "orderable": false
        });

        //MGC ADD 03-10-2018 solicitud con orden de compra
        arrCols.push({
            "name": 'CHECK',
            "className": 'CHECK',
            "orderable": false,
            "visible": false//MGC 22-10-2018 Etiquetas
        });

        //Lej 17.09.18
        extraCols = tRet2.length;
        $('#table_info').DataTable({
            language: {
                "url": "../Scripts/lang/ES.json"
            },
            "destroy": true,
            "paging": false,
            "info": false,
            "searching": false,
            "columns": arrCols,
            columnDefs: [
                {
                    targets: [17, 19],
                    className: 'mdl-data-table__cell--non-numeric'
                }
            ]
        });

        //tamanosRenglones();
        //MGC 22-10-2018 Etiquetas------------------------------------------>
        //Columna tipo de concepto y columna tipo imputación ocultarlas

        //MGC 22-10-2018 Etiquetas------------------------------------------<
        //Lej 12.09.18-------------------------------------------------------
    } else {
        //Enviar mensaje de error true
    }
    //tamanosRenglones();
    // alinearEstiloR();
}
//LEJGG 05-12-2018--------------------------------------------------------------------T

$('body').on('keydown.autocomplete', '#condiciones_prov', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: '../getCondicion',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.COND_PAGO + "-" + item.TEXT, value: item.COND_PAGO + "-" + item.TEXT };
                    }))
                }
            })
        },
        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },
        change: function (e, ui) {
            if (!(ui.item)) {
                e.target.value = "";
                $('#condiciones_provt').val("");
            }
        },
        select: function (event, ui) {

            var label = ui.item.label;
            var value = ui.item.value;

            //Obtener el despliegue de la llave
            var cadena = value.split("-");
            var cond = cadena[0];
            var text = cadena[1];

            ui.item.value = cond;//MGC 22-10-2018 Etiquetas


            selectCondicion(cond, text);
        }
    });
});

function selectCondicion(val, text) {

    //Obtener las sociedad//MGC 19-10-2018 Condiciones
    var soc = $("#SOCIEDAD_ID").val();//MGC 19-10-2018 Condiciones

    $('#condiciones_prov').val(val);
    $('#condiciones_provt').val(text);

}

function insertarCondicion() {
    var val = $('#condiciones_prov').val();
    if (val !== "") {
        $.ajax({
            type: "POST",
            url: '../getCondicionEdit',
            data: { "id": val },
            success: function (data) {
                if (data !== null || data !== "") {
                    $('#condiciones_provt').val(data);//LEJGG 06-11-18 Condiciones
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }
}

function selectCondicionP(val) {

    $.ajax({
        type: "POST",
        url: '../getCondicionT',
        //contentType: "application/json; charset=UTF-8",
        data: { "cond": val },
        success: function (data) {
            if (data !== null || data !== "") {
                $('#condiciones_provt').val(data);//MGC 30-10-2018 Condiciones
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });

}
//LEJGG 06-11-18------------------------------------------------------------<

$('body').on('focusout', '.extrasC', function (e) {
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
        while (_nnm.indexOf(',') > -1) {
            _nnm = _nnm.replace('$', '').replace(',', '');
        }
        _nnm = parseFloat(_nnm.replace(',', ''));

    }

    if (_nnm !== "") {
        var cl = _this.attr('class');
        var arrcl = cl.split('p');
        var _res = porcentajeImpRet(tRet2[arrcl[1]]);
        var indret = 0;
        $("#table_ret > tbody  > tr[role='row']").each(function () {
            var t_ret = $(this).find("td.TRET").text().trim();
            if (t_ret === tRet2[arrcl[1]]) {
                indret = $(this).find("td.INDRET").text().trim();
            }
        });
        var campo = "";
        $.ajax({
            type: "POST",
            url: '../getCampoMult',
            dataType: "json",
            data: { 'witht': tRet2[arrcl[1]], 'ir': indret },
            success: function (data) {
                if (data !== null || data !== "") {
                    campo = data;
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
        if (campo == "MONTO") {
            _res = (_nnm * _res) / 100;//Saco el porcentaje
        }
        if (campo == "IVA") {
            _res = (_nnm * _res);//Saco el porcentaje
        }
        tr.find("td.ImpRet" + tRet2[arrcl[1]] + " input").val(toShow(_res));

        //--------------------------------------LEJ18102018---------------------->
        //hare la operacion para actualizar el total del renglon
        var _mnt = tr.find("td.MONTO input").val().replace('$', '');
        if (_mnt === "") {
            //si esta vacio le agrego un valor de 0.0
            _mnt = parseFloat("0.0");
        }
        else {
            while (_mnt.indexOf(',') > -1) {
                _mnt = _mnt.replace('$', '').replace(',', '');
            }
            _mnt = parseFloat(_mnt.replace(',', ''));
        }
        var _iva = tr.find("td.IVA input").val().replace('$', '');

        if (_iva === "") {
            //si esta vacio le agrego un valor de 0.0
            _iva = parseFloat("0.0");
        } else {
            while (_iva.indexOf(',') > -1) {
                _iva = _iva.replace('$', '').replace(',', '');
            }
            _iva = parseFloat(_iva.replace(',', ''));
        }
        var _ttal = (_mnt + _iva) - sumarColumnasExtras(tr);;
        //actualizar el total
        tr.find("td.TOTAL input").val(toShow(_ttal));
        //--------------------------------------LEJ18102018----------------------<
    }
    $(this).val("$" + _nnm.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    updateFooter();
    llenarRetencionesBImp();
    llenarRetencionesIRet();
});

$('body').on('focusout', '.extrasC2', function (e) {
    //var y = parseFloat(num);
    var total = 0;
    var _t = $('#table_ret').DataTable();
    var centi = 999;
    var _this = $(this);

    sumarizarTodoRow(_this);
    var _v2 = "";
    //Convertir a formato monetario y numerico
    var _nnm = $(this).val().replace("$", "");
    if (_nnm === "") {
        //si esta vacio le agrego un valor de 0.0
        _nnm = parseFloat("0.0");
    } else {
        while (_nnm.indexOf(',') > -1) {
            _nnm = _nnm.replace('$', '').replace(',', '');
        }
        _nnm = parseFloat(_nnm.replace(',', ''));
    }
    $(this).val("$" + _nnm.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $("#table_info > tbody > tr[role = 'row']").each(function (index) {
        for (x = 0; x < tRet2.length; x++) {
            var _var = "ImpRet" + x;
            _v2 = "ImpRetF" + (x + 1);
            if (_this.hasClass(_var)) {
                centi = x;
                break;
            }
        }
        var colex = $(this).find("td." + _v2 + " input").val().replace("$", "").replace(',', '');
        while (colex.indexOf(',') > -1) {
            colex = colex.replace('$', '').replace(',', '');
        }
        //de esta manera saco el renglon y la celad en especifico
        var er = $('#table_ret tbody tr').eq(x).find('td').eq(3).text().replace('$', '');;
        var txbi = $.trim(colex);
        var sum = parseFloat(txbi);
        // sum = parseFloat(sum + y).toFixed(2);
        total += sum;

    });
    if (centi != 9999) {
        $('#table_ret tbody tr').eq(centi).find('td').eq(4).text('$' + total.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        $('#table_ret tbody tr').eq(centi + 2).find('td').eq(4).text('$' + total.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    }
});

function ocultarCamposEdicion(pacc) {
    if (pacc == "B") {
        var _v = $('#PAYER_ID').val();
        if (_v !== "") {
            $('#PAYER_ID').attr("disabled", "none");
        }
    }
    if (pacc == "P") {
        $('#PAYER_ID').attr("disabled", "disabled");
        $('#list_detaa').attr("disabled", "disabled");
    }
    if (pacc == "R") {
        $('#PAYER_ID').attr("disabled", "disabled");
        $('#SOCIEDAD_ID').attr("disabled", "disabled");
        $('#FECHAD').attr("disabled", "disabled");
        $('#list_detaa').attr("disabled", "disabled");
    }
}

function sumarColumnasExtras(tr) {
    //Las columnsas a sumarizar
    //Lej 19.09.18
    //Aqui se guardara la suma de las columnas añadidas
    var sumColAn = 0;
    for (x = 0; x < tRet2.length; x++) {
        var x2 = tr.find("td.ImpRet" + tRet2[x] + " input").val().replace("$", "").replace(",", "");
        while (x2.indexOf(',') > -1) {
            x2 = x2.replace('$', '').replace(',', '');
        }
        if (x2 != "") {
            x2 = parseFloat(x2);
        } else {
            x2 = parseFloat("0");
        }
        //sumColAn = x1 + parseFloat(sumColAn);
        sumColAn = x2 + parseFloat(sumColAn);
    }
    return sumColAn;
}

function addSelectImpuesto(addedRowInfo, imp, idselect, disabled, clase) {

    //Obtener la celda del row agregado
    var ar = $(addedRowInfo).find("td.IMPUESTO");


    var sel = $("<select class = \"IMPUESTO_SELECT  browser-default\" id = \"" + idselect + "\"> ").appendTo(ar);
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
    //var elem = document.getElementById(idselect);
    //var instance = M.Select.init(elem, []);
    $(".IMPUESTO_SELECT").trigger("change");
}

function copiarTableInfoControl() {
    var _numdoc = $("#ndoc").val();
    var lengthT = $("table#table_info tbody tr[role='row']").length;
    var docsenviar = {};
    var docsenviar2 = {};
    var docsenviar3 = {};//lej01.10.2018
    var docsenviar4 = {};//lejgg02.11.2018
    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla oculta y agregarlos al json
        //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
        jsonObjDocs = [];
        jsonObjDocs2 = [];
        jsonObjDocs3 = [];//lej01.10.2018
        //jsonObjDocs4 = [];//lejgg02.11.2018  frt03122018 se mueve para validar
        var i = 1;
        var t = $('#table_info').DataTable();
        //Lej 14.09.18---------------------
        //Aqui armo la tabla oculta de acuerdo a los valores y columnas ingresados de retencion
        var taInf = $("#table_inforeth");
        taInf.append($("<thead />"));
        taInf.append($("<tbody />"));
        var thead = $("#table_inforeth thead");
        thead.append($("<tr />"));
        //for (y = 0; y < tRet2.length; y++) {
        //$("#table_inforeth>thead>tr").append("<th>" + tRet2[i] + " B.Imp.</th>");//Base imponible
        //$("#table_inforeth>thead>tr").append("<th>" + tRet2[i] + " I. Ret.</th>");//Imp Ret
        $("#table_inforeth>thead>tr").append("<th>WITHT</th>");
        $("#table_inforeth>thead>tr").append("<th>WT_WITHCD</th>");
        $("#table_inforeth>thead>tr").append("<th>I. Ret.</th>");//Imp Ret
        $("#table_inforeth>thead>tr").append("<th>B.Imp.</th>");//Base imponible
        // }
        //Lej 14.09.18----------------
        $("#table_info > tbody  > tr[role='row']").each(function () {

            //Obtener el row para el plugin
            var tr = $(this);
            var indexopc = t.row(tr).index();

            var tconcepto = "";
            //Obtener el concepto
            var inpt = t.row(indexopc).data()[10];
            var x_inpt = inpt.split('');
            if (x_inpt.length > 3) {
                var itag = $(inpt);
                inpt = itag.val();
            }
            //LEJ 03-10-2018
            if (inpt == "" || inpt == null) {
                tconcepto = "";
            }
            else {
                tconcepto = inpt;
            }
            //LEJ 03-10-2018
            //MGC 11-10-2018 Obtener valor de columnas ocultas --------------------------->
            //Obtener la cuenta
            var cuenta = t.row(indexopc).data()[12];

            //Obtener la imputación
            var imputacion = t.row(indexopc).data()[15];

            //MGC 22-10-2018 Modificación en etiquetas
            //Obtener el nombre de la cuenta
            var cuentanom = t.row(indexopc).data()[13];

            //MGC 11-10-2018 Obtener valor de columnas ocultas <---------------------------
            //Lej 14.08.2018-------------------------------------------------------------I
            var colsAdded = tRet2.length;//Las retenciones que se agregaron a la tabla
            var retTot = tRet.length;//Todas las retenciones
            //Lej 14.08.2018-------------------------------------------------------------T
            var pos = toNum($(this).find("td.POS").text());
            // var ca = $(this).find("td.CA").text(); //MGC 04092018 Conceptos
            var ca = t.row(indexopc).data()[8];//lejgg 09-10-2018 Conceptos
            var factura = $(this).find("td.FACTURA input").val();
            //var tconcepto = $(this).find("td.TCONCEPTO").text();
            var grupo = $(this).find("td.GRUPO input").val();

            //quitar espacios en blanco //MGC 22-10-2018 Modificación en etiquetas
            grupo = grupo.replace(/\s/g, '');
            var grupoaux = grupo;
            grupo = "";

            //Quitar el tipo de concepto de la llave
            grupo = grupoaux.substring(2, grupoaux.length);

            //var cuenta = $(this).find("td.CUENTA").text();//MGC 04092018 Conceptos //MGC 11-10-2018 Obtener valor de columnas oculta
            //var cuentanom = $(this).find("td.CUENTANOM").text();//MGC 22-10-2018 Modificación en etiquetas
            //var tipoimp = $(this).find("td.TIPOIMP").text();//MGC 22-10-2018 Modificación en etiquetas
            var tipoimp = t.row(indexopc).data()[14];//MGC 22-10-2018 Modificación en etiquetas

            //var imputacion = $(this).find("td.IMPUTACION").text(); //MGC 11-10-2018 Obtener valor de columnas oculta
            var ccosto = $(this).find("td.CCOSTO input").val(); //MGC 11-10-2018 Obtener valor de columnas oculta
            // var impuesto = $(this).find("td.IMPUESTO input").val();
            var impuesto = tr.find("td.IMPUESTO select").val();
            var monto1 = $(this).find("td.MONTO input").val();
            while (monto1.indexOf(',') > -1) {
                monto1 = monto1.replace('$', '').replace(',', '');
            }
            monto1 = monto1.replace(/\s/g, '');
            var monto = toNum(monto1);
            monto = parseFloat(monto);
            var iva1 = $(this).find("td.IVA input").val();
            while (iva1.indexOf(',') > -1) {
                iva1 = iva1.replace('$', '').replace(',', '');
            }
            iva1 = iva1.replace(/\s/g, '');
            var iva = toNum(iva1);
            iva = parseFloat(iva);
            var total1 = $(this).find("td.TOTAL input").val();
            var texto = $(this).find("td.TEXTO Textarea").val();//FRT20112018 
            while (total1.indexOf(',') > -1) {
                total1 = total1.replace('$', '').replace(',', '');
            }
            total1 = total1.replace(/\s/g, '');
            var total = toNum(total1);
            total = parseFloat(total);
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
            //-----------------------
            for (j = 0; j < tRet2.length; j++) {
                //var xvl = $(this).find("td.BaseImp" + tRet2[j] + " input").val();
                //baseImp.push($(this).find("td.BaseImp" + tRet2[j] + " input").val());//LEJ 14.09.2018
                //ImpRet.push($(this).find("td.ImpRet" + tRet2[j] + " input").val());//LEJ 14.09.2018
                //llenare mis documentorp's
                var item2 = {};
                item2["NUM_DOC"] = 0;
                item2["POS"] = pos;
                item2["WITHT"] = tRet2[j];
                item2["WT_WITHCD"] = "01";
                var bim = $(this).find("td.BaseImp" + tRet2[j] + " input").val().replace('$', '').replace(',', '');
                while (bim.indexOf(',') > -1) {
                    bim = bim.replace('$', '').replace(',', '');
                }
                item2["BIMPONIBLE"] = parseFloat(bim);
                var bimret = $(this).find("td.ImpRet" + tRet2[j] + " input").val().replace('$', '').replace(',', '');
                while (bimret.indexOf(',') > -1) {
                    bimret = bimret.replace('$', '').replace(',', '');
                }
                item2["IMPORTE_RET"] = parseFloat(bimret);
                jsonObjDocs2.push(item2);
                item2 = "";
            }

            var item = {};

            item["NUM_DOC"] = 0;
            item["POS"] = pos;
            item["ACCION"] = ca;
            item["FACTURA"] = factura;
            item["TCONCEPTO"] = tconcepto;
            item["GRUPO"] = grupo;
            item["CUENTA"] = cuenta;
            item["NOMCUENTA"] = cuentanom;
            item["TIPOIMP"] = tipoimp;
            item["IMPUTACION"] = imputacion;
            item["CCOSTO"] = ccosto;
            item["MONTO"] = monto;
            item["MWSKZ"] = impuesto;
            item["IVA"] = iva;
            item["TEXTO"] = texto;
            item["TOTAL"] = total;

            jsonObjDocs.push(item);
            i++;
            item = "";

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

        var lengthTA = $("table#table_anexa tbody tr[role='row']").length;

        if (lengthTA > 0) {
            //Para mandar la tabla y comparar
            jsonObjDocs4 = [];//lejgg02.11.2018
            $("#table_anexa > tbody  > tr[role='row']").each(function () {
                //Obtener el row para el plugin
                var tr = $(this);
                var pos = $(this).find("td.POS").text();
                var nombre = $(this).find("td.NAME").text();
                var tipo = $(this).find("td.TYPE").text();
                var desc = $(this).find("td.DESC").text();
                if (desc === "") {
                    desc = $(this).find("td.DESC input").val();
                }
                var item4 = {};
                item4["TIPO"] = tipo;
                item4["DESC"] = desc;
                item4["NAME"] = nombre;
                item4["PATH"] = nombre;
                jsonObjDocs4.push(item4);
                item4 = "";
            });
            docsenviar4 = JSON.stringify({ 'docs': jsonObjDocs4, 'nd': _numdoc });

            $.ajax({
                type: "POST",
                url: '../getPartialCon4',
                contentType: "application/json; charset=UTF-8",
                data: docsenviar4,
                success: function (data) {

                    if (data !== null || data !== "") {

                        $("table#table_anexah tbody").append(data);
                    }

                },
                error: function (xhr, httpStatusMessage, customErrorMessage) {
                    M.toast({ html: httpStatusMessage });
                },
                async: false
            });
        }


    }
}

function porcentajeImpRet(val) {
    var res = 0;
    var indret = 0;
    $("#table_ret > tbody  > tr[role='row']").each(function () {
        var t_ret = $(this).find("td.TRET").text().trim();
        if (t_ret === val) {
            indret = $(this).find("td.INDRET").text().trim();
        }
    });
    $.ajax({
        type: "POST",
        url: '../getPercentage',
        dataType: "json",
        data: { 'witht': val, 'ir': indret },
        success: function (data) {
            if (data !== null || data !== "") {
                res = data;
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
    return res;
}

function copiarTableSopControl() {
    var lengthT = $("table#table_sop tbody tr[role='row']").length;

    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla oculta y agregarlos al json
        //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
        jsonObjDocs = [];
        var i = 1;


        $("#table_sop > tbody  > tr[role='row']").each(function () {

            //Obtener el id de la categoría            
            var t = $('#table_sop').DataTable();
            var tr = $(this);

            var indexopc = t.row(tr).index();
            var opc = t.row(indexopc).data()[1];
        });

        docsenviar = JSON.stringify({ 'docs': jsonObjDocs });

        $.ajax({
            type: "POST",
            url: 'getPartialDis',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {

                if (data !== null || data !== "") {

                    $("table#table_dish tbody").append(data);
                    if (borrador != "X") { //B20180625 MGC 2018.07.03
                        $('#delRow').click();
                    }
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }
}

function copiarTableRet() {

    var lengthT = $("table#table_ret tbody tr[role='row']").length;
    var docsenviar = {};
    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla oculta y agregarlos al json
        //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
        jsonObjDocs = [];
        var i = 1;
        var t = $('#table_ret').DataTable();


        $("#table_ret > tbody  > tr[role='row']").each(function () {
            //Obtener el row para el plugin
            var tr = $(this);
            var indexopc = t.row(tr).index();

            //Obtener la sociedad oculta
            var socret = t.row(indexopc).data()[0];
            //Obtener el proveedor oculto
            var provr = t.row(indexopc).data()[1];
            var ret = $(this).find("td.TRET").text();
            var descret = $(this).find("td.DESCTRET").text();
            var indret = $(this).find("td.INDRET").text();
            var bimp = $(this).find("td.BIMPONIBLE").text();
            var tipoimp = $(this).find("td.IMPRET").text();

            bimp = bimp.replace(/\s/g, '');
            bimp = toNum(bimp);

            var _bimp = parseFloat(toNum(bimp));

            tipoimp = tipoimp.replace(/\s/g, '');
            tipoimp = toNum(tipoimp);

            var _tipoimp = parseFloat(toNum(tipoimp));

            var item = {};

            item["DESC"] = descret;
            item["WITHT"] = ret;
            item["WT_WITHCD"] = indret;
            item["POS"] = i;
            item["BIMPONIBLE"] = _bimp;
            item["IMPORTE_RET"] = _tipoimp;
            item["LIFNR"] = provr;
            item["BUKRS"] = socret;
            jsonObjDocs.push(item);
            i++;
            item = "";

        });

        docsenviar = JSON.stringify({ 'docs': jsonObjDocs });

        $.ajax({
            type: "POST",
            url: '../getPartialRet',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {

                if (data !== null || data !== "") {

                    $("table#table_reth tbody").append(data);
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }

}

function solicitarDatos() {
    var _ref = $('#REFERENCIA').val();
    $.ajax({
        type: "POST",
        url: '../getDocsPSTR',
        dataType: "json",
        data: { 'id': _ref },
        success: function (data) {
            if (data !== null || data !== "") {
                if (data !== "Null") {
                    //
                    inicio = 0;   //FRT08112018
                    armarTablaInfo(data);
                    inicio = 1;    //FRT21112018
                }
                else {
                    //
                }
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
}

//lejgg 23/10/18
function llenarRetencionesIRet() {
    var _t = [];
    var centi = 9999;
    for (x = 0; x < tRet2.length; x++) {
        _t.push("0");
    }
    $("#table_info > tbody > tr[role = 'row']").each(function (index) {
        for (x = 0; x < tRet2.length; x++) {
            var _var = "ImpRet" + x;
            _v2 = "ImpRet" + tRet2[x];
            if ($(this).find("td." + _v2 + " input").hasClass(_var)) {
                centi = x;
                var colex = $(this).find("td." + _v2 + " input").val().replace("$", "").replace(',', '');
                if (colex === "") {
                    colex = parseFloat("0.0");
                }
                while (colex.indexOf(',') > -1) {
                    colex = colex.replace('$', '').replace(',', '');
                }
                var txbi = $.trim(colex);
                var sum = parseFloat(txbi);
                _t[x] = parseFloat(_t[x]) + sum;
            }
        }
    });
    for (x = 0; x < tRet2.length; x++) {
        $('#table_ret tbody tr').eq(x).find('td').eq(4).text('$' + parseFloat(_t[x]).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    }
}

function llenarRetencionesBImp() {
    var _t = [];
    var centi = 0;
    for (x = 0; x < tRet2.length; x++) {
        _t.push("0");
    }
    $("#table_info > tbody > tr[role = 'row']").each(function (index) {
        for (x = 0; x < tRet2.length; x++) {
            var _var = "BaseImp" + x;
            _v2 = "BaseImp" + tRet2[x];
            if ($(this).find("td." + _v2 + " input").hasClass(_var)) {
                var colex = $(this).find("td." + _v2 + " input").val().replace("$", "").replace(',', '');
                if (colex === "") {
                    colex = parseFloat("0.0");
                }
                while (colex.indexOf(',') > -1) {
                    colex = colex.replace('$', '').replace(',', '');
                }
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

function armarTablaInfo(datos) {
    var arrCols = [
        {
            "className": 'select_row',
            "data": null,
            "defaultContent": '',
            "orderable": false
        },
        {
            "name": 'Fila',
            "className": 'POS',
            "orderable": false,
            "visible": true //MGC 04092018 Conceptos  frt04122018
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
            "name": 'TEXTO',
            "className": 'TEXTO',
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
            "visible": false
        },
        {
            "name": 'GRUPO',
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
            "visible": false//lej 11.09.2018
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
            "name": 'IMPUESTO',
            "className": 'IMPUESTO',
            "orderable": false
        },
        {
            "name": 'IVA',
            "className": 'IVA',
            "orderable": false
        }
    ];
    //Se rearmara la tabla en HTML
    var taInf = $("#table_info");
    taInf.append($("<thead />"));
    taInf.append($("<tbody />"));
    taInf.append($("<tfoot />"));
    var thead = $("#table_info thead");
    thead.append($("<tr />"));
    //Theads
    $("#table_info>thead>tr").append("<th></th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_pos\">Fila</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A1</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A2</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A3</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A4</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_NmAnexo\">A5</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_Texto\">Texto</th>");//FRT08112018
    $("#table_info>thead>tr").append("<th class=\"lbl_cargoAbono\">D/H NT</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_factura\">Factura</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_tconcepto\">TIPO CONCEPTO</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_grupo\">Concepto</th>"); //FRT08112018
    $("#table_info>thead>tr").append("<th class=\"lbl_cuenta\">Cuenta NT</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_cuentaNom\">Nombre de cuenta NT</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_tipoimp\">Tipo Imp. NT</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_imputacion\">Imputación NT</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_ccosto\">Centro de Costo</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_monto\">Monto</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_impuesto\">Impuesto</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_iva\">IVA</th>");

    var res = null;
    //----------------------
    //Hare un ajax para traer las columnas extras
    var _ref = $('#NUM_DOC').val();
    $.ajax({
        type: "POST",
        url: '../traerColsExtras',
        dataType: "json",
        data: { 'id': _ref },
        success: function (data) {
            if (data !== null || data !== "") {
                if (data !== "Null") {
                    //
                    res = data;
                }
                else {
                    //
                }
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
    var tRet = [];
    for (x = 0; x < res.length; x++) {
        tRet.push(res[x].WITHT);
    }
    //----------------------
    var colspan = 20;
    tRet2 = tRet;
    for (i = 0; i < tRet.length; i++) {//Revisare las retenciones que tienes ligadas
        $.ajax({
            type: "POST",
            url: '../getRetLigadas',
            data: { 'id': tRet[i] },
            dataType: "json",
            success: function (data) {
                if (data !== null || data !== "") {
                    if (data !== "Null") {
                        tRet2 = jQuery.grep(tRet2, function (value) {
                            return value !== data;
                        });
                    }
                    else {
                        //
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
        $("#table_info>thead>tr").append("<th class=\"bi" + tRet2[i] + "\">" + tRet2[i] + "B. I.</th>");
        $("#table_info>thead>tr").append("<th class=\"ir" + tRet2[i] + "\">" + tRet2[i] + "I. R.</th>");
        colspan++;
        colspan++;
    }
    $("#table_info>thead>tr").append("<th class=\"lbl_total\">Total</th>");
    $("#table_info>thead>tr").append("<th class=\"lbl_check\">Check</th>");
    //Tfoot       
    var tfoot = $("#table_info tfoot");
    tfoot.append($("<tr />"));
    //$("#table_info>tfoot>tr").append("<th colspan=\"" + colspan + "\" style=\"text-align:right\"></th>"); FRT22112018 para quitar el footer
    ////$("#table_info>tfoot>tr").append("<th colspan=\"" + colspan + "\" style=\"text-align:right\">Total:</th>");  FRT08112018 footer FRT22112018 para quitar el footer
    //$("#table_info>tfoot>tr").append("<th id=\"total_info\"></th>"); FRT22112018 para quitar el footer
    //Se hara un push al arreglo de columnas original
    for (i = 0; i < tRet2.length; i++) {
        arrCols.push({
            "name": tRet2[i] + " B.Imp.",
            "orderable": false
        }, {
                "name": tRet2[i] + " I. Ret.",
                "orderable": false
            });
    }
    //Lej 17.09.18
    //Para agregar columna texto al final
    arrCols.push({
        "name": 'TOTAL',
        "className": 'TOTAL',
        "orderable": false
    });
    //solicitud con orden de compra
    arrCols.push({
        "name": 'CHECK',
        "className": 'CHECK',
        "orderable": false,
        "visible": false
    });

    //Lej 17.09.18
    extraCols = tRet2.length;
    $('#table_info').DataTable({

        language: {
            "url": "../../Scripts/lang/ES.json"
        },
        "destroy": true,
        "paging": false,
        "info": false,
        "searching": false,
        "columns": arrCols,
        columnDefs: [
            { targets: 2, width: '580px' },
            { targets: 3, width: '40px' },
            { targets: 4, width: '580px' },
            { targets: 5, width: '580px' },
            { targets: 6, width: '580px' },
            { targets: 19, width: '580px' }

        ]
    });

    //LEJ16102018
    //traigo los campos de la tabla de detalle nt de la parte de baseimpoinle e importe de retencion
    var _infoBIIR = [];
    var _infoAnex = [];
    $.ajax({
        type: "POST",
        url: '../getDocsPr',
        dataType: "json",
        data: { 'id': _ref },
        success: function (data) {
            if (data !== null || data !== "") {
                _infoBIIR = data;
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });

    $.ajax({
        type: "POST",
        url: '../getAnexos',
        dataType: "json",
        data: { 'id': _ref },
        success: function (data) {
            if (data !== null || data !== "") {
                _infoAnex = data;
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
    var _infoc = _infoBIIR.length / 2;
    var arrColExTA = [];

    // if (_infoc === datos.DOCUMENTOPSTR.length) {//LEJGG-05-11-2018
    if (datos.DOCUMENTOPSTR.length > 0) {
        for (var i = 0; i < datos.DOCUMENTOPSTR.length; i++) {
            arrColExTA = [];//Para limpiar el arreglo
            posrows = i;  //FRT08112018

            for (var x = 0; x < _infoBIIR.length; x++) {
                if (_infoBIIR[x].POS === (i + 1)) {
                    arrColExTA.push(_infoBIIR[x]);
                }
            }
            if (datos.DOCUMENTOPSTR[i].TEXTO === null) {
                datos.DOCUMENTOPSTR[i].TEXTO = "";
            }
            if (datos.DOCUMENTOPSTR[i].IMPUTACION === null) {
                datos.DOCUMENTOPSTR[i].IMPUTACION = "";
            }
            if (datos.DOCUMENTOPSTR[i].IMPUTACION === null) {
                datos.DOCUMENTOPSTR[i].IMPUTACION = "";
            }
            if (datos.DOCUMENTOPSTR[i].TCONCEPTO === null) {
                datos.DOCUMENTOPSTR[i].TCONCEPTO = "";
            }
            if (datos.DOCUMENTOPSTR[i].CCOSTO === null) {
                datos.DOCUMENTOPSTR[i].CCOSTO = "";
            }
            if (datos.DOCUMENTOPSTR[i].FACTURA === null) {
                datos.DOCUMENTOPSTR[i].FACTURA = "";
            }

            if (_infoAnex.length > 0) {
                //FRT05122018 Para quitar los ceros en los vacios
                if (_infoAnex[i].a1 == 0) {
                    _infoAnex[i].a1 = "";
                } else {
                    _infoAnex[i].a1;
                }

                if (_infoAnex[i].a2 == 0) {
                    _infoAnex[i].a2 = "";
                } else {
                    _infoAnex[i].a2;
                }

                if (_infoAnex[i].a3 == 0) {
                    _infoAnex[i].a3 = "";
                } else {
                    _infoAnex[i].a3;
                }

                if (_infoAnex[i].a4 == 0) {
                    _infoAnex[i].a4 = "";
                } else {
                    _infoAnex[i].a4;
                }

                if (_infoAnex[i].a5 == 0) {
                    _infoAnex[i].a5 = "";
                } else {
                    _infoAnex[i].a5;
                }

                //ENDFRT05122018 Para quitar los ceros en los vacios


                var ar = addRowInfo($('#table_info').DataTable(), datos.DOCUMENTOPSTR[i].POS, _infoAnex[i].a1, _infoAnex[i].a2, _infoAnex[i].a3, _infoAnex[i].a4, _infoAnex[i].a5, datos.DOCUMENTOPSTR[i].ACCION, datos.DOCUMENTOPSTR[i].FACTURA, datos.DOCUMENTOPSTR[i].TCONCEPTO, datos.DOCUMENTOPSTR[i].GRUPO, datos.DOCUMENTOPSTR[i].CUENTA,
                    datos.DOCUMENTOPSTR[i].NOMCUENTA, datos.DOCUMENTOPSTR[i].TIPOIMP, datos.DOCUMENTOPSTR[i].IMPUTACION, datos.DOCUMENTOPSTR[i].CCOSTO, datos.DOCUMENTOPSTR[i].MONTO, "", datos.DOCUMENTOPSTR[i].IVA, datos.DOCUMENTOPSTR[i].TEXTO, datos.DOCUMENTOPSTR[i].TOTAL, "", "", arrColExTA);

                //Obtener el select de impuestos en la cabecera

                //var idselect = "infoSel0";
                var idselect = "infoSel" + i;  //FRT08112018 Para mostrar todos los impuestos

                //Obtener el valor 
                var imp = $('#IMPUESTO').val();
                //var imp = datos.DOCUMENTOPSTR[i].MWSKZ; //FRT08112018 para traerlo directo de registro
                //Crear el nuevo select con los valores de impuestos
                addSelectImpuesto(ar, imp, idselect, "", "X");
            }
            else {

                var ar = addRowInfo($('#table_info').DataTable(), datos.DOCUMENTOPSTR[i].POS, "", "", "", "", "", datos.DOCUMENTOPSTR[i].ACCION, datos.DOCUMENTOPSTR[i].FACTURA, datos.DOCUMENTOPSTR[i].TCONCEPTO/*Tipo Concepto*/, datos.DOCUMENTOPSTR[i].GRUPO, datos.DOCUMENTOPSTR[i].CUENTA,
                    datos.DOCUMENTOPSTR[i].NOMCUENTA, datos.DOCUMENTOPSTR[i].TIPOIMP, datos.DOCUMENTOPSTR[i].IMPUTACION, datos.DOCUMENTOPSTR[i].CCOSTO/*Centro de costo*/, datos.DOCUMENTOPSTR[i].MONTO, "", datos.DOCUMENTOPSTR[i].IVA, datos.DOCUMENTOPSTR[i].TEXTO, datos.DOCUMENTOPSTR[i].TOTAL, "", "", arrColExTA);
                //Obtener el select de impuestos en la cabecera
                var idselect = "infoSel" + i;
                //Obtener el valor 
                var imp = $('#IMPUESTO').val();
                //Crear el nuevo select con los valores de impuestos
                addSelectImpuesto(ar, imp, idselect, "", "X");

            }
        }
    }

}

function addRowInfo(t, POS, NumAnexo, NumAnexo2, NumAnexo3, NumAnexo4, NumAnexo5, CA, FACTURA, TIPO_CONCEPTO, GRUPO, CUENTA, CUENTANOM, TIPOIMP, IMPUTACION, CCOSTO, MONTO, IMPUESTO, IVA, TEXTO, TOTAL, disabled, check, colsBIIR) { //MGC 03 - 10 - 2018 solicitud con orden de compra
    var _tcgp = TIPO_CONCEPTO + GRUPO;//Para que grupo se muestre correctamente //Lejgg 06-11-18
    var ceco = "";
    if (TIPOIMP == 'P') {
        ceco = "<input disabled class=\"CCOSTO\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + CCOSTO + "\">";
    } else {
        ceco = "<input class=\"CCOSTO\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + CCOSTO + "\">";
    }

    var r = addRowl(
        t,
        POS,
        "<input class=\"NumAnexo\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + NumAnexo + "\">",
        "<input class=\"NumAnexo2\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + NumAnexo2 + "\">",
        "<input class=\"NumAnexo3\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + NumAnexo3 + "\">",
        "<input class=\"NumAnexo4\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + NumAnexo4 + "\">",
        "<input class=\"NumAnexo5\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + NumAnexo5 + "\">",

        CA,//MGC 04092018 Conceptos
        "<input " + disabled + " class=\"FACTURA\" style=\"font-size:12px;width:75px;\" type=\"text\" id=\"\" name=\"\" value=\"" + FACTURA + "\">",
        TIPO_CONCEPTO,
        "<input " + disabled + " class=\"GRUPO GRUPO_INPUT\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + _tcgp + "\">",//LEJGG 06-11-18
        CUENTA,//MGC 04092018 Conceptos
        CUENTANOM,
        TIPOIMP,
        IMPUTACION,
        ceco,
        "<input " + disabled + " class=\"MONTO OPER\" style=\"font-size:12px;width:76px;\" type=\"text\" id=\"\" name=\"\" value=\"" + MONTO + "\">",
        "",
        "<input disabled class=\"IVA\" style=\"font-size:12px;width:75px;\" type=\"text\" id=\"\" name=\"\" value=\"" + IVA + "\">",
        "<textarea " + disabled + " class=\"materialize-textarea\" style=\"font-size:12px;width:100px;height:0px;\" maxlength=\"50\" type=\"text\" id=\"TEXTO\" name=\"TEXTO\" value=\"" + TEXTO + "\">" + TEXTO + "</textarea>",//Lej 13.09.2018//FRT20112018 
        //"<input " + disabled + " style=\"font-size:12px;width:100px;height:91px;\" maxlength=\"50\" type=\"text\" id=\"TEXTO\" name=\"TEXTO\" value=\"" + TEXTO + "\">",//Lej 13.09.2018//FRT20112018 
        TOTAL,
        check, //MGC 03-10-2018 solicitud con orden de compra
        colsBIIR
    );

    return r;
}

function addRowl(t, pos, nA, nA2, nA3, nA4, nA5, ca, factura, tipo_concepto, grupo, cuenta, cuentanom, tipoimp, imputacion, ccentro, monto, impuesto,
    iva, texto, total, check, _dExtra) {
    //Lej 13.09.2018---
    var colstoAdd = "";
    for (i = 0; i < extraCols; i++) {
        if (_dExtra === "") {
            colstoAdd += '<td class=\"BaseImp' + tRet2[i] + '\"><input class=\"extrasC BaseImp' + i + '\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td>';
            colstoAdd += '<td class=\"ImpRet' + tRet2[i] + '\"><input class=\"extrasC2 ImpRet' + i + '\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td>';
        }
        else {
            colstoAdd += '<td class=\"BaseImp' + tRet2[i] + '\"><input class=\"extrasC BaseImp' + i + '\" style=\"font-size:12px;width:75px;\" type=\"text\" id=\"\" name=\"\" value=\"' + toShow(_dExtra[i].BIMPONIBLE) + '\"></td>';
            colstoAdd += '<td class=\"ImpRet' + tRet2[i] + '\"><input class=\"extrasC2 ImpRet' + i + '\" style=\"font-size:12px;width:75px;\" type=\"text\" id=\"\" name=\"\" value=\"' + toShow(_dExtra[i].IMPORTE_RET) + '\"></td>';
        }
    }
    colstoAdd += "<td><input disabled class=\"TOTAL OPER\" style=\"font-size:12px;width:80px;\" type=\"text\" id=\"\" name=\"\" value=\"" + total + "\"></td>"
        + "<td><p><label><input type=\"checkbox\" checked=\"" + check + "\" /><span></span></label></p></td>";//MGC 03 - 10 - 2018 solicitud con orden de compra
    //var table_rows = '<tr><td></td><td>' + pos + '</td><td><input class=\"NumAnexo\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td><input class=\"NumAnexo2\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td><input class=\"NumAnexo3\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td><input class=\"NumAnexo4\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td><td><input class=\"NumAnexo5\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\"></td>' +
    //    '<td> ' + texto + '</td><td>' + ca + '</td><td>' + factura + '</td><td>' + tipo_concepto
    //    + '</td><td>' + grupo + '</td><td>' + cuenta + '</td><td>' + cuentanom + '</td><td>' + tipoimp + '</td><td>' + imputacion
    //    + '</td><td>' + ccentro + '</td><td>' + monto + '</td><td>' + impuesto + '</td><td>' + iva + '</td>' + colstoAdd + '</tr>';
    var table_rows = '<tr><td></td><td>' + pos + '</td><td>' + nA + '</td><td>' + nA2 + '</td><td>' + nA3 + '</td><td>' + nA4 + '</td><td>' + nA5 + '</td>' +
        '<td> ' + texto + '</td><td>' + ca + '</td><td>' + factura + '</td><td>' + tipo_concepto
        + '</td><td>' + grupo + '</td><td>' + cuenta + '</td><td>' + cuentanom + '</td><td>' + tipoimp + '</td><td>' + imputacion
        + '</td><td>' + ccentro + '</td><td>' + monto + '</td><td>' + impuesto + '</td><td>' + iva + '</td>' + colstoAdd + '</tr>';
    //Lej 13.09.2018--------------------------------
    if (extraCols === 0) {//Lej 13.09.2018
        var r = t.row.add([
            "",
            pos,
            nA,
            nA2,
            nA3,
            nA4,
            nA5,
            texto,
            ca,
            factura,
            tipo_concepto,
            grupo,
            cuenta,
            cuentanom,
            tipoimp,
            imputacion,
            ccentro,
            monto,
            impuesto,
            iva,
            "<input disabled class=\"TOTAL OPER\" style=\"font-size:12px;width:80px;\" type=\"text\" id=\"\" name=\"\" value=\"" + total + "\">",
            "<input class=\"CHECK\" style=\"font-size:12px;\" type=\"checkbox\" id=\"\" name=\"\" value=\"" + check + "\">" //MGC 03 - 10 - 2018 solicitud con orden de compra
        ]).draw(false).node();
    } else {
        var r = t.row.add(
            $(table_rows)//Lej 15.09.2018
        ).draw(false).node();
    }

    return r;
}

function updateTableRet() {
    llenarRetencionesIRet();
    llenarRetencionesBImp();
}

function updateFooter() {
    resetFooter();

    var t = $('#table_info').DataTable();
    var total = 0;

    $("#table_info > tbody > tr[role = 'row']").each(function (index) {
        //var col11 = $(this).find("td.TOTAL input").val();
        var col11 = $(this).find("td.TOTAL input").val();

        //Saber si el renglón se va a sumar
        var tr = $(this);
        var indexopc = t.row(tr).index();

        //Obtener la accion
        var ac = t.row(indexopc).data()[2];



        col11 = col11.replace(/\s/g, '');
        var val = toNum(col11);
        val = convertI(val);
        if ($.isNumeric(val)) {
            if (ac != "H") {
                total += val;
            }
        }
    });

    total = total.toFixed(2);


    //FRT08112018 Para eliminar el Total
    if (inicio == 0) {
        totalinicio = "";
        $('#total_info').text((totalinicio));
        //inicio = 1;
    } else {
        totalinicio = "";
        $('#total_info').text(toShow(totalinicio));
    }
    //$('#total_info').text(toShow(total));  
    //FRT08112018 Para eliminar el Total

    $('#MONTO_DOC_MD').val(toShow(total));//Lej 18.09.2018
    $('#mtTot').val($('#MONTO_DOC_MD').val());//Lej 29.09.2018
    $('#total_info1').text(toShow(total));//FRT22112018


}

function resetTabs() {

    var ell = document.getElementById("tabs");
    var instances = M.Tabs.getInstance(ell);

    var active = $('.tabs').find('.active').attr('href');
    active = active.replace("#", "");
    instances.select(active);
    //instances.updateTabIndicator
}

function impuestoVal(ti) {

    var res = 0;

    if (ti != "") {
        var tsol_val = $('#impuestosval').val();
        var jsval = $.parseJSON(tsol_val);
        $.each(jsval, function (i, dataj) {
            var _i = ti.split('-');
            var _im = $.trim(_i[0]);
            if (dataj.MWSKZ == _im) {
                res = dataj.KBETR;
                return false;
            }
        });
    }

    return res;
}

//lEJGG 07-11-2018
function validarFacs() {
    var _ban = false;
    $("#table_anexa > tbody  > tr[role='row']").each(function () {
        var t = $("#table_anexa").DataTable();
        //Obtener el row para el plugin
        var tr = $(this);
        var indexopc = t.row(tr).index();

        //Obtener valores visibles en la tabla
        var _tipoAr = $(this).find("td.TYPE").text();
        if ("xml" === _tipoAr) {
            _ban = true;
        }
        if (_ban)
            return;
    });
    return _ban;
}

function resetFooter() {
    $('#total_dis').text("$0");
}

function convertI(i) {
    return typeof i === 'string' ?
        i.replace(/[\$,]/g, '') * 1 :
        typeof i === 'number' ?
            i : 0;
}

function selectConcepto(val, tr, tipo) {
    var t = $('#table_info').DataTable();

    ////Add Validar que los conceptos no existan duplicados en la tabla
    var conExist = valConcepto(val, tipo);


    //Obtener el row para el plugin //MGC 11-10-2018 No enviar correos 
    var trp = $(tr);
    var indexopc = t.row(trp).index();

    //Add MGC Validar que los conceptos no existan duplicados en la tabla
    if (conExist) {
        M.toast({ html: 'Ya hay un concepto con ese mismo identificador' });
        tr.find("td.GRUPO input").val();
    } else {
        //Agregar el id
        tr.find("td.GRUPO input").val();
        tr.find("td.GRUPO input").val(tipo + "" + val);
        //Obtener la sociedad
        var soc = $("#SOCIEDAD_ID").val();
        //Cancepto
        var con = getConceptoC(val, tipo, soc, "");

        //Asignar los valores en la tabla
        if (con != "" & con != null) {

            //Cuenta
            t.cell(indexopc, 12).data(con.CUENTA).draw();

            //Nombre de la cuenta
            t.cell(indexopc, 13).data(con.DESC_CONCEPTO).draw();

            //Tipo de imputación
            t.cell(indexopc, 14).data(con.TIPO_IMPUTACION).draw();

            //Actualizar el tipo concepto
            var indexopc = t.row(tr).index();
            t.cell(indexopc, 10).data("<input class=\"\" disabled style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + tipo + "\">").draw();//LEJ 01.10.2018

            //ocultar o mostrar el centro de costo
            if (con.TIPO_IMPUTACION == "P") {
                //Armar el elemento pep
                //Obtener los guiones
                var p0 = val.substring(0, 3);
                var p1 = val.substring(3, 6);
                //var PEP = "RE-00900-I" + soc + "" + tipo + "-" + val;
                var PEP = "RE-00900-I" + soc + "" + tipo + "-" + p0 + "-" + p1;

                t.cell(indexopc, 15).data(PEP).draw();

                tr.find("td.CCOSTO input").prop('disabled', true);
            } else if (con.TIPO_IMPUTACION == "K") {
                tr.find("td.CCOSTO input").prop('disabled', false);
            } else {
                tr.find("td.CCOSTO input").prop('disabled', false);
            }

        } else {
            tr.find("td.GRUPO input").val();
        }
    }
}

function valConcepto(con, tipo) {

    var res = false;

    var lengthT = $("table#table_info tbody tr[role='row']").length;

    if (lengthT > 0) {

        $("#table_info > tbody  > tr[role='row']").each(function () {
            var c = "";
            c = $(this).find("td.GRUPO input").val();

            var t = "";
            t = $(this).find("td.CONCEPTO").text();

            if (con == c && tipo == t) {
                res = true;
                return false;
            }

        });
    }
    return res;
}

//Pestaña contabilidad
function getConceptoC(con, tipo, bukrs, message) {
    conceptoValC = "";
    var localval = "";
    if (con != "") {
        $.ajax({
            type: "POST",
            url: '../getConcepto',
            dataType: "json",
            data: { "id": con, "tipo": tipo, "bukrs": bukrs },

            success: function (data) {

                if (data !== null || data !== "") {
                    asignarValConC(data);
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (message == "X") {
                    M.toast({ html: "Valor no encontrado" });
                }
            },
            async: false
        });
    }

    localval = conceptoValC;
    return localval;
}

function asignarValConC(val) {
    conceptoValC = val;
}
//LEJGG 13/11/2018
function tamanosRenglones() {
    //TEXTO
    var t_ret = $("#table_info>thead>tr").find('th.TEXTO');
    t_ret.css("text-align", "center");
    //total
    var t_fac = $("#table_info>thead>tr").find('th.FACTURA');
    t_fac.css("text-align", "center");
    //Monto
    var t_mt = $("#table_info>thead>tr").find('th.MONTO');
    t_mt.css("text-align", "center");
    // t_mt.css("width", "100px");
    //IVA
    var t_iva = $("#table_info>thead>tr").find('th.IVA');
    t_iva.css("text-align", "center");
    //SELECT IMPUESTO
    var t_imps = $("#table_info>thead>tr").find('th.IMPUESTO_SELECT');
    t_imps.css("text-align", "center");
    //reviso si tiene retenciones para agregarles nueva medidas
    if (tRet2.length > 0) {
        for (var i = 0; i < tRet2.length; i++) {
            var _cex = $("#table_info>thead>tr").find("th.bi" + tRet2[i]);
            _cex.css("text-align", "center");
            var _cex2 = $("#table_info>thead>tr").find("th.ir" + tRet2[i]);
            _cex2.css("text-align", "center");
        }
    }
    //total
    var t_tot = $("#table_info>thead>tr").find('th.TOTAL');
    t_tot.css("text-align", "center");
    //FILA
    var tpos = $("#table_info>tbody>tr").find('td.POS');
    tpos.css("text-align", "center");
    tpos.css("font-size", "15px");
    $(window).resize();
}


function validarUuid(uuid) {
    var ban = false;
    $.ajax({
        type: "POST",
        url: '../getUuid',
        data: { 'id': uuid },
        dataType: "json",
        success: function (data) {
            if (data !== null || data !== "") {
                if (data != "Null") {
                    //Si es diferente a null significa que si hay coincidencia
                    ban = true;
                }
                else {
                    //
                }
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: customErrorMessage });
        },
        async: false
    });
    return ban;
}

//FRT2011218 Para Validacion individual de RFCs
function validarRFCEmisor(rfc_pro) {
    var _rfc_pro = $('#rfc_proveedor').val();
    if (_rfc_pro.trim() === rfc_pro) {
        return true;
    }
    else {
        return false;
    }
}

function validarRFCReceptor(rfc_soc, rfc_soc_doc) {

    if (rfc_soc_doc.trim() === rfc_soc.trim()) {
        return true;
    }
    else {
        return false;
    }
}

//LEJGG 21-11-2018 Cadena de autorización----------------------------------------------------------------------------->
//Al seleccionar un solicitante, obtener la cadena para mostrar

function obtenerCadena(version, usuarioc, id_ruta, usuarioa, monto, sociedad, vc1, vc2) {//MGC 11-12-2018 Agregar Contabilizador 0

    try {
        monto = parseFloat(monto) || 0.0;
    } catch (err) {
        monto = 0.0;
    }

    //Eliminar Registros
    $("#tableAutorizadores > tbody > tr").remove();

    //MGC 11-12-2018 Agregar Contabilizador 0--------------->
    $('#VERSIONC1').val("");
    $('#VERSIONC2').val("");
   //MGC 11-12-2018 Agregar Contabilizador 0---------------<

    $.ajax({
        type: "POST",
        url: '../getCadena',
        data: { 'version': version, 'usuarioc': usuarioc, 'id_ruta': id_ruta, 'usuarioa': usuarioa, 'monto': monto, 'bukrs': sociedad, 'vc1': vc1, 'vc2': vc2 },//MGC 11-12-2018 Agregar Contabilizador 0
        dataType: "json",
        success: function (data) {
            if (data !== null || data !== "") {

                //MGC 11-12-2018 Agregar Contabilizador 0--------------->
                //Obtener la cadena y las versiones de los autorizadores
                var c1 = data.vc1;
                var c2 = data.vc2;

                $('#VERSIONC1').val(c1);
                $('#VERSIONC2').val(c2);
                //MGC 11-12-2018 Agregar Contabilizador 0---------------<

                $.each(data.cadenal, function (i, dataj) {//MGC 11-12-2018 Agregar Contabilizador 0
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

//Para verificar el tamaño del textarea
//LEJGG 22/11/2018
function tamanoTextArea() {
    $("#table_info > tbody > tr[role = 'row']").each(function (index) {
        var colex = $(this).find("td.TEXTO");
        colex.css('height', '100px');
    });
}

//FRT21112018.3 fUNCIONES PARA TENER EL TIPO DE CAMBIO
function getTipoCambio(moneda, fecha) {
    tipocambio = "";
    var localval = "";
    if (moneda != "") {
        $.ajax({
            type: "POST",
            url: '../getTipoCambio',
            dataType: "json",
            data: { "tcurr": moneda, "gdatu": fecha },//MGC 19-10-2018 Condiciones

            success: function (data) {

                if (data !== null || data !== "") {
                    asignarVal(data);
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (message == "X") {
                    M.toast({ html: "Valor no encontrado" });
                }
            },
            async: false
        });
    }

    localval = tipocambio;
    return localval;
}

function asignarVal(val) {
    tipocambio = val;
}


//END FRT21112018

//LEJGG 21-11-2018 Cadena de autorización-----------------------------------------------------------------------------<
function sumarizarTodoRow(_this) {
    //Inicio codio sumarizar
    //Ejecutamos el metodo para sumarizar las columnas
    //var t = $('#table_info').DataTable();
    var tr = _this.closest('tr'); //Obtener el row 
    //Obtener el valor del impuesto
    var imp = tr.find("td.IMPUESTO select").val();
    //Calcular impuesto y subtotal
    var impimp = impuestoVal(imp);
    impimp = parseFloat(impimp);
    var colTotal = sumarColumnasExtras(tr);

    //Desde el subtotal
    var sub = tr.find("td.MONTO input").val().replace('$', '').replace(',', '');
    while (sub.indexOf(',') > -1) {
        sub = sub.replace('$', '').replace(',', '');
    }
    sub = parseFloat(sub);

    //rimpimp = 100 - impimp;

    var impv = (sub * impimp) / 100;
    impv = parseFloat(impv);

    var total = sub + impv;
    total = parseFloat(total);
    var sub = total - impv;

    impv = toShow(impv);
    sub = toShow(sub);
    total = toShow(total);

    //Enviar los valores a la tabla
    //Subtotal
    tr.find("td.MONTO input").val();
    tr.find("td.MONTO input").val(sub);

    //IVA
    tr.find("td.IVA input").val();
    tr.find("td.IVA input").val(impv);

    //Total
    tr.find("td.TOTAL input").val();
    if (colTotal > 0) {
        var _tot = total.replace('$', '').replace(',', '');
        while (_tot.indexOf(',') > -1) {
            _tot = _tot.replace('$', '').replace(',', '');
        }
        var sumt = parseFloat(_tot) - parseFloat(colTotal);
        tr.find("td.TOTAL input").val(toShow(sumt));
    }
    else {
        tr.find("td.TOTAL input").val(total);
    }
    //Fin de codigo que sumariza
    updateFooter();

}

function alinearEstilo() {
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
    //--------
    //Para los titulos
    var tpo = $("#table_anexa>thead>tr").find('th.POS');
    tpo.css("text-align", "left");
    var ts = $("#table_anexa>thead>tr").find('th.STAT');
    ts.css("text-align", "left");
    var tn = $("#table_anexa>thead>tr").find('th.NAME');
    tn.css("text-align", "left");
    var tt = $("#table_anexa>thead>tr").find('th.TYPE');
    tt.css("text-align", "left");
    var tde = $("#table_anexa>thead>tr").find('th.DESC');
    tde.css("text-align", "left");
    //--------
    $("#table_anexa > tbody  > tr[role='row']").each(function () {
        //1
        var R1 = $(this).find("td.POS");
        R1.css("text-align", "left");
        //2
        var R2 = $(this).find("td.STAT");
        R2.css("text-align", "left");
        //3
        var R3 = $(this).find("td.NAME");
        R3.css("text-align", "left");
        //4
        var R4 = $(this).find("td.TYPE");
        R4.css("text-align", "left");
        //5
        var R5 = $(this).find("td.DESC");
        R5.css("text-align", "left");
    });
}

////MGC 10-12-2018 Firma del usuario cancelar -------------------------------------------------->
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
 ////MGC 10-12-2018 Firma del usuario cancelar --------------------------------------------------<