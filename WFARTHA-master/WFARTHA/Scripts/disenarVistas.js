$(document).ready(function () {
    ajustarTaman();
});
function ajustarTaman() {
    //TEXTO
    var t_ret = $("#table_info>thead>tr").find('th');
    t_ret.css("text-align", "center");
}