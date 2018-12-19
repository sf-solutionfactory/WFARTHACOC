
//function EnterKeyFilter() {
//    var enter = window.event.target.id;

//    if (window.event.keyCode == 13) {

//        if (enter == "monto_doc_md") {

//                //e.preventDefault(); //stops default action: submitting form
//                //var msg = 'Enter';
//                //M.toast({ html: msg })

//                var monto_doc_md = $('#monto_doc_md').val();
//                var mt = parseFloat(monto_doc_md.replace(',', '.')).toFixed(2);
//                if (mt > 0) {
//                    //Obtener la moneda en la lista
//                    //var MONEDA_ID = $('#moneda_id').val();

//                    //selectTcambio(MONEDA_ID, mt);
//                    var tipo_cambio = $('#tipo_cambio').val();
//                    var tc = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
//                    //Validar el monto en tipo de cambio
//                    if (tc > 0) {

//                        var monto = mt / tc;
//                        monto = parseFloat(monto).toFixed(2);
//                        $('#monto_doc_ml2').val(monto);
//                        $('#montos_doc_ml2').val(monto);
//                        $("label[for='montos_doc_ml2']").addClass("active");
//                    }else {
//                            $('#monto_doc_ml2').val(monto);
//                            $('#montos_doc_ml2').val(monto);
//                            var msg = 'Tipo de cambio incorrecto';
//                            M.toast({ html: msg })
//                    }

//                    } else {
//                        $('#monto_doc_ml2').val(monto);
//                        $('#montos_doc_ml2').val(monto);
//                    var msg = 'Monto incorrecto';
//                    M.toast({ html: msg })
//                }

//        }
//        if (enter == "tipo_cambio") {
//            var tipo_cambio = $('#tipo_cambio').val()
//            var tc = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
//            //Validar el monto en tipo de cambio
//            if (tc > 0) {
//                //Validar el monto
//                var monto_doc_md = $('#monto_doc_md').val();
//                var mt = parseFloat(monto_doc_md.replace(',', '.')).toFixed(2);
//                if (mt > 0) {
//                    //Validar la moneda
//                    var moneda_id = $('#moneda_id').val();
//                    if (moneda_id != null && moneda_id != "") {
//                        $('#monto_doc_ml2').val();
//                        $('#montos_doc_ml2').val();

//                        //Los valores son correctos, proceso para generar nuevo monto
//                        var monto = mt / tc;
//                         monto = parseFloat(monto).toFixed(2);
//                        $('#monto_doc_ml2').val(monto);
//                        $('#montos_doc_ml2').val(monto);
//                        $("label[for='montos_doc_ml2']").addClass("active");

//                    } else {
//                        $('#monto_doc_md').val();                        
//                        $('#monto_doc_ml2').val(monto);
//                        $('#montos_doc_ml2').val(monto);
//                        var msg = 'Moneda incorrecta';
//                        M.toast({ html: msg })
//                    }                    

//                } else {
//                    $('#monto_doc_md').val();
//                    $('#tipo_cambio').val("");
//                    $('#monto_doc_ml2').val(monto);
//                    $('#montos_doc_ml2').val(monto);
//                    var msg = 'Monto incorrecto';
//                    M.toast({ html: msg })
//                }

//            } else {
//                $('#monto_doc_ml2').val(monto);
//                $('#montos_doc_ml2').val(monto);
//                var msg = 'Tipo de cambio incorrecto';
//                M.toast({ html: msg })
//            }

//        }

//        event.returnValue = false;
//        event.cancel = true;
//    }

//}