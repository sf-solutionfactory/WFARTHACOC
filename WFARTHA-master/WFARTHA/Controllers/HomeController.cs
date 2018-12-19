using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WFARTHA.Entities;
using WFARTHA.Models;
using ClosedXML.Excel;

using System.Web.Script.Serialization;

namespace WFARTHA.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private WFARTHAEntities db = new WFARTHAEntities();

        [Authorize]
        public ActionResult Index(string id)
        {
            using (WFARTHAEntities db = new WFARTHAEntities())
            {

                int pagina = 101; //ID EN BASE DE DATOS
                string u = User.Identity.Name;
                ////if (pais != null)
                ////    Session["pais"] = pais;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                try
                {
                    string p = Session["pr"].ToString();
                    ViewBag.PrSl = p;
                }
                catch
                {
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Proyectos", "Home", new { returnUrl = Request.Url.AbsolutePath });
                }
                Session["spras"] = user.SPRAS_ID;
                Session["pr"] = null;   //FRT06112018  Se agrega esta linea para poder ingresar nuevamente a la pantalla de proyectos

                Session["Edit"] = 0;//FRT08112018  Se agrega esta linea para poder ingresar nuevamente a la pantalla de proyectos
                Session["create"] = 0; //FRT14112018  Se agrega esta linea para poder ingresar nuevamente a la pantalla de proyectos

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

                try//Mensaje de documento creado
                {
                    string error_files = Session["ERROR_FILES"].ToString();
                    ViewBag.ERROR_FILES = error_files;
                    Session["ERROR_FILES"] = null;
                }
                catch
                {
                    ViewBag.ERROR_FILES = "";
                }
            }

            string us = "";
            DateTime fecha = DateTime.Now.Date;
            List<WFARTHA.Entities.DELEGAR> del = db.DELEGARs.Where(a => a.USUARIOD_ID.Equals(User.Identity.Name) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).ToList();

            if (del.Count > 0)
            {
                List<USUARIO> users = new List<USUARIO>();
                foreach (DELEGAR de in del)
                {
                    users.Add(de.USUARIO);
                }
                users.Add(ViewBag.usuario);
                ViewBag.delegados = users.ToList();

                if (id != null)
                    us = id;
                else
                    us = User.Identity.Name;
                ViewBag.usuariod = us;
            }
            else
                us = User.Identity.Name;

            //MGC Cancelar preliminar
            //MGC 26-10-2018 Modificación a borrador obtener todas las solicitudes, excepto el borrador a.ESTATUS != "B"
            //var dOCUMENTOes = db.DOCUMENTOes.Where(a => a.USUARIOC_ID.Equals(us) | a.USUARIOD_ID.Equals(us)).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.SOCIEDAD).Include(d => d.DOCUMENTOPREs).ToList();
            //var dOCUMENTOes = db.DOCUMENTOes.Where(a => (a.USUARIOC_ID.Equals(us) | a.USUARIOD_ID.Equals(us)) & a.ESTATUS != "B").Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.SOCIEDAD).Include(d => d.DOCUMENTOPREs).ToList();            
            //MGC 26-10-2018 Modificación a borrador obtener todas las solicitudes, excepto las solicitudes canceladas a.estatus_c != "C"  o contabilizadas a.ESTATUS != "A"
            var dOCUMENTOes = db.DOCUMENTOes.Where(a => (a.USUARIOC_ID.Equals(us) | a.USUARIOD_ID.Equals(us)) & (a.ESTATUS != "A" & a.ESTATUS_C != "C")).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.SOCIEDAD).Include(d => d.DOCUMENTOPREs).ToList();
            var dOCUMENTOVs = db.DOCUMENTOVs.Where(a => a.USUARIOA_ID.Equals(us)).ToList();

            var tsol = db.TSOLs.ToList();
            foreach (DOCUMENTOV v in dOCUMENTOVs)
            {
                DOCUMENTO d = new DOCUMENTO();
                var ppd = d.GetType().GetProperties();
                var ppv = v.GetType().GetProperties();
                foreach (var pv in ppv)
                {
                    foreach (var pd in ppd)
                    {
                        if (pd.Name == pv.Name)
                        {
                            pd.SetValue(d, pv.GetValue(v));
                            break;
                        }
                    }
                }
                d.TSOL = tsol.Where(a => a.ID.Equals(d.TSOL_ID)).FirstOrDefault();
                d.ID_PSPNR = db.DOCUMENTOes.Where(a => a.NUM_DOC.Equals(v.NUM_DOC)).FirstOrDefault().ID_PSPNR;
                d.FLUJOes = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).ToList();
                d.DOCUMENTOPREs = db.DOCUMENTOPREs.Where(docpr => docpr.NUM_DOC.Equals(d.NUM_DOC)).ToList();
                dOCUMENTOes.Add(d);

            }
            dOCUMENTOes = dOCUMENTOes.Distinct(new DocumentoComparer()).ToList();
            //dOCUMENTOes = dOCUMENTOes.OrderByDescending(a => a.FECHAC).OrderByDescending(a => a.NUM_DOC).ToList();//lejgg 05/12/18
            dOCUMENTOes = dOCUMENTOes.OrderBy(a => a.FECHAC).OrderBy(a => a.NUM_DOC).ToList();
            ViewBag.Proveedores = db.PROVEEDORs.ToList();


            return View(dOCUMENTOes);

        }
        //Lej 03.09.2018
        [Authorize]
        public ActionResult Proyectos(string returnUrl)
        {
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                int pagina = 102; //ID EN BASE DE DATOS
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                //ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.flag = true;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();


                Session["Edit"] = 0;///FRT20112018
                Session["create"] = 0;///FRT20112018

                //MGC 16-10-2018 Conociendo mi usuario, obtener las sociedades a las que tengo acceso
                List<DET_TIPOPRESUPUESTO> detsoc = new List<DET_TIPOPRESUPUESTO>();
                detsoc = db.DET_TIPOPRESUPUESTO.Where(tp => tp.ID_USER == user.ID).ToList();

                //MGC 16-10-2018 Obtener los proyectos de las sociedades -->
                //List<ASIGN_PROY_SOC> ps = new List<ASIGN_PROY_SOC>();//MGC 19-10-2018 Cambio en archivo
                var ps = (from det in detsoc//MGC 19-10-2018 Cambio en archivo
                          join aps in db.ASIGN_PROY_SOC //MGC 04-12-2018 Desde la vista se obtienen nada más las sociedades activas
                      on det.BUKRS equals aps.ID_BUKRS
                          select new
                          {
                              ID_PSPNR = aps.ID_PSPNR,
                              //ID_BUKRS = aps.ID_BUKRS//MGC 19-10-2018 Cambio en archivo
                          }
                      ).Distinct().ToList();


                var p = (from asig in ps
                         join pr in db.PROYECTOes.Where(pr => pr.ACTIVO == true)//MGC 04-12-2018 Desde la vista se obtienen nada más los proyectos activos
                         on asig.ID_PSPNR equals pr.ID_PSPNR
                         select new PROYECTO
                         {

                             ID_PSPNR = pr.ID_PSPNR,
                             NOMBRE = pr.NOMBRE
                         }).ToList();

                //var p = db.PROYECTOes.ToList();//MGC 16-10-2018 Obtener los proyectos de las sociedades
                ViewBag.returnUrl = returnUrl;
                return View(p);

                //MGC 16-10-2018 Obtener los proyectos de las sociedades <--
            }
            //return View();

        }

        [HttpGet]
        public ActionResult SelPais(string pais, string returnUrl)
        {
            Session["pais"] = pais.ToUpper();

            return Redirect(returnUrl);
        }

        //Lej 03.09.2018
        [HttpGet]
        public ActionResult SelProv(string prov, string id, string returnUrl)
        {
            Session["id_pr"] = id.ToUpper();
            Session["pr"] = prov.ToUpper();
            ViewBag.flag = false;
            return Redirect(returnUrl);
        }
    }
}