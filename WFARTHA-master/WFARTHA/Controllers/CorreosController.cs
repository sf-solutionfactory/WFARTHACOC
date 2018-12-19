using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFARTHA.Entities;
using WFARTHA.Services;

namespace WFARTHA.Controllers
{
    public class CorreosController : Controller
    {
        private WFARTHAEntities db = new WFARTHAEntities();

        // GET: Correos
        public ActionResult Index(decimal id, bool? mail) //B20180803 MGC Correos
        {
            var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
            var flujo = db.FLUJOes.Where(x => x.NUM_DOC == id).OrderByDescending(o => o.POS).Select(s => s.POS).ToList();
            ViewBag.Pos = flujo[0];
            ViewBag.url = "http://localhost:60621";
            ViewBag.url = "http://192.168.1.30";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            //ViewBag.miles = dOCUMENTOes.PAI.MILES;//LEJGG 090718
            //ViewBag.dec = dOCUMENTOes.PAI.DECIMAL;//LEJGG 090718

            //Obtener miles y dec
            FORMATO formato = new FORMATO();
            formato = db.FORMATOes.Where(f => f.ACTIVO == true).FirstOrDefault();
            ViewBag.miles = formato.MILES;
            ViewBag.dec = formato.DECIMALES;


            FormatosC fc = new FormatosC();
            ViewBag.monto = fc.toShow((decimal?)dOCUMENTO.MONTO_DOC_MD, formato.DECIMALES);
            if (mail == null)
                mail = true;
            //B20180803 MGC Correos............
            string mailv = "";
            if (mail != null)
            {
                if (mail == true)
                {
                    mailv = "X";
                }
            }

            ViewBag.mail = mailv;
            //B20180803 MGC Correos............

            //B20180803 MGC Presupuesto............
            //Models.PresupuestoModels carga = new Models.PresupuestoModels();
            ViewBag.ultMod = "";//carga.consultarUCarga();

            //MGC 08-10-2018 Obtener el nombre del cliente
            var prov = db.PROVEEDORs.Where(p => p.LIFNR == dOCUMENTO.PAYER_ID).FirstOrDefault().NAME1;
            ViewBag.prov = prov;


            //CLIENTE_MOD cli = new CLIENTE_MOD();

            //cli = SelectCliente(dOCUMENTO.PAYER_ID);

            ViewBag.kunnr = "";// cli.KUNNR;
            ViewBag.vtweg = "";// cli.VTWEG;

            Services.FormatosC format = new FormatosC();

            //PRESUPUESTO_MOD presu = new PRESUPUESTO_MOD();
            //presu = getPresupuesto(dOCUMENTO.PAYER_ID);

            //decimal pcanal = 0;
            //try
            //{
            //    pcanal = Convert.ToDecimal(presu.P_CANAL) / 1;
            //}
            //catch (Exception)
            //{

            //}
            //decimal pbanner = 0;
            //try
            //{
            //    pbanner = Convert.ToDecimal(presu.P_BANNER) / 1;
            //}
            //catch (Exception)
            //{

            //}
            //decimal pcc = 0;
            //try
            //{
            //    pcc = Convert.ToDecimal(presu.PC_C) / 1;
            //}
            //catch (Exception)
            //{

            //}
            //decimal pca = 0;
            //try
            //{
            //    pca = Convert.ToDecimal(presu.PC_A) / 1;
            //}
            //catch (Exception)
            //{

            //}
            //decimal pcp = 0;
            //try
            //{
            //    pcp = Convert.ToDecimal(presu.PC_P) / 1;
            //}
            //catch (Exception)
            //{

            //}
            //decimal pct = 0;
            //try
            //{
            //    pct = Convert.ToDecimal(presu.PC_T) / 1;
            //}
            //catch (Exception)
            //{

            //}
            //decimal consu = 0;
            //try
            //{
            //    consu = Convert.ToDecimal(presu.CONSU) / 1;
            //}
            //catch (Exception)
            //{

            //}
            ViewBag.pcan = "";//format.toShow(pcanal, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pban = "";//format.toShow(pbanner, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pcc = "";//format.toShow(pcc, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pca = "";//format.toShow(pca, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pcp = "";//format.toShow(pcp, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pct = "";//format.toShow(pct, dOCUMENTO.PAI.DECIMAL);
            ViewBag.consu = "";//format.toShow(consu, dOCUMENTO.PAI.DECIMAL);

            //B20180803 MGC Presupuesto............

            return View(dOCUMENTO);
        }

