using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    public class AppsettingController : Controller
    {

        private WFARTHAEntities db = new WFARTHAEntities();



        public ActionResult Index()
        {
            int pagina = 921; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            var appsetting = db.APPSETTINGs;
            AppsettingNuevo un = new AppsettingNuevo();
            un.L = appsetting.ToList();
            return View(un);
        }


       // GET: Usuarios/Create
        public ActionResult Create()
        {
            int pagina = 923; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            return View();
        }


        [HttpPost]

        public ActionResult Create([Bind(Include = "NOMBRE,VALUE,ACTIVO")] Appsetting appsetting)
        {
            APPSETTING u = new APPSETTING();
            string nOMBRE = appsetting.NOMBRE.Replace(" ", "_");

            u.NOMBRE = nOMBRE;
            u.VALUE = appsetting.VALUE;
            u.ACTIVO = true;

            db.APPSETTINGs.Add(u);

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch {
                ViewBag.Error = "El usuario ya existe. Introduzca un ID de usuario diferente";
                int pagina = 923; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
                return View();
            }

        }

        public ActionResult Delete(int ID )
        {
            APPSETTING aPPSETTING = db.APPSETTINGs.Find( ID);
            aPPSETTING.ID = aPPSETTING.ID;
            aPPSETTING.NOMBRE = aPPSETTING.NOMBRE;
            aPPSETTING.VALUE = aPPSETTING.VALUE;
            aPPSETTING.ACTIVO = false;
            db.Entry(aPPSETTING).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");


        }


        public ActionResult Edit(int ID)
        {
            int pagina = 924; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            APPSETTING aPPSETTING = db.APPSETTINGs.Find(ID);
            return View(aPPSETTING);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NOMBRE,VALUE,ACTIVO")] APPSETTING aPPSETTING)
        {
            string nOMBRE = aPPSETTING.NOMBRE.Replace(" ", "_");
            aPPSETTING.ID = aPPSETTING.ID;
            aPPSETTING.NOMBRE = nOMBRE;
            aPPSETTING.VALUE = aPPSETTING.VALUE;
            aPPSETTING.ACTIVO = true;
            db.Entry(aPPSETTING).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int ID)
        {
            int pagina = 924; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            APPSETTING aPPSETTING = db.APPSETTINGs.Find(ID);
            return View(aPPSETTING);
        }

    }
}