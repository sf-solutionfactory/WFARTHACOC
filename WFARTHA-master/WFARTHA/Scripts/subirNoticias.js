$(document).ready(function () {
    $('#fileImg').change(function () {
        var length = $(this).length;
        if (length > 0) {
            cargarImagen(this);
        }
    });

    $("#btnCancelar").click(function () {
        //Limpiamos y escondemos los header, imagenes, y botones
        $('#previa').css('display', 'none');
        $('#lblPrevia').css('display', 'none');
        $('#btnCancelar').css('display', 'none');
        $('#btnAp').css('display', 'none');
        //document.getElementById("fileImg").value = "";
        $("#fileImg").val('');
        $("#lblImgText").val('');
        $('.oc').css('display', 'none');
    });

    $("#btnAp").click(function () {
        //Realizar guardado
        var res = loadImage();
        if (res != "0") {
            //POST
            $("#btnCrear").click();
        }
    });
});

function cargarImagen(_this) {
    var length = $('#fileImg').length;
    var message = "";
    var namefile = "";
    var f = $('#fileImg').val();
    if (length > 0) {
        //Validar tamaño y extensión
        var file = $('#fileImg').get(0).files;
        if (file.length > 0) {
            var sizefile = file[0].size;
            namefile = file[0].name;
            if (sizefile > 20971520) {
                message = 'Error! Tamaño máximo del archivo 20 M --> Archivo ' + namefile + " sobrepasa el tamaño";

            }
        }
    } else {
        message = "No selecciono archivo";
    }
    if (message != "") {
        $(this).val("");
        M.toast({ html: message });
    } else {
        preview(_this);
    }
}

function loadImage() {
    var formData = new FormData();
    var file_s = document.getElementById("fileImg").files.length;
    for (var i = 0; i < file_s; i++) {
        var file = document.getElementById("fileImg").files[i];
        formData.append("fileImg", file);
    }
    var res = "";
    $.ajax({
        type: "POST",
        url: 'guardarImagen',
        data: formData,
        //dataType: 'json',
        cache: false,
        contentType: false,
        processData: false,
        // contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        success: function (data) {
            res = data;
            if (data != "0") {
                //Accion a realizar
                $("#PATH").val(data);
            }
            if (data == "0") {
                M.toast({ html: "Archivo Existente" });
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
    return res;
}

function preview(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#previa').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
        //Activamos los botones de cancelar, aplicar(guardar),
        //el header de vista previa, y el tag imagen
        $('#previa').css('display', 'block');
        $('#lblPrevia').css('display', 'block');
        $('#btnCancelar').css('display', 'inline-block');
        $('#btnAp').css('display', 'inline-block');
        $('.oc').css('display', 'block');
        cargarDaPic();
    }
}

function cargarDaPic() {
    var elemdp = document.querySelectorAll('#FECHAI');
    var optionsdp = {
        format: 'dd/mm/yyyy',
        onClose: function (e) {
            var date = $('#FECHAI').val();
        },
        i18n: {
            clear: 'Limpiar',
            today: 'Hoy',
            done: 'Seleccionar',
            previousMonth: '‹',
            nextMonth: '›',
            months: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthsShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab'],
            weekdays: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            weekdaysAbbrev: ['D', 'L', 'M', 'X', 'J', 'V', 'S']
        }
    };
    var instancedp = M.Datepicker.init(elemdp, optionsdp);

    var elemdp_ = document.querySelectorAll('#FECHAF');
    var optionsdp_ = {
        format: 'dd/mm/yyyy',
        onClose: function (e) {
            var date = $('#FECHAF').val();
        },
        i18n: {
            clear: 'Limpiar',
            today: 'Hoy',
            done: 'Seleccionar',
            previousMonth: '‹',
            nextMonth: '›',
            months: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthsShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab'],
            weekdays: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            weekdaysAbbrev: ['D', 'L', 'M', 'X', 'J', 'V', 'S']
        }
    };
    var instancedp1 = M.Datepicker.init(elemdp_, optionsdp_);
}