using EntityFramework.BulkInsert.Extensions;
using ExcelDataReader;
using Newtonsoft.Json;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFARTHA.Entities;
using WFARTHA.Models;

namespace WFARTHA.Services
{
    public class ProcesaFlujo
    {

        //MGC 02-10-2018 Cadena de autorización
        public string procesaPreliminar(DOCUMENTO d, bool edit, string nuevo, bool borrado,string fechacon)//MGC 10-12-2018 Firma del usuario cancelar//MGC-14-12-2018 Modificación fechacon
        {
            string correcto = String.Empty;
            WFARTHAEntities db = new WFARTHAEntities();

            //DOCUMENTO d = db.DOCUMENTOes.Find(id);
            bool ban = true;
            string accion = "";
            if (nuevo == "N")
            {
                accion = "P";
            }
            else
            {
                if (edit)
                {
                    accion = "R";
                }
                else
                {
                    accion = "P";
                }
            }

            //MGC 10-12-2018 Firma del usuario cancelar--------------------------------------->
            if (borrado == true)
            {
                accion = "B";
            }
            //MGC 10-12-2018 Firma del usuario cancelar---------------------------------------<

            ArchivoPreliminar sa = new ArchivoPreliminar();
            string file = sa.generarArchivo(d.NUM_DOC, accion, fechacon);//MGC-14-12-2018 Modificación fechacon

            if (file == "")
            {
                correcto = "0";
            }
            else
            {
                ban = false;
                correcto = file;
            }

            return correcto;
        }

        public string procesa(decimal id)
        {
            string correcto = String.Empty;
            WFARTHAEntities db = new WFARTHAEntities();


            DOCUMENTO d = db.DOCUMENTOes.Find(id);
            bool ban = true;
            ArchivoContable sa = new ArchivoContable();
            string file = sa.generarArchivo(d.NUM_DOC, 0, "A","");

            if (file == "")
            {
                correcto = "0";
            }
            else
            {
                ban = false;
                correcto = file;
            }

            return correcto;
        }

        public string procesa(FLUJO f, string recurrente, bool edit, string email, string nuevo_acc)
        {
            string fechacon = "";//MGC-14-12-2018 Modificación fechacon
            bool emails = false; //MGC 08-10-2018 Obtener los datos para el correo
            string correcto = String.Empty;
            WFARTHAEntities db = new WFARTHAEntities();
            FLUJO actual = new FLUJO();
            string emailsto = ""; //MGC 09-10-2018 Envío de correos
            if (f.ESTATUS.Equals("I"))//--------------------------------------------------------------------------------------NUEVO REGISTRO
            {
                actual.NUM_DOC = f.NUM_DOC;
                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                actual.COMENTARIO = f.COMENTARIO;
                actual.ESTATUS = f.ESTATUS;
                actual.FECHAC = f.FECHAC;
                actual.FECHAM = f.FECHAM;
                actual.LOOP = f.LOOP;
                actual.NUM_DOC = f.NUM_DOC;
                actual.POS = f.POS;

                if (email == "X")
                {
                    emails = true;

                    //MGC 09-10-2018 Envío de correos
                    //Obtener el email del creador
                    string emailc = "";
                    emailc = db.USUARIOs.Where(us => us.ID == d.USUARIOC_ID).FirstOrDefault().EMAIL;
                    emailsto = emailc;

                }

                actual.ID_RUTA_A = f.ID_RUTA_A;
                actual.RUTA_VERSION = f.RUTA_VERSION;
                actual.STEP_AUTO = f.STEP_AUTO;

                //MGC 11-12-2018 Agregar Contabilizador 0---------------->
                actual.VERSIONC1 = f.VERSIONC1;
                actual.VERSIONC2 = f.VERSIONC2;
                //MGC 11-12-2018 Agregar Contabilizador 0----------------<

                //DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
                //                    a.USUARIOC_ID.Equals(d.USUARIOC_ID)).OrderByDescending(a => a.VERSION).FirstOrDefault();
                //MGC COMM
                //DET_AGENTEC dah = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(d.USUARIOD_ID) & a.PAIS_ID == d.PAIS_ID &
                //                    a.VKORG.Equals(d.VKORG) & a.VTWEG.Equals(d.VTWEG) & a.SPART.Equals(d.SPART) & a.KUNNR.Equals(d.PAYER_ID))
                //                    .OrderByDescending(a => a.VERSION).FirstOrDefault();
                //DET_AGENTECC dah = db.DET_AGENTECC.Where(a => a.USUARIOC_ID.Equals(d.USUARIOD_ID) & a.PAIS_ID == d.PAIS_ID &
                //                    a.VKORG.Equals(d.VKORG) & a.VTWEG.Equals(d.VTWEG) & a.SPART.Equals(d.SPART) & a.KUNNR.Equals(d.PAYER_ID))
                //                    .OrderByDescending(a => a.VERSION).FirstOrDefault();

                int step = 0;
                int stepd = 0;//MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador
                if (actual.STEP_AUTO == 0)
                {
                    step = Convert.ToInt32(actual.STEP_AUTO) + 1;
                    stepd = Convert.ToInt32(actual.STEP_AUTO);//MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador
                }
                //MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador-------->
                //actual.DETVER = dah.VERSION; //MGC COMM
                actual.DETVER = f.RUTA_VERSION; //MGC COMM
                actual.USUARIOA_ID = f.USUARIOA_ID;
                actual.USUARIOD_ID = f.USUARIOD_ID;
                actual.WF_POS = f.WF_POS;
                actual.WF_VERSION = f.WF_VERSION;
                actual.WORKF_ID = f.WORKF_ID;
                f.ESTATUS = "A";
                actual.ESTATUS = f.ESTATUS;
                //MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador--------<

                //DET_AGENTECC dah = db.DET_AGENTECC.Where(a => a.VERSION == deta.VERSION && a.USUARIOC_ID == deta.USUARIOC_ID && a.ID_RUTA_AGENTE == deta.ID_RUTA_AGENTE && a.USUARIOA_ID == deta.USUARIOA_ID).OrderByDescending(a => a.VERSION).FirstOrDefault();
                List<DET_AGENTECA> dap = db.DET_AGENTECA.Where(a => a.VERSION == f.RUTA_VERSION && a.ID_RUTA_AGENTE == f.ID_RUTA_A && a.STEP_FASE == step).OrderByDescending(a => a.VERSION).ToList();
                DET_AGENTECA dah = new DET_AGENTECA();
                //MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador-------->
                if (dap == null || dap.Count == 0)
                {

                    dah = detAgenteLimite(dap, Convert.ToDecimal(d.MONTO_DOC_MD), stepd, actual);//MGC 19-10-2018 Cambio a detonador  

                }
                else
                {
                    dah = detAgenteLimite(dap, Convert.ToDecimal(d.MONTO_DOC_MD));
                }
                //MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador--------<

                //Si es edición, solamente actualizar el status en el flujo
                if (edit)
                {
                    FLUJO fedit = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) & a.ESTATUS.Equals("P")).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                    actual.DETPOS = f.DETPOS;
                    fedit.ESTATUS = f.ESTATUS;
                    db.Entry(fedit).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //Si es creación, se guarda el flujo
                    actual.DETPOS = 1;
                    db.FLUJOes.Add(actual);
                }

                WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                int next_step_a = 0;
                if (paso_a.NEXT_STEP != null)
                    next_step_a = (int)paso_a.NEXT_STEP;

