using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFARTHA.Entities;

namespace WFARTHA.Controllers
{
    public class MensajesController : Controller
    {
        private WFARTHAEntities db = new WFARTHAEntities();

        // GET: Mensajes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Mensajes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAMPOS cAMPOS = db.CAMPOS.Find(id);
            if (cAMPOS == null)
            {
                return HttpNotFound();
            }
            return View(cAMPOS);
        }

        // GET: Mensajes/Create
        public ActionResult Create()
        {
            ViewBag.PAGINA_ID = new SelectList(db.PAGINAs, "ID", "URL");
            return View();
        }

        // POST: Mensajes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PAGINA_ID,ID,DESCRIPCION,TIPO")] CAMPOS cAMPOS)
        {
            if (ModelState.IsValid)
            {
                db.CAMPOS.Add(cAMPOS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PAGINA_ID = new SelectList(db.PAGINAs, "ID", "URL", cAMPOS.PAGINA_ID);
            return View(cAMPOS);
        }

        // GET: Mensajes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAMPOS cAMPOS = db.CAMPOS.Find(id);
            if (cAMPOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.PAGINA_ID = new SelectList(db.PAGINAs, "ID", "URL", cAMPOS.PAGINA_ID);
            return View(cAMPOS);
        }

        // POST: Mensajes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PAGINA_ID,ID,DESCRIPCION,TIPO")] CAMPOS cAMPOS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cAMPOS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PAGINA_ID = new SelectList(db.PAGINAs, "ID", "URL", cAMPOS.PAGINA_ID);
            return View(cAMPOS);
        }

        // GET: Mensajes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAMPOS cAMPOS = db.CAMPOS.Find(id);
            if (cAMPOS == null)
            {
                return HttpNotFound();
            }
            return View(cAMPOS);
        }

        // POST: Mensajes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CAMPOS cAMPOS = db.CAMPOS.Find(id);
            db.CAMPOS.Remove(cAMPOS);
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
    }
}
