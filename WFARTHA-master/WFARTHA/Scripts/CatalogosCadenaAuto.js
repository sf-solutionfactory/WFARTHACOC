$(document).ready(function () {
//Tabla de autorizadores
    $('#tableca').DataTable({
    language: {
        "url": "../Scripts/lang/ES.json"
    },
    "paging": false,
    "info": false,
    "searching": false,
    "scrollX": false,
    "ordering": false,
    "columns": [
        {
            "name": 'Agente',
            "className": 'AGENTE',
            "orderable": false
        },
        {
            "name": 'Fase',
            "className": 'FASE',
            "orderable": false
        },
        {
            "name": 'Accion',
            "className": 'ACCION',
            "orderable": false
        }
    ]
    });

    $('#btn_addagente').on('click', function () {

        //Obtener el agente
        var agente = $('#AGENTE_LISTV').val();

        if (agente != null && agente != "") {
            addRow(agente);
        }
        
    });

});

$('body').on('click', '.deleterow', function (e) {

    //Obtener el renglón
    var tr = $(this).closest('tr');
    //Obtener la tabla
    var t = $('#tableca').DataTable();

    //Eliminar el renglón
    t.rows(tr).remove().draw(false);

    //Actualizar las fases en la tabla
    updateFase();

});

function addRow(agente) {
    var t = $('#tableca').DataTable();
    var addedRow = "";
    addedRow = addRowAu(t, agente);
    updateFase();
}

function addRowAu(t, agente) {

    var r = addRowl(
        t,
        agente,
        "",
        "<a href=\"\"><i class=\"material-icons red-text deleterow\">delete</i></a>"
    );

    return r;
}

function addRowl(t, agente, pos, icon) {
   
        var r = t.row.add([
            agente,
            pos,
            icon
        ]).draw(false).node();
    
    return r;
}



function updateFase() {
    var i = 1;
    $("#tableca > tbody > tr[role = 'row']").each(function (index) {
        //var col11 = $(this).find("td.TOTAL input").val();
        $(this).find("td.FASE").text("");
        $(this).find("td.FASE").text(i);

        i++;
    });
}

