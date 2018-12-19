using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFARTHA.Entities;
using WFARTHA.Models;
using WFARTHA.Services;

namespace WFARTHA.Controllers
{
    [Authorize]
    public class FlujosController : Controller
    {
        private WFARTHAEntities db = new WFARTHAEntities();
       
        [HttpPost]
        public string Procesa2(decimal id)
        {
            String res = "Archivo generado";
            ProcesaFlujo pf = new ProcesaFlujo();
            if (ModelState.IsValid)
            {

                res = pf.procesa(id);
                if (res.Equals("0"))//Aprobado
                {
                    //return RedirectToAction("Details", "Solicitudes", new { id = id });
                    res = "Archivo generado";
                }
                //else if (res.Equals("1") | res.Equals("2") | res.Equals("3"))//CORREO
                //{
                    
                //    return RedirectToAction("Details", "Solicitudes", new { id = id });
                //}
                //else
                //{
                //    TempData["error"] = res;
                //    return RedirectToAction("Details", "Solicitudes", new { id = id });
                //}


            }

            return res;
        }

        //FRT 13-12-2018 para realizar la actualización del tipo de cambio  y la fechacon segun la fecha en el modal
        public string TipoCambio(FLUJO F,string moneda,FLUJO P)
        {
            var correcto = "0";
            var _bol = false;
            decimal? _tipocambio = 0;
            var dia = 0;
            var wf_p = F.WF_POS;
            //DateTime fechacon = P.FECHACON.Value; //MGC 14-12-2018 Fecha contabilización
            if (P.FECHACON.HasValue){ //verificar fecha null
                while (dia < 100) // Buscar el tipo de cambio en caso de no encontrarlo buscar el mas cercano hacia abajo 
                {
                    DateTime fecha = P.FECHACON.Value.AddDays(-dia);

                    string displayName = null;
                    var keyValue = db.TCAMBIOs.FirstOrDefault(a => a.TCURR == moneda & a.GDATU == fecha);
                    if (keyValue != null)
                    {
                        var lprov = db.TCAMBIOs.Where(a => a.TCURR == moneda & a.GDATU == fecha).First().UKURS;
                        _tipocambio = lprov;
                        break;
                    }
                    dia++;
                }
               DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(F.NUM_DOC);
                dOCUMENTO.TIPO_CAMBIO = _tipocambio;

                try
                {
                    db.Entry(dOCUMENTO).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                }

                try
                {
                    FLUJO fLUJO = db.FLUJOes.Find(F.WORKF_ID,F.WF_VERSION, F.WF_POS,F.NUM_DOC, F.POS, F.DETPOS);
                    fLUJO.FECHACON = P.FECHACON;
                    db.Entry(fLUJO).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e) {

                }
                

            }

            return correcto;
        }


        //ENDFRT 13-12-2018 para realizar la actualización del tipo de cambio segun la fecha en el modal y la fechacon

