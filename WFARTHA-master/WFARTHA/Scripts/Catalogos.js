$(document).ready(function () {

    $('#btn_guardarh').on('click', function () {
        var _mailnull = true;
        var _mailerror = true;
        var _idexiste = true;
        var _idnull = true;
        var _nombrenull = true;
        var _usuarionull = true;
        var _appnull = true;
        var _apm = true;
        //var _sociedadnull = true;
        var msgerror = "";
        var mail = $("#EMAIL").val();
        var id = $("#ID").val();
        var nombre = $("#NOMBRE").val();
        var vista = $("#vista").val();
        var app = $('#APELLIDO_P').val();

        if (vista == "E") {
            var usuario = $("#PUE").val();
            //var sociedad = $("#BUKRS").val();
        } else {
            var usuario = $("#PUESTO_ID").val();
            //var sociedad = $("#BUNIT").val();
        }



        if (mail == null || mail == "") {
            msgerror = "El correo es obligatorio";
            _mailnull = false;

        } else {
            emailRegex = /^[-\w.%+]{1,64}@(?:[A-Z0-9-]{1,63}\.){1,125}[A-Z]{2,63}$/i;
            if (!emailRegex.test(mail)) {
                _mailerror = false;
                msgerror = "El formato de correo no es correcto";
            } else {
                if (nombre.trim() == null || nombre.trim() == "") {
                    _nombrenull = false;
                    msgerror = "El nombre de usuario es obligatorio";
                } else {
                    if (app.trim() == null || app.trim() == "") {
                        _appnull = false;
                        msgerror = "Ingrese Apellido Paterno";
                    } else {
                        if (usuario.trim() == null || usuario.trim() == "") {
                            _usuarionull = false;
                            msgerror = "Ingrese tipo de Usuario";
                        } else {
                            if (vista != "E") {
                                if (id.trim() == null || id.trim() == "") {
                                    _idnull = false;
                                    msgerror = "El ID de usuario es obligatorio";
                                } else {
                                    $.ajax({
                                        type: "POST",
                                        url: 'getUsuario',
                                        data: { 'id': id },
                                        dataType: "json",
                                        success: function (data) {
                                            if (!data) {
                                                _idexiste = true;
                                            } else {
                                                msgerror = "Ya existe un usuario registrado con ese ID";
                                                _idexiste = false;
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
                    }
                }
            }
        }


        if (_mailnull) {
            if (_mailerror) {
                if (_idnull) {
                    if (_nombrenull) {
                        if (_appnull) {
                            if (_usuarionull) {
                                if (_idexiste) {
                                    $('#btn_guardar').trigger("click");
                                } else {
                                    M.toast({ html: msgerror });
                                }
                            } else {
                                M.toast({ html: msgerror });
                            }
                        } else {
                            M.toast({ html: msgerror });
                        }
                    } else {
                        M.toast({ html: msgerror });
                    }

                } else {
                    M.toast({ html: msgerror });
                }
            } else {
                M.toast({ html: msgerror });
            }
        } else {
            M.toast({ html: msgerror });
        }
        
    });

    $('#btn_guardarhp').on('click', function () {
        var pass1 = $("#PASS").val();
        var pass2 = $("#FIRMA").val();
        var _passnull = true;
        var _passcoi = true;
        if (pass1.trim() == null || pass1.trim() == "" || pass2.trim() == null || pass2.trim() == "") {
            msgerror = "Las contraseñas son obligatorias";
            _passnull = false
        } else {
            if (pass1.trim() !== pass2.trim()) {
                msgerror = "Las contraseñas no coinciden";
                _passcoi = false;
            }
        }

        if (_passnull) {
            if (_passcoi) {
                $('#btn_guardarp').trigger("click");
            } else {
                M.toast({ html: msgerror });
            }
        } else {
            M.toast({ html: msgerror });
        }

    });


    $('#btn_guardarhcc').on('click', function () {
        var usuarioc = $("#USUARIOC_ID").val();
        var ruta = $("#ID_RUTA_AGENTE").val();

        var _usuariocnull = true;
        var _rutanull = true;
        var _rutar = true;

        //MGC Número de revisores
        //Debe de haber al menos un revisor
        var num_rows = 0;
        num_rows = $("table#tableca tbody tr[role='row']").length;

        if (usuarioc == null || usuarioc == "") {
            _usuariocnull = false;
            msgerror = "Debes seleccionar agente";
        } else {
            if (ruta == null || ruta == "") {

                _rutanull = false;
                msgerror = "La ruta es obligatoria";
                //msgerror = "Tiene que agregar al menos un autorizador a la cadena";
            } else {
                if (num_rows == null || num_rows == 0) {
                    _rutar = false;
                    //msgerror = "La ruta es obligatoria";
                    msgerror = "Tiene que agregar al menos un autorizador a la cadena";
                }
            }
        }

        if (_usuariocnull) {
            if (_rutanull) {
                if (_rutar) {


                    if (num_rows > 0) {
                        var jsonObjDocs = [];
                        //Agregar valores de la tabla
                        $("#tableca > tbody  > tr[role='row']").each(function () {

                            var fase = $(this).find("td.FASE").text();
                            var agente = $(this).find("td.AGENTE").text();

                            var item = {};
                            item["STEP_FASE"] = fase;
                            item["AGENTE_SIG"] = agente.trim();

                            jsonObjDocs.push(item);
                            item = "";

                            var t = $('#tableca').DataTable();

                            //Eliminar el renglón
                            t.rows($(this)).remove().draw(false);


                        });
                        var docsenviar = {};
                        docsenviar = JSON.stringify({ 'docs': jsonObjDocs });

                        $.ajax({
                            type: "POST",
                            url: 'getPartialDET_AGENTECA',
                            contentType: "application/json; charset=UTF-8",
                            data: docsenviar,
                            success: function (data) {

                                if (data !== null || data !== "") {

                                    $("table#tableca tbody").append(data);
                                }

                            },
                            error: function (xhr, httpStatusMessage, customErrorMessage) {
                                M.toast({ html: httpStatusMessage });
                            },
                            async: false
                        });
                    }


                    $('#btn_guardarcc').trigger("click");
                } else {
                    M.toast({ html: msgerror });
                }
            } else {
                M.toast({ html: msgerror });
            }
        } else {
            M.toast({ html: msgerror });
        }

    });


    $('#btn_guardarhca').on('click', function () {
        var usuarioc = $("#USUARIOC_ID").val();
        var usuarioa = $("#USUARIOA_ID").val();
        //var ruta = $("#ID_RUTA_AGENTE").val();

        //MGC Número de revisores
        //Debe de haber al menos un revisor
        var num_rows = 0;
        num_rows = $("table#tableca tbody tr[role='row']").length;

        var _usuariocnull = true;
        var _usuarioanulli = true;
        var _rutanull = true;


        if (usuarioc == null || usuarioc == "") {
            _usuariocnull = false;
            msgerror = "Debes seleccionar usuario creador";
        } else {
            if (usuarioa == null || usuarioa == "") {
                _usuarioanulli = false;
                msgerror = "Debes seleccionar agente";
            } else {
                //if (ruta == null || ruta == "") {
                if (num_rows == null || num_rows == 0) {
                    _rutanull = false;
                    //msgerror = "La ruta es obligatoria";
                    msgerror = "Tiene que agregar al menos un autorizador a la cadena";
                }
            }
        }

        if (_usuariocnull) {
            if (_usuarioanulli) {
                if (_rutanull) {

                    if (num_rows > 0) {
                        var jsonObjDocs = [];
                        //Agregar valores de la tabla
                        $("#tableca > tbody  > tr[role='row']").each(function () {

                            var fase = $(this).find("td.FASE").text();
                            var agente = $(this).find("td.AGENTE").text();

                            var item = {};
                            item["STEP_FASE"] = fase;
                            item["AGENTE_SIG"] = agente;

                            jsonObjDocs.push(item);
                            item = "";

                            var t = $('#tableca').DataTable();

                            //Eliminar el renglón
                            t.rows($(this)).remove().draw(false);


                        });
                        var docsenviar = {};
                        docsenviar = JSON.stringify({ 'docs': jsonObjDocs });

                        $.ajax({
                            type: "POST",
                            url: 'getPartialDET_AGENTECA',
                            contentType: "application/json; charset=UTF-8",
                            data: docsenviar,
                            success: function (data) {

                                if (data !== null || data !== "") {

                                    $("table#tableca tbody").append(data);
                                }

                            },
                            error: function (xhr, httpStatusMessage, customErrorMessage) {
                                M.toast({ html: httpStatusMessage });
                            },
                            async: false
                        });
                    }

                    $('#btn_guardarca').trigger("click");
                } else {
                    M.toast({ html: msgerror });
                }
            } else {
                M.toast({ html: msgerror });
            }
        } else {
            M.toast({ html: msgerror });
        }

    });

    $('#btn_guardarhps').on('click', function () {
        var bukrs = $("#BUKRS").val();
        var id_user = $("#ID_USER").val();
        var tipopre = $("#TIPOPRE").val();

        var _bukrsnull = true;
        var _id_usernull = true;
        var _tipoprenull = true;
        var _existe = true;


        if (bukrs == null || bukrs == "") {
            _bukrsnull = false;
            msgerror = "Es necesario seleccionar una sociedad";
        } else {
            if (id_user == null || id_user == "") {
                _id_usernull = false;
                msgerror = "Es necesario seleccionar un usuario";
            } else {
                if (tipopre == null || tipopre == "") {
                    _tipoprenull = false;
                    msgerror = "Es necesario seleccionar un tipo de presupuesto";
                } else {
                    $.ajax({
                        type: "POST",
                        url: 'getUsuarioSociedad',
                        data: { 'id': id_user, 'bukrs': bukrs },
                        dataType: "json",
                        success: function (data) {
                            if (!data) {
                                _existe = true;
                            } else {
                                msgerror = "Ya existe un usuario y sociedad registrado con ese ID";
                                _existe = false;
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


        if (_bukrsnull) {
            if (_id_usernull) {
                if (_tipoprenull) {
                    if (_existe) {
                        $('#btn_guardarps').trigger("click");
                    } else {
                        M.toast({ html: msgerror });
                    }
                } else {
                    M.toast({ html: msgerror });
                }
            } else {
                M.toast({ html: msgerror });
            }
        } else {
            M.toast({ html: msgerror });
        }

    });

    $('#btn_guardarhpse').on('click', function () {

        var tipopre = $("#TIPOPRE").val();
        var _tipoprenull = true;



        if (tipopre == null || tipopre == "") {
            _tipoprenull = false;
            msgerror = "Es necesario seleccionar un tipo de presupuesto";
        }
        if (_tipoprenull) {
            $('#btn_guardarpse').trigger("click");
        } else {
            M.toast({ html: msgerror });
        }

    });
});