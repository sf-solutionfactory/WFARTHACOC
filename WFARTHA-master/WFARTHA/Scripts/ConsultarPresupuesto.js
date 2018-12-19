//$(document).ready(function () {
function jsConsulta(idioma) {
    var arrFiltr = ['canalFltr', 'totCanFltr', 'bannerFltr', 'totBannerFltr', 'periodoFltr', 'VVX17Fltr', 'CSHDCFltr', 'RECUNFltr', 'DSTRBFltr', 'OTHTAFltr', 'ADVERFltr', 'CORPMFltr', 'POPFltr', 'PMVARFltr', 'CONPRFltr', 'RSRDVFltr', 'SPAFltr', 'FREEGFltr', 'ALLBFltr', 'ALLFFltr', 'PROCESOFltr', 'consumidoFltr', 'disponibleFltr'];
    var ids = ['id']

    $('.collapsible').collapsible();
    try {
        var table = $('#table').DataTable({
            scrollY: "200",
            scroller: true,
            "autoWidth": false,
            dom: "Bfrtip",
            scrollX: "10vh",
            language: {
                //lengthMenu: "Display _MENU_ records per page",
                //zeroRecords: "No se encontraron datos",
                //info: "Página _PAGE_ de _PAGES_",
                //infoEmpty: "No hay datos",
                //infoFiltered: "(Filtrado de _MAX_ líneas totales)"
                "url": "../Scripts/lang/" + idioma + ".json"
            },
            "fixedHeader": {
                header: true,
                position: 'fixed'
            },
            //"ScrollX": true,
            "scrollCollapse": true,
            "sScrollY": 550,
            columnDefs: [
                {
                    className: 'mdl-data-table__cell--non-numeric',
                    width: "100%"
                }
            ],
            initComplete: function () {
                for (var i = 0; i <= 22; i++) {
                    try {
                        this.api().columns([i]).every(function () {
                            var column = this;
                            //console.log(column);
                            var select = $("#" + arrFiltr[i]);
                            column.data().unique().sort().each(function (d, j) {
                                select.append('<option value="' + d + '">' + d + '</option>')
                            });
                        });
                    } catch (e) {
                        console.log(e);
                    }
                }
            },
            "footerCallback": function (row, data, start, end, display) {//suma de totales por columna en pie de tabla footer
                var api = this.api();
                var intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };
                var currency = function (value) {
                    return value.replace(/\D/g, "")
                        .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ",");
                };
                for (var j = 0; j <= 22; j++) {
                    if (j > 4) {
                        try {
                            api.columns([j], { page: 'current' }).every(function () {
                                var sum = this
                                    .data()
                                    .reduce(function (a, b) {
                                        return intVal(a) + intVal(b);
                                    }, 0);
                                $(this.footer()).html("$" + currency(sum.toString()));
                            });
                        } catch (e) {
                            console.log(e);
                        }
                    }
                }
                //$('[name="filtro"] option').remove();
                for (var i = 0; i <= 22; i++) {
                    try {
                        this.api().columns([i], { page: 'current' }).every(function () {
                            var column = this;
                            //console.log(column);                            
                            if (existe(arrFiltr[i]) === false) {
                                $("#" + arrFiltr[i] + ' option').remove();
                                var select = $("#" + arrFiltr[i]);
                                column.data().unique().sort().each(function (d, j) {
                                    select.append('<option value="' + d + '">' + d + '</option>')
                                });
                            }
                        });
                    } catch (e) {
                        console.log(e);
                    }
                }
                $("select").select();
                $(".f > .select-wrapper > .select-dropdown").prepend(
                    '<li class="toggle selectnone"><span><label></label>Select none</span></li>'
                );
                $(".f > .select-wrapper > .select-dropdown").prepend(
                    '<li class="toggle selectall"><span><label></label>Select all</span></li>'
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
                        //$(id + '> .select-wrapper > .select-dropdown .toggle').toggle();
                        var ido = $(this, ' select').parent().parent().parent().attr('name');
                        cambio(ido);
                        //$('[name="filtro"]').trigger('change');
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
                        //$(id + ' > .select-wrapper > .select-dropdown .toggle').toggle();
                        var ido = $(this, ' select').parent().parent().parent().attr('name');
                        cambio(ido);
                        //$('[name="filtro"]').trigger('change');
                    }
                );
                $('th').css({
                    'text-align': 'center',

                });

            },
            //fixedColumns: true,
            fixedColumns: {
                leftColumns: 5,
                //width: '100'
            },
            //"columnDefs": [
            //    { "width": "1000%" }
            //]
        });
        //table.columns.adjust().draw();
        $('[name="filtro"]').on('change', function () {//filtro de busqueda por columna
            var col = 0;
            var index = ids.indexOf($(this, ' select').attr('id'));
            if (index === -1) {
                ids.push($(this, ' select').attr('id'));
            }
            col = $(this).attr('col');
            var search = new Array();
            var sech = ""
            $(this, ' option:selected').each(function () {
                search = $(this).val();
            });
            if (search.length === 0) {
                index = ids.indexOf($(this, ' select').attr('id'));
                if (index > -1) {
                    //delete ids[index];
                    ids.splice(index, 1);
                }
            }
            sech = search.join('|');
            sech = sech.replace(/<br>/g, '');
            sech = sech.replace(/\$/g, "");
            table.column(col).search(sech, true, false).draw();
        });

        //$('[class="input-field col s2 f"]').on('dblclick', function () { //marcar y desmarcar selec con doble clic
        //    var id = '#' + $(this).attr('name');
        //    var col = $(id).attr('col');
        //    id += ' option';
        //    if (arrFiltr[col] == '') {
        //        $(id).each(function () {
        //            $(this).removeAttr("selected");
        //        });
        //        arrFiltr[col] = 'X';
        //    } else {
        //        $(id).each(function () {
        //            $(this).attr('selected', '');
        //        });
        //        arrFiltr[col] = '';
        //    }
        //    $('select').select();

        //});
        //$('label').attr('unselectable', 'on') /*quitar seleccion de texto por mouse */
        //     .css({
        //         '-moz-user-select': '-moz-none',
        //         '-moz-user-select': 'none',
        //         '-o-user-select': 'none',
        //         '-khtml-user-select': 'none', /* you could also put this in a class */
        //         '-webkit-user-select': 'none',/* and add the CSS class here instead */
        //         '-ms-user-select': 'none',
        //         'user-select': 'none'
        //     }).bind('selectstart', function () { return false; });

        var a = $('#selecc').val();
        table.page.len(a).draw();
        $('#selecc').on('change', function () {
            table.page.len(this.value).draw();
        });

        $('input.global_filter').on('keyup click', function () {
            filterGlobal();
        });
        $('select').select();
        M.Select.init($('select'), []);

        //$('#chkfiltro').on('click', function () {
        //    if ($(this).is(':checked')) {
        //        // Hacer algo si el checkbox ha sido seleccionado
        //        $('[name="filtro"] option').each(function () {
        //            $(this).attr('selected', '');
        //        });
        //        selectAll();
        //    } else {
        //        // Hacer algo si el checkbox ha sido deseleccionado
        //        $('[name="filtro"] option').each(function () {
        //            $(this).removeAttr("selected");
        //        });
        //        selectNone();
        //    }
        //    var elem = document.getElementsByName("periodocpt")
        //    //instance = M.Select.init(elem, []);
        //    //$('select').select();
        //});
        //function selectNone() {
        //    $('[name="filtro"] option:selected')
        //        .not(':disabled')
        //        .prop('selected', false);
        //    $('.dropdown-content.multiple-select-dropdown input[type="checkbox"]:checked')
        //        .not(':disabled')
        //        .prop('checked', '');
        //    //$('.dropdown-content.multiple-select-dropdown input[type='checkbox']:checked').not(':disabled').trigger('click');
        //    var values = $(
        //        '.dropdown-content.multiple-select-dropdown input[type="checkbox"]:disabled'
        //    )
        //        .parent()
        //        .text();
        //    $('input.select-dropdown').val(values);
        //    $('.f > .select-wrapper > .select-dropdown .toggle').toggle();
        //    $('[name="filtro"]').trigger('change');

        //}

        //function selectAll() {
        //    $('[name="filtro"] option:not(:disabled)').each(function () {
        //        var id = '[name=' + $(this).parent().parent().parent().attr('name') + ']';
        //        $(id + ' select option:not(:disabled)')
        //            .not(':selected')
        //            .prop('selected', true);
        //        $(id + ' .dropdown-content.multiple-select-dropdown input[type="checkbox"]:not(:checked)'
        //        )
        //            .not(':disabled')
        //            .prop('checked', 'checked');
        //        //$('.dropdown-content.multiple-select-dropdown input[type='checkbox']:not(:checked)').not(':disabled').trigger('click');
        //        var values = $(id + ' .dropdown-content.multiple-select-dropdown input[type="checkbox"]:checked'
        //        )
        //            .not(':disabled')
        //            .parent()
        //            .map(function () {
        //                return $(this).text();
        //            })
        //            .get();
        //        $(id + ' input.select-dropdown').val(values.join(', '));
        //        //console.log($('select').val());
        //        $(id + ' > .select-wrapper > .select-dropdown .toggle').toggle();
        //    });
        //    $('[name="filtro"]').trigger('change');
        //}
    } catch (e) {
        console.log(e);
    }
    function cambio(id) {//filtro de busqueda por columna
        var col = 0;
        var index = ids.indexOf(id);
        if (index === -1) {
            ids.push(id);
        }
        col = $('#' + id).attr('col');
        var search = new Array();
        var sech = ""
        $('#' + id + ' option:selected').each(function () {
            search.push($(this).val());
        });
        if (search.length === 0) {
            index = ids.indexOf(id);
            if (index > -1) {
                //delete ids[index];
                ids.splice(index, 1);
            }
        }
        sech = search.join('|');
        sech = sech.replace(/<br>/g, '');
        table.column(col).search(sech, true, false).draw();
    };
    function existe(a) {
        var res = false;
        for (var i = 0; i < ids.length; i++) {
            if (a === ids[i]) {
                res = true;
            }
        }
        return res;
    }
    $("select").select();

    $(".f > .select-wrapper > .select-dropdown").prepend(
        '<li class="toggle selectnone"><span><label></label>Select none</span></li>'
    );
    $(".f > .select-wrapper > .select-dropdown").prepend(
        '<li class="toggle selectall"><span><label></label>Select all</span></li>'
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
            //$(id + '> .select-wrapper > .select-dropdown .toggle').toggle();
            var ido = $(this, ' select').parent().parent().parent().attr('name');
            cambio(ido);
            //$('[name="filtro"]').trigger('change');
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
            //$(id + ' > .select-wrapper > .select-dropdown .toggle').toggle();
            var id0 = $(this, ' select').parent().parent().parent().attr('name');
            cambio(ido);
            //$('[name="filtro"]').trigger('change');
        }
    );
    $('th').css({
        'text-align': 'center',
    });
    //'width': '350px'});//porque no se deja de otra forma
    $('td').css('text-align', 'center');

//});
}