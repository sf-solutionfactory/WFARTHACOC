using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using WFARTHA.Common;
using WFARTHA.Entities;
using WFARTHA.Models;
using WFARTHA.Services;

namespace WFARTHA.Controllers.Catalogos
{
    [Authorize]
    public class CadenaAutorizacionController : Controller
    {

        private WFARTHAEntities db = new WFARTHAEntities();



        // GET: UsuarioC
        public ActionResult Index()
        {
            int pagina = 711; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            var CadenaAutorizacion1 = db.DET_AGENTECCV;
            CadenaAutorizacionNuevo un = new CadenaAutorizacionNuevo();
            un.C = CadenaAutorizacion1.ToList();
            return View(un);


        }

        public ActionResult Details(int VERSION, string ID_RUTA_AGENTE)
        {
            int pagina = 712; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            var CadenaAutorizacion1 = db.DET_AGENTECAV.Where(a => a.VERSION == VERSION && a.ID_RUTA_AGENTE == ID_RUTA_AGENTE);
            ViewBag.version = VERSION;
            ViewBag.ruta = ID_RUTA_AGENTE;
            var usuarioc = db.DET_AGENTECCV.Where(c => c.VERSION == VERSION && c.ID_RUTA_AGENTE == ID_RUTA_AGENTE).FirstOrDefault().USUARIOC_ID;
            ViewBag.usuarioc = usuarioc;
            CadenaAutorizacion un = new CadenaAutorizacion();
            un.A = CadenaAutorizacion1.ToList();
            return View(un);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            int pagina = 713; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            CadenaAutorizacion obj = new CadenaAutorizacion();
            return View(obj);
        }

        // GET: Usuarios/Create
        public ActionResult Edit(int VERSION, string ID_RUTA_AGENTE)
        {

            int pagina = 714; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            var usuarioc = db.DET_AGENTECC.Where(c => c.VERSION == VERSION && c.ID_RUTA_AGENTE == ID_RUTA_AGENTE).FirstOrDefault().USUARIOC_ID;
            ViewBag.usuarioc = usuarioc;
            var CadenaAutorizacion1 = db.DET_AGENTECAV.Where(a => a.VERSION == VERSION && a.ID_RUTA_AGENTE == ID_RUTA_AGENTE);
            var step = CadenaAutorizacion1.Count();
            CadenaAutorizacion un = new CadenaAutorizacion();
            un.STEP_FASE = step + 1;
            un.A = CadenaAutorizacion1.ToList();
            return View(un);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_RUTA_AGENTE,VERSION,STEP_FASE_STEP_ACCION,LIM_SUP,AGENTE_SIG,OVERRIDE")] DET_AGENTECA dET_AGENTECA)
        {


            try
            {
                var CadenaAutorizacion1 = db.DET_AGENTECA.Where(a => a.VERSION == dET_AGENTECA.VERSION && a.ID_RUTA_AGENTE == dET_AGENTECA.ID_RUTA_AGENTE);
                var step = CadenaAutorizacion1.Count();
                var version = dET_AGENTECA.VERSION;
                var ruta = dET_AGENTECA.ID_RUTA_AGENTE;
                dET_AGENTECA.STEP_FASE = step + 1;
                dET_AGENTECA.STEP_ACCION = 1;
                dET_AGENTECA.LIM_SUP = 99999999999;
                db.DET_AGENTECA.Add(dET_AGENTECA);
                db.SaveChanges();

                return RedirectToAction("Details", new { VERSION = version, ID_RUTA_AGENTE = ruta });

            }
            catch (Exception e)
            {

            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "USUARIOC_ID,ID_RUTA_AGENTE,USUARIOA_ID")] CadenaAutorizacion cadenaAutorizacion)
        {


            var st = false;
            var version = 1;
            var ruta = cadenaAutorizacion.ID_RUTA_AGENTE;
            string displayName = null;
            var keyValue = db.DET_AGENTECC.FirstOrDefault(a => a.ID_RUTA_AGENTE == cadenaAutorizacion.ID_RUTA_AGENTE);

            if (keyValue != null)
            {
                var reg = db.DET_AGENTECC.Where(a => a.ID_RUTA_AGENTE == cadenaAutorizacion.ID_RUTA_AGENTE);
                version = reg.Count() + 1;
            }
            DateTime fechaR = DateTime.Today;




            try
            {
                DET_AGENTECC u = new DET_AGENTECC();
                u.VERSION = version;
                u.USUARIOC_ID = cadenaAutorizacion.USUARIOC_ID;
                u.ID_RUTA_AGENTE = cadenaAutorizacion.ID_RUTA_AGENTE;
                u.DESCRIPCION = "";
                u.FECHAC = fechaR;

                u.USUARIOA_ID = cadenaAutorizacion.USUARIOA_ID;

                db.DET_AGENTECC.Add(u);
                db.SaveChanges();

                DET_AGENTECA c = new DET_AGENTECA();
                c.ID_RUTA_AGENTE = cadenaAutorizacion.ID_RUTA_AGENTE;
                c.VERSION = version;
                c.STEP_FASE = 1;
                c.STEP_ACCION = 1;
                c.LIM_SUP = 99999999999;
                c.AGENTE_SIG = cadenaAutorizacion.USUARIOA_ID;

                db.DET_AGENTECA.Add(c);
                db.SaveChanges();

                return RedirectToAction("Details", new { VERSION = version, ID_RUTA_AGENTE = ruta });
                //return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.Error = "Ocurrio un error al taratar de ingresar registro";
                int pagina = 673; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
                CadenaAutorizacion obj = new CadenaAutorizacion();
                return View(obj);
            }

        }

        public ActionResult Delete(String ID_SOCIEDAD, String ID_USUARIO)
        {
            DET_APROB dET_APROB = db.DET_APROB.Find(1, ID_SOCIEDAD, ID_USUARIO);
            db.DET_APROB.Remove(dET_APROB);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



    }
}