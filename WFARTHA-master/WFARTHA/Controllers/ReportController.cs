using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WFARTHA.Common;
using WFARTHA.Entities;
using WFARTHA.Models;

namespace WFARTHA.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult Index(string ruta, decimal ids)
        {
            int pagina = 1101; //ID EN BASE DE DATOS
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            }

            ViewBag.url = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "") + HostingEnvironment.ApplicationVirtualPath + "/" + ruta;
            ViewBag.miNum = ids;

            return View();
        }
        // GET: Report
        public ActionResult Reporte()
        {
            int pagina = 1101;
            var spras = "ES";
            REPORT_MOD rm = new REPORT_MOD();

            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
                string uz = User.Identity.Name;
                var puesto_user = db.USUARIOs.Where(x => x.ID == uz).Select(x => x.PUESTO_ID).FirstOrDefault();
                var tsoll = (from ts in db.TSOLs
                             join tt in db.TSOLTs
                             on ts.ID equals tt.TSOL_ID
                             into jj
                             from tt in jj.DefaultIfEmpty()
                             where ts.ESTATUS == "X" && tt.SPRAS_ID.Equals(spras) && ts.ID.Equals("SRE")
                             select new
                             {
                                 Value = ts.ID,
                                 Text = ts.ID + " - " + tt.TXT50
                             }).ToList();
                if (puesto_user != 4)
                {
                    tsoll = (from ts in db.TSOLs
                             join tt in db.TSOLTs
                             on ts.ID equals tt.TSOL_ID
                             into jj
                             from tt in jj.DefaultIfEmpty()
                             where ts.ESTATUS == "X" && tt.SPRAS_ID.Equals(spras)
                             select new
                             {
                                 Value = ts.ID,
                                 Text = ts.ID + " - " + tt.TXT50
                             }).ToList();
                }
                var sociedades = (from tp in db.DET_TIPOPRESUPUESTO
                                  join soc in db.SOCIEDADs
                                  on tp.BUKRS equals soc.BUKRS
                                  where tp.ID_USER == uz
                                  select new { Value = soc.BUKRS, Text = soc.BUKRS + " - " + soc.BUTXT }).ToList();
                List<DOCUMENTO> docs = new List<DOCUMENTO>();
                for (int i = 0; i < sociedades.Count(); i++)
                {
                    string buk = sociedades[i].Value;
                    List<DOCUMENTO> doc = db.DOCUMENTOes.Where(x => x.SOCIEDAD_ID == buk).OrderBy(x => x.NUM_DOC).ToList();
                    foreach (DOCUMENTO d in doc)
                    {
                        docs.Add(d);
                    }
                }
                List<string> fechas1 = new List<string>();
                List<string> prov1 = new List<string>();
                List<string> nsap1 = new List<string>();
                List<string> user1 = new List<string>();
                List<string> ndoc1 = new List<string>();
                List<string> mont1 = new List<string>();
                List<SelectListItem> fechas = new List<SelectListItem>();
                List<SelectListItem> prov = new List<SelectListItem>();
                List<SelectListItem> nsap = new List<SelectListItem>();
                List<SelectListItem> user = new List<SelectListItem>();
                List<SelectListItem> ndoc = new List<SelectListItem>();
                List<SelectListItem> mont = new List<SelectListItem>();
                foreach (DOCUMENTO dd in docs)
                {
                    string prob = (!string.IsNullOrEmpty(dd.PAYER_ID) ? dd.PAYER_ID : "").Trim();
                    prob = Completa(prob, 10);
                    var u = db.USUARIOs.Where(x => x.ID == dd.USUARIOD_ID).FirstOrDefault();
                    var p = db.PROVEEDORs.Where(x => x.LIFNR == prob).FirstOrDefault();

                    if (dd.FECHAC_USER != null && !fechas1.Contains(dd.FECHAC_USER.ToString()))
                    {
                        fechas1.Add(dd.FECHAC_USER.ToString());
                        string fech = string.Format(DateTime.Parse(dd.FECHAC_USER.ToString()).ToString(@"dd/MM/yyyy"));
                        fechas.Add(new SelectListItem() { Text = fech, Value = dd.FECHAC_USER.ToString() });
                    }
                    if (p != null && !prov1.Contains(p.LIFNR))
                    {
                        prov1.Add(p.LIFNR);
                        prov.Add(new SelectListItem() { Text = p.LIFNR + " - " + p.NAME1, Value = dd.PAYER_ID });
                    }
                    if (dd.NUM_PRE != null && !nsap1.Contains(dd.NUM_PRE))
                    {
                        nsap1.Add(dd.NUM_PRE);
                        nsap.Add(new SelectListItem() { Text = dd.NUM_PRE, Value = dd.NUM_PRE });
                    }
                    if (dd.USUARIOD_ID != null && !user1.Contains(dd.USUARIOD_ID))
                    {
                        user1.Add(dd.USUARIOD_ID);
                        user.Add(new SelectListItem() { Text = u.ID + " - " + u.NOMBRE + " " + u.APELLIDO_P + " " + u.APELLIDO_M, Value = u.ID });
                    }
                    if (dd.NUM_DOC.ToString() != null && !ndoc1.Contains(dd.NUM_DOC.ToString()))
                    {
                        ndoc1.Add(dd.NUM_DOC.ToString());
                        ndoc.Add(new SelectListItem() { Text = dd.NUM_DOC.ToString(), Value = dd.NUM_DOC.ToString() });
                    }
                    if (dd.MONTO_DOC_MD != null && !mont1.Contains(dd.MONTO_DOC_MD.ToString()))
                    {
                        mont1.Add(dd.MONTO_DOC_MD.ToString());
                        mont.Add(new SelectListItem() { Text = dd.MONTO_DOC_MD.ToString(), Value = dd.MONTO_DOC_MD.ToString() });
                    }
                }
                var moneda = db.MONEDAs.Where(m => m.ACTIVO == true).Select(m => new { Value = m.WAERS, Text = m.WAERS + " - " + m.LTEXT }).ToList();
                //var stat = db.DOCUMENTOes.Select(x => new { Value = x.ESTATUS, Text = x.ESTATUS }).Distinct().ToList();

                List<SelectListItem> stat = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "Eliminando Solicitud", Value = ",A,,,," },
                    new SelectListItem() { Text = "Error Eliminar", Value = ",B,,,," },
                    new SelectListItem() { Text = "Solicitud Eliminada", Value = ",C,,,," },
                    new SelectListItem() { Text = "Borrador", Value = "B,,,,," },
                    new SelectListItem() { Text = "Contabilizado SAP", Value = "A,,,,," },
                    new SelectListItem() { Text = "Procesando Preliminar", Value = "N,,,null/P,G," },
                    new SelectListItem() { Text = "Error Preliminar Portal", Value = "N,,,null/P,E," },
                    new SelectListItem() { Text = "Error Preliminar SAP", Value = "N,,E,,E," },
                    new SelectListItem() { Text = "Se Generó Preliminar SAP", Value = "N,,P,,G," },
                    new SelectListItem() { Text = "Pendiente Aprobador", Value = "F,,,P/S,G,P" },
                    new SelectListItem() { Text = "Pendiente Contabilizar", Value = "C,,,P,G,P" },
                    new SelectListItem() { Text = "Error Contabilizar Portal", Value = "C,,,A,E,P" },
                    new SelectListItem() { Text = "Procesando Contabilizar SAP", Value = "C,,!E,A,G," },
                    new SelectListItem() { Text = "Error Contabilizar SAP", Value = "C,,E,A,,P" },
                    new SelectListItem() { Text = "Por Contabilizar", Value = "C,,,A,," },
                    new SelectListItem() { Text = "Contabilizar SAP", Value = "P,,,A,," },
                    new SelectListItem() { Text = "Aprobada", Value = ",,,A,," },
                    new SelectListItem() { Text = "Pendiente Corrección", Value = "F,,,R,G,P" },
                    //new SelectListItem() { Text = "Pendiente Tax", Value = ",,,T,," },
                    new SelectListItem() { Text = "Estatus desconocido", Value = ",,,,," }
                };
                List<SelectListItem> lst = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "Pagado", Value = "P" },
                    new SelectListItem() { Text = "Efectivamente Pagado", Value = "EP" }
                };

                ViewBag.tsol = new SelectList(tsoll, "Value", "Text");
                ViewBag.bukrs = new SelectList(sociedades, "Value", "Text");
                ViewBag.fecha = new SelectList(fechas, "Value", "Text");
                ViewBag.payer = new SelectList(prov, "Value", "Text");
                ViewBag.num_pre = new SelectList(nsap, "Value", "Text");
                ViewBag.user = new SelectList(user, "Value", "Text");
                ViewBag.num_doc = new SelectList(ndoc, "Value", "Text");
                ViewBag.monto = new SelectList(mont, "Value", "Text");
                ViewBag.moneda = new SelectList(moneda, "Value", "Text");
                ViewBag.estatus = new SelectList(stat, "Value", "Text");
                ViewBag.pagado = new SelectList(lst, "Value", "Text");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ReportTemplate(int id)
        {
            int pagina = 1101; //ID EN BASE DE DATOS
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);

                SP_CABECERA_Result c = db.SP_CABECERA(id).Single();
                List<SP_DETALLE_Result> d = db.SP_DETALLE(id).ToList();
                //List<SP_FIRMAS_Result> f = db.SP_FIRMAS(id).ToList();
                List<ReportFirmasResult> f = db.Database.SqlQuery<ReportFirmasResult>("SP_FIRMAS @NUM_DOC", new SqlParameter("@NUM_DOC", id)).ToList();

                ReportCabecera cab = new ReportCabecera
                {
                    NUM_DOC = c.NUM_DOC.ToString(),
                    NUM_PRE = c.NUM_PRE,
                    SOCIEDAD_ID = c.SOCIEDAD_ID,
                    SOCIEDAD_TEXT = c.SOCIEDAD_TEXT,
                    PAYER_ID = c.PAYER_ID,
                    PAYER_NAME1 = c.PAYER_NAME1,
                    MONTO_DOC_MD = c.MONTO_DOC_MD.ToString(),
                    CONDICIONES_ID = c.CONDICIONES_ID,
                    CONDICIONES_TEXT = c.CONDICIONES_TEXT,
                    CONCEPTO = (c.CONCEPTO ?? "")
                };
                List<ReportDetalle> det = new List<ReportDetalle>();
                foreach (SP_DETALLE_Result dd in d)
                {
                    ReportDetalle rd = new ReportDetalle
                    {
                        CONCEPTO = dd.CONCEPTO,
                        DESCRIPCION = dd.DESCRIPCION,
                        FACTURA = dd.FACTURA,
                        IMPORTE = dd.IMPORTE.ToString(),
                        TEXTO = (dd.TEXTO ?? "")
                    };
                    det.Add(rd);
                }
                List<ReportFirmas> fir = new List<ReportFirmas>();
                var cont = 1;
                var cont2 = f.Count();
                ReportFirmas rf = new ReportFirmas();
                for (int i = 0; i < f.Count(); i++)
                {
                    if (cont == 1)
                    {
                        rf = new ReportFirmas();
                    }
                    switch (cont)
                    {
                        case 1:
                            rf.fasen1 = f[i].fasen.ToString();
                            rf.faseletrero1 = f[i].faseletrero;
                            rf.usuariocadena1 = f[i].usuariocadena;
                            rf.fecham1 = string.Format(DateTime.Parse(f[i].fecham.ToString()).ToString(@"dd/MM/yyyy"));
                            cont++;
                            cont2--;
                            break;
                        case 2:
                            rf.fasen2 = (f[i].fasen.ToString() ?? "");
                            rf.faseletrero2 = (f[i].faseletrero ?? "");
                            rf.usuariocadena2 = (f[i].usuariocadena ?? "");
                            rf.fecham2 = (string.Format(DateTime.Parse(f[i].fecham.ToString()).ToString(@"dd/MM/yyyy")) ?? "");
                            cont++;
                            cont2--;
                            break;
                        case 3:
                            rf.fasen3 = (f[i].fasen.ToString() ?? "");
                            rf.faseletrero3 = (f[i].faseletrero ?? "");
                            rf.usuariocadena3 = (f[i].usuariocadena ?? "");
                            rf.fecham3 = (string.Format(DateTime.Parse(f[i].fecham.ToString()).ToString(@"dd/MM/yyyy")) ?? "");
                            cont++;
                            cont2--;
                            break;
                        case 4:
                            rf.fasen4 = (f[i].fasen.ToString() ?? "");
                            rf.faseletrero4 = (f[i].faseletrero ?? "");
                            rf.usuariocadena4 = (f[i].usuariocadena ?? "");
                            rf.fecham4 = (string.Format(DateTime.Parse(f[i].fecham.ToString()).ToString(@"dd/MM/yyyy")) ?? "");
                            cont++;
                            cont2--;
                            break;
                        case 5:
                            rf.fasen5 = (f[i].fasen.ToString() ?? "");
                            rf.faseletrero5 = (f[i].faseletrero ?? "");
                            rf.usuariocadena5 = (f[i].usuariocadena ?? "");
                            rf.fecham5 = (string.Format(DateTime.Parse(f[i].fecham.ToString()).ToString(@"dd/MM/yyyy")) ?? "");
                            cont = 1;
                            cont2--;
                            break;
                    }
                    if (cont == 1 || cont2 == 0)
                    {
                        fir.Add(rf);
                    }
                }

                ReportEsqueleto re = new ReportEsqueleto();
                string recibeRuta = re.crearPDF(cab, det, fir);
                ViewBag.url = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "") + HostingEnvironment.ApplicationVirtualPath + "/" + recibeRuta;
                ViewBag.miNum = c.NUM_DOC;

                return View();
            }
        }
        [HttpPost]
        public ActionResult ReportTemplate2([Bind(Include = "Tsol,Bukrs,Fecha,Payer,Num_pre,User,Num_doc,Monto,Moneda,Estatus,Pagado")] Models.REPORT_MOD rep)
        {
            int pagina = 1101; //ID EN BASE DE DATOS
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
                string uz = User.Identity.Name;
                var sociedades = (from tp in db.DET_TIPOPRESUPUESTO
                                  join soc in db.SOCIEDADs
                                  on tp.BUKRS equals soc.BUKRS
                                  where tp.ID_USER == uz
                                  select new { Value = soc.BUKRS, TEXT = soc.BUKRS + " - " + soc.BUTXT }).ToList();
                List<DOCUMENTO> docs = new List<DOCUMENTO>();
                for (int i = 0; i < sociedades.Count(); i++)
                {
                    string buk = sociedades[i].Value;
                    List<DOCUMENTO> doc = db.DOCUMENTOes.Where(x => x.SOCIEDAD_ID == buk).OrderBy(x => x.NUM_DOC).ToList();
                    foreach (DOCUMENTO d in doc)
                    {
                        docs.Add(d);
                    }
                }
                List<DOCUMENTO> docs1 = new List<DOCUMENTO>();
                // filtrado por Tipo de solicitud
                if (rep.Tsol != null)
                {
                    for (int i = 0; i < rep.Tsol.Count(); i++)
                    {
                        string com = rep.Tsol[i];
                        List<DOCUMENTO> doc = docs.Where(x => x.TSOL_ID == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por sociedad
                if (rep.Bukrs != null)
                {
                    for (int i = 0; i < rep.Bukrs.Count(); i++)
                    {
                        string com = rep.Bukrs[i];
                        List<DOCUMENTO> doc = docs.Where(x => x.SOCIEDAD_ID == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por fecha
                if (rep.Fecha != null)
                {
                    for (int i = 0; i < rep.Fecha.Count(); i++)
                    {
                        DateTime com = DateTime.Parse(rep.Fecha[i]);
                        List<DOCUMENTO> doc = docs.Where(x => x.FECHAC_USER == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por proveedor
                if (rep.Payer != null)
                {
                    for (int i = 0; i < rep.Payer.Count(); i++)
                    {
                        string com = rep.Payer[i];
                        List<DOCUMENTO> doc = docs.Where(x => x.PAYER_ID == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por numero sap
                if (rep.Num_pre != null)
                {
                    for (int i = 0; i < rep.Num_pre.Count(); i++)
                    {
                        string com = rep.Num_pre[i];
                        List<DOCUMENTO> doc = docs.Where(x => x.PAYER_ID == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por solicitante
                if (rep.User != null)
                {
                    for (int i = 0; i < rep.User.Count(); i++)
                    {
                        string com = rep.User[i];
                        List<DOCUMENTO> doc = docs.Where(x => x.USUARIOD_ID == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por numero de documento
                if (rep.Num_doc != null)
                {
                    for (int i = 0; i < rep.Num_doc.Count(); i++)
                    {
                        decimal com = decimal.Parse(rep.Num_doc[i]);
                        List<DOCUMENTO> doc = docs.Where(x => x.NUM_DOC == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por monto
                if (rep.Monto != null)
                {
                    for (int i = 0; i < rep.Monto.Count(); i++)
                    {
                        decimal com = decimal.Parse(rep.Monto[i]);
                        List<DOCUMENTO> doc = docs.Where(x => x.MONTO_DOC_MD == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por moneda
                if (rep.Moneda != null)
                {
                    for (int i = 0; i < rep.Moneda.Count(); i++)
                    {
                        string com = rep.Moneda[i];
                        List<DOCUMENTO> doc = docs.Where(x => x.MONEDA_ID == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por estatus
                if (rep.Estatus != null)
                {
                    for (int i = 0; i < rep.Estatus.Count(); i++)
                    {
                        var stats = rep.Estatus[i].Split(',');
                        var statswf = stats[3].Split('/');
                        List<DOCUMENTO> doc = new List<DOCUMENTO>();
                        if (statswf.Length == 1)
                        {
                            doc = docs.Where(x => x.ESTATUS == stats[0] && x.ESTATUS_C == stats[1] && x.ESTATUS_SAP == stats[2] && x.ESTATUS_WF == stats[3] && x.ESTATUS_PRE == stats[4]).ToList();
                        }
                        else
                        {
                            doc = docs.Where(x => x.ESTATUS == stats[0] && x.ESTATUS_C == stats[1] && x.ESTATUS_SAP == stats[2] && (x.ESTATUS_WF == statswf[0] || x.ESTATUS_WF == statswf[1]) && x.ESTATUS_PRE == stats[4]).ToList();
                        }
                        foreach (DOCUMENTO d in doc)
                        {
                            docs1.Add(d);
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }
                // filtrado por estatus de pago
                if (rep.Pagado != null)
                {
                    for (int i = 0; i < rep.Pagado.Count(); i++)
                    {
                        string com = rep.Pagado[i];
                        List<string> pago = db.ESTATUS_PAGO.Where(x => x.TIPO == com).Select(x => x.BELNR_P).ToList();
                        List<DOCUMENTO> doc = docs.Where(x => x.MONEDA_ID == com).ToList();
                        foreach (DOCUMENTO d in doc)
                        {
                            if (pago.Contains(d.NUM_DOC.ToString()))
                            {
                                docs1.Add(d);
                            }
                        }
                    }
                    docs = docs1;
                    docs1 = new List<DOCUMENTO>();
                }

                List<ReportSols> solicitudes = new List<ReportSols>();
                foreach (DOCUMENTO d in docs)
                {
                    string firmas = "";
                    string pag = "";
                    List<ReportFirmasResult> f = db.Database.SqlQuery<ReportFirmasResult>("SP_FIRMAS @NUM_DOC", new SqlParameter("@NUM_DOC", d.NUM_DOC)).ToList();
                    foreach (ReportFirmasResult ff in f)
                    {
                        firmas += ff.usuariocadena + "\n" + ff.faseletrero;
                    }
                    string flu = db.FLUJOes.Where(x => x.NUM_DOC == d.NUM_DOC).OrderByDescending(x => x.POS).Select(x => x.USUARIOA_ID).FirstOrDefault();
                    string estatus = "";
                    if (d.ESTATUS_C != null)
                    {
                        if (d.ESTATUS_C.Equals("A") || d.ESTATUS_C.Equals("B") || d.ESTATUS_C.Equals("C"))
                        {
                            if (d.ESTATUS_C.Equals("A"))
                            {
                                estatus = "Eliminando Solicitud";
                            }
                            else if (d.ESTATUS_C.Equals("B"))
                            {
                                estatus = "Error Eliminar";
                            }
                            else if (d.ESTATUS_C.Equals("C"))
                            {
                                estatus = "Solicitud Eliminada";
                            }
                        }
                    }
                    else if (d.ESTATUS.Equals("B"))
                    {
                        estatus = "Borrador";
                    }
                    else if (d.ESTATUS.Equals("A"))
                    {
                        estatus = "Contabilizado SAP";
                    }
                    else if (d.ESTATUS.Equals("N") && d.ESTATUS_C == null && d.ESTATUS_SAP == null && (d.ESTATUS_WF == null | d.ESTATUS_WF == "P"))
                    {
                        if (d.ESTATUS_PRE == "G")
                        {
                            estatus = "Procesando Preliminar";
                        }
                        else if (d.ESTATUS_PRE == "E")
                        {
                            estatus = "Error Preliminar Portal";
                        }
                    }
                    else if (d.ESTATUS.Equals("N") && d.ESTATUS_C == null && d.ESTATUS_SAP != null && d.ESTATUS_WF == null)
                    {
                        if (d.ESTATUS_PRE == "E" && d.ESTATUS_SAP == "E")
                        {
                            estatus = "Error Preliminar SAP";
                        }
                        if (d.ESTATUS_PRE == "G" && d.ESTATUS_SAP == "P")
                        {
                            estatus = "Se Generó Preliminar SAP";
                        }
                    }
                    else if (d.ESTATUS == "F" && (d.ESTATUS_WF.Equals("P") | d.ESTATUS_WF.Equals("S")))
                    {
                        if (d.ESTATUS_PRE == "G")
                        {
                            if (d.FLUJOes.Count > 0)
                            {
                                if (ViewBag.usuario.ID == flu)
                                {
                                    estatus = "Pendiente Aprobador";
                                }
                            }
                        }
                    }
                    else if (d.ESTATUS == "C" && (d.ESTATUS_WF.Equals("P") | d.ESTATUS_WF.Equals("A")))
                    {
                        if (d.ESTATUS_WF.Equals("P") && d.ESTATUS_PRE == "G")
                        {
                            if (d.FLUJOes.Count > 0)
                            {
                                if (ViewBag.usuario.ID == flu)
                                {
                                    estatus = "Pendiente Contabilizar";
                                }
                            }
                        }
                        else if (d.ESTATUS_WF.Equals("A") && d.ESTATUS_PRE == "E")
                        {
                            if (d.FLUJOes.Count > 0)
                            {
                                if (ViewBag.usuario.ID == flu)
                                {
                                    estatus = "Error Contabilizar Portal";
                                }
                            }
                        }
                        else if (d.ESTATUS_WF.Equals("A") && d.ESTATUS_PRE == "G" && d.ESTATUS_SAP != "E")
                        {
                            estatus = "Procesando Contabilizar SAP";
                        }
                        else if (d.ESTATUS_WF.Equals("A") && d.ESTATUS_SAP == "E")
                        {
                            if (d.FLUJOes.Count > 0)
                            {
                                if (ViewBag.usuario.ID == flu)
                                {
                                    estatus = "Error Contabilizar SAP";
                                }
                            }
                        }
                        else if (d.ESTATUS_WF != null && d.ESTATUS_WF.Equals("A"))
                        {
                            if (d.ESTATUS.Equals("C"))
                            {
                                estatus = "Por Contabilizar";
                            }
                            else if (d.ESTATUS.Equals("P"))
                            {
                                estatus = "Contabilizar SAP";
                            }
                            else
                            {
                                estatus = "Aprobada";
                            }
                        }
                        else if (d.ESTATUS.Equals("F") && d.ESTATUS_WF != null && d.ESTATUS_WF.Equals("R") && d.ESTATUS_PRE != null && d.ESTATUS_PRE.Equals("G"))
                        {
                            estatus = "Pendiente Corrección";
                        }
                        else if (d.ESTATUS_WF != null && d.ESTATUS_WF.Equals("T"))
                        {
                            estatus = "Pendiente Tax";
                        }
                        else
                        {
                            estatus = "Estatus desconocido";
                        }
                    }
                    DateTime fecha2 = DateTime.Parse(f.OrderByDescending(x => x.fecham).Select(x => x.fecham).FirstOrDefault().ToString());
                    DateTime fecha1 = DateTime.Parse(d.FECHAC.ToString());
                    int horas = 0;
                    if (fecha2 > fecha1)
                    {
                        horas = Convert.ToInt32((fecha2 - fecha1).TotalDays);
                    }
                    List<ESTATUS_PAGO> ep = db.ESTATUS_PAGO.Where(x => x.BELNR_P == d.NUM_DOC.ToString()).ToList();
                    foreach (ESTATUS_PAGO p in ep)
                    {
                        pag += p.TIPO;
                        if (ep.Count > 1)
                        {
                            pag += "/";
                        }
                    }
                    var mon = "";
                    if (d.MONEDA_ID == "EUR")
                    {
                        mon = double.Parse(d.MONTO_DOC_MD.ToString()).ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-ie"));
                    }
                    else if (d.MONEDA_ID == "USD")
                    {
                        mon = double.Parse(d.MONTO_DOC_MD.ToString()).ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-us"));
                    }
                    else
                    {
                        mon = double.Parse(d.MONTO_DOC_MD.ToString()).ToString("C", System.Globalization.CultureInfo.GetCultureInfo("es-mx"));
                    }
                    ReportSols s = new ReportSols
                    {
                        Tsol = (from x in db.TSOLTs where x.TSOL_ID == d.TSOL_ID select x.TXT50).FirstOrDefault(),
                        Fecha = string.Format(DateTime.Parse(d.FECHAC.ToString()).ToString(@"yyyy/MM/dd")),
                        Num_doc = d.NUM_DOC,
                        Num_pre = d.NUM_PRE,
                        Pspnr = (from x in db.PROYECTOes where x.ID_PSPNR == d.ID_PSPNR select x.NOMBRE).FirstOrDefault(),
                        Bukrsn = (from x in db.SOCIEDADs where x.BUKRS == d.SOCIEDAD_ID select x.BUTXT).FirstOrDefault(),
                        Moneda = d.MONEDA_ID,
                        Montol = mon,
                        Explicacion = d.CONCEPTO,
                        Usuarion = (from x in db.USUARIOs where x.ID == d.USUARIOD_ID select x.NOMBRE + " " + x.APELLIDO_P + " " + x.APELLIDO_M).FirstOrDefault(),
                        Usuario = d.USUARIOD_ID,
                        Wf = firmas,
                        Estatus = estatus,
                        Totald = horas.ToString(),
                        EstPago = pag
                    };
                    solicitudes.Add(s);
                }
                ReportEsqueleto2 re = new ReportEsqueleto2();
                string recibeRuta = re.crearPDF(solicitudes);
                ViewBag.url = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "") + HostingEnvironment.ApplicationVirtualPath + "/" + recibeRuta;
                ViewBag.miNum = uz;

                return View();
            }
        }
        private string Completa(string s, int longitud)
        {
            string cadena = "";
            try
            {
                long a = Int64.Parse(s);
                int l = a.ToString().Length;
                for (int i = l; i < longitud; i++)
                {
                    cadena += "0";
                }
                cadena += a.ToString();
            }
            catch
            {
                cadena = s;
            }
            return cadena;
        }
    }
}