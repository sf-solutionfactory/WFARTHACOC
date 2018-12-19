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
    public class SociedadesUsuarioController : Controller
    {

        private WFARTHAEntities db = new WFARTHAEntities();

        // GET: SociedadesUsuario
        public ActionResult Index()
        {
            int pagina = 881; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            var dET_TIPOPRESUPUESTOVs = db.DET_TIPOPRESUPUESTOV;
            SociedadesUsuarioNuevo un = new SociedadesUsuarioNuevo();
            un.L = dET_TIPOPRESUPUESTOVs.ToList();
            return View(un);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            int pagina = 883; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            SociedadesUsuario obj = new SociedadesUsuario();
            return View(obj);
        }


        [HttpPost]
        public ActionResult Create([Bind(Include = "BUKRS,ID_USER,TIPOPRE")] SociedadesUsuario sociedadesUsuario)
        {
            DET_TIPOPRESUPUESTO u = new DET_TIPOPRESUPUESTO();
            u.BUKRS = sociedadesUsuario.BUKRS;
            u.ID_USER = sociedadesUsuario.ID_USER;
            u.TIPOPRE = sociedadesUsuario.TIPOPRE;
     

            db.DET_TIPOPRESUPUESTO.Add(u);

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {

                ViewBag.Error = "El usuario ya existe. Introduzca un ID de usuario diferente";
                int pagina = 723; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
                SociedadesUsuario obj = new SociedadesUsuario();
                return View(obj);

            }

        }


        public ActionResult Delete(String BUKRS, String ID_USER)
        {
            DET_TIPOPRESUPUESTO dET_TIPOPRESUPUESTO = db.DET_TIPOPRESUPUESTO.Find(BUKRS, ID_USER);
            db.DET_TIPOPRESUPUESTO.Remove(dET_TIPOPRESUPUESTO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Edit(String BUKRS, String ID_USER)
        {
            int pagina = 884; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            DET_TIPOPRESUPUESTO dET_TIPOPRESUPUESTO = db.DET_TIPOPRESUPUESTO.Find(BUKRS, ID_USER);
            return View(dET_TIPOPRESUPUESTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BUKRS,ID_USER,TIPOPRE")] DET_TIPOPRESUPUESTO dET_TIPOPRESUPUESTO)
        {

            dET_TIPOPRESUPUESTO.BUKRS = dET_TIPOPRESUPUESTO.BUKRS;
            dET_TIPOPRESUPUESTO.ID_USER = dET_TIPOPRESUPUESTO.ID_USER;
            dET_TIPOPRESUPUESTO.TIPOPRE = dET_TIPOPRESUPUESTO.TIPOPRE;

            db.Entry(dET_TIPOPRESUPUESTO).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }





    }
}