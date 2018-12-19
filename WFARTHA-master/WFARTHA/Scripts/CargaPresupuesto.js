var hilo = true;
var acept = false;
$(document).ready(function () {
    var activateOption = function (collection, newOption) {
        if (newOption) {
            collection.find("li.selected").removeClass("selected");

            var option = $(newOption);
            option.addClass("selected");
        }
    };
    var elem = document.querySelector('.modal');
    var instance = M.Modal.init(elem, []);
    try {
        var table = $('#table').DataTable({
            //scrollY: "70vh",
            scrollX: "10vh",
            language: {
                lengthMenu: "Display _MENU_ records per page",
                zeroRecords: "No se encontraron datos",
                info: "Página _PAGE_ de _PAGES_",
                infoEmpty: "No hay datos",
                infoFiltered: "(Filtrado de _MAX_ líneas totales)"
            },
            columnDefs: [
                {
                    //targets: [0, 1, 2],
                    className: 'mdl-data-table__cell--non-numeric'
                }
            ]
        });
        var a = $('#selecc').val();
        table.page.len(a).draw();
        $('#selecc').on('change', function () {
            table.page.len(this.value).draw();
        });

        $('input.global_filter').on('keyup click', function () {
            filterGlobal();
        });
    } catch (e) {

    }
    try {
        var table2 = $('#table2').DataTable({
            //scrollY: "70vh",
            scrollX: "10vh",
            language: {
                lengthMenu: "Display _MENU_ records per page",
                zeroRecords: "No se encontraron datos",
                info: "Página _PAGE_ de _PAGES_",
                infoEmpty: "No hay datos",
                infoFiltered: "(Filtrado de _MAX_ líneas totales)"
            },
            columnDefs: [
                {

                    //targets: [0, 1, 2],
                    className: 'mdl-data-table__cell--non-numeric'
                }
            ]
        });
        var b = $('#selecc2').val();
        table2.page.len(b).draw();
        $('#selecc2').on('change', function () {
            table2.page.len(this.value).draw();
        });

        $('#serch').on('keyup click', function () {
            filterGlobal2();
        });
    } catch (e) {

    }
    try {
        var table3 = $('#table3').DataTable({
            //scrollY: "70vh",
            scrollX: "10vh",
            language: {
                //lengthMenu: "Display _MENU_ records per page",
                //zeroRecords: "No se encontraron datos",
                //info: "Página _PAGE_ de _PAGES_",
                //infoEmpty: "No hay datos",
                //infoFiltered: "(Filtrado de _MAX_ líneas totales)"
                "url": "../Scripts/lang/@Session["spras"].ToString()"+ ".json"
            },
            columnDefs: [
                {

                    //targets: [0, 1, 2],
                    className: 'mdl-data-table__cell--non-numeric'
                }
            ]
        });
    } catch (e) {

    }
    $('#formulario ').submit(function (e) {
        var value = $(document.activeElement).val();
        if ($(document.activeElement).val() == 'Cargar') {
            var concatValorS = 0;
            var concatValorP = 0;
            var concatValorA = 0;
            var mostrar = false;
            var mostrarG = false;
            if (acept == false) {
                if ($('#CPT').val() != '') {
                    var totalsoccpt = $('[name="sociedadcpt"] option').length;
                    $('[name="sociedadcpt"] option').filter(':selected').each(function () {
                        if ($(this).val() != "") {
                            concatValorS++;
                        }
                    });
                    var totalpercpt = $('[name="periodocpt"] option').length;
                    $('[name="periodocpt"] option').filter(':selected').each(function () {
                        if ($(this).val() != "") {
                            concatValorP++;
                        }
                    });
                    var totalanicpt = $('[name="aniocpt"] option').length;
                    $('[name="aniocpt"] option').filter(':selected').each(function () {
                        if ($(this).val() != "") {
                            concatValorA++;
                        }
                    });
                    if (concatValorS == 0 || concatValorP == 0 || concatValorA == 0) {
                        $('#mensaje').text("Seleccione una opcion en Sociedad, Año y Periodo para carga CPT");
                        $('#footer').html('<a class="modal-action modal-close waves-effect waves-green btn-flat">OK</a>');
                        $('#dialogo').trigger('click');
                        e.preventDefault();
                        return;
                    }
                    if (totalsoccpt == concatValorS && totalpercpt == concatValorP) {
                        $('#mensaje').text("Se cargaran todos los datos de las sociedaes y periodos seleccionados,\n ¿Desea continuar?");
                        mostrar = true;
                    }
                    else {
                        if (totalsoccpt == concatValorS) {
                            $('#mensaje').text("Se cargaran todos los datos de las sociedaes seleccionadas,\n ¿Desea continuar?");
                            mostrar = true;
                        }
                        if (totalpercpt == concatValorP) {
                            $('#mensaje').text("Se cargaran todos los datos de los periodos seleccionados,\n ¿Desea continuar?");
                            mostrar = true;
                        }
                    }
                    if (mostrar) {
                        $('#footer').html('<a id="acept" class="modal-action modal-close waves-effect waves-green btn-flat">Aceptar</a>' +
                            ' <a id="cance" class="modal-action modal-close waves-effect waves-red btn-flat">Cancelar</a>');
                        $('#dialogo').trigger('click');
                        e.preventDefault();
                    }
                }
                else {
                    if ($('#SAP').val() != '') {
                        $('[name="sociedadsap"] option').filter(':selected').each(function () {
                            if ($(this).val() != "") {
                                concatValorS++;
                            }
                        });
                        $('[name="periodosap"] option').filter(':selected').each(function () {
                            if ($(this).val() != "") {
                                concatValorP++;
                            }
                        });
                        if (concatValorS == 0 || concatValorP == 0) {
                            $('#mensaje').text("Seleccione una opcion en Sociedad y Presupuesto para carga SAP");
                            $('#footer').html('<a class="modal-action modal-close waves-effect waves-green btn-flat">OK</a>');
                            $('#dialogo').trigger('click');
                            e.preventDefault();
                            return;
                        }
                    }
                    else {
                        $('#mensaje').text("Agrege archivo CPT o archivo SAP");
                        $('#footer').html('<a class="modal-action modal-close waves-effect waves-green btn-flat">OK</a>');
                        $('#dialogo').trigger('click');
                    }
                }
            }
            else {
                acept = false;
                document.getElementById("loader").style.display = "initial";
            }
        }
        else {
            document.getElementById("loader").style.display = "initial";
        }
    });
    $('select').select();
    $(".f > .select-wrapper > .select-dropdown").prepend(
        '<li style="display:none" class="toggle selectnone"><span><label></label>Select none</span></li>'
    );
    $(".f > .select-wrapper > .select-dropdown").prepend(
        '<li  class="toggle selectall"><span><label></label>Select all</span></li>'
    );
    $(".f > .select-wrapper > .select-dropdown .selectall").on(
        "click",
        function () {
            var id = '[name=' + $(this).parent().parent().parent().attr('name') + ']';
            $(id + ' option:not(:disabled)')
                .not(':selected')
                .prop('selected', true);

            $(id + ' .dropdown-content.multiple-select-dropdown input[type="checkbox"]:not(:checked)'
            )
                .not(':disabled')
                .prop('checked', 'checked');
            //$('.dropdown-content.multiple-select-dropdown input[type='checkbox']:not(:checked)').not(':disabled').trigger('click');
            var values = $(id + ' .dropdown-content.multiple-select-dropdown input[type="checkbox"]:checked')
                .not(':disabled')
                .parent()
                .map(function () {
                    return $(this).text();
                })
                .get();
            $(id + ' input.select-dropdown').val(values.join(', '));
            $(id + '> .select-wrapper > .select-dropdown .toggle').toggle();
        }
    );
    $(".f > .select-wrapper > .select-dropdown .selectnone").on(
        "click",
        function () {
            var id = '[name=' + $(this).parent().parent().parent().attr('name') + ']';
            $(id + ' option:selected')
                .not(':disabled')
                .prop('selected', false);
            $(id + ' .dropdown-content.multiple-select-dropdown input[type="checkbox"]:checked')
                .not(':disabled')
                .prop('checked', '');
            //$('.dropdown-content.multiple-select-dropdown input[type='checkbox']:checked').not(':disabled').trigger('click');
            var values = $(id + ' .dropdown-content.multiple-select-dropdown input[type="checkbox"]:disabled')
                .parent()
                .text();
            $(id + ' input.select-dropdown').val(values);
            $(id + ' > .select-wrapper > .select-dropdown .toggle').toggle();
        }
    );
});