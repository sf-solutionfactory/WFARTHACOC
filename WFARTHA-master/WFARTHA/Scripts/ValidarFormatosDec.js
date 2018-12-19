$(document).ready(function () {
    var _miles = $("#miles").val();
    var _decimales = $("#dec").val();
    var _ctlCount = $('#ctlDis tr').length;
    //LEJ 12.07.2018------------------------------
    for (var i = 0; i < _ctlCount - 1; i++) {
        if (_decimales === '.') {
            //Recuperamos los valores con jquery
            var _dsMn = $("#dsMn-" + i).text();
            var _dsApo = $("#dsApo-" + i).text();
            var _dsPS = $("#dsPS-" + i).text();
            var _dsAE = $("#dsAE-" + i).text();
            var _dsAR = $("#dsAR-" + i).text();
            var _dsPAp = $("#dsPAp-" + i).text().replace('%', '');
            var _dsVE = $("#dsVE-" + i).text();
            var _dsVR = $("#dsVR-" + i).text();

            //Separo enteros[0] y decimales[1]
            var _dsMnA = _dsMn.split('.');
            var _dsApoA = _dsApo.split('.');
            var _dsPSA = _dsPS.split('.');
            var _dsAEA = _dsAE.split('.');
            var _dsARA = _dsAR.split('.');
            var _dsPapA = _dsPAp.split('.');
            var _dsVEA = _dsVE.split('.');
            var _dsVRA = _dsVR.split('.');

            //La siguiente linea agrega la coma(,) o el(.) cada que sea unidad de millar
            var _millesdsMn = _dsMnA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            var _millesdsApo = _dsApoA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            var _millesdsPs = _dsPSA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            var _millesdsAe = _dsAEA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            var _millesdsAr = _dsARA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            var _millesdsPap = _dsPapA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            var _millesdsVE = _dsVEA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            var _millesdsVR = _dsVRA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

            //junto la cantidad separada por millar junto con su parte decimal
            var dsMn = _millesdsMn + "." + _dsMnA[1];
            var dsApo = _millesdsApo + "." + _dsApoA[1];
            var dsPS = _millesdsPs + "." + _dsPSA[1];
            var dsAE = _millesdsAe + "." + _dsAEA[1];
            var dsAR = _millesdsAr + "." + _dsARA[1];
            var dsPAp = _millesdsPap + "." + _dsPapA[1];
            var dsVE = _millesdsVE + "." + _dsVEA[1];
            var dsVR = _millesdsVR + "." + _dsVRA[1];

            //compruebo si es un valor negativo
            if (dsMn.indexOf("-") >= 0) {
                var _dsMnx = dsMn;
                _dsMnx = _dsMnx.replace('-', '(');
                _dsMnx += ")";
                dsMn = _dsMnx;
            }
            if (dsApo.indexOf("-") >= 0) {
                var _dsApox = dsApo;
                _dsApox = _dsApox.replace('-', '(');
                _dsApox += ")";
                dsApo = _dsApox;
            }
            if (dsPS.indexOf("-") >= 0) {
                var _dsPSx = dsPS;
                _dsPSx = _dsPSx.replace('-', '(');
                _dsPSx += ")";
                dsPS = _dsPSx;
            }
            if (dsAE.indexOf("-") >= 0) {
                var _dsAEx = dsAE;
                _dsAEx = _dsAEx.replace('-', '(');
                _dsAEx += ")";
                dsAE = _dsAEx;
            }
            if (dsAR.indexOf("-") >= 0) {
                var _dsARx = dsAR;
                _dsARx = _dsARx.replace('-', '(');
                _dsARx += ")";
                dsAR = _dsARx;
            }
            if (dsPAp.indexOf("-") >= 0) {
                var _dsAEx = dsPAp;
                _dsAEx = _dsAEx.replace('-', '(');
                _dsAEx += ")";
                dsPAp = _dsAEx;
            }
            if (dsVE.indexOf("-") >= 0) {
                var _dsARx = dsVE;
                _dsARx = _dsARx.replace('-', '(');
                _dsARx += ")";
                dsVE = _dsARx;
            }
            if (dsVR.indexOf("-") >= 0) {
                var _dsARx = dsVR;
                _dsARx = _dsARx.replace('-', '(');
                _dsARx += ")";
                dsVR = _dsARx;
            }

            //Reasignamos los valores
            $("#dsMn-" + i).text("$" + dsMn);
            $("#dsApo-" + i).text("$" + dsApo);
            $("#dsPS-" + i).text("$" + dsPS);
            $("#dsAE-" + i).text("$" + dsAE);
            $("#dsAR-" + i).text("$" + dsAR);
            $("#dsPAp-" + i).text(dsPAp + "%");
            $("#dsVE-" + i).text(dsVE);
            $("#dsVR-" + i).text(dsVR);
        }
        else if (_decimales === ',') {
            //Recuperamos los valores con jquery
            var _dsMn = $("#dsMn-" + i).text();
            var _dsApo = $("#dsApo-" + i).text();
            var _dsPS = $("#dsPS-" + i).text();
            var _dsAE = $("#dsAE-" + i).text();
            var _dsAR = $("#dsAR-" + i).text();
            var _dsPAp = $("#dsPAp-" + i).text().replace('%', '');
            var _dsVE = $("#dsVE-" + i).text();
            var _dsVR = $("#dsVR-" + i).text();

            //Separo enteros[0] y decimales[1]
            var _dsMnA = _dsMn.split('.');
            var _dsApoA = _dsApo.split('.');
            var _dsPSA = _dsPS.split('.');
            var _dsAEA = _dsAE.split('.');
            var _dsARA = _dsAR.split('.');
            var _dsPapA = _dsPAp.split('.');
            var _dsVEA = _dsVE.split('.');
            var _dsVRA = _dsVR.split('.');

            //La siguiente linea agrega la coma(,) o el(.) cada que sea unidad de millar
            var _millesdsMn = _dsMnA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
            var _millesdsApo = _dsApoA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
            var _millesdsPs = _dsPSA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
            var _millesdsAe = _dsAEA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
            var _millesdsAr = _dsARA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
            var _millesdsPap = _dsPapA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
            var _millesdsVE = _dsVEA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
            var _millesdsVR = _dsVRA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");

            //junto la cantidad separada por millar junto con su parte decimal
            var dsMn = _millesdsMn + "," + _dsMnA[1];
            var dsApo = _millesdsApo + "," + _dsApoA[1];
            var dsPS = _millesdsPs + "," + _dsPSA[1];
            var dsAE = _millesdsAe + "," + _dsAEA[1];
            var dsAR = _millesdsAr + "," + _dsARA[1];
            var dsPAp = _millesdsPap + "," + _dsPapA[1];
            var dsVE = _millesdsVE + "," + _dsVEA[1];
            var dsVR = _millesdsVR + "," + _dsVRA[1];

            //compruebo si es un valor negativo           
            //--
            if (dsMn.indexOf("-") >= 0) {
                var _dsMnx = dsMn;
                _dsMnx = _dsMnx.replace('-', '(');
                _dsMnx += ")";
                dsMn = _dsMnx;
            }
            if (dsApo.indexOf("-") >= 0) {
                var _dsApox = dsApo;
                _dsApox = _dsApox.replace('-', '(');
                _dsApox += ")";
                dsApo = _dsApox;
            }
            if (dsPS.indexOf("-") >= 0) {
                var _dsPSx = dsPS;
                _dsPSx = _dsPSx.replace('-', '(');
                _dsPSx += ")";
                dsPS = _dsPSx;
            }
            if (dsAE.indexOf("-") >= 0) {
                var _dsAEx = dsAE;
                _dsAEx = _dsAEx.replace('-', '(');
                _dsAEx += ")";
                dsAE = _dsAEx;
            }
            if (dsAR.indexOf("-") >= 0) {
                var _dsARx = dsAR;
                _dsARx = _dsARx.replace('-', '(');
                _dsARx += ")";
                dsAR = _dsARx;
            }
            if (dsPAp.indexOf("-") >= 0) {
                var _dsAEx = dsPAp;
                _dsAEx = _dsAEx.replace('-', '(');
                _dsAEx += ")";
                dsPAp = _dsAEx;
            }
            if (dsVE.indexOf("-") >= 0) {
                var _dsARx = dsVE;
                _dsARx = _dsARx.replace('-', '(');
                _dsARx += ")";
                dsVE = _dsARx;
            }
            if (dsVR.indexOf("-") >= 0) {
                var _dsARx = dsVR;
                _dsARx = _dsARx.replace('-', '(');
                _dsARx += ")";
                dsVR = _dsARx;
            }

            //Reasignamos los valores
            $("#dsMn-" + i).text("$" + dsMn);
            $("#dsApo-" + i).text("$" + dsApo);
            $("#dsPS-" + i).text("$" + dsPS);
            $("#dsAE-" + i).text("$" + dsAE);
            $("#dsAR-" + i).text("$" + dsAR);
            $("#dsPAp-" + i).text(dsPAp + "%");
            $("#dsVE-" + i).text(dsVE);
            $("#dsVR-" + i).text(dsVR);
        }
    }
    //LEJ 12.07.2018------------------------------
    if (_decimales === '.') {
        //Recuperamos los valores con jquery
        var _mt = $("._vmonto").val();
        var _tc = $("._tc").val();
        var _mt2 = $("._vmonto2").val();//
        var _pcanal = $("#p_canal").text().replace('$', '');
        var _pbanner = $("#p_banner").text().replace('$', '');
        var _pcc = $("#pc_c").text().replace('$', '');
        var _pca = $("#pc_a").text().replace('$', '');
        var _pcp = $("#pc_p").text().replace('$', '');
        var _pct = $("#pc_t").text().replace('$', '');
        var _consu = $("#consu").text().replace('$', '');//

        //Separo enteros[0] y decimales[1]
        var _mtA = _mt.split('.');
        var _tcA = _tc.split('.');
        var _mtA2 = _mt2.split('.');
        var _pcanalA = _pcanal.split('.');
        var _pbannerA = _pbanner.split('.');
        var _pccA = _pcc.split('.');
        var _pcaA = _pca.split('.');
        var _pcpA = _pcp.split('.');
        var _pctA = _pct.split('.');
        var _consuA = _consu.split('.');//--

        //La siguiente linea agrega la coma(,) o el(.) cada que sea unidad de millar
        var _millesMt = _mtA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        var _millesTc = _tcA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        var _millesMt2 = _mtA2[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");//--
        var _millespcanal = _pcanalA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        var _millespbanner = _pbannerA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        var _millespcc = _pccA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        var _millespca = _pcaA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        var _millespcp = _pcpA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        var _millespct = _pctA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        var _millesconsu = _consuA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");//--

        //junto la cantidad separada por millar junto con su parte decimal
        var _vmonto = _millesMt + "." + _mtA[1];
        var _tc = _millesTc + "." + _tcA[1];
        var _vmonto2 = _millesMt2 + "." + _mtA2[1];//--
        var p_canal = _millespcanal + "." + _pcanalA[1];
        var p_banner = _millespbanner + "." + _pbannerA[1];
        var pc_c = _millespcc + "." + _pccA[1];
        var pc_a = _millespca + "." + _pcaA[1];
        var pc_p = _millespcp + "." + _pcpA[1];
        var pc_t = _millespct + "." + _pctA[1];
        var consu = _millesconsu + "." + _consuA[1];//--

        //compruebo si es un valor negativo
        if (_vmonto.indexOf("-") >= 0) {
            var _vmx = _vmonto;
            _vmx = _vmx.replace('-', '(');
            _vmx += ")";
            _vmonto = _vmx;
        }
        if (_tc.indexOf("-") >= 0) {
            var _tcx = _tc;
            _tcx = _tcx.replace('-', '(');
            _tcx += ")";
            _tc = _tcx;
        }
        if (_vmonto2.indexOf("-") >= 0) {
            var _v2x = _vmonto2;
            _v2x = _v2x.replace('-', '(');
            _v2x += ")";
            _vmonto2 = _v2x;
        }
        if (p_canal.indexOf("-") >= 0) {
            var _pcanx = p_canal;
            _pcanx = _pcanx.replace('-', '(');
            _pcanx += ")";
            p_canal = _pcanx;
        }
        if (p_banner.indexOf("-") >= 0) {
            var _pbannerx = p_banner;
            _pbannerx = _pbannerx.replace('-', '(');
            _pbannerx += ")";
            p_banner = _pbannerx;
        }
        if (pc_c.indexOf("-") >= 0) {
            var _pccx = pc_c;
            _pccx = _pccx.replace('-', '(');
            _pccx += ")";
            pc_c = _pccx;
        }
        if (pc_a.indexOf("-") >= 0) {
            var _pcax = pc_a;
            _pcax = _pcax.replace('-', '(');
            _pcax += ")";
            pc_a = _pcax;
        }
        if (pc_p.indexOf("-") >= 0) {
            var _pcax = pc_p;
            _pcpx = _pcpx.replace('-', '(');
            _pcpx += ")";
            pc_p = _pcpx;
        }
        if (pc_t.indexOf("-") >= 0) {
            var _pctx = pc_t;
            _pctx = _pctx.replace('-', '(');
            _pctx += ")";
            pc_t = _pcpx;
        }
        if (consu.indexOf("-") >= 0) {
            var _cx = consu;
            _cx = _cx.replace('-', '(');
            _cx += ")";
            consu = _cx;
        }

        //Reasignamos los valores
        $("._vmonto").val("$" + _vmonto);
        $("._tc").val("$" + _tc);
        $("._vmonto2").val("$" + _vmonto2);
        $("#p_canal").text("$" + p_canal);
        $("#p_banner").text("$" + p_banner);
        $("#pc_c").text("$" + pc_c);
        $("#pc_a").text("$" + pc_a);
        $("#pc_p").text("$" + pc_p);
        $("#pc_t").text("$" + pc_t);
        $("#consu").text("$" + consu);

    }
    else if (_decimales === ',') {
        //Recuperamos los valores con jquery
        var _mt = $("._vmonto").val();
        var _tc = $("._tc").val();
        var _mt2 = $("._vmonto2").val();//
        var _pcanal = $("#p_canal").text().replace('$', '');
        var _pbanner = $("#p_banner").text().replace('$', '');
        var _pcc = $("#pc_c").text().replace('$', '');
        var _pca = $("#pc_a").text().replace('$', '');
        var _pcp = $("#pc_p").text().replace('$', '');
        var _pct = $("#pc_t").text().replace('$', '');
        var _consu = $("#consu").text().replace('$', '');//
        var _dsMn = $("#dsMn").text();
        var _dsApo = $("#dsApo").text();
        var _dsPS = $("#dsPS").text();
        var _dsAE = $("#dsAE").text();
        var _dsAR = $("#dsAR").text();
        var _dsPAp = $("#dsPAp").text().replace('%', '');//
        var _dsVE = $("#dsVE").text();
        var _dsVR = $("#dsVR").text();

        //Separo enteros[0] y decimales[1]
        var _mtA = _mt.split('.');
        var _tcA = _tc.split('.');
        var _mtA2 = _mt2.split('.');
        var _pcanalA = _pcanal.split('.');
        var _pbannerA = _pbanner.split('.');
        var _pccA = _pcc.split('.');
        var _pcaA = _pca.split('.');
        var _pcpA = _pcp.split('.');
        var _pctA = _pct.split('.');
        var _consuA = _consu.split('.');//--
        var _dsMnA = _dsMn.split('.');
        var _dsApoA = _dsApo.split('.');
        var _dsPSA = _dsPS.split('.');
        var _dsAEA = _dsAE.split('.');
        var _dsARA = _dsAR.split('.');
        var _dsPapA = _dsPAp.split('.');
        var _dsVEA = _dsVE.split('.');
        var _dsVRA = _dsVR.split('.');

        //La siguiente linea agrega la coma(,) o el(.) cada que sea unidad de millar
        var _millesMt = _mtA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesTc = _tcA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesMt2 = _mtA2[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");//--
        var _millespcanal = _pcanalA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millespbanner = _pbannerA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millespcc = _pccA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millespca = _pcaA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millespcp = _pcpA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millespct = _pctA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesconsu = _consuA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");//--
        var _millesdsMn = _dsMnA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesdsApo = _dsApoA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesdsPs = _dsPSA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesdsAe = _dsAEA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesdsAr = _dsARA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesdsPap = _dsPapA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesdsVE = _dsVEA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        var _millesdsVR = _dsVRA[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");

        //junto la cantidad separada por millar junto con su parte decimal
        var _vmonto = _millesMt + "," + _mtA[1];
        var _tc = _millesTc + "," + _tcA[1];
        var _vmonto2 = _millesMt2 + "," + _mtA2[1];//--
        var p_canal = _millespcanal + "," + _pcanalA[1];
        var p_banner = _millespbanner + "," + _pbannerA[1];
        var pc_c = _millespcc + "," + _pccA[1];
        var pc_a = _millespca + "," + _pcaA[1];
        var pc_p = _millespcp + "," + _pcpA[1];
        var pc_t = _millespct + "," + _pctA[1];
        var consu = _millesconsu + "," + _consuA[1];//--
        var dsMn = _millesdsMn + "," + _dsMnA[1];
        var dsApo = _millesdsApo + "," + _dsApoA[1];
        var dsPS = _millesdsPs + "," + _dsPSA[1];
        var dsAE = _millesdsAe + "," + _dsAEA[1];
        var dsAR = _millesdsAr + "," + _dsARA[1];
        var dsPAp = _millesdsPap + "," + _dsPapA[1];
        var dsVE = _millesdsVE + "," + _dsVEA[1];
        var dsVR = _millesdsVR + "," + _dsVRA[1];

        //compruebo si es un valor negativo
        if (_vmonto.indexOf("-") >= 0) {
            var _vmx = _vmonto;
            _vmx = _vmx.replace('-', '(');
            _vmx += ")";
            _vmonto = _vmx;
        }
        if (_tc.indexOf("-") >= 0) {
            var _tcx = _tc;
            _tcx = _tcx.replace('-', '(');
            _tcx += ")";
            _tc = _tcx;
        }
        if (_vmonto2.indexOf("-") >= 0) {
            var _v2x = _vmonto2;
            _v2x = _v2x.replace('-', '(');
            _v2x += ")";
            _vmonto2 = _v2x;
        }
        if (p_canal.indexOf("-") >= 0) {
            var _pcanx = p_canal;
            _pcanx = _pcanx.replace('-', '(');
            _pcanx += ")";
            p_canal = _pcanx;
        }
        if (p_banner.indexOf("-") >= 0) {
            var _pbannerx = p_banner;
            _pbannerx = _pbannerx.replace('-', '(');
            _pbannerx += ")";
            p_banner = _pbannerx;
        }
        if (pc_c.indexOf("-") >= 0) {
            var _pccx = pc_c;
            _pccx = _pccx.replace('-', '(');
            _pccx += ")";
            pc_c = _pccx;
        }
        if (pc_a.indexOf("-") >= 0) {
            var _pcax = pc_a;
            _pcax = _pcax.replace('-', '(');
            _pcax += ")";
            pc_a = _pcax;
        }
        if (pc_p.indexOf("-") >= 0) {
            var _pcax = pc_p;
            _pcpx = _pcpx.replace('-', '(');
            _pcpx += ")";
            pc_p = _pcpx;
        }
        if (pc_t.indexOf("-") >= 0) {
            var _pctx = pc_t;
            _pctx = _pctx.replace('-', '(');
            _pctx += ")";
            pc_t = _pcpx;
        }
        if (consu.indexOf("-") >= 0) {
            var _cx = consu;
            _cx = _cx.replace('-', '(');
            _cx += ")";
            consu = _cx;
        }//--
        if (dsMn.indexOf("-") >= 0) {
            var _dsMnx = dsMn;
            _dsMnx = _dsMnx.replace('-', '(');
            _dsMnx += ")";
            dsMn = _dsMnx;
        }
        if (dsApo.indexOf("-") >= 0) {
            var _dsApox = dsApo;
            _dsApox = _dsApox.replace('-', '(');
            _dsApox += ")";
            dsApo = _dsApox;
        }
        if (dsPS.indexOf("-") >= 0) {
            var _dsPSx = dsPS;
            _dsPSx = _dsPSx.replace('-', '(');
            _dsPSx += ")";
            dsPS = _dsPSx;
        }
        if (dsAE.indexOf("-") >= 0) {
            var _dsAEx = dsAE;
            _dsAEx = _dsAEx.replace('-', '(');
            _dsAEx += ")";
            dsAE = _dsAEx;
        }
        if (dsAR.indexOf("-") >= 0) {
            var _dsARx = dsAR;
            _dsARx = _dsARx.replace('-', '(');
            _dsARx += ")";
            dsAR = _dsARx;
        }
        if (dsPAp.indexOf("-") >= 0) {
            var _dsAEx = dsPAp;
            _dsAEx = _dsAEx.replace('-', '(');
            _dsAEx += ")";
            dsPAp = _dsAEx;
        }
        if (dsVE.indexOf("-") >= 0) {
            var _dsARx = dsVE;
            _dsARx = _dsARx.replace('-', '(');
            _dsARx += ")";
            dsVE = _dsARx;
        }
        if (dsVR.indexOf("-") >= 0) {
            var _dsARx = dsVR;
            _dsARx = _dsARx.replace('-', '(');
            _dsARx += ")";
            dsVR = _dsARx;
        }

        //Reasignamos los valores
        $("._vmonto").val("$" + _vmonto);
        $("._tc").val("$" + _tc);
        $("._vmonto2").val("$" + _vmonto2);
        $("#p_canal").text("$" + p_canal);
        $("#p_banner").text("$" + p_banner);
        $("#pc_c").text("$" + pc_c);
        $("#pc_a").text("$" + pc_a);
        $("#pc_p").text("$" + pc_p);
        $("#pc_t").text("$" + pc_t);
        $("#consu").text("$" + consu);
        $("#dsMn").text("$" + dsMn);
        $("#dsApo").text("$" + dsApo);
        $("#dsPS").text("$" + dsPS);
        $("#dsAE").text("$" + dsAE);
        $("#dsAR").text("$" + dsAR);
        $("#dsPAp").text(dsPAp + "%");
        $("#dsVE").text(dsVE);
        $("#dsVR").text(dsVR);
    }

    //Para que no ingrese letras ni signos extra
    $('body').on('keydown', '._vmonto', function (e) {
        if (_decimales === ".") {
            if (e.keyCode == 110 || e.keyCode == 190) {
                if ($(this).val().indexOf('.') != -1) {
                    e.preventDefault();
                }
            }
            else {  // Allow: backspace, delete, tab, escape, enter and .
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    // Allow: Ctrl+A, Command+A
                    (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything

                    return;
                }

                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            }
        }
        else if (_decimales === ",") {
            if (e.keyCode == 188) {
                if ($(this).val().indexOf(',') != -1) {
                    e.preventDefault();
                }
            }
            else {  // Allow: backspace, delete, tab, escape, enter and ','
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 188]) !== -1 ||
                    // Allow: Ctrl+A, Command+A
                    (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything

                    return;
                }

                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            }
        }
    });

    //Para validar los decimales del costo unitario
    $('body').on('focusout', '.input_dc', function () {
        var xx = $(this).val();
        if (_decimales == ",") {
            if (xx.indexOf(",") >= 0) {
                var _xx = xx;
                _xx = _xx.replace(',', '.');
                xx = _xx;
            }
        }
        if (xx.indexOf("$") >= 0) {
            var _xx2 = xx;
            _xx2 = _xx2.replace('$', '');
            xx = _xx2;
        }
        if (xx != '') {
            //Hace la conversion a 2 decimales
            var _re = parseFloat(xx).toFixed(2);
            //if para decimales es una coma es el siguiente proceso
            if (_decimales == ",") {
                if (_re.indexOf(".") >= 0) {
                    var _rex = _re;
                    _rex = _rex.replace('.', ',');
                    _re = _rex;
                }
            }
            $(this).val("$" + _re.toString().replace(/\B(?=(\d{3})+(?!\d))/g, _miles));
        }
        else {
            $(this).val($(this).val());
        }
    });

    //LEJ 12.07.2018----------------------------------------
    //Para cuando clicke el checkbox
    $("input[type=checkbox]").on("click", function (e) {
        //recupero los datos de lo que clickeo
        var x = e.target;
        if (x.checked === true) {
            revisarChecked(x.id);
        }
    });

    function revisarChecked(id) {
        //deshabilito los checbox, para que solo este 1 checked true
        $("input[type=checkbox]").each(function () {
            if ($(this).attr('id') !== id) {
                $(this).prop('checked', false);
            }
        });
    }

    //LEJ 17.07.2018--------------------------------------
    //Saco los nodos span para modificarlos
    var _ctlColsp = $('.collapsible span').length;
    var t = 0;
    for (var i = 0; i < _ctlColsp; i++) {
        //saco los renglones de cada tabla
        var rowCount = $('#tableC-' + i + ' tbody tr').length;
        t = 0;
        for (var y = 0; y < rowCount; y++) {
            if (_decimales === '.') {

                //Para los valores de la tabla
                var _cant = $("#cant-" + i + '-' + y).text();
                var _pap = $("#pap-" + i + '-' + y).text();
                var _papo = $("#papo-" + i + '-' + y).text();
                var _mnt = $("#mnt-" + i + '-' + y).text();
                var _psu = $("#psu-" + i + '-' + y).text();
                var _ape = $("#ape-" + i + '-' + y).text();

                //Convertimos a 2 decimales
                _cant = parseFloat(_cant).toFixed(2);
                _pap = parseFloat(_pap).toFixed(2);
                _papo = parseFloat(_papo).toFixed(2);
                _mnt = parseFloat(_mnt).toFixed(2);
                _psu = parseFloat(_psu).toFixed(2);
                _ape = parseFloat(_ape).toFixed(2);
                t = t + parseFloat(_ape);

                //Reasignacion de valores y asignacion de millares
                $("#cant-" + i + '-' + y).text("$" + _cant.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#pap-" + i + '-' + y).text("$" + _pap.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#papo-" + i + '-' + y).text(_papo.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "%");
                $("#mnt-" + i + '-' + y).text("$" + _mnt.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#psu-" + i + '-' + y).text("$" + _psu.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#ape-" + i + '-' + y).text("$" + _ape.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
            }
            else if (_decimales === ',') {
                //Para los valores de la tabla
                var _cant = $("#cant-" + i + '-' + y).text();
                var _pap = $("#pap-" + i + '-' + y).text();
                var _papo = $("#papo-" + i + '-' + y).text();
                var _mnt = $("#mnt-" + i + '-' + y).text();
                var _psu = $("#psu-" + i + '-' + y).text();
                var _ape = $("#ape-" + i + '-' + (y)).text();

                //Convertimos a 2 decimales
                _cant = parseFloat(_cant).toFixed(2);
                _pap = parseFloat(_pap).toFixed(2);
                _papo = parseFloat(_papo).toFixed(2);
                _mnt = parseFloat(_mnt).toFixed(2);
                _psu = parseFloat(_psu).toFixed(2);
                _ape = parseFloat(_ape).toFixed(2);
                t = t + parseFloat(_ape);
                //Remplazamos el simbolo de decimal x la coma','
                _cant = (_cant).replace('.', ',');
                _pap = (_pap).replace('.', ',');
                _papo = (_papo).replace('.', ',');
                _mnt = (_mnt).replace('.', ',');
                _psu = (_psu).replace('.', ',');
                _ape = (_ape).replace('.', ',');
                //Reasignacion de valores
                $("#cant-" + i + '-' + y).text("$" + _cant.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
                $("#pap-" + i + '-' + y).text("$" + _pap.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
                $("#papo-" + i + '-' + y).text(_papo.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") + "%");
                $("#mnt-" + i + '-' + y).text("$" + _mnt.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
                $("#psu-" + i + '-' + y).text("$" + _psu.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
                $("#ape-" + i + '-' + y).text("$" + _ape.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
            }
        }
        //LEJ 17.07.2018----------------------------------------------------------------
        var _txt = $('#Tt-' + i).text();
        if (_decimales === '.') {
            $('#Tt-' + i).text(_txt + '- ' + '$' + t.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        }
        else if (_decimales === ',') {
            t = t.toFixed(2).replace('.', ',');
            $('#Tt-' + i).text(_txt + '- ' + '$' + t.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
        }
    }

    //LEJ 13.07.2018-----------------------------------------------------
    $('.btnEnviar').on("click", function (e) {
        //var _xfile = document.getElementById("file_carta").files.length;
        var msg = "";
        var ban = 0;
        if (document.getElementById("file_carta").files.length > 0) {
            var inputs = $(".cpC").find($("input[type=checkbox]"));
            if (ligada()) {
                ban++;
                //var _pTb = _xid.split('b');
                //var tt = $("#Tt-" + _pTb[1]).text();
                //var arrTt = tt.split('-');
                //$("#pos").val(tt[0]);
                $('#btn_guardar').click();
            } else {
                $(".cpC :input[type=checkbox]").each(function () {
                    var _xid = $(this).attr('id');
                    var checkedTrue = $('#' + _xid + ':checked').length > 0;
                    if (checkedTrue) {
                        ban++;
                        var _pTb = _xid.split('b');
                        var tt = $("#Tt-" + _pTb[1]).text();
                        var arrTt = tt.split('-');
                        $("#pos").val(tt[0]);
                        $('#btn_guardar').click();
                    }
                });
            }
            if (ban == 0) {
                msg = "Seleccionar una Distribucion";
                M.toast({ html: msg })
                document.getElementById("loader").style.display = "none"; //LEJ 13.07.2018
            }
        } else {
            msg = "Favor de subir archivo";
            M.toast({ html: msg })
            document.getElementById("loader").style.display = "none"; //LEJ 13.07.2018
        }
    });
});