        [HttpPost]
        public ActionResult Procesa(FLUJO f)
        {
           
            ProcesaFlujo pf = new ProcesaFlujo();
            DOCUMENTO d = db.DOCUMENTOes.Find(f.NUM_DOC);
            FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
            
           
            
            //MGC 12092018
            //List<TS_FORM> tts = db.TS_FORM.Where(a => a.BUKRS_ID.Equals(d.SOCIEDAD_ID) & a.LAND_ID.Equals(d.PAIS_ID)).ToList();

            //bool c = false;
            //if (actual.WORKFP.ACCION.TIPO == "R")
            //{
            //    List<DOCUMENTOT> ddt = new List<DOCUMENTOT>();
            //    foreach (TS_FORM ts in tts)
            //    {
            //        DOCUMENTOT dts = new DOCUMENTOT();
            //        dts.NUM_DOC = f.NUM_DOC;
            //        dts.TSFORM_ID = ts.POS;
            //        try
            //        {
            //            string temp = Request.Form["chk-" + ts.POS].ToString();
            //            if (temp == "on")
            //                dts.CHECKS = true;
            //            c = true;
            //        }
            //        catch
            //        {
            //            dts.CHECKS = false;
            //        }
            //        int tt = db.DOCUMENTOTS.Where(a => a.NUM_DOC.Equals(f.NUM_DOC) & a.TSFORM_ID == ts.POS).Count();
            //        if (tt == 0)
            //            ddt.Add(dts);
            //        else
            //            db.Entry(dts).State = EntityState.Modified;
            //    }
            //    if (ddt.Count > 0)
            //        db.DOCUMENTOTS.AddRange(ddt);
            //    db.SaveChanges();

            //    db.Dispose();
            //}

            FLUJO flujo = actual;
            flujo.ESTATUS = f.ESTATUS;
            flujo.FECHAM = DateTime.Now;
            flujo.COMENTARIO = f.COMENTARIO;
            flujo.USUARIOA_ID = User.Identity.Name;

            flujo.ID_RUTA_A = f.ID_RUTA_A;
            flujo.RUTA_VERSION = f.RUTA_VERSION;
            flujo.STEP_AUTO = f.STEP_AUTO;

            //MGC 11-12-2018 Agregar Contabilizador 0----------------->
            flujo.VERSIONC1 = actual.VERSIONC1;
            flujo.VERSIONC2 = actual.VERSIONC2;
            //MGC 11-12-2018 Agregar Contabilizador 0-----------------<

            flujo.FECHACON = f.FECHACON; //MGC-14-12-2018 Modificación fechacon



            //Agregar funcionalidad, para checar si el próximo es contabilización, y si es contabilización 
            //checar que el usuario contabilizador esté asignado a la sociedad
            ContabilizarRes resc = new ContabilizarRes();
            if (d.ESTATUS == "F" && (d.ESTATUS_WF.Equals("P") | d.ESTATUS_WF.Equals("S")))
            {
                //MGC 30-10-2018 Modificación estatus, Pendiente por aprobadores  *@
                if (d.ESTATUS_PRE == "G")
                {
                    //Pendiente verificar quién es el dueño del flujo si C o A
                    if (User.Identity.Name == actual.USUARIOA_ID)
                    {

                        //Simular el pf.procesa
                        //ContabilizarRes res = new ContabilizarRes();
                        resc = pf.procesaConta(flujo);
                    }
                }
            }

            //Validar la respuesta
            //Hay respuesta
            if (resc.contabilizar != null && resc.res != null)
            {
                if (resc.contabilizar == true && resc.res == false)
                {
                    TempData["error"] = "Se necesita asignar usuario contabilizador a la sociedad: " + d.SOCIEDAD_ID;
                    return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        string res = pf.procesa(flujo, "", false, "", "");
                        if (res.Equals("0"))//Aprobado
                        {
                            //FRT 13-12-2018 para realizar la actualización del tipo de cambio segun la fecha en el modal
                            if (d.MONEDA_ID != "MXN") {
                                var moneda = d.MONEDA_ID;
                                var restipo = TipoCambio(actual, moneda, f);
                            }
                            
                            //ENDFRT 13-12-2018 para realizar la actualización del tipo de cambio segun la fecha en el modal
                            return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                        }
                        else if (res.Equals("1") | res.Equals("2") | res.Equals("3"))//CORREO
                        {
                            //FRT 13-12-2018 para realizar la actualización del tipo de cambio segun la fecha en el modal
                            if (d.MONEDA_ID != "MXN") {
                                var moneda = d.MONEDA_ID;
                                var restipo = TipoCambio(actual, moneda, f);
                            }
                            
                            //ENDFRT 13-12-2018 para realizar la actualización del tipo de cambio segun la fecha en el modal

                            //return RedirectToAction("Enviar", "Mails", new { id = flujo.NUM_DOC, index = false, tipo = "A" });
                            //MGC 12092018
                            //Email em = new Email();
                            //string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                            //string image = Server.MapPath("~/images/logo_kellogg.png");
                            //if (res.Equals("1") | res.Equals("2"))//CORREO
                            //{
                            //    em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image);
                            //}
                            //else
                            //{
                            //    em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Details", image);
                            //}
                            return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                        }
                        else
                        {
                            TempData["error"] = res;
                            return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                        }
                    }
                }
            }

