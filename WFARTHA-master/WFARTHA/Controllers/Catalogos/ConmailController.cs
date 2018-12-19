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
    public class ConmailController : Controller
    {

        private WFARTHAEntities db = new WFARTHAEntities();



        public ActionResult Index()
        {
            int pagina = 931; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            var conmail = db.CONMAILs;
            ConmailNuevo un = new ConmailNuevo();
            un.L = conmail.ToList();
            return View(un);
        }


       // GET: Usuarios/Create
        public ActionResult Create()
        {
            int pagina = 933; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            Conmail cONMAIL = new Conmail();
            return View(cONMAIL);
        }


        [HttpPost]

        public ActionResult Create([Bind(Include = "ID,HOST,PORT,SSL,MAIL,PASS")] Conmail conmail)
        {
            CONMAIL u = new CONMAIL();

            u.ID = conmail.ID;
            u.HOST = conmail.HOST;
            u.PORT = conmail.PORT;
            u.SSL = conmail.SSL;
            u.MAIL = conmail.MAIL;
            u.PASS = conmail.PASS;
            u.ACTIVO = true;



            db.CONMAILs.Add(u);

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch {
                ViewBag.Error = "El usuario ya existe. Introduzca un ID de usuario diferente";
                int pagina = 933; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
                Conmail cONMAIL = new Conmail();
                return View(cONMAIL);
            }

        }

        public ActionResult Delete(string ID )
        {
            CONMAIL cONMAIL = db.CONMAILs.Find( ID);
            cONMAIL.ID = cONMAIL.ID;
            cONMAIL.HOST = cONMAIL.HOST;
            cONMAIL.PORT = cONMAIL.PORT;
            cONMAIL.SSL = cONMAIL.SSL;
            cONMAIL.MAIL = cONMAIL.MAIL;
            cONMAIL.PASS = cONMAIL.PASS;
            cONMAIL.ACTIVO = false;
            db.Entry(cONMAIL).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");


        }


        public ActionResult Edit(string ID)
        {
            int pagina = 924; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            CONMAIL cONMAIL = db.CONMAILs.Find(ID);
            return View(cONMAIL);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,HOST,PORT,SSL,MAIL,PASS,ACTIVO")] CONMAIL cONMAIL)
        {
            if (Request.Form["ckb_ssl"] == "on")
            { cONMAIL.SSL = true;}
            else { cONMAIL.SSL = false; }

            if (Request.Form["ckb_activo"] == "on")
            { cONMAIL.ACTIVO =  true; }
            else { cONMAIL.ACTIVO = false; }


           

            cONMAIL.ID = cONMAIL.ID;
            cONMAIL.HOST = cONMAIL.HOST;
            cONMAIL.PORT = cONMAIL.PORT;
            cONMAIL.MAIL = cONMAIL.MAIL;
            cONMAIL.PASS = cONMAIL.PASS;
            
            db.Entry(cONMAIL).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(string ID)
        {
            int pagina = 933; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            CONMAIL cONMAIL = db.CONMAILs.Find(ID);
            return View(cONMAIL);
        }

    }
}