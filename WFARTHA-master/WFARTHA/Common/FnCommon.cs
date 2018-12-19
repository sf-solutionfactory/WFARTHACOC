using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WFARTHA.Entities;
using WFARTHA.Models;

namespace WFARTHA.Common
{
    public static class FnCommon
    {
        public static USUARIO ObtenerUsuario(WFARTHAEntities db, string user_id)
        {
            return db.USUARIOs.Where(a => a.ID.Equals(user_id)).FirstOrDefault();
        }
        public static string ObtenerSprasId(WFARTHAEntities db, string user_id)
        {
            return db.USUARIOs.Where(a => a.ID.Equals(user_id)).FirstOrDefault().SPRAS_ID;
        }
        public static string ObtenerTextoMnj(WFARTHAEntities db, int pagina_id, string campo_id, string user_id)
        {
            string spras_id = ObtenerSprasId(db, user_id);
            string texto = "";
            if (db.TEXTOes.Any(a => (a.PAGINA_ID.Equals(pagina_id) && a.SPRAS_ID.Equals(spras_id) && a.CAMPO_ID.Equals(campo_id))))
            {
                texto = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina_id) && a.SPRAS_ID.Equals(spras_id) && a.CAMPO_ID.Equals(campo_id))).FirstOrDefault().TEXTOS;
            }
            return texto;
        }
        public static void ObtenerTextos(WFARTHAEntities db, int pagina_id_textos, string user_id, ControllerBase controller)
        {
            var user = ObtenerUsuario(db, user_id);
            controller.ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina_id_textos) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

        }
        public static void ObtenerConfPage(WFARTHAEntities db, int pagina_id, string user_id, ControllerBase controller, int? pagina_id_textos = null)
        {
            var user = ObtenerUsuario(db, user_id);
            controller.ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            controller.ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            controller.ViewBag.usuario = user;
            controller.ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            controller.ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina_id)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            if (pagina_id_textos != null)
            {
                pagina_id = pagina_id_textos.Value;
            }
            controller.ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina_id) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            controller.ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina_id) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            controller.ViewBag.spras_id = user.SPRAS_ID;
        }
        public static List<SelectListItem> ObtenerCmbSociedades(WFARTHAEntities db, string id)
        {
            return db.SOCIEDADs
                .Where(x => (x.BUKRS == id || id == null) && x.ACTIVO)
                .Select(x => new SelectListItem
                {
                    Value = x.BUKRS,
                    Text = x.BUKRS
                }).ToList();
        }
        //MGC 24-10-2018 Usuarios
        //public static List<SelectListItem> ObtenerCmbPeriodos(WFARTHAEntities db, string spras_id, int? id)
        //{
        //    return db.PERIODOes
        //        .Join(db.PERIODOTs, p => p.ID, pt => pt.PERIODO_ID, (p, pt) => pt)
        //        .Where(x => x.SPRAS_ID == spras_id && (x.PERIODO_ID == id || id == null))
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.PERIODO_ID.ToString(),
        //            Text = (x.PERIODO_ID.ToString() + " - " + x.TXT50)
        //        }).ToList();
        //}
        public static List<SelectListItem> ObtenerCmbUsuario(WFARTHAEntities db, string id)
        {
            return db.USUARIOs
                .Where(x => (x.ID == id || id == null) && (x.ACTIVO == null ? false : x.ACTIVO.Value))
                .Select(x => new SelectListItem
                {
                    Value = x.ID,
                    Text = (x.ID +" - "+x.NOMBRE + " " + x.APELLIDO_P + " " + (x.APELLIDO_M == null ? "" : x.APELLIDO_M))
                }).ToList();
        }
        //MGC 24-10-2018 Usuarios
        //public static List<SelectListItem> ObtenerCmbTabs(WFARTHAEntities db, string spras_id,bool? activo, string id)
        //{
        //    return db.TABs.Where(x=>x.TAB_CAMPO.Any(y=>y.ACTIVO == activo.Value || activo == null))
        //        .Where(x=>(x.ACTIVO==activo.Value || activo==null))
        //        .Join(db.TEXTOes, ta => ta.ID, te => te.CAMPO_ID, (ta, te) => te)
        //        .Where(x => x.SPRAS_ID == spras_id && x.PAGINA_ID == 202 && (x.CAMPO_ID == id || id == null))
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.CAMPO_ID,
        //            Text = x.TEXTOS
        //        }).ToList();
        //}
        //public static List<SelectListItem> ObtenerCmbCamposPoTabId(WFARTHAEntities db, string spras_id, string tab_id,bool? activo, string id)
        //{
        //    return db.TAB_CAMPO
        //            .Where(x => x.TAB_ID == tab_id && (x.ACTIVO == activo.Value || activo == null))
        //            .Join(db.TEXTOes, tc => tc.CAMPO_ID, te => te.CAMPO_ID, (ta, te) => te)
        //            .Where(x => x.SPRAS_ID == spras_id && x.PAGINA_ID == 202 && (x.CAMPO_ID == id || id == null))
        //            .Select(x => new SelectListItem
        //            {
        //                Value = x.CAMPO_ID,
        //                Text = x.TEXTOS
        //            }).ToList();
        //}
        //public static List<SelectListItem> ObtenerCmbTiposSolicitud(WFARTHAEntities db, string spras_id, string id,bool? esReversa = false)
        //{
        //    List<TSOL> tiposSolicitudes = new List<TSOL>();
        //    return  db.TSOLs
        //        .Where(x => ((esReversa.Value == true && x.TSOLR == null) || esReversa.Value == false) && (id == null || x.ID == id)&& (x.ACTIVO==null?false: x.ACTIVO.Value))
        //        .Join(db.TSOLTs, s => s.ID, st => st.TSOL_ID, (s, st) => st)
        //        .Where(x => x.SPRAS_ID == spras_id)
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.TSOL_ID,
        //            Text = (x.TSOL_ID + " - " + x.TXT50)
        //        }).ToList();
        //}
        //public static List<SelectTreeItem> ObtenerTreeTiposSolicitud(WFARTHAEntities db, string spras_id, string tipo = null,bool? esReversa=false)
        //{
        //    // tipo
        //    // SD = Solicitud directa
        //    // SR = Solicitud relacionada

        //    List<SelectTreeItem> tree = new List<SelectTreeItem>();
        //    if (esReversa.Value)
        //    {
        //        tree.Add(new SelectTreeItem
        //        {
        //            text = "-",
        //            expanded = false,
        //            items = ObtenerItemsTSOLT(db, "", "", spras_id, esReversa)
        //        });
        //    }
        //    else
        //    {
        //        db.TSOL_GROUP
        //            .Where(x => x.ID_PADRE == null && x.TIPO_PADRE == null && (tipo == x.TIPO || tipo == null))
        //            .Join(db.TSOL_GROUPT, tg => new { ID = tg.ID, TIPO = tg.TIPO }, tgt => new { ID = tgt.TSOL_GROUP_ID, TIPO = tgt.TSOL_GROUP_TIPO }, (tg, tgt) => tgt)
        //            .Where(x => x.SPRAS_ID == spras_id).OrderBy(x => x.TSOL_GROUP_ID)
        //            .ToList().ForEach(x =>
        //            {
        //                SelectTreeItem item = new SelectTreeItem
        //                {
        //                    text = x.TXT50,
        //                    expanded = false,
        //                    items = ObtenerItemsSelectTree(db, x.TSOL_GROUP_ID, x.TSOL_GROUP_TIPO, spras_id)
        //                };
        //                if (item.items.Any())
        //                {
        //                    tree.Add(item);
        //                }
        //            });
        //    }
        //    return tree;
        //}
        
        //static List<SelectTreeItem> ObtenerItemsSelectTree(WFARTHAEntities db, string id_padre, string tipo_padre, string spras_id)
        //{
        //    List<SelectTreeItem> items = new List<SelectTreeItem>();
        //    db.TSOL_GROUP
        //        .Where(x => x.ID_PADRE == id_padre && x.TIPO_PADRE == tipo_padre)
        //        .ToList().ForEach(x =>
        //        {
        //            SelectTreeItem item = new SelectTreeItem();
        //            item.text = x.DESCRIPCION;
        //            item.expanded = false;
        //            item.items = ObtenerItemsSelectTree(db, x.ID, x.TIPO, spras_id);
        //            if (item.items.Count() == 0)
        //            {
        //                item.items=ObtenerItemsTSOLT(db, x.ID, x.TIPO, spras_id);
        //            }
        //            items.Add(item);
        //        });
        //    if (items.Count() == 0)
        //    {
        //       items = ObtenerItemsTSOLT(db, id_padre, tipo_padre, spras_id);
        //    }
        //    return items;
        //}
        //static List<SelectTreeItem> ObtenerItemsTSOLT(WFARTHAEntities db, string id_padre, string tipo_padre, string spras_id, bool? esReversa = false)
        //{
        //    List<SelectTreeItem> items = new List<SelectTreeItem>();
        //    if (esReversa.Value)
        //    {
        //        items = db.TSOLs
        //            .Where(x=>x.TSOLR==null && (x.ACTIVO == null ? false : x.ACTIVO.Value))
        //            .Join(db.TSOLTs, s => s.ID, st => st.TSOL_ID, (s, st) => st)
        //            .Where(x=>x.SPRAS_ID == spras_id)
        //            .Select(x=> new SelectTreeItem
        //            {
        //                value = x.TSOL_ID,
        //                text = (x.TSOL_ID + " - " + x.TXT50),
        //                expanded = true
        //            })
        //            .ToList();
        //    }
        //    else
        //    {
        //        db.TSOL_TREE
        //                   .Where(y => y.TSOL_GROUP_ID == id_padre && y.TSOL_GROUP_TIPO == tipo_padre)
        //                   .Join(db.TSOLTs, tst => tst.TSOL_ID, st => st.TSOL_ID, (tst, st) => st)
        //                   .Where(y => y.SPRAS_ID == spras_id && (y.TSOL.ACTIVO == null ? false : y.TSOL.ACTIVO.Value))
        //                   .ToList().ForEach(y =>
        //                   {
        //                       items.Add(new SelectTreeItem
        //                       {
        //                           value = y.TSOL_ID,
        //                           text = (y.TSOL_ID + " - " + y.TXT50),
        //                           expanded = true
        //                       });
        //                   });
        //    }
        //    return items;
        //}

        //public static bool  ValidarPeriodoEnCalendario445(WFARTHAEntities db,string sociedad_id, string tsol_id,int periodo_id,string tipo, string usuario_id=null)
        //{
        //    //tipo
        //    //PRE = PreCierre
        //    //CI =  Cierre
        //    bool esPeriodoAbierto=false;
        //    DateTime fechaActual = DateTime.Now;
        //    short ejercicio = short.Parse(fechaActual.Year.ToString());

        //    switch (tipo)
        //    {
        //        case "PRE":
        //            esPeriodoAbierto = db.CALENDARIO_AC.Any(x =>
        //            x.ACTIVO == true &&
        //            x.SOCIEDAD_ID == sociedad_id && 
        //            x.TSOL_ID == tsol_id && 
        //            x.PERIODO==periodo_id &&
        //            x.EJERCICIO==ejercicio &&
        //            (fechaActual>= DbFunctions.CreateDateTime(x.PRE_FROMF.Year, x.PRE_FROMF.Month, x.PRE_FROMF.Day, x.PRE_FROMH.Hours, x.PRE_FROMH.Minutes, x.PRE_FROMH.Seconds) && 
        //             fechaActual<= DbFunctions.CreateDateTime(x.PRE_TOF.Year, x.PRE_TOF.Month, x.PRE_TOF.Day, x.PRE_TOH.Hours, x.PRE_TOH.Minutes, x.PRE_TOH.Seconds)));
        //            if (!esPeriodoAbierto && usuario_id != null)
        //            {
        //                esPeriodoAbierto = db.CALENDARIO_EX.Any(x =>
        //                  x.ACTIVO == true &&
        //                  x.SOCIEDAD_ID == sociedad_id &&
        //                  x.TSOL_ID == tsol_id &&
        //                  x.PERIODO == periodo_id &&
        //                  x.USUARIO_ID==usuario_id &&
        //                  x.EJERCICIO == ejercicio &&
        //                  (fechaActual >= DbFunctions.CreateDateTime(x.EX_FROMF.Year, x.EX_FROMF.Month, x.EX_FROMF.Day, x.EX_FROMH.Hours, x.EX_FROMH.Minutes,x.EX_FROMH.Seconds) &&
        //                  fechaActual <= DbFunctions.CreateDateTime(x.EX_TOF.Year, x.EX_TOF.Month, x.EX_TOF.Day, x.EX_TOH.Hours, x.EX_TOH.Minutes, x.EX_TOH.Seconds)));
        //            }
        //            break;
        //        case "CI":
        //            esPeriodoAbierto = db.CALENDARIO_AC.Any(x=>
        //                x.ACTIVO == true &&
        //                x.SOCIEDAD_ID == sociedad_id &&
        //                x.TSOL_ID == tsol_id &&
        //                x.PERIODO == periodo_id &&
        //                x.EJERCICIO == ejercicio &&
        //                (fechaActual >= DbFunctions.CreateDateTime(x.CIE_FROMF.Year, x.CIE_FROMF.Month, x.CIE_FROMF.Day, x.CIE_FROMH.Hours, x.CIE_FROMH.Minutes, x.CIE_FROMH.Seconds) && 
        //                fechaActual <= DbFunctions.CreateDateTime(x.CIE_TOF.Year, x.CIE_TOF.Month, x.CIE_TOF.Day, x.CIE_TOH.Hours, x.CIE_TOH.Minutes, x.CIE_TOH.Seconds)));
        //            break;
        //        default:
        //            break;
        //    }
        //    return esPeriodoAbierto;
        //}
        public static List<SelectListItem> ObtenerCmbEjercicio()
        {
            DateTime fechaActual = DateTime.Now;
            int anio = fechaActual.Year;
            return new List<SelectListItem> {
                    new SelectListItem{Text=(anio).ToString(),Value=(anio).ToString()},
                    new SelectListItem{Text=(anio+1).ToString(),Value=(anio+1).ToString()},
            };
        }
        public static List<SelectListItem> ObtenerCmbPageSize()
        {
            return new List<SelectListItem> {
                    new SelectListItem{Text="10",Value="10"},
                    new SelectListItem{Text="25",Value="25"},
                    new SelectListItem{Text="50",Value="50"},
                    new SelectListItem{Text="100",Value="100"}
            };
        }
    }
}