            int pagina = 103; //ID EN BASE DE DATOS
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
            }
            return View(f);
        }

        //public ActionResult Carga()
        //{
        //    int pagina = 601; //ID EN BASE DE DATOS
        //    using (TAT001Entities db = new TAT001Entities())
        //    {
        //        string u = User.Identity.Name;
        //        //string u = "admin";
        //        var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
        //        ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
        //        ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
        //        ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
        //        ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //        ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //        ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
        //        ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

        //        try
        //        {
        //            string p = Session["pais"].ToString();
        //            ViewBag.pais = p + ".png";
        //        }
        //        catch
        //        {
        //            //ViewBag.pais = "mx.png";
        //            //return RedirectToAction("Pais", "Home");
        //        }
        //        Session["spras"] = user.SPRAS_ID;
        //    }
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Carga(IEnumerable<HttpPostedFileBase> files)
        //{
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult LoadExcel()
        //{
        //    List<DET_AGENTEC> ld = new List<DET_AGENTEC>();


        //    if (Request.Files.Count > 0)
        //    {
        //        HttpPostedFileBase file = Request.Files["FileUpload"];
        //        //using (var stream2 = System.IO.File.Open(url, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        //        //{
        //        string extension = System.IO.Path.GetExtension(file.FileName);
        //        // Auto-detect format, supports:
        //        //  - Binary Excel files (2.0-2003 format; *.xls)
        //        //  - OpenXml Excel files (2007 format; *.xlsx)
        //        //using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream))
        //        //{
        //        IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
        //        // 2. Use the AsDataSet extension method
        //        DataSet result = reader.AsDataSet();

        //        // The result of each spreadsheet is in result.Tables
        //        // 3.DataSet - Create column names from first row
        //        DataTable dt = result.Tables[0];
        //        ld = objAList(dt);

        //        reader.Close();

        //    }

        //    List<Flujos> ff = new List<Flujos>();
        //    List<USUARIO> usuarios = new List<USUARIO>();
        //    List<CLIENTE> clientes = new List<CLIENTE>();
        //    List<PAI> paises = new List<PAI>();

        //    foreach (DET_AGENTEC da in ld)
        //    {
        //        Flujos f = new Flujos();
        //        ////---------------------------------------USUARIO
        //        f.USUARIOC_ID = da.USUARIOC_ID;
        //        f.USUARIOC_IDX = true;
        //        USUARIO u = usuarios.Where(x => x.ID.Equals(f.USUARIOC_ID)).FirstOrDefault();
        //        if (u == null)
        //        {
        //            u = db.USUARIOs.Where(x => x.ID.Equals(f.USUARIOC_ID) & x.ACTIVO == true).FirstOrDefault();
        //            if (u == null)
        //                f.USUARIOC_IDX = false;
        //            else
        //                usuarios.Add(u);
        //        }
        //        if (!f.USUARIOC_IDX)
        //            f.USUARIOC_ID = "<span class='red white-text'>" + f.USUARIOC_ID + "</span>";
        //        ////---------------------------------------PAIS
        //        f.PAIS_ID = da.PAIS_ID;
        //        f.PAIS_IDX = true;

        //        PAI p = paises.Where(x => x.LAND.Equals(f.PAIS_ID)).FirstOrDefault();
        //        if (p == null)
        //        {
        //            p = db.PAIS.Where(x => x.LAND.Equals(f.PAIS_ID) & x.ACTIVO == true & x.SOCIEDAD_ID != null).FirstOrDefault();
        //            if (p == null)
        //                f.PAIS_IDX = false;
        //            else
        //                paises.Add(p);
        //        }
        //        if (!f.PAIS_IDX)
        //            f.PAIS_ID = "<span class='red white-text'>" + f.PAIS_ID + "</span>";

        //        ////---------------------------------------CLIENTE
        //        f.KUNNR = da.KUNNR;
        //        f.KUNNRX = true;

        //        CLIENTE c = clientes.Where(x => x.KUNNR.Equals(f.KUNNR)).FirstOrDefault();
        //        if (c == null)
        //        {
        //            c = db.CLIENTEs.Where(cc => cc.KUNNR.Equals(f.KUNNR) & cc.ACTIVO == true).FirstOrDefault();
        //            if (c == null)
        //                f.KUNNRX = false;
        //            else
        //                clientes.Add(c);
        //        }
        //        if (!f.KUNNRX)
        //            f.KUNNR = "<span class='red white-text'>" + f.KUNNR + "</span>";


        //        f.VERSION = da.VERSION.ToString();
        //        f.POS = da.POS.ToString();
        //        ////---------------------------------------USUARIOA
        //        f.USUARIOA_ID = da.USUARIOA_ID;
        //        f.USUARIOA_IDX = true;
        //        USUARIO ua = usuarios.Where(x => x.ID.Equals(f.USUARIOA_ID)).FirstOrDefault();
        //        if (ua == null)
        //        {
        //            ua = db.USUARIOs.Where(x => x.ID.Equals(f.USUARIOA_ID) & x.ACTIVO == true).FirstOrDefault();
        //            if (ua == null)
        //                f.USUARIOA_IDX = false;
        //            else
        //                usuarios.Add(ua);
        //        }
        //        if (!f.USUARIOA_IDX)
        //            f.USUARIOA_ID = "<span class='red white-text'>" + f.USUARIOA_ID + "</span>";

        //        if (da.MONTO == null)
        //            f.MONTO = null;
        //        else
        //            f.MONTO = da.MONTO.ToString();
        //        f.PRESUPUESTO = da.PRESUPUESTO.ToString();
        //        ff.Add(f);
        //    }
        //    JsonResult jl = Json(ff, JsonRequestBehavior.AllowGet);
        //    return jl;
        //}

        //private string completa(string s, int longitud)
        //{
        //    string cadena = "";
        //    try
        //    {
        //        long a = Int64.Parse(s);
        //        int l = a.ToString().Length;

        //        //cadena = s;
        //        for (int i = l; i < longitud; i++)
        //        {
        //            cadena += "0";
        //        }
        //        cadena += a.ToString();
        //    }
        //    catch
        //    {
        //        cadena = s;
        //    }
        //    return cadena;
        //}

        //private List<DET_AGENTEC> objAList(DataTable dt)
        //{

        //    List<DET_AGENTEC> ld = new List<DET_AGENTEC>();
        //    List<CLIENTE> clientes = new List<CLIENTE>();
        //    //Rows
        //    var rowsc = dt.Rows.Count;
        //    //columns
        //    var columnsc = dt.Columns.Count;

        //    //Columnd and row to start
        //    var rows = 1; // 2
        //                  //var cols = 0; // A
        //    var pos = 1;

        //    for (int i = rows; i < rowsc; i++)
        //    {
        //        //for (var j = 0; j < columnsc; j++)
        //        //{
        //        //    var data = dt.Rows[i][j];
        //        //}
        //        if (i >= 4)
        //        {
        //            var v = dt.Rows[i][1];
        //            if (Convert.ToString(v) == "")
        //            {
        //                break;
        //            }
        //        }
        //        DET_AGENTEC doc = new DET_AGENTEC();

        //        string a = Convert.ToString(pos);

        //        doc.POS = Convert.ToInt32(a);
        //        try
        //        {
        //            doc.USUARIOC_ID = dt.Rows[i][0].ToString(); //Usuario creador

        //        }
        //        catch (Exception e)
        //        {
        //            doc.USUARIOC_ID = null;
        //        }
        //        try
        //        {
        //            doc.PAIS_ID = dt.Rows[i][1].ToString(); //País
        //        }
        //        catch (Exception e)
        //        {
        //            doc.PAIS_ID = null;
        //        }
        //        try
        //        {
        //            doc.KUNNR = dt.Rows[i][2].ToString();
        //            doc.KUNNR = completa(doc.KUNNR, 10);

        //            CLIENTE u = clientes.Where(x => x.KUNNR.Equals(doc.KUNNR)).FirstOrDefault();
        //            if (u == null)
        //            {
        //                u = db.CLIENTEs.Where(cc => cc.KUNNR.Equals(doc.KUNNR) & cc.ACTIVO == true).FirstOrDefault();
        //                if (u == null)
        //                    doc.VKORG = null;
        //                else
        //                    clientes.Add(u);
        //            }

        //            CLIENTE c = clientes.Where(cc => cc.KUNNR.Equals(doc.KUNNR) & cc.ACTIVO == true).FirstOrDefault();
        //            if (c != null)
        //            {
        //                doc.VKORG = c.VKORG;
        //                doc.VTWEG = c.VTWEG;
        //                doc.SPART = c.SPART;
        //            }
        //            else
        //            {
        //                doc.VKORG = null;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            doc.KUNNR = null;
        //        }
        //        try
        //        {
        //            doc.POS = int.Parse(dt.Rows[i][3].ToString());
        //        }
        //        catch (Exception e)
        //        {
        //            doc.POS = 0;
        //        }
        //        try
        //        {
        //            doc.USUARIOA_ID = dt.Rows[i][4].ToString();
        //        }
        //        catch (Exception e)
        //        {
        //            doc.USUARIOA_ID = null;
        //        }

        //        try
        //        {
        //            doc.MONTO = decimal.Parse(dt.Rows[i][5].ToString());
        //        }
        //        catch (Exception e)
        //        {
        //            doc.MONTO = null;
        //        }
        //        try
        //        {
        //            string p = dt.Rows[i][6].ToString();
        //            if (p == "X" | p == "x")
        //                doc.PRESUPUESTO = true;
        //        }
        //        catch (Exception e)
        //        {
        //            doc.PRESUPUESTO = false;
        //        }

        //        //DET_AGENTEC poss = ld.Where(x => x.USUARIOC_ID.Equals(doc.USUARIOC_ID) & x.PAIS_ID.Equals(doc.PAIS_ID)
        //        //& x.KUNNR.Equals(doc.KUNNR)).FirstOrDefault();
        //        //if (poss == null)
        //        //    pos = 1;
        //        //else
        //        //    pos = ld.Where(x => x.USUARIOC_ID.Equals(doc.USUARIOC_ID) & x.PAIS_ID.Equals(doc.PAIS_ID) & x.KUNNR.Equals(doc.KUNNR)).Count() + 1;

        //        ld.Add(doc);
        //        pos++;
        //    }
        //    return ld;
        //}
    }
}
