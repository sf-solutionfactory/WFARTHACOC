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

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    public class ProyectoSociedadController : Controller
    {

        private WFARTHAEntities db = new WFARTHAEntities();
        // GET: ProyectoSociedad
        public ActionResult Index()
        {
            int pagina = 721; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            var dET_PROYECTO_DEC_V = db.DET_PROYECTO_DEC_V;
            ProyectoSociedadNuevo un = new ProyectoSociedadNuevo();
            un.L = dET_PROYECTO_DEC_V.ToList();
            return View(un);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            int pagina = 723; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            ProyectoSociedad obj = new ProyectoSociedad();
            return View(obj);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "ID_PSPNR,ID_BUKRS")] ProyectoSociedad proyectoSociedad)
        {
            DET_PROYECTOV u = new DET_PROYECTOV();
            u.ID_PSPNR = proyectoSociedad.ID_PSPNR;
            u.ID_BUKRS = proyectoSociedad.ID_BUKRS;

            db.DET_PROYECTOV.Add(u);

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
                ProyectoSociedad obj = new ProyectoSociedad();
                return View(obj);
            }


        }

        public ActionResult Delete(String ID_PSPNR, String ID_BUKRS)
        {
            DET_PROYECTOV dET_PROYECTOV = db.DET_PROYECTOV.Find(ID_PSPNR, ID_BUKRS);
            db.DET_PROYECTOV.Remove(dET_PROYECTOV);
            db.SaveChanges();
            return RedirectToAction("Index");
          
        }
    }
}