        // GET: Correos/Details/5
        public ActionResult Details(decimal id, bool? mail)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
            ViewBag.workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            ViewBag.acciones = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id) & a.ESTATUS.Equals("P") & a.USUARIOA_ID.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }

            //MGC 08-10-2018 Obtener el nombre del cliente
            var prov = db.PROVEEDORs.Where(p => p.LIFNR == dOCUMENTO.PAYER_ID).FirstOrDefault().NAME1;
            ViewBag.prov = prov;

            //Obtener miles y dec
            FORMATO formato = new FORMATO();
            formato = db.FORMATOes.Where(f => f.ACTIVO == true).FirstOrDefault();
            ViewBag.miles = formato.MILES;
            ViewBag.dec = formato.DECIMALES;


            //ViewBag.miles = dOCUMENTO.PAI.MILES;//LEJGG 090718
            //ViewBag.dec = dOCUMENTO.PAI.DECIMAL;//LEJGG 090718
            FormatosC fc = new FormatosC();
            ViewBag.monto = fc.toShow((decimal?)dOCUMENTO.MONTO_DOC_MD, formato.DECIMALES);
            if (mail == null)
                mail = true;
            //B20180803 MGC Correos............
            string mailv = "";
            if (mail != null)
            {
                if (mail == true)
                {
                    mailv = "X";
                }
            }

            ViewBag.mail = mailv;
            //B20180803 MGC Correos............
            return View(dOCUMENTO);
        }

        //// GET: Correos
        //public ActionResult Recurrente(decimal id, bool? mail)
        //{
        //    var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
        //    var flujo = db.FLUJOes.Where(x => x.NUM_DOC == id).OrderByDescending(o => o.POS).Select(s => s.POS).ToList();
        //    ViewBag.Pos = flujo[0];
        //    ViewBag.url = "http://localhost:64497";
        //    ViewBag.url = "http://192.168.1.77";
        //    ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");

        //    DOCUMENTOL dl = dOCUMENTO.DOCUMENTOLs.OrderByDescending(x => x.POS).FirstOrDefault();
        //    FormatosC fc = new FormatosC();
        //    ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL);
        //    ViewBag.mes = dl.FECHAF.Value.Month;
        //    ViewBag.venta = fc.toShow((decimal)dl.MONTO_VENTA, dOCUMENTO.PAI.DECIMAL);
        //    DOCUMENTOREC dr = db.DOCUMENTORECs.Where(x => x.DOC_REF == dOCUMENTO.NUM_DOC).FirstOrDefault();
        //    ViewBag.objetivo = fc.toShow((decimal)dr.MONTO_BASE, dOCUMENTO.PAI.DECIMAL);
        //    ViewBag.porc = fc.toShowPorc((decimal)dr.PORC, dOCUMENTO.PAI.DECIMAL);
        //    if (dl.MONTO_VENTA < dr.MONTO_BASE)
        //    {
        //        ViewBag.tsol = dOCUMENTO.TSOL.TSOLR;
        //        ViewBag.nota = false;
        //    }
        //    else
        //    {
        //        ViewBag.tsol = "";
        //        ViewBag.nota = true;
        //    }
        //    if (mail == null)
        //        mail = true;
        //    //B20180803 MGC Correos............
        //    string mailv = "";
        //    if (mail != null)
        //    {
        //        if (mail == true)
        //        {
        //            mailv = "X";
        //        }
        //    }

        //    ViewBag.mail = mailv;
        //    //B20180803 MGC Correos............
        //    return View(dOCUMENTO);
        //}

        //// GET: Correos
        //public ActionResult Backorder(decimal id, bool? mail)
        //{
        //    var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
        //    var flujo = db.FLUJOes.Where(x => x.NUM_DOC == id).OrderByDescending(o => o.POS).Select(s => s.POS).ToList();
        //    ViewBag.Pos = flujo[0];
        //    ViewBag.url = "http://localhost:64497";
        //    ViewBag.url = "http://192.168.1.77";
        //    ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");

        //    DOCUMENTOL dl = dOCUMENTO.DOCUMENTOLs.OrderByDescending(x => x.POS).FirstOrDefault();
        //    FormatosC fc = new FormatosC();
        //    ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL);
        //    ViewBag.mes = dl.FECHAF.Value.Month;
        //    ViewBag.venta = fc.toShow((decimal)dl.MONTO_VENTA, dOCUMENTO.PAI.DECIMAL);
        //    DOCUMENTOREC dr = db.DOCUMENTORECs.Where(x => x.DOC_REF == dOCUMENTO.NUM_DOC).FirstOrDefault();
        //    ViewBag.objetivo = fc.toShow((decimal)dr.MONTO_BASE, dOCUMENTO.PAI.DECIMAL);
        //    ViewBag.porc = fc.toShowPorc((decimal)dr.PORC, dOCUMENTO.PAI.DECIMAL);
        //    if (dl.MONTO_VENTA < dr.MONTO_BASE)
        //    {
        //        ViewBag.tsol = dOCUMENTO.TSOL.TSOLR;
        //        ViewBag.nota = false;
        //    }
        //    else
        //    {
        //        ViewBag.tsol = "";
        //        ViewBag.nota = true;
        //    }
        //    if (mail == null)
        //        mail = true;
        //    //B20180803 MGC Correos............
        //    string mailv = "";
        //    if (mail != null)
        //    {
        //        if (mail == true)
        //        {
        //            mailv = "X";
        //        }
        //    }

        //    ViewBag.mail = mailv;
        //    //B20180803 MGC Correos............
        //    return View(dOCUMENTO);
        //}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public DOCUMENTO SelectCliente(string kunnr)
        {

            //TAT001Entities db = new TAT001Entities();

            DOCUMENTO d = new DOCUMENTO();

            //CLIENTE_MOD id_cl = (from c in db.CLIENTEs
            //                     join co in db.CONTACTOCs
            //                     on new { c.VKORG, c.VTWEG, c.SPART, c.KUNNR } equals new { co.VKORG, co.VTWEG, co.SPART, co.KUNNR } into jjcont
            //                     from co in jjcont.DefaultIfEmpty()
            //                     where (c.KUNNR == kunnr & co.DEFECTO == true)
            //                     select new CLIENTE_MOD
            //                     {
            //                         VKORG = c.VKORG,
            //                         VTWEG = c.VTWEG,
            //                         VTWEG2 = c.VTWEG,//RSG 05.07.2018
            //                         SPART = c.SPART,//RSG 28.05.2018-------------------
            //                         NAME1 = c.NAME1,
            //                         KUNNR = c.KUNNR,
            //                         STCD1 = c.STCD1,
            //                         PARVW = c.PARVW,
            //                         BANNER = c.BANNER,
            //                         CANAL = c.CANAL,
            //                         PAYER_NOMBRE = co == null ? String.Empty : co.NOMBRE,
            //                         PAYER_EMAIL = co == null ? String.Empty : co.EMAIL,
            //                     }).FirstOrDefault();

            //if (id_cl == null)
            //{
            //    id_cl = (from c in db.CLIENTEs
            //             where (c.KUNNR == kunnr)
            //             select new CLIENTE_MOD
            //             {
            //                 VKORG = c.VKORG,
            //                 VTWEG = c.VTWEG,
            //                 VTWEG2 = c.VTWEG,//RSG 05.07.2018
            //                 SPART = c.SPART,//RSG 28.05.2018-------------------
            //                 NAME1 = c.NAME1,
            //                 KUNNR = c.KUNNR,
            //                 STCD1 = c.STCD1,
            //                 PARVW = c.PARVW,
            //                 BANNER = c.BANNER,
            //                 CANAL = c.CANAL,
            //                 PAYER_NOMBRE = String.Empty,
            //                 PAYER_EMAIL = String.Empty,
            //             }).FirstOrDefault();
            //}

            //if (id_cl != null)
            //{
            //    //Obtener el cliente
            //    //CANAL canal = db.CANALs.Where(ca => ca.BANNER == id_cl.BANNER && ca.KUNNR == kunnr).FirstOrDefault();
            //    CANAL canal = db.CANALs.Where(ca => ca.CANAL1 == id_cl.CANAL).FirstOrDefault();
            //    id_cl.VTWEG = "";
            //    //if (canal == null)
            //    //{
            //    //    string kunnrwz = kunnr.TrimStart('0');
            //    //    string bannerwz = id_cl.BANNER.TrimStart('0');
            //    //    canal = db.CANALs.Where(ca => ca.BANNER == bannerwz && ca.KUNNR == kunnrwz).FirstOrDefault();
            //    //}

            //    if (canal != null)
            //    {
            //        //id_cl.VTWEG = canal.CANAL1 + " - " + canal.CDESCRIPCION;
            //        id_cl.VTWEG = canal.CDESCRIPCION;
            //    }

            //    //Obtener el tipo de cliente
            //    var clientei = (from c in db.TCLIENTEs
            //                    join ct in db.TCLIENTETs
            //                    on c.ID equals ct.PARVW_ID
            //                    where c.ID == id_cl.PARVW && c.ACTIVO == true
            //                    select ct).FirstOrDefault();
            //    id_cl.PARVW = "";
            //    if (clientei != null)
            //    {
            //        id_cl.PARVW = clientei.TXT50;
            //    }

            //}

            //return id_cl;
            return d;
        }

        public DOCUMENTO getPresupuesto(string kunnr)
        {
            DOCUMENTO pm = new DOCUMENTO();
            //try
            //{
            //    if (kunnr == null)
            //        kunnr = "";

            //    //Obtener presupuesto
            //    string mes = DateTime.Now.Month.ToString();
            //    var presupuesto = db.CSP_PRESU_CLIENT(cLIENTE: kunnr, pERIODO: mes).Select(p => new { DESC = p.DESCRIPCION.ToString(), VAL = p.VALOR.ToString() }).ToList();
            //    string clien = db.CLIENTEs.Where(x => x.KUNNR == kunnr).Select(x => x.BANNERG).First();
            //    if (presupuesto != null)
            //    {
            //        if (String.IsNullOrEmpty(clien))
            //        {
            //            pm.P_CANAL = presupuesto[0].VAL;
            //            pm.P_BANNER = presupuesto[1].VAL;
            //            pm.PC_C = (float.Parse(presupuesto[4].VAL) + float.Parse(presupuesto[5].VAL) + float.Parse(presupuesto[6].VAL)).ToString();
            //            pm.PC_A = presupuesto[8].VAL;
            //            pm.PC_P = presupuesto[9].VAL;
            //            pm.PC_T = presupuesto[10].VAL;
            //            pm.CONSU = (float.Parse(presupuesto[1].VAL) - float.Parse(presupuesto[10].VAL)).ToString();
            //        }
            //        else
            //        {
            //            pm.P_CANAL = presupuesto[0].VAL;
            //            pm.P_BANNER = presupuesto[0].VAL;
            //            pm.PC_C = (float.Parse(presupuesto[4].VAL) + float.Parse(presupuesto[5].VAL) + float.Parse(presupuesto[6].VAL)).ToString();
            //            pm.PC_A = presupuesto[8].VAL;
            //            pm.PC_P = presupuesto[9].VAL;
            //            pm.PC_T = presupuesto[10].VAL;
            //            pm.CONSU = (float.Parse(presupuesto[0].VAL) - float.Parse(presupuesto[10].VAL)).ToString();
            //        }
            //    }
            //}
            //catch (Exception e)
            //{

            //}

            return pm;
        }
    }
}