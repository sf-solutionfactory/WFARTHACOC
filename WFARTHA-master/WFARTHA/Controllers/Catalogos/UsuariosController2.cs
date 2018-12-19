//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using WFARTHA.Entities;
//using WFARTHA.Models;

//namespace TAT001.Controllers.Catalogos
//{
//    [Authorize]
//    public class UsuariosController2 : Controller
//    {
//        private WFARTHAEntities db = new WFARTHAEntities();

//        // GET: Usuarios
//        public ActionResult Index()
//        {
//            int pagina = 601; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                //string u = "admin";
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            string spra = Session["spras"].ToString();
//            ViewBag.PUESTO_ID = new SelectList(db.PUESTOTs.Where(a => a.SPRAS_ID.Equals(spra)), "PUESTO_ID", "TXT50");
//            ViewBag.ROLs = new SelectList(db.ROLs, "ID", "ID");
//            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "DESCRIPCION");
//            ViewBag.BUNIT = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS");

//            var uSUARIOs = db.USUARIOs.Include(u => u.PUESTO).Include(u => u.SPRA);
//            UsuarioNuevo un = new UsuarioNuevo();
//            un.L = uSUARIOs;
//            return View(un);
//        }

//        // GET: Usuarios/Details/5
//        public ActionResult Details(string id)
//        {
//            int pagina = 603; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                //string u = "admin";
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                pagina = pagina - 1;
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            USUARIO uSUARIO = db.USUARIOs.Find(id);
//            if (uSUARIO == null)
//            {
//                return HttpNotFound();
//            }
//            //ViewBag.PUESTO_ID = new SelectList(db.PUESTOes, "ID", "ID", uSUARIO.PUESTO_ID);
//            string spra = Session["spras"].ToString();
//            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "ID", uSUARIO.SPRAS_ID);
//            ViewBag.PUESTO_ID = new SelectList(db.PUESTOTs.Where(a => a.SPRAS_ID.Equals(spra)), "PUESTO_ID", "TXT50", uSUARIO.PUESTO_ID);
//            ViewBag.BUNIT = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS", uSUARIO.BUNIT);
//            ViewBag.ROLES = db.ROLTs.Where(a => a.SPRAS_ID.Equals(spra));
//            ViewBag.SOCIEDADES = db.SOCIEDADs;
//            ViewBag.PAISES = db.PAIS.Where(a=>a.SOCIEDAD_ID != null & a.ACTIVO == true).ToList();
//            ViewBag.APROBADORES = db.DET_APROB.Where(a => a.BUKRS.Equals("KCMX") & a.PUESTOC_ID == uSUARIO.PUESTO_ID).ToList();
//            return View(uSUARIO);
//        }

//        // GET: Usuarios/Create
//        public ActionResult Create()
//        {
//            int pagina = 602; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                //string u = "admin";
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            string spra = Session["spras"].ToString();
//            ViewBag.PUESTO_ID = new SelectList(db.PUESTOTs.Where(a => a.SPRAS_ID.Equals(spra)), "PUESTO_ID", "TXT50");
//            ViewBag.ROLs = new SelectList(db.ROLTs.Where(a => a.SPRAS_ID.Equals(spra)), "ROL_ID", "TXT50");
//            //ViewBag.ROLs = new SelectList(db.ROLs, "ID", "ID");
//            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "DESCRIPCION");
//            ViewBag.BUNIT = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS");
//            return View();
//        }

//        // POST: Usuarios/Create
//        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
//        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "ID,PASS,NOMBRE,APELLIDO_P,APELLIDO_M,EMAIL,SPRAS_ID,ACTIVO,PUESTO_ID,MANAGER,BACKUP_ID,BUNIT,ROL")] Usuario uSUARIO)
//        {
//            if (ModelState.IsValid)
//            {
//                if (uSUARIO.PASS != null & uSUARIO.MANAGER != null)
//                {
//                    if (uSUARIO.PASS != "" & uSUARIO.MANAGER != "")
//                    {
//                        if (uSUARIO.PASS == uSUARIO.MANAGER)
//                        {
//                            Cryptography c = new Cryptography();
//                            uSUARIO.PASS = c.Encrypt(uSUARIO.PASS);
//                            USUARIO u = new USUARIO();
//                            var ppd = u.GetType().GetProperties();
//                            var ppv = uSUARIO.GetType().GetProperties();
//                            foreach (var pv in ppv)
//                            {
//                                foreach (var pd in ppd)
//                                {
//                                    if (pd.Name == pv.Name)
//                                    {
//                                        pd.SetValue(u, pv.GetValue(uSUARIO));
//                                        break;
//                                    }
//                                }
//                            }
//                            db.USUARIOs.Add(u);

