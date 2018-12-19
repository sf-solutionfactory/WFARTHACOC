$(document).ready(function () {

});
//Variables globales
var reng2 = tRet2.length;
var reng = tRet.length;

$('body').on('focusout', '.extrasC', function (e) {
   // updateretenciones($(this).val(), $(this));
    //var tr = $(this);
    //var y = parseFloat($(this).val());
    //var i = 0;
    //var _t = $('#table_ret');
    //$("#table_info > tbody > tr[role = 'row']").each(function (index) {
    //    for (x = 0; x < tRet2.length; x++) {
    //        var _var = "BaseImp" + x;
    //        if (tr.hasClass(_var)) {
    //            var celdaval = $('#table_ret').DataTable().find("td.BIMPONIBLE").eq(x).text().replace('$', '');
    //            var txbi = $.trim(celdaval);
    //            var sum = parseFloat(txbi);
    //            sum = parseFloat(sum + y).toFixed(2);
    //            $_t.find("table_ret tbody td.BIMPONIBLE").eq(x).text('$' + sum);
    //            $_t.find("table_ret tbody td.BIMPONIBLE").eq(x + 2).text('$' + sum);
    //        }
    //    }
    //});
});
$('body').on('focusout', '.extrasC2', function (e) {
    var y = parseFloat($(this).val());
    $("#table_ret > tbody > tr[role = 'row']").each(function (index) {
        var ren = index % 2;
        if (ren == 0) {
            var celdaval = $(this).find("td.IMPRET").text().replace('$', '');
            var txbi = $.trim(celdaval);
            var sum = parseFloat(txbi);
            sum = parseFloat(sum + y).toFixed(2);
            $(this).find("td.IMPRET").text('$' + sum);
        }
    });
});