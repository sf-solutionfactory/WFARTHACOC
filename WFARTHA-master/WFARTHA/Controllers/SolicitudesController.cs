using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using WFARTHA.Entities;
using WFARTHA.Models;
using WFARTHA.Services;
using System.Linq.Expressions;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web.UI;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace WFARTHA.Controllers
{
    [Authorize]//ADD RSG 17.10.2018
    public class SolicitudesController : Controller
    {
        private WFARTHAEntities db = new WFARTHAEntities();

        // GET: Solicitudes

        public ActionResult Index()
        {
            int pagina = 201; //ID EN BASE DE DATOS
            string spras = "";
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                spras = user.SPRAS_ID;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try//Mensaje de documento creado
                {
                    string p = Session["NUM_DOC"].ToString();
                    ViewBag.NUM_DOC = p;
                    Session["NUM_DOC"] = null;

                }
                catch
                {
                    ViewBag.NUM_DOC = "";
                }
            }

            //Modificación de consulta para mostrar solamente las solicitudes en la sección Solicitude, dependiendo del estatus en el que estan
            //var dOCUMENTOes = db.DOCUMENTOes.Where(a => a.USUARIOC_ID.Equals(User.Identity.Name) | a.USUARIOD_ID.Equals(User.Identity.Name)).ToList();  
            //Descartar el borrador
            var dOCUMENTOes = db.DOCUMENTOes.Where(a => (a.USUARIOC_ID.Equals(User.Identity.Name) | a.USUARIOD_ID.Equals(User.Identity.Name)) && a.ESTATUS != "B").Include(a => a.SOCIEDAD).ToList();

            dOCUMENTOes = dOCUMENTOes.Distinct(new DocumentoComparer()).ToList();
            var _d = dOCUMENTOes.OrderBy(x => x.FECHAC).OrderBy(x => x.NUM_DOC);//lejgg 04-12-2018
            return View(dOCUMENTOes);
        }

        // GET: Solicitudes/Details/5
        public ActionResult Details(decimal id)
        {
            int pagina = 203; //ID EN BASE DE DATOS
            FORMATO formato = new FORMATO();
            string spras = "";
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                spras = user.SPRAS_ID;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title += " ";
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

            }
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }

            //Obtener miles y dec
            formato = db.FORMATOes.Where(fo => fo.ACTIVO == true).FirstOrDefault();
            ViewBag.miles = formato.MILES;
            ViewBag.dec = formato.DECIMALES;

            //Documento a documento mod //Copiar valores del post al nuevo objeto
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();

            if (dOCUMENTO.DOCUMENTOPs != null)
            {
                List<DOCUMENTOP_MODSTR> dml = new List<DOCUMENTOP_MODSTR>();
                FormatosC fc = new FormatosC();
                //Agregar a documento p_mod para agregar valores faltantes
                var dps = dOCUMENTO.DOCUMENTOPs.Where(x => x.ACCION != "H").ToList();
                for (int i = 0; i < dps.Count; i++)
                {
                    DOCUMENTOP_MODSTR dm = new DOCUMENTOP_MODSTR();

                    try
                    {
                        dm.NUM_DOC = dps.ElementAt(i).NUM_DOC;
                        dm.POS = dps.ElementAt(i).POS;
                        dm.ACCION = dps.ElementAt(i).ACCION;
                        dm.FACTURA = dps.ElementAt(i).FACTURA;
                        dm.GRUPO = dps.ElementAt(i).GRUPO;
                        dm.CUENTA = dps.ElementAt(i).CUENTA;
                        string ct = dps.ElementAt(i).GRUPO;
                        var tct = dps.ElementAt(i).TCONCEPTO;
                        dm.NOMCUENTA = db.CONCEPTOes.Where(x => x.ID_CONCEPTO == ct && x.TIPO_CONCEPTO == tct).FirstOrDefault().DESC_CONCEPTO.Trim();
                        dm.TIPOIMP = dps.ElementAt(i).TIPOIMP;
                        dm.IMPUTACION = dps.ElementAt(i).IMPUTACION;
                        dm.MONTO = fc.toShow(dps.ElementAt(i).MONTO, formato.DECIMALES);
                        dm.IVA = fc.toShow(dps.ElementAt(i).IVA, formato.DECIMALES);
                        dm.TEXTO = dps.ElementAt(i).TEXTO;
                        dm.TOTAL = fc.toShow(dps.ElementAt(i).TOTAL, formato.DECIMALES);
                        //FRT06112018 se agrego para poder visualizar en pantalla
                        dm.CCOSTO = dps.ElementAt(i).CCOSTO;
                        dm.TCONCEPTO = dps.ElementAt(i).TCONCEPTO;
                        var imp = dps.ElementAt(i).MWSKZ;
                        //var impuestot = db.IMPUESTOTs.Where(a => a.MWSKZ.Equals(imp)).FirstOrDefault().TXT50;//MGC 07-11-2018 Descripción corta
                        var impuestot = db.IMPUESTOTs.Where(a => a.MWSKZ.Equals(imp)).FirstOrDefault().TXT20;//MGC 07-11-2018 Descripción corta
                        dm.IMPUESTOT = impuestot;
                        //END FRT06112018 
                        dml.Add(dm);
                    }


                    catch (Exception e) { }
                }

                //FRT 03112018.4--------Evitar doble encabezado de Detalles >

                ViewBag.lstdet = dml;

                //FRT END 

                var _t = db.DOCUMENTOPs.Where(x => x.NUM_DOC == id && x.ACCION != "H").ToList();
                decimal _total = 0;
                for (int i = 0; i < _t.Count; i++)
                {
                    _total = _total + _t[i].TOTAL;
                }

                ViewBag.total = _total;

                //FRT08112018  Mostrar el Total en Details 
                var _vtotal = Math.Round(_total, 2);
                ViewBag.totalt = _vtotal.ToString("C");
                //FRT08112018

                doc.DOCUMENTOPSTR = dml;
            }
            var anexos = db.DOCUMENTOAs.Where(a => a.NUM_DOC == id).ToList();
            doc.DOCUMENTOAL = anexos;
            List<Anexo> lstAn = new List<Anexo>();
            var result = anexos.Select(m => m.POSD).Distinct().ToList();
            bool ban = false;
            if (anexos.Count > 0)
            {

                for (int i = 0; i < result.Count; i++)
                {
                    Anexo _ax = new Anexo();
                    var posd = result[i];
                    var anexos2 = anexos.Where(x => x.POSD == posd).ToList();
                    int[] arrN = new int[5];
                    for (int j = 0; j < anexos2.Count; j++)
                    {
                        arrN[j] = anexos2[j].POS;
                        ban = true;
                    }
                    if (ban)
                    {
                        try
                        {
                            _ax.a1 = arrN[0];
                        }
                        catch (Exception e)
                        {
                            _ax.a1 = 0;
                        }
                        try
                        {
                            _ax.a2 = arrN[1];
                        }
                        catch (Exception e)
                        {
                            _ax.a3 = 0;
                        }
                        try
                        {
                            _ax.a3 = arrN[2];
                        }
                        catch (Exception e)
                        {
                            _ax.a3 = 0;
                        }
                        try
                        {
                            _ax.a4 = arrN[3];
                        }
                        catch (Exception e)
                        {
                            _ax.a4 = 0;
                        }
                        try
                        {
                            _ax.a5 = arrN[4];
                        }
                        catch (Exception e)
                        {
                            _ax.a5 = 0;
                        }
                        lstAn.Add(_ax);
                    }
                    ban = false;
                }
                doc.Anexo = lstAn;
            }
            //Obtener las sociedadess
            //List<SOCIEDAD> sociedadesl = new List<SOCIEDAD>();
            var sociedades = db.SOCIEDADs.Select(s => new { s.BUKRS, TEXT = s.BUKRS + " - " + s.BUTXT }).ToList();
            var tsoll = (from ts in db.TSOLs
                         join tt in db.TSOLTs
                         on ts.ID equals tt.TSOL_ID
                         into jj
                         from tt in jj.DefaultIfEmpty()
                         where ts.ESTATUS == "X" && tt.SPRAS_ID.Equals(spras)
                         select new
                         {
                             ts.ID,
                             TEXT = ts.ID + " - " + tt.TXT50
                         }).ToList();


            //FRT 03112018.3--------Mostrar anexos de la misma forma que en Editar >


            var lst = db.DOCUMENTOAs.Where(a => a.NUM_DOC == id && a.PATH != "" && a.ACTIVO == true).OrderBy(a => a.POS).ToList();  //FRT05122018 para ordenar corecto los anexos
            var originales = lst.Select(w => w.POS).Distinct().ToList();

            DOCUMENTOA d_a = new DOCUMENTOA();

            var la1 = db.DOCUMENTOAS1.Where(a => a.NUM_DOC == id && a.PATH != "" && a.ACTIVO == true).ToList();
            var files = la1.Count + originales.Count;
            var anexar = 0;

            if (la1.Count > 0)
            {
                for (int h = 1; h < files + 1; h++)
                {
                    var _f = false;
                    for (int p = 0; p < lst.Count; p++)
                    {
                        if (h == lst[p].POS)
                        {
                            _f = true;
                            break;
                        }
                    }
                    if (!_f)
                    {
                        d_a = new DOCUMENTOA();
                        d_a.POS = h;
                        d_a.NUM_DOC = la1[anexar].NUM_DOC;
                        d_a.TIPO = la1[anexar].TIPO;
                        d_a.DESC = la1[anexar].DESC;
                        d_a.CLASE = la1[anexar].CLASE;
                        d_a.PATH = la1[anexar].PATH;
                        lst.Add(d_a);
                        anexar++;

                    }
                }
            }

            //for (int i = 0; i < la1.Count; i++)
            //{
            //    d_a = new DOCUMENTOA();
            //    d_a.POS = i;
            //    d_a.NUM_DOC = la1[i].NUM_DOC;
            //    d_a.TIPO = la1[i].TIPO;
            //    d_a.DESC = la1[i].DESC;
            //    d_a.CLASE = la1[i].CLASE;
            //    d_a.PATH = la1[i].PATH;
            //    lst.Add(d_a);
            //}

            lst = lst.OrderBy(a => a.POS).ToList();//FRT05122018 para ordenar corecto los anexos


            List<DOCUMENTOA> result1 = new List<DOCUMENTOA>();

            for (int i = 0; i < lst.Count; i++)
            {
                // Assume not duplicate.
                bool duplicate = false;
                for (int z = 0; z < i; z++)
                {
                    if (lst[z].PATH == lst[i].PATH)
                    {
                        // This is a duplicate.
                        duplicate = true;
                        break;
                    }
                }
                // If not duplicate, add to result.
                if (!duplicate)
                {
                    result1.Add(lst[i]);
                }
            }







            for (int x = 0; x < lst.Count; x++)
            {
                var _xtr = lst[x].PATH.Split('\\');
                var _nxtr = _xtr[_xtr.Length - 1];
                _nxtr = Uri.EscapeUriString(_nxtr);
                string _path = "";
                for (int j = 0; j < _xtr.Length; j++)
                {
                    if (j != _xtr.Length - 1)
                        _path += _xtr[j] + '\\';
                    else
                        _path += _nxtr;
                }
                lst[x].PATH = _path;
            }


            ViewBag.docAn = result1; //frt06122018
            //ViewBag.docAn = lst;

            //FRT END 

            var monedal = db.MONEDAs.Where(m => m.ACTIVO == true).Select(m => new { m.WAERS, TEXT = m.WAERS + " - " + m.LTEXT }).ToList();

            var impuestol = db.IMPUESTOes.Where(i => i.ACTIVO == true).Select(i => new { i.MWSKZ });

            doc.NUM_DOC = dOCUMENTO.NUM_DOC;
            doc.TSOL_ID = dOCUMENTO.TSOL_ID;
            doc.SOCIEDAD_ID = dOCUMENTO.SOCIEDAD_ID;
            doc.FECHAD = dOCUMENTO.FECHAD;
            doc.FECHACON = dOCUMENTO.FECHACON;
            doc.FECHA_BASE = dOCUMENTO.FECHA_BASE;
            doc.MONEDA_ID = dOCUMENTO.MONEDA_ID;
            doc.TIPO_CAMBIO = dOCUMENTO.TIPO_CAMBIO;
            doc.IMPUESTO = dOCUMENTO.IMPUESTO;
            doc.MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
            doc.REFERENCIA = dOCUMENTO.REFERENCIA;
            doc.CONCEPTO = dOCUMENTO.CONCEPTO;
            doc.PAYER_ID = dOCUMENTO.PAYER_ID;
            doc.CONDICIONES = dOCUMENTO.CONDICIONES;
            doc.TEXTO_POS = dOCUMENTO.TEXTO_POS;
            doc.ASIGNACION_POS = dOCUMENTO.ASIGNACION_POS;
            doc.CLAVE_CTA = dOCUMENTO.CLAVE_CTA;
            doc.ESTATUS = dOCUMENTO.ESTATUS;
            doc.ESTATUS_C = dOCUMENTO.ESTATUS_C;
            doc.ESTATUS_EXT = doc.ESTATUS_EXT;
            doc.ESTATUS_SAP = doc.ESTATUS_SAP;
            doc.ESTATUS_WF = dOCUMENTO.ESTATUS_WF;
            doc.ESTATUS_PRE = dOCUMENTO.ESTATUS_PRE;//MGC 17-12-2018 Reprocesar Archivo preliminar
            doc.USUARIOC_ID = dOCUMENTO.USUARIOC_ID;//MGC 17-12-2018 Reprocesar Archivo preliminar

            //FRT07112018 Se agrega el nombre del impueto en la cabecera
            var impcab = dOCUMENTO.IMPUESTO;
            var impuestotcab = db.IMPUESTOTs.Where(a => a.MWSKZ.Equals(impcab)).FirstOrDefault().TXT50;//MGC 07-11-2018 Descripción corta
            doc.IMPUESTO = doc.IMPUESTO + " - " + impuestotcab;

            //FRT07112018 END

            //FRT06112018  Se agrega para poder mostra el nombre de la condicion de pago en pantalla

            if (dOCUMENTO.CONDICIONES != null)
            {
                var condicion = dOCUMENTO.CONDICIONES;
                var desccondicion = db.CONDICIONES_PAGO.Where(a => a.COND_PAGO == condicion).FirstOrDefault().TEXT;
                doc.DESC_CONDICION = desccondicion;
            }

            // END FRT06112018

            //LEJGG 06-12-2018------------------------------------I
            var np = dOCUMENTO.NUM_PRE;
            var sp = dOCUMENTO.SOCIEDAD_PRE;
            var ejp = dOCUMENTO.EJERCICIO_PRE;
            var _pag = db.ESTATUS_PAGO.Where(x => x.BELNR == np && x.GJAHR == ejp && x.BUKRS == sp && x.TIPO == "P").ToList();
            var _epag = db.ESTATUS_PAGO.Where(x => x.BELNR == np && x.GJAHR == ejp && x.BUKRS == sp && x.TIPO == "EP").ToList();

            ViewBag.pagado = _pag;
            ViewBag.epagado = _epag;

            //LEJGG 06-12-2018------------------------------------I

            ViewBag.SOCIEDAD_ID = new SelectList(sociedades, "BUKRS", "TEXT", doc.SOCIEDAD_ID);
            ViewBag.TSOL_ID = new SelectList(tsoll, "ID", "TEXT", doc.TSOL_ID);
            ViewBag.IMPUESTO = new SelectList(impuestol, "MWSKZ", "MWSKZ", doc.IMPUESTO);
            ViewBag.MONEDA_ID = new SelectList(monedal, "WAERS", "TEXT", doc.MONEDA_ID);

            ViewBag.Title += id;

            List<DOCUMENTOR> retl = new List<DOCUMENTOR>();
            List<DOCUMENTOR_MOD> retlt = new List<DOCUMENTOR_MOD>();

            retl = db.DOCUMENTORs.Where(x => x.NUM_DOC == id).ToList();

            //FRT06112018 Se agregan las lineas para obtener nombre del proyecto
            var id_pspnr = dOCUMENTO.ID_PSPNR;
            var nombre = db.PROYECTOes.Where(a => a.ID_PSPNR == id_pspnr).FirstOrDefault().NOMBRE;

            ViewBag.pid = id_pspnr;
            ViewBag.PrSl = nombre;
            //END FRT06112018


            List<listRet> lstret = new List<listRet>();
            var lifnr = dOCUMENTO.PAYER_ID;
            var bukrs = dOCUMENTO.SOCIEDAD_ID;
            var _retl = db.RETENCION_PROV.Where(rtp => rtp.LIFNR == lifnr && rtp.BUKRS == bukrs).ToList();
            var _retl2 = db.RETENCIONs.Where(rtp => rtp.ESTATUS == true).ToList();
            for (int x = 0; x < _retl.Count; x++)
            {
                listRet _l = new listRet();
                var rttt = _retl2.Where(o => o.WITHT == _retl[x].WITHT && o.WT_WITHCD == _retl[x].WT_WITHCD).FirstOrDefault();
                _l.LIFNR = _retl[x].LIFNR;
                _l.BUKRS = _retl[x].BUKRS;
                _l.WITHT = rttt.WITHT;
                _l.DESC = rttt.DESCRIPCION;
                _l.WT_WITHCD = rttt.WT_WITHCD;

                lstret.Add(_l);
            }
            var _relt = lstret;
            if (_relt != null && _relt.Count > 0)
            {
                //Obtener los textos de las retenciones
                retlt = (from r in _relt
                         join rt in db.RETENCIONTs
                         on new { A = r.WITHT, B = r.WT_WITHCD } equals new { A = rt.WITHT, B = rt.WT_WITHCD }
                         into jj
                         from rt in jj.DefaultIfEmpty()
                         where rt.SPRAS_ID.Equals("ES")
                         select new DOCUMENTOR_MOD
                         {
                             LIFNR = r.LIFNR,
                             BUKRS = r.BUKRS,
                             WITHT = r.WITHT,
                             WT_WITHCD = r.WT_WITHCD,
                             DESC = rt.TXT50 == null ? String.Empty : r.DESC,

                         }).ToList();
            }
            for (int i = 0; i < retlt.Count; i++)
            {
                var wtht = retlt[i].WITHT;
                decimal _bi = 0;
                decimal _iret = 0;
                //aqui hacemos la sumatoria
                var _res = db.DOCUMENTORPs.Where(nd => nd.NUM_DOC == dOCUMENTO.NUM_DOC && nd.WITHT == wtht).ToList();
                for (int y = 0; y < _res.Count; y++)
                {
                    if (_res[y].BIMPONIBLE == null)
                    {
                        _bi = _bi + 0;
                    }
                    else
                    {
                        _bi = _bi + decimal.Parse(_res[y].BIMPONIBLE.ToString());
                    }
                    if (_res[y].IMPORTE_RET == null)
                    {
                        _iret = _iret + 0;
                    }
                    else
                    {
                        _iret = _iret + decimal.Parse(_res[y].IMPORTE_RET.ToString());
                    }
                }
                retlt[i].BIMPONIBLE = _bi;
                retlt[i].IMPORTE_RET = _iret;
            }

            //LEJGG26-11-2018----------------
            for (int i = 0; i < retlt.Count; i++)
            {
                var _a = retlt[i].WITHT;
                var _b = retlt[i].WT_WITHCD;
                var ret = db.RETENCIONs.Where(x => x.WITHT == _a && x.WT_WITHCD == _b).FirstOrDefault().WITHT_SUB;
                if (ret != null)
                {
                    for (int x = 0; x < retlt.Count; x++)
                    {
                        if (retlt[x].WITHT == ret)
                        {
                            retlt.Remove(retlt[x]);
                        }
                    }
                }
            }
            //LEJGG26-11-2018-----------------

            ViewBag.ret = retlt;

            //Obtener datos del proveedor
            PROVEEDOR prov = db.PROVEEDORs.Where(pr => pr.LIFNR == doc.PAYER_ID).FirstOrDefault();
            ViewBag.prov = prov;

            //LEj 24.09.2018

            var _Se = db.DOCUMENTORPs.Where(x => x.NUM_DOC == id).ToList();
            var rets = _Se.Select(w => w.WITHT).Distinct().ToList();
            var rets2 = rets;
            for (int i = 0; i < rets.Count; i++)
            {
                var _rt = rets2[i];
                var ret = db.RETENCIONs.Where(x => x.WITHT == _rt).FirstOrDefault().WITHT_SUB;
                for (int j = 0; j < rets.Count; j++)
                {
                    if (rets2[j] == ret)
                    {
                        rets2.RemoveAt(j);
                    }
                }
            }
            var _xdocsrp = db.DOCUMENTORPs.Where(x => x.NUM_DOC == id).ToList();
            List<DOCUMENTORP_MOD> _xdocsrp2 = new List<DOCUMENTORP_MOD>();
            DOCUMENTORP_MOD _Data = new DOCUMENTORP_MOD();
            for (int x = 0; x < rets2.Count; x++)
            {
                for (int j = 0; j < _xdocsrp.Count; j++)
                {
                    if (rets2[x] == _xdocsrp[j].WITHT)
                    {
                        _Data = new DOCUMENTORP_MOD();
                        _Data.NUM_DOC = _xdocsrp[j].NUM_DOC;
                        _Data.POS = _xdocsrp[j].POS;
                        _Data.WITHT = _xdocsrp[j].WITHT;
                        _Data.WT_WITHCD = _xdocsrp[j].WT_WITHCD;
                        _Data.BIMPONIBLE = _xdocsrp[j].BIMPONIBLE;
                        _Data.IMPORTE_RET = _xdocsrp[j].IMPORTE_RET;
                        _xdocsrp2.Add(_Data);
                    }
                }
            }
            ViewBag.DocsRp = _xdocsrp2;
            ViewBag.Retenciones = rets2;
            //LEj 24.09.2018---

            //MGC 04 - 10 - 2018 Botones para acciones y flujo -- >
            //Obtener datos del flujo
            var vbFl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            ViewBag.workflow = vbFl;

            //MGC 22-11-2018.2 Cadena de autorización----------------------------------------------------------------------->
            ////FRT06112018 Se agregan las lineas para poder llevar a pantalla la Cadena de Autorización 
            //var _ruta = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).FirstOrDefault().ID_RUTA_A;
            //int? _version = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).FirstOrDefault().RUTA_VERSION;
            //var _user = db.DET_AGENTECC.Where(a => a.ID_RUTA_AGENTE == (_ruta) && a.VERSION == (_version)).FirstOrDefault().USUARIOA_ID;
            //var _nombre = db.USUARIOs.Where(a => a.ID.Equals(_user)).ToList();
            //for (int n = 0; n < _nombre.Count; n++)
            //{
            //    var _nom = _nombre[n].ID + " - " + _nombre[n].NOMBRE + " " + _nombre[n].APELLIDO_P + " " + _nombre[n].APELLIDO_M;
            //    ViewBag.nomautoriza = _nom;

            //}
            //// END FRT06112018
            ///
            IQueryable<DET_AGENTECC> detcc = Enumerable.Empty<DET_AGENTECC>().AsQueryable(); ;
            //Obtener la descripción de la cadena
            FLUJO vbFlujo = new FLUJO();
            vbFlujo = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).FirstOrDefault();


            string id_cadena = "";
            int? rutav = 0;
            string usuarioc = "";
            if (vbFlujo != null)
            {
                id_cadena = vbFlujo.ID_RUTA_A;
                rutav = vbFlujo.RUTA_VERSION;
                usuarioc = dOCUMENTO.USUARIOC_ID;
            }

            //Es una edición de un rechazo, por lo que solo se debe de mostrar la cadena correspondiente
            detcc = (from detc in db.DET_AGENTECC
                     where detc.USUARIOC_ID == usuarioc && detc.VERSION == rutav && detc.ID_RUTA_AGENTE == id_cadena
                     select detc);
            //group detc by new { detc.USUARIOC_ID, detc.ID_RUTA_AGENTE, detc.USUARIOA_ID } into grp
            //select grp.OrderByDescending(x => x.VERSION).FirstOrDefault());

            //Consulta tomada de sql
            //select DISTINCT MAX([VERSION]) AS VERSION
            //      ,[USUARIOC_ID]
            //      ,[ID_RUTA_AGENTE]
            //      ,[USUARIOA_ID]
            //FROM[PAGOS].[dbo].[DET_AGENTECC]
            //where USUARIOC_ID = 'admin'
            //group by[USUARIOC_ID], [ID_RUTA_AGENTE], [USUARIOA_ID]
            //order by[ID_RUTA_AGENTE]

            //Obtener las cadenas
            List<DET_AGENTECC> detcl = new List<DET_AGENTECC>();

            detcl = (from dv in detcc.ToList()
                     join dccl in db.DET_AGENTECC.ToList()
                     on new { dv.VERSION, dv.USUARIOC_ID, dv.ID_RUTA_AGENTE, dv.USUARIOA_ID } equals new { dccl.VERSION, dccl.USUARIOC_ID, dccl.ID_RUTA_AGENTE, dccl.USUARIOA_ID }
                     select new DET_AGENTECC
                     {
                         VERSION = dccl.VERSION,
                         USUARIOC_ID = dccl.USUARIOC_ID,
                         ID_RUTA_AGENTE = dccl.ID_RUTA_AGENTE,
                         DESCRIPCION = dccl.DESCRIPCION,
                         USUARIOA_ID = dccl.USUARIOA_ID,
                         FECHAC = dccl.FECHAC
                     }).ToList();

            //Obtener el usario a
            DET_AGENTECC deta = new DET_AGENTECC();

            if (detcl != null && detcl.Count > 0)
            {
                deta = detcl.FirstOrDefault();
            }

            var dta = detcl.
                Join(
                db.USUARIOs,
                da => da.USUARIOA_ID,
                us => us.ID,
                (da, us) => new
                {
                    //ID = new List<string>() { da.VERSION, da.USUARIOC_ID, da.ID_RUTA_AGENTE, da.USUARIOA_ID},                    
                    ID = new { VERSION = da.VERSION.ToString().Replace(" ", ""), USUARIOC_ID = da.USUARIOC_ID.ToString().Replace(" ", ""), ID_RUTA_AGENTE = da.ID_RUTA_AGENTE.ToString().Replace(" ", ""), USUARIOA_ID = da.USUARIOA_ID.ToString().Replace(" ", "") },
                    TEXT = us.NOMBRE.ToString() + " " + us.APELLIDO_P.ToString()
                }).ToList();

            //MGC 11-12-2018 Agregar Contabilizador 0------>
            int vc1 = vbFlujo.VERSIONC1;
            int vc2 = vbFlujo.VERSIONC2;
            //MGC 11-12-2018 Agregar Contabilizador 0------<

            var _v = "";
            var _usc = "";
            var _id_ruta = "";
            var _usa = "";
            var _mt = "";
            var _sc = "";
            var lstDta = new List<object>();
            //LEJGG03-12-2018-----------------------------I
            for (int i = 0; i < dta.Count; i++)
            {
                _v = dta[i].ID.VERSION;
                _usc = dta[i].ID.USUARIOC_ID;
                _id_ruta = dta[i].ID.ID_RUTA_AGENTE;
                _usa = dta[i].ID.USUARIOA_ID;
                if (dOCUMENTO.MONTO_DOC_MD == null)
                {
                    _mt = 0 + "";
                }
                else
                { _mt = dOCUMENTO.MONTO_DOC_MD.ToString(); }
                _sc = dOCUMENTO.SOCIEDAD_ID;
                //var _lst = getCad2(int.Parse(_v), _usc, _id_ruta, _usa, decimal.Parse(_mt), _sc, out vc1, out vc2);//MGC 11-12-2018 Agregar Contabilizador 0
                CadenaAutorizadores _lst = getCad2(int.Parse(_v), _usc, _id_ruta, _usa, decimal.Parse(_mt), _sc, vc1, vc2); //MGC 11-12-2018 Agregar Contabilizador 0
                List<CadenaAutorizadoresItem> cadi = new List<CadenaAutorizadoresItem>();//MGC 11-12-2018 Agregar Contabilizador 0
                cadi = _lst.cadenal;//MGC 11-12-2018 Agregar Contabilizador 0

                string cad = "";
                //for (int j = 0; j < _lst.Count; j++)//MGC 11-12-2018 Agregar Contabilizador 0
                for (int j = 0; j < cadi.Count; j++)//MGC 11-12-2018 Agregar Contabilizador 0
                {
                    //var aut = lst[j].autorizador.Split('-');//MGC 11-12-2018 Agregar Contabilizador 0
                    var aut = cadi[j].autorizador.Split('-');//MGC 11-12-2018 Agregar Contabilizador 0
                    //if (j == lst.Count - 1)//MGC 11-12-2018 Agregar Contabilizador 0
                    if (j == cadi.Count - 1)//MGC 11-12-2018 Agregar Contabilizador 0
                    {
                        cad += aut[0].Trim();
                    }
                    else
                    {
                        cad += aut[0].Trim() + " - ";
                    }
                }
                var nuevo = new
                {
                    ID = dta[i].ID,
                    //TEXT = dta[i].TEXT + " (" + cad + ")"
                    TEXT = cad
                };
                lstDta.Add(nuevo);
            }
            //ViewBag.DETAA = new SelectList(dta, "ID", "TEXT");
            ViewBag.DETAA = new SelectList(lstDta, "ID", "TEXT");//LEJGG 03-12-2018

            //Obtener los aprobadores
            JsonResult autorizadores = new JsonResult();
            autorizadores = getCadena(Convert.ToInt32(rutav), usuarioc, id_cadena, deta.USUARIOA_ID, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), dOCUMENTO.SOCIEDAD_ID, vc1, vc2);//MGC 11-12-2018 Agregar Contabilizador 0

            CadenaAutorizadores lcandena = new CadenaAutorizadores();
            List<CadenaAutorizadoresItem> lcandenai = new List<CadenaAutorizadoresItem>();
            try
            {
                lcandena = autorizadores.Data as CadenaAutorizadores;
                lcandenai = lcandena.cadenal;
            }
            catch (Exception)
            {

            }

            ViewBag.lcadena = lcandenai;//MGC 11-12-2018 Agregar Contabilizador 0
            //MGC 22-11-2018.2 Cadena de autorización-----------------------------------------------------------------------<


            // frt obtener el flujo de SAP

            var vbSap = db.DOCUMENTOLOGs.Where(s => s.NUM_DOC.Equals(id)).OrderBy(s => s.FECHA).ToList();
            ViewBag.LogSap = vbSap;

            string usuariodel = "";
            DateTime fecha = DateTime.Now.Date;
            List<WFARTHA.Entities.DELEGAR> del = db.DELEGARs.Where(a => a.USUARIOD_ID.Equals(User.Identity.Name) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).ToList();

            FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id) & a.ESTATUS.Equals("P")).FirstOrDefault();
            ViewBag.acciones = f;

            if (f != null)
                if (f.USUARIOA_ID != null)
                {
                    if (del.Count > 0)
                    {
                        DELEGAR dell = del.Where(a => a.USUARIO_ID.Equals(f.USUARIOA_ID)).FirstOrDefault();
                        if (dell != null)
                            usuariodel = dell.USUARIO_ID;
                        else
                            usuariodel = User.Identity.Name;
                    }
                    else
                        usuariodel = User.Identity.Name;

                    if (f.USUARIOA_ID.Equals(usuariodel))
                        ViewBag.accion = db.WORKFPs.Where(a => a.ID.Equals(f.WORKF_ID) & a.POS.Equals(f.WF_POS) & a.VERSION.Equals(f.WF_VERSION)).FirstOrDefault().ACCION.TIPO;
                }
                else
                {
                    ViewBag.accion = db.WORKFPs.Where(a => a.ID.Equals(f.WORKF_ID) & a.POS.Equals(f.WF_POS) & a.VERSION.Equals(f.WF_VERSION)).FirstOrDefault().ACCION.TIPO;
                }

            DocumentoFlujo DF = new DocumentoFlujo();
            DF.D = doc;
            DF.F = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderByDescending(a => a.POS).FirstOrDefault();

            //MGC 04 - 10 - 2018 Botones para acciones y flujo <--

            ////MGC 12-11-2018 Saber si el siguiente aprobador es el contabilizador------------------------------------------------------->
            ///

            ContabilizarRes contanext = new ContabilizarRes();
            //Validar que esté en proceso de aprobación
            if (dOCUMENTO.ESTATUS == "F" && (dOCUMENTO.ESTATUS_WF.Equals("P") | dOCUMENTO.ESTATUS_WF.Equals("S")))
            {
                //MGC 30-10-2018 Modificación estatus, Pendiente por aprobadores  *@
                if (dOCUMENTO.ESTATUS_PRE == "G")
                {
                    //Pendente verificar quién es el dueño del flujo si C o A
                    if (ViewBag.usuario.ID == DF.F.USUARIOA_ID)
                    {
                        ProcesaFlujo pf = new ProcesaFlujo();
                        contanext = pf.nextContabilizar(DF.F, User.Identity.Name);
                    }
                }
            }

            ViewBag.contanext = contanext;
            ////MGC 12-11-2018 Saber si el siguiente aprobador es el contabilizador-------------------------------------------------------<


            return View(DF);
        }

        // GET: Solicitudes/Create
        public ActionResult Create()
        {
            int pagina = 202; //ID EN BASE DE DATOS
            FORMATO formato = new FORMATO();
            string spras = "";
            string user_id = "";//MGC 02-10-2018 Cadena de autorización
            string pselG = "";//MGC 16-10-2018 Obtener las sociedades asignadas al usuario
            using (WFARTHAEntities db = new WFARTHAEntities())
            {

                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                spras = user.SPRAS_ID;
                user_id = user.ID;//MGC 02-10-2018 Cadena de autorización
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                //Obtener miles y dec
                formato = db.FORMATOes.Where(f => f.ACTIVO == true).FirstOrDefault();
                ViewBag.miles = formato.MILES;
                ViewBag.dec = formato.DECIMALES;

            }
            try
            {
                //FRT14112018 Para abrir proyectos
                //frt 081118---------------------
                string editar = Session["Edit"].ToString();
                string create = Session["create"].ToString();

                if (editar == 1 + "")
                {
                    Session["Edit"] = 0;
                    Session["create"] = 0;
                    return RedirectToAction("Proyectos", "Home", new { returnUrl = Request.Url.AbsolutePath });
                    //frt 081118---------------------
                }
                else if (create == 1 + "")
                {
                    Session["Edit"] = 0;
                    Session["create"] = 0;
                    return RedirectToAction("Proyectos", "Home", new { returnUrl = Request.Url.AbsolutePath });
                    //frt 081118---------------------
                }
                else
                {
                    string p = Session["pr"].ToString();
                    string pid = Session["id_pr"].ToString();
                    ViewBag.PrSl = p;
                    pselG = pid;//MGC 16-10-2018 Obtener las sociedades asignadas al usuario

                    ViewBag.pid = pid;//MGC 29-10-2018 Guardar el proyecto en el create
                    Session["create"] = 1;  //FRT14112018 para mostrar pantalla de proyectos en create
                }
                //END FRT14112018 Para abrir proyectos

            }
            catch
            {
                //ViewBag.pais = "mx.png";
                return RedirectToAction("Proyectos", "Home", new { returnUrl = Request.Url.AbsolutePath });
            }
            //Obtener las sociedadess
            //List<SOCIEDAD> sociedadesl = new List<SOCIEDAD>();
            //MGC 16-10-2018 Obtener las sociedades asignadas al usuario a partir del proyecto seleccionado -->
            List<ASIGN_PROY_SOC> aps = new List<ASIGN_PROY_SOC>();
            aps = db.ASIGN_PROY_SOC.Where(ap => ap.ID_PSPNR == pselG).ToList();

            var sociedades = (from ap in aps
                              join soc in db.SOCIEDADs
                              on ap.ID_BUKRS equals soc.BUKRS
                              select new
                              {
                                  soc.BUKRS,
                                  TEXT = soc.BUKRS + " - " + soc.BUTXT
                              }).ToList();

            Session["SOC"] = sociedades[0].BUKRS;  //FRT 02112018

            //var sociedades = db.SOCIEDADs.Select(s => new { s.BUKRS, TEXT = s.BUKRS + " - " + s.BUTXT }).ToList();//MGC 16-10-2018 Obtener las sociedades asignadas al usuario
            //MGC 16-10-2018 Obtener las sociedades asignadas al usuario <--
            //MGC 03-10-2018 solicitud con orden de compra -->
            //Obtener la solicitud con la configuración
            var tsoll = (from ts in db.TSOLs
                         join tt in db.TSOLTs
                         on ts.ID equals tt.TSOL_ID
                         into jj
                         from tt in jj.DefaultIfEmpty()
                         where ts.ESTATUS == "X" && tt.SPRAS_ID.Equals(spras)
                         select new
                         {
                             //ts.ID,
                             ID = new { ID = ts.ID.ToString().Replace(" ", ""), RANGO = ts.RANGO_ID.ToString().Replace(" ", ""), EDITDET = ts.EDITDET.ToString().Replace(" ", "") },
                             TEXT = ts.ID + " - " + tt.TXT50,
                             DEFAULT = ts.DEFAULT
                         }).ToList();

            //Obtener el valor default
            var tsolldef = tsoll.Where(tsd => tsd.DEFAULT == true).Select(tsds => tsds.ID).FirstOrDefault();
            //MGC 03-10-2018 solicitud con orden de compra <--

            var monedal = db.MONEDAs.Where(m => m.ACTIVO == true).Select(m => new { m.WAERS, TEXT = m.WAERS + " - " + m.LTEXT }).ToList();

            //var impuestol = db.IMPUESTOes.Where(i => i.ACTIVO == true).Select(i => new { i.MWSKZ }).ToList();

            var impuestol = (from im in db.IMPUESTOes
                             join imt in db.IMPUESTOTs.Where(imtt => imtt.SPRAS_ID == spras)
                             on im.MWSKZ equals imt.MWSKZ
                             into jj
                             from tt in jj.DefaultIfEmpty()
                             where im.ACTIVO == true
                             select new
                             {
                                 im.MWSKZ,
                                 TEXT = im.MWSKZ + " - " + tt.TXT50
                             }).ToList();

            ViewBag.SOCIEDAD_ID = new SelectList(sociedades, "BUKRS", "TEXT");
            //MGC 03-10-2018 solicitud con orden de compra -->
            //Obtener la solicitud con la configuración
            ViewBag.TSOL_IDL = new SelectList(tsoll, "ID", "TEXT", selectedValue: tsolldef);
            //MGC 03-10-2018 solicitud con orden de compra <--

            ViewBag.IMPUESTO = new SelectList(impuestol, "MWSKZ", "TEXT", "V3");
            ViewBag.MONEDA_ID = new SelectList(monedal, "WAERS", "TEXT");
            //lej 30.08.2018------------------
            var xsc = db.SOCIEDADs.ToList();
            var p1 = "";
            if (xsc.Count > 0)
            {
                //saco el primer registro, que sera el que ponga el combobox por default
                p1 = xsc[0].BUKRS;
            }

            //var sc = db.SOCIEDADs.Where(x => x.BUKRS == p1).First();//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad

            var detprov = db.DET_PROVEEDOR.Where(pp => pp.ID_BUKRS == p1).Select(pp1 => pp1.ID_LIFNR).Distinct().ToList();//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad

            //ViewBag.PROVE = new SelectList(sc.PROVEEDORs, "LIFNR", "LIFNR");//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad
            ViewBag.PROVE = new SelectList(detprov, "LIFNR", "LIFNR");//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad

            //MGC 04092018 Conceptos
            //Obtener los valores de los impuestos
            var impl = (from i in db.IMPUESTOes
                        join ii in db.IIMPUESTOes
                        on i.MWSKZ equals ii.MWSKZ
                        where i.ACTIVO == true && ii.ACTIVO == true
                        select new
                        {
                            MWSKZ = ii.MWSKZ,
                            //KSCHL = ii.KSCHL,
                            KBETR = ii.KBETR,
                            ACTIVO = ii.ACTIVO
                        }).ToList();

            var impuestosv = JsonConvert.SerializeObject(impl, Newtonsoft.Json.Formatting.Indented);
            ViewBag.impuestosval = impuestosv;
            //Workflow
            //ViewBag.worflw = "'aaa','bbb','ccc','ddd','eee'";//lej 10.09.2018
            //Insertar las fechas predefinidas de hoy
            string dates = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime theTime = DateTime.ParseExact(dates, //"06/04/2018 12:00:00 a.m."
                                        "dd/MM/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None);

            ViewBag.fechah = theTime.ToString();

            DOCUMENTO_MOD d = new DOCUMENTO_MOD();

            d.FECHAD = theTime;
            d.FECHACON = theTime;
            d.FECHA_BASE = theTime;

            //MGC 14-11-2018 Cadena de autorización----------------------------------------------------------------------------->
            //Obtener las cadenas vigentes, considerando la versión
            var detcv = (from detc in db.DET_AGENTECC
                         where detc.USUARIOC_ID == user_id
                         group detc by new { detc.USUARIOC_ID, detc.ID_RUTA_AGENTE, detc.USUARIOA_ID } into grp
                         select grp.OrderByDescending(x => x.VERSION).FirstOrDefault());

            //Consulta tomada de sql
            //select DISTINCT MAX([VERSION]) AS VERSION
            //      ,[USUARIOC_ID]
            //      ,[ID_RUTA_AGENTE]
            //      ,[USUARIOA_ID]
            //FROM[PAGOS].[dbo].[DET_AGENTECC]
            //where USUARIOC_ID = 'admin'
            //group by[USUARIOC_ID], [ID_RUTA_AGENTE], [USUARIOA_ID]
            //order by[ID_RUTA_AGENTE]

            //Obtener las cadenas
            List<DET_AGENTECC> detcl = new List<DET_AGENTECC>();

            detcl = (from dv in detcv.ToList()
                     join dccl in db.DET_AGENTECC.ToList()
                     on new { dv.VERSION, dv.USUARIOC_ID, dv.ID_RUTA_AGENTE, dv.USUARIOA_ID } equals new { dccl.VERSION, dccl.USUARIOC_ID, dccl.ID_RUTA_AGENTE, dccl.USUARIOA_ID }
                     select new DET_AGENTECC
                     {
                         VERSION = dccl.VERSION,
                         USUARIOC_ID = dccl.USUARIOC_ID,
                         ID_RUTA_AGENTE = dccl.ID_RUTA_AGENTE,
                         DESCRIPCION = dccl.DESCRIPCION,
                         USUARIOA_ID = dccl.USUARIOA_ID,
                         FECHAC = dccl.FECHAC
                     }).ToList();


            //MGC 14-11-2018 Cadena de autorización-----------------------------------------------------------------------------<

            //MGC 02-10-2018 Cadena de autorización
            //List<DET_AGENTECC> dta = new List<DET_AGENTECC>();
            //Falta vigencia
            //MGC 14-11-2018 Cadena de autorización----------------------------------------------------------------------------->
            //var dta = db.DET_AGENTECC.Where(dt => dt.USUARIOC_ID == user_id).
            var dta = detcl.
            //MGC 14-11-2018 Cadena de autorización-----------------------------------------------------------------------------<
                Join(
                db.USUARIOs,
                da => da.USUARIOA_ID,
                us => us.ID,
                (da, us) => new
                {
                    //ID = new List<string>() { da.VERSION, da.USUARIOC_ID, da.ID_RUTA_AGENTE, da.USUARIOA_ID},                    
                    ID = new { VERSION = da.VERSION.ToString().Replace(" ", ""), USUARIOC_ID = da.USUARIOC_ID.ToString().Replace(" ", ""), ID_RUTA_AGENTE = da.ID_RUTA_AGENTE.ToString().Replace(" ", ""), USUARIOA_ID = da.USUARIOA_ID.ToString().Replace(" ", "") },
                    TEXT = us.NOMBRE.ToString() + " " + us.APELLIDO_P.ToString()
                }).ToList();

            //MGC 11-12-2018 Agregar Contabilizador 0------>
            //versión del contabilizador 0 y contabilizador 1
            int vc1 = 0;
            int vc2 = 0;
            //MGC 11-12-2018 Agregar Contabilizador 0------<

            var _v = "";
            var _usc = "";
            var _id_ruta = "";
            var _usa = "";
            var _mt = "";
            var _sc = "";
            var lstDta = new List<object>();
            //LEJGG03-12-2018-----------------------------I
            for (int i = 0; i < dta.Count; i++)
            {
                _v = dta[i].ID.VERSION;
                _usc = dta[i].ID.USUARIOC_ID;
                _id_ruta = dta[i].ID.ID_RUTA_AGENTE;
                _usa = dta[i].ID.USUARIOA_ID;
                if (d.MONTO_DOC_MD == null)
                {
                    _mt = 0 + "";
                }
                else
                { _mt = d.MONTO_DOC_MD.ToString(); }
                //_sc = d.SOCIEDAD_ID;
                _sc = Session["id_pr"].ToString();
                //var lst = getCad2(int.Parse(_v), _usc, _id_ruta, _usa, decimal.Parse(_mt), _sc, out vc1, out vc2); //MGC 11-12-2018 Agregar Contabilizador 0
                CadenaAutorizadores lst = getCad2(int.Parse(_v), _usc, _id_ruta, _usa, decimal.Parse(_mt), _sc, vc1, vc2); //MGC 11-12-2018 Agregar Contabilizador 0
                List<CadenaAutorizadoresItem> cadi = new List<CadenaAutorizadoresItem>();//MGC 11-12-2018 Agregar Contabilizador 0
                cadi = lst.cadenal;//MGC 11-12-2018 Agregar Contabilizador 0

                string cad = "";
                //for (int j = 0; j < lst.Count; j++)//MGC 11-12-2018 Agregar Contabilizador 0
                for (int j = 0; j < cadi.Count; j++)//MGC 11-12-2018 Agregar Contabilizador 0
                {
                    //var aut = lst[j].autorizador.Split('-');//MGC 11-12-2018 Agregar Contabilizador 0
                    var aut = cadi[j].autorizador.Split('-');//MGC 11-12-2018 Agregar Contabilizador 0
                    //if (j == lst.Count - 1)//MGC 11-12-2018 Agregar Contabilizador 0
                    if (j == cadi.Count - 1)//MGC 11-12-2018 Agregar Contabilizador 0
                    {
                        cad += aut[0].Trim();
                    }
                    else
                    {
                        cad += aut[0].Trim() + " - ";
                    }
                }
                var nuevo = new
                {
                    ID = dta[i].ID,
                    //TEXT = dta[i].TEXT + " (" + cad + ")"
                    TEXT = cad
                };
                lstDta.Add(nuevo);
            }
            //ViewBag.DETAA = new SelectList(dta, "ID", "TEXT");
            ViewBag.DETAA = new SelectList(lstDta, "ID", "TEXT");//LEJGG 03-12-2018
            //LEJGG03-12-2018-----------------------------F
            ViewBag.DETAA2 = JsonConvert.SerializeObject(db.DET_AGENTECC.Where(dt => dt.USUARIOC_ID == user_id).ToList(), Newtonsoft.Json.Formatting.Indented);

            return View(d);
        }

        // POST: Solicitudes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NUM_DOC,NUM_PRE,TSOL_ID,TALL_ID,SOCIEDAD_ID,CANTIDAD_EV,USUARIOC_ID," +
            "USUARIOD_ID,FECHAD,FECHAC,HORAC,FECHAC_PLAN,FECHAC_USER,HORAC_USER,ESTATUS,ESTATUS_C,ESTATUS_SAP,ESTATUS_WF," +
            "DOCUMENTO_REF,CONCEPTO,NOTAS,MONTO_DOC_MD,MONTO_FIJO_MD,MONTO_BASE_GS_PCT_MD,MONTO_BASE_NS_PCT_MD,MONTO_DOC_ML," +
            "MONTO_FIJO_ML,MONTO_BASE_GS_PCT_ML,MONTO_BASE_NS_PCT_ML,MONTO_DOC_ML2,MONTO_FIJO_ML2,MONTO_BASE_GS_PCT_ML2," +
            "MONTO_BASE_NS_PCT_ML2,PORC_ADICIONAL,IMPUESTO,ESTATUS_EXT,PAYER_ID,MONEDA_ID,MONEDAL_ID,MONEDAL2_ID," +
            "TIPO_CAMBIO,TIPO_CAMBIOL,TIPO_CAMBIOL2,NO_FACTURA,FECHAD_SOPORTE,METODO_PAGO,NO_PROVEEDOR,PASO_ACTUAL," +
            "AGENTE_ACTUAL,FECHA_PASO_ACTUAL,PUESTO_ID,GALL_ID,CONCEPTO_ID,DOCUMENTO_SAP,FECHACON,FECHA_BASE,REFERENCIA," +
            "CONDICIONES,TEXTO_POS,ASIGNACION_POS,CLAVE_CTA, DOCUMENTOP,DOCUMENTOR,DOCUMENTORP,DOCUMENTOA_TAB,Anexo")] Models.DOCUMENTO_MOD doc, IEnumerable<HttpPostedFileBase> file_sopAnexar, string[] labels_desc,
            //MGC 02-10-2018 Cadenas de autorización
            string DETTA_VERSION, string DETTA_USUARIOC_ID, string DETTA_ID_RUTA_AGENTE, string mtTot, string DETTA_USUARIOA_ID, string borr, string FECHADO, string Uuid
            //MGC 11-12-2018 Agregar Contabilizador 0
            ,string VERSIONC1, string VERSIONC2)
        {
            int pagina = 202; //ID EN BASE DE DATOS
            string errorString = "";
            FORMATO formato = new FORMATO();
            string spras = "";
            string user_id = ""; //MGC 02-10-2018 Cadenas de autorización
                                 //LEJGG 22/10/2018

            var filenull = false;  //FRT04122018

            if (doc.FECHAD == null)
            {
                //Si doc.FECHAD viene vacio o nulo, le asgino la fecha que tiene fechado, su campo oculto
                doc.FECHAD = DateTime.Parse(FECHADO);
            }

            //MGC 26-10-2018 Agregar usuario solicitante a la bd
            doc.USUARIOD_ID = DETTA_USUARIOA_ID;

            if (ModelState.IsValid)
            {
                try
                {
                    DOCUMENTO dOCUMENTO = new DOCUMENTO();

                    //Copiar valores del post al nuevo objeto
                    // FRT06112018 Se agrega linea para obtener el ID del proyecto
                    string pid = Session["id_pr"].ToString();
                    dOCUMENTO.ID_PSPNR = pid;
                    //FRT06112018
                    dOCUMENTO.TSOL_ID = doc.TSOL_ID;
                    dOCUMENTO.SOCIEDAD_ID = doc.SOCIEDAD_ID;
                    dOCUMENTO.FECHAD = doc.FECHAD;
                    dOCUMENTO.FECHACON = doc.FECHAD;
                    dOCUMENTO.FECHA_BASE = doc.FECHAD;
                    dOCUMENTO.MONEDA_ID = doc.MONEDA_ID;
                    if (doc.TIPO_CAMBIO == null)//Lejgg 15-11-2018
                    {
                        dOCUMENTO.TIPO_CAMBIO = 0;//Lejgg 15-11-2018
                    }
                    else
                    { dOCUMENTO.TIPO_CAMBIO = doc.TIPO_CAMBIO; }
                    dOCUMENTO.IMPUESTO = doc.IMPUESTO;
                    dOCUMENTO.USUARIOD_ID = doc.USUARIOD_ID;//MGC 26-10-2018 Agregar usuario solicitante a la bd
                                                            // dOCUMENTO.MONTO_DOC_MD = doc.MONTO_DOC_MD;
                                                            //LEJGG 10-10-2018------------------------>
                    try
                    {
                        var _t = mtTot.Replace("$", "");
                        _t = _t.Replace(",", "");
                        dOCUMENTO.MONTO_DOC_MD = decimal.Parse(_t);
                    }
                    catch (Exception e)
                    {
                        dOCUMENTO.MONTO_DOC_MD = 0;
                    }
                    //LEJGG 10-10-2018------------------------<
                    //dOCUMENTO.REFERENCIA = doc.REFERENCIA;
                    dOCUMENTO.CONCEPTO = doc.CONCEPTO;
                    dOCUMENTO.PAYER_ID = doc.PAYER_ID;
                    dOCUMENTO.CONDICIONES = doc.CONDICIONES;
                    dOCUMENTO.TEXTO_POS = doc.TEXTO_POS;
                    dOCUMENTO.ASIGNACION_POS = doc.ASIGNACION_POS;
                    dOCUMENTO.CLAVE_CTA = doc.CLAVE_CTA;

                    //B20180625 MGC 2018.06.25
                    //Obtener usuarioc
                    USUARIO us = db.USUARIOs.Find(User.Identity.Name);//RSG 02/05/2018
                    dOCUMENTO.PUESTO_ID = us.PUESTO_ID;//RSG 02/05/2018
                    dOCUMENTO.USUARIOC_ID = User.Identity.Name;


                    //Obtener el rango de números
                    Rangos rangos = new Rangos();//RSG 01.08.2018
                                                 //Obtener el número de documento
                    decimal N_DOC = rangos.getSolID(dOCUMENTO.TSOL_ID);
                    dOCUMENTO.NUM_DOC = N_DOC;
                    //Actualizar el rango
                    rangos.updateRango(dOCUMENTO.TSOL_ID, dOCUMENTO.NUM_DOC);

                    //Referencia
                    dOCUMENTO.REFERENCIA = dOCUMENTO.NUM_DOC.ToString();//LEJ 11.09.2018

                    //Obtener el tipo de documento
                    var doct = db.DET_TIPODOC.Where(dt => dt.TIPO_SOL == doc.TSOL_ID).FirstOrDefault();
                    doc.DOCUMENTO_SAP = doct.BLART.ToString();
                    dOCUMENTO.DOCUMENTO_SAP = doct.BLART.ToString();

                    //Fechac
                    dOCUMENTO.FECHAC = DateTime.Now;

                    //Horac
                    dOCUMENTO.HORAC = DateTime.Now.TimeOfDay;

                    //FECHAC_PLAN
                    dOCUMENTO.FECHAC_PLAN = DateTime.Now.Date;

                    //FECHAC_USER
                    dOCUMENTO.FECHAC_USER = DateTime.Now.Date;

                    //HORAC_USER
                    dOCUMENTO.HORAC_USER = DateTime.Now.TimeOfDay;

                    if (borr == string.Empty)
                    {
                        //Estatus
                        dOCUMENTO.ESTATUS = "N";
                    }
                    else if (borr == "B")
                    {
                        //Estatus
                        dOCUMENTO.ESTATUS = borr;
                    }

                    //Estatus wf
                    //dOCUMENTO.ESTATUS_WF = "P";//MGC 30-10-2018 Si el wf es p es que no se ha creado, si es A, es que se creo el archivo, cambia al generar el preliminar

                    db.DOCUMENTOes.Add(dOCUMENTO);
                    db.SaveChanges();//Codigolej

                    doc.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //return RedirectToAction("Index");

                    //Redireccionar al inicio
                    //Guardar número de documento creado
                    Session["NUM_DOC"] = dOCUMENTO.NUM_DOC;

                    //Guardar las posiciones de la solicitud
                    try
                    {
                        decimal _monto = 0;
                        decimal _iva = 0;
                        decimal _total = 0;
                        var _mwskz = "";
                        var _pos_err_imputacion = "";

                        int j = 1;
                        for (int i = 0; i < doc.DOCUMENTOP.Count; i++)
                        {

                            try
                            {

                                decimal _error_imputacion = 0;


                                DOCUMENTOP dp = new DOCUMENTOP();

                                dp.NUM_DOC = doc.NUM_DOC;
                                dp.POS = j;
                                dp.ACCION = doc.DOCUMENTOP[i].ACCION;
                                dp.FACTURA = doc.DOCUMENTOP[i].FACTURA;
                                dp.TCONCEPTO = doc.DOCUMENTOP[i].TCONCEPTO;
                                dp.GRUPO = doc.DOCUMENTOP[i].GRUPO;
                                dp.CUENTA = doc.DOCUMENTOP[i].CUENTA;//MGC 17-10-2018.2 Modificación Cuenta de PEP
                                dp.TIPOIMP = doc.DOCUMENTOP[i].TIPOIMP;
                                dp.IMPUTACION = doc.DOCUMENTOP[i].IMPUTACION;
                                dp.CCOSTO = doc.DOCUMENTOP[i].CCOSTO;
                                dp.MONTO = doc.DOCUMENTOP[i].MONTO;
                                _monto = _monto + doc.DOCUMENTOP[i].MONTO;//lejgg 10-10-2018
                                var sp = doc.DOCUMENTOP[i].MWSKZ.Split('-');
                                _mwskz = sp[0];//lejgg 10-10-2018
                                dp.MWSKZ = sp[0];
                                dp.IVA = doc.DOCUMENTOP[i].IVA;
                                _iva = _iva + doc.DOCUMENTOP[i].IVA;//lejgg 10-10-2018
                                dp.TOTAL = doc.DOCUMENTOP[i].TOTAL;
                                _total = _total + doc.DOCUMENTOP[i].TOTAL;//lejgg 10-10-2018
                                dp.TEXTO = doc.DOCUMENTOP[i].TEXTO;
                                db.DOCUMENTOPs.Add(dp);
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                //
                            }
                            j++;
                        }
                        //lejgg 10-10-2018-------------------->
                        DOCUMENTOP _dp = new DOCUMENTOP();

                        _dp.NUM_DOC = doc.NUM_DOC;
                        _dp.POS = j;
                        _dp.ACCION = "H";
                        _dp.CUENTA = doc.PAYER_ID;
                        _dp.MONTO = _monto + _iva;////MGC 29-10-2018 Obtener las retenciones relacionadas con las ya mostradas en la tabla
                        _dp.MWSKZ = _mwskz;
                        _dp.IVA = _iva;
                        _dp.TOTAL = _total;
                        db.DOCUMENTOPs.Add(_dp);
                        db.SaveChanges();
                        //lejgg 10-10-2018-------------------------<
                    }
                    //Guardar las retenciones de la solicitud

                    catch (Exception e)
                    {
                        errorString = e.Message.ToString();
                        //Guardar número de documento creado

                    }

                    //FRT25112018 cambio para guardar anexos

                    List<string> listaDirectorios = new List<string>();
                    List<string> listaNombreArchivos = new List<string>();
                    List<string> listaDescArchivos = new List<string>();

                    //FRT20112018 PARA ARREGAR EL ANEXO DE VARIAS LINEAS
                    List<string> listaDirectorios3 = new List<string>();
                    List<string> listaNombreArchivos3 = new List<string>();
                    List<string> listaDescArchivos3 = new List<string>();
                    //ENDFRT20112018
                    try
                    {
                        //Guardar los documentos cargados en la sección de soporte
                        var res = "";
                        string errorMessage = "";
                        int numFiles = 1;
                        //Checar si hay archivos para subir
                        foreach (HttpPostedFileBase file in file_sopAnexar)
                        {
                            if (file != null)
                            {
                                if (file.ContentLength > 0)
                                {
                                    numFiles++;
                                }
                            }
                        }
                        if (numFiles > 0)
                        {
                            //Evaluar que se creo el directorio
                            try
                            {
                                int indexlabel = 0;
                                var server = "";
                                var car_ser = "att";
                                //FRT13112018 Para cambio de fuente de archivos
                                var temporal = Convert.ToDecimal(Session["Temporal"]);
                                server = getDirPrel(car_ser, temporal);
                                System.IO.DirectoryInfo directorio = new System.IO.DirectoryInfo(server + "\\");
                                if (Directory.Exists(server))
                                {
                                    filenull = true;

                                }

                                if (filenull)
                                {
                                    FileInfo[] archivos = directorio.GetFiles();

                                    for (int i = 0; i < doc.DOCUMENTOA_TAB.Count; i++)
                                    {
                                        var descripcion = "";
                                        foreach (var a in archivos)
                                        {
                                            if (a.Name.Trim() == doc.DOCUMENTOA_TAB[i].NAME.Trim())
                                            {
                                                try
                                                {
                                                    listaDescArchivos.Add(labels_desc[indexlabel]);
                                                    listaDescArchivos3.Add(labels_desc[indexlabel]);
                                                }
                                                catch (Exception ex)
                                                {
                                                    descripcion = "";
                                                    listaDescArchivos.Add(descripcion);
                                                    listaDescArchivos3.Add(descripcion);
                                                }

                                                try
                                                {
                                                    listaNombreArchivos.Add(a.Name);
                                                    listaNombreArchivos3.Add(a.Name);
                                                }
                                                catch (Exception ex)
                                                {
                                                    listaNombreArchivos.Add("");
                                                    listaNombreArchivos3.Add("");
                                                }

                                                string errorfiles = "";

                                                var url_prel = "";
                                                var dirFile = "";
                                                string carpeta = "att";
                                                try
                                                {
                                                    url_prel = getDirPrel(carpeta, doc.NUM_DOC);
                                                    dirFile = url_prel;
                                                }
                                                catch (Exception e)
                                                {
                                                    dirFile = ConfigurationManager.AppSettings["URL_ATT"].ToString() + @"att";
                                                }

                                                bool existd = ValidateIOPermission(dirFile);

                                                if (existd)
                                                {
                                                    if (!System.IO.File.Exists(dirFile + "\\" + a.Name))
                                                    {
                                                        a.CopyTo(dirFile + "\\" + a.Name);
                                                        listaDirectorios.Add(dirFile + "\\" + a.Name);
                                                        listaDirectorios3.Add(dirFile + "\\" + a.Name);
                                                    }
                                                }
                                                indexlabel++;
                                                if (errorfiles != "")
                                                {
                                                    errorMessage += "Error con el archivo " + errorfiles;
                                                }
                                            }
                                        }
                                    }




                                    //foreach (var a in archivos)
                                    //{
                                    //    var descripcion = "";

                                    //    for (int i = 0; i < doc.DOCUMENTOA_TAB.Count; i++)
                                    //    {
                                    //        if (a.Name.Trim() == doc.DOCUMENTOA_TAB[i].NAME.Trim())
                                    //        {
                                    //            try
                                    //            {
                                    //                listaDescArchivos.Add(labels_desc[indexlabel]);
                                    //                listaDescArchivos3.Add(labels_desc[indexlabel]);
                                    //            }
                                    //            catch (Exception ex)
                                    //            {
                                    //                descripcion = "";
                                    //                listaDescArchivos.Add(descripcion);
                                    //                listaDescArchivos3.Add(descripcion);
                                    //            }

                                    //            try
                                    //            {
                                    //                listaNombreArchivos.Add(a.Name);
                                    //                listaNombreArchivos3.Add(a.Name);
                                    //            }
                                    //            catch (Exception ex)
                                    //            {
                                    //                listaNombreArchivos.Add("");
                                    //                listaNombreArchivos3.Add("");
                                    //            }

                                    //            string errorfiles = "";

                                    //            var url_prel = "";
                                    //            var dirFile = "";
                                    //            string carpeta = "att";
                                    //            try
                                    //            {
                                    //                url_prel = getDirPrel(carpeta, doc.NUM_DOC);
                                    //                dirFile = url_prel;
                                    //            }
                                    //            catch (Exception e)
                                    //            {
                                    //                dirFile = ConfigurationManager.AppSettings["URL_ATT"].ToString() + @"att";
                                    //            }

                                    //            bool existd = ValidateIOPermission(dirFile);

                                    //            if (existd)
                                    //            {
                                    //                if (!System.IO.File.Exists(dirFile + "\\" + a.Name))
                                    //                {
                                    //                    a.CopyTo(dirFile + "\\" + a.Name);
                                    //                    listaDirectorios.Add(dirFile + "\\" + a.Name);
                                    //                    listaDirectorios3.Add(dirFile + "\\" + a.Name);
                                    //                }
                                    //            }
                                    //            indexlabel++;
                                    //            if (errorfiles != "")
                                    //            {
                                    //                errorMessage += "Error con el archivo " + errorfiles;
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    System.IO.Directory.Delete(server, true);
                                    //END FRT13112018
                                }


                            }
                            catch (Exception e)
                            {
                                // errorMessage = dir;
                            }

                            errorString = errorMessage;
                            //Guardar número de documento creado
                            Session["ERROR_FILES"] = errorMessage;
                        }
                    }
                    catch (Exception e)
                    {

                    }



                    if (filenull)
                    {
                        List<string> listaDirectorios2 = listaDirectorios;
                        List<string> listaNombreArchivos2 = listaNombreArchivos;
                        List<string> listaDescArchivos2 = listaDescArchivos;




                        var listafiles = listaNombreArchivos.Count;//FRT16112018
                        var _name = "";//FRT20112018 NOMBRE PAERA QUIATR DE LA LISTA
                                       //DOCUMENTOA
                                       //Misma cantidad de archivos y nombres, osea todo bien
                        if (listaDirectorios.Count == listaDescArchivos.Count && listaDirectorios.Count == listaNombreArchivos.Count)
                        {
                            //un contador para los archivos que se borran de las listas
                            int arBorr = 0;
                            for (int i = 0; i < doc.Anexo.Count; i++)
                            {
                                var pos = 1;
                                DOCUMENTOA _dA = new DOCUMENTOA();
                                if (doc.Anexo[i].a1 != 0)
                                {
                                    _dA.NUM_DOC = doc.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    var de = "";
                                    int a1 = 0;
                                    int a11 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                    if (doc.Anexo[i].a1 > 0 && doc.Anexo[i].a1 <= listafiles)  //FRT16112018
                                    {
                                        a1 = doc.Anexo[i].a1;
                                        a1 = a1 - 1;

                                        a11 = doc.Anexo[i].a1;
                                        a11 = a11 - arBorr;
                                        a11 = a11 - 1;
                                        _dA.POS = doc.Anexo[i].a1;

                                        try
                                        {
                                            _name = "";
                                            _name = listaNombreArchivos3[a1];
                                            de = Path.GetExtension(listaNombreArchivos3[a1]);
                                            //de = Path.GetExtension(listaNombreArchivos[a1]);
                                            _dA.TIPO = de.Replace(".", "");
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.TIPO = "";
                                        }
                                        try
                                        {
                                            _dA.DESC = listaDescArchivos3[a1];
                                            //_dA.DESC = listaDescArchivos[a1];
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.DESC = "";
                                        }
                                        try
                                        {
                                            _dA.PATH = listaDirectorios3[a1];
                                            //_dA.PATH = listaDirectorios[a1];
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.PATH = "";
                                        }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }

                                        //listaDescArchivos2.RemoveAt(a11);
                                        listaNombreArchivos2.Remove(_name);
                                        //listaNombreArchivos2.RemoveAt(a11);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                                _dA = new DOCUMENTOA();
                                if (doc.Anexo[i].a2 != 0)
                                {
                                    _dA.NUM_DOC = doc.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    var de = "";
                                    int a2 = 0;
                                    int a12 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5

                                    if (doc.Anexo[i].a2 > 0 && doc.Anexo[i].a2 <= listafiles)  //FRT16112018
                                                                                               //if (doc.Anexo[i].a2 > 0 && doc.Anexo[i].a2 <= listaNombreArchivos.Count)
                                    {
                                        a2 = doc.Anexo[i].a2;
                                        a2 = a2 - 1;


                                        a12 = doc.Anexo[i].a2;
                                        a12 = a12 - arBorr;
                                        a12 = a12 - 1;
                                        _dA.POS = doc.Anexo[i].a2;
                                        try
                                        {


                                            _name = "";
                                            _name = listaNombreArchivos3[a2];
                                            //de = Path.GetExtension(listaNombreArchivos[a2]);
                                            de = Path.GetExtension(listaNombreArchivos3[a2]);
                                            _dA.TIPO = de.Replace(".", "");
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.TIPO = "";
                                        }
                                        try
                                        {
                                            //_dA.DESC = listaDescArchivos[a2];
                                            _dA.DESC = listaDescArchivos3[a2];
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.DESC = "";
                                        }
                                        try
                                        {
                                            //_dA.PATH = listaDirectorios[a2];
                                            _dA.PATH = listaDirectorios3[a2];
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.PATH = "";
                                        }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        //listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a12);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        //listaNombreArchivos2.RemoveAt(a12);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                                _dA = new DOCUMENTOA();
                                if (doc.Anexo[i].a3 != 0)
                                {
                                    _dA.NUM_DOC = doc.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    var de = "";
                                    int a3 = 0;
                                    int a13 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                    if (doc.Anexo[i].a3 > 0 && doc.Anexo[i].a3 <= listafiles)  //FRT16112018
                                    {
                                        a3 = doc.Anexo[i].a3;
                                        a3 = a3 - 1;


                                        a13 = doc.Anexo[i].a3;
                                        a13 = a13 - arBorr;
                                        a13 = a13 - 1;
                                        _dA.POS = doc.Anexo[i].a3;
                                        try
                                        {
                                            try
                                            {
                                                _name = "";
                                                _name = listaNombreArchivos3[a3];
                                                //de = Path.GetExtension(listaNombreArchivos[a3]);
                                                de = Path.GetExtension(listaNombreArchivos3[a3]);
                                                _dA.TIPO = de.Replace(".", "");
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.TIPO = "";
                                            }
                                            try
                                            {
                                                //_dA.DESC = listaDescArchivos[a3];
                                                _dA.DESC = listaDescArchivos3[a3];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.DESC = "";
                                            }
                                            try
                                            {
                                                //_dA.PATH = listaDirectorios[a3];
                                                _dA.PATH = listaDirectorios3[a3];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.PATH = "";
                                            }
                                        }
                                        catch (Exception e)
                                        { }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        ////listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a13);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        //listaNombreArchivos2.RemoveAt(a13);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                                _dA = new DOCUMENTOA();
                                if (doc.Anexo[i].a4 != 0)
                                {
                                    _dA.NUM_DOC = doc.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    var de = "";
                                    int a4 = 0;
                                    int a14 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                    if (doc.Anexo[i].a4 > 0 && doc.Anexo[i].a4 <= listafiles)  //FRT16112018
                                    {
                                        a4 = doc.Anexo[i].a4;
                                        a4 = a4 - 1;


                                        a14 = doc.Anexo[i].a4;
                                        a14 = a14 - arBorr;
                                        a14 = a14 - 1;
                                        _dA.POS = doc.Anexo[i].a4;
                                        try
                                        {
                                            try
                                            {
                                                _name = "";
                                                _name = listaNombreArchivos3[a4];
                                                de = Path.GetExtension(listaNombreArchivos3[a4]);
                                                _dA.TIPO = de.Replace(".", "");
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.TIPO = "";
                                            }
                                            try
                                            {
                                                _dA.DESC = listaDescArchivos3[a4];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.DESC = "";
                                            }
                                            try
                                            {
                                                _dA.PATH = listaDirectorios3[a4];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.PATH = "";
                                            }
                                        }
                                        catch (Exception e) { }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        ////listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a14);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                                _dA = new DOCUMENTOA();
                                if (doc.Anexo[i].a5 != 0)
                                {
                                    _dA.NUM_DOC = doc.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    _dA.POS = pos;
                                    var de = "";
                                    int a5 = 0;
                                    int a15 = 0;
                                    if (doc.Anexo[i].a5 > 0 && doc.Anexo[i].a5 <= listafiles)  //FRT16112018
                                    {
                                        a5 = doc.Anexo[i].a5;
                                        a5 = a5 - 1;
                                        a15 = doc.Anexo[i].a5;
                                        a15 = a15 - arBorr;
                                        a15 = a15 - 1;
                                        _dA.POS = doc.Anexo[i].a5;
                                        try
                                        {
                                            try
                                            {
                                                _name = "";
                                                _name = listaNombreArchivos3[a5];
                                                //de = Path.GetExtension(listaNombreArchivos[a5]);
                                                de = Path.GetExtension(listaNombreArchivos3[a5]);
                                                _dA.TIPO = de.Replace(".", "");
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.TIPO = "";
                                            }
                                            try
                                            {
                                                //_dA.DESC = listaDescArchivos[a5];
                                                _dA.DESC = listaDescArchivos3[a5];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.DESC = "";
                                            }
                                            try
                                            {
                                                _dA.PATH = listaDirectorios3[a5];
                                                //_dA.PATH = listaDirectorios[a5];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.PATH = "";
                                            }
                                        }
                                        catch (Exception e) { }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        ////listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a15);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                            }
                        }
                        //Lejgg 26.10.2018---------------------------------------->
                        //Los anexos que no se agreguen a documentoa se agregaran a documentoas(documentoa1), significa que estan en lista porque no se ligaron a ningun detalle
                        if (listaDirectorios2.Count == listaDescArchivos2.Count && listaDirectorios2.Count == listaNombreArchivos2.Count)
                        {
                            var pos = 1;
                            for (int i = 0; i < listaDescArchivos2.Count; i++)
                            {
                                DOCUMENTOA1 _dA = new DOCUMENTOA1();
                                _dA.NUM_DOC = doc.NUM_DOC;
                                _dA.POS = pos;
                                var de = "";
                                try
                                {
                                    de = Path.GetExtension(listaNombreArchivos2[i]);
                                    _dA.TIPO = de.Replace(".", "");
                                }
                                catch (Exception c)
                                {
                                    _dA.TIPO = "";
                                }
                                try
                                {
                                    _dA.DESC = listaDescArchivos2[i];
                                }
                                catch (Exception c)
                                {
                                    _dA.DESC = "";
                                }
                                try
                                {
                                    _dA.PATH = listaDirectorios2[i];
                                }
                                catch (Exception c)
                                {
                                    _dA.PATH = "";
                                }
                                _dA.CLASE = "OTR";
                                _dA.STEP_WF = 1;
                                _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                _dA.ACTIVO = true;
                                try
                                {
                                    db.DOCUMENTOAS1.Add(_dA);
                                    db.SaveChanges();
                                    pos++;
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }
                        }
                        //Lejgg 26.10.2018----------------------------------------<
                        //Lejgg 28.10.2018---------------------------------------->
                        Session["Temporal"] = null;
                        //ENDFRT25112018


                    }




                    //Guardar las retenciones por posición
                    //Lej14.09.2018------
                    try
                    {

                        if (doc.DOCUMENTORP != null)
                        {
                            for (int i = 0; i < doc.DOCUMENTORP.Count; i++)
                            {
                                //cantdidad de renglones añadidos y su posicion
                                var _op = ((i + 2) / 2);
                                var _pos = _op.ToString().Split('.');
                                try
                                {
                                    var docr = doc.DOCUMENTOR;
                                    var _str = doc.DOCUMENTORP[i].WITHT;
                                    var ret = db.RETENCIONs.Where(x => x.WITHT == _str).FirstOrDefault().WITHT_SUB;
                                    if (ret != null)
                                    {
                                        bool f = false;
                                        for (int _a = 0; _a < docr.Count; _a++)
                                        {
                                            //si encuentra coincidencia
                                            if (docr[_a].WITHT == ret)
                                            {
                                                DOCUMENTORP _dr = new DOCUMENTORP();
                                                _dr.NUM_DOC = doc.NUM_DOC;
                                                _dr.WITHT = doc.DOCUMENTORP[i].WITHT;
                                                _dr.WT_WITHCD = doc.DOCUMENTORP[i].WT_WITHCD;
                                                _dr.POS = int.Parse(_pos[0]);
                                                _dr.BIMPONIBLE = doc.DOCUMENTORP[i].BIMPONIBLE;
                                                _dr.IMPORTE_RET = doc.DOCUMENTORP[i].IMPORTE_RET;
                                                db.DOCUMENTORPs.Add(_dr);
                                                db.SaveChanges();
                                                _dr = new DOCUMENTORP();
                                                _dr.NUM_DOC = doc.NUM_DOC;
                                                _dr.WITHT = docr[_a].WITHT;
                                                _dr.WT_WITHCD = doc.DOCUMENTORP[i].WT_WITHCD;
                                                _dr.POS = int.Parse(_pos[0]);
                                                _dr.BIMPONIBLE = doc.DOCUMENTORP[i].BIMPONIBLE;
                                                _dr.IMPORTE_RET = doc.DOCUMENTORP[i].IMPORTE_RET;
                                                db.DOCUMENTORPs.Add(_dr);
                                                db.SaveChanges();
                                                f = true;
                                            }
                                        }
                                        if (!f)
                                        {
                                            DOCUMENTORP _dr = new DOCUMENTORP();
                                            _dr.NUM_DOC = doc.NUM_DOC;
                                            _dr.WITHT = doc.DOCUMENTORP[i].WITHT;
                                            _dr.WT_WITHCD = doc.DOCUMENTORP[i].WT_WITHCD;
                                            _dr.POS = int.Parse(_pos[0]);
                                            _dr.BIMPONIBLE = doc.DOCUMENTORP[i].BIMPONIBLE;
                                            _dr.IMPORTE_RET = doc.DOCUMENTORP[i].IMPORTE_RET;
                                            db.DOCUMENTORPs.Add(_dr);
                                            db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        DOCUMENTORP dr = new DOCUMENTORP();
                                        dr.NUM_DOC = doc.NUM_DOC;
                                        dr.WITHT = doc.DOCUMENTORP[i].WITHT;
                                        dr.WT_WITHCD = doc.DOCUMENTORP[i].WT_WITHCD;
                                        dr.POS = int.Parse(_pos[0]);
                                        dr.BIMPONIBLE = doc.DOCUMENTORP[i].BIMPONIBLE;
                                        dr.IMPORTE_RET = doc.DOCUMENTORP[i].IMPORTE_RET;
                                        db.DOCUMENTORPs.Add(dr);
                                        db.SaveChanges();
                                    }
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        }


                    }
                    catch (Exception e)
                    {
                        //
                    }
                    //Lej14.09.2018------

                    //Guardar las retenciones en el encabezado
                    try//LEJ 05.09.2018
                    {

                        if (doc.DOCUMENTOR != null)
                        {
                            List<DOCUMENTOR_MOD> docrr = new List<DOCUMENTOR_MOD>();

                            List<RETENCION> retsub = new List<RETENCION>();

                            retsub = (from dr in doc.DOCUMENTOR.ToList()
                                      join ret1 in db.RETENCIONs.ToList()
                                      on new { dr.WITHT, dr.WT_WITHCD } equals new { ret1.WITHT, ret1.WT_WITHCD }
                                      select new RETENCION
                                      {
                                          WITHT = ret1.WITHT,
                                          WT_WITHCD = ret1.WT_WITHCD,
                                          DESCRIPCION = ret1.DESCRIPCION,
                                          ESTATUS = ret1.ESTATUS,
                                          WITHT_SUB = ret1.WITHT_SUB,
                                          PORC = ret1.PORC,
                                          WT_WITHCD_SUB = ret1.WT_WITHCD_SUB,
                                          CAMPO = ret1.CAMPO
                                      }
                                   ).ToList();
                            //Guardar las retenciones de la solicitud
                            int ccr = 1;// Contador consecutivo ////MGC 29-10-2018
                            for (int i = 0; i < doc.DOCUMENTOR.Count; i++)
                            {
                                if (doc.DOCUMENTOR[i].BUKRS == doc.SOCIEDAD_ID && doc.DOCUMENTOR[i].LIFNR == doc.PAYER_ID)
                                {
                                    DOCUMENTOR dr = new DOCUMENTOR();
                                    try
                                    {

                                        //dr.NUM_DOC = decimal.Parse(Session["NUM_DOC"].ToString());
                                        dr.NUM_DOC = doc.NUM_DOC;
                                        dr.WITHT = doc.DOCUMENTOR[i].WITHT;
                                        dr.WT_WITHCD = doc.DOCUMENTOR[i].WT_WITHCD;
                                        //dr.POS = db.DOCUMENTORs.ToList().Count + 1; // Contador consecutivo ////MGC 29-10-2018                                    
                                        dr.POS = ccr; // Contador consecutivo ////MGC 29-10-2018
                                        dr.BIMPONIBLE = doc.DOCUMENTOR[i].BIMPONIBLE;
                                        dr.IMPORTE_RET = doc.DOCUMENTOR[i].IMPORTE_RET;
                                        dr.VISIBLE = true;
                                        db.DOCUMENTORs.Add(dr);
                                        db.SaveChanges();

                                        ////MGC 29-10-2018 Obtener las retenciones relacionadas con las ya mostradas en la tabla
                                        ccr++;// Contador consecutivo ////MGC 29-10-2018
                                    }
                                    catch (Exception e)
                                    {
                                    }

                                    //Obtener la relacionada
                                    RETENCION retrel = retsub.Where(rs => rs.WITHT == doc.DOCUMENTOR[i].WITHT && rs.WT_WITHCD == doc.DOCUMENTOR[i].WT_WITHCD).FirstOrDefault();

                                    if (retrel != null)
                                    {
                                        if (retrel.WITHT_SUB != null | retrel.WITHT_SUB != "")
                                        {
                                            DOCUMENTOR drr = new DOCUMENTOR();
                                            try
                                            {

                                                drr.NUM_DOC = doc.NUM_DOC;

                                                drr.WITHT = retrel.WITHT_SUB;
                                                drr.WT_WITHCD = retrel.WT_WITHCD_SUB;
                                                //dr.POS = db.DOCUMENTORs.ToList().Count + 1; // Contador consecutivo ////MGC 29-10-2018                                    
                                                drr.POS = ccr; // Contador consecutivo ////MGC 29-10-2018
                                                drr.BIMPONIBLE = doc.DOCUMENTOR[i].BIMPONIBLE;
                                                drr.IMPORTE_RET = doc.DOCUMENTOR[i].IMPORTE_RET;
                                                drr.VISIBLE = false;
                                                db.DOCUMENTORs.Add(drr);
                                                db.SaveChanges();

                                                ////MGC 29-10-2018 Obtener las retenciones relacionadas con las ya mostradas en la tabla
                                                ccr++;// Contador consecutivo ////MGC 29-10-2018
                                                      //LEJGG 25-11-2018----------------------------->
                                                      //despues de guardarlo eliminamos de la lista de documentor la ligada si es que llegara a tener
                                                bool bdr = false;
                                                for (int x = 0; x < doc.DOCUMENTOR.Count; x++)
                                                {
                                                    //buscara si en la lista tiene esa retencion ligada
                                                    if (doc.DOCUMENTOR[x].WITHT == drr.WITHT)
                                                    {
                                                        //si la tiene, cambio la bandera a true, y salgo del ciclo
                                                        bdr = true; break;
                                                    }
                                                }
                                                //si es true, significa que esta en la lista y la eliminara
                                                if (bdr)
                                                {
                                                    int indice = doc.DOCUMENTOR.FindIndex(x => x.WITHT == drr.WITHT);
                                                    doc.DOCUMENTOR.RemoveAt(indice);
                                                }
                                                //LEJGG 25-11-2018-----------------------------<
                                            }
                                            catch (Exception e)
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        ////MGC 29-10-2018 Obtener las retenciones relacionadas con las ya mostradas en la tabla------------------------------>

                    }
                    catch (Exception e)
                    {
                        errorString = e.Message.ToString();
                        //Guardar número de documento creado

                    }


                    //Lej26.09.2018------

                    //FRT25112018 aQUI QUITE PARA ANEXOS

                    //Lej26.09.2018------
                    if (borr == string.Empty)
                    {
                        //MGC 02-10-2018 Cadena de autorización work flow --->
                        //Flujo
                        ProcesaFlujo pf = new ProcesaFlujo();
                        //Comienza el wf
                        //Se obtiene la cabecera
                        try
                        {
                            WORKFV wf = db.WORKFHs.Where(a => a.TSOL_ID.Equals(dOCUMENTO.TSOL_ID)).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();

                            DET_AGENTECC deta = new DET_AGENTECC();
                            try
                            {
                                deta.VERSION = Convert.ToInt32(DETTA_VERSION);
                            }
                            catch (Exception e)
                            {

                            }

                            try
                            {
                                deta.USUARIOC_ID = DETTA_USUARIOC_ID;
                            }
                            catch (Exception e)
                            {

                            }


                            try
                            {
                                deta.ID_RUTA_AGENTE = DETTA_ID_RUTA_AGENTE;
                            }
                            catch (Exception e)
                            {

                            }

                            try
                            {
                                deta.USUARIOA_ID = DETTA_USUARIOA_ID;
                            }
                            catch (Exception e)
                            {

                            }

                            //MGC 11-12-2018 Agregar Contabilizador 0------>
                            int vc1 = 0;
                            int vc2 = 0;
                            try
                            {
                                vc1 = Convert.ToInt32(VERSIONC1);
                            }
                            catch (Exception e)
                            {

                            }

                            try
                            {
                                vc2 = Convert.ToInt32(VERSIONC2);
                            }
                            catch (Exception e)
                            {

                            }
                            //MGC 11-12-2018 Agregar Contabilizador 0------<

                            if (wf != null)
                            {
                                WORKFP wp = wf.WORKFPs.OrderBy(a => a.POS).FirstOrDefault();
                                string email = ""; //MGC 08-10-2018 Obtener el nombre del cliente
                                email = wp.EMAIL; //MGC 08-10-2018 Obtener el nombre del cliente


                                FLUJO f = new FLUJO();
                                f.WORKF_ID = wf.ID;
                                f.WF_VERSION = wf.VERSION;
                                f.WF_POS = wp.POS;
                                f.NUM_DOC = dOCUMENTO.NUM_DOC;
                                f.POS = 1;
                                f.LOOP = 1;
                                f.USUARIOA_ID = dOCUMENTO.USUARIOC_ID;
                                f.USUARIOD_ID = dOCUMENTO.USUARIOD_ID;
                                f.ESTATUS = "I";
                                f.FECHAC = DateTime.Now;
                                f.FECHAM = DateTime.Now;
                                f.STEP_AUTO = 0;

                                //Ruta tomada
                                f.ID_RUTA_A = deta.ID_RUTA_AGENTE;
                                f.RUTA_VERSION = deta.VERSION;

                                //MGC 11-12-2018 Agregar Contabilizador 0
                                f.VERSIONC1 = vc1;
                                f.VERSIONC2 = vc2;


                                //MGC 05-10-2018 Modificación para work flow al ser editada
                                string c = pf.procesa(f, "", false, email, "");
                            }
                        }
                        catch (Exception ee)
                        {
                            if (errorString == "")
                            {
                                errorString = ee.Message.ToString();
                            }
                            ViewBag.error = errorString;
                        }
                        //MGC 02-10-2018 Cadena de autorización work flow <---
                    }
                    //Lej-02.10.2018------

                    //FRT25112018 AQUI QUITE PARA ANEXOS

                    if (Uuid != string.Empty)
                    {
                        var pos = db.DOCUMENTOUUIDs.ToList();
                        DOCUMENTOUUID duid = new DOCUMENTOUUID();
                        duid.NUM_DOC = doc.NUM_DOC;
                        duid.POS = pos.Count + 1;
                        duid.DOCUMENTO_SAP = "";
                        duid.UUID = Uuid;
                        duid.ESTATUS = true;
                        db.DOCUMENTOUUIDs.Add(duid);
                        db.SaveChanges();
                    }
                    //Lejgg 28.10.2018----------------------------------------<
                }
                catch (Exception e)
                {
                    errorString += e.Message.ToString();
                }


                return RedirectToAction("Index", "Home");
            }
            else
            {

                string validationErrors = string.Join(",",
                    ModelState.Values.Where(E => E.Errors.Count > 0)
                    .SelectMany(E => E.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray());

                errorString += validationErrors;
            }
            ViewBag.error = errorString;

            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            spras = user.SPRAS_ID;
            ViewBag.returnUrl = Request.Url.PathAndQuery;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

            //Obtener miles y dec
            formato = db.FORMATOes.Where(f => f.ACTIVO == true).FirstOrDefault();
            ViewBag.miles = formato.MILES;
            ViewBag.dec = formato.DECIMALES;

            //Obtener las sociedadess
            //List<SOCIEDAD> sociedadesl = new List<SOCIEDAD>();
            var sociedades = db.SOCIEDADs.Select(s => new { s.BUKRS, TEXT = s.BUKRS + " - " + s.BUTXT }).ToList();
            var tsoll = (from ts in db.TSOLs
                         join tt in db.TSOLTs
                         on ts.ID equals tt.TSOL_ID
                         into jj
                         from tt in jj.DefaultIfEmpty()
                         where ts.ESTATUS == "X" && tt.SPRAS_ID.Equals(spras)
                         select new
                         {
                             ts.ID,
                             TEXT = ts.ID + " - " + tt.TXT50
                         }).ToList();

            var monedal = db.MONEDAs.Where(m => m.ACTIVO == true).Select(m => new { m.WAERS, TEXT = m.WAERS + " - " + m.LTEXT }).ToList();

            var impuestol = db.IMPUESTOes.Where(i => i.ACTIVO == true).Select(i => new { i.MWSKZ });

            ViewBag.SOCIEDAD_ID = new SelectList(sociedades, "BUKRS", "TEXT", doc.SOCIEDAD_ID);
            ViewBag.TSOL_ID = new SelectList(tsoll, "ID", "TEXT", doc.TSOL_ID);
            ViewBag.IMPUESTO = new SelectList(impuestol, "MWSKZ", "MWSKZ", doc.IMPUESTO);
            ViewBag.MONEDA_ID = new SelectList(monedal, "WAERS", "TEXT", doc.MONEDA_ID);

            return View(doc);
        }



        // GET: Solicitudes/Edit/5
        public ActionResult Edit(decimal id, string pacc)
        {

            Session["NUM_DOC_TEM"] = id;
            //lEJGG 7-11-18----------------------->
            int pagina = 204;//lEJGG 7-11-18
            if (pacc == "B")
            {
                pagina = 209; //ID EN BASE DE DATOS para borrador
            }
            else
            {
                pagina = 204; //ID EN BASE DE DATOS
            }
            //lEJGG 7-11-18-----------------------<
            FORMATO formato = new FORMATO();
            string spras = "";
            string user_id = "";//MGC 02-10-2018 Cadena de autorización
            string pselG = "";//MGC 16-10-2018 Obtener las sociedades asignadas al usuario
            using (WFARTHAEntities db = new WFARTHAEntities())
            {

                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                spras = user.SPRAS_ID;
                user_id = user.ID;//MGC 02-10-2018 Cadena de autorización
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title += " "; //MGC 05-10-2018 Modificación para work flow al ser editada
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                //Obtener miles y dec
                formato = db.FORMATOes.Where(f => f.ACTIVO == true).FirstOrDefault();
                ViewBag.miles = formato.MILES;
                ViewBag.dec = formato.DECIMALES;

            }

            //Lejgg05-11-2018------------------
            var soc_id = db.DOCUMENTOes.Where(n => n.NUM_DOC == id).FirstOrDefault().SOCIEDAD_ID;
            var _proy = db.DET_PROYECTOV.Where(x => x.ID_BUKRS == soc_id).FirstOrDefault();
            var _nameprov = db.PROYECTOes.Where(x => x.ID_PSPNR == _proy.ID_PSPNR).FirstOrDefault();
            Session["pr"] = _nameprov.NOMBRE;
            Session["id_pr"] = _proy.ID_PSPNR;
            Session["Edit"] = 1;  //FRT08112018 para mostrar pantalla de proyectos en edit

            //Lejgg05-11-2018-------------------
            if (pacc == null)
            {
                if (Session["pacc"] != null)
                {
                    ViewBag.pacc = Session["pacc"].ToString();
                }
            }
            else
            {
                ViewBag.pacc = pacc;
                Session["pacc"] = pacc;
            }

            try
            {
                /* string p = Session["pr"].ToString();
                 string pid = Session["id_pr"].ToString();
                 ViewBag.PrSl = p;
                 pselG = pid;//MGC 16-10-2018 Obtener las sociedades asignadas al usuario
                 ViewBag.pid = pid;//MGC 29-10-2018 Guardar el proyecto en el create*/

            }
            catch
            {
                //ViewBag.pais = "mx.png";
                //return RedirectToAction("Proyectos", "Home", new { returnUrl = Request.Url.AbsolutePath });
            }
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            var id_pspnr = dOCUMENTO.ID_PSPNR;
            var nombre = db.PROYECTOes.Where(x => x.ID_PSPNR == id_pspnr).FirstOrDefault().NOMBRE; //FRT03122018
            //var nombre = "Proyecto Prueba";
            ViewBag.PrSl = id_pspnr;
            ViewBag.pid = nombre;
            if (dOCUMENTO.TIPO_CAMBIO == null)//Lejgg 15-11-2018
            { dOCUMENTO.TIPO_CAMBIO = 0; }
            //Documento a documento mod //Copiar valores del post al nuevo objeto
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }

            //lejgg 05 10 2018
            if (dOCUMENTO.DOCUMENTOPs != null)
            {
                List<DOCUMENTOP_MODSTR> dml = new List<DOCUMENTOP_MODSTR>();
                FormatosC fc = new FormatosC();
                //Agregar a documento p_mod para agregar valores faltantes
                for (int i = 0; i < dOCUMENTO.DOCUMENTOPs.Count; i++)
                {
                    DOCUMENTOP_MODSTR dm = new DOCUMENTOP_MODSTR();

                    dm.NUM_DOC = dOCUMENTO.DOCUMENTOPs.ElementAt(i).NUM_DOC;
                    dm.POS = dOCUMENTO.DOCUMENTOPs.ElementAt(i).POS;
                    dm.ACCION = dOCUMENTO.DOCUMENTOPs.ElementAt(i).ACCION;
                    dm.FACTURA = dOCUMENTO.DOCUMENTOPs.ElementAt(i).FACTURA;
                    dm.GRUPO = dOCUMENTO.DOCUMENTOPs.ElementAt(i).GRUPO;
                    dm.CUENTA = dOCUMENTO.DOCUMENTOPs.ElementAt(i).CUENTA;
                    string ct = dOCUMENTO.DOCUMENTOPs.ElementAt(i).GRUPO;
                    var tct = dOCUMENTO.DOCUMENTOPs.ElementAt(i).TCONCEPTO;
                    try
                    {
                        dm.NOMCUENTA = db.CONCEPTOes.Where(x => x.ID_CONCEPTO == ct && x.TIPO_CONCEPTO == tct).FirstOrDefault().DESC_CONCEPTO.Trim();
                    }
                    catch (Exception e)
                    {
                        dm.NOMCUENTA = "Transporte";
                    }
                    dm.TIPOIMP = dOCUMENTO.DOCUMENTOPs.ElementAt(i).TIPOIMP;
                    dm.IMPUTACION = dOCUMENTO.DOCUMENTOPs.ElementAt(i).IMPUTACION;
                    dm.MONTO = fc.toShow(dOCUMENTO.DOCUMENTOPs.ElementAt(i).MONTO, formato.DECIMALES);
                    dm.IVA = fc.toShow(dOCUMENTO.DOCUMENTOPs.ElementAt(i).IVA, formato.DECIMALES);
                    dm.TEXTO = dOCUMENTO.DOCUMENTOPs.ElementAt(i).TEXTO;
                    dm.TOTAL = fc.toShow(dOCUMENTO.DOCUMENTOPs.ElementAt(i).TOTAL, formato.DECIMALES);

                    dml.Add(dm);
                }


                var _t = db.DOCUMENTOPs.Where(x => x.NUM_DOC == id && x.ACCION != "H").ToList();
                decimal _total = 0;
                for (int i = 0; i < _t.Count; i++)
                {
                    _total = _total + _t[i].TOTAL;
                }
                ViewBag.total = _total;
                doc.DOCUMENTOPSTR = dml;
            }
            //lejgg 05 10 2018
            //Obtener las sociedadess
            var sociedades = db.SOCIEDADs.Select(s => new { s.BUKRS, TEXT = s.BUKRS + " - " + s.BUTXT }).ToList();

            ViewBag.Title += id; //MGC 05-10-2018 Modificación para work flow al ser editada
            ViewBag.ndoc = id;

            //LEJGG19 10 2018----------------------------------------------------->

            var lst = db.DOCUMENTOAs.Where(a => a.NUM_DOC == id && a.PATH != "" && a.ACTIVO == true).OrderBy(a => a.POS).ToList(); //FRT05122018 para ordenar corecto los anexos
            var originales = lst.Select(w => w.POS).Distinct().ToList(); //FRT06122018

            DOCUMENTOA d_a = new DOCUMENTOA();
            var la1 = db.DOCUMENTOAS1.Where(a => a.NUM_DOC == id && a.PATH != "" && a.ACTIVO == true).ToList();
            var files = la1.Count + originales.Count;
            var anexar = 0;


            if (la1.Count > 0)
            {
                for (int h = 1; h < files + 1; h++)
                {
                    var _f = false;
                    for (int p = 0; p < lst.Count; p++)
                    {
                        if (h == lst[p].POS)
                        {
                            _f = true;
                            break;
                        }
                    }
                    if (!_f)
                    {
                        d_a = new DOCUMENTOA();
                        d_a.POS = h;
                        d_a.NUM_DOC = la1[anexar].NUM_DOC;
                        d_a.TIPO = la1[anexar].TIPO;
                        d_a.DESC = la1[anexar].DESC;
                        d_a.CLASE = la1[anexar].CLASE;
                        d_a.PATH = la1[anexar].PATH;
                        lst.Add(d_a);
                        anexar++;

                    }
                }

            }





            ////for (int i = 0; i < la1.Count; i++)
            ////{
            ////    d_a = new DOCUMENTOA();
            ////    d_a.POS = i;
            ////    d_a.NUM_DOC = la1[i].NUM_DOC;
            ////    d_a.TIPO = la1[i].TIPO;
            ////    d_a.DESC = la1[i].DESC;
            ////    d_a.CLASE = la1[i].CLASE;
            ////    d_a.PATH = la1[i].PATH;
            ////    lst.Add(d_a);
            ////}


            lst = lst.OrderBy(a => a.POS).ToList();//FRT05122018 para ordenar corecto los anexos

            List<DOCUMENTOA> result = new List<DOCUMENTOA>();

            for (int i = 0; i < lst.Count; i++)
            {
                // Assume not duplicate.
                bool duplicate = false;
                for (int z = 0; z < i; z++)
                {
                    if (lst[z].PATH == lst[i].PATH)
                    {
                        // This is a duplicate.
                        duplicate = true;
                        break;
                    }
                }
                // If not duplicate, add to result.
                if (!duplicate)
                {
                    result.Add(lst[i]);
                }
            }





            //ViewBag.docAn = lst;
            ViewBag.docAn = result;
            //ViewBag.docAn2 = db.DOCUMENTOAS1.Where(a => a.NUM_DOC == id).ToList(); //LEJGG 28-10 2018
            //LEJGG19 10 2018-----------------------------------------------------<
            //solicitud con orden de compra ------>
            //Obtener la solicitud con la configuración
            var tsoll = (from ts in db.TSOLs
                         join tt in db.TSOLTs
                         on ts.ID equals tt.TSOL_ID
                         into jj
                         from tt in jj.DefaultIfEmpty()
                         where ts.ESTATUS == "X" && tt.SPRAS_ID.Equals(spras)
                         select new
                         {
                             //ts.ID,
                             ID = new { ID = ts.ID.ToString().Replace(" ", ""), RANGO = ts.RANGO_ID.ToString().Replace(" ", ""), EDITDET = ts.EDITDET.ToString().Replace(" ", "") },
                             TEXT = ts.ID + " - " + tt.TXT50,
                             DEFAULT = ts.DEFAULT
                         }).ToList();
            //Obtener el valor default
            var tsolldef = tsoll.Where(tsd => tsd.DEFAULT == true).Select(tsds => tsds.ID).FirstOrDefault();
            //solicitud con orden de compra <------

            var monedal = db.MONEDAs.Where(m => m.ACTIVO == true).Select(m => new { m.WAERS, TEXT = m.WAERS + " - " + m.LTEXT }).ToList();

            var impuestol = (from im in db.IMPUESTOes
                             join imt in db.IMPUESTOTs.Where(imtt => imtt.SPRAS_ID == spras)
                             on im.MWSKZ equals imt.MWSKZ
                             into jj
                             from tt in jj.DefaultIfEmpty()
                             where im.ACTIVO == true
                             select new
                             {
                                 im.MWSKZ,
                                 TEXT = im.MWSKZ + " - " + tt.TXT50
                             }).ToList();

            ViewBag.SOCIEDAD_ID = new SelectList(sociedades, "BUKRS", "TEXT", dOCUMENTO.SOCIEDAD_ID);
            ViewBag.TSOL_IDL = new SelectList(tsoll, "ID", "TEXT", selectedValue: tsolldef);
            ViewBag.IMPUESTO = new SelectList(impuestol, "MWSKZ", "TEXT", "V3");
            ViewBag.MONEDA_ID = new SelectList(monedal, "WAERS", "TEXT");
            //LEJ 04 10 2018------------------------------
            doc.NUM_DOC = dOCUMENTO.NUM_DOC;
            doc.TSOL_ID = dOCUMENTO.TSOL_ID;
            doc.SOCIEDAD_ID = dOCUMENTO.SOCIEDAD_ID;
            doc.FECHAD = dOCUMENTO.FECHAD;
            doc.FECHACON = dOCUMENTO.FECHACON;
            doc.FECHA_BASE = dOCUMENTO.FECHA_BASE;
            doc.MONEDA_ID = dOCUMENTO.MONEDA_ID;
            doc.TIPO_CAMBIO = dOCUMENTO.TIPO_CAMBIO;
            doc.IMPUESTO = dOCUMENTO.IMPUESTO;
            doc.MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
            doc.REFERENCIA = dOCUMENTO.REFERENCIA;
            doc.CONCEPTO = dOCUMENTO.CONCEPTO;
            doc.PAYER_ID = dOCUMENTO.PAYER_ID;
            doc.CONDICIONES = dOCUMENTO.CONDICIONES;
            doc.TEXTO_POS = dOCUMENTO.TEXTO_POS;
            doc.ASIGNACION_POS = dOCUMENTO.ASIGNACION_POS;
            doc.CLAVE_CTA = dOCUMENTO.CLAVE_CTA;


            List<DOCUMENTOR> retl = new List<DOCUMENTOR>();
            List<DOCUMENTOR_MOD> retlt = new List<DOCUMENTOR_MOD>();

            retl = db.DOCUMENTORs.Where(x => x.NUM_DOC == id).ToList();

            List<listRet> lstret = new List<listRet>();
            var lifnr = dOCUMENTO.PAYER_ID;
            var bukrs = dOCUMENTO.SOCIEDAD_ID;

            Session["SOC"] = bukrs;  //FRT20112018

            var _retl = db.RETENCION_PROV.Where(rtp => rtp.LIFNR == lifnr && rtp.BUKRS == bukrs).ToList();
            var _retl2 = db.RETENCIONs.Where(rtp => rtp.ESTATUS == true).ToList();
            for (int x = 0; x < _retl.Count; x++)
            {
                listRet _l = new listRet();
                var rttt = _retl2.Where(o => o.WITHT == _retl[x].WITHT && o.WT_WITHCD == _retl[x].WT_WITHCD).FirstOrDefault();
                _l.LIFNR = _retl[x].LIFNR;
                _l.BUKRS = _retl[x].BUKRS;
                _l.WITHT = rttt.WITHT;
                _l.DESC = rttt.DESCRIPCION;
                _l.WT_WITHCD = rttt.WT_WITHCD;

                lstret.Add(_l);
            }
            ////ELIMINAR LAS LIGADAS
            //for (int i = 0; i < lstret.Count; i++)
            //{
            //    var ep = db.RETENCIONs.Where(o => o.WITHT == lstret[i].WITHT && o.WT_WITHCD == _retl[i].WT_WITHCD).FirstOrDefault().WITHT_SUB;
            //    if (ep != null || ep != string.Empty)
            //    {
            //        //sacar el indice para borrarlo
            //        var indice = lstret.FindIndex(x => x.WITHT == ep);
            //        lstret.RemoveAt(indice);
            //    }
            //}
            var _relt = lstret;
            if (_relt != null && _relt.Count > 0)
            {
                //Obtener los textos de las retenciones
                retlt = (from r in _relt
                         join rt in db.RETENCIONTs
                         on new { A = r.WITHT, B = r.WT_WITHCD } equals new { A = rt.WITHT, B = rt.WT_WITHCD }
                         into jj
                         from rt in jj.DefaultIfEmpty()
                         where rt.SPRAS_ID.Equals("ES")
                         select new DOCUMENTOR_MOD
                         {
                             LIFNR = r.LIFNR,
                             BUKRS = r.BUKRS,
                             WITHT = r.WITHT,
                             WT_WITHCD = r.WT_WITHCD,
                             DESC = rt.TXT50 == null ? String.Empty : r.DESC,

                         }).ToList();
            }
            for (int i = 0; i < retlt.Count; i++)
            {
                var wtht = retlt[i].WITHT;
                var _res = db.DOCUMENTORPs.Where(nd => nd.NUM_DOC == dOCUMENTO.NUM_DOC && nd.WITHT == wtht).FirstOrDefault();
                if (_res != null)
                {
                    retlt[i].BIMPONIBLE = _res.BIMPONIBLE;
                    retlt[i].IMPORTE_RET = _res.IMPORTE_RET;
                }
            }
            //LEJGG26-11-2018----------------
            for (int i = 0; i < retlt.Count; i++)
            {
                var _a = retlt[i].WITHT;
                var _b = retlt[i].WT_WITHCD;
                var ret = db.RETENCIONs.Where(x => x.WITHT == _a && x.WT_WITHCD == _b).FirstOrDefault().WITHT_SUB;
                if (ret != null)
                {
                    for (int x = 0; x < retlt.Count; x++)
                    {
                        if (retlt[x].WITHT == ret)
                        {
                            retlt.Remove(retlt[x]);
                        }
                    }
                }
            }
            //LEJGG26-11-2018-----------------
            ViewBag.ret = retlt;
            //Obtener datos del proveedor
            PROVEEDOR prov = db.PROVEEDORs.Where(pr => pr.LIFNR == doc.PAYER_ID).FirstOrDefault();
            ViewBag.prov = prov;
            //LEJ 04 10 2018-----------------------------
            //LEJ 05 10 2018-----------------------------
            var _Se = db.DOCUMENTORPs.Where(x => x.NUM_DOC == id).ToList();
            var _wcd = "";
            if (_Se.Count > 0)
            {
                _wcd = _Se[0].WT_WITHCD;
            }
            var rets = _Se.Select(w => w.WITHT).Distinct().ToList();
            var rets2 = rets;
            for (int i = 0; i < rets.Count; i++)
            {
                var _rt = rets2[i];
                var ret = db.RETENCIONs.Where(x => x.WITHT == _rt && x.WT_WITHCD == _wcd).FirstOrDefault().WITHT_SUB;
                for (int j = 0; j < rets.Count; j++)
                {
                    if (rets2[j] == ret)
                    {
                        rets2.RemoveAt(j);
                    }
                }
            }

            ViewBag.Retenciones = rets2;
            //LEJ 05 10 2018-----------------------------
            //lej 30.08.2018------------------
            var xsc = db.SOCIEDADs.ToList();
            var p1 = "";
            if (xsc.Count > 0)
            {
                //saco el primer registro, que sera el que ponga el combobox por default
                p1 = xsc[0].BUKRS;
            }

            var detprov = db.DET_PROVEEDOR.Where(pp => pp.ID_BUKRS == p1).Select(pp1 => pp1.ID_LIFNR).ToList();//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad

            //ViewBag.PROVE = new SelectList(sc.PROVEEDORs, "LIFNR", "LIFNR");//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad
            ViewBag.PROVE = new SelectList(detprov, "LIFNR", "LIFNR");//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad

            //MGC 04092018 Conceptos
            //Obtener los valores de los impuestos
            var impl = (from i in db.IMPUESTOes
                        join ii in db.IIMPUESTOes
                        on i.MWSKZ equals ii.MWSKZ
                        where i.ACTIVO == true && ii.ACTIVO == true
                        select new
                        {
                            MWSKZ = ii.MWSKZ,
                            //KSCHL = ii.KSCHL,
                            KBETR = ii.KBETR,
                            ACTIVO = ii.ACTIVO
                        }).ToList();

            var impuestosv = JsonConvert.SerializeObject(impl, Newtonsoft.Json.Formatting.Indented);
            ViewBag.impuestosval = impuestosv;
            //Workflow
            //ViewBag.worflw = "'aaa','bbb','ccc','ddd','eee'";//lej 10.09.2018
            //Insertar las fechas predefinidas de hoy
            string dates = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime theTime = DateTime.ParseExact(dates, //"06/04/2018 12:00:00 a.m."
                                        "dd/MM/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None);

            //MGC 14-11-2018 Cadena de autorización----------------------------------------------------------------------------->
            //Obtener las cadenas vigentes, considerando la versión
            //MGC 22-11-2018.2 Cadena de autorización----------------------------------------------------------------------->
            //var detcv = (from detc in db.DET_AGENTECC
            //             where detc.USUARIOC_ID == user_id
            //             group detc by new { detc.USUARIOC_ID, detc.ID_RUTA_AGENTE, detc.USUARIOA_ID } into grp
            //             select grp.OrderByDescending(x => x.VERSION).FirstOrDefault());

            IQueryable<DET_AGENTECC> detcc = Enumerable.Empty<DET_AGENTECC>().AsQueryable(); ;
            //Obtener la descripción de la cadena
            FLUJO vbFlujo = new FLUJO();
            vbFlujo = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).FirstOrDefault();

            if (pacc == "R" | pacc == "P")
            {
                string id_cadena = "";
                int? rutav = 0;
                string usuarioc = "";
                if (vbFlujo != null)
                {
                    id_cadena = vbFlujo.ID_RUTA_A;
                    rutav = vbFlujo.RUTA_VERSION;
                    usuarioc = dOCUMENTO.USUARIOC_ID;
                }

                //Es una edición de un rechazo, por lo que solo se debe de mostrar la cadena correspondiente
                detcc = (from detc in db.DET_AGENTECC
                         where detc.USUARIOC_ID == usuarioc && detc.VERSION == rutav && detc.ID_RUTA_AGENTE == id_cadena
                         select detc);
                //group detc by new { detc.USUARIOC_ID, detc.ID_RUTA_AGENTE, detc.USUARIOA_ID } into grp
                //select grp.OrderByDescending(x => x.VERSION).FirstOrDefault());

            }
            else if (pacc == "B")
            {
                //Es un borrador, obtener todas las cadenas para mostrar
                detcc = (from detc in db.DET_AGENTECC
                         where detc.USUARIOC_ID == user_id
                         group detc by new { detc.USUARIOC_ID, detc.ID_RUTA_AGENTE, detc.USUARIOA_ID } into grp
                         select grp.OrderByDescending(x => x.VERSION).FirstOrDefault());
            }

            //MGC 22-11-2018.2 Cadena de autorización-----------------------------------------------------------------------<

            //Consulta tomada de sql
            //select DISTINCT MAX([VERSION]) AS VERSION
            //      ,[USUARIOC_ID]
            //      ,[ID_RUTA_AGENTE]
            //      ,[USUARIOA_ID]
            //FROM[PAGOS].[dbo].[DET_AGENTECC]
            //where USUARIOC_ID = 'admin'
            //group by[USUARIOC_ID], [ID_RUTA_AGENTE], [USUARIOA_ID]
            //order by[ID_RUTA_AGENTE]

            //Obtener las cadenas
            List<DET_AGENTECC> detcl = new List<DET_AGENTECC>();

            detcl = (from dv in detcc.ToList()
                     join dccl in db.DET_AGENTECC.ToList()
                     on new { dv.VERSION, dv.USUARIOC_ID, dv.ID_RUTA_AGENTE, dv.USUARIOA_ID } equals new { dccl.VERSION, dccl.USUARIOC_ID, dccl.ID_RUTA_AGENTE, dccl.USUARIOA_ID }
                     select new DET_AGENTECC
                     {
                         VERSION = dccl.VERSION,
                         USUARIOC_ID = dccl.USUARIOC_ID,
                         ID_RUTA_AGENTE = dccl.ID_RUTA_AGENTE,
                         DESCRIPCION = dccl.DESCRIPCION,
                         USUARIOA_ID = dccl.USUARIOA_ID,
                         FECHAC = dccl.FECHAC
                     }).ToList();


            //MGC 14-11-2018 Cadena de autorización-----------------------------------------------------------------------------<

            //MGC 02-10-2018 Cadena de autorización
            //List<DET_AGENTECC> dta = new List<DET_AGENTECC>();
            //Falta vigencia
            //MGC 14-11-2018 Cadena de autorización----------------------------------------------------------------------------->
            //var dta = db.DET_AGENTECC.Where(dt => dt.USUARIOC_ID == user_id).
            var dta = detcl.
                //MGC 14-11-2018 Cadena de autorización-----------------------------------------------------------------------------<
                Join(
                db.USUARIOs,
                da => da.USUARIOA_ID,
                us => us.ID,
                (da, us) => new
                {
                    //ID = new List<string>() { da.VERSION, da.USUARIOC_ID, da.ID_RUTA_AGENTE, da.USUARIOA_ID},                    
                    ID = new { VERSION = da.VERSION.ToString().Replace(" ", ""), USUARIOC_ID = da.USUARIOC_ID.ToString().Replace(" ", ""), ID_RUTA_AGENTE = da.ID_RUTA_AGENTE.ToString().Replace(" ", ""), USUARIOA_ID = da.USUARIOA_ID.ToString().Replace(" ", "") },
                    TEXT = us.NOMBRE.ToString() + " " + us.APELLIDO_P.ToString()
                }).ToList();

            //MGC 11-12-2018 Agregar Contabilizador 0------>
            int vc1 = 0;
            int vc2 = 0;

            try
            {
                vc1 = vbFlujo.VERSIONC1;
                vc2 = vbFlujo.VERSIONC2;
            }
            catch (Exception)
            {
                vc1 = 0;
                vc2 = 0;
            }
            //MGC 11-12-2018 Agregar Contabilizador 0------<


            var _v = "";
            var _usc = "";
            var _id_ruta = "";
            var _usa = "";
            var _mt = "";
            var _sc = "";
            var lstDta = new List<object>();
            //LEJGG03-12-2018-----------------------------I
            for (int i = 0; i < dta.Count; i++)
            {
                _v = dta[i].ID.VERSION;
                _usc = dta[i].ID.USUARIOC_ID;
                _id_ruta = dta[i].ID.ID_RUTA_AGENTE;
                _usa = dta[i].ID.USUARIOA_ID;
                if (dOCUMENTO.MONTO_DOC_MD == null)
                {
                    _mt = 0 + "";
                }
                else
                { _mt = dOCUMENTO.MONTO_DOC_MD.ToString(); }
                _sc = dOCUMENTO.SOCIEDAD_ID;
                //var _lst = getCad2(int.Parse(_v), _usc, _id_ruta, _usa, decimal.Parse(_mt), _sc, out vc1, out vc2);//MGC 11-12-2018 Agregar Contabilizador 0
                CadenaAutorizadores _lst = getCad2(int.Parse(_v), _usc, _id_ruta, _usa, decimal.Parse(_mt), _sc, vc1, vc2); //MGC 11-12-2018 Agregar Contabilizador 0
                List<CadenaAutorizadoresItem> cadi = new List<CadenaAutorizadoresItem>();//MGC 11-12-2018 Agregar Contabilizador 0
                cadi = _lst.cadenal;//MGC 11-12-2018 Agregar Contabilizador 0
                string cad = "";
                //for (int j = 0; j < _lst.Count; j++)//MGC 11-12-2018 Agregar Contabilizador 0
                for (int j = 0; j < cadi.Count; j++)//MGC 11-12-2018 Agregar Contabilizador 0
                {
                    //var aut = lst[j].autorizador.Split('-');//MGC 11-12-2018 Agregar Contabilizador 0
                    var aut = cadi[j].autorizador.Split('-');//MGC 11-12-2018 Agregar Contabilizador 0
                    //if (j == lst.Count - 1)//MGC 11-12-2018 Agregar Contabilizador 0
                    if (j == cadi.Count - 1)//MGC 11-12-2018 Agregar Contabilizador 0
                    {
                        cad += aut[0].Trim();
                    }
                    else
                    {
                        cad += aut[0].Trim() + " - ";
                    }
                }
                var nuevo = new
                {
                    ID = dta[i].ID,
                    //TEXT = dta[i].TEXT + " (" + cad + ")"
                    TEXT = cad
                };
                lstDta.Add(nuevo);
            }
            //ViewBag.DETAA = new SelectList(dta, "ID", "TEXT");
            ViewBag.DETAA = new SelectList(lstDta, "ID", "TEXT");//LEJGG 03-12-2018
            //LEJGG03-12-2018-----------------------------F

            ViewBag.DETAA2 = JsonConvert.SerializeObject(db.DET_AGENTECC.Where(dt => dt.USUARIOC_ID == user_id).ToList(), Newtonsoft.Json.Formatting.Indented);

            //lejgg 05.10.2018------>
            DocumentoFlujo DF = new DocumentoFlujo();
            DF.D = doc;
            DF.F = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderByDescending(a => a.POS).FirstOrDefault();
            //lejgg 05.10.2018<------
            ViewBag.docFl = DF;

            //MGC 05 - 10 - 2018 flujo -- >
            //Obtener datos del flujo
            var vbFl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            ViewBag.workflow = vbFl;

            //MGC 04 - 10 - 2018 flujo <-- 

            // frt obtener el flujo de SAP

            var vbSap = db.DOCUMENTOLOGs.Where(s => s.NUM_DOC.Equals(id)).OrderBy(s => s.FECHA).ToList();
            ViewBag.LogSap = vbSap;

            return View(doc);
        }

        // POST: Solicitudes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NUM_DOC,NUM_PRE,TSOL_ID,TALL_ID,SOCIEDAD_ID,PAIS_ID,ESTADO,CIUDAD,PERIODO,EJERCICIO,TIPO_TECNICO,TIPO_RECURRENTE,CANTIDAD_EV," +
            "USUARIOC_ID,USUARIOD_ID,FECHAD,FECHAC,HORAC,FECHAC_PLAN,FECHAC_USER,HORAC_USER,ESTATUS,ESTATUS_C,ESTATUS_SAP,ESTATUS_WF,DOCUMENTO_REF,CONCEPTO,NOTAS,MONTO_DOC_MD," +
            "MONTO_FIJO_MD,MONTO_BASE_GS_PCT_MD,MONTO_BASE_NS_PCT_MD,MONTO_DOC_ML,MONTO_FIJO_ML,MONTO_BASE_GS_PCT_ML,MONTO_BASE_NS_PCT_ML,MONTO_DOC_ML2,MONTO_FIJO_ML2," +
            "MONTO_BASE_GS_PCT_ML2,MONTO_BASE_NS_PCT_ML2,PORC_ADICIONAL,IMPUESTO,FECHAI_VIG,FECHAF_VIG,ESTATUS_EXT,SOLD_TO_ID,PAYER_ID,PAYER_NOMBRE,PAYER_EMAIL,GRUPO_CTE_ID," +
            "CANAL_ID,MONEDA_ID,MONEDAL_ID,MONEDAL2_ID,TIPO_CAMBIO,TIPO_CAMBIOL,TIPO_CAMBIOL2,NO_FACTURA,FECHAD_SOPORTE,METODO_PAGO,NO_PROVEEDOR,PASO_ACTUAL,AGENTE_ACTUAL," +
            "FECHA_PASO_ACTUAL,VKORG,VTWEG,SPART,PUESTO_ID,GALL_ID,CONCEPTO_ID,DOCUMENTO_SAP,PORC_APOYO,LIGADA,OBJETIVOQ,FRECUENCIA_LIQ,OBJQ_PORC,FECHACON,FECHA_BASE,REFERENCIA," +
            "TEXTO_POS,ASIGNACION_POS,CLAVE_CTA,MONTO_DOC_IMP, DOCUMENTOP,DOCUMENTOR,DOCUMENTORP,Anexo,DOCUMENTOA_TAB")] Models.DOCUMENTO_MOD dOCUMENTO, IEnumerable<HttpPostedFileBase> file_sopAnexar, string[] labels_desc, string FECHADO, string mtTot, string Uuid,
            //MGC 02-10-2018 Cadenas de autorización
            string DETTA_VERSION, string DETTA_USUARIOC_ID, string DETTA_ID_RUTA_AGENTE, string DETTA_USUARIOA_ID, string borr

            //MGC 11-12-2018 Agregar Contabilizador 0
            , string VERSIONC1, string VERSIONC2
            )
        {


            string errorString = "";
            var est = "";
            var filenull = false;  //FRT04122018
            if (ModelState.IsValid)
            {
                try
                {



                    //Traigo los datos previamente registrados
                    var _doc = db.DOCUMENTOes.Where(n => n.NUM_DOC == dOCUMENTO.NUM_DOC).FirstOrDefault();
                    var _payid = _doc.PAYER_ID;
                    var _ndoc = _doc.NUM_DOC;
                    //Copiar valores del post al nuevo objeto
                    _doc.TSOL_ID = dOCUMENTO.TSOL_ID;
                    if (dOCUMENTO.SOCIEDAD_ID != null)
                    {
                        _doc.SOCIEDAD_ID = dOCUMENTO.SOCIEDAD_ID;
                    }
                    else
                    {//se queda el de la bd
                    }
                    if (dOCUMENTO.FECHAD != null)
                    {
                        _doc.FECHAD = dOCUMENTO.FECHAD;
                        _doc.FECHACON = dOCUMENTO.FECHAD;
                        _doc.FECHA_BASE = dOCUMENTO.FECHAD;
                    }
                    else
                    {//se queda el de la bd
                    }
                    if (_doc.ESTATUS == "B")
                    {
                        _doc.MONEDA_ID = dOCUMENTO.MONEDA_ID;
                    }
                    if (_doc.ESTATUS == "B")
                    {
                        _doc.TIPO_CAMBIO = dOCUMENTO.TIPO_CAMBIO;
                    }
                    _doc.IMPUESTO = dOCUMENTO.IMPUESTO;
                    //_doc.USUARIOD_ID = dOCUMENTO.USUARIOD_ID;//MGC 03-11-2018
                    _doc.USUARIOD_ID = DETTA_USUARIOA_ID;//MGC 03-11-2018
                    var _t = mtTot.Replace("$", "");
                    _t = _t.Replace(",", "");
                    _doc.MONTO_DOC_MD = decimal.Parse(_t);
                    _doc.CONCEPTO = dOCUMENTO.CONCEPTO;
                    var _payerid_ = "";
                    if (_doc.ESTATUS == "B")
                    {
                        //_doc.PAYER_ID = dOCUMENTO.PAYER_ID;
                        _payerid_ = _doc.PAYER_ID;
                    }
                    else
                    { //se queda el que tiene
                        _payerid_ = _doc.PAYER_ID;
                    }
                    if (dOCUMENTO.CONDICIONES != null)
                    {
                        _doc.CONDICIONES = dOCUMENTO.CONDICIONES;
                    }
                    else
                    { //se queda el que tiene
                    }
                    _doc.TEXTO_POS = dOCUMENTO.TEXTO_POS;
                    _doc.ASIGNACION_POS = dOCUMENTO.ASIGNACION_POS;
                    _doc.CLAVE_CTA = dOCUMENTO.CLAVE_CTA;

                    //Obtener usuarioc
                    USUARIO us = db.USUARIOs.Find(User.Identity.Name);
                    _doc.PUESTO_ID = us.PUESTO_ID;
                    _doc.USUARIOC_ID = User.Identity.Name;
                    var _usC = User.Identity.Name;
                    //Obtener el tipo de documento
                    var doct = db.DET_TIPODOC.Where(dt => dt.TIPO_SOL == dOCUMENTO.TSOL_ID).FirstOrDefault();
                    _doc.DOCUMENTO_SAP = doct.BLART.ToString();




                    //FRT03122018 para poder realizar el guardado de borrador sin afectar estatus

                    if (borr != "B")
                    {
                        //Si es B signfica que ya pasa a ser N
                        est = _doc.ESTATUS;
                        if (_doc.ESTATUS == "B")
                        {
                            //Estatus
                            _doc.ESTATUS = "N";
                        }
                        else
                        {
                            //Estatus
                        }
                        //Estatus wf
                        dOCUMENTO.ESTATUS_WF = "P";// Si el wf es p es que no se ha creado, si es A, es que se creo el archivo, cambia al generar el preliminar
                    }

                    //ENDFRT03122018 para poder realizar el guardado de borrador sin afectar estatus

                    //db.DOCUMENTOes.Add(_doc);
                    db.Entry(_doc).State = EntityState.Modified;
                    db.SaveChanges();//LEJGG 29-10-2018






                    //Guardar número de documento creado
                    Session["NUM_DOC"] = _doc.NUM_DOC;


                    //FRT25112018 PARA QUE LO ANEXOS NO TRUENEN
                    //Lejgg 09-11-2018-------------
                    List<string> listaDirectorios = new List<string>();
                    List<string> listaNombreArchivos = new List<string>();
                    List<string> listaDescArchivos = new List<string>();

                    //FRT20112018 PARA ARREGAR EL ANEXO DE VARIAS LINEAS
                    List<string> listaDirectorios3 = new List<string>();
                    List<string> listaNombreArchivos3 = new List<string>();
                    List<string> listaDescArchivos3 = new List<string>();
                    //ENDFRT20112018
                    try
                    {
                        //Guardar los documentos cargados en la sección de soporte
                        var res = "";
                        string errorMessage = "";
                        int numFiles = 1;
                        //Checar si hay archivos para subir
                        foreach (HttpPostedFileBase file in file_sopAnexar)
                        {
                            if (file != null)
                            {
                                if (file.ContentLength > 0)
                                {
                                    numFiles++;
                                }
                            }
                        }
                        if (numFiles > 0)
                        {
                            //Evaluar que se creo el directorio
                            try
                            {
                                int indexlabel = 0;

                                //FRT13112018 Para poder subir muchos archivos
                                var car_ser = "att";
                                var server = "";

                                var num_doc = Convert.ToDecimal(Session["NUM_DOC"]);
                                server = getDirPrel(car_ser, num_doc);
                                System.IO.DirectoryInfo directorio = new System.IO.DirectoryInfo(server + "\\");


                                if (Directory.Exists(server))  //FRT04122018 Para validar la carpeta de anexos que exista

                                {
                                    filenull = true;
                                }

                                if (filenull)
                                { //FRT04122018 Para validar la carpeta de anexos que exista
                                    FileInfo[] archivos = directorio.GetFiles();

                                    //FRT16112018
                                    var delDA = db.DOCUMENTOAs.Where(x => x.NUM_DOC == num_doc).ToList();
                                    for (int i = 0; i < delDA.Count; i++)
                                    {
                                        DOCUMENTOA dOCUMENTOAd = db.DOCUMENTOAs.Find(delDA[i].NUM_DOC, delDA[i].POSD, delDA[i].POS);
                                        db.DOCUMENTOAs.Remove(dOCUMENTOAd);
                                        db.SaveChanges();
                                    }

                                    var delDAS = db.DOCUMENTOAS1.Where(x => x.NUM_DOC == num_doc).ToList();

                                    for (int i = 0; i < delDAS.Count; i++)
                                    {

                                        DOCUMENTOA1 dOCUMENTOASd = db.DOCUMENTOAS1.Find(delDAS[i].NUM_DOC, delDAS[i].POS);
                                        db.DOCUMENTOAS1.Remove(dOCUMENTOASd);
                                        db.SaveChanges();
                                    }


                                    for (int i = 0; i < dOCUMENTO.DOCUMENTOA_TAB.Count; i++)
                                    {
                                        var descripcion = "";
                                        foreach (var a in archivos)
                                        {
                                            if (a.Name.Trim() == dOCUMENTO.DOCUMENTOA_TAB[i].NAME.Trim())
                                            {
                                                try
                                                {
                                                    listaDescArchivos.Add(labels_desc[indexlabel]);
                                                    listaDescArchivos3.Add(labels_desc[indexlabel]);
                                                }
                                                catch (Exception ex)
                                                {
                                                    descripcion = "";
                                                    listaDescArchivos.Add(descripcion);
                                                    listaDescArchivos3.Add(descripcion);
                                                }

                                                try
                                                {
                                                    listaNombreArchivos.Add(a.Name);
                                                    listaNombreArchivos3.Add(a.Name);
                                                }
                                                catch (Exception ex)
                                                {
                                                    listaNombreArchivos.Add("");
                                                    listaNombreArchivos3.Add("");
                                                }

                                                string errorfiles = "";

                                                var url_prel = "";
                                                var dirFile = "";
                                                string carpeta = "att";
                                                try
                                                {
                                                    url_prel = getDirPrel(carpeta, dOCUMENTO.NUM_DOC);
                                                    dirFile = url_prel;
                                                }
                                                catch (Exception e)
                                                {
                                                    dirFile = ConfigurationManager.AppSettings["URL_ATT"].ToString() + @"att";
                                                }

                                                listaDirectorios.Add(dirFile + "\\" + a.Name);
                                                listaDirectorios3.Add(dirFile + "\\" + a.Name);


                                                indexlabel++;
                                                if (errorfiles != "")
                                                {
                                                    errorMessage += "Error con el archivo " + errorfiles;
                                                }

                                            }
                                        }
                                    }




                                    //foreach (var a in archivos)
                                    //{
                                    //    var descripcion = "";

                                    //    for (int i = 0; i < dOCUMENTO.DOCUMENTOA_TAB.Count; i++)
                                    //    {
                                    //        if (a.Name.Trim() == dOCUMENTO.DOCUMENTOA_TAB[i].NAME.Trim())
                                    //        {
                                    //            try
                                    //            {
                                    //                listaDescArchivos.Add(labels_desc[indexlabel]);
                                    //                listaDescArchivos3.Add(labels_desc[indexlabel]);
                                    //            }
                                    //            catch (Exception ex)
                                    //            {
                                    //                descripcion = "";
                                    //                listaDescArchivos.Add(descripcion);
                                    //                listaDescArchivos3.Add(descripcion);
                                    //            }

                                    //            try
                                    //            {
                                    //                listaNombreArchivos.Add(a.Name);
                                    //                listaNombreArchivos3.Add(a.Name);
                                    //            }
                                    //            catch (Exception ex)
                                    //            {
                                    //                listaNombreArchivos.Add("");
                                    //                listaNombreArchivos3.Add("");
                                    //            }

                                    //            string errorfiles = "";

                                    //            var url_prel = "";
                                    //            var dirFile = "";
                                    //            string carpeta = "att";
                                    //            try
                                    //            {
                                    //                url_prel = getDirPrel(carpeta, dOCUMENTO.NUM_DOC);
                                    //                dirFile = url_prel;
                                    //            }
                                    //            catch (Exception e)
                                    //            {
                                    //                dirFile = ConfigurationManager.AppSettings["URL_ATT"].ToString() + @"att";
                                    //            }

                                    //            listaDirectorios.Add(dirFile + "\\" + a.Name);
                                    //            listaDirectorios3.Add(dirFile + "\\" + a.Name);


                                    //            indexlabel++;
                                    //            if (errorfiles != "")
                                    //            {
                                    //                errorMessage += "Error con el archivo " + errorfiles;
                                    //            }

                                    //        }

                                    //    }
                                    //}
                                }



                            }
                            catch (Exception e)
                            {
                                // errorMessage = dir;
                            }

                            errorString = errorMessage;
                            //Guardar número de documento creado
                            Session["ERROR_FILES"] = errorMessage;
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    //FRT16112018 SE PONE LO DE CREATE

                    if (filenull)
                    {
                        List<string> listaDirectorios2 = listaDirectorios;
                        List<string> listaNombreArchivos2 = listaNombreArchivos;
                        List<string> listaDescArchivos2 = listaDescArchivos;



                        var listafiles = listaNombreArchivos.Count;//FRT16112018


                        var _name = "";//FRT20112018 NOMBRE PAERA QUIATR DE LA LISTA
                                       //DOCUMENTOA
                                       //Misma cantidad de archivos y nombres, osea todo bien
                        if (listaDirectorios.Count == listaDescArchivos.Count && listaDirectorios.Count == listaNombreArchivos.Count)
                        {
                            //un contador para los archivos que se borran de las listas
                            int arBorr = 0;
                            for (int i = 0; i < dOCUMENTO.Anexo.Count; i++)
                            {
                                var pos = 1;
                                DOCUMENTOA _dA = new DOCUMENTOA();
                                if (dOCUMENTO.Anexo[i].a1 != 0)
                                {
                                    _dA.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    var de = "";
                                    int a1 = 0;
                                    int a11 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                    if (dOCUMENTO.Anexo[i].a1 > 0 && dOCUMENTO.Anexo[i].a1 <= listafiles)  //FRT16112018
                                                                                                           //if (doc.Anexo[i].a1 > 0 && doc.Anexo[i].a1 <= listaNombreArchivos.Count)
                                    {
                                        a1 = dOCUMENTO.Anexo[i].a1;
                                        a1 = a1 - 1;

                                        a11 = dOCUMENTO.Anexo[i].a1;
                                        a11 = a11 - arBorr;
                                        a11 = a11 - 1;

                                        _dA.POS = dOCUMENTO.Anexo[i].a1;

                                        try
                                        {
                                            _name = "";
                                            _name = listaNombreArchivos3[a1];
                                            de = Path.GetExtension(listaNombreArchivos3[a1]);
                                            //de = Path.GetExtension(listaNombreArchivos[a1]);
                                            _dA.TIPO = de.Replace(".", "");
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.TIPO = "";
                                        }
                                        try
                                        {
                                            _dA.DESC = listaDescArchivos3[a1];
                                            //_dA.DESC = listaDescArchivos[a1];
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.DESC = "";
                                        }
                                        try
                                        {
                                            _dA.PATH = listaDirectorios3[a1];
                                            //_dA.PATH = listaDirectorios[a1];
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.PATH = "";
                                        }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        ////listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a11);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        //listaNombreArchivos2.RemoveAt(a11);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                                _dA = new DOCUMENTOA();
                                if (dOCUMENTO.Anexo[i].a2 != 0)
                                {
                                    _dA.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    var de = "";
                                    int a2 = 0;
                                    int a12 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5

                                    if (dOCUMENTO.Anexo[i].a2 > 0 && dOCUMENTO.Anexo[i].a2 <= listafiles)  //FRT16112018
                                                                                                           //if (doc.Anexo[i].a2 > 0 && doc.Anexo[i].a2 <= listaNombreArchivos.Count)
                                    {
                                        a2 = dOCUMENTO.Anexo[i].a2;
                                        a2 = a2 - 1;


                                        a12 = dOCUMENTO.Anexo[i].a2;
                                        a12 = a12 - arBorr;
                                        a12 = a12 - 1;

                                        _dA.POS = dOCUMENTO.Anexo[i].a2;
                                        try
                                        {


                                            _name = "";
                                            _name = listaNombreArchivos3[a2];
                                            //de = Path.GetExtension(listaNombreArchivos[a2]);
                                            de = Path.GetExtension(listaNombreArchivos3[a2]);
                                            _dA.TIPO = de.Replace(".", "");
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.TIPO = "";
                                        }
                                        try
                                        {
                                            //_dA.DESC = listaDescArchivos[a2];
                                            _dA.DESC = listaDescArchivos3[a2];
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.DESC = "";
                                        }
                                        try
                                        {
                                            //_dA.PATH = listaDirectorios[a2];
                                            _dA.PATH = listaDirectorios3[a2];
                                        }
                                        catch (Exception c)
                                        {
                                            _dA.PATH = "";
                                        }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        ////listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a12);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        //listaNombreArchivos2.RemoveAt(a12);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                                _dA = new DOCUMENTOA();
                                if (dOCUMENTO.Anexo[i].a3 != 0)
                                {
                                    _dA.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    var de = "";
                                    int a3 = 0;
                                    int a13 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                    if (dOCUMENTO.Anexo[i].a3 > 0 && dOCUMENTO.Anexo[i].a3 <= listafiles)  //FRT16112018
                                                                                                           //if (doc.Anexo[i].a3 > 0 && doc.Anexo[i].a3 <= listaNombreArchivos.Count)
                                    {
                                        a3 = dOCUMENTO.Anexo[i].a3;
                                        a3 = a3 - 1;


                                        a13 = dOCUMENTO.Anexo[i].a3;
                                        a13 = a13 - arBorr;
                                        a13 = a13 - 1;



                                        _dA.POS = dOCUMENTO.Anexo[i].a3;
                                        try
                                        {
                                            try
                                            {
                                                _name = "";
                                                _name = listaNombreArchivos3[a3];
                                                //de = Path.GetExtension(listaNombreArchivos[a3]);
                                                de = Path.GetExtension(listaNombreArchivos3[a3]);
                                                _dA.TIPO = de.Replace(".", "");
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.TIPO = "";
                                            }
                                            try
                                            {
                                                //_dA.DESC = listaDescArchivos[a3];
                                                _dA.DESC = listaDescArchivos3[a3];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.DESC = "";
                                            }
                                            try
                                            {
                                                //_dA.PATH = listaDirectorios[a3];
                                                _dA.PATH = listaDirectorios3[a3];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.PATH = "";
                                            }
                                        }
                                        catch (Exception e)
                                        { }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        ////listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a13);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        //listaNombreArchivos2.RemoveAt(a13);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                                _dA = new DOCUMENTOA();
                                if (dOCUMENTO.Anexo[i].a4 != 0)
                                {
                                    _dA.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    var de = "";
                                    int a4 = 0;
                                    int a14 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                    if (dOCUMENTO.Anexo[i].a4 > 0 && dOCUMENTO.Anexo[i].a4 <= listafiles)  //FRT16112018
                                                                                                           //if (doc.Anexo[i].a4 > 0 && doc.Anexo[i].a4 <= listaNombreArchivos.Count)
                                    {
                                        a4 = dOCUMENTO.Anexo[i].a4;
                                        a4 = a4 - 1;


                                        a14 = dOCUMENTO.Anexo[i].a4;
                                        a14 = a14 - arBorr;
                                        a14 = a14 - 1;


                                        _dA.POS = dOCUMENTO.Anexo[i].a4;
                                        try
                                        {
                                            try
                                            {
                                                _name = "";
                                                _name = listaNombreArchivos3[a4];
                                                de = Path.GetExtension(listaNombreArchivos3[a4]);
                                                //de = Path.GetExtension(listaNombreArchivos[a4]);
                                                _dA.TIPO = de.Replace(".", "");
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.TIPO = "";
                                            }
                                            try
                                            {
                                                _dA.DESC = listaDescArchivos3[a4];
                                                //_dA.DESC = listaDescArchivos[a4];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.DESC = "";
                                            }
                                            try
                                            {
                                                _dA.PATH = listaDirectorios3[a4];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.PATH = "";
                                            }
                                        }
                                        catch (Exception e) { }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        ////listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a14);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        //listaNombreArchivos2.RemoveAt(a14);
                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                                _dA = new DOCUMENTOA();
                                if (dOCUMENTO.Anexo[i].a5 != 0)
                                {
                                    _dA.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    _dA.POSD = i + 1;
                                    _dA.POS = pos;
                                    var de = "";
                                    int a5 = 0;
                                    int a15 = 0;
                                    //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                    if (dOCUMENTO.Anexo[i].a5 > 0 && dOCUMENTO.Anexo[i].a5 <= listafiles)  //FRT16112018
                                                                                                           //if (doc.Anexo[i].a5 > 0 && doc.Anexo[i].a5 <= listaNombreArchivos.Count)
                                    {
                                        a5 = dOCUMENTO.Anexo[i].a5;
                                        a5 = a5 - 1;



                                        a15 = dOCUMENTO.Anexo[i].a5;
                                        a15 = a15 - arBorr;
                                        a15 = a15 - 1;

                                        _dA.POS = dOCUMENTO.Anexo[i].a5;
                                        try
                                        {
                                            try
                                            {
                                                _name = "";
                                                _name = listaNombreArchivos3[a5];
                                                //de = Path.GetExtension(listaNombreArchivos[a5]);
                                                de = Path.GetExtension(listaNombreArchivos3[a5]);
                                                _dA.TIPO = de.Replace(".", "");
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.TIPO = "";
                                            }
                                            try
                                            {
                                                //_dA.DESC = listaDescArchivos[a5];
                                                _dA.DESC = listaDescArchivos3[a5];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.DESC = "";
                                            }
                                            try
                                            {
                                                _dA.PATH = listaDirectorios3[a5];
                                                //_dA.PATH = listaDirectorios[a5];
                                            }
                                            catch (Exception c)
                                            {
                                                _dA.PATH = "";
                                            }
                                        }
                                        catch (Exception e) { }
                                    }
                                    else
                                    {
                                        _dA.TIPO = "";
                                        _dA.DESC = "";
                                        _dA.PATH = "";
                                    }
                                    _dA.CLASE = "OTR";
                                    _dA.STEP_WF = 1;
                                    _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                    _dA.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(_dA);
                                        db.SaveChanges();
                                        pos++;
                                        //listaDirectorios2.Remove(_dA.PATH);
                                        ////listaDescArchivos2.Remove(_dA.DESC);
                                        //listaDescArchivos2.RemoveAt(a15);
                                        if (listaDirectorios2.Remove(_dA.PATH))
                                        {
                                            listaDescArchivos2.Remove(_dA.DESC);
                                        }
                                        listaNombreArchivos2.Remove(_name);
                                        //listaNombreArchivos2.RemoveAt(a15);

                                        arBorr++;
                                    }
                                    catch (Exception e)
                                    {
                                        //
                                    }
                                }
                            }
                        }

                        //Lej-02.10.2018------
                        //Lejgg 26.10.2018---------------------------------------->
                        //Los anexos que no se agreguen a documentoa se agregaran a documentoas(documentoa1), significa que estan en lista porque no se ligaron a ningun detalle
                        if (listaDirectorios2.Count == listaDescArchivos2.Count && listaDirectorios2.Count == listaNombreArchivos2.Count)
                        {
                            var pos = 1;
                            for (int i = 0; i < listaDescArchivos2.Count; i++)
                            {
                                DOCUMENTOA1 _dA = new DOCUMENTOA1();
                                _dA.NUM_DOC = dOCUMENTO.NUM_DOC;
                                _dA.POS = pos;
                                var de = "";
                                try
                                {
                                    de = Path.GetExtension(listaNombreArchivos2[i]);
                                    _dA.TIPO = de.Replace(".", "");
                                }
                                catch (Exception c)
                                {
                                    _dA.TIPO = "";
                                }
                                try
                                {
                                    _dA.DESC = listaDescArchivos2[i];
                                }
                                catch (Exception c)
                                {
                                    _dA.DESC = "";
                                }
                                try
                                {
                                    _dA.PATH = listaDirectorios2[i];
                                }
                                catch (Exception c)
                                {
                                    _dA.PATH = "";
                                }
                                _dA.CLASE = "OTR";
                                _dA.STEP_WF = 1;
                                _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                _dA.ACTIVO = true;
                                try
                                {
                                    db.DOCUMENTOAS1.Add(_dA);
                                    db.SaveChanges();
                                    pos++;
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }
                        }

                    }





                    if (filenull)
                    {
                        var listaDA = db.DOCUMENTOAs.Where(x => x.NUM_DOC == _ndoc && x.ACTIVO == true).ToList();
                        var _listaDAS = db.DOCUMENTOAS1.Where(x => x.NUM_DOC == _ndoc && x.ACTIVO == true).ToList();
                        DOCUMENTOA dOCUMENTOA = new DOCUMENTOA();
                        for (int i = 0; i < _listaDAS.Count; i++)
                        {
                            dOCUMENTOA = new DOCUMENTOA();
                            dOCUMENTOA.PATH = _listaDAS[i].PATH;
                            listaDA.Add(dOCUMENTOA);
                        }

                        var _server = getDirPrel("att", _ndoc);
                        System.IO.DirectoryInfo _directorio = new System.IO.DirectoryInfo(_server + "\\");
                        FileInfo[] _archivos = _directorio.GetFiles();


                        List<string> listaDirectorios4 = new List<string>();
                        List<string> listaNombreArchivos4 = new List<string>();
                        List<string> listaDescArchivos4 = new List<string>();

                        foreach (var a in _archivos)
                        {
                            bool bandera = false;
                            for (int i = 0; i < listaDA.Count; i++)
                            {
                                var _xtrtemp = listaDA[i].PATH.Split('\\');
                                var nombre = _xtrtemp[_xtrtemp.Length - 1];
                                if (a.Name.Trim() == nombre.Trim())
                                {
                                    bandera = true;
                                    break;
                                }
                            }
                            if (!bandera)
                            {
                                try
                                {
                                    listaDirectorios4.Add(_server + "\\" + a.Name);
                                    listaNombreArchivos4.Add(a.Name);
                                    listaDescArchivos4.Add("");
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }
                        }
                        for (int i = 0; i < listaDescArchivos4.Count; i++)
                        {
                            DOCUMENTOA1 _d = new DOCUMENTOA1();
                            _d.NUM_DOC = _ndoc;
                            _d.POS = (db.DOCUMENTOAS1.Where(x => x.NUM_DOC == _ndoc).ToList().Count + 1);
                            var _de = Path.GetExtension(listaNombreArchivos4[i]);
                            _d.TIPO = _de.Replace(".", "");
                            _d.DESC = listaDescArchivos4[i];
                            _d.CLASE = "OTR";
                            _d.STEP_WF = 1;
                            _d.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                            _d.PATH = listaDirectorios4[i];
                            _d.ACTIVO = false;
                            db.DOCUMENTOAS1.Add(_d);
                            db.SaveChanges();
                        }


                        //ENDFRT16112018 SE REPLICA LO DE CREATE
                        //ENDFRT25112018
                    }





                    //FRT22112018 FUNCIONALIDAD PARA DETALLES AGREGAR Y ELIMINAR

                    var addDocumentoA = db.DOCUMENTOAs.Where(x => x.NUM_DOC == _ndoc).ToList(); //FRT04122018 para anexos despues de editar

                    var delDRP = db.DOCUMENTORPs.Where(x => x.NUM_DOC == _ndoc).ToList();
                    for (int i = 0; i < delDRP.Count; i++)
                    {
                        DOCUMENTORP dOCUMENTORP = db.DOCUMENTORPs.Find(delDRP[i].NUM_DOC, delDRP[i].POS, delDRP[i].WITHT, delDRP[i].WT_WITHCD);
                        db.DOCUMENTORPs.Remove(dOCUMENTORP);
                        db.SaveChanges();
                    }


                    var delDP = db.DOCUMENTOPs.Where(x => x.NUM_DOC == _ndoc).ToList();
                    for (int i = 0; i < delDP.Count; i++)
                    {
                        try
                        {
                            DOCUMENTOP dOCUMENTOP = db.DOCUMENTOPs.Find(delDP[i].NUM_DOC, delDP[i].POS);
                            db.DOCUMENTOPs.Remove(dOCUMENTOP);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                        }

                    }



                    decimal _monto = 0;
                    decimal _iva = 0;
                    decimal _total = 0;
                    var _mwskz = "";

                    var _pos_err_imputacion = "";
                    for (int i = 0; i < dOCUMENTO.DOCUMENTOP.Count; i++)
                    {
                        DOCUMENTOP dp = new DOCUMENTOP();

                        dp.NUM_DOC = dOCUMENTO.NUM_DOC;
                        dp.POS = i + 1;
                        dp.ACCION = dOCUMENTO.DOCUMENTOP[i].ACCION;
                        dp.FACTURA = dOCUMENTO.DOCUMENTOP[i].FACTURA;
                        dp.TCONCEPTO = dOCUMENTO.DOCUMENTOP[i].TCONCEPTO;
                        dp.GRUPO = dOCUMENTO.DOCUMENTOP[i].GRUPO;
                        dp.CUENTA = dOCUMENTO.DOCUMENTOP[i].CUENTA;//Modificación Cuenta de PEP
                        dp.TIPOIMP = dOCUMENTO.DOCUMENTOP[i].TIPOIMP;
                        dp.IMPUTACION = dOCUMENTO.DOCUMENTOP[i].IMPUTACION;
                        dp.CCOSTO = dOCUMENTO.DOCUMENTOP[i].CCOSTO;
                        dp.MONTO = dOCUMENTO.DOCUMENTOP[i].MONTO;
                        _monto = _monto + dOCUMENTO.DOCUMENTOP[i].MONTO;//lejgg 10-10-2018
                        var sp = dOCUMENTO.DOCUMENTOP[i].MWSKZ.Split('-');
                        _mwskz = sp[0];//lejgg 10-10-2018
                        dp.MWSKZ = sp[0];
                        dp.IVA = dOCUMENTO.DOCUMENTOP[i].IVA;
                        _iva = _iva + dOCUMENTO.DOCUMENTOP[i].IVA;//lejgg 10-10-2018
                        dp.TOTAL = dOCUMENTO.DOCUMENTOP[i].TOTAL;
                        _total = _total + dOCUMENTO.DOCUMENTOP[i].TOTAL;//lejgg 10-10-2018
                        dp.TEXTO = dOCUMENTO.DOCUMENTOP[i].TEXTO;
                        db.DOCUMENTOPs.Add(dp);
                        db.SaveChanges();

                    }

                    var delDPH = db.DOCUMENTOPs.Where(x => x.NUM_DOC == _ndoc).ToList();

                    DOCUMENTOP _dp = new DOCUMENTOP();
                    _dp.NUM_DOC = dOCUMENTO.NUM_DOC;
                    _dp.POS = delDPH.Count + 1;
                    _dp.ACCION = "H";
                    _dp.CUENTA = _payerid_;
                    _dp.MONTO = _monto + _iva;//Obtener las retenciones relacionadas con las ya mostradas en la tabla
                    _dp.MWSKZ = _mwskz;
                    _dp.IVA = _iva;
                    _dp.TOTAL = _total;
                    db.DOCUMENTOPs.Add(_dp);
                    db.SaveChanges();


                    if (dOCUMENTO.DOCUMENTORP != null)
                    {
                        for (int i = 0; i < dOCUMENTO.DOCUMENTORP.Count; i++)
                        {

                            var _op = ((i + 2) / 2);
                            var _pos = _op.ToString().Split('.');

                            DOCUMENTORP dr = new DOCUMENTORP();
                            dr.NUM_DOC = dOCUMENTO.NUM_DOC;
                            dr.WITHT = dOCUMENTO.DOCUMENTORP[i].WITHT;
                            dr.WT_WITHCD = dOCUMENTO.DOCUMENTORP[i].WT_WITHCD;
                            dr.POS = int.Parse(_pos[0]);
                            dr.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                            dr.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;
                            db.DOCUMENTORPs.Add(dr);
                            db.SaveChanges();

                        }
                    }



                    try
                    {
                        if (dOCUMENTO.DOCUMENTOR != null)
                        {
                            var _docrets = dOCUMENTO.DOCUMENTOR;
                            if (_docrets != null)//significa que hay valores
                            {
                                for (int i = 0; i < _docrets.Count; i++)
                                {
                                    var _with = _docrets[i].WITHT.Trim();
                                    var _wt_withcd = _docrets[i].WT_WITHCD.Trim();
                                    var _witht_sub = db.RETENCIONs.Where(a => a.WITHT == _with && a.WT_WITHCD == _wt_withcd).FirstOrDefault().WITHT_SUB;
                                    var bd_docr = db.DOCUMENTORs.Where(d => d.NUM_DOC == _ndoc && d.WITHT == _with).FirstOrDefault();
                                    if (_witht_sub != null)
                                    {
                                        //Se actualizan las f 
                                        bd_docr.BIMPONIBLE = _docrets[i].BIMPONIBLE;
                                        bd_docr.IMPORTE_RET = _docrets[i].IMPORTE_RET;
                                        db.Entry(bd_docr).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Se actualizan lasligadas
                                        var bd_docrl = db.DOCUMENTORs.Where(d => d.NUM_DOC == _ndoc && d.WITHT == _witht_sub).FirstOrDefault();
                                        bd_docrl.BIMPONIBLE = _docrets[i].BIMPONIBLE;
                                        bd_docrl.IMPORTE_RET = _docrets[i].IMPORTE_RET;
                                        db.Entry(bd_docrl).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        bd_docr.BIMPONIBLE = _docrets[i].BIMPONIBLE;
                                        bd_docr.IMPORTE_RET = _docrets[i].IMPORTE_RET;
                                        db.Entry(bd_docr).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }
                            }
                        }


                    }
                    catch (Exception e) { }



                    for (int i = 0; i < addDocumentoA.Count; i++) ///frt04122018 para anexos despues de aditar
                    {
                        try
                        {
                            DOCUMENTOA _d1 = new DOCUMENTOA();
                            _d1.NUM_DOC = _ndoc;
                            _d1.POSD = addDocumentoA[i].POSD;
                            _d1.POS = addDocumentoA[i].POS;
                            _d1.TIPO = addDocumentoA[i].TIPO;
                            _d1.DESC = addDocumentoA[i].DESC;
                            _d1.CLASE = addDocumentoA[i].CLASE;
                            _d1.STEP_WF = addDocumentoA[i].STEP_WF;
                            _d1.USUARIO_ID = addDocumentoA[i].USUARIO_ID;
                            _d1.PATH = addDocumentoA[i].PATH;
                            _d1.ACTIVO = addDocumentoA[i].ACTIVO;
                            db.DOCUMENTOAs.Add(_d1);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            //
                        }

                    }




                    //END FRT22112018








                    //FRT22112018 Comentado para Agregar y Eliminar Detalles

                    ////Guardar las posiciones de la solicitud
                    //    try
                    //{
                    //    decimal _monto = 0;
                    //    decimal _iva = 0;
                    //    decimal _total = 0;
                    //    var _mwskz = "";
                    //    int j = 1;
                    //    var _pos_err_imputacion = "";
                    //    for (int i = 0; i < dOCUMENTO.DOCUMENTOP.Count; i++)
                    //    {
                    //        try
                    //        {
                    //            decimal _error_imputacion = 0;
                    //            //Busco Informacion Previa del documento si es que existe.
                    //            var docpPrevio = db.DOCUMENTOPs.Where(x => x.NUM_DOC == _ndoc && x.POS == j).FirstOrDefault();
                    //            //Si es null signfica que no hay nada, osea se crea, si es diferente de null ya existe, osea se modifica
                    //            if (docpPrevio != null)//LEJGG 31-10-2018
                    //            {
                    //                DOCUMENTOP dp = docpPrevio;

                    //                dp.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //                dp.POS = j;
                    //                dp.ACCION = dOCUMENTO.DOCUMENTOP[i].ACCION;
                    //                dp.FACTURA = dOCUMENTO.DOCUMENTOP[i].FACTURA;
                    //                dp.TCONCEPTO = dOCUMENTO.DOCUMENTOP[i].TCONCEPTO;
                    //                dp.GRUPO = dOCUMENTO.DOCUMENTOP[i].GRUPO;
                    //                dp.CUENTA = dOCUMENTO.DOCUMENTOP[i].CUENTA;//Modificación Cuenta de PEP
                    //                dp.TIPOIMP = dOCUMENTO.DOCUMENTOP[i].TIPOIMP;
                    //                dp.IMPUTACION = dOCUMENTO.DOCUMENTOP[i].IMPUTACION;
                    //                dp.CCOSTO = dOCUMENTO.DOCUMENTOP[i].CCOSTO;
                    //                dp.MONTO = dOCUMENTO.DOCUMENTOP[i].MONTO;
                    //                _monto = _monto + dOCUMENTO.DOCUMENTOP[i].MONTO;//lejgg 10-10-2018
                    //                var sp = dOCUMENTO.DOCUMENTOP[i].MWSKZ.Split('-');
                    //                _mwskz = sp[0];//lejgg 10-10-2018
                    //                dp.MWSKZ = sp[0];
                    //                dp.IVA = dOCUMENTO.DOCUMENTOP[i].IVA;
                    //                _iva = _iva + dOCUMENTO.DOCUMENTOP[i].IVA;//lejgg 10-10-2018
                    //                dp.TOTAL = dOCUMENTO.DOCUMENTOP[i].TOTAL;
                    //                _total = _total + dOCUMENTO.DOCUMENTOP[i].TOTAL;//lejgg 10-10-2018
                    //                dp.TEXTO = dOCUMENTO.DOCUMENTOP[i].TEXTO;
                    //                db.Entry(dp).State = EntityState.Modified;
                    //                db.SaveChanges();

                    //            }
                    //            else
                    //            {
                    //                DOCUMENTOP dp = new DOCUMENTOP();

                    //                dp.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //                dp.POS = j;
                    //                dp.ACCION = dOCUMENTO.DOCUMENTOP[i].ACCION;
                    //                dp.FACTURA = dOCUMENTO.DOCUMENTOP[i].FACTURA;
                    //                dp.TCONCEPTO = dOCUMENTO.DOCUMENTOP[i].TCONCEPTO;
                    //                dp.GRUPO = dOCUMENTO.DOCUMENTOP[i].GRUPO;
                    //                dp.CUENTA = dOCUMENTO.DOCUMENTOP[i].CUENTA;//Modificación Cuenta de PEP
                    //                dp.TIPOIMP = dOCUMENTO.DOCUMENTOP[i].TIPOIMP;
                    //                dp.IMPUTACION = dOCUMENTO.DOCUMENTOP[i].IMPUTACION;
                    //                dp.CCOSTO = dOCUMENTO.DOCUMENTOP[i].CCOSTO;
                    //                dp.MONTO = dOCUMENTO.DOCUMENTOP[i].MONTO;
                    //                _monto = _monto + dOCUMENTO.DOCUMENTOP[i].MONTO;//lejgg 10-10-2018
                    //                var sp = dOCUMENTO.DOCUMENTOP[i].MWSKZ.Split('-');
                    //                _mwskz = sp[0];//lejgg 10-10-2018
                    //                dp.MWSKZ = sp[0];
                    //                dp.IVA = dOCUMENTO.DOCUMENTOP[i].IVA;
                    //                _iva = _iva + dOCUMENTO.DOCUMENTOP[i].IVA;//lejgg 10-10-2018
                    //                dp.TOTAL = dOCUMENTO.DOCUMENTOP[i].TOTAL;
                    //                _total = _total + dOCUMENTO.DOCUMENTOP[i].TOTAL;//lejgg 10-10-2018
                    //                dp.TEXTO = dOCUMENTO.DOCUMENTOP[i].TEXTO;
                    //                db.DOCUMENTOPs.Add(dp);
                    //                db.SaveChanges();
                    //            }
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            //
                    //        }
                    //        j++;
                    //    }


                    ////lejgg 10-10-2018-------------------->
                    ////Busco Informacion Previa del documento si es que existe.
                    //var docp2 = db.DOCUMENTOPs.Where(x => x.NUM_DOC == _ndoc && x.ACCION == "H").FirstOrDefault();
                    //    //Si es diferente a null, es que ya existe, sino se crea
                    //    if (docp2 != null)
                    //    {
                    //        DOCUMENTOP _dp = docp2;

                    //        _dp.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //        _dp.POS = j;
                    //        _dp.ACCION = "H";
                    //        _dp.CUENTA = _payerid_;
                    //        _dp.MONTO = _monto + _iva;//Obtener las retenciones relacionadas con las ya mostradas en la tabla
                    //        _dp.MWSKZ = _mwskz;
                    //        _dp.IVA = _iva;
                    //        _dp.TOTAL = _total;
                    //        db.Entry(_dp).State = EntityState.Modified;
                    //        db.SaveChanges();
                    //    }
                    //    else
                    //    {
                    //        DOCUMENTOP _dp = new DOCUMENTOP();
                    //        _dp.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //        _dp.POS = j;
                    //        _dp.ACCION = "H";
                    //        _dp.CUENTA = _payerid_;
                    //        _dp.MONTO = _monto + _iva;//Obtener las retenciones relacionadas con las ya mostradas en la tabla
                    //        _dp.MWSKZ = _mwskz;
                    //        _dp.IVA = _iva;
                    //        _dp.TOTAL = _total;
                    //        db.DOCUMENTOPs.Add(_dp);
                    //        db.SaveChanges();
                    //    }
                    //    //lejgg 10-10-2018-------------------------<
                    //}
                    ////Guardar las retenciones de la solicitud

                    //catch (Exception e)
                    //{
                    //    errorString = e.Message.ToString();
                    //    //Guardar número de documento creado

                    //}

                    ////Guardar las retenciones por posición
                    ////Lej14.09.2018------
                    //try
                    //{
                    //    for (int i = 0; i < dOCUMENTO.DOCUMENTORP.Count; i++)
                    //    {
                    //        //cantdidad de renglones añadidos y su posicion
                    //        var _op = ((i + 2) / 2);
                    //        var _pos = _op.ToString().Split('.');
                    //        try
                    //        {
                    //            var _str = dOCUMENTO.DOCUMENTORP[i].WITHT;
                    //            var _p_p = int.Parse(_pos[0]);
                    //            var _docrp = db.DOCUMENTORPs.Where(x => x.NUM_DOC == _ndoc && x.WITHT == _str && x.POS == _p_p).FirstOrDefault();
                    //            if (_docrp != null)//para cuando es edicion
                    //            {
                    //                var docr = dOCUMENTO.DOCUMENTOR;
                    //                var _wt_withcd = dOCUMENTO.DOCUMENTORP[i].WT_WITHCD;
                    //                //var ret = db.RETENCIONs.Where(x => x.WITHT == _str && x.WT_WITHCD == _wt_withcd).FirstOrDefault().WITHT_SUB;
                    //                // if (ret != null)
                    //                // {
                    //                // bool f = false;
                    //                /*for (int _a = 0; _a < docr.Count; _a++)
                    //                {
                    //                    //si encuentra coincidencia con una ret ligada a el
                    //                    if (docr[_a].WITHT == ret)
                    //                    {
                    //                        var p_s = int.Parse(_pos[0]);
                    //                        var _wt_w = dOCUMENTO.DOCUMENTORP[i].WT_WITHCD;
                    //                        DOCUMENTORP _docspr2 = db.DOCUMENTORPs.Where(x => x.NUM_DOC == _ndoc && x.POS == p_s && x.WITHT == _str && x.WT_WITHCD == _wt_w).FirstOrDefault();

                    //                        _docspr2.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                    //                        _docspr2.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;
                    //                        db.Entry(_docspr2).State = EntityState.Modified;
                    //                        db.SaveChanges();
                    //                        //valido si existe la ligada y la modifico o la creo segun sea el caso
                    //                        var _docrpl = db.DOCUMENTORPs.Where(x => x.NUM_DOC == _ndoc && x.WITHT == ret).FirstOrDefault();
                    //                        if (_docrpl != null)
                    //                        {
                    //                            var p_s2 = int.Parse(_pos[0]);
                    //                            var _wt_w2 = dOCUMENTO.DOCUMENTORP[i].WT_WITHCD;
                    //                            DOCUMENTORP _docspr3 = db.DOCUMENTORPs.Where(x => x.NUM_DOC == _ndoc && x.POS == p_s2 && x.WITHT == _str && x.WT_WITHCD == _wt_w2).FirstOrDefault();

                    //                            _docspr3.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                    //                            _docspr3.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;
                    //                            db.Entry(_docspr3).State = EntityState.Modified;
                    //                            db.SaveChanges();
                    //                            f = true;
                    //                        }
                    //                        else
                    //                        {
                    //                            //se crea la ret ligada
                    //                            var _dr = new DOCUMENTORP();
                    //                            _dr.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //                            _dr.WITHT = docr[_a].WITHT;
                    //                            _dr.WT_WITHCD = dOCUMENTO.DOCUMENTORP[i].WT_WITHCD;
                    //                            _dr.POS = int.Parse(_pos[0]);
                    //                            _dr.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                    //                            _dr.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;
                    //                            db.DOCUMENTORPs.Add(_dr);
                    //                            db.SaveChanges();
                    //                            f = true;
                    //                        }
                    //                    }
                    //                }*/
                    //                //if (f)
                    //                //{
                    //                //var p_s = int.Parse(_pos[0]);
                    //                //DOCUMENTORP _docspr2 = db.DOCUMENTORPs.Where(x => x.NUM_DOC == _ndoc && x.POS == p_s && x.WITHT == _str && x.WT_WITHCD == _wt_withcd).FirstOrDefault();
                    //                //if (_docspr2 != null)//signfica que hay valores, entonces se actualizan
                    //                //{
                    //                // _docspr2.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                    //                // _docspr2.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;

                    //                // db.Entry(_docspr2).State = EntityState.Modified;
                    //                // db.SaveChanges();
                    //                //la ligada
                    //                /* DOCUMENTORP _docrpligada = db.DOCUMENTORPs.Where(x => x.NUM_DOC == _ndoc && x.POS == p_s && x.WITHT == ret && x.WT_WITHCD == _wt_withcd).FirstOrDefault();
                    //                 if (_docrpligada != null)//signfica que hay valores, entonces se actualizan
                    //                 {
                    //                     _docrpligada.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                    //                     _docrpligada.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;

                    //                     db.Entry(_docrpligada).State = EntityState.Modified;
                    //                     db.SaveChanges();
                    //                 }
                    //                 else//sino, se crea
                    //                 {
                    //                     var docprlig = new DOCUMENTORP();
                    //                     docprlig.NUM_DOC = _docspr2.NUM_DOC;
                    //                     docprlig.POS = _docspr2.POS;
                    //                     docprlig.WITHT = ret;
                    //                     docprlig.WT_WITHCD = _docspr2.WT_WITHCD;
                    //                     docprlig.BIMPONIBLE = _docspr2.BIMPONIBLE;
                    //                     docprlig.IMPORTE_RET = _docspr2.IMPORTE_RET;
                    //                     db.DOCUMENTORPs.Add(docprlig);
                    //                     db.SaveChanges();
                    //                 }*/
                    //                // }
                    //                //}
                    //                //}
                    //                // else
                    //                // {
                    //                var p_s = int.Parse(_pos[0]);
                    //                DOCUMENTORP _docspr2 = db.DOCUMENTORPs.Where(x => x.NUM_DOC == _ndoc && x.POS == p_s && x.WITHT == _str && x.WT_WITHCD == _wt_withcd).FirstOrDefault();
                    //                if (_docspr2 != null)//signfica que hay valores, entonces se actualizan
                    //                {
                    //                    _docspr2.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                    //                    _docspr2.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;
                    //                    db.Entry(_docspr2).State = EntityState.Modified;
                    //                    db.SaveChanges();
                    //                }
                    //                // }
                    //            }
                    //            else//para cuando es nuevo
                    //            {
                    //                // var docr = dOCUMENTO.DOCUMENTOR;
                    //                /*var ret = db.RETENCIONs.Where(x => x.WITHT == _str).FirstOrDefault().WITHT_SUB;
                    //                if (ret != null)
                    //                {
                    //                    DOCUMENTORP dr = new DOCUMENTORP();
                    //                    dr.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //                    dr.WITHT = dOCUMENTO.DOCUMENTORP[i].WITHT;
                    //                    dr.WT_WITHCD = dOCUMENTO.DOCUMENTORP[i].WT_WITHCD;
                    //                    dr.POS = int.Parse(_pos[0]);
                    //                    dr.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                    //                    dr.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;
                    //                    db.DOCUMENTORPs.Add(dr);
                    //                    db.SaveChanges();
                    //                }
                    //                else
                    //                {*/
                    //                DOCUMENTORP dr = new DOCUMENTORP();
                    //                dr.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //                dr.WITHT = dOCUMENTO.DOCUMENTORP[i].WITHT;
                    //                dr.WT_WITHCD = dOCUMENTO.DOCUMENTORP[i].WT_WITHCD;
                    //                dr.POS = int.Parse(_pos[0]);
                    //                dr.BIMPONIBLE = dOCUMENTO.DOCUMENTORP[i].BIMPONIBLE;
                    //                dr.IMPORTE_RET = dOCUMENTO.DOCUMENTORP[i].IMPORTE_RET;
                    //                db.DOCUMENTORPs.Add(dr);
                    //                db.SaveChanges();
                    //                //}
                    //            }
                    //        }
                    //        catch (Exception e)
                    //        {
                    //        }
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    //
                    //}






                    //FRT22112018 esto ya estaba comentado


                    //Lej14.09.2018------
                    //Guardar las retenciones en el encabezado
                    /* try
                     {
                         //Obtener las retenciones relacionadas con las ya mostradas en la tabla------------------------------>
                         List<DOCUMENTOR_MOD> docrr = new List<DOCUMENTOR_MOD>();

                         List<RETENCION> retsub = new List<RETENCION>();

                         retsub = (from dr in dOCUMENTO.DOCUMENTOR.ToList()
                                   join ret1 in db.RETENCIONs.ToList()
                                   on new { dr.WITHT, dr.WT_WITHCD } equals new { ret1.WITHT, ret1.WT_WITHCD }
                                   select new RETENCION
                                   {
                                       WITHT = ret1.WITHT,
                                       WT_WITHCD = ret1.WT_WITHCD,
                                       DESCRIPCION = ret1.DESCRIPCION,
                                       ESTATUS = ret1.ESTATUS,
                                       WITHT_SUB = ret1.WITHT_SUB,
                                       PORC = ret1.PORC,
                                       WT_WITHCD_SUB = ret1.WT_WITHCD_SUB,
                                       CAMPO = ret1.CAMPO
                                   }
                                ).ToList();
                         //Guardar las retenciones de la solicitud
                         int ccr = 1;// Contador consecutivo ////MGC 29-10-2018
                         for (int i = 0; i < dOCUMENTO.DOCUMENTOR.Count; i++)
                         {
                             if (dOCUMENTO.DOCUMENTOR[i].BUKRS == dOCUMENTO.SOCIEDAD_ID && dOCUMENTO.DOCUMENTOR[i].LIFNR == dOCUMENTO.PAYER_ID)
                             {
                                 DOCUMENTOR dr = new DOCUMENTOR();
                                 try
                                 {
                                     dr.NUM_DOC = dOCUMENTO.NUM_DOC;
                                     dr.WITHT = dOCUMENTO.DOCUMENTOR[i].WITHT;
                                     dr.WT_WITHCD = dOCUMENTO.DOCUMENTOR[i].WT_WITHCD;
                                     dr.POS = ccr; // Contador consecutivo ////MGC 29-10-2018
                                     dr.BIMPONIBLE = dOCUMENTO.DOCUMENTOR[i].BIMPONIBLE;
                                     dr.IMPORTE_RET = dOCUMENTO.DOCUMENTOR[i].IMPORTE_RET;
                                     dr.VISIBLE = true;
                                     db.DOCUMENTORs.Add(dr);
                                     db.SaveChanges();

                                     //Obtener las retenciones relacionadas con las ya mostradas en la tabla
                                     ccr++;// Contador consecutivo ////MGC 29-10-2018
                                 }
                                 catch (Exception e)
                                 {
                                 }

                                 //Obtener la relacionada
                                 RETENCION retrel = retsub.Where(rs => rs.WITHT == dOCUMENTO.DOCUMENTOR[i].WITHT && rs.WT_WITHCD == dOCUMENTO.DOCUMENTOR[i].WT_WITHCD).FirstOrDefault();

                                 if (retrel != null)
                                 {
                                     if (retrel.WITHT_SUB != null | retrel.WITHT_SUB != "")
                                     {
                                         DOCUMENTOR drr = new DOCUMENTOR();
                                         try
                                         {

                                             drr.NUM_DOC = dOCUMENTO.NUM_DOC;

                                             drr.WITHT = retrel.WITHT_SUB;
                                             drr.WT_WITHCD = retrel.WT_WITHCD_SUB;
                                             drr.POS = ccr; // Contador consecutivo //
                                             drr.BIMPONIBLE = dOCUMENTO.DOCUMENTOR[i].BIMPONIBLE;
                                             drr.IMPORTE_RET = dOCUMENTO.DOCUMENTOR[i].IMPORTE_RET;
                                             drr.VISIBLE = false;
                                             db.DOCUMENTORs.Add(drr);
                                             db.SaveChanges();
                                             //Obtener las retenciones relacionadas con las ya mostradas en la tabla
                                             ccr++;// Contador consecutivo
                                         }
                                         catch (Exception e)
                                         {
                                         }
                                     }
                                 }
                             }
                         }
                     }
                     catch (Exception e)
                     {
                         errorString = e.Message.ToString();
                         //Guardar número de documento creado

                     }*/
                    //Lej26.09.2018------
                    //Lejgg 09-11-2018-------------


                    //FRT22112018 esto ya estaba comentado

                    //FRT22112018 Se comenta para agregar y eliminar detalles
                    //try
                    //{
                    //    var _docrets = dOCUMENTO.DOCUMENTOR;
                    //    if (_docrets != null)//significa que hay valores
                    //    {
                    //        for (int i = 0; i < _docrets.Count; i++)
                    //        {
                    //            var _with = _docrets[i].WITHT.Trim();
                    //            var _wt_withcd = _docrets[i].WT_WITHCD.Trim();
                    //            var _witht_sub = db.RETENCIONs.Where(a => a.WITHT == _with && a.WT_WITHCD == _wt_withcd).FirstOrDefault().WITHT_SUB;
                    //            var bd_docr = db.DOCUMENTORs.Where(d => d.NUM_DOC == _ndoc && d.WITHT == _with).FirstOrDefault();
                    //            if (_witht_sub != null)
                    //            {
                    //                //Se actualizan las f 
                    //                bd_docr.BIMPONIBLE = _docrets[i].BIMPONIBLE;
                    //                bd_docr.IMPORTE_RET = _docrets[i].IMPORTE_RET;
                    //                db.Entry(bd_docr).State = EntityState.Modified;
                    //                db.SaveChanges();
                    //                //Se actualizan lasligadas
                    //                var bd_docrl = db.DOCUMENTORs.Where(d => d.NUM_DOC == _ndoc && d.WITHT == _witht_sub).FirstOrDefault();
                    //                bd_docrl.BIMPONIBLE = _docrets[i].BIMPONIBLE;
                    //                bd_docrl.IMPORTE_RET = _docrets[i].IMPORTE_RET;
                    //                db.Entry(bd_docrl).State = EntityState.Modified;
                    //                db.SaveChanges();
                    //            }
                    //            else
                    //            {
                    //                bd_docr.BIMPONIBLE = _docrets[i].BIMPONIBLE;
                    //                bd_docr.IMPORTE_RET = _docrets[i].IMPORTE_RET;
                    //                db.Entry(bd_docr).State = EntityState.Modified;
                    //                db.SaveChanges();
                    //            }

                    //        }
                    //    }
                    //}
                    //catch (Exception e) { }

                    //END FRT22112018 se comenta para agregar y eliminar detalles



                    //FRT25112018 AQUI QUITE LOS ANEXOS

                    //FRT27112018

                    if (Uuid != string.Empty)
                    {
                        var pos = db.DOCUMENTOUUIDs.ToList();
                        DOCUMENTOUUID duid = new DOCUMENTOUUID();
                        duid.NUM_DOC = dOCUMENTO.NUM_DOC;
                        duid.POS = pos.Count + 1;
                        duid.DOCUMENTO_SAP = "";
                        duid.UUID = Uuid;
                        duid.ESTATUS = true;
                        db.DOCUMENTOUUIDs.Add(duid);
                        db.SaveChanges();
                    }


                    //Lejgg 26.10.2018----------------------------------------<
                    //Lejgg 05.11.2018---------------------------------------->
                    //if (Uuid != string.Empty)
                    //{
                    //    //reviso si existe ese uuid
                    //    var _uuid = db.DOCUMENTOUUIDs.Where(u => u.UUID == Uuid).FirstOrDefault();
                    //    if (_uuid == null)//si es null, no existe,osea que se crea.
                    //    {
                    //        //
                    //        var pos = db.DOCUMENTOUUIDs.ToList();
                    //        DOCUMENTOUUID duid = new DOCUMENTOUUID();
                    //        duid.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //        duid.POS = pos.Count + 1;
                    //        duid.DOCUMENTO_SAP = "";
                    //        duid.UUID = Uuid;
                    //        duid.ESTATUS = true;
                    //        db.DOCUMENTOUUIDs.Add(duid);
                    //        db.SaveChanges();
                    //    }
                    //    else//sino, se revisa si ese numdoc tiene ya un uuid
                    //    {
                    //        var _uuid2 = db.DOCUMENTOUUIDs.Where(u => u.NUM_DOC == _ndoc).FirstOrDefault();
                    //        if (_uuid2 == null)//sino se crea
                    //        {
                    //            //
                    //            var pos = db.DOCUMENTOUUIDs.ToList();
                    //            DOCUMENTOUUID duid = new DOCUMENTOUUID();
                    //            duid.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //            duid.POS = pos.Count + 1;
                    //            duid.DOCUMENTO_SAP = "";
                    //            duid.UUID = Uuid;
                    //            duid.ESTATUS = true;
                    //            db.DOCUMENTOUUIDs.Add(duid);
                    //            db.SaveChanges();
                    //        }
                    //        else//sino se modifica
                    //        {
                    //            //
                    //            var pos = db.DOCUMENTOUUIDs.ToList();
                    //            DOCUMENTOUUID duid = _uuid2;
                    //            duid.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //            duid.POS = pos.Count + 1;
                    //            duid.DOCUMENTO_SAP = "";
                    //            duid.UUID = Uuid;
                    //            duid.ESTATUS = true;
                    //            db.Entry(duid).State = EntityState.Modified;
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //}
                    //Lejgg 05.11.2018----------------------------------------<
                }
                catch (Exception e)
                {

                }



                //FRT03122018 Para Guardar el borrador sin afectar work
                if (borr != "B")
                {
                    if (est == "B")
                    {
                        //Inicia un nuevo flujo de trabajo
                        //MATIAS CODIGO
                        //MATIAS CODIGO
                        //----------------------29-10-2018-------------------\\
                        //MGC 02-10-2018 Cadena de autorización work flow --->
                        //Flujo
                        var _docf = db.DOCUMENTOes.Where(n => n.NUM_DOC == dOCUMENTO.NUM_DOC).FirstOrDefault();
                        ProcesaFlujo pf = new ProcesaFlujo();
                        //Comienza el wf
                        //Se obtiene la cabecera
                        try
                        {
                            WORKFV wf = db.WORKFHs.Where(a => a.TSOL_ID.Equals(_docf.TSOL_ID)).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();

                            DET_AGENTECC deta = new DET_AGENTECC();
                            try
                            {
                                deta.VERSION = Convert.ToInt32(DETTA_VERSION);
                            }
                            catch (Exception e)
                            {

                            }

                            try
                            {
                                deta.USUARIOC_ID = DETTA_USUARIOC_ID;
                            }
                            catch (Exception e)
                            {

                            }


                            try
                            {
                                deta.ID_RUTA_AGENTE = DETTA_ID_RUTA_AGENTE;
                            }
                            catch (Exception e)
                            {

                            }

                            try
                            {
                                deta.USUARIOA_ID = DETTA_USUARIOA_ID;
                            }
                            catch (Exception e)
                            {

                            }


                            //MGC 11-12-2018 Agregar Contabilizador 0------>
                            int vc1 = 0;
                            int vc2 = 0;
                            try
                            {
                                vc1 = Convert.ToInt32(VERSIONC1);
                            }
                            catch (Exception e)
                            {

                            }

                            try
                            {
                                vc2 = Convert.ToInt32(VERSIONC2);
                            }
                            catch (Exception e)
                            {

                            }
                            //MGC 11-12-2018 Agregar Contabilizador 0------<



                            if (wf != null)
                            {
                                WORKFP wp = wf.WORKFPs.OrderBy(a => a.POS).FirstOrDefault();
                                string email = ""; //MGC 08-10-2018 Obtener el nombre del cliente
                                email = wp.EMAIL; //MGC 08-10-2018 Obtener el nombre del cliente


                                FLUJO f = new FLUJO();
                                f.WORKF_ID = wf.ID;
                                f.WF_VERSION = wf.VERSION;
                                f.WF_POS = wp.POS;
                                f.NUM_DOC = _docf.NUM_DOC;
                                f.POS = 1;
                                f.LOOP = 1;
                                f.USUARIOA_ID = _docf.USUARIOC_ID;
                                f.USUARIOD_ID = _docf.USUARIOD_ID;
                                f.ESTATUS = "I";
                                f.FECHAC = DateTime.Now;
                                f.FECHAM = DateTime.Now;
                                f.STEP_AUTO = 0;

                                //Ruta tomada
                                f.ID_RUTA_A = deta.ID_RUTA_AGENTE;
                                f.RUTA_VERSION = deta.VERSION;

                                //MGC 11-12-2018 Agregar Contabilizador 0
                                f.VERSIONC1 = vc1;
                                f.VERSIONC2 = vc2;

                                //MGC 05-10-2018 Modificación para work flow al ser editada
                                string c = pf.procesa(f, "", false, email, "");
                                //while (c == "1")
                                //{
                                //    Email em = new Email();
                                //    string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                                //    string image = Server.MapPath("~/images/logo_kellogg.png");
                                //    em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image);

                                //    FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                                //    if (conta.WORKFP.ACCION.TIPO == "B")
                                //    {
                                //        WORKFP wpos = db.WORKFPs.Where(x => x.ID == conta.WORKF_ID & x.VERSION == conta.WF_VERSION & x.POS == conta.WF_POS).FirstOrDefault();
                                //        //FLUJO f1 = new FLUJO();
                                //        //f1.WORKF_ID = conta.WORKF_ID;
                                //        //f1.WF_VERSION = conta.WF_VERSION;
                                //        //f1.WF_POS = (int)wpos.NEXT_STEP;
                                //        //f1.NUM_DOC = dOCUMENTO.NUM_DOC;
                                //        //f1.POS = conta.POS + 1;
                                //        //f1.LOOP = 1;
                                //        ////f1.USUARIOA_ID = dOCUMENTO.USUARIOC_ID;
                                //        ////f1.USUARIOD_ID = dOCUMENTO.USUARIOD_ID;
                                //        conta.ESTATUS = "A";
                                //        //f1.FECHAC = DateTime.Now;
                                //        conta.FECHAM = DateTime.Now;
                                //        c = pf.procesa(conta, "");
                                //    }
                                //    else
                                //    {
                                //        c = "";
                                //    }
                                //}

                            }

                        }
                        catch (Exception ee)
                        {
                            if (errorString == "")
                            {
                                errorString = ee.Message.ToString();
                            }
                            ViewBag.error = errorString;
                        }
                        //MGC 02-10-2018 Cadena de autorización work flow <---
                        //MATIAS CODIGO
                        //MATIAS CODIGO
                    }
                    else
                    {
                        //MGC 05-10-2018 Modificación para work flow al ser editada -->
                        //Flujo
                        ProcesaFlujo pf = new ProcesaFlujo();
                        //Comienza el wf
                        //Se obtiene la cabecera
                        try
                        {
                            string nuevo = "N";
                            FLUJO f = new FLUJO();
                            var _docf = db.DOCUMENTOes.Where(n => n.NUM_DOC == dOCUMENTO.NUM_DOC).FirstOrDefault();
                            //MGC 03-11-2018.2 Obtener el flujo de la creación
                            if (_docf.ESTATUS == "N" & _docf.ESTATUS_PRE == "E")
                            {
                                f = db.FLUJOes.Where(a => a.NUM_DOC.Equals(dOCUMENTO.NUM_DOC)).OrderByDescending(x => x.POS).FirstOrDefault();

                                //MGC 04-11-2018 Generar Archivos ----------------------------------------------------------------->

                                //Crear el archivo para el preliminar //MGC Preliminar
                                string corr = pf.procesaPreliminar(_docf, false, "", false, "");//MGC-14-12-2018 Modificación fechacon

                                //Se genero el preliminar
                                if (corr == "0")
                                {
                                    //DOCUMENTOPRE dp = new DOCUMENTOPRE();

                                    //dp.NUM_DOC = _docf.NUM_DOC;
                                    //dp.POS = 1;
                                    //dp.MESSAGE = "Generando Preliminar";
                                    //try
                                    //{
                                    //    db.DOCUMENTOPREs.Add(dp);
                                    //    db.SaveChanges();
                                    //}
                                    //catch (Exception e)
                                    //{
                                    //    string r = "";
                                    //}

                                    //MGC 30-10-2018 Agregar mensaje a log de modificación
                                    try
                                    {
                                        DOCUMENTOLOG dl = new DOCUMENTOLOG();

                                        dl.NUM_DOC = _docf.NUM_DOC;
                                        dl.TYPE_LINE = "M";
                                        dl.TYPE = "S";
                                        dl.MESSAGE = "Se generó el Archivo Preliminar";
                                        dl.FECHA = DateTime.Now;

                                        db.DOCUMENTOLOGs.Add(dl);
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                    //MGC 30-10-2018 Agregar mensaje a log de modificación

                                    //Actualizar wf del documento
                                    //d.ESTATUS_WF = "A";//MGC 30-10-2018 Modificaión para validar creación del archivo

                                    if (true)
                                    {
                                        _docf.ESTATUS = "N"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                                        _docf.ESTATUS_WF = "P"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                                        _docf.ESTATUS_SAP = null; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                                    }

                                    //MGC 30-10-2018 Actualizar el estatus de preliminar del doc
                                    _docf.ESTATUS_PRE = "G";//MGC 30-10-2018 Modificaión para validar creación del archivo
                                    db.Entry(_docf).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                //No se genero el preliminar
                                else
                                {
                                    string m;
                                    if (corr.Length > 50)
                                    {
                                        m = corr.Substring(0, 50);
                                    }
                                    else
                                    {
                                        m = corr;
                                    }
                                    //DOCUMENTOPRE dp = new DOCUMENTOPRE();

                                    //dp.NUM_DOC = _docf.NUM_DOC;
                                    //dp.POS = 1;
                                    //dp.MESSAGE = m;
                                    //try
                                    //{
                                    //    db.DOCUMENTOPREs.Add(dp);
                                    //    db.SaveChanges();
                                    //}
                                    //catch (Exception E)
                                    //{
                                    //    string r = "";
                                    //}

                                    //MGC 30-10-2018 Agregar mensaje a log de modificación
                                    try
                                    {
                                        DOCUMENTOLOG dl = new DOCUMENTOLOG();

                                        dl.NUM_DOC = _docf.NUM_DOC;
                                        dl.TYPE_LINE = "M";
                                        dl.TYPE = "E";
                                        dl.MESSAGE = m;
                                        dl.FECHA = DateTime.Now;

                                        db.DOCUMENTOLOGs.Add(dl);
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                    //MGC 30-10-2018 Agregar mensaje a log de modificación

                                    //MGC 30-10-2018 Actualizar el estatus de preliminar del doc
                                    _docf.ESTATUS_PRE = "E";//MGC 30-10-2018 Modificaión Estatus en la creación del archivo
                                    db.Entry(_docf).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //Pendiente en edit, regresar a estatus de P en la creación del flujo

                                }


                                //MGC 04-11-2018 Generar Archivos -----------------------------------------------------------------<



                            }
                            else
                            {
                                //Obtener el último flujo
                                f = db.FLUJOes.Where(a => a.NUM_DOC.Equals(dOCUMENTO.NUM_DOC) & a.ESTATUS.Equals("P")).FirstOrDefault();


                                DET_AGENTECC deta = new DET_AGENTECC();

                                //f.ID_RUTA_A = f.ID_RUTA_A.Replace(" ", string.Empty);

                                //Obtener la ruta seleccionada desde la creación (inicio)
                                deta = db.DET_AGENTECC.Where(dcc => dcc.VERSION == f.RUTA_VERSION && dcc.USUARIOC_ID == f.USUARIOA_ID && dcc.ID_RUTA_AGENTE == f.ID_RUTA_A).FirstOrDefault();

                                //Si la ruta existe
                                if (deta != null)
                                {

                                    FLUJO fe = new FLUJO();
                                    fe.WORKF_ID = f.WORKF_ID;
                                    fe.WF_VERSION = f.WF_VERSION;
                                    fe.WF_POS = f.WF_POS;
                                    fe.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    fe.POS = f.POS;
                                    //fe.LOOP = 1;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                    fe.LOOP = f.LOOP;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                    //fe.USUARIOA_ID = dOCUMENTO.USUARIOC_ID;
                                    //fe.USUARIOD_ID = dOCUMENTO.USUARIOD_ID;
                                    fe.USUARIOA_ID = _docf.USUARIOC_ID;
                                    fe.USUARIOD_ID = _docf.USUARIOD_ID;
                                    fe.ESTATUS = "I";
                                    fe.FECHAC = DateTime.Now;
                                    fe.FECHAM = DateTime.Now;
                                    fe.STEP_AUTO = f.STEP_AUTO;

                                    //Ruta tomada
                                    fe.ID_RUTA_A = deta.ID_RUTA_AGENTE;
                                    fe.RUTA_VERSION = deta.VERSION;

                                    //MGC 11-12-2018 Agregar Contabilizador 0
                                    fe.VERSIONC1 = f.VERSIONC1;
                                    fe.VERSIONC2 = f.VERSIONC2;

                                    string c = pf.procesa(fe, "", true, "", "");
                                }
                            }
                        }
                        catch (Exception ee)
                        {
                            if (errorString == "")
                            {
                                errorString = ee.Message.ToString();
                            }
                            ViewBag.error = errorString;
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }

                //ENDFRT03122018 Para Guardar el borrador sin afectar work




            }



            var _numdoc = dOCUMENTO.NUM_DOC;
            var rpb = "";
            try
            {
                rpb = db.DOCUMENTOes.Where(x => x.NUM_DOC == _numdoc).FirstOrDefault().ESTATUS.Trim();
            }
            catch (Exception e)
            {

            }
            int pagina = 204;//lEJGG 7-11-18
            if (rpb == "B")
            {
                pagina = 209; //ID EN BASE DE DATOS
            }
            else
            {
                pagina = 204; //ID EN BASE DE DATOS
            }
            string spras = "";
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                spras = user.SPRAS_ID;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try//Mensaje de documento creado
                {
                    string p = Session["NUM_DOC"].ToString();
                    ViewBag.NUM_DOC = p;
                    Session["NUM_DOC"] = null;
                }
                catch
                {
                    ViewBag.NUM_DOC = "";
                }
            }
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", dOCUMENTO.SOCIEDAD_ID);
            ViewBag.TSOL_ID = new SelectList(db.TSOLs, "ID", "DESCRIPCION", dOCUMENTO.TSOL_ID);
            ViewBag.USUARIOC_ID = new SelectList(db.USUARIOs, "ID", "PASS", dOCUMENTO.USUARIOC_ID);
            return View(dOCUMENTO);
        }




        //Lejgg 11-10-2018
        [HttpPost]
        public ActionResult Borrador([Bind(Include = "NUM_DOC,NUM_PRE,TSOL_ID,TALL_ID,SOCIEDAD_ID,CANTIDAD_EV,USUARIOC_ID," +
            "USUARIOD_ID,FECHAD,FECHAC,HORAC,FECHAC_PLAN,FECHAC_USER,HORAC_USER,ESTATUS,ESTATUS_C,ESTATUS_SAP,ESTATUS_WF," +
            "DOCUMENTO_REF,CONCEPTO,NOTAS,MONTO_DOC_MD,MONTO_FIJO_MD,MONTO_BASE_GS_PCT_MD,MONTO_BASE_NS_PCT_MD,MONTO_DOC_ML," +
            "MONTO_FIJO_ML,MONTO_BASE_GS_PCT_ML,MONTO_BASE_NS_PCT_ML,MONTO_DOC_ML2,MONTO_FIJO_ML2,MONTO_BASE_GS_PCT_ML2," +
            "MONTO_BASE_NS_PCT_ML2,PORC_ADICIONAL,IMPUESTO,ESTATUS_EXT,PAYER_ID,MONEDA_ID,MONEDAL_ID,MONEDAL2_ID," +
            "TIPO_CAMBIO,TIPO_CAMBIOL,TIPO_CAMBIOL2,NO_FACTURA,FECHAD_SOPORTE,METODO_PAGO,NO_PROVEEDOR,PASO_ACTUAL," +
            "AGENTE_ACTUAL,FECHA_PASO_ACTUAL,PUESTO_ID,GALL_ID,CONCEPTO_ID,DOCUMENTO_SAP,FECHACON,FECHA_BASE,REFERENCIA," +
            "CONDICIONES,TEXTO_POS,ASIGNACION_POS,CLAVE_CTA, DOCUMENTOP,DOCUMENTOR,DOCUMENTORP,Anexo")]Models.DOCUMENTO_MOD doc, IEnumerable<HttpPostedFileBase> file_sopAnexar, string[] labels_desc)
        {
            int pagina = 202; //ID EN BASE DE DATOS
            string errorString = "";
            string _res = "false";
            FORMATO formato = new FORMATO();
            string spras = "";
            string user_id = ""; //Cadenas de autorización
            if (ModelState.IsValid)
            {
                try
                {
                    DOCUMENTO dOCUMENTO = new DOCUMENTO();

                    //Copiar valores del post al nuevo objeto
                    dOCUMENTO.TSOL_ID = doc.TSOL_ID;
                    dOCUMENTO.SOCIEDAD_ID = doc.SOCIEDAD_ID;
                    dOCUMENTO.FECHAD = doc.FECHAD;
                    dOCUMENTO.FECHACON = doc.FECHACON;
                    dOCUMENTO.FECHA_BASE = doc.FECHA_BASE;
                    dOCUMENTO.MONEDA_ID = doc.MONEDA_ID;
                    dOCUMENTO.TIPO_CAMBIO = doc.TIPO_CAMBIO;
                    dOCUMENTO.IMPUESTO = doc.IMPUESTO;
                    //dOCUMENTO.MONTO_DOC_MD = doc.MONTO_DOC_MD;
                    //LEJGG 10-10-2018------------------------>
                    try
                    {
                        dOCUMENTO.MONTO_DOC_MD = doc.DOCUMENTOP[0].TOTAL;
                    }
                    catch (Exception e)
                    {
                        dOCUMENTO.MONTO_DOC_MD = 0;
                    }
                    //LEJGG 10-10-2018------------------------<
                    //dOCUMENTO.REFERENCIA = doc.REFERENCIA;
                    dOCUMENTO.CONCEPTO = doc.CONCEPTO;
                    dOCUMENTO.PAYER_ID = doc.PAYER_ID;
                    dOCUMENTO.CONDICIONES = doc.CONDICIONES;
                    dOCUMENTO.TEXTO_POS = doc.TEXTO_POS;
                    dOCUMENTO.ASIGNACION_POS = doc.ASIGNACION_POS;
                    dOCUMENTO.CLAVE_CTA = doc.CLAVE_CTA;

                    //B20180625 MGC 2018.06.25
                    //Obtener usuarioc
                    USUARIO us = db.USUARIOs.Find(User.Identity.Name);//RSG 02/05/2018
                    dOCUMENTO.PUESTO_ID = us.PUESTO_ID;//RSG 02/05/2018
                    dOCUMENTO.USUARIOC_ID = User.Identity.Name;


                    //Obtener el rango de números
                    Rangos rangos = new Rangos();//RSG 01.08.2018
                                                 //Obtener el número de documento
                    decimal N_DOC = rangos.getSolID(dOCUMENTO.TSOL_ID);
                    dOCUMENTO.NUM_DOC = N_DOC;
                    //Actualizar el rango
                    rangos.updateRango(dOCUMENTO.TSOL_ID, dOCUMENTO.NUM_DOC);

                    //Referencia
                    dOCUMENTO.REFERENCIA = dOCUMENTO.NUM_DOC.ToString();//LEJ 11.09.2018

                    //Obtener el tipo de documento
                    var doct = db.DET_TIPODOC.Where(dt => dt.TIPO_SOL == doc.TSOL_ID).FirstOrDefault();
                    doc.DOCUMENTO_SAP = doct.BLART;

                    //Fechac
                    dOCUMENTO.FECHAC = DateTime.Now;

                    //Horac
                    dOCUMENTO.HORAC = DateTime.Now.TimeOfDay;

                    //FECHAC_PLAN
                    dOCUMENTO.FECHAC_PLAN = DateTime.Now.Date;

                    //FECHAC_USER
                    dOCUMENTO.FECHAC_USER = DateTime.Now.Date;

                    //HORAC_USER
                    dOCUMENTO.HORAC_USER = DateTime.Now.TimeOfDay;

                    //Estatus
                    dOCUMENTO.ESTATUS = "B";//Lej 11-10-2018

                    //Estatus wf
                    dOCUMENTO.ESTATUS_WF = "P";

                    db.DOCUMENTOes.Add(dOCUMENTO);
                    db.SaveChanges();//Codigolej

                    doc.NUM_DOC = dOCUMENTO.NUM_DOC;
                    //return RedirectToAction("Index");

                    //Redireccionar al inicio
                    //Guardar número de documento creado
                    Session["NUM_DOC"] = dOCUMENTO.NUM_DOC;

                    //Guardar las posiciones de la solicitud
                    try
                    {
                        decimal _monto = 0;
                        decimal _iva = 0;
                        decimal _total = 0;
                        var _mwskz = "";
                        int j = 1;
                        for (int i = 0; i < doc.DOCUMENTOP.Count; i++)
                        {
                            try
                            {
                                DOCUMENTOP dp = new DOCUMENTOP();

                                dp.NUM_DOC = doc.NUM_DOC;
                                dp.POS = j;
                                dp.ACCION = doc.DOCUMENTOP[i].ACCION;
                                dp.FACTURA = doc.DOCUMENTOP[i].FACTURA;
                                dp.TCONCEPTO = doc.DOCUMENTOP[i].TCONCEPTO;
                                dp.GRUPO = doc.DOCUMENTOP[i].GRUPO;
                                dp.CUENTA = doc.PAYER_ID;
                                dp.TIPOIMP = doc.DOCUMENTOP[i].TIPOIMP;
                                dp.IMPUTACION = doc.DOCUMENTOP[i].IMPUTACION;
                                dp.CCOSTO = doc.DOCUMENTOP[i].CCOSTO;
                                dp.MONTO = doc.DOCUMENTOP[i].MONTO;
                                _monto = _monto + doc.DOCUMENTOP[i].MONTO;//lejgg 10-10-2018
                                var sp = doc.DOCUMENTOP[i].MWSKZ.Split('-');
                                _mwskz = sp[0];//lejgg 10-10-2018
                                dp.MWSKZ = sp[0];
                                dp.IVA = doc.DOCUMENTOP[i].IVA;
                                _iva = _iva + doc.DOCUMENTOP[i].IVA;//lejgg 10-10-2018
                                dp.TOTAL = doc.DOCUMENTOP[i].TOTAL;
                                _total = doc.DOCUMENTOP[i].TOTAL;//lejgg 10-10-2018
                                dp.TEXTO = doc.DOCUMENTOP[i].TEXTO;
                                db.DOCUMENTOPs.Add(dp);
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                //
                            }
                            j++;
                        }
                        //lejgg 10-10-2018-------------------->
                        DOCUMENTOP _dp = new DOCUMENTOP();

                        _dp.NUM_DOC = doc.NUM_DOC;
                        _dp.POS = j;
                        _dp.ACCION = "H";
                        _dp.CUENTA = doc.PAYER_ID;
                        _dp.MONTO = _monto;
                        _dp.MWSKZ = _mwskz;
                        _dp.IVA = _iva;
                        _dp.TOTAL = _total;
                        db.DOCUMENTOPs.Add(_dp);
                        db.SaveChanges();
                        //lejgg 10-10-2018-------------------------<
                    }
                    //Guardar las retenciones de la solicitud

                    catch (Exception e)
                    {
                        errorString = e.Message.ToString();
                        //Guardar número de documento creado

                    }

                    //Lej14.09.2018------
                    try
                    {
                        for (int i = 0; i < doc.DOCUMENTORP.Count; i++)
                        {
                            //cantdidad de renglones añadidos y su posicion
                            var _op = ((i + 2) / 2);
                            var _pos = _op.ToString().Split('.');
                            try
                            {
                                var docr = doc.DOCUMENTOR;
                                var _str = doc.DOCUMENTORP[i].WITHT;
                                var ret = db.RETENCIONs.Where(x => x.WITHT == _str).FirstOrDefault().WITHT_SUB;
                                if (ret != null)
                                {
                                    bool f = false;
                                    for (int _a = 0; _a < docr.Count; _a++)
                                    {
                                        //si encuentra coincidencia
                                        if (docr[_a].WITHT == ret)
                                        {
                                            DOCUMENTORP _dr = new DOCUMENTORP();
                                            _dr.NUM_DOC = doc.NUM_DOC;
                                            _dr.WITHT = doc.DOCUMENTORP[i].WITHT;
                                            _dr.WT_WITHCD = doc.DOCUMENTORP[i].WT_WITHCD;
                                            _dr.POS = int.Parse(_pos[0]);
                                            _dr.BIMPONIBLE = doc.DOCUMENTORP[i].BIMPONIBLE;
                                            _dr.IMPORTE_RET = doc.DOCUMENTORP[i].IMPORTE_RET;
                                            db.DOCUMENTORPs.Add(_dr);
                                            db.SaveChanges();
                                            _dr = new DOCUMENTORP();
                                            _dr.NUM_DOC = doc.NUM_DOC;
                                            _dr.WITHT = docr[_a].WITHT;
                                            _dr.WT_WITHCD = doc.DOCUMENTORP[i].WT_WITHCD;
                                            _dr.POS = int.Parse(_pos[0]);
                                            _dr.BIMPONIBLE = doc.DOCUMENTORP[i].BIMPONIBLE;
                                            _dr.IMPORTE_RET = doc.DOCUMENTORP[i].IMPORTE_RET;
                                            db.DOCUMENTORPs.Add(_dr);
                                            db.SaveChanges();
                                            f = true;
                                        }
                                    }
                                    if (!f)
                                    {
                                        DOCUMENTORP _dr = new DOCUMENTORP();
                                        _dr.NUM_DOC = doc.NUM_DOC;
                                        _dr.WITHT = doc.DOCUMENTORP[i].WITHT;
                                        _dr.WT_WITHCD = doc.DOCUMENTORP[i].WT_WITHCD;
                                        _dr.POS = int.Parse(_pos[0]);
                                        _dr.BIMPONIBLE = doc.DOCUMENTORP[i].BIMPONIBLE;
                                        _dr.IMPORTE_RET = doc.DOCUMENTORP[i].IMPORTE_RET;
                                        db.DOCUMENTORPs.Add(_dr);
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    DOCUMENTORP dr = new DOCUMENTORP();
                                    dr.NUM_DOC = doc.NUM_DOC;
                                    dr.WITHT = doc.DOCUMENTORP[i].WITHT;
                                    dr.WT_WITHCD = doc.DOCUMENTORP[i].WT_WITHCD;
                                    dr.POS = int.Parse(_pos[0]);
                                    dr.BIMPONIBLE = doc.DOCUMENTORP[i].BIMPONIBLE;
                                    dr.IMPORTE_RET = doc.DOCUMENTORP[i].IMPORTE_RET;
                                    db.DOCUMENTORPs.Add(dr);
                                    db.SaveChanges();
                                }
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //
                    }
                    //Lej14.09.2018------

                    try//LEJ 05.09.2018
                    {
                        //Guardar las retenciones de la solicitud
                        for (int i = 0; i < doc.DOCUMENTOR.Count; i++)
                        {
                            if (doc.DOCUMENTOR[i].BUKRS == doc.SOCIEDAD_ID && doc.DOCUMENTOR[i].LIFNR == doc.PAYER_ID)
                            {
                                try
                                {
                                    DOCUMENTOR dr = new DOCUMENTOR();
                                    //dr.NUM_DOC = decimal.Parse(Session["NUM_DOC"].ToString());
                                    dr.NUM_DOC = doc.NUM_DOC;
                                    dr.WITHT = doc.DOCUMENTOR[i].WITHT;
                                    dr.WT_WITHCD = doc.DOCUMENTOR[i].WT_WITHCD;
                                    dr.POS = db.DOCUMENTORs.ToList().Count + 1;
                                    dr.BIMPONIBLE = doc.DOCUMENTOR[i].BIMPONIBLE;
                                    dr.IMPORTE_RET = doc.DOCUMENTOR[i].IMPORTE_RET;
                                    db.DOCUMENTORs.Add(dr);
                                    db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        errorString = e.Message.ToString();
                        //Guardar número de documento creado

                    }
                    //Lej26.09.2018------
                    List<string> listaDirectorios = new List<string>();
                    List<string> listaNombreArchivos = new List<string>();
                    List<string> listaDescArchivos = new List<string>();
                    try
                    {
                        //Guardar los documentos cargados en la sección de soporte
                        var res = "";
                        string errorMessage = "";
                        int numFiles = 0;
                        //Checar si hay archivos para subir
                        foreach (HttpPostedFileBase file in file_sopAnexar)
                        {
                            if (file != null)
                            {
                                if (file.ContentLength > 0)
                                {
                                    numFiles++;
                                }
                            }
                        }
                        if (numFiles > 0)
                        {
                            //Obtener las variables con los datos de sesión y ruta
                            string url = ConfigurationManager.AppSettings["URL_Serv"];
                            var bandera = false;
                            try
                            {
                                //WebRequest request = WebRequest.Create(url + "Nueva Carpeta");
                                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + url));
                                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                                const string Comillas = "\"";
                                string pwd = "Rumaki,2571" + Comillas + "k41";
                                requestDir.Credentials = new NetworkCredential("luis.gonzalez", pwd);
                                requestDir.UsePassive = true;
                                requestDir.UseBinary = true;
                                requestDir.KeepAlive = false;
                                using (FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse())
                                {
                                    var xpr = response.StatusCode;
                                }
                                //Stream ftpStream = response.GetResponseStream();
                                //ftpStream.Close();
                                // response.Close();
                                bandera = true;
                            }
                            catch (WebException ex)
                            {
                                FtpWebResponse response = (FtpWebResponse)ex.Response;
                                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                                {
                                    response.Close();
                                    bandera = true;
                                }
                                else
                                {
                                    response.Close();
                                    bandera = false;
                                }
                            }

                            //Evaluar que se creo el directorio
                            if (bandera)
                            {
                                int indexlabel = 0;
                                foreach (HttpPostedFileBase file in file_sopAnexar)
                                {
                                    var descripcion = "";
                                    try
                                    {
                                        listaDescArchivos.Add(labels_desc[indexlabel]);
                                    }
                                    catch (Exception ex)
                                    {
                                        descripcion = "";
                                        listaDescArchivos.Add(descripcion);
                                    }
                                    try
                                    {
                                        listaNombreArchivos.Add(file.FileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        listaDescArchivos.Add("");
                                    }
                                    string errorfiles = "";
                                    if (file != null)
                                    {
                                        if (file.ContentLength > 0)
                                        {
                                            string path = "";
                                            string filename = file.FileName;
                                            errorfiles = "";
                                            //res = SaveFile(file, url);
                                            res = SaveFile(file, doc.NUM_DOC);
                                            listaDirectorios.Add(res);
                                        }
                                    }
                                    indexlabel++;
                                    if (errorfiles != "")
                                    {
                                        errorMessage += "Error con el archivo " + errorfiles;
                                    }
                                }
                            }
                            else
                            {
                                // errorMessage = dir;
                            }

                            errorString = errorMessage;
                            //Guardar número de documento creado
                            Session["ERROR_FILES"] = errorMessage;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    //Lej26.09.2018------

                    //Lej-02.10.2018------
                    //DOCUMENTOA
                    //Misma cantidad de archivos y nombres, osea todo bien
                    if (listaDirectorios.Count == listaDescArchivos.Count && listaDirectorios.Count == listaNombreArchivos.Count)
                    {
                        for (int i = 0; i < doc.Anexo.Count; i++)
                        {
                            var pos = 1;
                            DOCUMENTOA _dA = new DOCUMENTOA();
                            if (doc.Anexo[i].a1 != 0)
                            {
                                _dA.NUM_DOC = doc.NUM_DOC;
                                _dA.POSD = i + 1;
                                _dA.POS = pos;
                                //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                if (doc.Anexo[i].a1 > 0 && doc.Anexo[i].a1 <= listaNombreArchivos.Count)
                                {
                                    var a1 = doc.Anexo[i].a1;
                                    try
                                    {
                                        var de = Path.GetExtension(listaNombreArchivos[a1 - 1]);
                                        _dA.TIPO = de.Replace(".", "");
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.TIPO = "";
                                    }
                                    try
                                    {
                                        _dA.DESC = listaDescArchivos[a1 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.DESC = "";
                                    }
                                    try
                                    {
                                        _dA.PATH = listaDirectorios[a1 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.PATH = "";
                                    }
                                }
                                else
                                {
                                    _dA.TIPO = "";
                                    _dA.DESC = "";
                                    _dA.PATH = "";
                                }
                                _dA.CLASE = "OTR";
                                _dA.STEP_WF = 1;
                                _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                _dA.ACTIVO = true;
                                try
                                {
                                    db.DOCUMENTOAs.Add(_dA);
                                    db.SaveChanges();
                                    pos++;
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }
                            _dA = new DOCUMENTOA();
                            if (doc.Anexo[i].a2 != 0)
                            {
                                _dA.NUM_DOC = doc.NUM_DOC;
                                _dA.POSD = i + 1;
                                _dA.POS = pos;
                                //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                if (doc.Anexo[i].a2 > 0 && doc.Anexo[i].a2 <= listaNombreArchivos.Count)
                                {
                                    var a2 = doc.Anexo[i].a2;
                                    try
                                    {
                                        var de = Path.GetExtension(listaNombreArchivos[a2 - 1]);
                                        _dA.TIPO = de.Replace(".", "");
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.TIPO = "";
                                    }
                                    try
                                    {
                                        _dA.DESC = listaDescArchivos[a2 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.DESC = "";
                                    }
                                    try
                                    {
                                        _dA.PATH = listaDirectorios[a2 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.PATH = "";
                                    }
                                }
                                else
                                {
                                    _dA.TIPO = "";
                                    _dA.DESC = "";
                                    _dA.PATH = "";
                                }
                                _dA.CLASE = "OTR";
                                _dA.STEP_WF = 1;
                                _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                _dA.ACTIVO = true;
                                try
                                {
                                    db.DOCUMENTOAs.Add(_dA);
                                    db.SaveChanges();
                                    pos++;
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }
                            _dA = new DOCUMENTOA();
                            if (doc.Anexo[i].a3 != 0)
                            {
                                _dA.NUM_DOC = doc.NUM_DOC;
                                _dA.POSD = i + 1;
                                _dA.POS = pos;
                                //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                if (doc.Anexo[i].a3 > 0 && doc.Anexo[i].a3 <= listaNombreArchivos.Count)
                                {
                                    var a3 = doc.Anexo[i].a3;
                                    try
                                    {
                                        var de = Path.GetExtension(listaNombreArchivos[a3 - 1]);
                                        _dA.TIPO = de.Replace(".", "");
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.TIPO = "";
                                    }
                                    try
                                    {
                                        _dA.DESC = listaDescArchivos[a3 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.DESC = "";
                                    }
                                    try
                                    {
                                        _dA.PATH = listaDirectorios[a3 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.PATH = "";
                                    }
                                }
                                else
                                {
                                    _dA.TIPO = "";
                                    _dA.DESC = "";
                                    _dA.PATH = "";
                                }
                                _dA.CLASE = "OTR";
                                _dA.STEP_WF = 1;
                                _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                _dA.ACTIVO = true;
                                try
                                {
                                    db.DOCUMENTOAs.Add(_dA);
                                    db.SaveChanges();
                                    pos++;
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }
                            _dA = new DOCUMENTOA();
                            if (doc.Anexo[i].a4 != 0)
                            {
                                _dA.NUM_DOC = doc.NUM_DOC;
                                _dA.POSD = i + 1;
                                _dA.POS = pos;
                                //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                if (doc.Anexo[i].a4 > 0 && doc.Anexo[i].a4 <= listaNombreArchivos.Count)
                                {
                                    var a4 = doc.Anexo[i].a4;
                                    try
                                    {
                                        var de = Path.GetExtension(listaNombreArchivos[a4 - 1]);
                                        _dA.TIPO = de.Replace(".", "");
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.TIPO = "";
                                    }
                                    try
                                    {
                                        _dA.DESC = listaDescArchivos[a4 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.DESC = "";
                                    }
                                    try
                                    {
                                        _dA.PATH = listaDirectorios[a4 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.PATH = "";
                                    }
                                }
                                else
                                {
                                    _dA.TIPO = "";
                                    _dA.DESC = "";
                                    _dA.PATH = "";
                                }
                                _dA.CLASE = "OTR";
                                _dA.STEP_WF = 1;
                                _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                _dA.ACTIVO = true;
                                try
                                {
                                    db.DOCUMENTOAs.Add(_dA);
                                    db.SaveChanges();
                                    pos++;
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }
                            _dA = new DOCUMENTOA();
                            if (doc.Anexo[i].a5 != 0)
                            {
                                _dA.NUM_DOC = doc.NUM_DOC;
                                _dA.POSD = i + 1;
                                _dA.POS = pos;
                                //Compruebo que el numero este dentro de los rangos de anexos MAXIMO 5
                                if (doc.Anexo[i].a5 > 0 && doc.Anexo[i].a5 <= listaNombreArchivos.Count)
                                {
                                    var a5 = doc.Anexo[i].a5;
                                    try
                                    {
                                        var de = Path.GetExtension(listaNombreArchivos[a5 - 1]);
                                        _dA.TIPO = de.Replace(".", "");
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.TIPO = "";
                                    }
                                    try
                                    {
                                        _dA.DESC = listaDescArchivos[a5 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.DESC = "";
                                    }
                                    try
                                    {
                                        _dA.PATH = listaDirectorios[a5 - 1];
                                    }
                                    catch (Exception c)
                                    {
                                        _dA.PATH = "";
                                    }
                                }
                                else
                                {
                                    _dA.TIPO = "";
                                    _dA.DESC = "";
                                    _dA.PATH = "";
                                }
                                _dA.CLASE = "OTR";
                                _dA.STEP_WF = 1;
                                _dA.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                _dA.ACTIVO = true;
                                try
                                {
                                    db.DOCUMENTOAs.Add(_dA);
                                    db.SaveChanges();
                                    pos++;
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }
                        }
                    }
                    //Lej-02.10.2018------

                }
                catch (Exception e)
                {
                    errorString += e.Message.ToString();
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        // GET: Solicitudes/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }
            return View(dOCUMENTO);
        }

        // POST: Solicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            db.DOCUMENTOes.Remove(dOCUMENTO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult getPartialCon(List<DOCUMENTOP_MOD> docs)
        {
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();

            doc.DOCUMENTOP = docs;
            return PartialView("~/Views/Solicitudes/_PartialConTr.cshtml", doc);
        }

        [HttpPost]
        public ActionResult getPartialCon2(List<DOCUMENTORP_MOD> docs)
        {
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();
            doc.DOCUMENTORP = docs;
            return PartialView("~/Views/Solicitudes/_PartialConTr2.cshtml", doc);
        }

        [HttpPost]
        public ActionResult getPartialCon3(List<Anexo> docs)
        {
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();
            doc.Anexo = docs;
            return PartialView("~/Views/Solicitudes/_PartialConTr3.cshtml", doc);
        }

        [HttpPost]
        public ActionResult getPartialCon4(List<DOCUMENTOA_TAB> docs, decimal nd)
        {
            var path = db.DOCUMENTOAs.Where(x => x.NUM_DOC == nd).ToList();
            //var xx = ConfigurationManager.AppSettings["URL_ATT"].ToString() + @"att";
            var xx = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_ATT") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString() + @"att";

            if (path.Count > 0)
            {
                for (int i = 0; i < docs.Count; i++)
                {
                    docs[i].PATH = xx + "\\" + nd + "\\" + docs[i].PATH;
                }
            }
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();
            doc.DOCUMENTOA_TAB = docs;
            return PartialView("~/Views/Solicitudes/_PartialConTr4.cshtml", doc);
        }

        [HttpPost]
        public ActionResult getPartialRet(List<DOCUMENTOR_MOD> docs)
        {
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();
            var nd = docs[0].BIMPONIBLE;
            doc.DOCUMENTOR = docs;
            return PartialView("~/Views/Solicitudes/_PartialRetTr.cshtml", doc);
        }

        [HttpPost]
        public JsonResult getRetenciones(List<DOCUMENTOR_MOD> items, string bukrs, string lifnr)
        {

            List<DOCUMENTOR_MOD> retlt = new List<DOCUMENTOR_MOD>();
            //Obtener las retenciones a partir del proveedor y sociedad
            //var retl = db.RETENCIONs.Where(rt => rt.ESTATUS == true)
            //    .Join(
            //    db.RETENCION_PROV.Where(rtp => rtp.LIFNR == lifnr && rtp.BUKRS == bukrs),
            //    ret => ret.WITHT,
            //    retp => retp.WITHT,
            //    (ret, retp) => new
            //    {
            //        LIFNR = retp.LIFNR,
            //        BUKRS = retp.BUKRS,
            //        WITHT = retp.WITHT,
            //        DESC = ret.DESCRIPCION,
            //        WT_WITHCD = retp.WT_WITHCD

            //    }).ToList();
            List<listRet> lstret = new List<listRet>();
            var _retl = db.RETENCION_PROV.Where(rtp => rtp.LIFNR == lifnr && rtp.BUKRS == bukrs).ToList();
            var _retl2 = db.RETENCIONs.Where(rtp => rtp.ESTATUS == true).ToList();
            for (int x = 0; x < _retl.Count; x++)
            {
                listRet _l = new listRet();
                var rttt = _retl2.Where(o => o.WITHT == _retl[x].WITHT && o.WT_WITHCD == _retl[x].WT_WITHCD).FirstOrDefault();
                _l.LIFNR = _retl[x].LIFNR;
                _l.BUKRS = _retl[x].BUKRS;
                _l.WITHT = rttt.WITHT;
                _l.DESC = rttt.DESCRIPCION;
                _l.WT_WITHCD = rttt.WT_WITHCD;

                lstret.Add(_l);
            }
            var retl = lstret;
            if (retl != null && retl.Count > 0)
            {
                //Obtener los textos de las retenciones
                retlt = (from r in retl
                         join rt in db.RETENCIONTs
                         on new { A = r.WITHT, B = r.WT_WITHCD } equals new { A = rt.WITHT, B = rt.WT_WITHCD }
                         into jj
                         from rt in jj.DefaultIfEmpty()
                         where rt.SPRAS_ID.Equals("ES")
                         select new DOCUMENTOR_MOD
                         {
                             LIFNR = r.LIFNR,
                             BUKRS = r.BUKRS,
                             WITHT = r.WITHT,
                             WT_WITHCD = r.WT_WITHCD,
                             DESC = rt.TXT50 == null ? String.Empty : r.DESC,

                         }).ToList();
            }


            foreach (DOCUMENTOR_MOD it in retlt)
            {
                //Buscar si los registros que se obtuvieron, están en la lista de la vista
                DOCUMENTOR_MOD doco = new DOCUMENTOR_MOD();
                try
                {
                    doco = items.Where(m => m.LIFNR == it.LIFNR && m.BUKRS == it.BUKRS && m.WITHT == it.WITHT && m.WT_WITHCD == it.WT_WITHCD).FirstOrDefault();
                }
                catch (Exception e)
                {

                }

                if (doco != null)
                {
                    it.IMPORTE_RET = doco.IMPORTE_RET;
                    it.BIMPONIBLE = doco.BIMPONIBLE;
                }
            }

            JsonResult jc = Json(retlt, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]
        public JsonResult getConcepto(string id, string tipo, string bukrs)
        {

            //Obtener el concepto
            CONCEPTO c = new CONCEPTO();

            c = db.CONCEPTOes.Where(co => co.ID_CONCEPTO == id && co.TIPO_CONCEPTO == tipo).FirstOrDefault();

            JsonResult jc = Json(c, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]
        public JsonResult getPercentage(string witht, string ir)
        {
            //Obtener el concepto
            decimal? porc = 0; //MGC 16-10-2018 Convertir a decimal

            var ret = db.RETENCIONs.Where(co => co.WITHT == witht && co.WT_WITHCD == ir).FirstOrDefault();//lejgg 03-11-18 Convertir a decimal
            if (ret.CAMPO.Trim() == "MONTO")
            {
                porc = ret.PORC;
            }
            if (ret.CAMPO.Trim() == "IVA")
            {
                porc = ret.PORC;
            }
            JsonResult jc = Json(porc, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]//LEJGG 03-11-18
        public JsonResult getCampoMult(string witht, string ir)
        {
            //Obtener el concepto           
            var ret = db.RETENCIONs.Where(co => co.WITHT == witht && co.WT_WITHCD == ir).FirstOrDefault();//lejgg 03-11-18 Convertir a decimal
            JsonResult jc = Json(ret.CAMPO.Trim(), JsonRequestBehavior.AllowGet);
            return jc;
        }

        //------------------------------------------------------------------------------->
        //Lejggg 22-10-2018
        [HttpPost]
        public JsonResult procesarXML(IEnumerable<HttpPostedFileBase> file)
        {
            try
            {

                //FRT07112018.2  Lectura de XML
                var _ff = file.ToList();
                var lines = ReadLines(() => _ff[0].InputStream, Encoding.UTF8).ToArray();
               
                    var _lin = lines.Count();
                    string _soc = (string)Session["SOC"];

                    string _rfc_soc = db.SOCIEDADs.Where(soc => soc.BUKRS == _soc).FirstOrDefault().STCD1;
                    var _xml = "";
                    //Proceso a realizar si el xml viene estructurado en varios renglones, se concatena en 1 solo //FRT Y LEJGG 07-11-18
                    for (int i = 0; i < _lin; i++)
                    {
                        _xml += lines[i];
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_xml);

                    var xmlnode = doc.GetElementsByTagName("cfdi:Comprobante");
                    var xmlnode2 = doc.GetElementsByTagName("cfdi:Receptor");
                    var xmlnode3 = doc.GetElementsByTagName("cfdi:Emisor");
                    var xmlnode4 = doc.GetElementsByTagName("tfd:TimbreFiscalDigital");

                    var _F = DateTime.Parse(xmlnode[0].Attributes["Fecha"].Value).ToShortDateString();
                    var _Mt = xmlnode[0].Attributes["Total"].Value;
                    var _RFCReceptor = xmlnode2[0].Attributes["Rfc"].Value;
                    var _RFCEmisor = xmlnode3[0].Attributes["Rfc"].Value;
                    var _Uuid = xmlnode4[0].Attributes["UUID"].Value;
                    var _Moneda = xmlnode[0].Attributes["Moneda"].Value;   //FRT14112018.3 para agregar la moneda al JSON
                    var _TipoCambio = "0";
                    if (_Moneda != "MXN")
                    {
                        _TipoCambio = xmlnode[0].Attributes["TipoCambio"].Value;  //FRT14112018.3 para agregar la TIPO CAMBIO al JSON
                    }


                    var _xmlcorrecto = "1";  //FRT20112018 Para poder saber si el XML esta correcto

                    List<string> lstvals = new List<string>();
                    lstvals.Add(_xmlcorrecto);//FRT20112018 Xml correcto en primera posicion
                    lstvals.Add(_F);//Fecha
                    lstvals.Add(_Mt);//Monto
                    lstvals.Add(_RFCReceptor);//RFCReceptor
                    lstvals.Add(_RFCEmisor);//RFCEmisor
                    lstvals.Add(_Uuid);//UUID
                    lstvals.Add(_Moneda);//Moneda //FRT14112018.3 para agregar la moneda al JSON
                    lstvals.Add(_rfc_soc);//Sociedad
                    lstvals.Add(_TipoCambio);//Tipo de Cambio //FRT14112018.3 para agregar tipo de cambio  al JSON
                    JsonResult jc = Json(lstvals, JsonRequestBehavior.AllowGet);
                    return jc;

              
               
            }
            catch (Exception e)
            {
                List<string> lstvals_error = new List<string>();
                lstvals_error.Add("0");//FRT20112018Xml error en primera posicion
                JsonResult jc = Json(lstvals_error, JsonRequestBehavior.AllowGet); //FRT20112018Xml error en primera posicion
                                                                                   //JsonResult jc = Json("", JsonRequestBehavior.AllowGet); //FRT20112018 Se comenta para poder tener los valores
                return jc;
            }
        }

        public IEnumerable<string> ReadLines(Func<Stream> streamProvider,
                                     Encoding encoding)
        {
            using (var stream = streamProvider())
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
        //-------------------------------------------------------------------------------<

        [HttpPost]
        public JsonResult getConceptoI(string Prefix, string bukrs)
        {

            if (Prefix == null)
                Prefix = "";

            WFARTHAEntities db = new WFARTHAEntities();

            ////FRT02122018 pARA VALIDAR MAYUSCULAS MINUSCULAS
            //Prefix = Prefix.ToUpper();
            ////ENDFRT02122018

            //Obtener el tipo de operación de usuario
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).Include(usr => usr.DET_TIPOPRESUPUESTO).FirstOrDefault();

            DET_TIPOPRESUPUESTO tp = new DET_TIPOPRESUPUESTO();

            //Veridficar si hay tipo presupuesto
            if (user != null && user.DET_TIPOPRESUPUESTO.Count > 0)
            {
                try
                {
                    tp = user.DET_TIPOPRESUPUESTO.Where(t => t.BUKRS == bukrs).FirstOrDefault();
                }
                catch (Exception e)
                {

                }
            }
            var lcont = (dynamic)null;

            //Tiene un tipo de presupuesto
            if (tp != null && tp.TIPOPRE != "" && tp.TIPOPRE != null)
            {
                var _s = Session["id_pr"].ToString();
                if (tp.TIPOPRE != "*")
                {
                    //LEJ 09102018------------------------------------------------->
                    int _n;
                    bool is_Numeric = int.TryParse(Prefix, out _n);
                    //LEJ 09102018-------------------------------------------------<

                    //MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k------------------------------------------------> 

                    List<CONCEPTO> listcon = new List<CONCEPTO>();

                    listcon = (from f in db.DET_K.Where(k => k.BUKRS == bukrs).ToList()
                               join c in db.CONCEPTOes.Where(co => co.TIPO_PRESUPUESTO == tp.TIPOPRE && co.TIPO_IMPUTACION == "K").ToList()
                               on f.CUENTA equals c.CUENTA
                               select new CONCEPTO
                               {
                                   ID_CONCEPTO = c.ID_CONCEPTO,
                                   DESC_CONCEPTO = c.DESC_CONCEPTO,
                                   TIPO_CONCEPTO = c.TIPO_CONCEPTO,
                                   TIPO_IMPUTACION = c.TIPO_IMPUTACION,
                                   ID_PRESUPUESTO = c.ID_PRESUPUESTO,
                                   TIPO_PRESUPUESTO = c.TIPO_PRESUPUESTO,
                                   CUENTA = c.CUENTA
                               }).ToList();

                    //MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k------------------------------------------------<


                    ////Obtener todos los elementos k-------------------------------------------------------------------------------------

                    //var lconk = (from co in db.CONCEPTOes//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                    var lconk = (from co in listcon//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                                                   //where co.TIPO_CONCEPTO.Contains(Prefix.ToUpper()) && co.TIPO_PRESUPUESTO == tp.TIPOPRE && co.TIPO_IMPUTACION == "K" //FRT04122018
                                 where co.TIPO_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_PRESUPUESTO == tp.TIPOPRE && co.TIPO_IMPUTACION == "K"
                                 select new
                                 {
                                     ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                     TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                     DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                 }).ToList();


                    if (lconk.Count == 0)
                    {
                        //lconk = (from co in db.CONCEPTOes//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                        lconk = (from co in listcon//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                                                   //where co.ID_CONCEPTO.Contains(Prefix) && co.TIPO_PRESUPUESTO == tp.TIPOPRE && co.TIPO_IMPUTACION == "K"//FRT04122018
                                 where co.ID_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_PRESUPUESTO == tp.TIPOPRE && co.TIPO_IMPUTACION == "K"
                                 select new
                                 {
                                     ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                     TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                     DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                 }).ToList();
                    }
                    if (lconk.Count == 0)
                    {
                        //var lconk2 = (from co in db.CONCEPTOes.ToList()//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                        var lconk2 = (from co in listcon//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                                                        //where co.DESC_CONCEPTO.Contains(Prefix) && co.TIPO_PRESUPUESTO == tp.TIPOPRE && co.TIPO_IMPUTACION == "K"//FRT04122018
                                      where co.DESC_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_PRESUPUESTO == tp.TIPOPRE && co.TIPO_IMPUTACION == "K"
                                      select new
                                      {
                                          ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                          TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                          DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                      }).ToList();

                        lconk.AddRange(lconk2);
                    }
                    try
                    {
                        lcont.AddRange(lconk);
                    }
                    catch (Exception c)
                    {//
                    }

                    //Obtener todos los elementos k-------------------------------------------------------------------------------------

                    //Obtener los elementos asignados al proyecto (_s)
                    List<DET_PEP> detpep = new List<DET_PEP>();
                    detpep = db.DET_PEP.Where(dp => dp.PSPNR == _s).ToList();

                    var lcon = (from co in db.CONCEPTOes.ToList()
                                join dp in detpep
                                on new { A = co.ID_CONCEPTO, B = co.TIPO_CONCEPTO } equals new { A = dp.CONCEPTO, B = dp.TIPO_CONCEPTO } //
                                //where co.ID_CONCEPTO.Contains(Prefix) && co.TIPO_PRESUPUESTO == tp.TIPOPRE//FRT04122018
                                where co.ID_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_PRESUPUESTO == tp.TIPOPRE
                                select new
                                {
                                    ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                    TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                    DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                }).ToList();
                    //LEJ 09102018------------------------------------------------->
                    if (!is_Numeric)
                    {
                        // BUSQUEDA POR TIPO_CONCEPTO
                        if (lcon.Count == 0)
                        {
                            lcon = (from co in db.CONCEPTOes.ToList()
                                    join dp in detpep
                                         on new { A = co.ID_CONCEPTO, B = co.TIPO_CONCEPTO } equals new { A = dp.CONCEPTO, B = dp.TIPO_CONCEPTO } //
                                                                                                                                                  //where co.TIPO_CONCEPTO.Contains(Prefix.ToUpper()) && co.TIPO_PRESUPUESTO == tp.TIPOPRE//FRT04122018
                                    where co.TIPO_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_PRESUPUESTO == tp.TIPOPRE
                                    select new
                                    {
                                        ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                        TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                        DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                    }).ToList();
                            // lcon = _lcon.Where(x=>x.TIPO_CONCEPTO==Prefix.ToUpper()).ToList();
                        }
                    }
                    //LEJ 09102018-------------------------------------------------<
                    if (lcon.Count == 0)
                    {
                        var lcon2 = (from co in db.CONCEPTOes.ToList()
                                     join dp in detpep
                                     on new { A = co.ID_CONCEPTO, B = co.TIPO_CONCEPTO } equals new { A = dp.CONCEPTO, B = dp.TIPO_CONCEPTO } //
                                     //where co.DESC_CONCEPTO.Contains(Prefix) && co.TIPO_PRESUPUESTO == tp.TIPOPRE//FRT04122018
                                     where co.DESC_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_PRESUPUESTO == tp.TIPOPRE
                                     select new
                                     {
                                         ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                         TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                         DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                     }).ToList();

                        lcon.AddRange(lcon2);
                    }

                    lcont = lcon;
                }
                else
                {
                    //LEJ 09102018------------------------------------------------->
                    int _n;
                    bool is_Numeric = int.TryParse(Prefix, out _n);
                    //LEJ 09102018-------------------------------------------------<
                    var lcont2 = (dynamic)null;

                    //MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k------------------------------------------------> 

                    List<CONCEPTO> listcon = new List<CONCEPTO>();

                    listcon = (from f in db.DET_K.Where(k => k.BUKRS == bukrs).ToList()
                               join c in db.CONCEPTOes.Where(co => co.TIPO_IMPUTACION == "K").ToList()
                               on f.CUENTA equals c.CUENTA
                               select new CONCEPTO
                               {
                                   ID_CONCEPTO = c.ID_CONCEPTO,
                                   DESC_CONCEPTO = c.DESC_CONCEPTO,
                                   TIPO_CONCEPTO = c.TIPO_CONCEPTO,
                                   TIPO_IMPUTACION = c.TIPO_IMPUTACION,
                                   ID_PRESUPUESTO = c.ID_PRESUPUESTO,
                                   TIPO_PRESUPUESTO = c.TIPO_PRESUPUESTO,
                                   CUENTA = c.CUENTA
                               }).ToList();

                    //MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k------------------------------------------------<


                    //Obtener todos los elementos k-------------------------------------------------------------------------------------
                    //var lconk = (from co in db.CONCEPTOes//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                    var lconk = (from co in listcon//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                                                   //where co.TIPO_CONCEPTO.Contains(Prefix.ToUpper()) && co.TIPO_IMPUTACION == "K"//FRT04122018
                                 where co.TIPO_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_IMPUTACION == "K"
                                 select new
                                 {
                                     ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                     TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                     DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                 }).ToList();

                    //LEJ 09102018------------------------------------------------->
                    // BUSQUEDA POR id_CONCEPTO
                    if (lconk.Count == 0)
                    {
                        //lconk = (from co in db.CONCEPTOes //MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                        lconk = (from co in listcon//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                                                   //where co.ID_CONCEPTO.Contains(Prefix) && co.TIPO_IMPUTACION == "K"//FRT04122018
                                 where co.ID_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_IMPUTACION == "K"
                                 select new
                                 {
                                     ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                     TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                     DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                 }).ToList();
                    }
                    //LEJ 09102018-------------------------------------------------<
                    if (lconk.Count == 0)
                    {
                        //var lconk2 = (from co in db.CONCEPTOes//MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                        var lconk2 = (from co in listcon //MGC 30-10-2018 Obtener los elementos k filtrados en la tabla det_k
                                                         //where co.DESC_CONCEPTO.Contains(Prefix) && co.TIPO_IMPUTACION == "K"//FRT04122018
                                      where co.DESC_CONCEPTO.ToLower().Contains(Prefix.ToLower()) && co.TIPO_IMPUTACION == "K"
                                      select new
                                      {
                                          ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                          TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                          DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                      }).ToList();

                        lconk.AddRange(lconk2);
                    }

                    //Obtener todos los elementos k-------------------------------------------------------------------------------------

                    //Obtener los elementos asignados al proyecto (_s)
                    List<DET_PEP> detpep = new List<DET_PEP>();
                    detpep = db.DET_PEP.Where(dp => dp.PSPNR == _s).ToList();


                    // BUSQUEDA POR ID_CONCEPTO
                    var lcon = (from co in db.CONCEPTOes.ToList()
                                join dp in detpep
                                     on new { A = co.ID_CONCEPTO, B = co.TIPO_CONCEPTO } equals new { A = dp.CONCEPTO, B = dp.TIPO_CONCEPTO } //
                                                                                                                                              //where co.ID_CONCEPTO.Contains(Prefix) //FRT04122018
                                where co.ID_CONCEPTO.ToLower().Contains(Prefix.ToLower())
                                select new
                                {
                                    ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                    TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                    DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                }).ToList();
                    //LEJ 09102018------------------------------------------------->
                    if (!is_Numeric)
                    {
                        // BUSQUEDA POR TIPO_CONCEPTO
                        if (lcon.Count == 0)
                        {
                            lcon = (from co in db.CONCEPTOes.ToList()
                                    join dp in detpep
                                         on new { A = co.ID_CONCEPTO, B = co.TIPO_CONCEPTO } equals new { A = dp.CONCEPTO, B = dp.TIPO_CONCEPTO } //
                                                                                                                                                  //where co.TIPO_CONCEPTO.Contains(Prefix.ToUpper()) //FRT04122018
                                    where co.TIPO_CONCEPTO.ToLower().Contains(Prefix.ToLower())
                                    select new
                                    {
                                        ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                        TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                        DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                    }).ToList();
                            // lcon = _lcon.Where(x=>x.TIPO_CONCEPTO==Prefix.ToUpper()).ToList();
                        }
                    }
                    //LEJ 09102018-------------------------------------------------<
                    //sI EN LA BUSQUEDA POR ID_CONCEPTO Y POR TIPO_CONCEPTO ESTA VACIO BUSCARA POR DESC_CONCEPTO
                    if (lcon.Count == 0)
                    {
                        var lcon2 = (from co in db.CONCEPTOes.ToList()
                                     join dp in detpep
                                     on new { A = co.ID_CONCEPTO, B = co.TIPO_CONCEPTO } equals new { A = dp.CONCEPTO, B = dp.TIPO_CONCEPTO } //
                                     //where co.DESC_CONCEPTO.Contains(Prefix) //FRT04122018
                                     where co.DESC_CONCEPTO.ToLower().Contains(Prefix.ToLower())
                                     select new
                                     {
                                         ID_CONCEPTO = co.ID_CONCEPTO.ToString(),
                                         TIPO_CONCEPTO = co.TIPO_CONCEPTO.ToString(),
                                         DESC_CONCEPTO = co.DESC_CONCEPTO.ToString()
                                     }).ToList();

                        lcon.AddRange(lcon2);
                    }
                    lcon.AddRange(lconk);
                    lcont = lcon;
                }


            }

            JsonResult cc = Json(lcont, JsonRequestBehavior.AllowGet);
            return cc;
        }



        //FRT 12112018 PARA ENVIAR LOS ARCHIVOS EN LA TABLA ANEXOS Y PODER HACER EL BARRIDO
        [HttpPost]
        public ActionResult getPartialCon41(List<DOCUMENTOA_TAB> docs)
        {
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();
            doc.DOCUMENTOA_TAB = docs;
            return PartialView("~/Views/Solicitudes/_PartialConTr4.cshtml", doc);
        }
        //END FRT 12112018 PARA ENVIAR LOS ARCHIVOS EN LA TABLA ANEXOS Y PODER HACER EL BARRIDO



        //FRT13112018 Para subir los temporales

        [HttpPost]
        public JsonResult subirTemporal(IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                Random random = new Random();
                if (Session["Temporal"] == null)
                {
                    Session["Temporal"] = random.Next(10000, 50000);
                }

                decimal carpetatemporal = Convert.ToDecimal(Session["Temporal"]);
                foreach (HttpPostedFileBase files in file)
                {
                    if (files.ContentLength > 0)
                    {
                        string res;
                        string path = "";
                        string filename = files.FileName;

                        //res = SaveFile(file, url);
                        res = SaveFile(files, carpetatemporal);

                    }
                }
                JsonResult jc = Json(true, JsonRequestBehavior.AllowGet);
                return jc;
            }
            catch
            {
                JsonResult jc = Json(false, JsonRequestBehavior.AllowGet);
                return jc;
            }

        }
        //END FRT13112018

        //FRT13112018 Para subir los temporales

        [HttpPost]
        public JsonResult subirTemporalEditar(IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                decimal carpetatemporal = Convert.ToDecimal(Session["NUM_DOC_TEM"]);
                foreach (HttpPostedFileBase files in file)
                {
                    if (files.ContentLength > 0)
                    {
                        string res;
                        string path = "";
                        string filename = files.FileName;

                        //res = SaveFile(file, url);
                        res = SaveFile(files, carpetatemporal);

                    }
                }
                JsonResult jc = Json(true, JsonRequestBehavior.AllowGet);
                return jc;
            }
            catch
            {
                JsonResult jc = Json(false, JsonRequestBehavior.AllowGet);
                return jc;
            }

        }
        //END FRT13112018

        [HttpPost]
        public JsonResult getCondicion(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";


            ////FRT02122018 pARA VALIDAR MAYUSCULAS MINUSCULAS
            //Prefix = Prefix.ToUpper();
            ////ENDFRT02122018

            WFARTHAEntities db = new WFARTHAEntities();

            //var cond = from con in db.CONDICIONES_PAGO where con.COND_PAGO.Contains(Prefix) select new { COND_PAGO = con.COND_PAGO.ToString(), TEXT = con.TEXT.ToString() };//FRT04122018
            if (Prefix == " ")
            {
                var cond = from con in db.CONDICIONES_PAGO select new { COND_PAGO = con.COND_PAGO.ToString(), TEXT = con.TEXT.ToString() };
                JsonResult cc = Json(cond, JsonRequestBehavior.AllowGet);
                return cc;
            }
            else
            {
                var cond = from con in db.CONDICIONES_PAGO where con.COND_PAGO.ToLower().Contains(Prefix.ToLower()) select new { COND_PAGO = con.COND_PAGO.ToString(), TEXT = con.TEXT.ToString() };
                JsonResult cc = Json(cond, JsonRequestBehavior.AllowGet);
                return cc;
            }

        }

        [HttpPost]//LEJGG 06-11-18
        public JsonResult getCondicionEdit(string id)
        {

            WFARTHAEntities db = new WFARTHAEntities();
            var texto = db.CONDICIONES_PAGO.Where(x => x.COND_PAGO == id).FirstOrDefault().TEXT;

            JsonResult cc = Json(texto, JsonRequestBehavior.AllowGet);
            return cc;
        }


        [HttpPost]//FRT27112018
        public JsonResult deleteUuid(decimal num_doc)
        {

            var st = false;

            string displayName = null;
            var keyValue = db.DOCUMENTOUUIDs.FirstOrDefault(a => a.NUM_DOC == num_doc && a.ESTATUS == true);
            if (keyValue != null)
            {

                var _uuid2 = db.DOCUMENTOUUIDs.Where(u => u.NUM_DOC == num_doc && u.ESTATUS == true).FirstOrDefault();
                DOCUMENTOUUID duid = _uuid2;
                duid.NUM_DOC = num_doc;
                duid.POS = _uuid2.POS;
                duid.DOCUMENTO_SAP = "";
                duid.UUID = _uuid2.UUID;
                duid.ESTATUS = false;
                db.Entry(duid).State = EntityState.Modified;
                db.SaveChanges();
                st = true;
            }

            JsonResult jc = Json(st, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]
        public string getCondicionT(string cond)
        {
            if (cond == null)
                cond = "";

            WFARTHAEntities db = new WFARTHAEntities();
            var text = "";
            text = db.CONDICIONES_PAGO.Where(con => con.COND_PAGO == cond).FirstOrDefault().TEXT.ToString();

            return text;
        }

        //FRT14112018.3 funcion para traer de la base de datos el tipo de cambio
        [HttpPost]
        public JsonResult getTipoCambio(string tcurr, DateTime gdatu)//MGC 19-10-2018 Condiciones
        {
            var _bol = false;
            decimal? _tipocambio = 0;
            var dia = 0;

            if (tcurr == null)
                tcurr = "";

            WFARTHAEntities db = new WFARTHAEntities();

            while (true)
            {
                DateTime fecha = gdatu.AddDays(-dia);

                string displayName = null;
                var keyValue = db.TCAMBIOs.FirstOrDefault(a => a.TCURR == tcurr & a.GDATU == fecha);
                if (keyValue != null)
                {
                    var lprov = db.TCAMBIOs.Where(a => a.TCURR == tcurr & a.GDATU == fecha).First().UKURS;
                    _tipocambio = lprov;
                    break;
                }
                dia++;
            }
            JsonResult cc = Json(_tipocambio, JsonRequestBehavior.AllowGet);
            return cc;

        }
        //FRT14112018.3 funcion para traer de la base de datos el tipo de cambio

        [HttpPost]
        public JsonResult getProveedor(string Prefix, string bukrs)
        {

            if (Prefix == null)
                Prefix = "";

            WFARTHAEntities db = new WFARTHAEntities();

            //var c = (from m in db.MAT
            //         where m.ID.Contains(Prefix) && m.ACTIVO == true
            //         select new { m.ID, m.MAKTX }).ToList();
            //if (c.Count == 0)
            //{
            //    var c2 = (from m in db.MATERIALs
            //              where m.MAKTX.Contains(Prefix) && m.ACTIVO == true
            //              select new { m.ID, m.MAKTX }).ToList();
            //    c.AddRange(c2);

            //SOCIEDAD c = db.SOCIEDADs.Where(soc => soc.BUKRS == bukrs).Include(s => s.PROVEEDORs).FirstOrDefault();//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad

            //FRT02122018 pARA VALIDAR MAYUSCULAS MINUSCULAS
            //Prefix = Prefix.ToUpper();
            //ENDFRT02122018


            var pp = db.DET_PROVEEDOR.Where(soc => soc.ID_BUKRS == bukrs).Select(pp1 => new { pp1.ID_LIFNR }).Distinct().ToList();//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad

            //List<PROVEEDOR> prov = new List<PROVEEDOR>();//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad

            var prov = (from ppl in pp.ToList()
                        join pr in db.PROVEEDORs
                        on ppl.ID_LIFNR equals pr.LIFNR
                        select new
                        {
                            LIFNR = pr.LIFNR.ToString(),
                            NAME1 = pr.NAME1.ToString()
                        }).ToList();

            //List<PROVEEDOR> lprov = new List<PROVEEDOR>();
            //List<PROVEEDOR> lprov2 = new List<PROVEEDOR>();

            var r = (dynamic)null;

            if (pp != null)//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad
            {
                //var lprov = (from p in c.PROVEEDORs                
                var lprov = (from p in prov//MGC 25-10-2018 Modificación a la relación de proveedores-sociedad
                                           //where p.LIFNR.Contains(Prefix)//FRT04122018
                             where p.LIFNR.ToLower().Contains(Prefix.ToLower())
                             select new
                             {
                                 LIFNR = p.LIFNR.ToString(),
                                 NAME1 = p.NAME1.ToString()
                             }).ToList();

                if (lprov.Count == 0)
                {
                    //var lprov2 = (from p in c.PROVEEDORs
                    var lprov2 = (from p in prov //MGC 25-10-2018 Modificación a la relación de proveedores-sociedad
                                                 //where p.NAME1.Contains(Prefix) //FRT04122018
                                  where p.NAME1.ToLower().Contains(Prefix.ToLower())
                                  select new
                                  {
                                      LIFNR = p.LIFNR.ToString(),
                                      NAME1 = p.NAME1.ToString()
                                  }).ToList();

                    lprov.AddRange(lprov2);
                }

                r = lprov;
            }

            JsonResult cc = Json(r, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpPost]
        public JsonResult getProveedorD(string lifnr, string soc)//MGC 19-10-2018 Condiciones
        {

            if (lifnr == null)
                lifnr = "";

            WFARTHAEntities db = new WFARTHAEntities();

            //MGC 19-10-2018 Condiciones-->
            var lprov = (from pr in db.PROVEEDORs.Where(p => p.LIFNR == lifnr)
                         join rt in db.DET_PROVEEDORV.Where(dp => dp.ID_LIFNR == lifnr && dp.ID_BUKRS == soc)
                                  on pr.LIFNR equals rt.ID_LIFNR
                                  into jj
                         from rt in jj.DefaultIfEmpty()
                         select new
                         {
                             LIFNR = pr.LIFNR.ToString(),
                             NAME1 = pr.NAME1.ToString(),
                             STCD1 = pr.STCD1.ToString(),
                             COND_PAGO = rt.COND_PAGO.ToString() == null ? String.Empty : rt.COND_PAGO.ToString()
                         }).FirstOrDefault();

            //var lprov = db.PROVEEDORs.Where(p => p.LIFNR == lifnr).Select(pr => new { LIFNR = pr.LIFNR.ToString(), NAME1 = pr.NAME1.ToString(), STCD1 = pr.STCD1.ToString() }).FirstOrDefault();
            //MGC 19-10-2018 Condiciones--<
            JsonResult cc = Json(lprov, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpPost]//lej 11.09.2018   
        public JsonResult getImpDesc()
        {
            WFARTHAEntities db = new WFARTHAEntities();
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            TXTImp txt = new TXTImp();
            List<TXTImp> lsttxt = new List<TXTImp>();
            var impuestot = db.IMPUESTOTs.Where(x => x.SPRAS_ID == user.SPRAS_ID).ToList();
            for (int i = 0; i < impuestot.Count; i++)
            {
                txt = new TXTImp();
                txt.spras_id = impuestot[i].SPRAS_ID;
                txt.imp = impuestot[i].MWSKZ;
                txt.txt = impuestot[i].TXT50;
                lsttxt.Add(txt);
            }
            JsonResult cc = Json(lsttxt, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpPost]
        public JsonResult getRetLigadas(string id)
        {
            var ret = db.RETENCIONs.Where(x => x.WITHT == id).FirstOrDefault().WITHT_SUB;
            if (ret == null)
            {
                ret = "Null";
            }
            JsonResult jc = Json(ret, JsonRequestBehavior.AllowGet);
            return jc;
        }

        //FRT02122018 Para mensaje al cancelar

        [HttpPost]
        public ActionResult getIndex()
        {
            return RedirectToAction("Index", "Home");
        }
        //ENDFRT02122018 Para mensaje al candelar


        //FRT27112018
        [HttpPost]
        public JsonResult getUuid(string id)
        {
            var ret = db.DOCUMENTOUUIDs.Where(x => x.UUID == id && x.ESTATUS == true).FirstOrDefault();
            var st = "";
            //Significa que no hay coincidencia
            if (ret == null)
            {
                st = "Null";
            }
            else
            {
                st = "Match";
            }
            JsonResult jc = Json(st, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]//lej 16.10.2018   
        public JsonResult getDocsPr(decimal id)
        {
            var _Se = db.DOCUMENTORPs.Where(x => x.NUM_DOC == id).ToList();
            var rets = _Se.Select(w => w.WITHT).Distinct().ToList();
            var rets2 = rets;
            for (int i = 0; i < rets.Count; i++)
            {
                var _rt = rets2[i];
                var ret = db.RETENCIONs.Where(x => x.WITHT == _rt).FirstOrDefault().WITHT_SUB;
                for (int j = 0; j < rets.Count; j++)
                {
                    if (rets2[j] == ret)
                    {
                        rets2.RemoveAt(j);
                    }
                }
            }
            var _xdocsrp = db.DOCUMENTORPs.Where(x => x.NUM_DOC == id).ToList();
            List<DOCUMENTORP_MOD> _xdocsrp2 = new List<DOCUMENTORP_MOD>();
            DOCUMENTORP_MOD _Data = new DOCUMENTORP_MOD();
            for (int x = 0; x < rets2.Count; x++)
            {
                for (int j = 0; j < _xdocsrp.Count; j++)
                {
                    if (rets2[x] == _xdocsrp[j].WITHT)
                    {
                        _Data = new DOCUMENTORP_MOD();
                        _Data.NUM_DOC = _xdocsrp[j].NUM_DOC;
                        _Data.POS = _xdocsrp[j].POS;
                        _Data.WITHT = _xdocsrp[j].WITHT;
                        _Data.WT_WITHCD = _xdocsrp[j].WT_WITHCD;
                        _Data.BIMPONIBLE = _xdocsrp[j].BIMPONIBLE;
                        _Data.IMPORTE_RET = _xdocsrp[j].IMPORTE_RET;
                        _xdocsrp2.Add(_Data);
                    }
                }
            }
            JsonResult jc = Json(_xdocsrp2, JsonRequestBehavior.AllowGet);
            return jc;
        }

        // GET: Solicitudes/Edit/5
        public JsonResult traerColsExtras(decimal id)
        {
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            //Documento a documento mod //Copiar valores del post al nuevo objeto
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();

            List<DOCUMENTOR> retl = new List<DOCUMENTOR>();
            List<DOCUMENTOR_MOD> retlt = new List<DOCUMENTOR_MOD>();

            retl = db.DOCUMENTORs.Where(x => x.NUM_DOC == id).ToList();

            //retlt = (from r in retl
            //         join rt in db.RETENCIONTs
            //         on r.WITHT equals rt.WITHT
            //         into jj
            //         from rt in jj.DefaultIfEmpty()
            //         where rt.SPRAS_ID.Equals("ES")
            //         select new DOCUMENTOR_MOD
            //         {
            //             WITHT = r.WITHT,
            //             DESC = rt.TXT50 == null ? String.Empty : "",
            //             WT_WITHCD = r.WT_WITHCD,
            //             BIMPONIBLE = r.BIMPONIBLE,
            //             IMPORTE_RET = r.IMPORTE_RET

            //         }).ToList();

            //List<DOCUMENTOR_MOD> _relt = new List<DOCUMENTOR_MOD>();
            //var _retl = db.RETENCIONs.Where(rt => rt.ESTATUS == true)
            //    .Join(
            //    db.RETENCION_PROV.Where(rtp => rtp.LIFNR == dOCUMENTO.PAYER_ID && rtp.BUKRS == dOCUMENTO.SOCIEDAD_ID),
            //    ret => ret.WITHT,
            //    retp => retp.WITHT,
            //    (ret, retp) => new
            //    {
            //        LIFNR = retp.LIFNR,
            //        BUKRS = retp.BUKRS,
            //        WITHT = retp.WITHT,
            //        DESC = ret.DESCRIPCION,
            //        WT_WITHCD = retp.WT_WITHCD

            //    }).ToList();
            List<listRet> lstret = new List<listRet>();
            var lifnr = dOCUMENTO.PAYER_ID;
            var bukrs = dOCUMENTO.SOCIEDAD_ID;
            var _retl = db.RETENCION_PROV.Where(rtp => rtp.LIFNR == lifnr && rtp.BUKRS == bukrs).ToList();
            var _retl2 = db.RETENCIONs.Where(rtp => rtp.ESTATUS == true).ToList();
            for (int x = 0; x < _retl.Count; x++)
            {
                listRet _l = new listRet();
                var rttt = _retl2.Where(o => o.WITHT == _retl[x].WITHT && o.WT_WITHCD == _retl[x].WT_WITHCD).FirstOrDefault();
                _l.LIFNR = _retl[x].LIFNR;
                _l.BUKRS = _retl[x].BUKRS;
                _l.WITHT = rttt.WITHT;
                _l.DESC = rttt.DESCRIPCION;
                _l.WT_WITHCD = rttt.WT_WITHCD;

                lstret.Add(_l);
            }
            var _relt = lstret;
            if (_relt != null && _relt.Count > 0)
            {
                //Obtener los textos de las retenciones
                retlt = (from r in _relt
                         join rt in db.RETENCIONTs
                         on new { A = r.WITHT, B = r.WT_WITHCD } equals new { A = rt.WITHT, B = rt.WT_WITHCD }
                         into jj
                         from rt in jj.DefaultIfEmpty()
                         where rt.SPRAS_ID.Equals("ES")
                         select new DOCUMENTOR_MOD
                         {
                             LIFNR = r.LIFNR,
                             BUKRS = r.BUKRS,
                             WITHT = r.WITHT,
                             WT_WITHCD = r.WT_WITHCD,
                             DESC = rt.TXT50 == null ? String.Empty : r.DESC,

                         }).ToList();
            }
            for (int i = 0; i < retlt.Count; i++)
            {
                var wtht = retlt[i].WITHT;
                decimal _bi = 0;
                decimal _iret = 0;
                //aqui hacemos la sumatoria
                var _res = db.DOCUMENTORPs.Where(nd => nd.NUM_DOC == dOCUMENTO.NUM_DOC && nd.WITHT == wtht).ToList();
                for (int y = 0; y < _res.Count; y++)
                {
                    if (_res[y].BIMPONIBLE == null)
                    {
                        _bi = _bi + 0;
                    }
                    else
                    {
                        _bi = _bi + decimal.Parse(_res[y].BIMPONIBLE.ToString());
                    }
                    if (_res[y].IMPORTE_RET == null)
                    {
                        _iret = _iret + 0;
                    }
                    else
                    {
                        _iret = _iret + decimal.Parse(_res[y].IMPORTE_RET.ToString());
                    }
                }
                retlt[i].BIMPONIBLE = _bi;
                retlt[i].IMPORTE_RET = _iret;
            }
            ViewBag.ret = retlt;
            JsonResult jc = Json(retlt, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]
        public JsonResult getDocsPSTR(string id)
        {
            var _id = decimal.Parse(id);
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(_id);

            //Obtener miles y dec
            var formato = db.FORMATOes.Where(fo => fo.ACTIVO == true).FirstOrDefault();
            //Documento a documento mod //Copiar valores del post al nuevo objeto
            DOCUMENTO_MOD doc = new DOCUMENTO_MOD();

            if (dOCUMENTO.DOCUMENTOPs != null)
            {
                List<DOCUMENTOP_MODSTR> dml = new List<DOCUMENTOP_MODSTR>();
                FormatosC fc = new FormatosC();
                var dps = dOCUMENTO.DOCUMENTOPs.Where(x => x.ACCION != "H").ToList();
                //Agregar a documento p_mod para agregar valores faltantes
                for (int i = 0; i < dps.Count; i++)
                {
                    DOCUMENTOP_MODSTR dm = new DOCUMENTOP_MODSTR();

                    dm.NUM_DOC = dps.ElementAt(i).NUM_DOC;
                    dm.POS = dps.ElementAt(i).POS;
                    dm.ACCION = dps.ElementAt(i).ACCION;
                    dm.FACTURA = dps.ElementAt(i).FACTURA;
                    dm.TCONCEPTO = dps.ElementAt(i).TCONCEPTO;//LEJGG 05-11-18
                    dm.GRUPO = dps.ElementAt(i).GRUPO;
                    dm.CUENTA = dps.ElementAt(i).CUENTA;
                    string ct = dps.ElementAt(i).GRUPO;
                    var tct = dps.ElementAt(i).TCONCEPTO;

                    //FRT08112018 Agrgar Impuesto
                    var _imp = dps.ElementAt(i).MWSKZ;
                    var _imptxt = db.IMPUESTOTs.Where(a => a.MWSKZ == _imp).FirstOrDefault().TXT50;
                    dm.IMPUESTOT = _imp + " - " + _imptxt;
                    dm.MWSKZ = dps.ElementAt(i).MWSKZ;
                    //END FRT08112018


                    try
                    {
                        dm.NOMCUENTA = db.CONCEPTOes.Where(x => x.ID_CONCEPTO == ct && x.TIPO_CONCEPTO == tct).FirstOrDefault().DESC_CONCEPTO.Trim();
                    }
                    catch (Exception e)
                    {
                        dm.NOMCUENTA = "Transporte";
                    }
                    dm.TIPOIMP = dps.ElementAt(i).TIPOIMP;
                    dm.IMPUTACION = dps.ElementAt(i).IMPUTACION;
                    dm.CCOSTO = dps.ElementAt(i).CCOSTO;//LEJGG 05-11-18
                    dm.MONTO = fc.toShow(dps.ElementAt(i).MONTO, formato.DECIMALES);
                    dm.IVA = fc.toShow(dps.ElementAt(i).IVA, formato.DECIMALES);
                    dm.TEXTO = dps.ElementAt(i).TEXTO;
                    dm.TOTAL = fc.toShow(dps.ElementAt(i).TOTAL, formato.DECIMALES);

                    dml.Add(dm);
                }
                var _id2 = decimal.Parse(id);
                var _t = db.DOCUMENTOPs.Where(x => x.NUM_DOC == _id2 && x.ACCION != "H").ToList();
                decimal _total = 0;
                for (int i = 0; i < _t.Count; i++)
                {
                    _total = _total + _t[i].TOTAL;
                }
                ViewBag.total = _total;
                doc.DOCUMENTOPSTR = dml;
            }
            JsonResult jc = Json(doc, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]
        [AllowAnonymous]
        public void LoadExcelSop()
        {

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                //using (var stream2 = System.IO.File.Open(url, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                //{
                string extension = System.IO.Path.GetExtension(file.FileName);
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                //using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream))
                //{
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
                // 2. Use the AsDataSet extension method
                DataSet result = reader.AsDataSet();

                // The result of each spreadsheet is in result.Tables
                // 3.DataSet - Create column names from first row
                DataTable dt = result.Tables[0];

                //Rows
                var rowsc = dt.Rows.Count;
                //columns
                var columnsc = dt.Columns.Count;

                //Columnd and row to start
                var rows = 1; // 2
                              //var cols = 0; // A
                int pos = 1;

                for (int i = rows; i < rowsc; i++)
                {

                    CONCEPTO doc = new CONCEPTO();


                    try
                    {
                        doc.ID_CONCEPTO = dt.Rows[i][1].ToString(); //id
                    }
                    catch (Exception e)
                    {
                        doc.ID_CONCEPTO = null;
                    }
                    try
                    {
                        doc.DESC_CONCEPTO = dt.Rows[i][2].ToString(); //desc
                    }
                    catch (Exception e)
                    {
                        doc.DESC_CONCEPTO = null;
                    }

                    try
                    {
                        doc.TIPO_CONCEPTO = dt.Rows[i][3].ToString(); //tipo concepto
                    }
                    catch (Exception e)
                    {
                        doc.TIPO_CONCEPTO = null;
                    }
                    try
                    {

                        doc.TIPO_IMPUTACION = dt.Rows[i][4].ToString(); //tipo imputación
                    }
                    catch (Exception e)
                    {
                        doc.TIPO_IMPUTACION = null;
                    }
                    try
                    {
                        doc.ID_PRESUPUESTO = dt.Rows[i][5].ToString(); //id presupuesto
                    }
                    catch (Exception e)
                    {
                        doc.ID_PRESUPUESTO = null;
                    }
                    try
                    {
                        doc.TIPO_PRESUPUESTO = dt.Rows[i][6].ToString(); //tipo presupuesto
                    }
                    catch (Exception e)
                    {
                        doc.TIPO_PRESUPUESTO = null;
                    }
                    try
                    {
                        doc.CUENTA = dt.Rows[i][7].ToString(); //cuenta
                    }
                    catch (Exception e)
                    {
                        doc.CUENTA = null;
                    }

                    try
                    {

                        db.CONCEPTOes.Add(doc);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string mess = e.Message.ToString();
                    }

                }
            }
        }

        [HttpPost]//LEJGG 19102018
        public JsonResult getAnexos(decimal id)
        {
            var c = db.DOCUMENTOAs.Where(i => i.NUM_DOC == id).ToList();
            var result = c.Select(m => m.POSD).Distinct().ToList();

            var dp = db.DOCUMENTOPs.Where(i => i.ACCION != "H" && i.NUM_DOC == id).ToList();




            List<Anexo> _lstax = new List<Anexo>();
            Anexo _ax = new Anexo();
            List<decimal> lstd = new List<decimal>();


            for (int w = 0; w < dp.Count; w++)
            {
                var _1filtro = c.Where(x => x.POSD == dp[w].POS).ToList();
                if (_1filtro.Count > 0)
                {
                    _ax = new Anexo();
                    for (int y = 0; y < _1filtro.Count; y++)
                    {
                        //_ax = new Anexo();
                        if (y == 0)
                        {
                            _ax.a1 = int.Parse(_1filtro[y].POS.ToString());
                        }
                        if (y == 1)
                        {
                            _ax.a2 = int.Parse(_1filtro[y].POS.ToString());
                        }
                        if (y == 2)
                        {
                            _ax.a3 = int.Parse(_1filtro[y].POS.ToString());
                        }
                        if (y == 3)
                        {
                            _ax.a4 = int.Parse(_1filtro[y].POS.ToString());
                        }
                        if (y == 4)
                        {
                            _ax.a5 = int.Parse(_1filtro[y].POS.ToString());
                        }


                    }
                }
                else
                {
                    _ax = new Anexo();
                    _ax.a1 = 0;
                    _ax.a2 = 0;
                    _ax.a3 = 0;
                    _ax.a4 = 0;
                    _ax.a5 = 0;

                }
                _lstax.Add(_ax);
            }

            //for (int i = 0; i < result.Count; i++)
            //{

            //    //var _1filtro = c.Where(x => x.POS == (p + 1) && x.POSD == c[i].POSD).ToList();
            //    var _1filtro = c.Where(x => x.POSD == c[i].POSD).ToList();

            //    //_ax = new Anexo();
            //    //for (int y = 0; y < _1filtro.Count; y++)
            //    //{
            //    //    //_ax = new Anexo();
            //    //    if (y == 0)
            //    //    {
            //    //        _ax.a1 = int.Parse(_1filtro[y].POS.ToString());
            //    //    }
            //    //    if (y == 1)
            //    //    {
            //    //        _ax.a2 = int.Parse(_1filtro[y].POS.ToString());
            //    //    }
            //    //    if (y == 2)
            //    //    {
            //    //        _ax.a3 = int.Parse(_1filtro[y].POS.ToString());
            //    //    }
            //    //    if (y == 3)
            //    //    {
            //    //        _ax.a4 = int.Parse(_1filtro[y].POS.ToString());
            //    //    }
            //    //    if (y == 4)
            //    //    {
            //    //        _ax.a5 = int.Parse(_1filtro[y].POS.ToString());
            //    //    }



            //    //    //switch (y + 1)
            //    //    //{
            //    //    //    case 1:
            //    //    //        _ax.a1 = int.Parse(_1filtro[y].POS.ToString());
            //    //    //        break;
            //    //    //    case 2:
            //    //    //        _ax.a2 = int.Parse(_1filtro[y].POS.ToString());
            //    //    //        break;
            //    //    //    case 3:
            //    //    //        _ax.a3 = int.Parse(_1filtro[y].POS.ToString());
            //    //    //        break;
            //    //    //    case 4:
            //    //    //        _ax.a4 = int.Parse(_1filtro[y].POS.ToString());
            //    //    //        break;
            //    //    //    case 5:
            //    //    //        _ax.a5 = int.Parse(_1filtro[y].POS.ToString());
            //    //    //        break;
            //    //    //    default:
            //    //    //        break;
            //    //    //}

            //    //}

            //    _lstax.Add(_ax);


            //}
            JsonResult jc = Json(_lstax, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]
        public FileResult Descargar(string archivo)
        {
            //LEJGG 09-11-2018------------------------------------
            if (archivo.Contains("%20"))//codificacion a tipo de url
            {
                var _Ar = archivo.Replace("%20", " ");

                //LEJ 03.10.2018
                string nombre = "", contentyp = "";
                contDescarga(_Ar, ref contentyp, ref nombre);
                //return File(descargarArchivo(btnArchivo, contentyp, nombre), contentyp, nombre);
                return File(_Ar, contentyp, nombre);
            }
            //LEJGG 09-11-2018------------------------------------
            else
            {
                //LEJ 03.10.2018
                string nombre = "", contentyp = "";
                contDescarga(archivo, ref contentyp, ref nombre);
                //return File(descargarArchivo(btnArchivo, contentyp, nombre), contentyp, nombre);
                return File(archivo, contentyp, nombre);
            }
        }

        [HttpPost]
        //public FileResult crearReporte()
        public string crearReporte()
        {
            var xr = db.SP_REPORTESOLS(1, "1", "1", 2);
            //return File(archivo, contentyp, nombre);
            return "";
        }

        //FRT06112018 Se agrega para poder descargar archivos desde detail
        [HttpPost]
        public FileResult DescargarDetails()
        {

            var archivo = Request.Form["file"];
            //LEJ 03.10.2018
            string nombre = "", contentyp = "";
            contDescarga(archivo, ref contentyp, ref nombre);
            //return File(descargarArchivo(btnArchivo, contentyp, nombre), contentyp, nombre);
            return File(archivo, contentyp, nombre);
        }

        // END FRT06112018

        /*  public string SaveFile(HttpPostedFileBase file, string path)
          {
              string ex = "";
              //string exdir = "";
              // Get the name of the file to upload.
              string fileName = file.FileName;//System.IO.Path.GetExtension(file.FileName);    // must be declared in the class above

              // Specify the path to save the uploaded file to.
              string savePath = path + "//";

              // Create the path and file name to check for duplicates.
              string pathToCheck = savePath;

              // Append the name of the file to upload to the path.
              savePath += fileName;
              //-------------------------------------------------------------------
              FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + path + "/" + fileName);
              request.Method = WebRequestMethods.Ftp.UploadFile;

              const string Comillas = "\"";
              string pwd = "Rumaki,2571" + Comillas + "k41";
              request.Credentials = new NetworkCredential("luis.gonzalez", pwd);
              var sourceStream = file.InputStream;
              Stream requestStream = request.GetRequestStream();
              request.ContentLength = sourceStream.Length;

              StreamReader streamReader = new StreamReader(file.InputStream);
              byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
              //sourceStream.Close();           
              requestStream.Write(fileContents, 0, fileContents.Length);
              requestStream.Close();
              request.ContentLength = fileContents.Length;

              var response = (FtpWebResponse)request.GetResponse();
              //-------------------------------------------------------------------
              //Parte para guardar archivo en el servidor
              try
              {
                  //Guardar el archivo
                  // file.SaveAs(savePath);
              }
              catch (Exception e)
              {
                  ex = "";
                  ex = fileName;
              }

              //Guardarlo en la base de datos
              if (ex == "")
              {

              }
              return savePath;
          }*/


        //------------------------------------------------------------------------<
        //Lejgg 25-10-2018
        public string SaveFile(HttpPostedFileBase file, decimal nd)
        {
            var url_prel = "";
            var dirFile = "";
            string carpeta = "att";
            try
            {
                url_prel = getDirPrel(carpeta, nd);
                dirFile = url_prel;
            }
            catch (Exception e)
            {
                dirFile = ConfigurationManager.AppSettings["URL_ATT"].ToString() + @"att";
            }
            bool existd = ValidateIOPermission(dirFile);

            try
            {
                //El direcorio existe
                if (existd)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(Path.Combine(dirFile, fileName));
                }
                else
                {
                    //System.IO.Directory.CreateDirectory(Server.MapPath(dirFile));
                }
            }
            catch (Exception e)
            { }
            return dirFile + "\\" + file.FileName;
        }

        private string getDirPrel(string carpeta, decimal numdoc)
        {
            string dir = "";
            try
            {
                dir = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_ATT") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();
                dir += carpeta + "\\" + numdoc;
            }
            catch (Exception e)
            {
                dir = "";
            }

            return dir;
        }

        private bool ValidateIOPermission(string path)
        {
            string user = "";
            string pass = "";

            user = getUserPrel();
            pass = getPassPrel();
            try
            {
                try
                {
                    if (Directory.Exists(path))
                        return true;

                    else
                    {
                        Directory.CreateDirectory(path);
                        return true;
                    }
                }

                catch (Exception ex)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string getUserPrel()
        {
            string user = "";
            try
            {
                user = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("USER_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

            }
            catch (Exception e)
            {
                user = "";
            }

            return user;

        }

        private string getPassPrel()
        {
            string pass = "";
            try
            {
                pass = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("PASS_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

            }
            catch (Exception e)
            {
                pass = "";
            }

            return pass;

        }
        //------------------------------------------------------------------------<
        //public string descargarArchivo(string dir, string tipoDoc, string nombre)
        //{
        //    int resp = 0;
        //    string LocalDestinationPath = @"C:\Users\EQUIPO\Documents\GitHub\WFARTHA\WFARTHA\Descargas\" + nombre;
        //    string Username = "luis.gonzalez";
        //    const string Comillas = "\"";
        //    string pwd = "Rumaki,2571" + Comillas + "k41";
        //    bool UseBinary = true; // use true for .zip file or false for a text file
        //    bool UsePassive = false;

        //    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + dir));
        //    // FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://"+dir);
        //    request.Method = WebRequestMethods.Ftp.DownloadFile;
        //    request.KeepAlive = true;
        //    request.UsePassive = UsePassive;
        //    request.UseBinary = UseBinary;

        //    request.Credentials = new NetworkCredential(Username, pwd);

        //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        //    Stream responseStream = response.GetResponseStream();
        //    StreamReader reader = new StreamReader(responseStream);

        //    using (FileStream writer = new FileStream(LocalDestinationPath, FileMode.Create))
        //    {
        //        long length = response.ContentLength;
        //        int bufferSize = 2048;
        //        int readCount;
        //        byte[] buffer = new byte[2048];

        //        readCount = responseStream.Read(buffer, 0, bufferSize);
        //        while (readCount > 0)
        //        {
        //            writer.Write(buffer, 0, readCount);
        //            readCount = responseStream.Read(buffer, 0, bufferSize);
        //        }
        //        resp++;
        //    }

        //    reader.Close();
        //    response.Close();
        //    if (resp > 0)
        //    {
        //        //return LocalDestinationPath;
        //        return dir;
        //    }
        //    return "";
        //}

        public void contDescarga(string ruta, ref string contentType, ref string nombre)
        {
            string[] archivo = ruta.Split('/');
            nombre = archivo[archivo.Length - 1];
            string[] extencion = archivo[archivo.Length - 1].Split('.');
            switch (extencion[extencion.Length - 1].ToLower())
            {
                case "xltx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                    break;
                case "xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "xlsm":
                    contentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
                    break;
                case "xltm":
                    contentType = "application/vnd.ms-excel.template.macroEnabled.12";
                    break;
                case "xlam":
                    contentType = "application/vnd.ms-excel.addin.macroEnabled.12";
                    break;
                case "xlsb":
                    contentType = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                    break;
                case "xls":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "xlt":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "xla":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "doc":
                    contentType = "application/msword";
                    break;
                case "dot":
                    contentType = "application/msword";
                    break;
                case "docx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "dotx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                    break;
                case "docm":
                    contentType = "application/vnd.ms-word.document.macroEnabled.12";
                    break;
                case "dotm":
                    contentType = "application/vnd.ms-word.template.macroEnabled.12";
                    break;
                case "pdf":
                    contentType = "application/pdf";
                    break;
                case "zip":
                    contentType = "application/zip";
                    break;
                case "jpeg":
                    contentType = "image/jpeg";
                    break;
                case "ppt":
                    contentType = "application/vnd.ms-powerpoint";
                    break;
                case "pptx":
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case "jpg":
                    contentType = "image/jpg";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                case "msg":
                    contentType = "application/vnd.ms-outlook";
                    break;
                case "txt":
                    contentType = "text/plain";
                    break;
                case "xml":
                    contentType = "text/html";
                    break;
            }
        }
        //BEGIN OF INSERT RSG 17.10.2018
        [HttpPost]
        public JsonResult getPedidos(string Prefix, string lifnr)
        {
            try
            {
                var c = (from N in db.EKKO_DUMMY
                         where (N.LIFNR == lifnr)// & N.EBELN.Contains(Prefix))
                         select new { N.EBELN }).ToList();

                JsonResult jc = Json(c, JsonRequestBehavior.AllowGet);
                return jc;
            }
            catch (Exception e)
            {
                JsonResult jc = Json("", JsonRequestBehavior.AllowGet);
                return jc;
            }
        }
        [HttpPost]
        public JsonResult getPedidosPos(string ebeln)
        {
            var c = (from N in db.EKPO_DUMMY
                     where (N.EBELN.Equals(ebeln))
                     select new
                     {
                         N.EBELN,
                         N.EBELP,
                         N.BEDAT,
                         N.MATNR,
                         N.TXZ01,
                         N.MATKL,
                         N.WERKS,
                         N.LGORT,
                         N.MENGE,
                         N.MEINS,
                         N.NETPR,
                         N.WAERS,
                         N.PEINH,
                         N.MENGE_DEL,
                         N.NETPR_DEL,
                         N.MENGE_BIL,
                         N.NETPR_BIL,
                         N.MWSKZ
                     }).ToList();

            JsonResult jc = Json(c, JsonRequestBehavior.AllowGet);
            return jc;
        }
        //END OF INSERT RSG 17.10.2018
        //START OF INSERT RSG 19.10.2018
        [HttpPost]
        public JsonResult getFondos(string ebeln)
        {
            var c = (from N in db.EKKO_DUMMY
                     where (N.EBELN.Equals(ebeln))
                     select new
                     {
                         N.EBELN,
                         N.BUKRS,
                         N.BSART,
                         N.LIFNR,
                         N.RETTP,
                         N.RETPC,
                         N.DPTYP,
                         N.DPPCT,
                         N.DPAMT,
                         N.ESTATUS
                     });
            JsonResult jc = Json(c, JsonRequestBehavior.AllowGet);
            return jc;
        }
        //END OF INSERT RSG 19.10.2018

        [HttpPost]
        public JsonResult getCadAut(decimal nd)
        {
            //Traigo el usuario
            var usA = db.DOCUMENTOes.Where(x => x.NUM_DOC == nd).FirstOrDefault();
            var uC = usA.USUARIOC_ID;
            var ud = usA.USUARIOD_ID;
            var detcv = (from detc in db.DET_AGENTECC
                         where detc.USUARIOC_ID == uC
                         group detc by new { detc.USUARIOC_ID, detc.ID_RUTA_AGENTE, detc.USUARIOA_ID } into grp
                         select grp.OrderByDescending(x => x.VERSION).FirstOrDefault()).ToList();
            var res = detcv.Where(x => x.USUARIOA_ID == ud).FirstOrDefault();
            string[] _r = { usA.SOCIEDAD_ID, usA.MONTO_DOC_MD.ToString(), res.ID_RUTA_AGENTE, res.USUARIOA_ID, res.USUARIOC_ID, res.VERSION.ToString() };
            JsonResult jc = Json(_r, JsonRequestBehavior.AllowGet);
            return jc;
        }

        //MGC 18-10-2018 Firma del usuario ------------------------------------------------->
        [HttpPost]
        public string ValF(string pws)
        {
            //Valor para devolver
            string res = "false";
            //Obtener el usuario
            string u = User.Identity.Name;

            bool m = false;
            //Add usuario
            if (m == true)
            {
                Cryptography cr = new Cryptography();
                USUARIO us = db.USUARIOs.Where(usr => usr.ID == "admin").FirstOrDefault();
                string passn = cr.Encrypt("admin1");

                us.FIRMA = passn;

                db.Entry(us).State = EntityState.Modified;
                db.SaveChanges();
            }


            USUARIO userm = new USUARIO();
            USUARIO user = new USUARIO();
            userm.ID = u;
            userm.PASS = pws;

            Cryptography c = new Cryptography();
            string pass = c.Encrypt(userm.PASS);

            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                user = db.USUARIOs.Where(a => a.ID.Equals(userm.ID) && a.FIRMA.Equals(pass)).FirstOrDefault();
            }

            if (user != null)
            {
                res = "true";
            }

            return res;
        }

        //MGC 18-10-2018 Firma del usuario --------------------------------------------------<
        //MGC 19-10-2018 CECOS -------------------------------------------------------------->
        [HttpPost]
        public JsonResult getCcosto(string Prefix, string bukrs)
        {

            if (Prefix == null)
                Prefix = "";

            WFARTHAEntities db = new WFARTHAEntities();


            ////FRT02122018 pARA VALIDAR MAYUSCULAS MINUSCULAS
            //Prefix = Prefix.ToUpper();
            ////ENDFRT02122018


            SOCIEDAD c = db.SOCIEDADs.Where(soc => soc.BUKRS == bukrs).FirstOrDefault();
            //List<PROVEEDOR> lprov = new List<PROVEEDOR>();
            //List<PROVEEDOR> lprov2 = new List<PROVEEDOR>();

            var r = (dynamic)null;

            if (c != null)
            {
                var lprov = (from cc1 in db.CECOes.ToList()
                                 //where cc1.CECO1.Contains(Prefix) && cc1.BUKRS == c.BUKRS//FRT04122018
                             where cc1.CECO1.ToLower().Contains(Prefix.ToLower()) && cc1.BUKRS == c.BUKRS
                             select new CECO
                             {
                                 BUKRS = cc1.BUKRS.ToString(),
                                 CECO1 = cc1.CECO1.ToString(),
                                 TEXT = cc1.TEXT.ToString()
                             }).ToList();

                if (lprov.Count == 0)
                {
                    var lprov2 = (from cc1 in db.CECOes.ToList()
                                      //where cc1.TEXT.Contains(Prefix) && cc1.BUKRS == c.BUKRS//FRT04122018
                                  where cc1.TEXT.ToLower().Contains(Prefix.ToLower()) && cc1.BUKRS == c.BUKRS
                                  select new CECO
                                  {
                                      BUKRS = cc1.BUKRS.ToString(),
                                      CECO1 = cc1.CECO1.ToString(),
                                      TEXT = cc1.TEXT.ToString()
                                  }).ToList();

                    lprov.AddRange(lprov2);
                }

                r = lprov;
            }

            JsonResult cc = Json(r, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //MGC 19-10-2018 CECOS --------------------------------------------------------------<

        //LEJGG 05-12-2018
        [HttpPost]
        public JsonResult getCad3(string user_id, string bukrs)
        {
            var detcv = (from detc in db.DET_AGENTECC
                         where detc.USUARIOC_ID == user_id
                         group detc by new { detc.USUARIOC_ID, detc.ID_RUTA_AGENTE, detc.USUARIOA_ID } into grp
                         select grp.OrderByDescending(x => x.VERSION).FirstOrDefault());

            //Obtener las cadenas
            List<DET_AGENTECC> detcl = new List<DET_AGENTECC>();

            detcl = (from dv in detcv.ToList()
                     join dccl in db.DET_AGENTECC.ToList()
                     on new { dv.VERSION, dv.USUARIOC_ID, dv.ID_RUTA_AGENTE, dv.USUARIOA_ID } equals new { dccl.VERSION, dccl.USUARIOC_ID, dccl.ID_RUTA_AGENTE, dccl.USUARIOA_ID }
                     select new DET_AGENTECC
                     {
                         VERSION = dccl.VERSION,
                         USUARIOC_ID = dccl.USUARIOC_ID,
                         ID_RUTA_AGENTE = dccl.ID_RUTA_AGENTE,
                         DESCRIPCION = dccl.DESCRIPCION,
                         USUARIOA_ID = dccl.USUARIOA_ID,
                         FECHAC = dccl.FECHAC
                     }).ToList();

            //Obtener el usario a
            DET_AGENTECC deta = new DET_AGENTECC();

            if (detcl != null && detcl.Count > 0)
            {
                deta = detcl.FirstOrDefault();
            }

            var dta = detcl.
                Join(
                db.USUARIOs,
                da => da.USUARIOA_ID,
                us => us.ID,
                (da, us) => new
                {
                    //ID = new List<string>() { da.VERSION, da.USUARIOC_ID, da.ID_RUTA_AGENTE, da.USUARIOA_ID},                    
                    ID = new { VERSION = da.VERSION.ToString().Replace(" ", ""), USUARIOC_ID = da.USUARIOC_ID.ToString().Replace(" ", ""), ID_RUTA_AGENTE = da.ID_RUTA_AGENTE.ToString().Replace(" ", ""), USUARIOA_ID = da.USUARIOA_ID.ToString().Replace(" ", "") },
                    TEXT = us.NOMBRE.ToString() + " " + us.APELLIDO_P.ToString()
                }).ToList();

            //MGC 11-12-2018 Agregar Contabilizador 0------>
            int vc1 = 0;
            int vc2 = 0;
            //MGC 11-12-2018 Agregar Contabilizador 0------<

            var _v = "";
            var _usc = "";
            var _id_ruta = "";
            var _usa = "";
            var _mt = "";
            var _sc = "";
            var lstDta = new List<object>();
            //LEJGG03-12-2018-----------------------------I
            for (int i = 0; i < dta.Count; i++)
            {
                _v = dta[i].ID.VERSION;
                _usc = dta[i].ID.USUARIOC_ID;
                _id_ruta = dta[i].ID.ID_RUTA_AGENTE;
                _usa = dta[i].ID.USUARIOA_ID;
                _mt = 0 + "";
                _sc = bukrs;
                //var _lst = getCad2(int.Parse(_v), _usc, _id_ruta, _usa, decimal.Parse(_mt), _sc, out vc1, out vc2); //MGC 11-12-2018 Agregar Contabilizador 0
                CadenaAutorizadores _lst = getCad2(int.Parse(_v), _usc, _id_ruta, _usa, decimal.Parse(_mt), _sc, vc1, vc2); //MGC 11-12-2018 Agregar Contabilizador 0
                List<CadenaAutorizadoresItem> cadi = new List<CadenaAutorizadoresItem>();//MGC 11-12-2018 Agregar Contabilizador 0
                cadi = _lst.cadenal;//MGC 11-12-2018 Agregar Contabilizador 0
                string cad = "";
                //for (int j = 0; j < _lst.Count; j++)//MGC 11-12-2018 Agregar Contabilizador 0
                for (int j = 0; j < cadi.Count; j++)//MGC 11-12-2018 Agregar Contabilizador 0
                {
                    //var aut = lst[j].autorizador.Split('-');//MGC 11-12-2018 Agregar Contabilizador 0
                    var aut = cadi[j].autorizador.Split('-');//MGC 11-12-2018 Agregar Contabilizador 0
                    //if (j == lst.Count - 1)//MGC 11-12-2018 Agregar Contabilizador 0
                    if (j == cadi.Count - 1)//MGC 11-12-2018 Agregar Contabilizador 0
                    {
                        cad += aut[0].Trim();
                    }
                    else
                    {
                        cad += aut[0].Trim() + " - ";
                    }
                }
                var nuevo = new
                {
                    ID = dta[i].ID,
                    TEXT = cad
                };
                lstDta.Add(nuevo);
            }
            var listaCad = lstDta;
            JsonResult cc = Json(listaCad, JsonRequestBehavior.AllowGet);
            return cc;
        }
        //LEJGG 05-12-2018
        //MGC 14-11-2018 Cadena de autorización----------------------------------------------------------------------------->

        [HttpPost]
        public JsonResult getCadena(int version, string usuarioc, string id_ruta, string usuarioa, decimal monto, string bukrs, int vc1, int vc2) //MGC 11-12-2018 Agregar Contabilizador 0
        {
            //MGC 11-12-2018 Agregar Contabilizador 0--------->
            int vcl1 = 0;
            int vcl2 = 0;
            //MGC 11-12-2018 Agregar Contabilizador 0---------<


            //Obtener el encabezado de la cadena
            DET_AGENTECC detc = new DET_AGENTECC();
            List<int> fases = new List<int>();

            detc = db.DET_AGENTECC.Where(dc => dc.VERSION == version && dc.USUARIOC_ID == usuarioc && dc.ID_RUTA_AGENTE == id_ruta && dc.USUARIOA_ID == usuarioa).FirstOrDefault();

            List<DET_AGENTECA> deta = new List<DET_AGENTECA>();
            //Existe el encabezado de la cadena
            if (detc != null)
            {
                deta = db.DET_AGENTECA.Where(da => da.ID_RUTA_AGENTE == detc.ID_RUTA_AGENTE && da.VERSION == detc.VERSION).OrderBy(da => da.STEP_FASE).ToList();
            }

            //Lista de cadena de autorizadores
            //List<CadenaAutorizadores> lcadena = new List<CadenaAutorizadores>();//MGC 11-12-2018 Agregar Contabilizador 0
            List<CadenaAutorizadoresItem> lcadena = new List<CadenaAutorizadoresItem>();//MGC 11-12-2018 Agregar Contabilizador 0

            //Agregar el solicitante
            try
            {
                if (detc != null)
                {
                    CadenaAutorizadoresItem sol = new CadenaAutorizadoresItem();
                    sol.fase = "Solicitante";
                    sol.autorizador = detc.USUARIOA_ID;

                    lcadena.Add(sol);
                }
            }
            catch (Exception e)
            {

            }

            //MGC 11-12-2018 Agregar Contabilizador 0--------------------------------->
            ProcesaFlujo pf = new ProcesaFlujo();
            try
            {
                DET_APROB0 dap = new DET_APROB0();
                dap = pf.determinaAgenteContabilidadCadena0(bukrs, vc1); //MGC 11-12-2018 Agregar Contabilizador 0
                if (dap != null)
                {
                    //Se obtiene el agente contabilizador 
                    CadenaAutorizadoresItem cadenaauto = new CadenaAutorizadoresItem();
                    cadenaauto.fase = "Contabilizador";
                    cadenaauto.autorizador = dap.ID_USUARIO;


                    vcl1 = dap.VERSION;//MGC 11-12-2018 Agregar Contabilizador 0

                    lcadena.Add(cadenaauto);
                }
            }
            catch (Exception)
            {

            }
            //MGC 11-12-2018 Agregar Contabilizador 0---------------------------------<

            //Obtener las fases del proyecto
            fases = deta.Select(da => da.STEP_FASE).Distinct().ToList();
            
            //loop para obtener los autorizadores por fase
            for (int i = 0; i < fases.Count(); i++)
            {
                List<DET_AGENTECA> detafase = new List<DET_AGENTECA>();

                detafase = deta.Where(detf => detf.STEP_FASE == fases[i]).ToList();

                if (detafase != null || detafase.Count > 0)
                {
                    DET_AGENTECA dap = new DET_AGENTECA();
                    dap = pf.detAgenteLimite(detafase, monto);

                    //Si se obtiene el agente por monto y fase, agregarlo a la lista
                    CadenaAutorizadoresItem cadenaauto = new CadenaAutorizadoresItem();
                    cadenaauto.fase = "Aprobador " + dap.STEP_FASE;
                    cadenaauto.autorizador = dap.AGENTE_SIG;

                    lcadena.Add(cadenaauto);

                }
            }

            //Obtener el contabilizador
            try
            {
                DET_APROB dap = new DET_APROB();
                dap = pf.determinaAgenteContabilidadCadena(bukrs, vc2);//MGC 11-12-2018 Agregar Contabilizador 0
                if (dap != null)
                {
                    //Se obtiene el agente contabilizador 
                    CadenaAutorizadoresItem cadenaauto = new CadenaAutorizadoresItem();
                    cadenaauto.fase = "Contabilizador";
                    cadenaauto.autorizador = dap.ID_USUARIO;

                    vcl2 = dap.VERSION;//MGC 11-12-2018 Agregar Contabilizador 0

                    lcadena.Add(cadenaauto);
                }
            }
            catch (Exception)
            {

            }

            //Obtener el nombre de los usuarios
            //List<CadenaAutorizadores> lcadenan = new List<CadenaAutorizadores>();//MGC 11-12-2018 Agregar Contabilizador 0
            List<CadenaAutorizadoresItem> lcadenan = new List<CadenaAutorizadoresItem>();//MGC 11-12-2018 Agregar Contabilizador 0
            try
            {
                lcadenan = (from ca in lcadena
                            join us in db.USUARIOs
                            on ca.autorizador equals us.ID
                            into jj
                            from us in jj.DefaultIfEmpty()
                            select new CadenaAutorizadoresItem//MGC 11-12-2018 Agregar Contabilizador 0
                            {
                                fase = ca.fase,
                                //autorizador = ca.autorizador + " - " + us.NOMBRE + " " + us.APELLIDO_P + " " + us.APELLIDO_M
                                autorizador = us.NOMBRE + " " + us.APELLIDO_P + " " + us.APELLIDO_M//lejgg 03-12-2018
                            }).ToList();
            }
            catch (Exception)
            {

            }

            //MGC 11-12-2018 Agregar Contabilizador 0--->
            CadenaAutorizadores cda = new CadenaAutorizadores();
            cda.vc1 = vcl1;
            cda.vc2 = vcl2;
            cda.cadenal = lcadenan;
            //MGC 11-12-2018 Agregar Contabilizador 0---<

            //JsonResult jc = Json(lcadenan, JsonRequestBehavior.AllowGet);//MGC 11-12-2018 Agregar Contabilizador 0
            JsonResult jc = Json(cda, JsonRequestBehavior.AllowGet);//MGC 11-12-2018 Agregar Contabilizador 0
            return jc;
        }

        //LEJGG 03-12-2018
        //public List<CadenaAutorizadores> getCad2(int version, string usuarioc, string id_ruta, string usuarioa, decimal monto, string bukrs, out int vc1, out int vc2)//MGC 11-12-2018 Agregar Contabilizador 0
        public CadenaAutorizadores getCad2(int version, string usuarioc, string id_ruta, string usuarioa, decimal monto, string bukrs, int vc1, int vc2)//MGC 11-12-2018 Agregar Contabilizador 0
        {
            //MGC 11-12-2018 Agregar Contabilizador 0---->
            int vcl1 = 0;
            int vcl2 = 0;

            //MGC 11-12-2018 Agregar Contabilizador 0----<

            //Obtener el encabezado de la cadena
            DET_AGENTECC detc = new DET_AGENTECC();
            List<int> fases = new List<int>();

            detc = db.DET_AGENTECC.Where(dc => dc.VERSION == version && dc.USUARIOC_ID == usuarioc && dc.ID_RUTA_AGENTE == id_ruta && dc.USUARIOA_ID == usuarioa).FirstOrDefault();

            List<DET_AGENTECA> deta = new List<DET_AGENTECA>();
            //Existe el encabezado de la cadena
            if (detc != null)
            {
                deta = db.DET_AGENTECA.Where(da => da.ID_RUTA_AGENTE == detc.ID_RUTA_AGENTE && da.VERSION == detc.VERSION).OrderBy(da => da.STEP_FASE).ToList();
            }

            //Lista de cadena de autorizadores
            //List<CadenaAutorizadores> lcadena = new List<CadenaAutorizadores>();//MGC 11-12-2018 Agregar Contabilizador 0
            List<CadenaAutorizadoresItem> lcadena = new List<CadenaAutorizadoresItem>();//MGC 11-12-2018 Agregar Contabilizador 0


            //Agregar el solicitante
            try
            {
                if (detc != null)
                {
                    CadenaAutorizadoresItem sol = new CadenaAutorizadoresItem();//MGC 11-12-2018 Agregar Contabilizador 0
                    sol.fase = "Solicitante";
                    sol.autorizador = detc.USUARIOA_ID;

                    lcadena.Add(sol);
                }
            }
            catch (Exception e)
            {

            }

            //MGC 11-12-2018 Agregar Contabilizador 0--------------------------------->
            ProcesaFlujo pf = new ProcesaFlujo();
            try
            {
                DET_APROB0 dap = new DET_APROB0();
                dap = pf.determinaAgenteContabilidadCadena0(bukrs, vc1); //MGC 11-12-2018 Agregar Contabilizador 0
                if (dap != null)
                {
                    //Se obtiene el agente contabilizador 
                    CadenaAutorizadoresItem cadenaauto = new CadenaAutorizadoresItem();//MGC 11-12-2018 Agregar Contabilizador 0
                    cadenaauto.fase = "Contabilizador";
                    cadenaauto.autorizador = dap.ID_USUARIO;

                    vcl1 = dap.VERSION;//MGC 11-12-2018 Agregar Contabilizador 0

                    lcadena.Add(cadenaauto);
                }
            }
            catch (Exception)
            {

            }
            //MGC 11-12-2018 Agregar Contabilizador 0---------------------------------<

            //Obtener las fases del proyecto
            fases = deta.Select(da => da.STEP_FASE).Distinct().ToList();
            
            //loop para obtener los autorizadores por fase
            for (int i = 0; i < fases.Count(); i++)
            {
                List<DET_AGENTECA> detafase = new List<DET_AGENTECA>();

                detafase = deta.Where(detf => detf.STEP_FASE == fases[i]).ToList();

                if (detafase != null || detafase.Count > 0)
                {
                    DET_AGENTECA dap = new DET_AGENTECA();
                    dap = pf.detAgenteLimite(detafase, monto);

                    //Si se obtiene el agente por monto y fase, agregarlo a la lista
                    CadenaAutorizadoresItem cadenaauto = new CadenaAutorizadoresItem();//MGC 11-12-2018 Agregar Contabilizador 0
                    cadenaauto.fase = "Aprobador " + dap.STEP_FASE;
                    cadenaauto.autorizador = dap.AGENTE_SIG;

                    lcadena.Add(cadenaauto);

                }
            }

            //Obtener el contabilizador
            try
            {
                DET_APROB dap = new DET_APROB();
                dap = pf.determinaAgenteContabilidadCadena(bukrs, vc2); //MGC 11-12-2018 Agregar Contabilizador 0
                if (dap != null)
                {
                    //Se obtiene el agente contabilizador 
                    CadenaAutorizadoresItem cadenaauto = new CadenaAutorizadoresItem();//MGC 11-12-2018 Agregar Contabilizador 0
                    cadenaauto.fase = "Contabilizador";
                    cadenaauto.autorizador = dap.ID_USUARIO;

                    vcl2 = dap.VERSION;//MGC 11-12-2018 Agregar Contabilizador 0

                    lcadena.Add(cadenaauto);
                }
            }
            catch (Exception)
            {

            }

            //Obtener el nombre de los usuarios
            List<CadenaAutorizadoresItem> lcadenan = new List<CadenaAutorizadoresItem>(); //MGC 11-12-2018 Agregar Contabilizador 0
            try
            {
                lcadenan = (from ca in lcadena
                            join us in db.USUARIOs
                            on ca.autorizador equals us.ID
                            into jj
                            from us in jj.DefaultIfEmpty()
                            select new CadenaAutorizadoresItem //MGC 11-12-2018 Agregar Contabilizador 0
                            {
                                fase = ca.fase,
                                autorizador = ca.autorizador + " - " + us.NOMBRE + " " + us.APELLIDO_P + " " + us.APELLIDO_M
                            }).ToList();
            }
            catch (Exception)
            {

            }


            //MGC 11-12-2018 Agregar Contabilizador 0--->
            CadenaAutorizadores cda = new CadenaAutorizadores();
            cda.vc1 = vcl1;
            cda.vc2 = vcl2;
            cda.cadenal = lcadenan;
            //MGC 11-12-2018 Agregar Contabilizador 0---<

            //return lcadenan;//MGC 11-12-2018 Agregar Contabilizador 0
            return cda;
        }
        //MGC 14-11-2018 Cadena de autorización-----------------------------------------------------------------------------<

        //MGC 10-12-2018 Firma del usuario cancelar------------------------------------------------------------------------->
        [HttpPost]
        public ActionResult Cancelar(decimal id)
        {
            //Session["sol_tipo"] = null;
            DOCUMENTO d = db.DOCUMENTOes.Find(id);
            //d.ESTATUS_C = "C";
            //FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC == id).OrderByDescending(a => a.POS).FirstOrDefault();
            //db.Entry(d).State = EntityState.Modified;
            //if (actual != null)
            //{
            //    FLUJO nuevo = new FLUJO();
            //    WORKFP fin = db.WORKFPs.Where(a => a.ID == actual.WORKF_ID & a.VERSION == actual.WF_VERSION & a.NEXT_STEP == 99).FirstOrDefault();
            //    if (fin != null)
            //    {
            //        nuevo.COMENTARIO = "";
            //        nuevo.DETPOS = 0;
            //        nuevo.DETVER = 0;
            //        nuevo.ESTATUS = "A";
            //        nuevo.FECHAC = DateTime.Now;
            //        nuevo.FECHAM = nuevo.FECHAC;
            //        nuevo.LOOP = 0;
            //        nuevo.NUM_DOC = actual.NUM_DOC;
            //        nuevo.POS = actual.POS + 1;
            //        nuevo.USUARIOA_ID = User.Identity.Name;
            //        nuevo.WF_POS = fin.POS;
            //        nuevo.WF_VERSION = fin.VERSION;
            //        nuevo.WORKF_ID = fin.ID;
            //        db.FLUJOes.Add(nuevo);
            //    }
            //}
            //Enviar el archivo con parámetro de Borrado
            //Crear el archivo para el preliminar //MGC Preliminar
            ProcesaFlujo pf = new ProcesaFlujo();
            string corr = pf.procesaPreliminar(d, false, "", true, "");//MGC-14-12-2018 Modificación fechacon
            //Se genero el archivo de cancelación
            if (corr == "0")
            {
                try
                {
                    DOCUMENTOLOG dl = new DOCUMENTOLOG();
                    dl.NUM_DOC = d.NUM_DOC;
                    dl.TYPE_LINE = "M";
                    dl.TYPE = "S";
                    dl.MESSAGE = "Se generó el Archivo Cancelación Portal";
                    dl.FECHA = DateTime.Now;
                    db.DOCUMENTOLOGs.Add(dl);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                }
                try
                {
                    d.ESTATUS_C = "A";
                    db.Entry(d).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                }
            }
            //No se genero el archivo de preliminar
            else
            {
                string m;
                if (corr.Length > 50)
                {
                    m = corr.Substring(0, 50);
                }
                else
                {
                    m = corr;
                }
                //MGC 30-10-2018 Agregar mensaje a log de modificación
                try
                {
                    DOCUMENTOLOG dl = new DOCUMENTOLOG();
                    dl.NUM_DOC = d.NUM_DOC;
                    dl.TYPE_LINE = "M";
                    dl.TYPE = "E";
                    dl.MESSAGE = m;
                    dl.FECHA = DateTime.Now;
                    db.DOCUMENTOLOGs.Add(dl);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                }
                try
                {
                    d.ESTATUS_C = "B";
                    db.Entry(d).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        //MGC 10-12-2018 Firma del usuario cancelar-------------------------------------------------------------------------<

        //MGC 17-12-2018 Reprocesar Archivo preliminar-------------------------------------------------------------------------->
        [HttpPost]
        public ActionResult Reprocesar(decimal id)
        {

            DOCUMENTO d = db.DOCUMENTOes.Find(id);

            bool edit = false;
            ProcesaFlujo pf = new ProcesaFlujo();
            string fechacon = "";
            string nuevo = "";

            if (d != null)
            {
                //Obtener resultado si hay algún flujo o el estatus del flujo está p, para saber si es edit o solicitud nueva
                string rech = "";
                try
                {
                    rech = db.FLUJOes.Where(fl => fl.NUM_DOC == d.NUM_DOC && fl.ESTATUS == "R").FirstOrDefault().ESTATUS;
                }catch(Exception)
                {
                    rech = "";
                }

                if(d.ESTATUS_WF == "P" && rech == "R")
                {
                    //Edición
                    edit = true;
                }
                else
                {
                    //Nueva
                    nuevo = "N";
                }

                //Crear el archivo para el preliminar //MGC Preliminar
                string corr = pf.procesaPreliminar(d, edit, nuevo, false, fechacon);//MGC 10-12-2018 Firma del usuario cancelar//MGC-14-12-2018 Modificación fechacon

                //Se genero el preliminar
                if (corr == "0")
                {


                    //MGC 30-10-2018 Agregar mensaje a log de modificación
                    try
                    {
                        DOCUMENTOLOG dl = new DOCUMENTOLOG();

                        dl.NUM_DOC = d.NUM_DOC;
                        dl.TYPE_LINE = "M";
                        dl.TYPE = "S";
                        dl.MESSAGE = "Se generó el Archivo Preliminar";
                        dl.FECHA = DateTime.Now;

                        db.DOCUMENTOLOGs.Add(dl);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }
                    //MGC 30-10-2018 Agregar mensaje a log de modificación

                    //Actualizar wf del documento
                    //d.ESTATUS_WF = "A";//MGC 30-10-2018 Modificaión para validar creación del archivo

                    if (edit)
                    {
                        d.ESTATUS = "N"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                        d.ESTATUS_WF = "P"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                    }

                    //MGC 30-10-2018 Actualizar el estatus de preliminar del doc
                    d.ESTATUS_PRE = "G";//MGC 30-10-2018 Modificaión para validar creación del archivo
                    db.Entry(d).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //No se genero el preliminar
                else
                {
                    string m;
                    if (corr.Length > 50)
                    {
                        m = corr.Substring(0, 50);
                    }
                    else
                    {
                        m = corr;
                    }

                    //MGC 30-10-2018 Agregar mensaje a log de modificación
                    try
                    {
                        DOCUMENTOLOG dl = new DOCUMENTOLOG();

                        dl.NUM_DOC = d.NUM_DOC;
                        dl.TYPE_LINE = "M";
                        dl.TYPE = "E";
                        dl.MESSAGE = m;
                        dl.FECHA = DateTime.Now;

                        db.DOCUMENTOLOGs.Add(dl);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }
                    //MGC 30-10-2018 Agregar mensaje a log de modificación

                    //MGC 09-11-2018 Modificaión Estatus en la creación del archivo cuando la acción fue borrar-crear
                    //Si es edit, cambiar a P el flujo
                    //Y el estaus a N
                    if (edit)
                    {
                        d.ESTATUS = "N"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                        d.ESTATUS_WF = "P"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                    }



                    //MGC 30-10-2018 Actualizar el estatus de preliminar del doc
                    d.ESTATUS_PRE = "E";//MGC 30-10-2018 Modificaión Estatus en la creación del archivo
                    db.Entry(d).State = EntityState.Modified;
                    db.SaveChanges();

                    //Pendiente en edit, regresar a estatus de P en la creación del flujo

                }
            }

            return RedirectToAction("Index", "Home");
        }
        //MGC 17-12-2018 Reprocesar Archivo preliminar--------------------------------------------------------------------------<

    }
    public class TXTImp
    {
        public string spras_id { get; set; }
        public string imp { get; set; }
        public string txt { get; set; }
    }

    public class Anexo
    {
        public int a1 { get; set; }
        public int a2 { get; set; }
        public int a3 { get; set; }
        public int a4 { get; set; }
        public int a5 { get; set; }
    }

}
