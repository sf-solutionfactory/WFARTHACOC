$(document).ready(function () {

    $('#btn_guardarh').on('click', function () {
        var _mailnull = true;
        var _mailerror = true;
        var _idexiste = true;
        var _idnull = true;
        var _nombrenull = true;
        var _usuarionull = true;
        //var _sociedadnull = true;
        var msgerror = "";
        var mail = $("#EMAIL").val();
        var id = $("#ID").val();
        var nombre = $("#NOMBRE").val();
        var vista = $("#vista").val();

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


        if (_mailnull) {
            if (_mailerror) {
                if (_idnull) {
                    if (_nombrenull) {
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
});