//                            ////MIEMBRO m = new MIEMBRO();
//                            ////m.ROL_ID = int.Parse(Request.Form["ROLs"].ToString());
//                            ////m.USUARIO_ID = uSUARIO.ID;
//                            ////m.ACTIVO = true;
//                            ////db.MIEMBROS.Add(m);

//                            db.SaveChanges();
//                            return RedirectToAction("Index");
//                        }
//                        else
//                        {
//                            ViewBag.Error = "La contraseña no coincide";
//                        }
//                    }
//                }
//            }

//            int pagina = 602; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            string spra = Session["spras"].ToString();
//            ViewBag.PUESTO_ID = new SelectList(db.PUESTOTs.Where(a => a.SPRAS_ID.Equals(spra)), "PUESTO_ID", "TXT50");
//            ViewBag.ROLs = new SelectList(db.ROLTs.Where(a => a.SPRAS_ID.Equals(spra)), "ROL_ID", "TXT50");
//            //ViewBag.ROLs = new SelectList(db.ROLs, "ID", "ID");
//            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "DESCRIPCION");
//            ViewBag.BUNIT = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS");

//            return View(uSUARIO);
//        }

//        // GET: Usuarios/Edit/5
//        public ActionResult Edit(string id)
//        {
//            int pagina = 603; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                //string u = "admin";
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                pagina = pagina - 1;
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            USUARIO uSUARIO = db.USUARIOs.Find(id);
//            if (uSUARIO == null)
//            {
//                return HttpNotFound();
//            }
//            //ViewBag.PUESTO_ID = new SelectList(db.PUESTOes, "ID", "ID", uSUARIO.PUESTO_ID);
//            string spra = Session["spras"].ToString();
//            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "ID", uSUARIO.SPRAS_ID);
//            ViewBag.PUESTO_ID = new SelectList(db.PUESTOTs.Where(a => a.SPRAS_ID.Equals(spra)), "PUESTO_ID", "TXT50", uSUARIO.PUESTO_ID);
//            ViewBag.BUNIT = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS", uSUARIO.BUNIT);
//            ViewBag.ROLES = db.ROLTs.Where(a => a.SPRAS_ID.Equals(spra));
//            ViewBag.SOCIEDADES = db.SOCIEDADs;
//            ViewBag.PAISES = db.PAIS;
//            ViewBag.APROBADORES = db.DET_APROB.Where(a => a.BUKRS.Equals("KCMX") & a.PUESTOC_ID == uSUARIO.PUESTO_ID).ToList();
//            return View(uSUARIO);
//        }

//        // POST: Usuarios/Edit/5
//        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
//        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "ID,PASS,NOMBRE,APELLIDO_P,APELLIDO_M,EMAIL,SPRAS_ID,ACTIVO,PUESTO_ID,MANAGER,BACKUP_ID,BUNIT")] USUARIO uSUARIO)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(uSUARIO).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Details", new { id = uSUARIO.ID });
//                //return RedirectToAction("Index");
//            }
//            int pagina = 603; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                //string u = "admin";
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                pagina = pagina - 1;
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            string spra = Session["spras"].ToString();
//            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "ID", uSUARIO.SPRAS_ID);
//            ViewBag.PUESTO_ID = new SelectList(db.PUESTOTs.Where(a => a.SPRAS_ID.Equals(spra)), "PUESTO_ID", "TXT50", uSUARIO.PUESTO_ID);
//            ViewBag.BUNIT = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS", uSUARIO.BUNIT);
//            ViewBag.ROLES = db.ROLTs.Where(a => a.SPRAS_ID.Equals(spra));
//            return View(uSUARIO);
//        }

//        // GET: Usuarios/Delete/5
//        public ActionResult Delete(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            USUARIO uSUARIO = db.USUARIOs.Find(id);
//            if (uSUARIO == null)
//            {
//                return HttpNotFound();
//            }
//            return View(uSUARIO);
//        }

//        // POST: Usuarios/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(string id)
//        {
//            USUARIO uSUARIO = db.USUARIOs.Find(id);
//            db.USUARIOs.Remove(uSUARIO);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }


//        // GET: Usuarios/Edit/5
//        public ActionResult Pass(string id)
//        {
//            int pagina = 604; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                //string u = "admin";
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Pass uSUARIO = new Pass();
//            uSUARIO.ID = id;
//            return View(uSUARIO);
//        }
//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult Pass(/*[Bind(Include = "ID,pass,npass1,npass2")] */Pass pp)
//        {
//            Pass pass = new Pass();
//            pass.ID = Request.Form.Get("ID");
//            pass.pass = Request.Form.Get("pass");
//            pass.npass1 = Request.Form.Get("npass1");
//            pass.npass2 = Request.Form.Get("npass2");
//            USUARIO us = db.USUARIOs.Find(pass.ID);
//            Cryptography c = new Cryptography();
//            string pass_a = c.Decrypt(us.PASS);
//            if (pass.pass.Equals(pass_a))
//            {
//                if (pass.npass1.Equals(pass.npass2))
//                {
//                    if (ModelState.IsValid)
//                    {
//                        us.PASS = c.Encrypt(pass.npass1);
//                        db.Entry(us).State = EntityState.Modified;
//                        db.SaveChanges();
//                        return RedirectToAction("Index");
//                    }
//                }
//                else
//                {
//                    ViewBag.message = "Los datos no coinciden";
//                }
//            }
//            else
//            {
//                ViewBag.message = "Los datos no coinciden";
//            }
//            int pagina = 604; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                //string u = "admin";
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            return View(pass);
//        }
//        public ActionResult AgregarRol(string id)
//        {
//            int pagina = 603; //ID EN BASE DE DATOS
//            using (WFARTHAEntities db = new WFARTHAEntities())
//            {
//                string u = User.Identity.Name;
//                //string u = "admin";
//                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
//                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
//                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
//                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
//                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
//                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
//                pagina = pagina - 1;
//                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

//                try
//                {
//                    string p = Session["pais"].ToString();
//                    ViewBag.pais = p + ".png";
//                }
//                catch
//                {
//                    //ViewBag.pais = "mx.png";
//                    //return RedirectToAction("Pais", "Home");
//                }
//                Session["spras"] = user.SPRAS_ID;
//            }
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            USUARIO uSUARIO = db.USUARIOs.Find(id);
//            if (uSUARIO == null)
//            {
//                return HttpNotFound();
//            }
//            //ViewBag.PUESTO_ID = new SelectList(db.PUESTOes, "ID", "ID", uSUARIO.PUESTO_ID);
//            string spra = Session["spras"].ToString();
//            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "ID", uSUARIO.SPRAS_ID);
//            ViewBag.PUESTO_ID = new SelectList(db.PUESTOTs.Where(a => a.SPRAS_ID.Equals(spra)), "PUESTO_ID", "TXT50", uSUARIO.PUESTO_ID);
//            ViewBag.BUNIT = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS", uSUARIO.BUNIT);
//            ViewBag.ROLES = db.ROLTs.Where(a => a.SPRAS_ID.Equals(spra));
//            ViewBag.SOCIEDADES = db.SOCIEDADs;
//            ViewBag.PAISES = db.PAIS;
//            return View(uSUARIO);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult AgregarRol(USUARIO u)
//        {
//            int rol = Int32.Parse(Request.Form["txt_rol"].ToString());
//            string soc = Request.Form["txt_soc"].ToString();
//            string pais = Request.Form["txt_pai"].ToString();

//            MIEMBRO m = db.MIEMBROS.Where(a => a.USUARIO_ID.Equals(u.ID) & a.ROL_ID == rol).FirstOrDefault();
//            if (m == null)
//            {
//                m = new MIEMBRO();
//                m.ROL_ID = rol;
//                m.USUARIO_ID = u.ID;
//                m.ACTIVO = true;
//                db.MIEMBROS.Add(m);
//            }

//            //List<DET_APROB> dd = db.DET_APROB.Where(a => a.PUESTOC_ID == u.PUESTO_ID & a.BUKRS.Equals(soc)).ToList();
//            GAUTORIZACION ga = new GAUTORIZACION();
//            ga.LAND = pais;
//            ga.BUKRS = soc;
//            ga.CLAVE = pais;
//            ga.NOMBRE = soc;
//            //db.GAUTORIZACIONs.Add(ga);
//            USUARIO user = db.USUARIOs.Find(u.ID);
//            user.GAUTORIZACIONs.Add(ga);
//            db.Entry(user).State = EntityState.Modified;
//            db.SaveChanges();