                WORKFP next = new WORKFP();
                if (recurrente != "X")
                {
                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                }
                //else
                //{
                //    WORKFP autoriza = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.ACCION_ID == 5).FirstOrDefault();
                //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == autoriza.NS_ACCEPT).FirstOrDefault();
                //}
                if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                {
                    d.ESTATUS_WF = "A";
                    if (paso_a.EMAIL != null)
                    {
                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "2";
                    }
                }
                else
                {
                    //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                    FLUJO nuevo = new FLUJO();
                    nuevo.WORKF_ID = next.ID;
                    nuevo.WF_VERSION = next.VERSION;
                    nuevo.WF_POS = next.POS;
                    nuevo.NUM_DOC = actual.NUM_DOC;
                    nuevo.POS = actual.POS + 1;
                    //nuevo.LOOP = 1;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                    nuevo.LOOP = actual.LOOP;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                    //nuevo.STEP_AUTO = next.ACCION_ID;

                    //Agregar autorización MGC
                    nuevo.ID_RUTA_A = actual.ID_RUTA_A;
                    nuevo.RUTA_VERSION = actual.RUTA_VERSION;

                    //MGC 11-12-2018 Agregar Contabilizador 0----------------->
                    nuevo.VERSIONC1 = actual.VERSIONC1;
                    nuevo.VERSIONC2 = actual.VERSIONC2;
                    //MGC 11-12-2018 Agregar Contabilizador 0-----------------<

                    if (next.ACCION.TIPO == "E")
                    {
                        nuevo.USUARIOA_ID = null;
                        nuevo.DETPOS = 0;
                        nuevo.DETVER = 0;
                    }
                    else
                    {
                        if (recurrente != "X")
                        {
                            //MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador-------->
                            FLUJO detA = new FLUJO();
                            //MGC COMM
                            //FLUJO detA = determinaAgenteI(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0, dah);
                            if (dap == null || dap.Count == 0)
                            {
                                detA = determinaAgenteI(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0, dah, stepd, actual);//MGC 19-10-2018 Cambio a detonador 
                            }
                            else
                            {
                                detA = determinaAgenteI(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0, dah);
                            }
                            //MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador--------<

                            nuevo.USUARIOA_ID = detA.USUARIOA_ID;
                            nuevo.USUARIOD_ID = nuevo.USUARIOA_ID;
                            nuevo.STEP_AUTO = detA.STEP_AUTO;

                            DateTime fecha = DateTime.Now.Date;
                            //MGC COMM
                            DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
                            if (del != null)
                                nuevo.USUARIOA_ID = del.USUARIOD_ID;
                            else
                                nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                            nuevo.DETPOS = detA.DETPOS;
                            nuevo.DETVER = dah.VERSION;

                        }
                        //else
                        //{
                        //    nuevo.USUARIOA_ID = null;
                        //    nuevo.DETPOS = 0;
                        //    nuevo.DETVER = 0;
                        //}
                    }
                    nuevo.ESTATUS = "P";
                    nuevo.FECHAC = DateTime.Now;
                    nuevo.FECHAM = DateTime.Now;
                    try
                    {
                        //db.FLUJOes.Add(nuevo); //MGC Modificaión preliminar
                    }
                    catch (Exception e)
                    {

                    }
                    if (paso_a.EMAIL != null)
                    {
                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "1";
                    }

                    //d.ESTATUS_WF = "P";//MGC 26-10-2018 Modificaión para validar creación del archivo
                    db.Entry(d).State = EntityState.Modified;

                    //MGC-14-12-2018 Modificación fechacon
                    fechacon = "";
                    if(f.FECHACON != null)
                    {
                        fechacon = "";
                    }

                    //Crear el archivo para el preliminar //MGC Preliminar
                    string corr = procesaPreliminar(d, edit, nuevo_acc, false, fechacon);//MGC 10-12-2018 Firma del usuario cancelar//MGC-14-12-2018 Modificación fechacon

                    //Se genero el preliminar
                    if (corr == "0")
                    {
                        //DOCUMENTOPRE dp = new DOCUMENTOPRE();

                        //dp.NUM_DOC = d.NUM_DOC;
                        //dp.POS = 1;
                        //dp.MESSAGE = "Generando Preliminar";
                        //try
                        //{
                        //    db.DOCUMENTOPREs.Add(dp);
                        //    db.SaveChanges();
                        //}
                        //catch (Exception e)
                        //{
                        //    string r = "";
                        //}

                        //MGC 30-10-2018 Agregar mensaje a log de modificación
                        try
                        {
                            DOCUMENTOLOG dl = new DOCUMENTOLOG();

                            dl.NUM_DOC = d.NUM_DOC;
                            dl.TYPE_LINE = "M";
                            dl.TYPE = "S";
                            dl.MESSAGE = "Se generó el Archivo Preliminar";
                            dl.FECHA = DateTime.Now;

                            db.DOCUMENTOLOGs.Add(dl);
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {

                        }
                        //MGC 30-10-2018 Agregar mensaje a log de modificación

                        //Actualizar wf del documento
                        //d.ESTATUS_WF = "A";//MGC 30-10-2018 Modificaión para validar creación del archivo

                        if (edit)
                        {
                            d.ESTATUS = "N"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                            d.ESTATUS_WF = "P"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                        }

                        //MGC 30-10-2018 Actualizar el estatus de preliminar del doc
                        d.ESTATUS_PRE = "G";//MGC 30-10-2018 Modificaión para validar creación del archivo
                        db.Entry(d).State = EntityState.Modified;
                    }
                    //No se genero el preliminar
                    else
                    {
                        string m;
                        if (corr.Length > 50)
                        {
                            m = corr.Substring(0, 50);
                        }
                        else
                        {
                            m = corr;
                        }
                        //DOCUMENTOPRE dp = new DOCUMENTOPRE();

                        //dp.NUM_DOC = d.NUM_DOC;
                        //dp.POS = 1;
                        //dp.MESSAGE = m;
                        //try
                        //{
                        //    db.DOCUMENTOPREs.Add(dp);
                        //    db.SaveChanges();
                        //}
                        //catch (Exception E)
                        //{
                        //    string r = "";
                        //}

                        //MGC 30-10-2018 Agregar mensaje a log de modificación
                        try
                        {
                            DOCUMENTOLOG dl = new DOCUMENTOLOG();

                            dl.NUM_DOC = d.NUM_DOC;
                            dl.TYPE_LINE = "M";
                            dl.TYPE = "E";
                            dl.MESSAGE = m;
                            dl.FECHA = DateTime.Now;

                            db.DOCUMENTOLOGs.Add(dl);
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {

                        }
                        //MGC 30-10-2018 Agregar mensaje a log de modificación

                        //MGC 09-11-2018 Modificaión Estatus en la creación del archivo cuando la acción fue borrar-crear
                        //Si es edit, cambiar a P el flujo
                        //Y el estaus a N
                        if (edit)
                        {
                            d.ESTATUS = "N"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                            d.ESTATUS_WF = "P"; //MGC 02-11-2018 Regresa a estatus de crear preliminar
                        }



                        //MGC 30-10-2018 Actualizar el estatus de preliminar del doc
                        d.ESTATUS_PRE = "E";//MGC 30-10-2018 Modificaión Estatus en la creación del archivo
                        db.Entry(d).State = EntityState.Modified;

                        //Pendiente en edit, regresar a estatus de P en la creación del flujo

                    }

                    //d.ESTATUS_WF = "P";//MGC 30-10-2018 Modificaión para validar creación del archivo
                    db.Entry(d).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }
            else if (f.ESTATUS.Equals("A"))   //---------------------EN PROCESO DE APROBACIÓN
            {

                DOCUMENTO d = db.DOCUMENTOes.Find(f.NUM_DOC);//MGC 09-10-2018 Envío de correos

                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC) & a.POS == f.POS).OrderByDescending(a => a.POS).FirstOrDefault();

                if (!actual.ESTATUS.Equals("P"))
                {
                    return "1";//-----------------YA FUE PROCESADA
                }
                else
                {
                    var wf = actual.WORKFP;
                    actual.ESTATUS = f.ESTATUS;
                    //actual.FECHAM = f.FECHAM; //MGC 03-12-2018 Loop para firmas y obtener el más actual
                    actual.FECHAM = DateTime.Now; ; //MGC 03-12-2018 Loop para firmas y obtener el más actual
                    actual.COMENTARIO = f.COMENTARIO;
                    actual.USUARIOA_ID = f.USUARIOA_ID;
                    actual.FECHACON = f.FECHACON;//MGC-14-12-2018 Modificación fechacon
                    db.Entry(actual).State = EntityState.Modified;
                    db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual

                    //MGC-14-12-2018 Modificación fechacon------------>
                    if (actual.FECHACON != null)
                    {
                        fechacon = String.Format("{0:dd.MM.yyyy}", actual.FECHACON).Replace(".", "");
                    }
                    //MGC-14-12-2018 Modificación fechacon------------<

                    //MGC 09-10-2018 Envío de correos
                    if (actual.WORKFP.EMAIL == "X")
                    {
                        emails = true;

                        //MGC 09-10-2018 Envío de correos
                        //Obtener el email del creador
                        string emailc = "";
                        emailc = db.USUARIOs.Where(us => us.ID == d.USUARIOC_ID).FirstOrDefault().EMAIL;
                        emailsto = emailc;
                        emailc = "";

                        //Obtener el usuario aprobador
                        emailc = db.USUARIOs.Where(us => us.ID == f.USUARIOA_ID).FirstOrDefault().EMAIL;
                        emailsto += "," + emailc;

                        //Obtener el usuario del siguiente aprobador 


                    }


                    WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                    bool ban = true;
                    while (ban)         //--------------PARA LOOP
                    {
                        int next_step_a = 0;
                        int next_step_r = 0;
                        if (paso_a.NEXT_STEP != null)
                            next_step_a = (int)paso_a.NEXT_STEP;
                        if (paso_a.NS_ACCEPT != null)
                            next_step_a = (int)paso_a.NS_ACCEPT;
                        if (paso_a.NS_REJECT != null)
                            next_step_r = (int)paso_a.NS_REJECT;

                        WORKFP next = new WORKFP();
                        if (paso_a.ACCION.TIPO == "A" | paso_a.ACCION.TIPO == "N" | paso_a.ACCION.TIPO == "R" | paso_a.ACCION.TIPO == "T" | paso_a.ACCION.TIPO == "E" | paso_a.ACCION.TIPO == "B" | paso_a.ACCION.TIPO == "M")//Si está en proceso de aprobación
                        {
                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N") | f.ESTATUS.Equals("M"))//APROBAR SOLICITUD
                            {
                                //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC); //MGC 09-10-2018 Envío de correos

                                //Verificar si hay un siguiente aprobador
                                int stepnew = 0;
                                if (paso_a.ACCION.TIPO == "A")
                                {
                                    bool res = false;

                                    //MGC 11-12-2018 Agregar Contabilizador 0----------------->
                                    //Verificar si hay un primer contabilizador contabilizador 
                                    WORKFP nextconp = new WORKFP();
                                    nextconp = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                    if (nextconp != null && nextconp.ACCION.TIPO == "S")
                                    {
                                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                        stepnew = Convert.ToInt32(actual.STEP_AUTO);
                                    }
                                    else
                                    {

                                        res = detAgenteSiguiente(d.NUM_DOC, actual);

                                        if (res)
                                        {
                                            next = paso_a;
                                            stepnew = Convert.ToInt32(actual.STEP_AUTO) + 1;
                                        }
                                        else
                                        {
                                            next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                            stepnew = Convert.ToInt32(actual.STEP_AUTO);
                                        }
                                    }//MGC 11-12-2018 Agregar Contabilizador 0-----------------<
                                }

                                FLUJO nuevo = new FLUJO();
                                nuevo.WORKF_ID = paso_a.ID;
                                nuevo.WF_VERSION = paso_a.VERSION;
                                nuevo.WF_POS = next.POS;
                                nuevo.NUM_DOC = actual.NUM_DOC;
                                nuevo.POS = actual.POS + 1;

                                //MGC 11-12-2018 Agregar Contabilizador 0----------------->
                                nuevo.VERSIONC1 = actual.VERSIONC1;
                                nuevo.VERSIONC2 = actual.VERSIONC2;
                                //MGC 11-12-2018 Agregar Contabilizador 0-----------------<

                                //nuevo.LOOP = 1;//-----------------------------------cc//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                nuevo.LOOP = actual.LOOP;//-----------------------------------cc//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                                         //                                   //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();	                                               //    
                                                         //                                   //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
                                                         //                                   //if (loop >= next.LOOPS)
                                                         //                                   //{
                                                         //                                   //    paso_a = next;
                                                         //                                   //    continue;
                                                         //                                   //}
                                                         //                                   //if (loop != 0)
                                                         //                                   //    nuevo.LOOP = loop + 1;
                                                         //                                   //else
                                                         //                                   //    nuevo.LOOP = 1;
                                FLUJO detA = new FLUJO();
                                if (paso_a.ACCION.TIPO == "N")
                                    actual.DETPOS = actual.DETPOS - 1;
                                int sop = 99;
                                if (next.ACCION.TIPO == "S")
                                    sop = 98;

                                nuevo.STEP_AUTO = stepnew;
                                nuevo.RUTA_VERSION = actual.RUTA_VERSION;
                                nuevo.ID_RUTA_A = actual.ID_RUTA_A;

                                //MGC 08-10-2018 Modificación para mensaje por contabilizar
                                bool contabilizar = false;


                                //Para la contabilización se obtiene otra ruta
                                if (next.ACCION.TIPO == "P")
                                {
                                    detA = determinaAgenteContabilidad(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop, Convert.ToInt32(actual.STEP_AUTO));
                                    contabilizar = true;//MGC 08-10-2018 Modificación para mensaje por contabilizar
                                }
                                //MGC 11-12-2018 Agregar Contabilizador 0----------------->
                                else if (next.ACCION.TIPO == "S")
                                {
                                    detA = determinaAgenteContabilidad0(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop, Convert.ToInt32(actual.STEP_AUTO));

                                }
                                //MGC 11-12-2018 Agregar Contabilizador 0-----------------<
                                else
                                {

                                    detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop, stepnew);
                                }
                                //nuevo.USUARIOA_ID = detA.USUARIOA_ID;

                                //MGC 09-10-2018 Envío de correos
                                if (emails)
                                {

                                    //MGC 09-10-2018 Envío de correos
                                    string emailc = "";

                                    //Obtener el usuario del siguiente aprobador 
                                    emailc = db.USUARIOs.Where(us => us.ID == detA.USUARIOA_ID).FirstOrDefault().EMAIL;
                                    emailsto += "," + emailc;
                                }


                                nuevo.USUARIOD_ID = detA.USUARIOA_ID;

                                DateTime fecha = DateTime.Now.Date;
                                DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
                                if (del != null)
                                    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                                else
                                    nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                                nuevo.DETPOS = detA.DETPOS;
                                nuevo.DETVER = actual.DETVER;
                                nuevo.STEP_AUTO = detA.STEP_AUTO;
                                if (paso_a.ACCION.TIPO == "N")
                                {
                                    nuevo.DETPOS = nuevo.DETPOS + 1;
                                    actual.DETPOS = actual.DETPOS + 1;
                                }

                                //                    if (d.DOCUMENTORECs.Count > 0)
                                //                        recurrente = "X";

                                if (nuevo.DETPOS == 0 | nuevo.DETPOS == 99)
                                {
                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                    //if (recurrente == "X" & next.ACCION.TIPO.Equals("P"))
                                    //{
                                    //    next_step_a++;
                                    //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                    //}
                                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                                    {
                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                                        d.ESTATUS_WF = "A";
                                        if (paso_a.EMAIL.Equals("X"))
                                            correcto = "2";
                                        //if (recurrente == "X")
                                        //{
                                        //    FLUJO nuevos = new FLUJO();
                                        //    nuevos.WORKF_ID = paso_a.ID;
                                        //    nuevos.WF_VERSION = paso_a.VERSION;
                                        //    nuevos.WF_POS = next.POS;
                                        //    nuevos.NUM_DOC = actual.NUM_DOC;
                                        //    nuevos.POS = actual.POS + 1;
                                        //    nuevos.ESTATUS = "A";
                                        //    nuevos.FECHAC = DateTime.Now;
                                        //    nuevos.FECHAM = DateTime.Now;

                                        //    d.ESTATUS = "A";

                                        //    db.FLUJOes.Add(nuevos);
                                        //    //db.SaveChanges();
                                        //    ban = false;
                                        //}
                                    }
                                    else
                                    {
                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                                        //FLUJO nuevo = new FLUJO();
                                        nuevo.WORKF_ID = next.ID;
                                        nuevo.WF_VERSION = next.VERSION;
                                        nuevo.WF_POS = next.POS + detA.POS;
                                        nuevo.NUM_DOC = actual.NUM_DOC;
                                        nuevo.POS = actual.POS + 1;
                                        //nuevo.LOOP = 1;//-----------------------------------//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                        nuevo.LOOP = actual.LOOP;//-----------------------------------//MGC 03-12-2018 Loop para firmas y obtener el más actual

                                        //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0);
                                        //nuevo.USUARIOA_ID = "admin";
                                        //nuevo.DETPOS = 1;
                                        bool finR = false;
                                        d.ESTATUS_WF = "P";
                                        if (next.ACCION.TIPO.Equals("T"))
                                        {
                                            //    TAX_LAND tl = db.TAX_LAND.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault();
                                            //    if (tl != null)
                                            //    {
                                            //        //nuevo.USUARIOA_ID = db.DET_TAX.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.PAIS_ID.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
                                            //        //nuevo.USUARIOA_ID = db.DET_TAXEO.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
                                            //        nuevo.USUARIOA_ID = db.DET_TAXEOC.Where(a => a.USUARIOC_ID.Equals(d.USUARIOD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.KUNNR == d.PAYER_ID & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
                                            //        d.ESTATUS_WF = "T";
                                            //    }
                                            //    else
                                            //    {
                                            //        nuevo.WF_POS = nuevo.WF_POS + 1;
                                            //        nuevo.USUARIOA_ID = null;
                                            //        d.ESTATUS_WF = "A";
                                            //        d.ESTATUS_SAP = "P";
                                            //        if (recurrente == "X")
                                            //        {
                                            //            nuevo.WF_POS++;
                                            //            d.ESTATUS_SAP = "";
                                            //            finR = true;
                                            //        }
                                            //    }
                                        }
                                        else if (paso_a.ACCION.TIPO == "E")
                                        {
                                            nuevo.USUARIOA_ID = null;
                                        }
                                        else
                                        {
                                            if (nuevo.DETPOS == 0)
                                            {
                                                nuevo.USUARIOA_ID = null;
                                                d.ESTATUS_WF = "A";
                                                d.ESTATUS_SAP = "P";
                                            }
                                        }
                                        nuevo.ESTATUS = "P";
                                        nuevo.FECHAC = DateTime.Now;
                                        nuevo.FECHAM = DateTime.Now;

                                        if (finR)
                                        {
                                            nuevo.ESTATUS = "A";
                                            d.ESTATUS = "A";
                                        }

                                        db.FLUJOes.Add(nuevo);
                                        db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                        if (paso_a.EMAIL != null)
                                        {
                                            if (paso_a.EMAIL.Equals("X"))
                                                correcto = "1";
                                        }
                                    }
                                }
                                //else if(nuevo.DETPOS == 99)
                                //{
                                //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                //}
                                else
                                {
                                    //nuevo.USUARIOD_ID
                                    nuevo.ESTATUS = "P";
                                    nuevo.FECHAC = DateTime.Now;
                                    nuevo.FECHAM = DateTime.Now;
                                    nuevo.WF_POS = nuevo.WF_POS + detA.POS;

                                    db.FLUJOes.Add(nuevo);
                                    db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                    if (paso_a.EMAIL != null)
                                    {
                                        if (paso_a.EMAIL.Equals("X"))
                                            correcto = "1";
                                    }

                                    d.ESTATUS_WF = "P";
                                }
                                if (nuevo.DETPOS.Equals(98))
                                    d.ESTATUS_WF = "S";
                                db.Entry(d).State = EntityState.Modified;
                                db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                correcto = "1";


                                //MGC 08-10-2018 Modificación para mensaje por contabilizar
                                if (contabilizar)
                                {
                                    //MGC 08-10-2018 Modificación para mensaje por contabilizar
                                    //Eliminar los mensajes de la tabla
                                    try
                                    {
                                        db.DOCUMENTOPREs.RemoveRange(db.DOCUMENTOPREs.Where(dpre => dpre.NUM_DOC == d.NUM_DOC));
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {

                                    }

                                    //MGC 29-10-2018 Cambiar estatus conforme a matriz, contabilizar
                                    DOCUMENTO dcc = db.DOCUMENTOes.Find(f.NUM_DOC);
                                    dcc.ESTATUS = "C";
                                    db.Entry(dcc).State = EntityState.Modified;
                                    db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                    ////MGC 08-10-2018 Modificación para mensaje por contabilizar
                                    ////Guardar Mensaje en tabla
                                    //DOCUMENTOPRE dp = new DOCUMENTOPRE();

                                    //dp.NUM_DOC = d.NUM_DOC;
                                    //dp.POS = 1;
                                    //dp.MESSAGE = "Por contabilizar";

                                    //try
                                    //{
                                    //    db.DOCUMENTOPREs.Add(dp);
                                    //    db.SaveChanges();
                                    //}
                                    //catch (Exception e)
                                    //{
                                    //    string r = "";
                                    //}

                                }


                                db.SaveChanges();
                                ban = false;
                            }
                        }
                        //Contabilizar
                        //else if (paso_a.ACCION.TIPO == "P")//MGC               
                        else if (paso_a.ACCION.TIPO == "P")
                        {
                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
                            {
                                //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);//MGC 09-10-2018 Envío de correos

                                ArchivoContable sa = new ArchivoContable();
                                string file = sa.generarArchivo(d.NUM_DOC, 0, "A",fechacon);//MGC-14-12-2018 Modificación fechacon

                                if (file == "")
                                {
                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

                                    //MGC
                                    FLUJO nuevo = new FLUJO();
                                    if (next == null)
                                    {
                                        //nuevo = actual;
                                        //nuevo.ESTATUS = "A";
                                        //nuevo.POS = actual.POS + 1;



                                    }
                                    else
                                    {

                                        nuevo.WORKF_ID = paso_a.ID;
                                        nuevo.WF_VERSION = paso_a.VERSION;
                                        nuevo.WF_POS = next.POS;
                                        nuevo.NUM_DOC = actual.NUM_DOC;
                                        nuevo.POS = actual.POS + 1;
                                        nuevo.ESTATUS = "A";
                                        nuevo.FECHAC = DateTime.Now;
                                        nuevo.FECHAM = DateTime.Now;

                                        //MGC 11-12-2018 Agregar Contabilizador 0----------------->
                                        nuevo.VERSIONC1 = actual.VERSIONC1;
                                        nuevo.VERSIONC2 = actual.VERSIONC2;
                                        //MGC 11-12-2018 Agregar Contabilizador 0-----------------<

                                        db.FLUJOes.Add(nuevo);
                                        db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual


                                    }

                                    //d.ESTATUS = "A";//MGC 29-10-2018 El nuevo estatus es C              

                                    DOCUMENTO dcc = db.DOCUMENTOes.Find(d.NUM_DOC);

                                    dcc.ESTATUS_WF = "A";//MGC 29-10-2018 El nuevo estatus es C
                                    dcc.ESTATUS = "C";//MGC 29-10-2018 El nuevo estatus es C
                                    dcc.ESTATUS_PRE = "G";//MGC 29-10-2018 El nuevo estatus es C
                                    db.Entry(dcc).State = EntityState.Modified;
                                    correcto = file;

                                    db.SaveChanges();

                                    ban = false;

                                    //MGC 04 - 10 - 2018 Botones para acciones WF
                                    //Eliminar los mensajes de la tabla
                                    try
                                    {
                                        db.DOCUMENTOPREs.RemoveRange(db.DOCUMENTOPREs.Where(dpre => dpre.NUM_DOC == d.NUM_DOC));
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {

                                    }

                                    //MGC 04 - 10 - 2018 Botones para acciones WF
                                    //Mensaje para contabilizando SAP
                                    DOCUMENTOPRE dp = new DOCUMENTOPRE();

                                    dp.NUM_DOC = d.NUM_DOC;
                                    dp.POS = 1;
                                    dp.MESSAGE = "Contabilizando SAP";

                                    try
                                    {
                                        db.DOCUMENTOPREs.Add(dp);
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        string r = "";
                                    }

                                }
                                else
                                {
                                    DOCUMENTO dcc = db.DOCUMENTOes.Find(d.NUM_DOC);

                                    dcc.ESTATUS_WF = "A";//MGC 29-10-2018 El nuevo estatus es C
                                    dcc.ESTATUS = "C";//MGC 29-10-2018 El nuevo estatus es C
                                    dcc.ESTATUS_PRE = "E";//MGC 29-10-2018 El nuevo estatus es C
                                    db.Entry(dcc).State = EntityState.Modified;
                                    correcto = file;
                                    db.SaveChanges();

                                    ban = false;
                                    correcto = file;
                                }
                            }
                        }
                        else if (paso_a.ACCION.TIPO == "S")
                        {
                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
                            {
                                //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);//MGC 09-10-2018 Envío de correos

                                //MGC 11-12-2018 Agregar Contabilizador 0----------------->

                                //next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

                                //FLUJO nuevo = new FLUJO();
                                //nuevo.WORKF_ID = paso_a.ID;
                                //nuevo.WF_VERSION = paso_a.VERSION;
                                //nuevo.WF_POS = next.POS;
                                //nuevo.NUM_DOC = actual.NUM_DOC;
                                //nuevo.POS = actual.POS + 1;
                                //nuevo.LOOP = 1;//-----------------------------------cc
                                //               //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
                                //               //if (loop >= next.LOOPS)
                                //               //{
                                //               //    paso_a = next;
                                //               //    continue;
                                //               //}
                                //               //if (loop != 0)
                                //               //    nuevo.LOOP = loop + 1;
                                //               //else
                                //               //    nuevo.LOOP = 1;
                                //if (paso_a.ACCION.TIPO == "N")
                                //    actual.DETPOS = actual.DETPOS - 1;
                                //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 98, next.LOOPS, 98, Convert.ToInt32(nuevo.STEP_AUTO));
                                ////nuevo.USUARIOA_ID = detA.USUARIOA_ID;

                                //nuevo.USUARIOD_ID = detA.USUARIOA_ID;

                                //DateTime fecha = DateTime.Now.Date;
                                //DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
                                //if (del != null)
                                //    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                                //else
                                //    nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;


                                //nuevo.DETPOS = detA.DETPOS;
                                //nuevo.DETVER = actual.DETVER;
                                //if (paso_a.ACCION.TIPO == "N")
                                //{
                                //    nuevo.DETPOS = nuevo.DETPOS + 1;
                                //    actual.DETPOS = actual.DETPOS + 1;
                                //}


                                //if (nuevo.DETPOS == 0 | nuevo.DETPOS == 99)
                                //{
                                //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                //    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                                //    {
                                //        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                                //        d.ESTATUS_WF = "A";
                                //        if (paso_a.EMAIL.Equals("X"))
                                //            correcto = "2";
                                //    }
                                //    else
                                //    {
                                //        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                                //        //FLUJO nuevo = new FLUJO();
                                //        nuevo.WORKF_ID = next.ID;
                                //        nuevo.WF_VERSION = next.VERSION;
                                //        nuevo.WF_POS = next.POS + detA.POS;
                                //        nuevo.NUM_DOC = actual.NUM_DOC;
                                //        nuevo.POS = actual.POS + 1;
                                //        nuevo.LOOP = 1;//-----------------------------------
                                //                       //int loop1 = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
                                //                       //if (loop1 >= next.LOOPS)
                                //                       //{
                                //                       //    paso_a = next;
                                //                       //    continue;
                                //                       //}
                                //                       //if (loop1 != 0)
                                //                       //    nuevo.LOOP = loop1 + 1;
                                //                       //else
                                //                       //    nuevo.LOOP = 1;

                                //        //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0);
                                //        //nuevo.USUARIOA_ID = "admin";
                                //        //nuevo.DETPOS = 1;
                                //        d.ESTATUS_WF = "P";
                                //        if (nuevo.DETPOS == 0)
                                //        {
                                //            nuevo.USUARIOA_ID = null;
                                //            d.ESTATUS_WF = "A";
                                //            d.ESTATUS_SAP = "P";
                                //        }
                                //        nuevo.ESTATUS = "P";
                                //        nuevo.FECHAC = DateTime.Now;
                                //        nuevo.FECHAM = DateTime.Now;

                                //        db.FLUJOes.Add(nuevo);
                                //        if (paso_a.EMAIL.Equals("X"))
                                //            correcto = "1";
                                //    }
                                //}
                                ////else if(nuevo.DETPOS == 99)
                                ////{
                                ////    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                ////}
                                //else
                                //{
                                //    //nuevo.USUARIOD_ID
                                //    nuevo.ESTATUS = "P";
                                //    nuevo.FECHAC = DateTime.Now;
                                //    nuevo.FECHAM = DateTime.Now;
                                //    nuevo.WF_POS = nuevo.WF_POS + detA.POS;

                                //    db.FLUJOes.Add(nuevo);
                                //    if (paso_a.EMAIL.Equals("X"))
                                //        correcto = "1";

                                //    d.ESTATUS_WF = "P";
                                //}
                                //db.Entry(d).State = EntityState.Modified;

                                //db.SaveChanges();
                                //ban = false;

                                //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC); //MGC 09-10-2018 Envío de correos

                                //Verificar si hay un siguiente aprobador
                                int stepnew = 0;
                                if (paso_a.ACCION.TIPO == "S")
                                {
                                    bool res = false;

                                    res = detAgenteSiguiente(d.NUM_DOC, actual);

                                    if (res)
                                    {
                                        //next = paso_a;//MGC 11-12-2018 Agregar Contabilizador 0
                                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                        stepnew = Convert.ToInt32(actual.STEP_AUTO) + 1;
                                    }
                                    else
                                    {
                                        //next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();//MGC 11-12-2018 Agregar Contabilizador 0
                                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a + 1).FirstOrDefault();//MGC 11-12-2018 Agregar Contabilizador 0
                                        stepnew = Convert.ToInt32(actual.STEP_AUTO);
                                    }
                                }

                                FLUJO nuevo = new FLUJO();
                                nuevo.WORKF_ID = paso_a.ID;
                                nuevo.WF_VERSION = paso_a.VERSION;
                                nuevo.WF_POS = next.POS;
                                nuevo.NUM_DOC = actual.NUM_DOC;
                                nuevo.POS = actual.POS + 1;

                                //MGC 11-12-2018 Agregar Contabilizador 0----------------->
                                nuevo.VERSIONC1 = actual.VERSIONC1;
                                nuevo.VERSIONC2 = actual.VERSIONC2;
                                //MGC 11-12-2018 Agregar Contabilizador 0-----------------<

                                //nuevo.LOOP = 1;//-----------------------------------cc//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                nuevo.LOOP = actual.LOOP;//-----------------------------------cc//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                                         //                                   //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();	                                               //    
                                                         //                                   //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
                                                         //                                   //if (loop >= next.LOOPS)
                                                         //                                   //{
                                                         //                                   //    paso_a = next;
                                                         //                                   //    continue;
                                                         //                                   //}
                                                         //                                   //if (loop != 0)
                                                         //                                   //    nuevo.LOOP = loop + 1;
                                                         //                                   //else
                                                         //                                   //    nuevo.LOOP = 1;
                                FLUJO detA = new FLUJO();
                                if (paso_a.ACCION.TIPO == "N")
                                    actual.DETPOS = actual.DETPOS - 1;
                                int sop = 99;
                                if (next.ACCION.TIPO == "S")
                                    sop = 98;

                                nuevo.STEP_AUTO = stepnew;
                                nuevo.RUTA_VERSION = actual.RUTA_VERSION;
                                nuevo.ID_RUTA_A = actual.ID_RUTA_A;

                                //MGC 08-10-2018 Modificación para mensaje por contabilizar
                                bool contabilizar = false;


                                //Para la contabilización se obtiene otra ruta
                                if (next.ACCION.TIPO == "P")
                                {
                                    detA = determinaAgenteContabilidad(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop, Convert.ToInt32(actual.STEP_AUTO));
                                    contabilizar = true;//MGC 08-10-2018 Modificación para mensaje por contabilizar
                                }
                                else
                                {

                                    detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop, stepnew);
                                }
                                //nuevo.USUARIOA_ID = detA.USUARIOA_ID;

                                //MGC 09-10-2018 Envío de correos
                                if (emails)
                                {

                                    //MGC 09-10-2018 Envío de correos
                                    string emailc = "";

                                    //Obtener el usuario del siguiente aprobador 
                                    emailc = db.USUARIOs.Where(us => us.ID == detA.USUARIOA_ID).FirstOrDefault().EMAIL;
                                    emailsto += "," + emailc;
                                }


                                nuevo.USUARIOD_ID = detA.USUARIOA_ID;

                                DateTime fecha = DateTime.Now.Date;
                                DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
                                if (del != null)
                                    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                                else
                                    nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                                nuevo.DETPOS = detA.DETPOS;
                                nuevo.DETVER = actual.DETVER;
                                nuevo.STEP_AUTO = detA.STEP_AUTO;
                                if (paso_a.ACCION.TIPO == "N")
                                {
                                    nuevo.DETPOS = nuevo.DETPOS + 1;
                                    actual.DETPOS = actual.DETPOS + 1;
                                }

                                //                    if (d.DOCUMENTORECs.Count > 0)
                                //                        recurrente = "X";

                                if (nuevo.DETPOS == 0 | nuevo.DETPOS == 99)
                                {
                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                    //if (recurrente == "X" & next.ACCION.TIPO.Equals("P"))
                                    //{
                                    //    next_step_a++;
                                    //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                    //}
                                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                                    {
                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                                        d.ESTATUS_WF = "A";
                                        if (paso_a.EMAIL.Equals("X"))
                                            correcto = "2";
                                        //if (recurrente == "X")
                                        //{
                                        //    FLUJO nuevos = new FLUJO();
                                        //    nuevos.WORKF_ID = paso_a.ID;
                                        //    nuevos.WF_VERSION = paso_a.VERSION;
                                        //    nuevos.WF_POS = next.POS;
                                        //    nuevos.NUM_DOC = actual.NUM_DOC;
                                        //    nuevos.POS = actual.POS + 1;
                                        //    nuevos.ESTATUS = "A";
                                        //    nuevos.FECHAC = DateTime.Now;
                                        //    nuevos.FECHAM = DateTime.Now;

                                        //    d.ESTATUS = "A";

                                        //    db.FLUJOes.Add(nuevos);
                                        //    //db.SaveChanges();
                                        //    ban = false;
                                        //}
                                    }
                                    else
                                    {
                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                                        //FLUJO nuevo = new FLUJO();
                                        nuevo.WORKF_ID = next.ID;
                                        nuevo.WF_VERSION = next.VERSION;
                                        nuevo.WF_POS = next.POS + detA.POS;
                                        nuevo.NUM_DOC = actual.NUM_DOC;
                                        nuevo.POS = actual.POS + 1;
                                        //nuevo.LOOP = 1;//-----------------------------------//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                        nuevo.LOOP = actual.LOOP;//-----------------------------------//MGC 03-12-2018 Loop para firmas y obtener el más actual

                                        //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0);
                                        //nuevo.USUARIOA_ID = "admin";
                                        //nuevo.DETPOS = 1;
                                        bool finR = false;
                                        d.ESTATUS_WF = "P";
                                        if (next.ACCION.TIPO.Equals("T"))
                                        {
                                            //    TAX_LAND tl = db.TAX_LAND.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault();
                                            //    if (tl != null)
                                            //    {
                                            //        //nuevo.USUARIOA_ID = db.DET_TAX.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.PAIS_ID.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
                                            //        //nuevo.USUARIOA_ID = db.DET_TAXEO.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
                                            //        nuevo.USUARIOA_ID = db.DET_TAXEOC.Where(a => a.USUARIOC_ID.Equals(d.USUARIOD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.KUNNR == d.PAYER_ID & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
                                            //        d.ESTATUS_WF = "T";
                                            //    }
                                            //    else
                                            //    {
                                            //        nuevo.WF_POS = nuevo.WF_POS + 1;
                                            //        nuevo.USUARIOA_ID = null;
                                            //        d.ESTATUS_WF = "A";
                                            //        d.ESTATUS_SAP = "P";
                                            //        if (recurrente == "X")
                                            //        {
                                            //            nuevo.WF_POS++;
                                            //            d.ESTATUS_SAP = "";
                                            //            finR = true;
                                            //        }
                                            //    }
                                        }
                                        else if (paso_a.ACCION.TIPO == "E")
                                        {
                                            nuevo.USUARIOA_ID = null;
                                        }
                                        else
                                        {
                                            if (nuevo.DETPOS == 0)
                                            {
                                                nuevo.USUARIOA_ID = null;
                                                d.ESTATUS_WF = "A";
                                                d.ESTATUS_SAP = "P";
                                            }
                                        }
                                        nuevo.ESTATUS = "P";
                                        nuevo.FECHAC = DateTime.Now;
                                        nuevo.FECHAM = DateTime.Now;

                                        if (finR)
                                        {
                                            nuevo.ESTATUS = "A";
                                            d.ESTATUS = "A";
                                        }

                                        db.FLUJOes.Add(nuevo);
                                        db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                        if (paso_a.EMAIL != null)
                                        {
                                            if (paso_a.EMAIL.Equals("X"))
                                                correcto = "1";
                                        }
                                    }
                                }
                                //else if(nuevo.DETPOS == 99)
                                //{
                                //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                //}
                                else
                                {
                                    //nuevo.USUARIOD_ID
                                    nuevo.ESTATUS = "P";
                                    nuevo.FECHAC = DateTime.Now;
                                    nuevo.FECHAM = DateTime.Now;
                                    nuevo.WF_POS = nuevo.WF_POS + detA.POS;

                                    db.FLUJOes.Add(nuevo);
                                    db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                    if (paso_a.EMAIL != null)
                                    {
                                        if (paso_a.EMAIL.Equals("X"))
                                            correcto = "1";
                                    }

                                    d.ESTATUS_WF = "P";
                                }
                                if (nuevo.DETPOS.Equals(98))
                                    d.ESTATUS_WF = "S";
                                db.Entry(d).State = EntityState.Modified;
                                db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                correcto = "1";


                                //MGC 08-10-2018 Modificación para mensaje por contabilizar
                                if (contabilizar)
                                {
                                    //MGC 08-10-2018 Modificación para mensaje por contabilizar
                                    //Eliminar los mensajes de la tabla
                                    try
                                    {
                                        db.DOCUMENTOPREs.RemoveRange(db.DOCUMENTOPREs.Where(dpre => dpre.NUM_DOC == d.NUM_DOC));
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {

                                    }

                                    //MGC 29-10-2018 Cambiar estatus conforme a matriz, contabilizar
                                    DOCUMENTO dcc = db.DOCUMENTOes.Find(f.NUM_DOC);
                                    dcc.ESTATUS = "C";
                                    db.Entry(dcc).State = EntityState.Modified;
                                    db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                    ////MGC 08-10-2018 Modificación para mensaje por contabilizar
                                    ////Guardar Mensaje en tabla
                                    //DOCUMENTOPRE dp = new DOCUMENTOPRE();

                                    //dp.NUM_DOC = d.NUM_DOC;
                                    //dp.POS = 1;
                                    //dp.MESSAGE = "Por contabilizar";

                                    //try
                                    //{
                                    //    db.DOCUMENTOPREs.Add(dp);
                                    //    db.SaveChanges();
                                    //}
                                    //catch (Exception e)
                                    //{
                                    //    string r = "";
                                    //}

                                }


                                db.SaveChanges();
                                ban = false;

                                //MGC 11-12-2018 Agregar Contabilizador 0-----------------<
                            }
                        }

                    }
                }
            }
            else if (f.ESTATUS.Equals("R"))//Rechazada
            {

                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC) & a.POS == f.POS).OrderByDescending(a => a.POS).FirstOrDefault();

                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);

                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
                WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();


                //MGC 09-10-2018 Envío de correos
                if (actual.WORKFP.EMAIL == "X")
                {
                    emails = true;

                    //MGC 09-10-2018 Envío de correos
                    //Obtener el email del creador
                    string emailc = "";
                    emailc = db.USUARIOs.Where(us => us.ID == d.USUARIOC_ID).FirstOrDefault().EMAIL;
                    emailsto = emailc;
                    emailc = "";

                    //Obtener el usuario aprobador
                    emailc = db.USUARIOs.Where(us => us.ID == f.USUARIOA_ID).FirstOrDefault().EMAIL;
                    emailsto += "," + emailc;

                }

                int next_step_a = 0;
                int next_step_r = 0;
                if (paso_a.NEXT_STEP != null)
                    next_step_a = (int)paso_a.NEXT_STEP;
                if (paso_a.NS_ACCEPT != null)
                    next_step_a = (int)paso_a.NS_ACCEPT;
                if (paso_a.NS_REJECT != null)
                    next_step_r = (int)paso_a.NS_REJECT;

                if (paso_a.NS_REJECT == null)
                    next_step_r = 1;

                WORKFP next = new WORKFP();
                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_r).FirstOrDefault();

                correcto = "3";
                actual.ESTATUS = f.ESTATUS;
                //actual.FECHAM = f.FECHAM;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                actual.FECHAM = DateTime.Now;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                actual.COMENTARIO = f.COMENTARIO;

                FLUJO nuevo = new FLUJO();
                nuevo.WORKF_ID = next.ID;
                nuevo.WF_VERSION = next.VERSION;
                nuevo.WF_POS = next.POS;
                nuevo.NUM_DOC = actual.NUM_DOC;
                nuevo.POS = actual.POS + 1;
                nuevo.DETPOS = 1;
                nuevo.DETVER = actual.DETVER;

                //MGC 11-12-2018 Agregar Contabilizador 0----------------->
                nuevo.VERSIONC1 = actual.VERSIONC1;
                nuevo.VERSIONC2 = actual.VERSIONC2;
                //MGC 11-12-2018 Agregar Contabilizador 0-----------------<

                nuevo.LOOP = actual.LOOP + 1;//-----------------------------------//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                             //nuevo.LOOP = 1;//-----------------------------------//MGC 03-12-2018 Loop para firmas y obtener el más actual
                                             //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC); //MGC 09-10-2018 Envío de correos

                //MGC 
                //nuevo.USUARIOD_ID = d.USUARIOD_ID;
                //Datos del flujo
                //nuevo.USUARIOD_ID = d.USUARIOC_ID;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                nuevo.USUARIOD_ID = d.USUARIOD_ID;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                nuevo.USUARIOA_ID = d.USUARIOC_ID;

                //Datos del flujo versiones
                nuevo.ID_RUTA_A = actual.ID_RUTA_A;
                nuevo.RUTA_VERSION = actual.RUTA_VERSION;
                nuevo.STEP_AUTO = 0;

                DateTime fecha = DateTime.Now.Date;
                DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
                if (del != null)
                    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                else
                    //nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                      nuevo.USUARIOA_ID = nuevo.USUARIOA_ID;//MGC 03-12-2018 Loop para firmas y obtener el más actual
                //nuevo.USUARIOD_ID
                nuevo.ESTATUS = "P";
                nuevo.FECHAC = DateTime.Now;
                nuevo.FECHAM = DateTime.Now;

                db.FLUJOes.Add(nuevo);//MGC Cancelar Preliminar
                db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual

                db.Entry(actual).State = EntityState.Modified;
                db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                //d.ESTATUS_WF = "R";
                //MGC 30-10-2018 Cambiar estatus en la solicitud
                d.ESTATUS_WF = "R";
                d.ESTATUS = "F";
                db.Entry(d).State = EntityState.Modified;//MGC 11-10-2018 No enviar correos
                db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual
                if (next.ACCION.TIPO == "S")
                {
                    d.ESTATUS = "R";
                    d.ESTATUS_WF = "S";
                }

                //MGC Cancelar Preliminar
                //Eliminar los mensajes de la tabla 
                //try
                //{
                //    db.DOCUMENTOPREs.RemoveRange(db.DOCUMENTOPREs.Where(dpre => dpre.NUM_DOC == actual.NUM_DOC));
                //    db.SaveChanges();
                //}
                //catch (Exception e)
                //{

                //}
                ////Agregar error al mensaje de preliminar
                //DOCUMENTOPRE dp = new DOCUMENTOPRE();

                //dp.NUM_DOC = actual.NUM_DOC;
                //dp.POS = 1;
                //dp.MESSAGE = "Cancelando Preliminar";

                //db.DOCUMENTOPREs.Add(dp);
                //db.SaveChanges();


                db.SaveChanges();

            }

            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            if (correcto.Equals(""))
            {
                FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                string corr = "";
                if (conta.WORKFP.ACCION.TIPO == "P")
                {
                    conta.ESTATUS = "A";
                    conta.FECHAM = DateTime.Now;


                    db.Entry(conta).State = EntityState.Modified;
                    //MGC COMM
                    //corr = procesa(conta, recurrente);
                    correcto = "1";
                }
            }

            //else if (correcto.Equals("1"))
            //{
            //    FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
            //    string corr = "";
            //    if (conta.WORKFP.ACCION.TIPO == "B")
            //    {
            //        Email em = new Email();
            //        em.enviaMailC(f.NUM_DOC, false, null, )
            //    }
            //}

            //MGC 08-10-2018 Obtener los datos para el correo
            if (emails && (correcto == "1" | correcto == "3"))
            {

                //MGC 08-10-2018 Obtener los datos para el correo comentar provisional
                Email em = new Email();
                string UrlDirectory = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                string image = System.Web.HttpContext.Current.Server.MapPath("~/images/artha_logo.jpg");
                string page = "";

                if (f.ESTATUS.Equals("R"))
                {
                    page = "Details";
                }
                else if (f.ESTATUS.Equals("I") | f.ESTATUS.Equals("A"))
                {
                    page = "Index";
                }
                //MGC 11-10-2018 No enviar correos
                try
                {
                    em.enviaMailC(f.NUM_DOC, true, System.Web.HttpContext.Current.Session["spras"].ToString(), UrlDirectory, page, image, emailsto);
                }
                catch (Exception e)
                {

                }

            }

            return correcto;
        }



        public FLUJO determinaAgente(DOCUMENTO d, string user, string delega, int pos, int? loop, int sop, int fase_det)
        {
            if (delega != null)
                user = delega;
            bool fin = false;
            WFARTHAEntities db = new WFARTHAEntities();
            DET_AGENTECA dap = new DET_AGENTECA();
            FLUJO f_actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).FirstOrDefault();
            //DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
            //                    a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.VERSION == f_actual.DETVER).FirstOrDefault();

            //Obtener lista de agentes y obtener uno dependiendo del monto -----------------------------------------------------
            List<DET_AGENTECA> dah = db.DET_AGENTECA.Where(a => a.VERSION == f_actual.RUTA_VERSION && a.ID_RUTA_AGENTE == f_actual.ID_RUTA_A && a.STEP_FASE == fase_det).ToList();

            if (dah != null || dah.Count > 0)
            {

                dap = detAgenteLimite(dah, Convert.ToDecimal(d.MONTO_DOC_MD));
            }
            else
            {
                //El flujo termino

            }
            int ppos = 0;

            //dap = db.DET_AGENTECA.Where(a => a.VERSION == dah.VERSION && a.ID_RUTA_AGENTE == dah.ID_RUTA_AGENTE && a.STEP == 1).FirstOrDefault();
            //dap = db.DET_AGENTECA.Where(a => a.VERSION == f_actual.RUTA_VERSION && a.ID_RUTA_AGENTE == f_actual.ID_RUTA_A && a.STEP == 1).FirstOrDefault();


            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);
            //long gaa = db.CREADOR2.Where(a => a.ID.Equals(u.ID) & a.BUKRS.Equals(d.SOCIEDAD_ID) & a.LAND.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.ACTIVO == true).FirstOrDefault().AGROUP_ID;
            //int ppos = 0;

            //if (pos.Equals(0))
            //{
            //    if (loop == null)
            //    {
            //        //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
            //        //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
            //        dap = dah.Where(a => a.STEP_FASE == 1).FirstOrDefault();
            //        dap.STEP_FASE = dap.STEP_FASE - 1;
            //    }
            //    else
            //    {
            //        FLUJO ffl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) & a.ESTATUS.Equals("R")).OrderByDescending(a => a.POS).FirstOrDefault();
            //        if (ffl.DETPOS == 99)
            //            ppos = 1;
            //        ffl.DETPOS = ffl.DETPOS - 1;
            //        fin = true;
            //        ffl.POS = ppos;

            //        return ffl;
            //    }
            //}
            //else if (pos.Equals(98))
            //{
            //    dap = dah.Where(a => a.STEP_FASE == (pos + 1)).FirstOrDefault();
            //}
            //else
            //{
            //    //DET_AGENTE actual = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos)).FirstOrDefault();
            //    DET_AGENTECA actual = dah.Where(a => a.POS == (pos)).FirstOrDefault();
            //    if (actual.STEP_FASE == 99)
            //    {
            //        fin = true;
            //    }
            //    else if (actual.STEP_FASE == 98)
            //    {
            //        //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
            //        dap = dah.Where(a => a.STEP_FASE == (pos)).FirstOrDefault();
            //    }
            //    else
            //    {
            //        if (actual.MONTO != null)
            //            if (d.MONTO_DOC_ML2 > actual.MONTO)
            //            {
            //                dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //                ppos = -1;
            //            }
            //        //if (actual.PRESUPUESTO != null)
            //        if ((bool)actual.PRESUPUESTO)
            //            if (d.MONTO_DOC_MD > 100000)
            //            {
            //                //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
            //                dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //                ppos = -1;
            //            }
            //    }
            //}

            //agente = dap.USUARIOA_ID;


            string agente = "";
            FLUJO f = new FLUJO();
            f.DETPOS = 0;


            agente = dap.AGENTE_SIG;
            //f.DETPOS = dap.POS;
            f.DETPOS = dap.STEP_FASE;
            f.STEP_AUTO = dap.STEP_FASE;
            //if (!fin)
            //{
            //    if (dap != null)
            //    {
            //        if (dap.USUARIOA_ID != null)
            //        {
            //            //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //            agente = dap.USUARIOA_ID;
            //            f.DETPOS = dap.POS;
            //        }
            //        else
            //        {
            //            dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
            //            if (dap == null)
            //            {
            //                agente = d.USUARIOD_ID;
            //                f.DETPOS = 98;
            //            }
            //            else
            //            {
            //                //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //                agente = dap.USUARIOA_ID;
            //                f.DETPOS = dap.POS;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
            //        if (dap == null)
            //        {
            //            agente = d.USUARIOD_ID;
            //            f.DETPOS = 98;
            //        }
            //        else
            //        {
            //            //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //            agente = dap.USUARIOA_ID;
            //            f.DETPOS = dap.POS;
            //        }
            //    }
            //}
            f.POS = ppos;
            if (agente != "")
                f.USUARIOA_ID = agente;
            else
                f.USUARIOA_ID = null;
            return f;
        }

        public FLUJO determinaAgenteContabilidad(DOCUMENTO d, string user, string delega, int pos, int? loop, int sop, int fase_det)
        {
            if (delega != null)
                user = delega;
            bool fin = false;
            WFARTHAEntities db = new WFARTHAEntities();
            DET_APROB dap = new DET_APROB();
            FLUJO f_actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).FirstOrDefault();
            //DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
            //                    a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.VERSION == f_actual.DETVER).FirstOrDefault();

            ////MGC 12-11-2018 El usuario aprobador, no tiene como tal una versión, se toma el actual-------------------------------------------------------------------->

            //dap = db.DET_APROB.Where(ap => ap.VERSION == f_actual.RUTA_VERSION && ap.ID_SOCIEDAD == d.SOCIEDAD_ID).OrderBy(apo => apo.VERSION).FirstOrDefault();
            //MGC 11-12-2018 Agregar Contabilizador 0----------------->
            //dap = db.DET_APROB.Where(ap => ap.ID_SOCIEDAD == d.SOCIEDAD_ID).OrderBy(apo => apo.VERSION).FirstOrDefault();
            dap = db.DET_APROB.Where(ap => ap.ID_SOCIEDAD == d.SOCIEDAD_ID && ap.VERSION == f_actual.VERSIONC2).OrderBy(apo => apo.VERSION).FirstOrDefault();
            //MGC 11-12-2018 Agregar Contabilizador 0-----------------<

            ////MGC 12-11-2018 El usuario aprobador, no tiene como tal una versión, se toma el actual--------------------------------------------------------------------<

            int ppos = 0;

            //dap = db.DET_AGENTECA.Where(a => a.VERSION == dah.VERSION && a.ID_RUTA_AGENTE == dah.ID_RUTA_AGENTE && a.STEP == 1).FirstOrDefault();
            //dap = db.DET_AGENTECA.Where(a => a.VERSION == f_actual.RUTA_VERSION && a.ID_RUTA_AGENTE == f_actual.ID_RUTA_A && a.STEP == 1).FirstOrDefault();


            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);
            //long gaa = db.CREADOR2.Where(a => a.ID.Equals(u.ID) & a.BUKRS.Equals(d.SOCIEDAD_ID) & a.LAND.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.ACTIVO == true).FirstOrDefault().AGROUP_ID;
            //int ppos = 0;

            //if (pos.Equals(0))
            //{
            //    if (loop == null)
            //    {
            //        //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
            //        //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
            //        dap = dah.Where(a => a.STEP_FASE == 1).FirstOrDefault();
            //        dap.STEP_FASE = dap.STEP_FASE - 1;
            //    }
            //    else
            //    {
            //        FLUJO ffl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) & a.ESTATUS.Equals("R")).OrderByDescending(a => a.POS).FirstOrDefault();
            //        if (ffl.DETPOS == 99)
            //            ppos = 1;
            //        ffl.DETPOS = ffl.DETPOS - 1;
            //        fin = true;
            //        ffl.POS = ppos;

            //        return ffl;
            //    }
            //}
            //else if (pos.Equals(98))
            //{
            //    dap = dah.Where(a => a.STEP_FASE == (pos + 1)).FirstOrDefault();
            //}
            //else
            //{
            //    //DET_AGENTE actual = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos)).FirstOrDefault();
            //    DET_AGENTECA actual = dah.Where(a => a.POS == (pos)).FirstOrDefault();
            //    if (actual.STEP_FASE == 99)
            //    {
            //        fin = true;
            //    }
            //    else if (actual.STEP_FASE == 98)
            //    {
            //        //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
            //        dap = dah.Where(a => a.STEP_FASE == (pos)).FirstOrDefault();
            //    }
            //    else
            //    {
            //        if (actual.MONTO != null)
            //            if (d.MONTO_DOC_ML2 > actual.MONTO)
            //            {
            //                dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //                ppos = -1;
            //            }
            //        //if (actual.PRESUPUESTO != null)
            //        if ((bool)actual.PRESUPUESTO)
            //            if (d.MONTO_DOC_MD > 100000)
            //            {
            //                //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
            //                dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //                ppos = -1;
            //            }
            //    }
            //}

            //agente = dap.USUARIOA_ID;


            string agente = "";
            FLUJO f = new FLUJO();
            f.DETPOS = 0;


            agente = dap.ID_USUARIO;
            //f.DETPOS = dap.POS;
            f.DETPOS = 1;
            f.STEP_AUTO = dap.STEP_FASE;
            //if (!fin)
            //{
            //    if (dap != null)
            //    {
            //        if (dap.USUARIOA_ID != null)
            //        {
            //            //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //            agente = dap.USUARIOA_ID;
            //            f.DETPOS = dap.POS;
            //        }
            //        else
            //        {
            //            dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
            //            if (dap == null)
            //            {
            //                agente = d.USUARIOD_ID;
            //                f.DETPOS = 98;
            //            }
            //            else
            //            {
            //                //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //                agente = dap.USUARIOA_ID;
            //                f.DETPOS = dap.POS;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
            //        if (dap == null)
            //        {
            //            agente = d.USUARIOD_ID;
            //            f.DETPOS = 98;
            //        }
            //        else
            //        {
            //            //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //            agente = dap.USUARIOA_ID;
            //            f.DETPOS = dap.POS;
            //        }
            //    }
            //}
            f.POS = ppos;
            if (agente != "")
                f.USUARIOA_ID = agente;
            else
                f.USUARIOA_ID = null;
            return f;
        }

        //MGC 11-12-2018 Agregar Contabilizador 0----------------->
        public FLUJO determinaAgenteContabilidad0(DOCUMENTO d, string user, string delega, int pos, int? loop, int sop, int fase_det)
        {
            if (delega != null)
                user = delega;
            bool fin = false;
            WFARTHAEntities db = new WFARTHAEntities();
            DET_APROB0 dap = new DET_APROB0();
            FLUJO f_actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).FirstOrDefault();
            //DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
            //                    a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.VERSION == f_actual.DETVER).FirstOrDefault();

            ////MGC 12-11-2018 El usuario aprobador, no tiene como tal una versión, se toma el actual-------------------------------------------------------------------->

            //dap = db.DET_APROB.Where(ap => ap.VERSION == f_actual.RUTA_VERSION && ap.ID_SOCIEDAD == d.SOCIEDAD_ID).OrderBy(apo => apo.VERSION).FirstOrDefault();
            dap = db.DET_APROB0.Where(ap => ap.ID_SOCIEDAD == d.SOCIEDAD_ID && ap.VERSION == f_actual.VERSIONC1).OrderBy(apo => apo.VERSION).FirstOrDefault();

            ////MGC 12-11-2018 El usuario aprobador, no tiene como tal una versión, se toma el actual--------------------------------------------------------------------<

            int ppos = 0;

            //dap = db.DET_AGENTECA.Where(a => a.VERSION == dah.VERSION && a.ID_RUTA_AGENTE == dah.ID_RUTA_AGENTE && a.STEP == 1).FirstOrDefault();
            //dap = db.DET_AGENTECA.Where(a => a.VERSION == f_actual.RUTA_VERSION && a.ID_RUTA_AGENTE == f_actual.ID_RUTA_A && a.STEP == 1).FirstOrDefault();


            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);
            //long gaa = db.CREADOR2.Where(a => a.ID.Equals(u.ID) & a.BUKRS.Equals(d.SOCIEDAD_ID) & a.LAND.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.ACTIVO == true).FirstOrDefault().AGROUP_ID;
            //int ppos = 0;

            //if (pos.Equals(0))
            //{
            //    if (loop == null)
            //    {
            //        //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
            //        //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
            //        dap = dah.Where(a => a.STEP_FASE == 1).FirstOrDefault();
            //        dap.STEP_FASE = dap.STEP_FASE - 1;
            //    }
            //    else
            //    {
            //        FLUJO ffl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) & a.ESTATUS.Equals("R")).OrderByDescending(a => a.POS).FirstOrDefault();
            //        if (ffl.DETPOS == 99)
            //            ppos = 1;
            //        ffl.DETPOS = ffl.DETPOS - 1;
            //        fin = true;
            //        ffl.POS = ppos;

            //        return ffl;
            //    }
            //}
            //else if (pos.Equals(98))
            //{
            //    dap = dah.Where(a => a.STEP_FASE == (pos + 1)).FirstOrDefault();
            //}
            //else
            //{
            //    //DET_AGENTE actual = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos)).FirstOrDefault();
            //    DET_AGENTECA actual = dah.Where(a => a.POS == (pos)).FirstOrDefault();
            //    if (actual.STEP_FASE == 99)
            //    {
            //        fin = true;
            //    }
            //    else if (actual.STEP_FASE == 98)
            //    {
            //        //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
            //        dap = dah.Where(a => a.STEP_FASE == (pos)).FirstOrDefault();
            //    }
            //    else
            //    {
            //        if (actual.MONTO != null)
            //            if (d.MONTO_DOC_ML2 > actual.MONTO)
            //            {
            //                dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //                ppos = -1;
            //            }
            //        //if (actual.PRESUPUESTO != null)
            //        if ((bool)actual.PRESUPUESTO)
            //            if (d.MONTO_DOC_MD > 100000)
            //            {
            //                //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
            //                dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //                ppos = -1;
            //            }
            //    }
            //}

            //agente = dap.USUARIOA_ID;


            string agente = "";
            FLUJO f = new FLUJO();
            f.DETPOS = 0;


            agente = dap.ID_USUARIO;
            //f.DETPOS = dap.POS;
            f.DETPOS = 1;
            f.STEP_AUTO = dap.STEP_FASE;
            //if (!fin)
            //{
            //    if (dap != null)
            //    {
            //        if (dap.USUARIOA_ID != null)
            //        {
            //            //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //            agente = dap.USUARIOA_ID;
            //            f.DETPOS = dap.POS;
            //        }
            //        else
            //        {
            //            dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
            //            if (dap == null)
            //            {
            //                agente = d.USUARIOD_ID;
            //                f.DETPOS = 98;
            //            }
            //            else
            //            {
            //                //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //                agente = dap.USUARIOA_ID;
            //                f.DETPOS = dap.POS;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
            //        if (dap == null)
            //        {
            //            agente = d.USUARIOD_ID;
            //            f.DETPOS = 98;
            //        }
            //        else
            //        {
            //            //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //            agente = dap.USUARIOA_ID;
            //            f.DETPOS = dap.POS;
            //        }
            //    }
            //}
            f.POS = ppos;
            if (agente != "")
                f.USUARIOA_ID = agente;
            else
                f.USUARIOA_ID = null;
            return f;
        }
        //MGC 11-12-2018 Agregar Contabilizador 0-----------------<
        //Obtener el agente que se ajusta de a partir del monto del documento
        public DET_AGENTECA detAgenteLimite(List<DET_AGENTECA> lag, decimal monto)
        {
            List<DET_AGENTECA> lr = new List<DET_AGENTECA>();
            foreach (DET_AGENTECA ag in lag)
            {
                if (monto < ag.LIM_SUP)
                {
                    lr.Add(ag);
                }
            }

            //Se obtuvieron los registros que pueden validar el monto
            DET_AGENTECA rv = new DET_AGENTECA();

            rv = lr.OrderBy(ll => ll.LIM_SUP).FirstOrDefault();


            return rv;
        }

        //Determinar si hay agentes en la fase especificada
        public bool detAgenteSiguiente(decimal num_doc, FLUJO f_actual)
        {

            WFARTHAEntities db = new WFARTHAEntities();
            List<DET_AGENTECA> dah = new List<DET_AGENTECA>();
            dah = db.DET_AGENTECA.Where(a => a.VERSION == f_actual.RUTA_VERSION && a.ID_RUTA_AGENTE == f_actual.ID_RUTA_A && a.STEP_FASE == (f_actual.STEP_AUTO + 1)).ToList();
            bool res = false;

            if (dah != null && dah.Count > 0)
            {
                res = true;
            }

            return res;
        }

        public FLUJO determinaAgenteI(DOCUMENTO d, string user, string delega, int pos, DET_AGENTECA dah)
        {
            if (delega != null)
                user = delega;
            bool fin = false;
            WFARTHAEntities db = new WFARTHAEntities();
            DET_AGENTECA dap = new DET_AGENTECA();
            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);

            if (pos.Equals(0))
            {
                //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
                //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
                //MGC COMM
                //dap = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(delega) & a.PAIS_ID == dah.PAIS_ID &
                //                   a.VKORG.Equals(dah.VKORG) & a.VTWEG.Equals(dah.VTWEG) & a.SPART.Equals(dah.SPART) & a.KUNNR.Equals(dah.KUNNR) &
                //                   a.VERSION == dah.VERSION & a.POS == 1).FirstOrDefault();
                //dap = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(delega) &
                //                   a.VERSION == dah.VERSION & a.POS == 1).FirstOrDefault();
                //dap = db.DET_AGENTECC.Where(a => a.USUARIOC_ID.Equals(delega) &
                //                   a.VERSION == dah.VERSION & a.POS == 1).FirstOrDefault();

                //Obtener lista de agentes y obtener uno dependiendo del monto -----------------------------------------------------
                List<DET_AGENTECA> dal = db.DET_AGENTECA.Where(a => a.VERSION == dah.VERSION && a.ID_RUTA_AGENTE == dah.ID_RUTA_AGENTE && a.STEP_FASE == dah.STEP_FASE).OrderByDescending(a => a.VERSION).ToList();

                dap = detAgenteLimite(dal, Convert.ToDecimal(d.MONTO_DOC_MD));

                //dap = db.DET_AGENTECA.Where(a => a.VERSION == dah.VERSION && a.ID_RUTA_AGENTE == dah.ID_RUTA_AGENTE && a.STEP_FASE == dah.STEP_FASE).FirstOrDefault();


            }

            string agente = "";
            FLUJO f = new FLUJO();
            f.DETPOS = 0;
            if (!fin)
            {
                //agente = dap.USUARIOA_ID;
                agente = dap.AGENTE_SIG;
                //f.DETPOS = dap.POS;
                f.DETPOS = dap.STEP_FASE;
                f.STEP_AUTO = dap.STEP_FASE;
            }
            f.USUARIOA_ID = agente;
            return f;
        }
        //public FLUJO determinaAgente(DOCUMENTO d, string user, string delega, int pos, int? loop, int sop)
        //{
        //    if (delega != null)
        //        user = delega;
        //    bool fin = false;
        //    TAT001Entities db = new TAT001Entities();
        //    DET_AGENTEC dap = new DET_AGENTEC();
        //    FLUJO f_actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).FirstOrDefault();
        //    //DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
        //    //                    a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.VERSION == f_actual.DETVER).FirstOrDefault();
        //    List<DET_AGENTEC> dah = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(d.USUARIOD_ID) & a.PAIS_ID == d.PAIS_ID &
        //                        a.VKORG.Equals(d.VKORG) & a.VTWEG.Equals(d.VTWEG) & a.SPART.Equals(d.SPART) & a.KUNNR.Equals(d.PAYER_ID))
        //                        .OrderByDescending(a => a.VERSION).ToList();

        //    USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);
        //    //long gaa = db.CREADOR2.Where(a => a.ID.Equals(u.ID) & a.BUKRS.Equals(d.SOCIEDAD_ID) & a.LAND.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.ACTIVO == true).FirstOrDefault().AGROUP_ID;
        //    int ppos = 0;

        //    if (pos.Equals(0))
        //    {
        //        if (loop == null)
        //        {
        //            //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
        //            //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
        //            dap = dah.Where(a => a.POS == 1).FirstOrDefault();
        //            dap.POS = dap.POS - 1;
        //        }
        //        else
        //        {
        //            FLUJO ffl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) & a.ESTATUS.Equals("R")).OrderByDescending(a => a.POS).FirstOrDefault();
        //            if (ffl.DETPOS == 99)
        //                ppos = 1;
        //            ffl.DETPOS = ffl.DETPOS - 1;
        //            fin = true;
        //            ffl.POS = ppos;

        //            return ffl;
        //        }
        //    }
        //    else if (pos.Equals(98))
        //    {
        //        dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
        //    }
        //    else
        //    {
        //        //DET_AGENTE actual = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos)).FirstOrDefault();
        //        DET_AGENTEC actual = dah.Where(a => a.POS == (pos)).FirstOrDefault();
        //        if (actual.POS == 99)
        //        {
        //            fin = true;
        //        }
        //        else if (actual.POS == 98)
        //        {
        //            //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
        //            dap = dah.Where(a => a.POS == (pos)).FirstOrDefault();
        //        }
        //        else
        //        {
        //            if (actual.MONTO != null)
        //                if (d.MONTO_DOC_ML2 > actual.MONTO)
        //                {
        //                    dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
        //                    ppos = -1;
        //                }
        //            //if (actual.PRESUPUESTO != null)
        //            if ((bool)actual.PRESUPUESTO)
        //                if (d.MONTO_DOC_MD > 100000)
        //                {
        //                    //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
        //                    dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
        //                    ppos = -1;
        //                }
        //        }
        //    }

        //    string agente = "";
        //    FLUJO f = new FLUJO();
        //    f.DETPOS = 0;
        //    if (!fin)
        //    {
        //        if (dap != null)
        //        {
        //            if (dap.USUARIOA_ID != null)
        //            {
        //                //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
        //                agente = dap.USUARIOA_ID;
        //                f.DETPOS = dap.POS;
        //            }
        //            else
        //            {
        //                dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
        //                if (dap == null)
        //                {
        //                    agente = d.USUARIOD_ID;
        //                    f.DETPOS = 98;
        //                }
        //                else
        //                {
        //                    //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
        //                    agente = dap.USUARIOA_ID;
        //                    f.DETPOS = dap.POS;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
        //            if (dap == null)
        //            {
        //                agente = d.USUARIOD_ID;
        //                f.DETPOS = 98;
        //            }
        //            else
        //            {
        //                //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
        //                agente = dap.USUARIOA_ID;
        //                f.DETPOS = dap.POS;
        //            }
        //        }
        //    }
        //    f.POS = ppos;
        //    if (agente != "")
        //        f.USUARIOA_ID = agente;
        //    else
        //        f.USUARIOA_ID = null;
        //    return f;
        //}

        //public FLUJO determinaAgenteI(DOCUMENTO d, string user, string delega, int pos, DET_AGENTEC dah)
        //{
        //    if (delega != null)
        //        user = delega;
        //    bool fin = false;
        //    TAT001Entities db = new TAT001Entities();
        //    DET_AGENTEC dap = new DET_AGENTEC();
        //    USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);

        //    if (pos.Equals(0))
        //    {
        //        //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
        //        //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
        //        dap = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(delega) & a.PAIS_ID == dah.PAIS_ID &
        //                           a.VKORG.Equals(dah.VKORG) & a.VTWEG.Equals(dah.VTWEG) & a.SPART.Equals(dah.SPART) & a.KUNNR.Equals(dah.KUNNR) &
        //                           a.VERSION == dah.VERSION & a.POS == 1).FirstOrDefault();


        //    }

        //    string agente = "";
        //    FLUJO f = new FLUJO();
        //    f.DETPOS = 0;
        //    if (!fin)
        //    {
        //        agente = dap.USUARIOA_ID;
        //        f.DETPOS = dap.POS;
        //    }
        //    f.USUARIOA_ID = agente;
        //    return f;
        //}


        //Modificación para checar que el usuario contabilizador esté asignado a la sociedad
        ////MGC 12-11-2018 Saber si el siguiente aprobador es el contabilizador------------------------------------------------------->
        public ContabilizarRes nextContabilizar(FLUJO f, string user)
        {
            WFARTHAEntities db = new WFARTHAEntities();
            string festatus = f.ESTATUS; //MGC 26-11-2018 nO ACTUALIZAR ESTATUS A FLUJO
            //Cambio a A en la vista details
            f.ESTATUS = "A";

            //Simular el post en FlujosController
            FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();

            DOCUMENTO d = db.DOCUMENTOes.Find(f.NUM_DOC);

            FLUJO flujo = actual;
            flujo.ESTATUS = f.ESTATUS;
            flujo.FECHAM = DateTime.Now;
            flujo.COMENTARIO = f.COMENTARIO;
            flujo.USUARIOA_ID = user;

            flujo.ID_RUTA_A = f.ID_RUTA_A;
            flujo.RUTA_VERSION = f.RUTA_VERSION;
            flujo.STEP_AUTO = f.STEP_AUTO;

            //Simular el pf.procesa
            ContabilizarRes res = new ContabilizarRes();
            res = procesaConta(flujo);

            //Regresar el estatus original
            f.ESTATUS = festatus;//MGC 26-11-2018 nO ACTUALIZAR ESTATUS A FLUJO

            return res;
        }

        public ContabilizarRes procesaConta(FLUJO f)
        {

            ContabilizarRes res = new ContabilizarRes();

            res.contabilizar = false;
            res.res = false;

            WFARTHAEntities db = new WFARTHAEntities();
            FLUJO actual = new FLUJO();
            string emailsto = ""; //MGC 09-10-2018 Envío de correos
            ProcesaFlujo pf = new ProcesaFlujo();
            if (f.ESTATUS.Equals("A"))   //---------------------EN PROCESO DE APROBACIÓN
            {

                DOCUMENTO d = db.DOCUMENTOes.Find(f.NUM_DOC);//MGC 09-10-2018 Envío de correos

                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC) & a.POS == f.POS).OrderByDescending(a => a.POS).FirstOrDefault();

                if (!actual.ESTATUS.Equals("P"))
                {
                    res.contabilizar = false;//-----------------YA FUE PROCESADA
                    res.res = false;
                    return res;
                }
                else
                {
                    var wf = actual.WORKFP;
                    actual.ESTATUS = f.ESTATUS;
                    actual.FECHAM = f.FECHAM;
                    actual.COMENTARIO = f.COMENTARIO;
                    actual.USUARIOA_ID = f.USUARIOA_ID;
                    //db.Entry(actual).State = EntityState.Modified;

                    WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                    bool ban = true;
                    while (ban)         //--------------PARA LOOP
                    {
                        int next_step_a = 0;
                        int next_step_r = 0;
                        if (paso_a.NEXT_STEP != null)
                            next_step_a = (int)paso_a.NEXT_STEP;
                        if (paso_a.NS_ACCEPT != null)
                            next_step_a = (int)paso_a.NS_ACCEPT;
                        if (paso_a.NS_REJECT != null)
                            next_step_r = (int)paso_a.NS_REJECT;

                        WORKFP next = new WORKFP();
                        if (paso_a.ACCION.TIPO == "A" | paso_a.ACCION.TIPO == "N" | paso_a.ACCION.TIPO == "R" | paso_a.ACCION.TIPO == "T" | paso_a.ACCION.TIPO == "E" | paso_a.ACCION.TIPO == "B" | paso_a.ACCION.TIPO == "M")//Si está en proceso de aprobación
                        {
                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N") | f.ESTATUS.Equals("M"))//APROBAR SOLICITUD
                            {
                                //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC); //MGC 09-10-2018 Envío de correos

                                //Verificar si hay un siguiente aprobador
                                int stepnew = 0;
                                if (paso_a.ACCION.TIPO == "A")
                                {
                                    bool resl = false;

                                    resl = pf.detAgenteSiguiente(d.NUM_DOC, actual);

                                    if (resl)
                                    {
                                        next = paso_a;
                                        stepnew = Convert.ToInt32(actual.STEP_AUTO) + 1;
                                    }
                                    else
                                    {
                                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                                        stepnew = Convert.ToInt32(actual.STEP_AUTO);
                                    }
                                }

                                FLUJO nuevo = new FLUJO();
                                nuevo.WORKF_ID = paso_a.ID;
                                nuevo.WF_VERSION = paso_a.VERSION;
                                nuevo.WF_POS = next.POS;
                                nuevo.NUM_DOC = actual.NUM_DOC;
                                nuevo.POS = actual.POS + 1;
                                nuevo.LOOP = 1;//-----------------------------------cc
                                               //                                   //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
                                               //                                   //if (loop >= next.LOOPS)
                                               //                                   //{
                                               //                                   //    paso_a = next;
                                               //                                   //    continue;
                                               //                                   //}
                                               //                                   //if (loop != 0)
                                               //                                   //    nuevo.LOOP = loop + 1;
                                               //                                   //else
                                               //                                   //    nuevo.LOOP = 1;
                                DET_APROB detA = new DET_APROB();
                                if (paso_a.ACCION.TIPO == "N")
                                    actual.DETPOS = actual.DETPOS - 1;
                                int sop = 99;
                                if (next.ACCION.TIPO == "S")
                                    sop = 98;

                                nuevo.STEP_AUTO = stepnew;
                                nuevo.RUTA_VERSION = actual.RUTA_VERSION;
                                nuevo.ID_RUTA_A = actual.ID_RUTA_A;

                                //MGC 08-10-2018 Modificación para mensaje por contabilizar
                                bool contabilizar = false;


                                //Para la contabilización se obtiene otra ruta
                                if (next.ACCION.TIPO == "P")
                                {
                                    res.contabilizar = true;
                                    try
                                    {
                                        detA = determinaAgenteContabilidadAp(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop, Convert.ToInt32(actual.STEP_AUTO));
                                        contabilizar = true;//MGC 08-10-2018 Modificación para mensaje por contabilizar
                                        if (detA == null)
                                        {
                                            res.res = false;
                                        }
                                        else
                                        {
                                            res.res = true;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        res.res = false;
                                    }
                                }
                                else
                                {

                                    //detA = pf.determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop, stepnew);
                                    res.contabilizar = false;
                                    res.res = false;
                                }

                            }
                        }
                        ban = false;
                    }
                }
            }

            return res;
        }

        public DET_APROB determinaAgenteContabilidadAp(DOCUMENTO d, string user, string delega, int pos, int? loop, int sop, int fase_det)
        {

            WFARTHAEntities db = new WFARTHAEntities();
            DET_APROB dap = new DET_APROB();

            ////MGC 12-11-2018 El usuario aprobador, no tiene como tal una versión, se toma el actual-------------------------------------------------------------------->

            //dap = db.DET_APROB.Where(ap => ap.VERSION == f_actual.RUTA_VERSION && ap.ID_SOCIEDAD == d.SOCIEDAD_ID).OrderBy(apo => apo.VERSION).FirstOrDefault();
            try
            {
                dap = db.DET_APROB.Where(ap => ap.ID_SOCIEDAD == d.SOCIEDAD_ID).OrderBy(apo => apo.VERSION).FirstOrDefault();
            }
            catch (Exception)
            {

            }
            ////MGC 12-11-2018 El usuario aprobador, no tiene como tal una versión, se toma el actual--------------------------------------------------------------------<

            return dap;
        }
        ////MGC 12-11-2018 Saber si el siguiente aprobador es el contabilizador-------------------------------------------------------<
        ///
        ///
        //MGC 14-11-2018 Cadena de autorización----------------------------------------------------------------------------->
        public DET_APROB determinaAgenteContabilidadCadena(string sociedad, int version) //MGC 11-12-2018 Agregar Contabilizador 0
        {

            WFARTHAEntities db = new WFARTHAEntities();
            DET_APROB dap = new DET_APROB();

            try
            {
                //MGC 11-12-2018 Agregar Contabilizador 0
                if (version > 0)
                {
                    dap = db.DET_APROB.Where(ap => ap.ID_SOCIEDAD == sociedad && ap.VERSION == version).OrderBy(apo => apo.VERSION).FirstOrDefault();
                }
                else
                {
                    dap = db.DET_APROB.Where(ap => ap.ID_SOCIEDAD == sociedad).OrderBy(apo => apo.VERSION).FirstOrDefault();
                }
            }
            catch (Exception)
            {

            }


            return dap;
        }
        //MGC 14-11-2018 Cadena de autorización-----------------------------------------------------------------------------<

        ////MGC 11-12-2018 Agregar Contabilizador 0----------------------------------------------------------------------------->
        public DET_APROB0 determinaAgenteContabilidadCadena0(string sociedad, int version)//MGC 11-12-2018 Agregar Contabilizador 0
        {

            WFARTHAEntities db = new WFARTHAEntities();
            DET_APROB0 dap = new DET_APROB0();

            try
            {
                //MGC 11-12-2018 Agregar Contabilizador 0
                if (version > 0)
                {
                    dap = db.DET_APROB0.Where(ap => ap.ID_SOCIEDAD == sociedad && ap.VERSION == version).OrderBy(apo => apo.VERSION).FirstOrDefault();
                }
                else
                {

                    dap = db.DET_APROB0.Where(ap => ap.ID_SOCIEDAD == sociedad).OrderBy(apo => apo.VERSION).FirstOrDefault();
                }
            }
            catch (Exception)
            {

            }


            return dap;
        }
        ////MGC 11-12-2018 Agregar Contabilizador 0-----------------------------------------------------------------------------<

        //MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador--------<
        //Obtener el agente que se ajusta de a partir del monto del documento
        public DET_AGENTECA detAgenteLimite(List<DET_AGENTECA> lag, decimal monto, int step, FLUJO flujo)//MGC 19-10-2018 Cambio a detonador  
        {

            List<DET_AGENTECA> lr = new List<DET_AGENTECA>();
            //MGC 19-10-2018 Cambio a detonador 
            if (lag.Count == 0 & step == 0)
            {
                WFARTHAEntities db = new WFARTHAEntities();
                //Obtener el usuario al que se le creo la orden
                DET_AGENTECC dc = new DET_AGENTECC();

                dc = db.DET_AGENTECC.Where(dtc => dtc.VERSION == flujo.RUTA_VERSION && dtc.USUARIOC_ID == flujo.USUARIOA_ID && dtc.ID_RUTA_AGENTE == flujo.ID_RUTA_A).FirstOrDefault();

                DET_AGENTECA dt = new DET_AGENTECA();

                dt.ID_RUTA_AGENTE = flujo.ID_RUTA_A;
                dt.VERSION = Convert.ToInt32(flujo.RUTA_VERSION);
                dt.STEP_FASE = 0;
                dt.STEP_ACCION = 0;
                dt.LIM_SUP = Convert.ToDecimal(99999999999.00);
                dt.AGENTE_SIG = dc.USUARIOA_ID;

                lr.Add(dt);

            }
            //MGC 19-10-2018 Cambio a detonador 
            else
            {
                //Funcionamiento en cadena
                //List<DET_AGENTECA> lr = new List<DET_AGENTECA>();
                foreach (DET_AGENTECA ag in lag)
                {
                    if (monto < ag.LIM_SUP)
                    {
                        lr.Add(ag);
                    }
                }
            }

            //Se obtuvieron los registros que pueden validar el monto
            DET_AGENTECA rv = new DET_AGENTECA();

            rv = lr.OrderBy(ll => ll.LIM_SUP).FirstOrDefault();


            return rv;
        }

        public FLUJO determinaAgenteI(DOCUMENTO d, string user, string delega, int pos, DET_AGENTECA dah, int step, FLUJO actual)//MGC 19-10-2018 Cambio a detonador 
        {
            if (delega != null)
                user = delega;
            bool fin = false;
            WFARTHAEntities db = new WFARTHAEntities();
            DET_AGENTECA dap = new DET_AGENTECA();
            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);

            if (pos.Equals(0))
            {
                //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
                //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
                //dap = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(delega) & a.PAIS_ID == dah.PAIS_ID &
                //                   a.VKORG.Equals(dah.VKORG) & a.VTWEG.Equals(dah.VTWEG) & a.SPART.Equals(dah.SPART) & a.KUNNR.Equals(dah.KUNNR) &
                //                   a.VERSION == dah.VERSION & a.POS == 1).FirstOrDefault();

                List<DET_AGENTECA> dal = db.DET_AGENTECA.Where(a => a.VERSION == dah.VERSION && a.ID_RUTA_AGENTE == dah.ID_RUTA_AGENTE && a.STEP_FASE == dah.STEP_FASE).OrderByDescending(a => a.VERSION).ToList();

                dap = detAgenteLimite(dal, Convert.ToDecimal(d.MONTO_DOC_MD), step, actual);//MGC 19-10-2018 Cambio a detonador 


            }

            //string agente = "";
            //FLUJO f = new FLUJO();
            //f.DETPOS = 0;
            //if (!fin)
            //{
            //    agente = dap.USUARIOA_ID;
            //    f.DETPOS = dap.POS;
            //}
            //f.USUARIOA_ID = agente;
            //return f;

            string agente = "";
            FLUJO f = new FLUJO();
            f.DETPOS = 0;
            if (!fin)
            {
                //agente = dap.USUARIOA_ID;
                agente = dap.AGENTE_SIG;
                //f.DETPOS = dap.POS;
                f.DETPOS = dap.STEP_FASE;
                f.STEP_AUTO = dap.STEP_FASE;
            }
            f.USUARIOA_ID = agente;
            return f;
        }

        //MGC 21-11-2018 Si es el inicio de la solicitud y si no hay cadena de autorización, asignar al solicitante/detonador--------<

    }
}