$(document).ready(function () {

    //Validar que los labels esten activos
    //Información
    $("label[for='notas_txt']").addClass("active");
    //Nombre    
    if ($('#cli_name').val() != "") {
        $("label[for='cli_name']").addClass("active");
    }
    //Razón social
    if ($('#parvw').val() != "") {
        $("label[for='parvw']").addClass("active");
    }
    //Razón social
    if ($('#vkorg').val() != "") {
        $("label[for='vkorg']").addClass("active");
    }
    //Tax ID
    if ($('#stcd1').val() != "") {
        $("label[for='stcd1']").addClass("active");
    }
    //Canal
    if ($('#vtweg').val() != "") {
        $("label[for='vtweg']").addClass("active");
    }
    //Payer nombre
    if ($('#payer_nombre').val() != "") {
        $("label[for='payer_nombre']").addClass("active");
    }
    //Email nombre
    if ($('#payer_email').val() != "") {
        $("label[for='payer_email']").addClass("active");
    }
    //Soporte
    $('#table_sop').DataTable({
        "language": {
            "zerorecords": "no hay registros",
            "infoempty": "registros no disponibles",
            "decimal": ".",
            "thousands": ","
        },
        "paging": false,
        //        "ordering": false,
        "info": false,
        "searching": false,
        "columns": [
            //{
            //    "classname": 'select_row',
            //    "orderable": false,
            //    "data": null,
            //    "defaultcontent": ''
            //},
            {
                "name": 'POS',
                "className": 'POS',
            },
            {
                "name": 'FACTURA',
                "className": 'FACTURA'
            },
            {
                "name": 'FECHA',
                "className": 'FECHA'
            },
            {
                "name": 'PROVEEDOR',
                "className": 'PROVEEDOR'
            },
            {
                "name": 'PROVEEDOR_TXT',
                "className": 'PROVEEDOR_TXT'
            },
            {
                "name": 'CONTROL',
                "className": 'CONTROL'

            },
            {
                "name": 'AUTORIZACION',
                "className": 'AUTORIZACION'
            },
            {
                "name": 'VENCIMIENTO',
                "className": 'VENCIMIENTO'
            },
            {
                "name": 'FACTURAK',
                "className": 'FACTURAK'
            },
            {
                "name": 'EJERCICIOK',
                "className": 'EJERCICIOK'
            },
            {
                "name": 'BILL_DOC',
                "className": 'BILL_DOC'
            },
            {
                "name": 'BELNR',
                "className": 'BELNR'
            }
        ]
    });

    $('#matcat').click(function (e) {

        var kunnr = $('#payer_id').val();
        definirTipoCliente(kunnr)
        event.returnvalue = false;
        event.cancel = true;
    });


    //$('#sendTable').click(function (e) {

    //    event.returnvalue = false;
    //    event.cancel = true;
    //});

    //Evaluar la extensión y tamaño del archivo a cargar
    $('.file_soporte').change(function () {
        var length = $(this).length;
        var message = "";
        var namefile = "";
        if (length > 0) {
            //Validar tamaño y extensión
            var file = $(this).get(0).files;
            if (file.length > 0) {
                var sizefile = file[0].size;
                namefile = file[0].name;
                if (sizefile > 20971520) {
                    message = 'Error! Tamaño máximo del archivo 20 M --> Archivo ' + namefile + " sobrepasa el tamaño";

                }

                if (!evaluarExtSoporte(namefile)) {
                    message = "Error! Tipos de archivos aceptados 'xlsx', 'doc', 'pdf', 'png', 'msg', 'zip', 'jpg', 'docs' --> Archivo " + namefile + " no es compatible";

                }
            }
        } else {
            message = "No selecciono archivo";
        }

        if (message != "") {
            $(this).val("");
            M.toast({ html: message });
        } else {
            //Verificar los nombres
            var id = $(this).attr('id');
            var res = evaluarFilesName(id, namefile);

            if (res) {
                //Nombre duplicado
                M.toast({ html: 'Ya existe un archivo con ese mismo nombre' });
            }
        }
    });
    //Negociación
    if ($('#notas_soporte').val() != "") {
        $("label[for='notas_soporte']").addClass("active");
    }

    //Distribución    
    $('#table_dis').DataTable({
        "language": {
            "zeroRecords": "No hay registros",
            "infoEmpty": "Registros no disponibles",
            "decimal": ".",
            "thousands": ","
        },
        "paging": false,
        //        "ordering": false,
        "info": false,
        "searching": false,
        "columns": [
            {
                "className": 'id_row',
                "orderable": false,
                "defaultContent": ''

            },
            {
                "className": 'detail_row',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            {
                "className": 'select_row',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            {},
            {},
            {},
            {},
            {},
            {},
            {},
            {},
            {},
            {},
            {},
            {
                "className": 'total'//RSG 11.06.2018
            }

        ]
    });

    $('#table_dis tbody').on('click', 'td.select_row', function () {
        var tr = $(this).closest('tr');
        $(tr).toggleClass('selected');
    });

    $('#delRow').click(function (e) {
        var t = $('#table_dis').DataTable();
        t.rows('.selected').remove().draw(false);
        //Validar si es categoría por porcentaje
        //Obtener el tipo de negociación
        var neg = $("#select_neg").val();
        //Obtener la distribución
        var dis = $("#select_dis").val();
        if (neg == "P" && dis == "C") {
            //Actualizar la tabla con los porcentajes
            updateTableCat();
        } else {
            updateFooter();
        }
        event.returnValue = false;
        event.cancel = true;
    });

    //Mostrar los materiales (detalle) de la categoria 
    $('#table_dis tbody').on('click', 'td.detail_row', function () {
        var t = $('#table_dis').DataTable();
        var tr = $(this).closest('tr');
        var row = t.row(tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('details');
        }
        else {
            document.getElementById("loader").style.display = "initial";//RSG 26.04.2018
            //Obtener el id de la categoría
            var index = t.row(tr).index();
            var catid = t.row(index).data()[0];
            //Obtener las fechas del row de la categoría
            var indext = getIndex();
            var vd = (3 + indext);
            var va = (4 + indext);
            var vigencia_de = tr.find("td:eq(" + vd + ") input").val();
            var vigencia_al = tr.find("td:eq(" + va + ") input").val();

            row.child(format(catid, vigencia_de, vigencia_al)).show();
            tr.addClass('details');
            document.getElementById("loader").style.display = "none";//RSG 26.04.2018
        }
    });

    $('#addRow').on('click', function () {

        var relacionada = "";

        if ($("#txt_rel").length) {
            var vrelacionada = $('#txt_rel').val();
            if (vrelacionada != "") {
                relacionada = "prelacionada";
            }
        }

        var reversa = "";
        if ($("#txt_rev").length) {
            var vreversa = $('#txt_rev').val();
            if (vreversa == "preversa") {
                reversa = vreversa;
            }
        }

        //Obtener el tipo de negociación
        var neg = $("#select_neg").val();

        if (neg != "") {
            //Obtener los valores que se van a utilizar
            var t = $('#table_dis').DataTable();
            //Obtener las fechas de temporalidad para agregarlas a los items
            var val_de = $('#fechai_vig').val();
            var val_al = $('#fechaf_vig').val();

            var adate = formatDate(val_al);
            var ddate = formatDate(val_de);

            adate = formatDatef(adate);
            ddate = formatDatef(ddate);

            //Obtener la distribución
            var dis = $("#select_dis").val();

            if (dis == "") {
                M.toast({ html: 'Seleccione distribución' });
                return false;
            }

            //Negociación Monto
            if (neg == "M") {
                //Distribución por categoría
                if (dis == "C") {
                    //Obtener la categoría
                    var cat = $('#select_categoria').val();

                    //Validar si la categoría ya había sido agregada
                    var catExist = valcategoria(cat);

                    if (catExist != true) {
                        if (cat != "") {
                            ////Obtener el monto
                            //var montoDistribucion = $('#monto_dis').val();
                            //var mto = parseFloat(montoDistribucion);
                            //////Validar que este un monto
                            ////if (mto > 0) {


                            //    //Obtener el numero de renglones de la tabla
                            //    var lengthTable = $("table#table_dis tbody tr[role='row']").length;

                            //    var valPor = "";
                            //    var valCant = "";
                            //    if (lengthTable < 1) {
                            //        valPor = "100";
                            //        valCant = montoDistribucion;
                            //    }

                            var opt = $("#select_categoria option:selected").text();

                            var addedRow = addRowCat(t, cat, ddate, adate, opt, "", relacionada, reversa, "", "");

                            //t.row.add([
                            //    cat + "", //col0
                            //    "", //col1
                            //    "", ////col2
                            //    "<input class=\"" + relacionada + " input_oper format_date input_fe\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + ddate + "\">", //col3
                            //    "<input class=\"" + relacionada + " input_oper format_date input_fe\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + adate + "\">" + pickerFecha(".format_date"),// RSG 21.05.2018
                            //    "", //Material
                            //    opt + "",
                            //    opt + "",
                            //    //"<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                            //    //"<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                            //    //"<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                            //    //"",
                            //    //"<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                            //    //"<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                            //    //"",
                            //    "",
                            //    "", //+ valPor,
                            //    "",
                            //    "",
                            //    "",
                            //    "",
                            //    //"" + valCant,
                            //    "<input class=\"" + reversa + " input_oper numberd input_dc total\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                            //]).draw(false);


                            //} else {
                            //    M.toast({ html: 'Debe de capturar un monto' });
                            //}
                        } else {
                            M.toast({ html: 'Seleccione una categoría' });
                        }
                    } else {
                        M.toast({ html: 'La categoría ya había sido agregada' });
                    }

                } else if (dis == "M") {
                    //Distribución por material                     

                    var addedRow = addRowMat(t, "", "", "", "", "", "", "", "", "", "", "", relacionada, reversa, ddate, adate, "");
                    //t.row.add([
                    //    "",
                    //    "",
                    //    "",
                    //    "<input class=\"" + relacionada + " input_oper format_date input_fe\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //    "<input class=\"" + relacionada + " input_oper format_date input_fe\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //    "<input class=\"" + relacionada + " input_oper input_material number\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //    "",
                    //    "",
                    //    "<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //    "<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //    "<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //    "",
                    //    "<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //    "<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //    "<input class=\"" + reversa + " input_oper numberd input_dc total\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
                    //]).draw(false);

                    $('#table_dis').css("font-size", "12px");
                    $('#table_dis').css("display", "table");
                    //$('#tfoot_dis').css("display", "table-footer-group");

                    //if ($('#select_dis').val() == "M") {

                    t.column(0).visible(false);
                    t.column(1).visible(false);
                    //}
                }
                updateFooter();


            } else if (neg == "P") {
                //Negociación porcentaje
                //Obtener el porcentaje de apoyo base
                var p_apoyo = $('#bmonto_apoyo').val() + "";
                p_apoyo = parseFloat(p_apoyo);//RSG 29.06.2018
                //| 0.00;//RSG 29.06.2018
                if (p_apoyo == undefined)//RSG 29.06.2018
                    p_apoyo = 0;//RSG 29.06.2018

                var m_apoyo = $('#monto_dis').val() + "";
                m_apoyo = parseFloat(m_apoyo) | 0.00;


                //if (p_apoyo > 0) {
                //Distribución por categoría
                if (dis == "C") {
                    if (m_apoyo > 0) {
                        //Obtener la categoría
                        var cat = $('#select_categoria').val();

                        //Validar si la categoría ya había sido agregada
                        var catExist = valcategoria(cat);
                        if (catExist != true) {
                            if (cat != "") {
                                var opt = $("#select_categoria option:selected").text();
                                porcentaje_cat = "<input class=\"" + reversa + " input_oper numberd porc_cat pc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">";
                                var addedRow = addRowCat(t, cat, ddate, adate, opt, "", relacionada, reversa, porcentaje_cat, "pc");
                                $(".pc").prop('disabled', true);
                                $('.pc').trigger('click');
                                //Actualizar la tabla con los porcentajes
                                updateTableCat();
                            } else {
                                M.toast({ html: 'Seleccione una categoría' });

                            }
                        } else {
                            M.toast({ html: 'La categoría ya había sido agregada' });
                        }
                    } else {
                        M.toast({ html: 'El monto base debe de ser mayor a cero' });
                    }
                } else if (dis == "M") {
                    //Distribución por material  
                    var por_apoyo = "";
                    por_apoyo = p_apoyo;
                    if (por_apoyo > 0) {
                        var addedRow = addRowMat(t, "", "", "", "", "", por_apoyo, "", "", "", "", "", relacionada, reversa, ddate, adate, "", "pm");
                        //Si el porcentaje de apoyo es mayor a cero bloquear la columna de porcentaje de apoyo
                        //Eliminar los renglones que no contienen el mismo porcentaje
                        $(".pm").prop('disabled', true);
                        $('.pm').trigger('click');
                        //eliminarRowsDistribucion(por_apoyo);
                    } else {
                        //Si el porcentage es 0 desbloquear la columna de porcentaje de apoyo
                        M.toast({ html: 'Porcentaje de apoyo base debe de ser mayor a cero' });
                        return false;
                    }


                    //Inhabilitar la modificación del total



                    $('#table_dis').css("font-size", "12px");
                    $('#table_dis').css("display", "table");

                    t.column(0).visible(false);
                    t.column(1).visible(false);
                }
                updateFooter();
                //} else {
                //    M.toast({ html: 'Porcentaje de apoyo base debe de ser mayor a cero' });
                //}
            }
        } else {
            M.toast({ html: 'Seleccione negociación' });
        }

        event.returnValue = false;
        event.cancel = true;

    });

    $('#select_neg').change();
    $('#select_dis').change();

    //Archivo para tabla de distribución
    $("#file_dis").change(function () {
        var filenum = $('#file_dis').get(0).files.length;
        if (filenum > 0) {
            var file = document.getElementById("file_dis").files[0];
            var filename = file.name;
            if (evaluarExt(filename)) {
                M.toast({ html: 'Cargando ' + filename });
                loadExcelDis(file);
                updateFooter();
            } else {
                M.toast({ html: 'Tipo de archivo incorrecto: ' + filename });
            }
        } else {
            M.toast({ html: 'Seleccione un archivo' });
        }
    });

    $('#check_factura').change(function () {
        var table = $('#table_sop').DataTable();
        table.clear().draw(true);
        if ($(this).is(":checked")) {
            $(".table_sop").css("display", "table");
            $("#file_facturat").css("display", "none");
            //Add row 
            addRowSop(table);
            //Hide columns
            ocultarColumnasTablaSoporteDatos();
        } else {
            $(".table_sop").css("display", "none");
            $("#file_facturat").css("display", "block");
        }

        $('.file_sop').val('');
    });

    $('#file_sop').on('click touchstart', function () {
        $('.file_sop').val('');
    });

    //Archivo para facturas en soporte ahora información
    $("#file_sop").change(function () {
        var filenum = $('#file_sop').get(0).files.length;
        if (filenum > 0) {
            var file = document.getElementById("file_sop").files[0];
            var filename = file.name;
            if (evaluarExt(filename)) {
                M.toast({ html: 'Cargando ' + filename });
                loadExcelSop(file);
            } else {
                M.toast({ html: 'Tipo de archivo incorrecto: ' + filename });
            }
        } else {
            M.toast({ html: 'Seleccione un archivo' });
        }
    });


    //Temporalidad
    if ($('#monto_doc_md').val() != "") {
        $("label[for='monto_doc_md']").addClass("active");
    }

    $('#tabs').tabs();

    var elem = document.querySelectorAll('select');
    var instance = M.Select.init(elem, []);

    $('#tab_temp').on("click", function (e) {
        //$('#gall_id').change();
        evalInfoTab(false, e);
    });

    $('#tab_soporte').on("click", function (e) {

        evalTempTab(false, e);

    });

    $('#tab_dis').on("click", function (e) {
        var sol = $("#tsol_id").val();
        var mostrar = isFactura(sol);

        //if (sol == "NC" | sol == "NCI" | sol == "OP") {
        if (mostrar) {
            $('#lbl_volumen').html("Volumen real");
            $('#lbl_apoyo').html("Apoyo real");
        } else {
            $('#lbl_volumen').html("Volumen estimado");
            $('#lbl_apoyo').html("Apoyo estimado");
        }

        var res = evalSoporteTab(true, e);
        if (res) {
            var restemp = evalTempTab(true, e);
            if (restemp) {
                resinfo = evalInfoTab(true, e);
                if (!resinfo) {
                    msg = 'Verificar valores en los campos de Información!';
                    M.toast({ html: msg });
                    e.preventDefault();
                    e.stopPropagation();
                    var ell = document.getElementById("tabs");
                    var instances = M.Tabs.getInstance(ell);
                    instances.select('Informacion_cont');
                }
            } else {
                msg = 'Verificar valores en los campos de Temporalidad!';
                M.toast({ html: msg });
                e.preventDefault();
                e.stopPropagation();
                var ell = document.getElementById("tabs");
                var instances = M.Tabs.getInstance(ell);
                instances.select('Temporalidad_cont');
            }
        } else {
            msg = 'Verificar valores en los campos de Soporte!';
            M.toast({ html: msg });
            e.preventDefault();
            e.stopPropagation();
            var ell = document.getElementById("tabs");
            var instances = M.Tabs.getInstance(ell);
            instances.select('Soporte_cont');
        }

        updateFooter();
    });

    $('#tab_fin').on("click", function (e) {

        var res = evalDistribucionTab(true, e);
        if (res) {

            //Activar el botón de guardar
            $("#btn_guardarh").removeClass("disabled");

            //Copiar el monto de distribución de la tabla footer al monto financiera
            var total_dis = $('#total_dis').text();
            var basei = convertI(total_dis);

            //Obtiene el id del tipo de negociación, default envía vacío
            var select_neg = $('#select_neg').val();
            //Validar el monto base vs monto tabla
            if (select_neg == "M") {
                //Tiene que tener una moneda
                //Obtener la moneda de distribución y de financiera
                var monedadis_id = $('#monedadis_id').val();
                var monedafin_id = $('#moneda_id').val();

                //Si las monedas son iguales, se pasa el monto
                if (monedadis_id == monedafin_id) {
                    $('#monto_doc_md').val(basei);

                } else {
                    //Realizar conversión de monedas
                    var newMonto = cambioCurr(monedadis_id, monedafin_id, basei);
                    $('#monto_doc_md').val(newMonto);

                }

            } else if (select_neg == "P") {
                //Si no es por monto solo se copia la cantidad

                $('#monto_doc_md').val(basei);

            }

            //Emular un focus out para actualizar los campos
            $('#monto_doc_md').focusout();

            $("label[for='monto_doc_md']").addClass("active");

            //Obtener los valores para asignar persupuesto
            //Obtener canal desc
            var canal = $('#vtweg').val();
            var canal = canal.split('-');
            canal[1] = $.trim(canal[1]);
            $('#p_vtweg').text(canal[1]);
            //Obtener cliente id
            var kunnr = $('#payer_id').val();
            //$('#cli_name').val();
            $('#p_kunnr').text(kunnr);


            var num = $('#txt_rel').val();//RSG 12.06.2018
            var num2 = $('#monto_doc_md').val();//RSG 12.06.2018

            asignarPresupuesto(kunnr);
            asignarSolicitud(num, num2);//RSG 12.06.2018

        } else {
            M.toast({ html: 'Verificar valores en los campos de Distribución!' });
            e.preventDefault();
            e.stopPropagation();
            //var active = $('ul.tabs .active').attr('href');
            //$('ul.tabs').tabs('select_tab', active);
            var ell = document.getElementById("tabs");
            var instances = M.Tabs.getInstance(ell);
            instances.select('Distribucion_cont');
        }

        formaClearing();//RSG 18.06.2018
    });

    //Financiera   
    $('#monto_doc_md').focusout(function (e) {

        var monto_doc_md = $('#monto_doc_md').val();
        var is_num = $.isNumeric(monto_doc_md);
        var mt = parseFloat(monto_doc_md.replace(',', '')).toFixed(2);
        if (mt > 0 & is_num == true) {
            //Obtener la moneda en la lista
            //var MONEDA_ID = $('#moneda_id').val();
            $('#monto_doc_md').val(mt);

            //selectTcambio(MONEDA_ID, mt);
            var tipo_cambio = $('#tipo_cambio').val();
            var tc = parseFloat(tipo_cambio.replace(',', '')).toFixed(2);
            //Validar el monto en tipo de cambio
            var is_num2 = $.isNumeric(tipo_cambio);
            if (tc > 0 & is_num2 == true) {
                $('#tipo_cambio').val(tc);
                var monto = mt / tc;
                monto = parseFloat(monto).toFixed(2);
                $('#monto_doc_ml2').val(monto);
                $('#montos_doc_ml2').val(monto);
                $("label[for='montos_doc_ml2']").addClass("active");
            } else {
                $('#monto_doc_ml2').val(monto);
                $('#montos_doc_ml2').val(monto);
                $("label[for='montos_doc_ml2']").addClass("active");
                var msg = 'Tipo de cambio incorrecto';
                M.toast({ html: msg });
                e.preventDefault();
            }

        } else {
            $('#monto_doc_ml2').val(monto_doc_md);
            $('#montos_doc_ml2').val(monto_doc_md);
            $("label[for='montos_doc_ml2']").addClass("active");
            var msg = 'Monto incorrecto';
            M.toast({ html: msg });
            e.preventDefault();
        }

    });
    $('#tipo_cambio').focusout(function (e) {
        var tipo_cambio = $('#tipo_cambio').val();
        var is_num = $.isNumeric(tipo_cambio);
        var tc = parseFloat(tipo_cambio.replace(',', '')).toFixed(2);
        //Validar el monto en tipo de cambio
        if (tc > 0 & is_num == true) {
            //Validar el monto
            $('#tipo_cambio').val(tc)
            var monto_doc_md = $('#monto_doc_md').val();
            var mt = parseFloat(monto_doc_md.replace(',', '')).toFixed(2);
            var is_num2 = $.isNumeric(monto_doc_md);
            if (mt > 0 & is_num2 == true) {
                $('#monto_doc_md').val(mt);

                //Validar la moneda                    
                var moneda_id = $('#moneda_id').val();
                if (moneda_id != null && moneda_id != "") {
                    $('#monto_doc_ml2').val();
                    $('#montos_doc_ml2').val();

                    //Los valores son correctos, proceso para generar nuevo monto
                    var monto = mt / tc;
                    monto = parseFloat(monto).toFixed(2);
                    $('#monto_doc_ml2').val(monto);
                    $('#montos_doc_ml2').val(monto);
                    $("label[for='montos_doc_ml2']").addClass("active");

                } else {
                    $('#monto_doc_md').val();

                    $('#monto_doc_ml2').val(monto);
                    $('#montos_doc_ml2').val(monto);
                    var msg = 'Moneda incorrecta';
                    M.toast({ html: msg })
                }

            } else {
                $('#monto_doc_md').val();

                $('#tipo_cambio').val("");
                $('#monto_doc_ml2').val(monto);
                $('#montos_doc_ml2').val(monto);
                $("label[for='montos_doc_ml2']").addClass("active");
                var msg = 'Monto incorrecto';
                M.toast({ html: msg });
                e.preventDefault();
            }

        } else {
            $('#monto_doc_ml2').val(monto);
            $('#montos_doc_ml2').val(monto);
            $("label[for='montos_doc_ml2']").addClass("active");
            var msg = 'Tipo de cambio incorrecto';
            M.toast({ html: msg });
            e.preventDefault();
        }
    });

    var monto_doc_md = $('#monto_doc_md').val();
    var is_num = $.isNumeric(monto_doc_md);
    var mt = parseFloat(monto_doc_md.replace(',', '')).toFixed(2);
    if (mt > 0 & is_num == true) {
        //Obtener la moneda en la lista
        //var MONEDA_ID = $('#moneda_id').val();
        $('#monto_doc_md').val(mt);

        //selectTcambio(MONEDA_ID, mt);
        var tipo_cambio = $('#tipo_cambio').val();
        var tc = parseFloat(tipo_cambio.replace(',', '')).toFixed(2);
        //Validar el monto en tipo de cambio
        var is_num2 = $.isNumeric(tipo_cambio);
        if (tc > 0 & is_num2 == true) {
            $('#tipo_cambio').val(tc);
            var monto = mt / tc;
            monto = parseFloat(monto).toFixed(2);
            $('#monto_doc_ml2').val(monto);
            $('#montos_doc_ml2').val(monto);
            $("label[for='montos_doc_ml2']").addClass("active");
        } else {
            $('#monto_doc_ml2').val(monto);
            $('#montos_doc_ml2').val(monto);
            $("label[for='montos_doc_ml2']").addClass("active");
        }

    } else {
        $('#monto_doc_ml2').val(monto_doc_md);
        $('#montos_doc_ml2').val(monto_doc_md);
        $("label[for='montos_doc_ml2']").addClass("active");
    }




    $('#btn_getmat').on("click", function (e) {

        formatCat();
    });


    $('#btn_guardarh').on("click", function (e) {
        document.getElementById("loader").style.display = "initial";//RSG 26.04.2018
        var msg = 'Verificar valores en los campos de ';
        var res = true;
        //Evaluar TabInfo values
        var InfoTab = evalInfoTab(true, e);
        if (!InfoTab) {
            msg += 'Información';
            res = InfoTab;
        }
        //Evaluar TempTab values
        var TempTab = evalTempTab(true, e);
        if (!TempTab) {
            msg += ' ,Temporalidad';
            res = TempTab;
        }
        //Evaluar SoporteTab values
        var SoporteTab = evalSoporteTab(true, e);
        if (!SoporteTab) {
            msg += ' ,Soporte';
            res = SoporteTab;
        }

        //Evaluar SoporteTab values
        var FinancieraTab = evalFinancieraTab(true, e);
        if (!FinancieraTab) {
            msg += ' ,Financiera';
            res = FinancieraTab;
        }

        msg += '!';
        if (res) {
            //loadFilesf();
            //Provisional
            var tipo_cambio = $('#tipo_cambio').val();
            //var iNum = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
            var iNum = parseFloat(tipo_cambio.replace(',', ''));

            if (iNum > 0) {
                //var num = "" + iNum;
                //num = num.replace('.', ',');
                //var numexp = num;//* 60000000000;
                //$('#tipo_cambio').val(numexp);
            } else {
                $('#tipo_cambio').val(0);
            }
            var tipo_cambio = $('#monto_doc_ml2').val();
            //var iNum2 = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
            var iNum2 = parseFloat(tipo_cambio.replace(',', ''));
            //var iNum2 = parseFloat(tipo_cambio.replace('.', ','));
            if (iNum2 > 0) {
                //var nums = "" + iNum2;
                //nums = nums.replace('.', ',');
                //var numexp2 = nums;// * 60000000000;
                //$('#monto_doc_ml2').val(numexp2);
            } else {
                $('#monto_doc_ml2').val(0);
            }

            //Monto
            var monto = $('#monto_doc_md').val();
            //var numm = parseFloat(monto.replace(',', '.')).toFixed(2);   
            var numm = parseFloat(monto.replace(',', ''));
            if (numm > 0) {
                $('#MONTO_DOC_MD').val(numm);
            } else {
                $('#MONTO_DOC_MD').val(0);
                $('#monto_doc_md').val(0);
            }

            $('#select_negi').prop('disabled', false); //B20180618 v1 MGC 2018.06.18
            $('#select_disi').prop('disabled', false); //B20180618 v1 MGC 2018.06.18

            //Guardar los valores de la tabla en el modelo para enviarlos al controlador
            copiarTableControl();//Distribución
            copiarSopTableControl(); //Soporte ahora en información
            enviaRec();//RSG 28.05.2018
            //Termina provisional
            $('#btn_guardar').click();
        } else {
            M.toast({ html: msg })
            document.getElementById("loader").style.display = "none";//RSG 26.04.2018
        }

    });

    $('#btn_guardarr').on("click", function (e) {
        document.getElementById("loader").style.display = "initial";//RSG 26.04.2018
        var msg = 'Verificar valores en los campos de ';
        var res = true;
        //Evaluar TabInfo values
        var InfoTab = evalInfoTab(true, e);
        if (!InfoTab) {
            msg += 'Información';
            res = InfoTab;
        }
        //Evaluar TempTab values
        var TempTab = evalTempTab(true, e);
        if (!TempTab) {
            msg += ' ,Temporalidad';
            res = TempTab;
        }
        //Evaluar SoporteTab values
        var SoporteTab = evalSoporteTab(true, e);
        if (!SoporteTab) {
            msg += ' ,Soporte';
            res = SoporteTab;
        }

        //Evaluar SoporteTab values
        var FinancieraTab = evalFinancieraTab(true, e);
        if (!FinancieraTab) {
            msg += ' ,Financiera';
            res = FinancieraTab;
        }

        msg += '!';
        if (res) {
            //loadFilesf();
            //Provisional
            var tipo_cambio = $('#tipo_cambio').val();
            //var iNum = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
            var iNum = parseFloat(tipo_cambio.replace(',', ''));

            if (iNum > 0) {
                //var num = "" + iNum;
                //num = num.replace('.', ',');
                //var numexp = num;//* 60000000000;
                //$('#tipo_cambio').val(numexp);
            } else {
                $('#tipo_cambio').val(0);
            }
            var tipo_cambio = $('#monto_doc_ml2').val();
            //var iNum2 = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
            var iNum2 = parseFloat(tipo_cambio.replace(',', ''));
            //var iNum2 = parseFloat(tipo_cambio.replace('.', ','));
            if (iNum2 > 0) {
                //var nums = "" + iNum2;
                //nums = nums.replace('.', ',');
                //var numexp2 = nums;// * 60000000000;
                //$('#monto_doc_ml2').val(numexp2);
            } else {
                $('#monto_doc_ml2').val(0);
            }

            //Monto
            var monto = $('#monto_doc_md').val();
            //var numm = parseFloat(monto.replace(',', '.')).toFixed(2);   
            var numm = parseFloat(monto.replace(',', ''));
            if (numm > 0) {
                $('#MONTO_DOC_MD').val(numm);
            } else {
                $('#MONTO_DOC_MD').val(0);
                $('#monto_doc_md').val(0);
            }
            //Guardar los valores de la tabla en el modelo para enviarlos al controlador
            copiarTableControl();//Distribución
            copiarSopTableControl(); //Soporte ahora en información
            //Termina provisional
            $('#btn_guardar').click();
        } else {
            M.toast({ html: msg })
            document.getElementById("loader").style.display = "none";//RSG 26.04.2018
        }

    });
});

//Cuando se termina de cargar la página
$(window).on('load', function () {

    //MGC B20180611 Obtener los materiales por categoría en la relacionada
    var jsoncat = $('#catmat').val();
    try {
        var jsvalcat = $.parseJSON(jsoncat);
        var materiales = JSON.stringify(jsvalcat);
        $('#catmat').val("");
        $('#catmat').val(materiales);

    } catch (error) {
        $('#catmat').val("");
    }

    var monto = $('#MONTO_DOC_MD').val();
    monto = parseFloat(monto);
    //Encriptar valores del json para el tipo de solicitud
    var tsol_valn = $('#TSOL_VALUES').val();
    try {
        var jsval = $.parseJSON(tsol_valn);
        var docsenviar = JSON.stringify(jsval);
        $('#TSOL_VALUES').val(docsenviar);
    } catch (error) {
        $('#TSOL_VALUES').val("");
    }

    //Obtener los valores de los combos de negociación y distribución
    var sneg = $('#select_negi').val();
    var sdis = $('#select_disi').val();

    if (sneg != "") {

        //$("#select_neg").val(sneg);
        //$("#select_neg").trigger('onchange');
        $('#select_neg').val(sneg).change();
        var elemdpsn = document.querySelector('#select_neg');
        var optionsdpsn = [];
        var instancessn = M.Select.init(elemdpsn, optionsdpsn);
        //$('#select_neg').formSelect();
    }
    if (sdis != "") {
        //$("#select_dis").val();
        //$("#select_dis").trigger('onchange');
        $('#select_dis').val(sdis).change();
        var elemdpsd = document.querySelector('#select_dis');
        var optionsdpsd = [];
        var instancessd = M.Select.init(elemdpsd, optionsdpsd);
        //$('#select_dis').formSelect();
    }

    //una factura
    var check = $("#check_facturas").val();
    if (check == "true") {
        $('#check_factura').prop('checked', true);
    } else {
        $('#check_factura').prop('checked', false);
    }


    $('#gall_id').change(); //Cambio en allowance
    if ($('#gall_idt').hasClass("prelacionada")) {
        selectTall($('#gall_idt').val());
    }

    $('#payer_id').change(); //Cambiar datos del cliente
    //Fechas de temporalidad
    var fechai_vig = $('#fechai_vig').val();
    var fechaf_vig = $('#fechaf_vig').val();

    var fi = fechai_vig.split(' ');
    var ff = fechaf_vig.split(' ');

    if (fi[0] != "") {
        $('#fechai_vig').val($.trim(fi[0]));
    }

    if (ff[0] != "") {
        $('#fechaf_vig').val($.trim(ff[0]));
    }
    if (sneg == "P" && sdis == "C") {
        var m = monto + "";
        $('#monto_dis').val(m);
    }

    //Valores en información antes soporte
    copiarTableVistaSop();
    //Valores en  distribución    
    copiarTableVista("");

    updateFooter();
    //Pasar el total de la tabla al total en monto
    var total_dis = $('#total_dis').text();
    var footeri = convertI(total_dis);
    if (sneg != "P" && sdis != "C") {
        $('#monto_dis').val(footeri);
    }
    $("label[for='monto_dis']").addClass("active");

    //Validar si es una solicitud relacionada
    //Activar bloqueos
    //if ($("#txt_rel").length) {
    //    var relacionada = $('#txt_rel').val();
    //    if (relacionada != "") {

    //    }
    //}

    $(".prelacionada").prop('disabled', true);
    $('.prelacionada').trigger('click');

    $(".preversa").prop('disabled', true);
    $('.preversa').trigger('click');

    //MGC B20180611
    if (isRelacionada()) {
        $('#select_neg').prop('disabled', 'disabled');
        var elemdpsn = document.querySelector('#select_neg');
        var optionsdpsn = [];
        var instancessn = M.Select.init(elemdpsn, optionsdpsn);
        $('#select_dis').prop('disabled', 'disabled');
        var elemdpsd = document.querySelector('#select_dis');
        var optionsdpsd = [];
        var instancessd = M.Select.init(elemdpsd, optionsdpsd);
        $('#select_categoria').prop('disabled', 'disabled');
        var elemdpc = document.querySelector('#select_categoria');
        var optionsdpc = [];
        var instancesc = M.Select.init(elemdpc, optionsdpc);
    }

    //MGC B20180611
    if (isReversa()) {
        $('#select_neg').prop('disabled', 'disabled');
        var elemdpsn = document.querySelector('#select_neg');
        var optionsdpsn = [];
        var instancessn = M.Select.init(elemdpsn, optionsdpsn);
        $('#select_dis').prop('disabled', 'disabled');
        var elemdpsd = document.querySelector('#select_dis');
        var optionsdpsd = [];
        var instancessd = M.Select.init(elemdpsd, optionsdpsd);
        $('#select_categoria').prop('disabled', 'disabled');
        var elemdpc = document.querySelector('#select_categoria');
        var optionsdpc = [];
        var instancesc = M.Select.init(elemdpc, optionsdpc);
    }

});

function formatDate(val) {
    var vdate = "";
    try {
        vdate = val.split('/');
        //vdate = new Date(vdate[2] + "-" + vdate[1] + "-" + vdate[0]);
        vdate = new Date(vdate[2], vdate[1] - 1, vdate[0]);
    }
    catch (err) {
        vdate = "";
    }



    return vdate;
}

function formatDatef(vdate) {

    var dd = "";
    var mm = "";
    var yy = "";
    var de = true;
    var d = "";

    try {
        dd = vdate.getDate();
    }
    catch (err) {
        de = false;
    }

    try {
        mm = (vdate.getMonth() + 1);
    }
    catch (err) {
        de = false;
    }

    try {
        yy = vdate.getFullYear();
    }
    catch (err) {
        de = false;
    }

    if (de == true) {
        d = dd + "/" + mm + "/" + yy;
    } else {
        d = "";
    }

    return d;
}

function copiarTableVista(update) {

    var lengthT = $("table#table_dish tbody tr").length;

    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla de la vista
        //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
        $('#table_dis').css("font-size", "12px");
        $('#table_dis').css("display", "table");
        var tsol = "";
        var sol = "";
        //Obtener el tipo de solución a partir de la anterior
        if ($("#tsolant_id").length) {
            sol = $('#tsolant_id').val();
            //sol = $("#tsol_id").val();
        } else {
            sol = $("#tsol_id").val();
        }

        var dis = $("#select_dis").val();

        //Obtener el tipo de negociación
        var neg = $("#select_neg").val();
        var monto_apoyo = 0;
        var pm = "";
        if (neg == "P") {
            monto_apoyo = $("#bmonto_apoyo").val();
            monto_apoyo = parseFloat(monto_apoyo);
            pm = "pm";
        }

        var mostrar = isFactura(sol);
        //if (sol == "NC" | sol == "NCI" | sol == "OP") {
        if (mostrar) {
            tsol = "real";
        } else {
            tsol = "estimado";
        }
        var i = 1;
        $('#table_dish > tbody  > tr').each(function () {

            var vigencia_de = $(this).find("td:eq(" + 1 + ") input").val();
            var vigencia_al = $(this).find("td:eq(" + 2 + ") input").val();

            var ddate = vigencia_de.split(' ');
            var adate = vigencia_al.split(' ');

            var matnr = $(this).find("td:eq(" + 3 + ") input").val();
            var matkl = $(this).find("td:eq(" + 4 + ") input").val();
            var matkl_id = $(this).find("td:eq(" + 5 + ") input").val();
            var costo_unitario = $(this).find("td:eq(" + 6 + ") input").val();
            var porc_apoyo = $(this).find("td:eq(" + 7 + ") input").val();
            var monto_apoyo = $(this).find("td:eq(" + 8 + ") input").val();
            var precio_sug = $(this).find("td:eq(" + 9 + ") input").val();
            var volumen_est = $(this).find("td:eq(" + 10 + ") input").val();
            var volumen_real = $(this).find("td:eq(" + 11 + ") input").val();

            var apoyo_est = $(this).find("td:eq(" + 12 + ") input").val();
            var apoyo_real = $(this).find("td:eq(" + 13 + ") input").val();

            var vol = 0;
            var total = 0;
            if (tsol == "estimado") {
                vol = volumen_est;
                total = apoyo_est;
            } else {
                vol = volumen_real;
                total = apoyo_real;
            }


            var t = $('#table_dis').DataTable();

            var relacionada = "";

            ////if ($("#txt_rel").length) {//MGC B20180611
            ////    var vrelacionada = $('#txt_rel').val();
            ////    if (vrelacionada != "") {
            ////        relacionada = "prelacionada";
            ////    }
            ////}
            //MGC B20180611
            //if ($("#txt_rel").length) {
            if (isRelacionada()) {
                var vrelacionada = $('#txt_rel').val();
                //if (vrelacionada != "") {
                relacionada = "prelacionada";
                //}
            }

            var reversa = "";
            ////if ($("#txt_rev").length) {//MGC B20180611
            ////    var vreversa = $('#txt_rev').val();
            ////    if (vreversa == "preversa") {
            ////        reversa = vreversa;
            ////    }
            ////}
            //MGC B20180611
            //if ($("#txt_rev").length) {
            if (isReversa()) {
                var vreversa = $('#txt_rev').val();
                //if (vreversa == "preversa") {
                reversa = vreversa;
                //}
            }

            var calculo = "";
            //Definir si los valores van en 0 y nada más poner el total
            if (costo_unitario == "" || costo_unitario == "0.00" || porc_apoyo == "" || porc_apoyo == "0.00") {
                //Se mostrara nada más el total
                calculo = "sc";
            }
            var addedRow = "";
            //Si la distribución es por material
            if (dis == "M") {
                addedRow = addRowMat(t, matkl_id, matnr, matkl, matkl, costo_unitario, porc_apoyo, monto_apoyo, "", precio_sug, vol, total, relacionada, reversa, $.trim(ddate[0]), $.trim(adate[0]),
                    calculo, pm);



                //t.row.add([
                //    matkl_id + "", //col0 ID
                //    "", //col1
                //    "", ////col2
                //    "<input class=\"" + relacionada + " input_oper format_date\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + $.trim(ddate[0]) + "\">", //col3
                //    "<input class=\"" + relacionada + " input_oper format_date\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + $.trim(adate[0]) + "\">",
                //    "<input class=\"" + relacionada + " input_oper input_material\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + matnr + "\">", //Material
                //    matkl + "",
                //    matkl + "",
                //    "<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + costo_unitario + "\">",
                //    "<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + porc_apoyo + "\">",
                //    "<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + monto_apoyo + "\">",
                //    "",
                //    "<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + precio_sug + "\">",
                //    "<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + vol + "\">",
                //    "<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + total + "\">",
                //]).draw(false);
            } else if (dis == "C") {
                //Si la distribución es por categoría
                //Obtener el tipo de negociación
                var neg = $("#select_neg").val();
                if (neg == "M") {
                    addedRow = addRowCat(t, matkl_id, $.trim(ddate[0]), $.trim(adate[0]), matkl, total, relacionada, reversa, "", "");
                } else if (neg == "P") {
                    var porcentaje_cat = "<input class=\"" + reversa + " input_oper numberd porc_cat pc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">";
                    addedRow = addRowCat(t, matkl_id, $.trim(ddate[0]), $.trim(adate[0]), matkl, "", relacionada, reversa, porcentaje_cat, "pc");
                    $(".pc").prop('disabled', true);
                    $('.pc').trigger('click');
                    //Actualizar la tabla con los porcentajes
                    updateTableCat();
                }
            }
            //Quitar el row
            $(this).remove();

            if (calculo != "") {
                //    updateTotalRow(t, addedRow, this, "X", total);
                $(addedRow).addClass("" + calculo);
            }

        });

        if (update == "X") {
            $('.input_oper').trigger('focusout');
        } else {

            if (dis == "M") {
                //Actualizar campos y renglones
                var t = $('#table_dis').DataTable();
                var indext = getIndex();
                $("#table_dis > tbody  > tr[role='row']").each(function () {

                    //Validar el material
                    var mat = $(this).find("td:eq(" + (5 + indext) + ") input").val();
                    var val = valMaterial(mat, "X");

                    if (val.ID == null || val.ID == "") {
                        $(this).find('td').eq((5 + indext)).addClass("errorMaterial");
                        //} else if (val.ID == mat) {//RSG 07.06.2018
                    } else if (trimStart('0', val.ID) == mat) {//RSG 07.06.2018

                        selectMaterial(val.ID, val.MAKTX, $(this));

                    } else {
                        $(this).find('td').eq((5 + indext)).addClass("errorMaterial");
                    }

                    if ($(this).hasClass("sc")) {
                        //var total = $(this).find("td:eq(" + (14 + indext) + ") input").val();//RSG 06.06.2018
                        var total = $(this).find("td.total input").val();//RSG 06.06.2018
                        updateTotalRow(t, $(this), "", "X", total);
                        $(this).removeClass("sc");
                    }

                });
            } else if (dis == "C") {
                //Actualizar los campos y renglones de la categoría
                var t = $('#table_dis').DataTable();
                var indext = getIndex();
                $("#table_dis > tbody  > tr[role='row']").each(function () {

                    //Validar la categoría
                    var cat = $(this).find("td:eq(" + (6 + indext) + ")").text();
                    var val = getCategoriaDesc(cat);

                    if (val.CATEGORIA_ID == cat) {

                        $(this).find("td:eq(" + (6 + indext) + ")").text(val.TXT50);
                        $(this).find("td:eq(" + (7 + indext) + ")").text(val.TXT50);
                        $(this).removeClass("sc");
                    }
                });
            }
        }

        //$('.input_oper').trigger('focusout');
        //$('.input_oper').trigger('focusout');
        if (pm == "pm") {
            $(".pm").prop('disabled', true);
            $('.pm').trigger('click');
        }
    }

}

function copiarTableVistaSop() {

    var lengthT = $("table#table_soph tbody tr").length;

    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla de la vista en información
        //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
        $('#check_factura').trigger('change');
        $(".table_sop").css("display", "table");
        var rowsn = 0;
        if ($("#check_factura").is(':checked')) {
            //Tabla con inputs
            rowsn = 1;
        } else {
            //Tabla desde excel
            rowsn = lengthT;
        }


        var tsol = "";
        var sol = $("#tsol_id").val();

        var i = 1;
        $('#table_soph > tbody  > tr').each(function () {

            //var pos = $(this).find("td.POS").text();
            var pos = $(this).find("td:eq(0)").text();
            //var factura = $(this).find("td.FACTURA").text();
            var factura = $(this).find("td:eq(1)").text();
            //var fecha = $(this).find("td.FECHA").text();
            var fecha = $(this).find("td:eq(2)").text();

            var ffecha = fecha.split(' ');

            //var prov = $(this).find("td.PROVEEDOR").text();
            var prov = $(this).find("td:eq(3)").text();
            var prov_txt = "";
            //var control = $(this).find("td.CONTROL").text();
            var control = $(this).find("td:eq(5)").text();
            // var autorizacion = $(this).find("td.AUTORIZACION").text();
            var autorizacion = $(this).find("td:eq(6)").text();
            //var vencimiento = $(this).find("td.VENCIMIENTO").text();
            var vencimiento = $(this).find("td:eq(7)").text();

            var vven = vencimiento.split(' ');

            //var facturak = $(this).find("td.FACTURAK").text();
            var facturak = $(this).find("td:eq(8)").text();
            //var ejerciciok = $(this).find("td.EJERCICIOK").text();
            var ejerciciok = $(this).find("td:eq(9)").text();
            //var bill_doc = $(this).find("td.BILL_DOC").text();
            var bill_doc = $(this).find("td:eq(10)").text();
            //var belnr = $(this).find("td.BELNR").text();
            var belnr = $(this).find("td:eq(11)").text();

            if ($("#check_factura").is(':checked')) {

                factura = "<input class=\"FACTURA input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + factura + "\">";
                ffecha[0] = "<input class=\"FECHA input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + ffecha[0] + "\">";
                prov = "<input class=\"PROVEEDOR input_sop_f input_proveedor\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + prov + "\">";
                control = "<input class=\"CONTROL input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + control + "\">";
                autorizacion = "<input class=\"AUTORIZACION input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + autorizacion + "\">";
                vven[0] = "<input class=\"VENCIMIENTO input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + vven[0] + "\">";
                facturak = "<input class=\"FACTURAK input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + facturak + "\">";
                ejerciciok = "<input class=\"EJERCICIOK input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + ejerciciok + "\">";
                bill_doc = "<input class=\"BILL_DOC input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + bill_doc + "\">";
                belnr = "<input class=\"BELNR input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + belnr + "\">"

            }

            var t = $('#table_dis').DataTable();

            addRowSopl(pos, factura, ffecha[0], prov, prov_txt, control, autorizacion, vven[0], facturak, ejerciciok, bill_doc, belnr);

            //Quitar el row
            $(this).remove();
            if (i > rowsn) {

            }
        });
        //Hide columns
        ocultarColumnasTablaSoporteDatos();
        $('.input_sop_f').trigger('focusout');
    }

    var sol = $("#tsol_id").val();

    selectTsol(sol);
}