//            ////foreach (DET_APROB d in dd)
//            ////{
//            ////    DET_AGENTE da = new DET_AGENTE();
//            ////    da.PUESTOC_ID = (int)u.PUESTO_ID;
//            ////    da.USUARIOC = u.ID;
//            ////    da.POS = d.POS;
//            ////    da.PUESTOA_ID = d.PUESTOA_ID;
//            ////    da.USUARIOA = Request.Form["txt_p-" + d.PUESTOA_ID].ToString();
//            ////    da.ACTIVO = true;
//            ////    da.AGROUP_ID = ga.ID;
//            ////    da.MONTO = d.MONTO;
//            ////    da.PRESUPUESTO = d.PRESUPUESTO;
//            ////    db.DET_AGENTE.Add(da);


//            ////    string us = Request.Form["txt_p-" + d.PUESTOA_ID].ToString();
//            ////    USUARIO uu = db.USUARIOs.Find(us);
//            ////    uu.GAUTORIZACIONs.Add(ga);
//            ////    db.Entry(uu).State = EntityState.Modified;

//            ////    MIEMBRO mi = db.MIEMBROS.Where(a => a.USUARIO_ID.Equals(uu.ID) & a.ROL_ID == 2).FirstOrDefault();
//            ////    if (mi == null)
//            ////    {
//            ////        mi = new MIEMBRO();
//            ////        mi.ROL_ID = 2;
//            ////        mi.USUARIO_ID = uu.ID;
//            ////        mi.ACTIVO = true;
//            ////        db.MIEMBROS.Add(mi);
//            ////    }

//            ////    db.SaveChanges();
//            ////}
//            //string rol = Request.Form["txt_p-3"].ToString();

//            DET_APROBH dh = db.DET_APROBH.Where(a => a.SOCIEDAD_ID.Equals(soc) & a.PUESTOC_ID == u.PUESTO_ID).OrderByDescending(a => a.VERSION).FirstOrDefault();
//            if (dh != null)
//            {
//                DET_AGENTEH dah = new DET_AGENTEH();
//                dah.SOCIEDAD_ID = dh.SOCIEDAD_ID;
//                dah.PUESTOC_ID = (int)u.PUESTO_ID;
//                dah.VERSION = dh.VERSION;
//                dah.AGROUP_ID = (int)ga.ID;
//                dah.USUARIOC_ID = u.ID;
//                dah.ACTIVO = true;
//                db.DET_AGENTEH.Add(dah);
//                db.SaveChanges();

//                List<DET_APROBP> ddp = db.DET_APROBP.Where(a => a.SOCIEDAD_ID.Equals(soc) & a.PUESTOC_ID == u.PUESTO_ID & a.VERSION == dh.VERSION).ToList();
//                foreach (DET_APROBP dp in ddp)
//                {
//                    DET_AGENTEP dap = new DET_AGENTEP();
//                    dap.SOCIEDAD_ID = dah.SOCIEDAD_ID;
//                    dap.PUESTOC_ID = dah.PUESTOC_ID;
//                    dap.VERSION = dah.VERSION;
//                    dap.AGROUP_ID = dah.AGROUP_ID;
//                    dap.POS = dp.POS;
//                    dap.PUESTOA_ID = dp.PUESTOA_ID;
//                    dap.USUARIOA_ID = Request.Form["txt_p-" + dp.PUESTOA_ID].ToString();
//                    dap.MONTO = dp.MONTO;
//                    dap.PRESUPUESTO = dp.PRESUPUESTO;
//                    dap.ACTIVO = true;
//                    dah.DET_AGENTEP.Add(dap);
//                    //dgp.Add(dap);

//                    ////string us = dap.USUARIOA_ID;
//                    ////USUARIO uu = db.USUARIOs.Find(us);
//                    ////uu.GAUTORIZACIONs.Add(ga);
//                    ////db.Entry(uu).State = EntityState.Modified;

//                    ////MIEMBRO mi = db.MIEMBROS.Where(a => a.USUARIO_ID.Equals(uu.ID) & a.ROL_ID == 2).FirstOrDefault();
//                    ////if (mi == null)
//                    ////{
//                    ////    mi = new MIEMBRO();
//                    ////    mi.ROL_ID = 2;
//                    ////    mi.USUARIO_ID = uu.ID;
//                    ////    mi.ACTIVO = true;
//                    ////    db.MIEMBROS.Add(mi);
//                    ////}