function copiarTableControl() {

    var lengthT = $("table#table_dis tbody tr[role='row']").length;

    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla oculta y agregarlos al json
        //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
        var indext = getIndex();
        jsonObjDocs = [];
        var i = 1;
        var vol = "";
        var sol = $("#tsol_id").val();
        var mostrar = isFactura(sol);
        //if (sol == "NC" | sol == "NCI" | sol == "OP") {
        if (mostrar) {
            vol = "real";
        } else {
            vol = "estimado";
        }

        $("#table_dis > tbody  > tr[role='row']").each(function () {

            //Multiplicar costo unitario % por apoyo(dividirlo entre 100)
            //Columnas 8 * 9 res 10
            //Categoría es 7 * 8 = 9  --> -1
            //Material es 6 * 7 = 8   --> -2

            var vigencia_de = $(this).find("td:eq(" + (3 + indext) + ") input").val();
            var vigencia_al = $(this).find("td:eq(" + (4 + indext) + ") input").val();

            var matnr = "";
            matnr = $(this).find("td:eq(" + (5 + indext) + ") input").val();
            var matkl = $(this).find("td:eq(" + (6 + indext) + ")").text();

            //Obtener el id de la categoría            
            var t = $('#table_dis').DataTable();
            var tr = $(this);
            var indexcat = t.row(tr).index();
            var matkl_id = t.row(indexcat).data()[0];

            var costo_unitario = $(this).find("td:eq(" + (8 + indext) + ") input").val();
            var porc_apoyo = $(this).find("td:eq(" + (9 + indext) + ") input").val();
            var monto_apoyo = $(this).find("td:eq(" + (10 + indext) + ") input").val();

            var precio_sug = $(this).find("td:eq(" + (12 + indext) + ") input").val();
            var volumen_est = $(this).find("td:eq(" + (13 + indext) + ") input").val();

            var total = $(this).find("td:eq(" + (14 + indext) + ") input").val();

            var item = {};

            item["NUM_DOC"] = 0;
            item["POS"] = i;
            item["VIGENCIA_DE"] = vigencia_de + " 12:00:00 p.m.";
            item["VIGENCIA_AL"] = vigencia_al + " 12:00:00 p.m.";
            item["MATNR"] = matnr || "";
            item["MATKL"] = matkl;
            item["MATKL_ID"] = matkl_id;
            item["CANTIDAD"] = 0; //Siempre 0
            item["MONTO"] = costo_unitario;
            item["PORC_APOYO"] = porc_apoyo;
            item["MONTO_APOYO"] = monto_apoyo;
            item["PRECIO_SUG"] = precio_sug;
            volumen_est = volumen_est || 0
            total = parseFloat(total);
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
            i++;
            item = "";

            $(this).addClass('selected');

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
                    $('#delRow').click();
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }

}

//Copiar la tabla de soporte a la de control ahora en información
function copiarSopTableControl() {

    var lengthT = $("table#table_sop tbody tr[role='row']").length;

    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla oculta y agregarlos al json

        jsonObjDocs = [];
        var i = 1;

        var check = false;
        if ($("#check_factura").is(':checked')) {
            //Tabla con inputs
            check = true;
        }//} else {
        //  //Tabla desde excel
        //}

        //Obtener la configuración de las columnas
        var sociedad = $('#sociedad_id').val();
        //Obtener el país ID
        var pais = $('#pais_id').val();
        //Obtener el tipo de solicitud
        var tsol_id = $('#tsol_id').val();
        var data = configColumnasTablaSoporte(sociedad, pais, tsol_id, "X");

        $("#table_sop > tbody  > tr[role='row']").each(function () {

            //Tabla desde excel
            var item = {};
            //if (check) {

            //Llenar los valores para meterlos en el json
            if (data !== null || data !== "") {
                //True son los visibles
                //var prov_txt = true;;
                var i;
                for (i in data) {

                    if (data.hasOwnProperty(i)) {
                        //Valores en la tabla
                        if (data[i] != null) {
                            if (data[i] == true) {

                                //Obtener el valor guardado en la tabla
                                var rowcl = 'td.' + i;
                                //Obtener los valores como textos en la celda
                                if (!check | i == "POS") {
                                    var valtd = $(this).find(rowcl).text();
                                    item[i] = valtd;
                                } else {
                                    //Obtener los valores como textos de los inputs
                                    var valtd = $(this).find(rowcl + " input").val();
                                    item[i] = valtd;
                                }


                            } else if (data[i] == false) {
                                //Valores que no están en la tabla mandarlos como 0
                                item[i] = 0;
                            } else {
                                item[i] = "";
                            }
                        }
                    }
                }
            }

            //}

            jsonObjDocs.push(item);
            item = "";
            $(this).addClass('selected');

        });


        docsenviar = JSON.stringify({ 'docs': jsonObjDocs });

        $.ajax({
            type: "POST",
            url: 'getPartialSop',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {

                if (data !== null || data !== "") {
                    var t = $('#table_sop').DataTable();
                    t.rows('.selected').remove().draw(false);
                    $("table#table_soph tbody").append(data);

                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }

}

function asignarPresupuesto(kunnr) {

    $.ajax({
        type: "POST",
        url: 'getPresupuesto',
        dataType: "json",
        data: { "kunnr": kunnr },

        success: function (data) {

            if (data !== null || data !== "") {
                //$('#p_canal').text(data.P_CANAL);
                //$('#p_banner').text(data.P_BANNER);
                //$('#pc_c').text(data.PC_C);
                //$('#pc_a').text(data.PC_A);
                //$('#pc_p').text(data.PC_P);
                //$('#pc_t').text(data.PC_T);
                //RSG 26.04.2018----------------
                $('#p_canal').text('$' + ((data.P_CANAL / 1).toFixed(2)));
                $('#p_banner').text('$' + ((data.P_BANNER / 1).toFixed(2)));
                $('#pc_c').text('$' + ((data.PC_C / 1).toFixed(2)));
                $('#pc_a').text('$' + ((data.PC_A / 1).toFixed(2)));
                $('#pc_p').text('$' + ((data.PC_P / 1).toFixed(2)));
                $('#pc_t').text('$' + ((data.PC_T / 1).toFixed(2)));
                $('#consu').text('$' + ((data.CONSU / 1).toFixed(2)));
                //RSG 26.04.2018----------------
            }

        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });

}

$('body').on('focusout', '#monto_dis', function () {
    var neg = $("#select_neg").val();
    //Obtener la distribución
    var dis = $("#select_dis").val();


    if (neg == "P" && dis == "C") {
        var monto = $('#monto_dis').val();
        monto = parseFloat(monto);

        if (monto > 0) {
            //Actualizar la tabla con los porcentajes
            updateTableCat();
        } else {
            M.toast({ html: 'El monto debe de ser mayor a 0' });
            return false;
        }
    }
    cambiaRec();//RSG 06.06.2018
});

$('body').on('click', '.prelacionada', function () {
    $(this).prop('disabled', true);
});

$('body').on('click', '.preversa', function () {
    $(this).prop('disabled', true);
});

$('body').on('click', '.pm', function () {
    $(this).prop('disabled', true);
});

$('body').on('click', '.pc', function () {
    $(this).prop('disabled', true);
});

$('body').on('focusout', '.input_oper', function () {
    var t = $('#table_dis').DataTable();
    var tr = $(this).closest('tr'); //Obtener el row 

    //Obtener el tipo de negociación
    var neg = $("#select_neg").val();

    //Solo a cantidades
    if ($(this).hasClass("numberd")) {
        //Total aplica nadamás para el monto                
        if (neg == "M") {
            //Se dispara el evento desde el total
            if ($(this).hasClass("total")) {
                var total_val = $(this).val();
                //Agregar los valores a 0 y agregar el total
                updateTotalRow(t, tr, "", "X", total_val);
            } else {
                updateTotalRow(t, tr, "", "", 0);
            }
        } else if (neg == "P" && $(this).hasClass("porc_cat")) {
            //Modificar el valor del total
            var mont = $('#monto_dis').val();
            mont = parseFloat(mont);
            if (mont > 0) {
                var val = $(this).val();
                val = parseFloat(val);
                if (val > 0) {
                    var res = (val * mont) / 100;
                    updateTotalRow(t, tr, "X", "X", res);
                } else {
                    $(this).val("0.00");
                    updateTotalRow(t, tr, "X", "X", 0);
                }
            } else {
                M.toast({ html: 'El monto debe de ser mayor a 0' });
                return false;
            }
        } else {//if(neg == "C") {
            //if ($(this).hasClass("total")) {
            var total_val = $(this).val();
            //Agregar los valores a 0 y agregar el total
            //updateTotalRow(t, tr, "", "X", total_val);
            //} 
            updateTotalRow(t, tr, "", "", 0);
        }

        ////Se dispara el evento desde el porcentaje de apoyo
        //if ($(this).hasClass("pm")) {
        //    //Saber si el cálculo se hace desde tabla
        //    //Obtener el costo unitario
        //    //Si costo unitario mayor a cero, entonces se hace cálculo desde la tabla
        //    var index = getIndex();
        //    var c_unitario = tr.find("td:eq(" + (8 + index) + ") input").val();
        //    c_unitario = parseFloat(c_unitario);
        //    if (c_unitario > 0) {
        //        //Calculo desde la tabla
        //        updateTotalRow(t, tr, "", "", 0);
        //    } else {
        //        //Calculo con el monto base
        //        //Se obtiene el valor del monto base
        //        var monto_base = $('#monto_dis').val();
        //        monto_base = parseFloat(monto_base);

        //        //Se obtiene el valor del input que genero el evento
        //        var porcentaje = $(this).val();
        //        if (monto_base > 0) {
        //            if (porcentaje > 0) {
        //                var total_val2 = (porcentaje * monto_base) / 100;
        //                //Agregar los valores a 0 y agregar el total
        //                updateTotalRow(t, tr, "X", "X", total_val2);
        //            } else {
        //                M.toast({ html: 'El porcentaje de apoyo debe de ser mayor a cero' });
        //                return false;
        //            }
        //        } else {
        //            M.toast({ html: 'El monto base debe de ser mayor a cero' });
        //            return false;
        //        }
        //    }
        //} else {
        //    updateTotalRow(t, tr, "", "", 0);
        //}
    }

    //Validar si el focusout fue en la columna de material
    if ($(this).hasClass("input_material")) {
        //Validar el material
        var mat = $(this).val();
        var val = valMaterial(mat, "X");
        var index = getIndex();

        if (val.ID == null || val.ID == "") {
            tr.find('td').eq((5 + index)).addClass("errorMaterial");
        } else if (trimStart('0', val.ID) == mat) {//RSG 07.06.2018
            //} else if (trimStart('0', val.ID) == mat) {

            selectMaterial(val.ID, val.MAKTX, tr);

        } else {
            tr.find('td').eq((5 + index)).addClass("errorMaterial");
        }

    }

});

$('body').on('focusout', '.input_sop_f', function () {
    var t = $('#table_dis').DataTable();
    var tr = $(this).closest('tr'); //Obtener el row 

    //Validar si el focusout fue en la columna de proveedor
    if ($(this).hasClass("input_proveedor")) {
        //Validar el material
        var pro = $(this).val();
        var val = valProveedor(pro);

        if (val.ID == null || val.ID == "") {
            tr.find("td.PROVEEDOR").addClass("errorProveedor");
            tr.find("td.PROVEEDOR_TXT").text("");
        } else if (val.ID == pro) {

            selectProveedor(val.ID, val.NOMBRE, tr);

        } else {
            tr.find("td.PROVEEDOR").addClass("errorProveedor");
            tr.find("td.PROVEEDOR_TXT").text("");
        }

    }

});

$('body').on('focusout', '#bmonto_apoyo', function () {
    var val = $(this).val();
    updateTableValIndex(9, val);

});

//$('body').on('focusout', '#monto_dis', function () {

//    //Obtener el tipo de negociación
//    var neg = $("#select_neg").val();

//    //Obtener la distribución
//    var dis = $("#select_dis").val();

//    if (neg == "P" && dis == "C") {
//        var val = $(this).val();
//        val = parseFloat(val);

//        updateTableValIndexPor(9, val);
//    }



//});



//Variables globales
var detail = "";
var montocambio = 0;
var categoriamaterial = "";
var categoriaDesc = "";
var materialVal = "";
var proveedorVal = "";
var dataConfig = null;

function updateTotalRow(t, tr, tdp_apoyo, totals, total_val) {

    //totals = X cuando nada más se agrega el total

    //Add index
    //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
    //Obtener la distribución
    var index = getIndex();

    //Multiplicar costo unitario % por apoyo(dividirlo entre 100)
    //Columnas 8 * 9 res 10
    //Categoría es 7 * 8 = 9  --> -1
    //Material es 6 * 7 = 8   --> -2

    //Validar si las operaciones se hacen por renglón o solo agregar el valor del total
    if (totals != "X") {
        var col8 = tr.find("td:eq(" + (8 + index) + ") input").val();
        var col9 = tr.find("td:eq(" + (9 + index) + ") input").val();

        col9 = convertP(col9);

        if ($.isNumeric(col9)) {
            col9 = col9 / 100;
        }

        var col10 = col8 * col9;
        //Apoyo por pieza
        //Modificar el input
        tr.find("td:eq(" + (10 + index) + ") input").val(col10.toFixed(2));

        //Costo con apoyo
        var col11 = col8 - col10;
        //col11 = col11.toFixed(2);
        tr.find("td:eq(" + (11 + index) + ")").text(col11.toFixed(2));

        //Estimado apoyo
        var col13 = tr.find("td:eq(" + (13 + index) + ") input").val();
        var col14 = col10 * col13;
        //col14 = col14.toFixed(2);
        tr.find("td:eq(" + (14 + index) + ") input").val(col14.toFixed(2));

        //Agregar nada más el total
    } else {
        total_val = parseFloat(total_val);
        var col14 = total_val.toFixed(2);
        tr.find("td:eq(" + (8 + index) + ") input").val("");
        if (tdp_apoyo != "X") {
            tr.find("td:eq(" + (9 + index) + ") input").val("");
        }
        tr.find("td:eq(" + (10 + index) + ") input").val("");
        tr.find("td:eq(" + (11 + index) + ")").text("");
        tr.find("td:eq(" + (12 + index) + ") input").val("");
        tr.find("td:eq(" + (13 + index) + ") input").val("");
        tr.find("td:eq(" + (14 + index) + ") input").val(col14);
    }

    updateFooter();
    cambiaRec();//RSG 06.06.2018
}

function updateTable() {
    var t = $('#table_dis').DataTable();
    $('#table_dis > tbody  > tr').each(function () {
        if ($(this).hasClass("sc")) {//RSG 24.05.2018----------------
            var index = getIndex();
            var total = $(this).find('td').eq((index + 14)).find('input').val();
            updateTotalRow(t, $(this), "", "X", total);
        } else {//RSG 24.05.2018----------------
            updateTotalRow(t, $(this), "", "", 0);
        }//RSG 24.05.2018
    });

    updateFooter();

}

function updateTableCat() {

    var categorias = GetCategoriasTableCat();
    var totalmonto = GetTotalTableCat(categorias);
    var indext = getIndex();
    var t = $('#table_dis').DataTable();

    var m_base = $('#monto_dis').val();
    m_base = parseFloat(m_base) | 0;

    t.rows().every(function (rowIdx, tableLoop, rowLoop) {

        var tr = this.node();
        var row = t.row(tr);

        //Obtener el id de la categoría
        var index = t.row(tr).index();
        //Categoría en el row
        var catid = t.row(index).data()[0];
        //actualizar la catidad a cada categoría
        var totalr = 0;
        totalr = GetTotalCat(catid);

        //total  -- 100
        //totalr -- ?
        var rp = (totalr * 100) / totalmonto;

        //monto base    ---   100
        //              ---   rp
        var totalrow = (rp * m_base) / 100;

        //t.row(index).data()[7];
        //t.cell(7, 0).nodes().to$().find('input').val("" + rp).draw();
        updateTableCathtml(t, rowLoop, indext, rp, totalrow);
    });

    updateFooter();
    updateDetailRow();

}

function updateDetailRow() {

    var t = $('#table_dis').DataTable();
    t.rows().every(function (rowIdx, tableLoop, rowLoop) {

        var tr = this.node();
        var row = t.row(tr);

        if (row.child.isShown()) {
            //Se debe de actualizar el renglón
            //Ocultarlo
            row.child.hide();

            updateTableCathtmlClass(t, rowLoop, "details");
            ////Mostrarlo nuevamente
            ////Obtener el id de la categoría
            //var index = t.row(tr).index();
            //var catid = t.row(index).data()[0];
            ////Obtener las fechas del row de la categoría
            //var indext = getIndex();
            //var vd = (3 + indext);
            //var va = (4 + indext);
            //var vigencia_de = tr.find("td:eq(" + vd + ") input").val();
            //var vigencia_al = tr.find("td:eq(" + va + ") input").val();

            //row.child(format(catid, vigencia_de, vigencia_al)).show();

        }

    });

}

function updateTableCathtml(t, j, index, p, v) {

    var vd = (9 + index);
    var va = (14 + index);
    var i = 0;
    $("#table_dis > tbody  > tr[role='row']").each(function () {
        if (i == j) {
            $(this).find("td:eq(" + vd + ") input").val(p.toFixed(2));
            $(this).find("td:eq(" + va + ") input").val(v.toFixed(2));
        }
        i++;
    });
}

function updateTableCathtmlClass(t, j, clase) {

    var i = 0;
    $("#table_dis > tbody  > tr[role='row']").each(function () {
        if (i == j) {
            $(this).removeClass(clase);
        }
        i++;
    });

    updateFooter();

}

function GetTotalTableCat(cats) {
    //Obtener el total de las categorías agregadas  
    //
    var total = 0;
    for (var i = 0; i < cats.length; ++i) {
        // do something with `substr[i]`
        total += GetTotalCat(cats[i]);
    }

    return total;
}

//Obtener los materiales por categoría
function GetMaterialesCat(catid, total, m_base) {
    var vals = $('#catmat').val();
    try {
        var jsval = JSON.parse(vals);
    } catch (error) {
    }

    var materiales = [];

    try {
        $.each(jsval, function (i, d) {

            if (catid == d.ID) {
                materiales = GetMaterialesCatDetalle(d.MATERIALES, catid, total, m_base);
                return false;
            }
        }); //Fin de for
    } catch (error) {

    }

    return materiales;
}

function GetMaterialesCatDetalle(jsval, catid, total, m_base) {


    var materiales = [];
    //total   --   100
    //m.VAL   --   m.POR??

    //m_base  -- 100
    //VAL??   -- m.POR
    $.each(jsval, function (i, d) {
        var t = 0;
        var v = 0;
        if (catid == d.ID_CAT) {

            var por = 0;

            try {
                por = (d.VAL * 100) / total;
            } catch (error) {
                por = 0;
            }
            try {
                v = (por * m_base) / 100;
            } catch (error) {
                v = 0;
            }
            var m = {};
            m["MATNR"] = d.MATNR;
            m["DESC"] = d.DESC;
            m['POR'] = por;
            m["VAL"] = v;
            materiales.push(m);
        }
    }); //Fin de for

    return materiales;
}

//Obtener el total por categoría
function GetTotalCat(catid) {
    var vals = $('#catmat').val();
    try {
        var jsval = JSON.parse(vals);
    } catch (error) {
    }

    var total = 0;
    try {
        $.each(jsval, function (i, d) {

            if (catid == d.ID) {
                total = GetTotalCatDetalle(d.MATERIALES, catid);
                return false;
            }
        }); //Fin de for
    } catch (error) {

    }

    return total;
}

function GetTotalCatDetalle(jsval, catid) {


    var total = 0;

    $.each(jsval, function (i, d) {
        var t = 0;

        if (catid == d.ID_CAT) {
            try {
                t = parseFloat(d.VAL);
            } catch (error) {

            }
            total += t;
        }
    }); //Fin de for

    return total;
}

function GetCategoriasTableCat() {
    var categorias = [];
    var t = $('#table_dis').DataTable();
    t.rows().every(function (rowIdx, tableLoop, rowLoop) {

        var tr = this.node();
        var row = t.row(tr);

        //Obtener el id de la categoría
        var index = t.row(tr).index();
        //Categoría en el row
        var catid = t.row(index).data()[0];
        //Agregar la categoría al arreglo
        categorias.push(catid);
    });

    return categorias;

}


function updateTableValIndex(indexr, val) {
    var t = $('#table_dis').DataTable();
    var index = getIndex();
    var ind = (index + indexr);
    $('#table_dis > tbody  > tr').each(function () {

        $(this).find('td').eq(ind).find('input').val(val);

    });

    updateTable();

    updateFooter();

}

function updateTableValIndexPor(indexr, val) {
    var t = $('#table_dis').DataTable();
    var index = getIndex();
    var ind = (index + indexr);

    $('#table_dis > tbody  > tr').each(function () {

        var por = $(this).find('td').eq(ind).find('input').val();
        var res = (por * val) / 100;
        updateTotalRow(t, $(this), "X", "X", res);

    });

    updateFooter();

}

function resetFooter() {
    $('#total_dis').text("$0");
}

function getIndex() {
    var index = 0;
    var dis = $("#select_dis").val();
    if (dis != "") {
        var t = $('#table_dis').DataTable();
        //Distribución por categoría
        if (dis == "C") {
            index = -1;
        } else if (dis == "M") {
            //Distribución por material
            index = -2;
        }
    }

    return index;
}


function updateFooter() {
    resetFooter();
    var index = getIndex();
    coltotal = (14 + index);

    var t = $('#table_dis').DataTable();
    var total = 0;

    $('#table_dis').find("tr").each(function (index) {
        var col4 = $(this).find("td:eq(" + coltotal + ") input").val();

        col4 = convertI(col4);

        if ($.isNumeric(col4)) {
            total += col4;
        }

    });

    total = total.toFixed(2);

    $('#total_dis').text("$" + total);
}

function convertI(i) {
    return typeof i === 'string' ?
        i.replace(/[\$,]/g, '') * 1 :
        typeof i === 'number' ?
            i : 0;
};

function convertP(i) {
    return typeof i === 'string' ?
        i.replace(/[\$,]/g, '') * 1 :
        typeof i === 'number' ?
            i : 0;
};

function format(catid, idate, fdate) {

    //detail = "";
    //var id = parseInt(catid);
    var id = catid;//RSG 06.06.2018
    var tablamat = "";
    if (catid != "") {

        //Obtener los materiales de la categoría
        var total = 0;
        var categorias = GetCategoriasTableCat();
        total = GetTotalTableCat(categorias);
        var m_base = $('#monto_dis').val();
        m_base = parseFloat(m_base) | 0;

        var materiales = [];
        materiales = GetMaterialesCat(catid, total, m_base);

        var rows = "";
        for (var j = 0; j < materiales.length; j++) {

            var m = materiales[j];

            var r =
                '<tr>' +
                '<td style = "display:none">' + id + '</td>' +
                '<td>' + idate + '</td>' +
                '<td>' + fdate + '</td>' +
                '<td>' + m.MATNR + '</td>' +
                '<td>' + m.DESC + '</td>' +
                '<td>' + m.POR.toFixed(2) + '</td>' +
                '<td>' + m.VAL.toFixed(2) + '</td>' +
                '</tr>';

            rows += r;

        }

        tablamat = '<table class=\"display\" style=\"width: 90%; margin-left: 60px;\"><tbody>' + rows + '</tbody></table>';
    }

    return tablamat;

    //    ////Obtener el cliente
    //    //var kunnr = $('#payer_id').val();
    //    ////Obtener la sociedad
    //    //var soc_id = $('#sociedad_id').val();

    //    $.ajax({
    //        type: "POST",
    //        url: 'categoriaMateriales',
    //        data: { "kunnr": kunnr, "catid": id, "soc_id": soc_id },
    //        success: function (data) {
    //            var rows = "";
    //            if (data !== null || data !== "") {
    //                $.each(data, function (i, dataj) {

    //                    //Obtener la descripción del material
    //                    var val = valMaterial(dataj.MATNR, "");
    //                    var desc = "";
    //                    if (val.ID == dataj.MATNR) {

    //                        desc = val.MAKTX;

    //                    }

    //                    var r =
    //                        '<tr>' +
    //                        '<td style = "display:none">' + id + '</td>' +
    //                        '<td>' + idate + '</td>' +
    //                        '<td>' + fdate + '</td>' +
    //                        '<td>' + dataj.MATNR + '</td>' +
    //                        '<td>' + desc + '</td>';
    //                    //'<td>Nixon</td>' +
    //                    //'<td>System Architect</td>' +
    //                    //'<td>Edinburgh</td>' +
    //                    //'<td>$320,800</td>' +
    //                    //'<td>Tiger</td>' +
    //                    //'<td>Tiger</td>' +
    //                    //'<td>Tiger</td>' +
    //                    //'</tr>';

    //                    rows += r;

    //                }); //Fin de for
    //                //var tablamat = '<table class=\"display\" style=\"width: 100%; margin-left: 65px;\">' +
    //                var tablamat = '<table class=\"display\" style=\"width: 100%; margin-left: 60px;\"><tbody>' + rows + '</tbody></table>';

    //                useReturnData(tablamat);
    //            }

    //        },
    //        error: function (xhr, httpStatusMessage, customErrorMessage) {
    //            M.toast({ html: msg });
    //        },
    //        async: false
    //    });
    //}

    //return detail;
}

function useReturnData(data) {
    detail = data;
};

function formatCat() {


    $('#catmat').val("");

    $.ajax({
        type: "POST",
        url: 'grupoMateriales',
        data: {},
        success: function (data) {
            if (data !== null || data !== "") {
                $('#catmat').val(JSON.stringify(data));
            }

        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: msg });
        },
        async: false
    });

}

function evaluarExt(filename) {

    var exts = ['xls', 'xlsx'];
    // split file name at dot
    var get_ext = filename.split('.');
    // reverse name to check extension
    get_ext = get_ext.reverse();
    // check file type is valid as given in 'exts' array
    if ($.inArray(get_ext[0].toLowerCase(), exts) > -1) {
        return true;
    } else {
        return false;
    }
}

function loadFilesf() {
    var files = $('.file_soporte');
    var message = "";

    for (var i = 0; i < files.length; i++) {
        //var file = $(files[i]).get(0).files;
        var className = $(files[i]).attr("class");
        //Valida si el archivo es obligatorio
        if (className.indexOf('nec') >= 0) {
            //Validar archivo en archivo obligatorio
            var nfile = $(files[i]).get(0).files.length;
            if (!nfile > 0) {
                var lbltext = $(files[i]).closest('td').prev().children().eq(0).html();
                //var parenttd = $(files[i]).closest('td').prev().children().eq(0).html();
                //var sitd = $(parenttd).prev().children().eq(0).html();
                //var labeltext = $(sitd).children().eq(0).html();
                //M.toast({ html: 'Error! Archivo Obligatorio: ' + lbltext });
                message = 'Error! Archivo Obligatorio: ' + lbltext;
                break;
            }
        }
    }

    if (message == "") {
        loadFiles(files)
    } else {
        M.toast({ html: message });
    }

}

function loadExcelDis(file) {

    document.getElementById("loader").style.display = "initial";//RSG 24.05.2018
    var formData = new FormData();

    formData.append("FileUpload", file);

    //Obtener el tipo de negociación
    var neg = $("#select_neg").val();
    var monto_apoyo = 0;
    var pm = "";
    if (neg == "P") {
        monto_apoyo = $("#bmonto_apoyo").val();
        monto_apoyo = parseFloat(monto_apoyo);
        pm = "pm";
    }
    var table = $('#table_dis').DataTable();
    table.clear().draw();
    $.ajax({
        type: "POST",
        url: 'LoadExcel',
        data: formData,
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {

            if (data !== null || data !== "") {
                var index = getIndex();

                var relacionada = "";

                if ($("#txt_rel").length) {
                    var vrelacionada = $('#txt_rel').val();
                    if (vrelacionada != "") {
                        relacionada = "prelacionada";
                    }
                }

                var reversa = "";
                if ($("#txt_rev").length) {
                    var vreversa = $('#txt_rev').val();
                    if (vreversa == "preversa") {
                        reversa = vreversa;
                    }
                }

                $.each(data, function (i, dataj) {

                    var date_de = new Date(parseInt(dataj.VIGENCIA_DE.substr(6)));
                    var date_al = new Date(parseInt(dataj.VIGENCIA_AL.substr(6)));

                    date_de = formatDatef(date_de);
                    date_al = formatDatef(date_al);
                    //RSG 24.05.2018---------------------------------
                    //var calculo = "X";
                    //if (dataj.VOLUMEN_EST == 0) {
                    //    calculo = "X";
                    //}
                    var calculo = "";
                    //Definir si los valores van en 0 y nada más poner el total
                    if (dataj.MONTO == "" || dataj.MONTO == "0.00" || dataj.PORC_APOYO == "" || dataj.PORC_APOYO == "0.00") {
                        //Se mostrara nada más el total
                        calculo = "sc";
                    }

                    //RSG 24.05.2018---------------------------------

                    //Obtener el porcentaje de la negociación
                    if (monto_apoyo > 0) {
                        dataj.PORC_APOYO = monto_apoyo;
                    }
                    //var addedRow = addRowMat(table, dataj.POS, dataj.MATNR, dataj.MATKL, dataj.DESC, dataj.MONTO, dataj.PORC_APOYO, dataj.MONTO_APOYO, dataj.MONTOC_APOYO, dataj.PRECIO_SUG, dataj.VOLUMEN_EST, dataj.APOYO_EST, relacionada, reversa, date_de, date_al, calculo);
                    var addedRow = addRowMat(table, dataj.POS, dataj.MATNR, dataj.MATKL, dataj.DESC, dataj.MONTO, dataj.PORC_APOYO, dataj.MONTO_APOYO, dataj.MONTOC_APOYO, dataj.PRECIO_SUG, dataj.VOLUMEN_EST, dataj.APOYO_EST, relacionada, reversa, date_de, date_al, calculo, pm);//RSG 24.05.2018



                    if (calculo != "")//RSG 24.05.2018
                        $(addedRow).addClass(calculo);//RSG 24.05.2018

                    if (dataj.ACTIVO == false) {
                        $(addedRow).find('td').eq((index + 5)).addClass("errorMaterial");
                    }

                }); //Fin de for

                $('#table_dis').css("font-size", "12px");
                $('#table_dis').css("display", "table");
                $('#tfoot_dis').css("display", "table-footer-group");

                if ($('#select_dis').val() == "M") {

                    table.column(0).visible(false);
                    table.column(1).visible(false);
                }

                updateTable();

                if (pm == "pm") {
                    $(".pm").prop('disabled', true);
                    $('.pm').trigger('click');
                }

                document.getElementById("loader").style.display = "none";//RSG 24.05.2018
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            alert("Request couldn't be processed. Please try again later. the reason        " + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage);
            document.getElementById("loader").style.display = "none";//RSG 24.05.2018
        },
        async: false
    });

    //Actualizar los valores en la tabla
    updateTable();

}

function loadExcelSop(file) {

    var formData = new FormData();

    formData.append("FileUpload", file);

    var table = $('#table_sop').DataTable();
    table.clear().draw();
    $.ajax({
        type: "POST",
        url: 'LoadExcelSop',
        data: formData,
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {

            if (data !== null || data !== "") {

                $.each(data, function (i, dataj) {

                    var fecha = new Date(parseInt(dataj.FECHA.substr(6)));
                    var ven = new Date(parseInt(dataj.VENCIMIENTO.substr(6)));
                    var addedRow = table.row.add([
                        dataj.POS,
                        dataj.FACTURA,
                        "" + fecha.getDate() + "/" + (fecha.getMonth() + 1) + "/" + fecha.getFullYear(),
                        dataj.PROVEEDOR,
                        dataj.PROVEEDOR_TXT,
                        dataj.CONTROL,
                        dataj.AUTORIZACION,
                        "" + ven.getDate() + "/" + (ven.getMonth() + 1) + "/" + ven.getFullYear(),
                        dataj.FACTURAK,
                        dataj.EJERCICIOK,
                        dataj.BILL_DOC,
                        dataj.BELNR
                    ]).draw(false).node();

                    if (dataj.PROVEEDOR_ACTIVO == false) {
                        $(addedRow).find('td.PROVEEDOR').addClass("errorProveedor");
                    }

                });
                //Aplicar configuración de columnas en las tablas
                ocultarColumnasTablaSoporteDatos();
                $(".table_sop").css("display", "table");
                $("#table_sop").css("display", "table");
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            alert("Request couldn't be processed. Please try again later. the reason        " + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage);
        },
        async: false
    });

}

function selectTsol(sol) {

    //Obtener el tipo de solicitud NC
    //var sol = $("#tsol_id").val();
    //El valor de sol se obtiene de la vista
    //Obtener el valor de la configuración almacenada en la columna FACTURA
    //de la tabla TSOL en bd
    var mostrar = isFactura2(sol);
    var table = $('#table_sop').DataTable();
    //if (sol == "NC" | sol == "NCI" | sol == "OP") {
    if (mostrar) {
        $('#ref_soporte').css("display", "table");
        //Checar si mostrar la tabla o el archivo
        $('#check_factura').trigger('change');
    } else {

        table.clear().draw();
        $('#ref_soporte').css("display", "none");
    }
    $('.file_sop').val('');
}

function selectTsolr(sol) {

    //Obtener el tipo de reversa
    var rev = $("#treversa_id").val();
    if (rev != "") {
        $('#btn_guardarr').removeClass("disabled");
    } else {
        $('#btn_guardarr').addClass("disabled");
    }
}

function isFactura(tsol) {

    var res = false;

    if (tsol != "") {
        var tsol_val = $('#TSOL_VALUES').val();
        var jsval = $.parseJSON(tsol_val)
        $.each(jsval, function (i, dataj) {

            if (dataj.ID == tsol) {
                res = dataj.FACTURA;
                return false;
            }
        });
    }

    return res;
}

function isFactura2(tsol) {//RSG 18.06.2018

    var res = false;

    if (tsol != "") {
        var tsol_val = $('#TSOL_VALUES2').val();
        var jsval = $.parseJSON(tsol_val)
        $.each(jsval, function (i, dataj) {

            if (dataj.ID == tsol) {
                res = dataj.FACTURA;
                return false;
            }
        });
    }

    return res;
}

function addRowSop(t) {
    addRowSopl(
        t,
        "1", //POS
        "<input class=\"FACTURA input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "<input class=\"FECHA input_sop_f fv\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "<input class=\"PROVEEDOR input_sop_f input_proveedor prv\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "",
        "<input class=\"CONTROL input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "<input class=\"AUTORIZACION input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "<input class=\"VENCIMIENTO input_sop_f fv\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "<input class=\"FACTURAK input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "<input class=\"PEJERCICIOK input_sop_f prv\" maxlength=\"4\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "<input class=\"BILL_DOC input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        "<input class=\"BELNR input_sop_f\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">"
    );
}

function addRowSopl(t, pos, fac, fecha, prov, provt, control, aut, ven, fack, eje, bill, belnr) {
    //var t = $('#table_sop').DataTable();

    t.row.add([
        pos, //POS
        fac,
        fecha,
        prov,
        provt,
        control,
        aut,
        ven,
        fack,
        eje,
        bill,
        belnr
    ]).draw(false);

}

function addRowCat(t, cat, ddate, adate, opt, total, relacionada, reversa, porcentaje, porcentaje_cat) {
    var r = addRowCatl(
        t,
        cat,
        "",
        "",
        "<input class=\"" + relacionada + " input_oper format_date input_fe\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + ddate + "\">", //col3
        "<input class=\"" + relacionada + " input_oper format_date input_fe\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + adate + "\">" + pickerFecha(".format_date"),// RSG 
        opt,
        porcentaje,
        "<input class=\"" + reversa + " input_oper numberd input_dc total " + porcentaje_cat + "\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + total + "\">"
    );

    return r;
}

function addRowCatl(t, cat, exp, sel, ddate, adate, opt, porcentaje, total) {
    var r = t.row.add([
        cat + "", //col0
        exp + "", //col1
        sel + "", ////col2
        ddate + "", //col3
        adate + "",
        "", //Material
        opt + "",
        opt + "",
        //"<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        //"<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        //"<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        //"",
        //"<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        //"<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">",
        //"",
        "",
        porcentaje, //+ valPor,
        "",
        "",
        "",
        "",
        //"" + valCant,
        total + ""
    ]).draw(false).node();

    return r;
}

function addRowMat(t, POS, MATNR, MATKL, DESC, MONTO, PORC_APOYO, MONTO_APOYO, MONTOC_APOYO, PRECIO_SUG, VOLUMEN_EST, PORC_APOYOEST, relacionada, reversa, date_de, date_al, calculo, porcentaje_mat) {

    var r = addRowl(
        t,
        POS,
        "",
        "",
        "<input class=\"" + relacionada + " input_oper format_date input_fe\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + date_de + "\">",
        "<input class=\"" + relacionada + " input_oper format_date input_fe\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + date_al + "\">" + pickerFecha(".format_date"),// RSG 21.05.2018",
        "<input class=\"" + relacionada + " input_oper input_material number\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + MATNR + "\">",
        MATKL,
        DESC,
        "<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + MONTO + "\">",
        "<input class=\"" + reversa + " input_oper numberd input_dc " + porcentaje_mat + "\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + PORC_APOYO + "\">",
        "<input class=\"" + reversa + " input_oper numberd\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + MONTO_APOYO + "\">",
        MONTOC_APOYO,
        "<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + PRECIO_SUG + "\">",
        "<input class=\"" + reversa + " input_oper numberd input_dc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + VOLUMEN_EST + "\">",
        "<input class=\"" + reversa + " input_oper numberd input_dc total " + calculo + "\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + PORC_APOYOEST + "\">",
        "<input class=\"" + reversa + " input_oper numberd input_dc total " + porcentaje_mat + " " + calculo + "\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"" + PORC_APOYOEST + "\">"//RSG 24.05.2018
    );

    return r;
}

function addRowl(t, pos, exp, sel, dd, da, mat, matkl, desc, monto, por_a, monto_a, montoc_a, precio_s, vol_es, porc_apes, apoyo_est/*RSG 24.05.2018*/) {

    var r = t.row.add([
        pos,
        exp,
        sel,
        dd,
        da,
        mat,
        matkl,
        desc,
        monto,
        por_a,
        monto_a,
        montoc_a,
        precio_s,
        vol_es,
        //porc_apes,
        apoyo_est//RSG 24.05.2018
    ]).draw(false).node();

    return r;
}

function ocultarColumnasTablaSoporteDatos() {
    //Obtener la sociedad
    var sociedad = $('#sociedad_id').val();
    //Obtener el país ID
    var pais = $('#pais_id').val();
    //Obtener el tipo de solicitud
    var tsol_id = $('#tsol_id').val();
    ocultarColumnasTablaSoporte(sociedad, pais, tsol_id);

}

function ocultarColumnasTablaSoporte(sociedad, pais, tsol) {
    var table = $('#table_sop').DataTable();

    var data = configColumnasTablaSoporte(sociedad, pais, tsol, "");

    if (data !== null || data !== "") {
        //True son los visibles
        //var prov_txt = true;;
        var i;
        for (i in data) {
            //if (i == "PROVEEDOR") {
            //    prov_txt = data[i];
            //}
            if (data[i] == true | data[i] == false) {
                if (data.hasOwnProperty(i)) {
                    //alert(i + " -- " + data[i]);
                    table.column(i + ':name').visible(data[i]);
                }
            }
        }
        //table.column('PROVEEDOR_TXT:name').visible(prov_txt);
    }
}

function configColumnasTablaSoporte(sociedad, pais, tsol, nu) {

    dataConfig = null;
    var localdataConfig = null;
    $.ajax({
        type: "POST",
        url: 'LoadConfigSoporte',
        data: { "sociedad": sociedad, "pais": pais, "tsol": tsol, "nulos": nu },

        success: function (data) {

            if (data !== null || data !== "") {
                asignardataConfig(data)
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            alert("Request couldn't be processed. Please try again later. the reason        " + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage);
        },
        async: false
    });

    localdataConfig = dataConfig;
    return localdataConfig;
}

function asignardataConfig(val) {
    dataConfig = null;
    dataConfig = val;
}


//function eliminarRowsDistribucion(porc) {

//    var t = $('#table_dis').DataTable();
//    var index = getIndex();
//    $("#table_dis > tbody  > tr[role='row']").each(function () {

//        var porrow = $(this).find('td').eq((index + 14)).find('input').val();

//        if (porrow == porc) {
//            this.select
//        }

//    });

//    eliminar renglones;

//    updateFooter();

//}

function loadFile(f_carta) {//, f_contratos, f_factura, f_jbp) {

    var formData = new FormData();

    formData.append("f_carta", f_carta);


    $.ajax({
        type: "POST",
        url: 'saveFiles',
        data: formData,
        //dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {

            if (data !== null || data !== "") {
                alert("success" + data);

            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            alert("Request couldn't be processed. Please try again later. the reason        " + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage);
        },
        async: false
    });

}

function loadFiles(files) {

    var formData = new FormData();

    var count = 1;
    for (var i = 0; i < files.length; i++) {
        var file = $(files[i]).get(0);
        if ($(files[i]).get(0).files.length > 0) {
            formData.append(file.files[0].name, file.files[0]);
            count++;
        }
    }

    $.ajax({
        type: "POST",
        url: 'saveFiles',
        data: formData,
        //dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {

            if (data !== null || data !== "") {
                alert("success" + data);

            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            alert("Request couldn't be processed. Please try again later. the reason        " + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage);
        },
        async: false
    });

}

//Funciones de evaluación
function evalInfoTab(ret, e) {
    var res = true;
    var msg = "";

    if (evaluarInfoTab()) {
        msg = 'siguiente pestaña!';
    } else {
        msg = 'Verificar valores en los campos de Información!';
        res = false;
    }
    //Email
    var payer_email = $('#payer_email').val();

    if (!validateEmail(payer_email)) {
        msg = 'Introduzca un email válido!';
        res = false;
    }

    //Facuras Add MGC B20180619 2018.06.20
    var fact = "";
    fact = evaluarInfoFacturas();
    if (fact != "") {
        msg = "";
        msg = fact;
        res = false;
    }

    if (ret == true) {
        return res;
    } else {
        if (!res) {
            M.toast({ html: msg });
            e.preventDefault();
            e.stopPropagation();
            //var active = $('ul.tabs .active').attr('href');
            //$('ul.tabs').tabs('select_tab', active);
            var ell = document.getElementById("tabs");
            var instances = M.Tabs.getInstance(ell);
            instances.select('Informacion_cont');
        }
        return "";
    }
}

function evalTempTab(ret, e) {
    var res = true;
    var msg = "";

    if (evaluarTempTab()) {
        msg = 'siguiente pestaña!';
    } else {
        msg = 'Verificar valores en los campos de Temporalidad!';
        res = false;

    }

    if (ret == true) {
        return res;
    } else {
        if (!res) {
            M.toast({ html: msg });
            e.preventDefault();
            e.stopPropagation();
            //    //var active = $('ul.tabs .active').attr('href');
            //    //$('ul.tabs').tabs('select_tab', active);
            var ell = document.getElementById("tabs");
            var instances = M.Tabs.getInstance(ell);
            instances.select('Temporalidad_cont');
        }
        return "";
    }

}

function evalSoporteTab(ret, e) {
    var res = true;
    var msg = "";

    if (evaluarSoporteTab()) {
        msg = 'siguiente pestaña!';
    } else {
        msg = 'Verificar valores en los campos de Soporte!';
        res = false;
    }

    if (ret == true) {
        return res;
    } else {
        if (!res) {
            M.toast({ html: msg });
            e.preventDefault();
            e.stopPropagation();
            //var active = $('ul.tabs .active').attr('href');
            //$('ul.tabs').tabs('select_tab', active);
            var ell = document.getElementById("tabs");
            var instances = M.Tabs.getInstance(ell);
            instances.select('Soporte_cont');
        }
        return "";
    }
}

function evalDistribucionTab(ret, e) {
    var res = true;
    var msg = "";

    if (evaluarDisTab()) {
        msg = 'siguiente pestaña!';
    } else {
        msg = 'Verificar valores en los campos de Distribución!';
        res = false;
    }

    if (ret == true) {
        return res;
    } else {
        if (!res) {
            M.toast({ html: msg });
            e.preventDefault();
            e.stopPropagation();
            //var active = $('ul.tabs .active').attr('href');
            //$('ul.tabs').tabs('select_tab', active);
            var ell = document.getElementById("tabs");
            var instances = M.Tabs.getInstance(ell);
            instances.select('Distribucion_cont');
        }
        return "";
    }
}

function evalFinancieraTab(ret, e) {
    var res = true;
    var msg = "";

    if (evaluarFinancieraTab()) {
        msg = 'siguiente pestaña!';
    } else {
        msg = 'Verificar valores en los campos de Financiera!';
        res = false;
    }

    if (ret == true) {
        return res;
    } else {
        if (!res) {
            M.toast({ html: msg });
            e.preventDefault();
            e.stopPropagation();
            //var active = $('ul.tabs .active').attr('href');
            //$('ul.tabs').tabs('select_tab', active);
            var ell = document.getElementById("tabs");
            var instances = M.Tabs.getInstance(ell);
            instances.select('Financiera_cont');
        }
        return "";
    }
}

//Evaluar los elementos de tab_info
function evaluarInfoTab() {

    var res = true;

    //Obtiene el id de la lista id solicitud, default envía vacío
    if (!$('#txt_rel').length) {
        var tsol_id = $('#tsol_id').val();

        if (!evaluarVal(tsol_id)) {
            return false;
        }

        //Obtiene el id de la lista id clasificación, default envía vacío
        var tall_id = $('#tall_id').val();

        if (!evaluarVal(tall_id)) {
            return false;
        }
    }

    //Sociedad
    var sociedad_id = $('#sociedad_id').val();

    if (!evaluarVal(sociedad_id)) {
        return false;
    }

    //País
    var pais_id = $('#pais_id').val();

    if (!evaluarVal(pais_id)) {
        return false;
    }

    //Estado
    var state_id = $('#state_id').val();

    if (!evaluarVal(state_id)) {
        return false;
    }

    //Ciudad
    var city_id = $('#city_id').val();

    if (!evaluarVal(city_id)) {
        return false;
    }

    //Fecha
    var fechad = $('#fechad').val();

    if (!evaluarVal(fechad)) {
        return false;
    }

    //Periodo
    var periodo = $('#periodo').val();

    if (!evaluarVal(periodo)) {
        return false;
    }

    //Ejercicio
    var ejercicio = $('#ejercicio').val();

    if (!evaluarVal(ejercicio)) {
        return false;
    }
    //Concepto
    var concepto = $('#concepto').val();

    if (!evaluarVal(concepto)) {
        return false;
    }

    //Obtiene el id de la lista id cliente, default envía vacío
    var payer_id = $('#payer_id').val();

    if (!evaluarVal(payer_id)) {
        return false;
    }

    //Sociedad
    var vkorg = $('#vkorg').val();

    if (!evaluarVal(vkorg)) {
        return false;
    }

    //Taxt ID
    //var stcd1 = $('#stcd1').val();

    //if (!evaluarVal(stcd1)) {
    //    return false;
    //}

    //Canal
    var vtweg = $('#vtweg').val();

    if (!evaluarVal(vtweg)) {
        return false;
    }

    //Nombre de la persona
    var payer_nombre = $('#payer_nombre').val();

    if (!evaluarVal(payer_nombre)) {
        return false;
    }

    //Email
    var payer_email = $('#payer_email').val();

    if (!evaluarVal(payer_email)) {
        return false;
    }

    return res;
}
//Add MGC B20180619 2018.06.20 Evaluar las facturas
function evaluarInfoFacturas() {
    var res = "";

    //Saber si son varias facturas o una
    var check = false;
    if ($("#check_factura").is(':checked')) {
        //Tabla con inputs
        check = true;
    }

    //Evaluar referencia a facturas 
    var tsol_id = $('#tsol_id').val();
    var mostrar = isFactura2(tsol_id);
    if (mostrar) {
        //La tabla debe de contener como mínimo un registro
        var lengthT = $("table#table_sop tbody tr[role='row']").length;
        if (lengthT > 0) {
            //Evaluar los registros dentro de la tabla de referencia a facturas
            //Los campos visibles deben de ser obligatorios
            $("#table_sop > tbody  > tr[role='row']").each(function () {

                //Validar factura
                if ($(this).find('td.FACTURA').length) {
                    var fact = textval($(this), check, "FACTURA");
                    res = valcolumn(fact, "FACTURA");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar fecha
                if ($(this).find('td.FECHA').length) {
                    var fecha = textval($(this), check, "FECHA");
                    res = valcolumn(fecha, "FECHA");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar proveedor
                if ($(this).find('td.PROVEEDOR').length) {
                    var prov = textval($(this), check, "PROVEEDOR");
                    res = valcolumn(prov, "PROVEEDOR");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar control
                if ($(this).find('td.CONTROL').length) {
                    var control = textval($(this), check, "CONTROL");
                    res = valcolumn(control, "CONTROL");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar autorización
                if ($(this).find('td.AUTORIZACION').length) {
                    var aut = textval($(this), check, "AUTORIZACION");
                    res = valcolumn(aut, "AUTORIZACION");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar vencimiento
                if ($(this).find('td.VENCIMIENTO').length) {
                    var ven = textval($(this), check, "VENCIMIENTO");
                    res = valcolumn(ven, "VENCIMIENTO");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar facturak
                if ($(this).find('td.FACTURAK').length) {
                    var fk = textval($(this), check, "FACTURAK");
                    res = valcolumn(fk, "FACTURAK");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar ejerciciok
                if ($(this).find('td.EJERCICIOK').length) {
                    var ek = textval($(this), check, "EJERCICIOK");
                    res = valcolumn(ek, "EJERCICIOK");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar bill_doc
                if ($(this).find('td.BILL_DOC').length) {
                    var bd = textval($(this), check, "BILL_DOC");
                    res = valcolumn(bd, "BILL_DOC");
                    if (res != "") {
                        return false;
                    }
                }
                //Validar belnr
                if ($(this).find('td.BELNR').length) {
                    var belnr = textval($(this), check, "BELNR");
                    res = valcolumn(belnr, "BELNR");
                    if (res != "") {
                        return false;
                    }
                }
            });
        } else {
            res = "Referencia a facturas como mínimo un registro";
        }
    }

    return res;
}

//Add MGC B20180619 2018.06.20 Evaluar la columna en el renglón
function textval(tr, check, column) {
    var val = "";
    var rowcl = 'td.' + column;
    //Obtener los valores como textos en la celda
    if (!check) {
        val = tr.find(rowcl).text();
    } else {
        //Obtener los valores como textos de los inputs
        val = tr.find(rowcl + " input").val();
    }
    return val;
}

//Add MGC B20180619 2018.06.20 Evaluar el valor en el renglón
function valcolumn(val, campo) {
    var res = "";
    if (val == "") {
        res = "Error sin valor en el campo " + campo;
    }
    return res;
}

function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}
//Evaluar los elementos de tab_temp
function evaluarTempTab() {

    var res = true;

    //Obtiene el id de la lista id solicitud, default envía vacío
    var fechai_vig = $('#fechai_vig').val();

    if (!evaluarVal(fechai_vig)) {
        return false;
    }

    //Obtiene el id de la lista id clasificación, default envía vacío
    var fechaf_vig = $('#fechaf_vig').val();

    if (!evaluarVal(fechaf_vig)) {
        return false;
    }

    if (res) {
        var res = validar_fechas(fechai_vig, fechaf_vig);
    }

    return res;
}

function evaluarSoporteTab() {

    var res = true;

    if ($("#treversa_id").length) {
        var trev = $('#treversa_id').val();
        if (trev == "") {
            res = false;
        }
    }

    if (res) {
        res = evaluarFiles();
    }

    return res
}

function evaluarDisTab() {

    var res = true;

    //Obtiene el id del tipo de negociación, default envía vacío
    var select_neg = $('#select_neg').val();

    if (!evaluarVal(select_neg)) {
        return false;
    }

    //Obtiene el id de la lista distribución, default envía vacío
    var select_dis = $('#select_dis').val();

    if (!evaluarVal(select_dis)) {
        return false;
    }

    if (res) {
        //Validar los montos tipo de negociación monto
        //Validar el monto base vs monto tabla
        if (select_neg == "M") {
            var monedadis_id = $('#monedadis_id').val();
            var monto_dis = $('#monto_dis').val();
            var total_dis = $('#total_dis').text();

            if ((monto_dis != "" & total_dis != "") & monedadis_id != "") {

                //Base, monto footer
                var res = validar_montos(monto_dis, total_dis);
            } else {
                return false;
            }
            //Validar el porcentaje apoyo monto
        } else if (select_neg == "P") {
            if (select_dis == "C") {
                var monedadis_id = $('#monedadis_id').val();
                var monto_dis = $('#monto_dis').val();
                var total_dis = $('#total_dis').text();

                if ((monto_dis != "" & total_dis != "") & monedadis_id != "") {

                    //Base, monto footer
                    var res = validar_montos(monto_dis, total_dis);
                } else {
                    return false;
                }
            }
        }
    }
    return res;
}

function evaluarFinancieraTab() {

    var res = true;

    //Evaluar el monto
    var monto_doc_md = $('#monto_doc_md').val();

    if (!evaluarValInt(monto_doc_md)) {
        return false;
    }

    //Obtiene el id de la lista id clasificación, default envía vacío
    var moneda_id = $('#moneda_id').val();

    if (!evaluarVal(moneda_id)) {
        return false;
    }

    //Obtener el tipo de cambio
    var tipo_cambio = $('#tipo_cambio').val();

    if (!evaluarValInt(tipo_cambio)) {
        return false;
    }

    //Monto en dolares
    var tipo_cambio = $('#montos_doc_ml2').val();

    if (!evaluarValInt(tipo_cambio)) {
        return false;
    }

    return res;

}

function evaluarFile(id) {
    var filenum = $(id).get(0).files.length;

    if (filenum > 0) {
        var file = document.getElementById(id).files[0];
        var filename = file.name;
        return true;
    } else {
        return false;
    }
}

function evaluarFilesName(id, name) {
    var files = $('.file_soporte');
    var res = false;
    for (var i = 0; i < files.length; i++) {

        var idFile = $(files[i]).attr("id");

        if (idFile != id) {
            //Evaluar solamente los demás archivos
            var file = $(files[i]).get(0).files;
            if (file.length > 0) {
                var localfilename = file[i].name;
                if (localfilename == name) {
                    res = true;
                    break;
                }
            }
        }

    }

    return res;
}

function evaluarFiles() {
    var files = $('.file_soporte');
    var message = "";

    for (var i = 0; i < files.length; i++) {
        //var file = $(files[i]).get(0).files;
        var className = $(files[i]).attr("class");
        //Valida si el archivo es obligatorio
        if (className.indexOf('nec') >= 0) {
            //Validar archivo en archivo obligatorio
            var nfile = $(files[i]).get(0).files.length;
            if (!nfile > 0) {
                var lbltext = $(files[i]).closest('td').prev().children().eq(0).html();
                //var parenttd = $(files[i]).closest('td').prev().children().eq(0).html();
                //var sitd = $(parenttd).prev().children().eq(0).html();
                //var labeltext = $(sitd).children().eq(0).html();
                //M.toast({ html: 'Error! Archivo Obligatorio: ' + lbltext });
                message = 'Error! Archivo Obligatorio: ' + lbltext;
                break;
            }
        }

        //Validar tamaño y extensión
        var file = $(files[i]).get(0).files;
        if (file.length > 0) {
            var sizefile = file[0].size;
            if (sizefile > 20971520) {
                var lbltext = $(files[i]).closest('td').prev().children().eq(0).html();
                message = 'Error! Tamaño máximo del archivo 20 M --> Archivo ' + lbltext + " sobrepasa el tamaño";
                break;
            }

            var namefile = file[0].name;
            if (!evaluarExtSoporte(namefile)) {
                var lbltext = $(files[i]).closest('td').prev().children().eq(0).html();
                message = "Error! Tipos de archivos aceptados 'xlsx', 'doc', 'pdf', 'png', 'msg', 'zip', 'jpg', 'docs' --> Archivo " + lbltext + " no es compatible";
                break;
            }
        }

    }

    if (message == "") {
        return true;
    } else {
        return false;
    }
}

//function evaluarExt(filename) {
function evaluarExtSoporte(filename) {

    //var exts = ['xlsx', 'doc', 'pdf', 'png', 'msg', 'zip', 'jpg', 'docs'];//RSG 18.06.2018
    var exts = ['exe', 'bak'];//RSG 18.06.2018
    // split file name at dot
    var get_ext = filename.split('.');
    // reverse name to check extension
    get_ext = get_ext.reverse();
    // check file type is valid as given in 'exts' array
    if ($.inArray(get_ext[0].toLowerCase(), exts) > -1) {
        //return true;//RSG 18.06.2018
        return false;//RSG 18.06.2018
    } else {
        //return false;//RSG 18.06.2018
        return true;//RSG 18.06.2018
    }
}

function evaluarVal(v) {
    if (v != null && v != "") {
        return true;
    } else {
        return false
    }
}

function evaluarValInt(v) {

    if (v != null && v != "") {
        var is_num = $.isNumeric(v);
        var iNum = parseFloat(v.replace(',', '.'))
        if (iNum > 0 & is_num == true) {
            return true;
        } else {
            return false;
        }
    } else {
        return false
    }
}
function selectTall(valu) {
    if (valu != "") {
        $("#tall_id").empty();
        $.ajax({
            type: "POST",
            url: 'SelectTall',
            data: { "id": valu },
            dataType: "json",
            success: function (data) {

                if (data !== null || data !== "") {
                    $('<option>', {
                        text: "--Seleccione--"
                    }).html("--Seleccione--").appendTo($("#tall_id"));
                    $.each(data, function (i, optiondata) {
                        $('<option>', {
                            value: optiondata.ID,
                            text: optiondata.TEXT
                        }).html(optiondata.NAME).appendTo($("#tall_id"));
                    });

                    var elem = document.getElementById('tall_id');
                    var instance = M.Select.init(elem, []);
                    $("#tall_id").val(data[0].ID);
                }
            },
            error: function (data) {
                alert("Request couldn't be processed. Please try again later. the reason        " + data);
            },
            async: false
        });
    }
}

function selectDis(val) {
    resetFooter();
    var message = "X";
    if (val == "M") {//Monto
        $('#div_apoyobase').css("display", "none");
        $('#div_montobase').css("display", "inherit");
    } else if (val == "P") {//Porcentaje
        M.toast({ html: '¿Desea realizar esta solicitud por porcentaje?' });
        message = "";
        $('#div_montobase').css("display", "none");//none
        $('#div_apoyobase').css("display", "inherit");
    } else {
        $('#div_montobase').css("display", "none");
        $('#div_apoyobase').css("display", "none");
    }
    var select_dis = $('#select_dis').val();
    //$('#select_dis').val(select_dis).change();
    selectMonto(select_dis, message);
}

function selectMonto(val, message) {
    //message = "X";
    //Siempre inicializar la tabla
    var ta = $('#table_dis').DataTable();
    ta.clear().draw();

    //Reset los valores
    $('#monto_dis').val("");
    //$('#bmonto_apoyo').val("");//RSG 29.06.2018

    //Obtener la negociación
    var select_neg = $('#select_neg').val();

    //Desactivar el panel de monto
    if (val == "" || select_neg == "") {
        $('#div_montobase').css("display", "none");
        $('#div_apoyobase').css("display", "none");
        $('#cargar_excel').css("display", "none");
        $('#select_categoria').css("display", "none");
        $('.div_categoria').css("display", "none");
        $('#table_dis').css("display", "none");
        $('#div_btns_row').css("display", "none");
        ta.column(0).visible(false);
        ta.column(1).visible(false);
    } else {
        //Activar el panel de monto dependiendo del tipo de negociación
        //$('#div_montobase').css("display", "inherit");
        $('#div_btns_row').css("display", "inherit");

        if (select_neg == "M") {//Monto
            $('#div_apoyobase').css("display", "none");
            $('#div_montobase').css("display", "inherit");
        } else if (select_neg == "P") {//Porcentaje
            if (message == "X") {
                M.toast({ html: '¿Desea realizar esta solicitud por porcentaje?' });//Add
            }
            $('#div_montobase').css("display", "none");//none
            $('#div_apoyobase').css("display", "inherit");
        } else {
            $('#div_montobase').css("display", "none");
            $('#div_apoyobase').css("display", "none");
        }
    }

    //Cuando es negociación por porcentaje y distribución por categoría, mostrar el monto 
    if (select_neg == "P" && val == "C") {
        $('#div_montobase').css("display", "inherit");//Mostra el monto
        $('#div_apoyobase').css("display", "none");//Mostra el monto
    }

    if (select_neg != "") {
        //Monto
        if (val == "M") {
            $('#cargar_excel').css("display", "inherit");
            $('#select_categoria').css("display", "none");
            $('.div_categoria').css("display", "none");
            ta.column(0).visible(false);
            ta.column(1).visible(false);
        }

        //Categoría
        if (val == "C") {

            $('#cargar_excel').css("display", "none");
            $('.div_categoria').css("display", "inline-block");
            //Mostrar el encabezado de la tabla               
            $('#table_dis').css("font-size", "12px");
            $('#table_dis').css("display", "table");
            ta.column(0).visible(false);
            ta.column(1).visible(true);
        }

        resetFooter();
    } else {
        M.toast({ html: 'Seleccione Negociación' });
    }
}

function selectCity(valu) {
    if (valu != "") {
        $("#city_id").empty();
        $.ajax({
            type: "POST",
            url: 'SelectCity',
            data: { "id": valu },
            dataType: "json",
            success: function (data) {

                if (data !== null || data !== "") {
                    $('<option>', {
                        text: "--Seleccione--"
                    }).html("--Seleccione--").appendTo($("#city_id"));
                    $.each(data, function (i, optiondata) {
                        $('<option>', {
                            value: optiondata.ID,
                            text: optiondata.NAME
                        }).html(optiondata.NAME).appendTo($("#city_id"));
                    });

                    $('#city_id').formSelect();
                }
            },
            error: function (data) {
                alert("Request couldn't be processed. Please try again later. the reason        " + data);
            },
            async: false
        });
    }

}
function asignCity(valu) {
    if (valu != "") {
        var iNum = parseInt(valu);
        $('#citys_id').val(iNum);
    }
}

function selectCliente(valu) {
    if (valu != "") {
        $.ajax({
            type: "POST",
            url: 'SelectCliente',
            data: { "kunnr": valu },

            success: function (data) {

                if (data !== null || data !== "") {
                    $('#cli_name').val(data.NAME1);
                    $("label[for='cli_name']").addClass("active");
                    $('#vkorg').val(data.VKORG).focus();
                    $("label[for='vkorg']").addClass("active");
                    $('#parvw').val(data.PARVW).focus();
                    $("label[for='parvw']").addClass("active");
                    $('#stcd1').val(data.STCD1);
                    $("label[for='stcd1']").addClass("active");
                    $('#vtweg').val(data.VTWEG);
                    $("label[for='vtweg']").addClass("active");
                    //Si la solicitud es una relacionada, obtener el nombre y email del contacto almacenado en DOCUMENTO
                    if (!$('#payer_id').hasClass("prelacionada")) {
                        ////$('#payer_nombre').val(data.PAYER_NOMBRE);//RSG 20180622
                        ////$("label[for='payer_nombre']").addClass("active");
                        ////$('#payer_email').val(data.PAYER_EMAIL);
                        ////$("label[for='payer_email']").addClass("active");
                    }
                    //RSG 28.05.2018------------------------------------------
                    //MGC B20180611
                    if (!isRelacionada()) {
                        llenaCat(data.VKORG, data.VTWEG, data.SPART, valu);
                        getCatMateriales(data.VKORG, data.VTWEG, data.SPART, valu);
                    }
                    //RSG 28.05.2018------------------------------------------
                } else {
                    $('#cli_name').val("");
                    $("label[for='cli_name']").removeClass("active");
                    $('#vkorg').val("").focus();
                    $("label[for='vkorg']").removeClass("active");
                    $('#parvw').val("").focus();
                    $("label[for='parvw']").removeClass("active");
                    $('#stcd1').val("");
                    $("label[for='stcd1']").removeClass("active");
                    $('#vtweg').val("");
                    $("label[for='vtweg']").removeClass("active");
                    $('#payer_nombre').val("");
                    $("label[for='payer_nombre']").removeClass("active");
                    $('#payer_email').val("");
                    $("label[for='payer_email']").removeClass("active");
                }

            },
            error: function (data) {
                $('#cli_name').val("");
                $("label[for='cli_name']").removeClass("active");
                $('#vkorg').val("").focus();
                $("label[for='vkorg']").removeClass("active");
                $('#parvw').val("").focus();
                $("label[for='parvw']").removeClass("active");
                $('#stcd1').val("");
                $("label[for='stcd1']").removeClass("active");
                $('#vtweg').val("");
                $("label[for='vtweg']").removeClass("active");
                $('#payer_nombre').val("");
                $("label[for='payer_nombre']").removeClass("active");
                $('#payer_email').val("");
                $("label[for='payer_email']").removeClass("active");
            },
            async: false
        });
    }

}

function getCatMateriales(vkorg, vtweg, spart, kunnr) {
    document.getElementById("loader").style.display = "initial";
    var soc = document.getElementById("sociedad_id").value;
    $('#catmat').val("");
    $.ajax({
        type: "POST",
        url: 'grupoMateriales',
        dataType: "json",
        data: { vkorg: vkorg, spart: spart, kunnr: kunnr, soc_id: soc },
        success: function (data) {
            if (data !== null || data !== "") {
                $('#catmat').val(JSON.stringify(data));
            }
            document.getElementById("loader").style.display = "none";
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
            document.getElementById("loader").style.display = "none";
        },
        async: false
    });
}

function selectMoneda(valu) {
    $('#monto_doc_ml2').val("");
    $('#montos_doc_ml2').val("");
    $('#tipo_cambio').val("");
    $('#monedas_id').val("");

    if (valu != "") {
        var monto_doc_md = $('#monto_doc_md').val()
        var mt = parseFloat(monto_doc_md.replace(',', '.'))
        if (mt > 0) {

            $('#monedas_id').val(valu);

            $.ajax({
                type: "POST",
                url: 'SelectTcambio',
                data: { "fcurr": valu },

                success: function (data) {

                    if (data !== null || data !== "") {
                        var iNum = parseFloat(data.replace(',', '.')).toFixed(2);
                        if (iNum > 0) {

                            $('#tipo_cambio').val(iNum);

                            var monto_doc_md = $('#monto_doc_md').val()

                            if (monto_doc_md > 0) {
                                //Obtener la moneda en la lista
                                var MONEDA_ID = $('#moneda_id').val();

                                selectTcambio(MONEDA_ID, monto_doc_md);

                            }
                        } else {
                            M.toast({ html: data });
                        }
                    }

                },
                error: function (data) {
                    alert("Error tipo de cambio        " + data);
                },
                async: false
            });

        } else {
            var msg = 'Monto incorrecto';
            M.toast({ html: msg })
        }
    }
}

function selectTcambio(MONEDA_ID, monto_doc_md) {
    $('#monto_doc_ml2').val();
    $('#montos_doc_ml2').val();

    if (MONEDA_ID != "") {

        $.ajax({
            type: "POST",
            url: 'SelectVcambio',
            data: { "moneda_id": MONEDA_ID, "monto_doc_md": monto_doc_md },

            success: function (data) {

                if (data !== null || data !== "") {
                    var iNum = parseFloat(data.replace(',', '.')).toFixed(2);
                    if (iNum > 0) {
                        $('#monto_doc_ml2').val(iNum);
                        $('#montos_doc_ml2').val(iNum);
                        $("label[for='montos_doc_ml2']").addClass("active");

                    } else {
                        M.toast({ html: data });
                    }
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: msg });
            },
            async: false
        });
    }

}

function cambioCurr(fcurr, tcurr, monto) {
    montocambio = 0;
    var localmonto = 0;
    if (fcurr != "" & tcurr != "" & monto != "") {

        $.ajax({
            type: "POST",
            url: 'cambioCurr',
            data: { "fcurr": fcurr, "tcurr": tcurr, "monto": monto },

            success: function (data) {

                if (data !== null || data !== "") {

                    var iNum = parseFloat(data.replace(',', '.')).toFixed(2);
                    if (iNum > 0) {
                        asignarMonto(data);
                    } else {
                        M.toast({ html: data });
                    }
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: msg });
            },
            async: false
        });
    }

    localmonto = montocambio;
    return localmonto;

}

function asignarMonto(monto) {
    montocambio = monto;
}

function validar_fechas(ini_date, fin_date) {//RSG 22.05.2018

    var DateToValue = new Date();
    var DateFromValue = new Date();

    var idate = ini_date.split('/');
    //DateFromValue.setFullYear(idate[0], idate[1], idate[2], 0, 0, 0, 0);
    DateFromValue.setDate(idate[0]);
    DateFromValue.setMonth(idate[1] - 1);
    DateFromValue.setFullYear(idate[2]);

    var fdate = fin_date.split('/');
    //DateToValue.setFullYear(fdate[0], fdate[1], fdate[2], 0, 0, 0, 0);
    DateToValue.setDate(fdate[0]);
    DateToValue.setMonth(fdate[1] - 1);
    DateToValue.setFullYear(fdate[2]);

    var d1 = Date.parse(DateFromValue);
    var d2 = Date.parse(DateToValue);
    if (d1 <= d2) {
        return true;
    }
    return false;
}

function validar_montos(base, footer) {

    var basei = convertI(base);
    var footeri = convertI(footer);

    if (basei == footeri) {
        return true;
    }

    return false;
}

function getCategoria(mat) {
    categoriamaterial = "";
    var localcat = "";
    if (mat != "") {
        $.ajax({
            type: "POST",
            url: 'getCategoria',
            data: { "material": mat },
            dataType: "json",

            success: function (data) {

                if (data !== null || data !== "") {
                    asignarCategoria(data);
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }

    localcat = categoriamaterial;
    return localcat;

}

function asignarCategoria(cat) {
    categoriamaterial = cat;
}

function getCategoriaDesc(catid) {
    categoriaDesc = "";
    var localcat = "";
    if (catid != "") {
        $.ajax({
            type: "POST",
            url: 'getCategoriaDesc',
            data: { "cate": catid },
            dataType: "json",

            success: function (data) {

                if (data !== null || data !== "") {
                    asignarCategoriaDesc(data);
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }

    localcat = categoriaDesc;
    return localcat;

}

function asignarCategoriaDesc(cat) {
    categoriaDesc = cat;
}

function valMaterial(mat, message) {
    materialVal = "";
    var localval = "";
    if (mat != "") {
        $.ajax({
            type: "POST",
            url: 'getMaterial',
            dataType: "json",
            data: { "mat": mat },

            success: function (data) {

                if (data !== null || data !== "") {
                    asignarValMat(data);
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

    localval = materialVal;
    return localval;
}

function asignarValMat(val) {
    materialVal = val;
}


function valProveedor(prov) {
    proveedorVal = "";
    var localval = "";
    if (prov != "") {
        $.ajax({
            type: "POST",
            url: 'getProveedor',
            dataType: "json",
            data: { "prov": prov },
            success: function (data) {

                if (data !== null || data !== "") {
                    asignarValProv(data);
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: "Valor no encontrado" });
            },
            async: false
        });
    }

    localval = proveedorVal;
    return localval;
}

function asignarValProv(val) {
    proveedorVal = val;
}

function valcategoria(cat) {

    var res = false;
    var t = $('#table_dis').DataTable();
    t.rows().every(function (rowIdx, tableLoop, rowLoop) {

        var tr = this.node();
        var row = t.row(tr);

        //Obtener el id de la categoría
        var index = t.row(tr).index();
        //Categoría en el row
        var catid = t.row(index).data()[0];
        //Comparar la categoría en la tabla y la agregada
        if (catid === "000" | cat === "000") {//RSG 05.06.2018
            res = true;
        }
        if (cat == catid) {
            res = true;
        }

    });

    return res;
}

//MGC B20180611 Verificar si es relacionada
function isRelacionada() {
    var res = false;
    if ($("#txt_rel").length) {
        var vrelacionada = $('#txt_rel').val();
        if (vrelacionada != "") {
            res = true;
        }
    }

    return res;
}

//MGC B20180611 Verificar si es reversa
function isReversa() {
    var res = false;
    if ($("#txt_rev").length) {
        var vreversa = $('#txt_rev').val();
        if (vreversa == "preversa") {
            res = true;
        }
    }
    return res;
}