//                }

//                TAX_LAND tl = db.TAX_LAND.Where(a => a.SOCIEDAD_ID.Equals(soc)& a.ACTIVO == true).FirstOrDefault();
//                if (tl != null)
//                {
//                    DET_TAXEO dt = new DET_TAXEO();
//                    dt.SOCIEDAD_ID = soc;
//                    dt.PAIS_ID = pais;
//                    dt.PUESTOC_ID = dah.PUESTOC_ID;
//                    dt.USUARIOC_ID = dah.USUARIOC_ID;
//                    dt.VERSION = dah.VERSION;
//                    dt.PUESTOA_ID = 9;
//                    dt.USUARIOA_ID = Request.Form["txt_p-9"].ToString();
//                    dt.ACTIVO = true;
//                    db.DET_TAXEO.Add(dt);
//                }

//                db.SaveChanges();
//            }
//            return RedirectToAction("Details", new { id = u.ID });

//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult ModificarRol(USUARIO u)
//        {
//            //int rol = Int32.Parse(Request.Form["txt_rol"].ToString());
//            string pais = Request.Form["item.PAIS_ID"].ToString();
//            string vkorg = Request.Form["item.VKORG"].ToString();
//            string vtweg = Request.Form["item.VTWEG"].ToString();
//            string spart = Request.Form["item.SPART"].ToString();
//            string kunnr = Request.Form["item.KUNNR"].ToString();

//            //DET_AGENTEH dh = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(soc) & a.PUESTOC_ID == u.PUESTO_ID & a.AGROUP_ID == (agroup)).OrderByDescending(a => a.VERSION).FirstOrDefault();
//            DET_AGENTEC dh = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(u.ID) & a.PAIS_ID.Equals(pais) & a.VKORG.Equals(vkorg) & a.VTWEG.Equals(vtweg) & a.SPART.Equals(spart) & a.KUNNR.Equals(kunnr) & a.POS == 1 & a.ACTIVO == true).OrderByDescending(a => a.VERSION).FirstOrDefault();
//            if (dh != null)
//            {

//                List<DET_AGENTEC> ddp = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(u.ID) & a.PAIS_ID.Equals(pais) & a.VKORG.Equals(vkorg) & a.VTWEG.Equals(vtweg)
//                                        & a.SPART.Equals(spart) & a.KUNNR.Equals(kunnr) & a.VERSION == dh.VERSION & a.ACTIVO == true).ToList();
//                foreach (DET_AGENTEC dp in ddp)
//                {
//                    //DET_AGENTEP dap = new DET_AGENTEP();
//                    //dap.SOCIEDAD_ID = dh.SOCIEDAD_ID;
//                    //dap.PUESTOC_ID = dh.PUESTOC_ID;
//                    //dap.VERSION = dh.VERSION;
//                    //dap.AGROUP_ID = dh.AGROUP_ID;
//                    //dap.POS = dp.POS;
//                    //dap.PUESTOA_ID = dp.PUESTOA_ID;
//                    dp.USUARIOA_ID = Request.Form[pais + "-" + kunnr + "-" + dp.POS].ToString();
//                    try
//                    {
//                        string isMonto = Request.Form[pais + "-" + kunnr + "-" + dp.POS + "-IsMonto"].ToString();
//                        string monto = Request.Form[pais + "-" + kunnr + "-" + dp.POS + "-monto"].ToString();
//                        if (monto != "")
//                            dp.MONTO = decimal.Parse(monto);
//                        else
//                            dp.MONTO = null;
//                    }
//                    catch
//                    {
//                        dp.MONTO = null;
//                    }
//                    try
//                    {
//                        string presu = Request.Form[pais + "-" + kunnr + "-" + dp.POS + "-presup"].ToString();
//                        if (presu == "on")
//                            dp.PRESUPUESTO = true;
//                        else
//                            dp.PRESUPUESTO = false;
//                    }
//                    catch
//                    {
//                        dp.PRESUPUESTO = false;
//                    }
//                    dp.MONTO = dp.MONTO;
//                    dp.PRESUPUESTO = dp.PRESUPUESTO;
//                    dp.ACTIVO = true;
//                    db.Entry(dp).State = EntityState.Modified;

//                }
//                db.SaveChanges();
//            }
//            return RedirectToAction("Details", new { id = u.ID });

//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}