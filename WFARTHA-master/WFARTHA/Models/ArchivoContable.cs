using WFARTHA.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Reflection;
using WFARTHA.Services;
using System.Net;

namespace WFARTHA.Models
{
    public class ArchivoContable
    {
        WFARTHAEntities db = new WFARTHAEntities();
        public string generarArchivo(decimal docum, decimal relacion, string accion, string fechacon)//MGC-14-12-2018 Modificación fechacon
        {

            string errorMessage = "";

            try
            {
                string dirFile = "";
                DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == docum).Single();
                TSOL ts = db.TSOLs.Where(tsi => tsi.ID == doc.TSOL_ID).FirstOrDefault(); 
                //CONPOSAPH tab;
                //CLIENTE clien = new CLIENTE();
                //List<DOCUMENTOF> docf = new List<DOCUMENTOF>();
                //bool hijo = false;
                //try
                //{
                //    if (relacion == 0)
                //    {
                //        tab = db.CONPOSAPHs.Where(x => x.TIPO_SOL == doc.TSOL.TSOLC
                //        && x.SOCIEDAD == doc.SOCIEDAD_ID
                //        && x.FECHA_FINVIG >= doc.FECHAF_VIG
                //        && x.FECHA_INIVIG <= DateTime.Today
                //        && x.TIPO_DOC != "KG").FirstOrDefault();
                //        if (tab == null)
                //        {
                //            tab = db.CONPOSAPHs.First(x => x.TIPO_SOL == doc.TSOL_ID
                //            && x.SOCIEDAD == doc.SOCIEDAD_ID
                //            && x.FECHA_FINVIG >= doc.FECHAF_VIG
                //            && x.FECHA_INIVIG <= DateTime.Today
                //            && x.TIPO_DOC != "KG"
                //            );
                //        }
                //    }
                //    else
                //    {
                //        tab = db.CONPOSAPHs.Where(x => x.SOCIEDAD == doc.SOCIEDAD_ID
                //        && x.FECHA_FINVIG >= doc.FECHAF_VIG
                //        && x.CONSECUTIVO == relacion
                //        ).Single();
                //        hijo = true;
                //    }
                //}
                //catch (Exception)
                //{
                //    return "No se encontro configuracion para generar documento para este tipo de solicitud";
                //}

                string txt = "";
                string msj = "";
                string[] cc;
                string cta = "";
                //try
                //{
                //    clien = db.CLIENTEs.Where(x => x.KUNNR == doc.PAYER_ID).Single();
                //}
                //catch (Exception) { }
                //try
                //{
                //    docf = db.DOCUMENTOFs.Where(x => x.NUM_DOC == docum).ToList();
                //}
                //catch (Exception) { }

                //if (tab.TIPO_DOC == "RN" || tab.TIPO_DOC == "KR")
                //{
                //    dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + tab.TIPO_SOL.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-2.txt";
                //}
                //else if (tab.TIPO_DOC == "KG")
                //{
                //    dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + tab.TIPO_SOL.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-3.txt";
                //}
                //else
                //{
                //    dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + tab.TIPO_SOL.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1.txt";
                //}
                //dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + tab.TIPO_SOL.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1.txt";
                //dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + ts.ID.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1.txt";
                //dirFile = ConfigurationManager.AppSettings["URL_SAVE"].ToString();// + @"POSTING\INBOUND_" + ts.ID.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1.txt";
                //string docname = ConfigurationManager.AppSettings["URL_SAVE"].ToString() + @"POSTING\INBOUND_" + ts.ID.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1.txt";
                //Obtener la configuración de la url desde app setting
                string carpeta = "in";// @"POSTING";//MGC 22-10-2018 Archivo local en servidor

                //Obtener la configuración de la url desde app setting
                string url_prel = "";
                try
                {
                    //url_prel = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();//MGC 22-10-2018 Archivo local en servidor
                    //url_prel += @"POSTING";//MGC 22-10-2018 Archivo local en servidor
                    url_prel = getDirPrel(carpeta);
                    dirFile = url_prel;
                }
                catch (Exception e)
                {
                    dirFile = ConfigurationManager.AppSettings["URL_PREL"].ToString() + @"in";
                }

                //MGC 22-10-2018 Archivo local en servidor
                //Verificar si el directorio existe y si hay permiso
                bool existd = ValidateIOPermission(dirFile);

                //El direcorio existe
                if (existd)
                {
                    //dirFile = ConfigurationManager.AppSettings["URL_PREL"].ToString() + @"POSTING";
                    string docname = dirFile + @"\INBOUND_CON" + ts.ID.Substring(0, 3) + docum.ToString().PadLeft(10, '0') + "-1";

                    //MGC 11-10-2018 Acciones para el encabezado -->

                    string variable = "";
                    string accionhead = "";
                    //Preliminar
                    if (accion == "P")
                    {
                        variable = "ACCION_CREAR";
                        fechacon = ""; //MGC-14-12-2018 Modificación fechacon
                    }
                    else if (accion == "R")
                    {
                        variable = "ACCION_BC";
                        fechacon = "";//MGC-14-12-2018 Modificación fechacon
                    }
                    else if (accion == "A")
                    {
                        variable = "ACCION_CONTABILIZAR";
                    }

                    //Obtener el nombre de la acción desde la bd en APPSETTING
                    try
                    {
                        accionhead = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals(variable) && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

                    }
                    catch (Exception e)
                    {

                    }

                    //MGC 11-10-2018 Acciones para el encabezado <--


                    doc.FECHAC = Fecha("D", Convert.ToDateTime(doc.FECHAC));

                    List<DetalleContab> det = new List<DetalleContab>();

                    //MemoryStream stIn = new MemoryStream();
                    try
                    {
                        FileStream fs = null;
                        fs = new FileStream(docname, FileMode.CreateNew);
                        using (StreamWriter sw = new StreamWriter(fs,System.Text.Encoding.ASCII))
                        {
                            string belnr = "";
                            string bjahr = "";
                            string bukrs = "";

                            if (accion == "R" || accion == "A")//MGC 
                            {
                                belnr = doc.NUM_PRE + "";
                                bjahr = doc.EJERCICIO_PRE + "";
                                bukrs = doc.SOCIEDAD_PRE + "";
                            }

                            //DETDOC	|TIPODOC|ACCION|BELNR|GJAHR|BUKRS DETDOC EJE	FACSINOC|CONTABILIZAR|10000000|2018|1010| //MGC 11-10-2018 Acciones para el encabezado -->
                            sw.WriteLine(
                                "1" + "|" +
                                ts.TIPO_DOCFILE.Trim() + "|" +
                                doc.NUM_DOC + "|" +
                                accionhead.Trim() + "|" +
                                belnr + "|" +
                                bjahr + "|" +
                                bukrs //MGC 19-10-2018 Cambio en archivo
                                );
                            //sw.WriteLine(""); //MGC 17-10-2018.2 Adaptación a archivo

                            //DETDOC	|TIPODOC|ACCION|BELNR|GJAHR|BUKRS DETDOC EJE	FACSINOC|CONTABILIZAR|10000000|2018|1010| //MGC 11-10-2018 Acciones para el encabezado <--

                            //Formato a fecha mes, día, año
                            sw.WriteLine(
                                "2" + "|" +
                                doc.DOCUMENTO_SAP + "|" +
                                doc.SOCIEDAD_ID.Trim() + "|" +
                                String.Format("{0:dd.MM.yyyy}", doc.FECHAC).Replace(".", "") + "|" + //Formato MGC
                                doc.MONEDA_ID.Trim() + "|" +
                                //+ "|" + //MGC 11-10-2018 Acciones para el encabezado
                                doc.REFERENCIA.Trim() + "|" +
                                doc.CONCEPTO + "|" + //MGC 11-10-2018 Acciones para el encabezado
                                "" + "|" +
                                "" + "|" +
                                doc.TIPO_CAMBIO  //MGC 11-10-2018 Acciones para el encabezado
                                +"|"+ fechacon //MGC-14-12-2018 Modificación fechacon
                                );
                            //sw.WriteLine("");//MGC 17-10-2018.2 Adaptación a archivo
                            //for (int i = 0; i < det.Count; i++)

                            //Obtener los rows H
                            List<DOCUMENTOP> lh = doc.DOCUMENTOPs.Where(docl => docl.ACCION == "H").ToList();
                            List<DOCUMENTOP> ld = doc.DOCUMENTOPs.Where(docl => docl.ACCION == "D").ToList();

                            //MGC 30-10-2018 Obtener las claves de contabilización ------------------------------------------------>
                            List<CLAVES_CONTA> cls = new List<CLAVES_CONTA>();

                            //MGC 30-10-2018 Obtener las claves a partir del tipo de solicitud
                            cls = db.CLAVES_CONTA.Where(clsi => clsi.TSOL == doc.TSOL_ID).ToList();

                            //MGC 30-10-2018 Obtener las claves de contabilización ------------------------------------------------<

                            for (int i = 0; i < lh.Count; i++)
                            {
                                string post = "";
                                string postk = "";

                                //MGC 30-10-2018 Obtener las claves de contabilización ------------------------------------------------>
                                CLAVES_CONTA clsi = cls.Where(c => c.DH == lh[i].ACCION).FirstOrDefault();

                                if (clsi != null)
                                {
                                    post = clsi.BSCHLL;
                                    postk = clsi.BSCHL;
                                }

                                //if (lh[i].ACCION == "H")
                                //{
                                //    post = "P";
                                //    if (doc.TSOL_ID == "NCC" | doc.TSOL_ID == "NCS")
                                //    {
                                //        postk = "50";
                                //    }
                                //    else
                                //    {
                                //        postk = "31";
                                //    }
                                //}
                                //else if (lh[i].ACCION == "D")
                                //{
                                //    post = "G";
                                //    if (doc.TSOL_ID == "NCC" | doc.TSOL_ID == "NCS")
                                //    {
                                //        postk = "21";
                                //    }
                                //    else
                                //    {
                                //        postk = "40";
                                //    }

                                //}

                                //MGC 30-10-2018 Obtener las claves de contabilización ------------------------------------------------<

                                string cuenta = lh[i].CUENTA + "";
                                string ccosto = lh[i].CCOSTO + "";
                                string imputacion = lh[i].IMPUTACION + "";

                                sw.WriteLine(
                                    //det[i].POS_TYPE + "|" +
                                    "3" + "|" +
                                    post + "|" +
                                    doc.SOCIEDAD_ID.Trim() + "|" + //det[i].COMP_CODE + "|" + //
                                                                   //det[i].BUS_AREA + "|" +
                                    "|" +
                                    //det[i].POST_KEY + "|" +
                                    postk + "|" +
                                    cuenta.Trim() + "|" +//det[i].ACCOUNT + "|" +
                                    ccosto.Trim() + "|" +//det[i].COST_CENTER + "|" +
                                    imputacion.Trim() + "|" +
                                    lh[i].MONTO + "|" +//det[i].BALANCE + "|" +
                                    lh[i].TEXTO + "|" + //det[i].TEXT + "|" +
                                                        //det[i].SALES_ORG + "|" +
                                                        //det[i].DIST_CHANEL + "|" +
                                    "|" +
                                    "|" +
                                    //det[i].DIVISION + "|" +
                                    "|" +
                                    //"|" +
                                    //"|" +
                                    //"|" +
                                    //"|" +
                                    //"|" +
                                    //det[i].INV_REF + "|" +
                                    //det[i].PAY_TERM + "|" +
                                    //det[i].JURIS_CODE + "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    //"|" +
                                    //det[i].CUSTOMER + "|" +
                                    //det[i].PRODUCT + "|" +
                                    "|" +
                                    "|" +
                                    lh[i].MWSKZ + "|" +//det[i].TAX_CODE + "|" +
                                                       //det[i].PLANT + "|" +
                                                       //det[i].REF_KEY1 + "|" +
                                                       //det[i].REF_KEY3 + "|" +
                                                       //det[i].ASSIGNMENT + "|" +
                                                       //det[i].QTY + "|" +
                                                       //det[i].BASE_UNIT + "|" +
                                                       //det[i].AMOUNT_LC + "|" +
                                                       //det[i].RETENCION_ID + "|"
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|"
                                    );
                            }

                            for (int i = 0; i < ld.Count; i++)
                            {
                                string post = "";
                                string postk = "";

                                //MGC 30-10-2018 Obtener las claves de contabilización ------------------------------------------------>
                                CLAVES_CONTA clsi = cls.Where(c => c.DH == ld[i].ACCION).FirstOrDefault();

                                if (clsi != null)
                                {
                                    post = clsi.BSCHLL;
                                    postk = clsi.BSCHL;
                                }

                                //if (ld[i].ACCION == "H")
                                //{
                                //    post = "P";
                                //    if (doc.TSOL_ID == "NCC" | doc.TSOL_ID == "NCS")
                                //    {
                                //        postk = "50";
                                //    }
                                //    else
                                //    {
                                //        postk = "31";
                                //    }
                                //}
                                //else if (ld[i].ACCION == "D")
                                //{
                                //    post = "G";
                                //    if (doc.TSOL_ID == "NCC" | doc.TSOL_ID == "NCS")
                                //    {
                                //        postk = "21";
                                //    }
                                //    else
                                //    {
                                //        postk = "40";
                                //    }

                                //}

                                //MGC 30-10-2018 Obtener las claves de contabilización ------------------------------------------------<

                                string cuenta = ld[i].CUENTA + "";
                                string ccosto = ld[i].CCOSTO + "";
                                string imputacion = ld[i].IMPUTACION + "";

                                sw.WriteLine(
                                    //det[i].POS_TYPE + "|" +
                                    "3" + "|" +
                                    post + "|" +
                                    doc.SOCIEDAD_ID.Trim() + "|" + //det[i].COMP_CODE + "|" + //
                                                                   //det[i].BUS_AREA + "|" +
                                    "|" +
                                    //det[i].POST_KEY + "|" +
                                    postk + "|" +
                                    cuenta.Trim() + "|" +//det[i].ACCOUNT + "|" +
                                    ccosto.Trim() + "|" +//det[i].COST_CENTER + "|" +
                                    imputacion.Trim() + "|" +
                                    ld[i].MONTO + "|" +//det[i].BALANCE + "|" +
                                    ld[i].TEXTO + "|" + //det[i].TEXT + "|" +
                                                        //det[i].SALES_ORG + "|" +
                                                        //det[i].DIST_CHANEL + "|" +
                                    "|" +
                                    "|" +
                                    //det[i].DIVISION + "|" +
                                    "|" +
                                    //"|" +
                                    //"|" +
                                    //"|" +
                                    //"|" +
                                    //"|" +
                                    //det[i].INV_REF + "|" +
                                    //det[i].PAY_TERM + "|" +
                                    //det[i].JURIS_CODE + "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    //"|" +
                                    //det[i].CUSTOMER + "|" +
                                    //det[i].PRODUCT + "|" +
                                    "|" +
                                    "|" +
                                    ld[i].MWSKZ + "|" +//det[i].TAX_CODE + "|" +
                                                       //det[i].PLANT + "|" +
                                                       //det[i].REF_KEY1 + "|" +
                                                       //det[i].REF_KEY3 + "|" +
                                                       //det[i].ASSIGNMENT + "|" +
                                                       //det[i].QTY + "|" +
                                                       //det[i].BASE_UNIT + "|" +
                                                       //det[i].AMOUNT_LC + "|" +
                                                       //det[i].RETENCION_ID + "|"
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|" +
                                    "|"
                                    );
                            }

                            //MGC 11-10-2018 Acciones para el encabezado RETENCIONES -->
                            for (int i = 0; i < doc.DOCUMENTORs.Count; i++)
                            {
                                sw.WriteLine(
                                "4" + "|" +
                                "W" + "|" +
                                doc.DOCUMENTORs.ElementAt(i).WITHT + "|" +
                                doc.DOCUMENTORs.ElementAt(i).WT_WITHCD + "|" +
                                doc.DOCUMENTORs.ElementAt(i).BIMPONIBLE + "|" +
                                doc.DOCUMENTORs.ElementAt(i).IMPORTE_RET //+ "|" //MGC 17-10-2018.2 Adaptación a archivo

                                );
                            }
                            //MGC 11-10-2018 Acciones para el encabezado RETENCIONES <--

                            //sw.Close();
                            sw.Close();

                            //using (Stream stOut = reqFTP.GetRequestStream())
                            //{
                            //    stOut.Write(stIn.GetBuffer(), 0, (int)stIn.Length);
                            //}



                        }
                    }
                    catch (Exception e)
                    {
                        errorMessage = "Error al generar el archivo txt preliminar " + e.Message;
                    }

                }
                else
                {
                    errorMessage = "Error con el directorio para crear archivo";
                }
                ////dirFile = ConfigurationManager.AppSettings["URL_SAVE"].ToString() + @"POSTING";
                //string docname = dirFile + @"\INBOUND_" + ts.ID.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1.txt";

                ////MGC 11-10-2018 Acciones para el encabezado -->

                //string variable = "";
                //string accionhead = "";
                ////Preliminar
                //if (accion == "P")
                //{
                //    variable = "ACCION_CREAR";
                //}
                //else if (accion == "R")
                //{
                //    variable = "ACCION_BC";
                //}else if(accion == "A")
                //{
                //    variable = "ACCION_CONTABILIZAR";
                //}

                ////Obtener el nombre de la acción desde la bd en APPSETTING
                //try
                //{
                //    accionhead = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals(variable) && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

                //}
                //catch (Exception e)
                //{

                //}

                ////MGC 11-10-2018 Acciones para el encabezado <--

                ////cta = doc.GALL_ID;
                ////doc.GALL_ID = db.GALLs.Where(x => x.ID == doc.GALL_ID).Select(x => x.GRUPO_ALL).Single();
                ////var ppd = doc.GetType().GetProperties();
                ////tab.HEADER_TEXT = tab.HEADER_TEXT.Trim();
                ////if (String.IsNullOrEmpty(tab.HEADER_TEXT) == false)
                ////{
                ////    tab.HEADER_TEXT = Referencia(tab.HEADER_TEXT, doc, docf, clien);
                ////}
                ////else
                ////{
                ////    return "Agrege comando para generar texto de encabezado";
                ////}

                ////txt = "";
                ////tab.REFERENCIA = tab.REFERENCIA.Trim();
                ////if (String.IsNullOrEmpty(tab.REFERENCIA) == false)
                ////{
                ////    tab.REFERENCIA = Referencia(tab.REFERENCIA, doc, docf, clien);
                ////}
                ////else
                ////{
                ////    return "Agrege comando para generar referencia";
                ////}
                ////tab.REFERENCIA = txt;
                ////tab.NOTA = tab.NOTA.Trim();
                ////if (String.IsNullOrEmpty(tab.NOTA) == false)
                ////{
                ////    tab.NOTA = Referencia(tab.NOTA, doc, docf, clien);
                ////}
                ////tab.CORRESPONDENCIA = tab.CORRESPONDENCIA.Trim();
                ////if (String.IsNullOrEmpty(tab.CORRESPONDENCIA) == false)
                ////{
                ////    tab.CORRESPONDENCIA = Referencia(tab.CORRESPONDENCIA, doc, docf, clien);
                ////}
                ////doc.GALL_ID = cta;
                ////if (String.IsNullOrEmpty(tab.MONEDA))
                ////{
                ////    doc.MONEDA_ID = "";
                ////}

                //doc.FECHAC = Fecha("D", Convert.ToDateTime(doc.FECHAC));

                //List<DetalleContab> det = new List<DetalleContab>();
                ////List<DOCUMENTOP> det = new List<DOCUMENTOP>();
                ////msj = Detalle(doc, ref det, tab, docf, hijo);

                ////if (msj != "")
                ////{
                ////    return msj;
                ////}
                ////if (String.IsNullOrEmpty(clien.EXPORTACION) == false)
                ////{
                ////    doc.MONEDA_ID = "USD";
                ////}
                //var dir = new Files().createDir(dirFile);//RSG 01.08.2018

                //////Evaluar que se creo el directorio
                ////if (dir.Equals(""))
                ////{
                ////    using (StreamWriter sw = new StreamWriter(docname))
                ////    {

                ////        string belnr = "";
                ////        string bjahr = "";
                ////        string bukrs = "";

                ////        if (accion == "R")
                ////        {
                ////            belnr = doc.NUM_PRE + "";
                ////            bjahr = doc.EJERCICIO_PRE + "";
                ////            bukrs = doc.SOCIEDAD_PRE + "";
                ////        }

                ////        //DETDOC	|TIPODOC|ACCION|BELNR|GJAHR|BUKRS DETDOC EJE	FACSINOC|CONTABILIZAR|10000000|2018|1010| //MGC 11-10-2018 Acciones para el encabezado -->
                ////        sw.WriteLine(
                ////            "1" + "|" +
                ////            ts.TIPO_DOCFILE.Trim() + "|" +
                ////            doc.NUM_DOC + "|" +
                ////            accionhead.Trim() + "|" +
                ////            belnr + "|" +
                ////            bjahr + "|" +
                ////            bukrs + "|"
                ////            );
                ////        sw.WriteLine("");

                ////        //DETDOC	|TIPODOC|ACCION|BELNR|GJAHR|BUKRS DETDOC EJE	FACSINOC|CONTABILIZAR|10000000|2018|1010| //MGC 11-10-2018 Acciones para el encabezado <--

                ////        //Formato a fecha mes, día, año

                ////        sw.WriteLine(
                ////            "2" + "|" +
                ////            doc.DOCUMENTO_SAP + "|" +
                ////            doc.SOCIEDAD_ID.Trim() + "|"
                ////            + String.Format("{0:MM.dd.yyyy}", doc.FECHAC).Replace(".", "") + "|"
                ////            + doc.MONEDA_ID.Trim() + "|"+
                ////            //+ dir.HEADER_TEXT.Trim() + "|"
                ////            //+ dir.REFERENCIA.Trim() + "|"
                ////            //+ dir.CALC_TAXT.ToString().Replace("True", "X").Replace("False", "") + "|"
                ////            //+ dir.NOTA.Trim() + "|"
                ////            //+ dir.CORRESPONDENCIA.Trim()
                ////            //+ "|" +
                ////            doc.REFERENCIA + "|"+
                ////            doc.CONCEPTO + "|" + //MGC 11-10-2018 Acciones para el encabezado
                ////            "X" + "|" +
                ////            doc.TIPO_CAMBIO + "|" //MGC 11-10-2018 Acciones para el encabezado
                 
                ////            + ""
                ////            );
                ////        sw.WriteLine("");
                ////        //for (int i = 0; i < det.Count; i++)
                ////        for (int i = 0; i < doc.DOCUMENTOPs.Count; i++)
                ////        {
                ////            string post = "";
                ////            string postk = "";
                            
                ////            if(doc.DOCUMENTOPs.ElementAt(i).ACCION == "H")
                ////            {
                ////                post = "P";
                ////                postk = "31";
                ////            }
                ////            else if (doc.DOCUMENTOPs.ElementAt(i).ACCION == "D")
                ////            {
                ////                post = "G";
                ////                postk = "40";
                                
                ////            }
                ////            sw.WriteLine(
                ////                //det[i].POS_TYPE + "|" +
                ////                "3" + "|" +
                ////                post + "|" +
                ////                doc.SOCIEDAD_ID.Trim() + "|" + //det[i].COMP_CODE + "|" + //
                ////                //det[i].BUS_AREA + "|" +
                ////                "|" +
                ////                //det[i].POST_KEY + "|" +
                ////                postk + "|" +
                ////                doc.DOCUMENTOPs.ElementAt(i).CUENTA + "|" +//det[i].ACCOUNT + "|" +
                ////                doc.DOCUMENTOPs.ElementAt(i).CCOSTO + "|" +//det[i].COST_CENTER + "|" +
                ////                doc.DOCUMENTOPs.ElementAt(i).IMPUTACION + "|" +
                ////                doc.DOCUMENTOPs.ElementAt(i).MONTO + "|" +//det[i].BALANCE + "|" +
                ////                "TEXTO PRUEBA " + i +"|" + //det[i].TEXT + "|" +
                ////                //det[i].SALES_ORG + "|" +
                ////                //det[i].DIST_CHANEL + "|" +
                ////                "|" +
                ////                "|" +
                ////                //det[i].DIVISION + "|" +
                ////                "|" +
                ////                //"|" +
                ////                //"|" +
                ////                //"|" +
                ////                //"|" +
                ////                //"|" +
                ////                //det[i].INV_REF + "|" +
                ////                //det[i].PAY_TERM + "|" +
                ////                //det[i].JURIS_CODE + "|" +
                ////                "|" +
                ////                "|" +
                ////                "|" +
                ////                //"|" +
                ////                //det[i].CUSTOMER + "|" +
                ////                //det[i].PRODUCT + "|" +
                ////                "|" +
                ////                "|" +
                ////                doc.DOCUMENTOPs.ElementAt(i).MWSKZ + "|" +//det[i].TAX_CODE + "|" +
                ////                //det[i].PLANT + "|" +
                ////                //det[i].REF_KEY1 + "|" +
                ////                //det[i].REF_KEY3 + "|" +
                ////                //det[i].ASSIGNMENT + "|" +
                ////                //det[i].QTY + "|" +
                ////                //det[i].BASE_UNIT + "|" +
                ////                //det[i].AMOUNT_LC + "|" +
                ////                //det[i].RETENCION_ID + "|"
                ////                "|" +
                ////                "|" +
                ////                "|" +
                ////                "|" +
                ////                "|" +
                ////                "|" +
                ////                "|" +
                ////                "|"
                ////                );
                ////        }


                ////        //MGC 11-10-2018 Acciones para el encabezado RETENCIONES -->
                ////        for (int i = 0; i < doc.DOCUMENTORs.Count; i++)
                ////        {
                ////            sw.WriteLine(
                ////            "4" + "|" +
                ////            "W" + "|" +
                ////            doc.DOCUMENTORs.ElementAt(i).WITHT + "|" +
                ////            doc.DOCUMENTORs.ElementAt(i).WT_WITHCD + "|" +
                ////            doc.DOCUMENTORs.ElementAt(i).BIMPONIBLE + "|" +
                ////            doc.DOCUMENTORs.ElementAt(i).IMPORTE_RET + "|" +
                ////            ""
                ////            );
                ////        }
                ////        //MGC 11-10-2018 Acciones para el encabezado RETENCIONES <--
                ////        sw.Close();
                ////    }

                ////}
                ////else
                ////{
                ////    errorMessage = dir;
                ////}

                ////MGC prueba FTP---------------------------------------------------------------------------------------------------------------------------------------->

                ////Obtener la configuración de la url desde app setting
                //string ftpServerIP = "";
                //try
                //{
                //    ftpServerIP = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_FTP_PRELIMINAR") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();
                //    url_prel += @"POSTING";
                //    dirFile = url_prel;
                //}
                //catch (Exception e)
                //{

                //}
                //string targetFileName = "/SAP/POSTING/INBOUND_PREL" + ts.ID.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1.txt";

                ////string ftpServerIP = "192.168.32.207:21";
                ////string targetFileName = "/SAP/POSTING/prueba.txt";
                //string username = "matias.gallegos";
                //string password = "Mimapo-2179=p23";

                //Uri uri = new Uri(String.Format("ftp://{0}/{1}", ftpServerIP, targetFileName));
                //FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                //reqFTP.Credentials = new NetworkCredential(username, password);
                //reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                //reqFTP.KeepAlive = false;
                //reqFTP.UsePassive = false;


                //MemoryStream stIn = new MemoryStream();
                //using (StreamWriter sw = new StreamWriter(stIn))
                //{
                //    string belnr = "";
                //    string bjahr = "";
                //    string bukrs = "";

                //    if (accion == "R")
                //    {
                //        belnr = doc.NUM_PRE + "";
                //        bjahr = doc.EJERCICIO_PRE + "";
                //        bukrs = doc.SOCIEDAD_PRE + "";
                //    }

                //    //DETDOC	|TIPODOC|ACCION|BELNR|GJAHR|BUKRS DETDOC EJE	FACSINOC|CONTABILIZAR|10000000|2018|1010| //MGC 11-10-2018 Acciones para el encabezado -->
                //    sw.WriteLine(
                //        "1" + "|" +
                //        ts.TIPO_DOCFILE.Trim() + "|" +
                //        doc.NUM_DOC + "|" +
                //        accionhead.Trim() + "|" +
                //        belnr + "|" +
                //        bjahr + "|" +
                //        bukrs //MGC 19-10-2018 Cambio en archivo
                //        );

                //    //DETDOC	|TIPODOC|ACCION|BELNR|GJAHR|BUKRS DETDOC EJE	FACSINOC|CONTABILIZAR|10000000|2018|1010| //MGC 11-10-2018 Acciones para el encabezado <--

                //    //Formato a fecha mes, día, año
                //    sw.WriteLine(
                //        "2" + "|" +
                //        doc.DOCUMENTO_SAP + "|" +
                //        doc.SOCIEDAD_ID.Trim() + "|" +
                //        String.Format("{0:dd.MM.yyyy}", doc.FECHAC).Replace(".", "") + "|" + //Formato MGC
                //        doc.MONEDA_ID.Trim() + "|" +
                //        //+ "|" + //MGC 11-10-2018 Acciones para el encabezado
                //        doc.REFERENCIA.Trim() + "|" +
                //        doc.CONCEPTO + "|" + //MGC 11-10-2018 Acciones para el encabezado
                //        "" + "|" +
                //        "" + "|" +
                //        doc.TIPO_CAMBIO  //MGC 11-10-2018 Acciones para el encabezado
                //        );
                //    //sw.WriteLine("");//MGC 17-10-2018.2 Adaptación a archivo
                //    //for (int i = 0; i < det.Count; i++)

                //    //Obtener los rows H
                //    List<DOCUMENTOP> lh = doc.DOCUMENTOPs.Where(docl => docl.ACCION == "H").ToList();
                //    List<DOCUMENTOP> ld = doc.DOCUMENTOPs.Where(docl => docl.ACCION == "D").ToList();

                //    for (int i = 0; i < lh.Count; i++)
                //    {
                //        string post = "";
                //        string postk = "";

                //        if (lh[i].ACCION == "H")
                //        {
                //            post = "P";
                //            postk = "31";
                //        }
                //        else if (lh[i].ACCION == "D")
                //        {
                //            post = "G";
                //            postk = "40";

                //        }

                //        string cuenta = lh[i].CUENTA + "";
                //        string ccosto = lh[i].CCOSTO + "";
                //        string imputacion = lh[i].IMPUTACION + "";

                //        sw.WriteLine(
                //            //det[i].POS_TYPE + "|" +
                //            "3" + "|" +
                //            post + "|" +
                //            doc.SOCIEDAD_ID.Trim() + "|" + //det[i].COMP_CODE + "|" + //
                //                                           //det[i].BUS_AREA + "|" +
                //            "|" +
                //            //det[i].POST_KEY + "|" +
                //            postk + "|" +
                //            cuenta.Trim() + "|" +//det[i].ACCOUNT + "|" +
                //            ccosto.Trim() + "|" +//det[i].COST_CENTER + "|" +
                //            imputacion.Trim() + "|" +
                //            lh[i].MONTO + "|" +//det[i].BALANCE + "|" +
                //            lh[i].TEXTO + "|" + //det[i].TEXT + "|" +
                //                                //det[i].SALES_ORG + "|" +
                //                                //det[i].DIST_CHANEL + "|" +
                //            "|" +
                //            "|" +
                //            //det[i].DIVISION + "|" +
                //            "|" +
                //            //"|" +
                //            //"|" +
                //            //"|" +
                //            //"|" +
                //            //"|" +
                //            //det[i].INV_REF + "|" +
                //            //det[i].PAY_TERM + "|" +
                //            //det[i].JURIS_CODE + "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            //"|" +
                //            //det[i].CUSTOMER + "|" +
                //            //det[i].PRODUCT + "|" +
                //            "|" +
                //            "|" +
                //            lh[i].MWSKZ + "|" +//det[i].TAX_CODE + "|" +
                //                               //det[i].PLANT + "|" +
                //                               //det[i].REF_KEY1 + "|" +
                //                               //det[i].REF_KEY3 + "|" +
                //                               //det[i].ASSIGNMENT + "|" +
                //                               //det[i].QTY + "|" +
                //                               //det[i].BASE_UNIT + "|" +
                //                               //det[i].AMOUNT_LC + "|" +
                //                               //det[i].RETENCION_ID + "|"
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|"
                //            );
                //    }

                //    for (int i = 0; i < ld.Count; i++)
                //    {
                //        string post = "";
                //        string postk = "";

                //        if (ld[i].ACCION == "H")
                //        {
                //            post = "P";
                //            postk = "31";
                //        }
                //        else if (ld[i].ACCION == "D")
                //        {
                //            post = "G";
                //            postk = "40";

                //        }

                //        string cuenta = ld[i].CUENTA + "";
                //        string ccosto = ld[i].CCOSTO + "";
                //        string imputacion = ld[i].IMPUTACION + "";

                //        sw.WriteLine(
                //            //det[i].POS_TYPE + "|" +
                //            "3" + "|" +
                //            post + "|" +
                //            doc.SOCIEDAD_ID.Trim() + "|" + //det[i].COMP_CODE + "|" + //
                //                                           //det[i].BUS_AREA + "|" +
                //            "|" +
                //            //det[i].POST_KEY + "|" +
                //            postk + "|" +
                //            cuenta.Trim() + "|" +//det[i].ACCOUNT + "|" +
                //            ccosto.Trim() + "|" +//det[i].COST_CENTER + "|" +
                //            imputacion.Trim() + "|" +
                //            ld[i].MONTO + "|" +//det[i].BALANCE + "|" +
                //            ld[i].TEXTO + "|" + //det[i].TEXT + "|" +
                //                                //det[i].SALES_ORG + "|" +
                //                                //det[i].DIST_CHANEL + "|" +
                //            "|" +
                //            "|" +
                //            //det[i].DIVISION + "|" +
                //            "|" +
                //            //"|" +
                //            //"|" +
                //            //"|" +
                //            //"|" +
                //            //"|" +
                //            //det[i].INV_REF + "|" +
                //            //det[i].PAY_TERM + "|" +
                //            //det[i].JURIS_CODE + "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            //"|" +
                //            //det[i].CUSTOMER + "|" +
                //            //det[i].PRODUCT + "|" +
                //            "|" +
                //            "|" +
                //            ld[i].MWSKZ + "|" +//det[i].TAX_CODE + "|" +
                //                               //det[i].PLANT + "|" +
                //                               //det[i].REF_KEY1 + "|" +
                //                               //det[i].REF_KEY3 + "|" +
                //                               //det[i].ASSIGNMENT + "|" +
                //                               //det[i].QTY + "|" +
                //                               //det[i].BASE_UNIT + "|" +
                //                               //det[i].AMOUNT_LC + "|" +
                //                               //det[i].RETENCION_ID + "|"
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|" +
                //            "|"
                //            );
                //    }
                //    //MGC 11-10-2018 Acciones para el encabezado RETENCIONES -->
                //    for (int i = 0; i < doc.DOCUMENTORs.Count; i++)
                //    {
                //        sw.WriteLine(
                //        "4" + "|" +
                //        "W" + "|" +
                //        doc.DOCUMENTORs.ElementAt(i).WITHT + "|" +
                //        doc.DOCUMENTORs.ElementAt(i).WT_WITHCD + "|" +
                //        doc.DOCUMENTORs.ElementAt(i).BIMPONIBLE + "|" +
                //        doc.DOCUMENTORs.ElementAt(i).IMPORTE_RET //+ "|" //MGC 17-10-2018.2 Adaptación a archivo
                //        );
                //    }
                //    //MGC 11-10-2018 Acciones para el encabezado RETENCIONES <--

                //    //sw.Close();
                //    sw.Flush();

                //    using (Stream stOut = reqFTP.GetRequestStream())
                //    {
                //        stOut.Write(stIn.GetBuffer(), 0, (int)stIn.Length);
                //    }



                //}

                //FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                //response.Close();

                ////if (tab.RELACION != 0 && tab.RELACION != null)
                ////{
                ////    return generarArchivo(docum, Convert.ToInt32(tab.RELACION));
                ////}
                ////else
                ////{
                ////return "";
                ////}
            }
            catch (Exception e)
            {
                //return "Error al generar el documento contable " + e.Message;
                errorMessage = "Error al generar el documento contable " + e.Message;
            }

            return errorMessage;
        }


        //private string Referencia(string campo, DOCUMENTO doc, List<DOCUMENTOF> docf, CLIENTE clien)
        //{
        //    string[] cc = campo.Trim().Split('+');
        //    string[] indes = new string[cc.Length];
        //    string txt = "";
        //    int index = 0;
        //    PropertyInfo[] ppdf = new PropertyInfo[1];
        //    PropertyInfo[] ppd = doc.GetType().GetProperties();
        //    if (docf.Count > 0)
        //    {
        //        ppdf = docf[0].GetType().GetProperties();
        //    }
        //    PropertyInfo[] ppdc = clien.GetType().GetProperties();
        //    try
        //    {
        //        foreach (string c in cc)
        //        {
        //            try
        //            {
        //                txt += ppd.Where(x => x.Name == c).Single().GetValue(doc);
        //                //txt += ppd[1].GetValue(doc);
        //                indes[index] = "X";
        //            }
        //            catch (Exception) { }
        //            try
        //            {
        //                if (docf.Count > 0)
        //                {
        //                    if (indes[index] != "X")
        //                    {
        //                        for (int i = 0; i < docf.Count; i++)
        //                        {
        //                            txt += ppdf.Where(x => x.Name == c).Single().GetValue(docf[i]) + ",";
        //                            indes[index] = "X";
        //                        }
        //                        txt = txt.Substring(0, txt.Length - 1);
        //                    }
        //                }
        //            }
        //            catch (Exception) { }
        //            try
        //            {
        //                if (indes[index] != "X")
        //                {
        //                    txt += ppdc.Where(x => x.Name == c).Single().GetValue(clien);
        //                    indes[index] = "X";
        //                }
        //            }
        //            catch (Exception) { }
        //            index++;
        //        }
        //    }
        //    catch (Exception)
        //    { }
        //    if (String.IsNullOrEmpty(txt))
        //    {
        //        return "";
        //    }
        //    else
        //    {
        //        return txt;
        //    }

        //}
        private DateTime Fecha(string id_fecha, DateTime fech)
        {
            DateTime fecha = DateTime.Today;
            switch (id_fecha)
            {
                case "U":
                    fecha = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    fecha = fecha.AddMonths(1).AddDays(-1);
                    break;
                case "P":
                    fecha = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case "H":
                    fecha = DateTime.Today;
                    break;
                default:
                    fecha = fech;
                    break;
            }
            return fecha;
        }
        //private string Detalle(DOCUMENTO doc, ref List<DetalleContab> contas, CONPOSAPH enca, List<DOCUMENTOF> docf, bool hijo)
        //{
        //    contas = new List<DetalleContab>();
        //    WFARTHAEntities db = new WFARTHAEntities();
        //    List<CONPOSAPP> conp = new List<CONPOSAPP>();
        //    CLIENTE clien;
        //    CUENTA cuent;
        //    TCAMBIO cambio = new TCAMBIO();
        //    TAXEOH taxh = new TAXEOH();
        //    List<TAXEOP> taxp = new List<TAXEOP>();
        //    MATERIAL material;
        //    string[] grupos;
        //    string grupo = "";
        //    string materi = "";
        //    string factura = "";
        //    try
        //    {
        //        try
        //        {
        //            conp = db.CONPOSAPPs.Where(x => x.CONSECUTIVO == enca.CONSECUTIVO).ToList();
        //        }
        //        catch (Exception f)
        //        {
        //            return "No se encontro datos de configuracion para detalle contable";
        //        }
        //        try
        //        {
        //            clien = db.CLIENTEs.Where(x => x.KUNNR == doc.PAYER_ID).Single();
        //        }
        //        catch (Exception g)
        //        {
        //            return "No se encontro datos de cliente para detalle contable";
        //        }
        //        try
        //        {
        //            cuent = db.CUENTAs.Where(x => x.PAIS_ID == doc.PAIS_ID && x.SOCIEDAD_ID == doc.SOCIEDAD_ID && x.TALL_ID == doc.TALL_ID).Single();
        //        }
        //        catch (Exception h)
        //        {
        //            return "No se encontro datos de cuenta para detalle contable";
        //        }
        //        try
        //        {
        //            string pais = db.TAX_LAND.Where(x => x.ACTIVO == true && x.PAIS_ID == doc.PAIS_ID).Select(x => x.PAIS_ID).Single();
        //            if (String.IsNullOrEmpty(pais) == false)
        //            {
        //                if (enca.TIPO_DOC == "DG" || enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "KG")
        //                {
        //                    try
        //                    {
        //                        taxh = db.TAXEOHs.Where(x => x.PAIS_ID == doc.PAIS_ID && x.SOCIEDAD_ID == doc.SOCIEDAD_ID && x.KUNNR == clien.KUNNR && x.CONCEPTO_ID == doc.CONCEPTO_ID).Single();
        //                    }
        //                    catch (Exception)
        //                    {
        //                        return "No se encontro datos de configuracion de taxeo";
        //                    }
        //                }

        //            }
        //        }
        //        catch (Exception z)
        //        {
        //        }
        //        try
        //        {
        //            if (doc.PAIS_ID == "CO")
        //            {
        //                if (enca.TIPO_DOC == "DG" || enca.TIPO_DOC == "BB")
        //                {
        //                    taxp = db.TAXEOPs.Where(x => x.PAIS_ID == doc.PAIS_ID && x.SOCIEDAD_ID == doc.SOCIEDAD_ID && x.KUNNR == clien.KUNNR && x.CONCEPTO_ID == doc.CONCEPTO_ID).ToList();
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return "No se encontro configuracion para extraccion de retencion";
        //        }
        //        if (docf.Count > 0)
        //        {
        //            factura = docf[0].FACTURA;
        //        }
        //        if (String.IsNullOrEmpty(clien.EXPORTACION) == false)
        //        {
        //            try
        //            {
        //                cambio = db.TCAMBIOs.Where(x => x.FCURR == doc.MONEDA_ID && x.TCURR == "USD" && x.GDATU == DateTime.Today).Single();
        //            }
        //            catch (Exception)
        //            {
        //                return "No se encontro el cambio de moneda de la fecha actual.";
        //            }
        //        }

        //        for (int i = 0; i < conp.Count; i++)
        //        {
        //            if (conp[i].POSICION == 1)
        //            {
        //                DetalleContab conta = new DetalleContab();
        //                conta.POS_TYPE = conp[i].KEY;
        //                conta.ACCOUNT = cuent.ABONO.ToString();
        //                conta.COMP_CODE = doc.SOCIEDAD_ID;
        //                conta.BUS_AREA = conp[i].BUS_AREA;
        //                conta.POST_KEY = conp[i].POSTING_KEY;
        //                conta.TEXT = doc.CONCEPTO;
        //                conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                if (conp[i].POSTING_KEY == "11")
        //                {
        //                    if (enca.TIPO_DOC == "BB")
        //                    {
        //                        if (doc.PAIS_ID == "CO")
        //                        {
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        }
        //                        conta.REF_KEY1 = clien.STCD1;
        //                        conta.REF_KEY3 = clien.NAME1;
        //                        if (enca.CALC_TAXT == false)
        //                        {
        //                            conta.TAX_CODE = conp[i].TAX_CODE;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        conta.REF_KEY1 = conp[i].REF_KEY1;
        //                        conta.REF_KEY3 = conp[i].REF_KEY3;
        //                    }
        //                    conta.ACCOUNT = clien.PAYER;
        //                }
        //                if (conp[i].POSTING_KEY == "31")
        //                {
        //                    conta.ACCOUNT = clien.PROVEEDOR_ID;
        //                    if (enca.TIPO_DOC == "KR" && doc.PAIS_ID == "CO")
        //                    {
        //                        conta.ASSIGNMENT = clien.PAYER;
        //                        if (enca.CALC_TAXT == false)
        //                        {
        //                            conta.TAX_CODE = taxh.IMPUESTO_ID;
        //                        }
        //                    }
        //                    if (enca.TIPO_DOC == "KG" && doc.PAIS_ID == "CO")
        //                    {
        //                        conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                    }
        //                }
        //                if (conp[i].POSTING_KEY == "50" && enca.TIPO_DOC == "RN")
        //                {
        //                    conta.ACCOUNT = cuent.CLEARING.ToString();
        //                }
        //                if (conp[i].POSTING_KEY == "50" && enca.TIPO_DOC == "SA" && hijo)
        //                {
        //                    conta.ACCOUNT = cuent.CLEARING.ToString();
        //                }
        //                if (doc.PAIS_ID == "CO" && enca.TIPO_DOC != "SA")
        //                {
        //                    if (taxp.Count > 0)
        //                    {
        //                        for (int k = 0; k < taxp.Count; k++)
        //                        {
        //                            conta.RETENCION_ID += taxp[k].RETENCION_ID + ",";
        //                        }
        //                        conta.RETENCION_ID = conta.RETENCION_ID.Substring(0, conta.RETENCION_ID.Length - 1);
        //                    }
        //                    if (enca.TIPO_SOL == "KG")
        //                    {
        //                        conta.ACCOUNT = clien.PROVEEDOR_ID;
        //                    }
        //                }
        //                contas.Add(conta);
        //            }
        //            else
        //            {
        //                List<DOCUMENTOM> docm = db.DOCUMENTOMs.Where(x => x.NUM_DOC == doc.NUM_DOC).ToList();
        //                for (int j = 0; j < docm.Count; j++)
        //                {
        //                    DetalleContab conta = new DetalleContab();
        //                    conta.POS_TYPE = conp[i].KEY;
        //                    conta.COMP_CODE = doc.SOCIEDAD_ID;
        //                    conta.BUS_AREA = conp[i].BUS_AREA;
        //                    conta.POST_KEY = conp[i].POSTING_KEY;
        //                    conta.TEXT = doc.CONCEPTO;
        //                    conta.REF_KEY1 = clien.STCD1;
        //                    conta.REF_KEY3 = clien.NAME1;
        //                    conta.SALES_ORG = clien.VKORG;
        //                    conta.DIST_CHANEL = clien.VTWEG;
        //                    conta.DIVISION = clien.SPART;
        //                    if (enca.TIPO_DOC != "RN" && doc.PAIS_ID != "CO")
        //                    {
        //                        conta.CUSTOMER = doc.PAYER_ID;
        //                        conta.PRODUCT = docm[j].MATNR;
        //                        if (conp[i].QUANTITY != null && conp[i].QUANTITY != 0)
        //                        {
        //                            conta.QTY = conp[i].QUANTITY.ToString();
        //                        }
        //                        conta.AMOUNT_LC = conp[i].BASE_UNIT;
        //                        conta.ACCOUNT = cuent.CARGO.ToString();
        //                        //conta.BALANCE = (docp[j].MONTO_APOYO * docp[j].VOLUMEN_EST).ToString();

        //                        //conta.BALANCE = docp[j].APOYO_REAL.ToString();
        //                        if (enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "DG")
        //                        {
        //                            //conta.BALANCE = docm[j].APOYO_REAL.ToString(); //KCMX notacredito
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        }
        //                        else if (enca.TIPO_DOC == "SA" && hijo)
        //                        {
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        }
        //                        else
        //                        {
        //                            //conta.BALANCE = docm[j].APOYO_EST.ToString(); //KCMX solic
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //conta.SALES_ORG =
        //                        //conta.DIST_CHANEL =
        //                        //conta.DIVISION =
        //                        conta.CUSTOMER =
        //                        conta.PRODUCT = "";
        //                        conta.ACCOUNT = cuent.ABONO.ToString();
        //                        conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        if (enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "SA")
        //                        {
        //                            if (doc.PAIS_ID == "CO")
        //                            {
        //                                //conta.SALES_ORG = clien.VKORG;
        //                                //conta.DIST_CHANEL = clien.VTWEG;
        //                                //conta.DIVISION = clien.SPART;
        //                                conta.SALES_DIST = clien.BZIRK;
        //                                conta.CUSTOMER = doc.PAYER_ID;
        //                                conta.PRODUCT = docm[j].MATNR;
        //                                //conta.REF_KEY1 = clien.STCD1;
        //                                //conta.REF_KEY3 = clien.NAME1;
        //                                conta.ACCOUNT = cuent.CARGO.ToString();
        //                                if (enca.TIPO_DOC == "SA")
        //                                {
        //                                    conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                                    conta.ASSIGNMENT = doc.PAYER_ID;
        //                                }
        //                                if (enca.TIPO_DOC == "BB")
        //                                {
        //                                    conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                                }
        //                            }

        //                        }
        //                        //if (enca.TIPO_DOC != "KG" && doc.PAIS_ID == "CO")
        //                        //{
        //                        //    //conta.BALANCE = docm[j].APOYO_EST.ToString();
        //                        //    conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        //}
        //                        if (enca.TIPO_DOC == "KG" && doc.PAIS_ID == "CO")
        //                        {
        //                            //conta.BALANCE = (doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * tax.PORC / 100)).ToString();
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                            conta.ACCOUNT = cuent.CLEARING.ToString();
        //                            conta.PRODUCT = docm[j].MATNR;
        //                            conta.CUSTOMER = doc.PAYER_ID;
        //                        }
        //                        if (enca.TIPO_DOC == "DG" && doc.PAIS_ID == "CO")
        //                        {
        //                            conta.ACCOUNT = cuent.CARGO.ToString();
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                            conta.PRODUCT = docm[j].MATNR;
        //                            conta.CUSTOMER = doc.PAYER_ID;
        //                            conta.REF_KEY1 = factura;
        //                            conta.REF_KEY3 = clien.NAME1;
        //                        }
        //                        if (enca.TIPO_DOC == "RN")
        //                        {
        //                            conta.ACCOUNT = cuent.CLEARING.ToString();
        //                        }
        //                    }
        //                    if (enca.TIPO_DOC == "KR" && doc.PAIS_ID == "CO")
        //                    {
        //                        if (enca.CALC_TAXT == false)
        //                        {
        //                            conta.TAX_CODE = taxh.IMPUESTO_ID;
        //                        }
        //                        conta.ASSIGNMENT = clien.PAYER;
        //                        conta.PRODUCT = docm[j].MATNR;
        //                        conta.CUSTOMER = doc.PAYER_ID;
        //                        conta.ACCOUNT = cuent.CLEARING.ToString();
        //                    }
        //                    else
        //                    {
        //                        if (enca.CALC_TAXT == false)
        //                        {
        //                            materi = docm[j].MATNR;
        //                            material = db.MATERIALs.Where(x => x.ID == materi).First();
        //                            grupos = conp[i].MATERIALGP.Split('+');
        //                            grupo = grupos.Where(x => x == material.MATERIALGP_ID).FirstOrDefault();
        //                            if (String.IsNullOrEmpty(grupo) == false)
        //                            {
        //                                conta.TAX_CODE = conp[i].TAXCODEGP;
        //                            }
        //                            else
        //                            {
        //                                conta.TAX_CODE = conp[i].TAX_CODE;
        //                            }
        //                        }
        //                    }
        //                    if (enca.TIPO_DOC == "DG" || enca.TIPO_DOC == "BB")
        //                    {
        //                        if (doc.PAIS_ID != "CO")
        //                        {
        //                            conta.REF_KEY1 = conp[i].REF_KEY1;
        //                            conta.REF_KEY3 = conp[i].REF_KEY3;
        //                        }
        //                    }
        //                    contas.Add(conta);
        //                    if (enca.TIPO_DOC == "RN" || enca.TIPO_DOC == "KR" || enca.TIPO_DOC == "KG")
        //                    {
        //                        break;
        //                    }
        //                }
        //                List<DOCUMENTOP> docp = db.DOCUMENTOPs.Where(x => x.NUM_DOC == doc.NUM_DOC).ToList();
        //                for (int j = 0; j < docp.Count; j++)
        //                {
        //                    DetalleContab conta = new DetalleContab();
        //                    conta.POS_TYPE = conp[i].KEY;
        //                    conta.COMP_CODE = doc.SOCIEDAD_ID;
        //                    conta.BUS_AREA = conp[i].BUS_AREA;
        //                    conta.POST_KEY = conp[i].POSTING_KEY;
        //                    conta.TEXT = doc.CONCEPTO;
        //                    conta.REF_KEY1 = clien.STCD1;
        //                    conta.REF_KEY3 = clien.NAME1;
        //                    conta.SALES_ORG = clien.VKORG;
        //                    conta.DIST_CHANEL = clien.VTWEG;
        //                    conta.DIVISION = clien.SPART;
        //                    if (enca.TIPO_DOC != "RN" && doc.PAIS_ID != "CO")
        //                    {
        //                        conta.CUSTOMER = doc.PAYER_ID;
        //                        conta.PRODUCT = docp[j].MATNR;
        //                        if (conp[i].QUANTITY != null && conp[i].QUANTITY != 0)
        //                        {
        //                            conta.QTY = conp[i].QUANTITY.ToString();
        //                        }
        //                        conta.AMOUNT_LC = conp[i].BASE_UNIT;
        //                        conta.ACCOUNT = cuent.CARGO.ToString();
        //                        //conta.BALANCE = (docp[j].MONTO_APOYO * docp[j].VOLUMEN_EST).ToString();

        //                        //conta.BALANCE = docp[j].APOYO_REAL.ToString();
        //                        if (enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "DG")
        //                        {
        //                            //conta.BALANCE = docp[j].APOYO_REAL.ToString(); //KCMX notacredito
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        }
        //                        else if (enca.TIPO_DOC == "SA" && hijo)
        //                        {
        //                            //conta.BALANCE = docp[j].APOYO_EST.ToString(); //KCMX solic
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        }
        //                        else
        //                        {
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        }

        //                    }
        //                    else
        //                    {
        //                        //conta.SALES_ORG =
        //                        //conta.DIST_CHANEL =
        //                        //conta.DIVISION =
        //                        conta.CUSTOMER =
        //                        conta.PRODUCT = "";
        //                        conta.ACCOUNT = cuent.ABONO.ToString();
        //                        conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        if (enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "SA")
        //                        {
        //                            if (doc.PAIS_ID == "CO")
        //                            {
        //                                //conta.SALES_ORG = clien.VKORG;
        //                                //conta.DIST_CHANEL = clien.VTWEG;
        //                                //conta.DIVISION = clien.SPART;
        //                                conta.SALES_DIST = clien.BZIRK;
        //                                conta.CUSTOMER = doc.PAYER_ID;
        //                                conta.PRODUCT = docp[j].MATNR;
        //                                //conta.REF_KEY1 = clien.STCD1;
        //                                //conta.REF_KEY3 = clien.NAME1;
        //                                conta.ACCOUNT = cuent.CARGO.ToString();
        //                                if (enca.TIPO_DOC == "SA")
        //                                {
        //                                    conta.ASSIGNMENT = doc.PAYER_ID;
        //                                    conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                                }
        //                                if (enca.TIPO_DOC == "BB")
        //                                {
        //                                    conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                                }
        //                            }

        //                        }
        //                        //if (enca.TIPO_DOC != "KG" && doc.PAIS_ID == "CO")
        //                        //{
        //                        //    //conta.BALANCE = docp[j].APOYO_EST.ToString();
        //                        //    conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                        //}
        //                        if (enca.TIPO_DOC == "KG" && doc.PAIS_ID == "CO")
        //                        {
        //                            //conta.BALANCE = (doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * tax.PORC / 100)).ToString();
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                            conta.ACCOUNT = cuent.CLEARING.ToString();
        //                            conta.PRODUCT = docp[j].MATNR;
        //                            conta.CUSTOMER = doc.PAYER_ID;
        //                        }
        //                        if (enca.TIPO_DOC == "DG" && doc.PAIS_ID == "CO")
        //                        {
        //                            conta.ACCOUNT = cuent.CARGO.ToString();
        //                            conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
        //                            conta.PRODUCT = docp[j].MATNR;
        //                            conta.CUSTOMER = doc.PAYER_ID;
        //                            conta.REF_KEY1 = factura;
        //                            conta.REF_KEY3 = clien.NAME1;
        //                            //conta.SALES_ORG = clien.VKORG;
        //                            //conta.DIST_CHANEL = clien.VTWEG;
        //                            //conta.DIVISION = clien.SPART;
        //                        }
        //                        if (enca.TIPO_DOC == "RN")
        //                        {
        //                            conta.ACCOUNT = cuent.CLEARING.ToString();
        //                        }
        //                    }
        //                    if (enca.TIPO_DOC == "KR" && doc.PAIS_ID == "CO")
        //                    {
        //                        if (enca.CALC_TAXT == false)
        //                        {
        //                            conta.TAX_CODE = taxh.IMPUESTO_ID;
        //                        }
        //                        conta.ASSIGNMENT = clien.PAYER;
        //                        conta.PRODUCT = docp[j].MATNR;
        //                        conta.CUSTOMER = doc.PAYER_ID;
        //                        conta.ACCOUNT = cuent.CLEARING.ToString();
        //                    }
        //                    else
        //                    {
        //                        if (enca.CALC_TAXT == true)
        //                        {
        //                            materi = docp[j].MATNR;
        //                            material = db.MATERIALs.Where(y => y.ID == materi).Single();
        //                            grupos = conp[i].MATERIALGP.Split('+');
        //                            grupo = grupos.Where(x => x == material.MATERIALGP_ID).FirstOrDefault();
        //                            if (String.IsNullOrEmpty(grupo) == false)
        //                            {
        //                                conta.TAX_CODE = conp[i].TAXCODEGP;
        //                            }
        //                            else
        //                            {
        //                                conta.TAX_CODE = conp[i].TAX_CODE;
        //                            }
        //                        }
        //                    }
        //                    if (enca.TIPO_DOC == "DG" || enca.TIPO_DOC == "BB")
        //                    {
        //                        if (doc.PAIS_ID != "CO")
        //                        {
        //                            conta.REF_KEY1 = conp[i].REF_KEY1;
        //                            conta.REF_KEY3 = conp[i].REF_KEY3;
        //                        }
        //                    }
        //                    contas.Add(conta);
        //                    if (enca.TIPO_DOC == "RN" || enca.TIPO_DOC == "KR" || enca.TIPO_DOC == "KG")
        //                    {
        //                        break;
        //                    }
        //                }


        //                if (enca.TIPO_DOC == "BB" && doc.PAIS_ID == "CO")
        //                {
        //                    DetalleContab conta = new DetalleContab();
        //                    conta.POS_TYPE = conp[i].KEY;
        //                    conta.ACCOUNT = cuent.CLEARING.ToString();
        //                    conta.BALANCE = (Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC) - Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC)).ToString();
        //                    conta.COMP_CODE = doc.SOCIEDAD_ID;
        //                    conta.BUS_AREA = conp[i].BUS_AREA;
        //                    conta.POST_KEY = conp[i].POSTING_KEY;
        //                    conta.TEXT = doc.CONCEPTO;
        //                    conta.REF_KEY1 = clien.STCD1;
        //                    conta.REF_KEY3 = clien.NAME1;
        //                    conta.CUSTOMER = doc.PAYER_ID;
        //                    conta.SALES_ORG = clien.VKORG;
        //                    conta.DIST_CHANEL = clien.VTWEG;
        //                    conta.DIVISION = clien.SPART;
        //                    conta.TAX_CODE = conp[i].TAX_CODE;
        //                    contas.Add(conta);
        //                }
        //            }
        //        }
        //        return "";
        //    }
        //    catch (Exception e)
        //    {
        //        return "Error al obtener detalle contable";
        //    }
        //}
        public decimal Conversion(decimal cantidad, string exportacion, decimal conversion, ref string amount)
        {
            if (String.IsNullOrEmpty(exportacion) == false)
            {
                amount = cantidad.ToString();
                return Decimal.Round(cantidad / conversion, 2);
            }
            else
            {
                amount = "";
                return cantidad;
            }
        }

        private bool ValidateIOPermission(string path)
        {
            string user = "";
            string pass = "";

            user = getUserPrel();
            pass = getPassPrel();
            try
            {
                //using (Impersonation.LogonUser(path, user, pass, LogonType.NewCredentials))
                //{

                try
                {
                    if (Directory.Exists(path))
                        return true;

                    else
                    {
                        Directory.CreateDirectory(path);
                        return true;
                    }
                }

                catch (Exception ex)
                {
                    return false;
                }
                //}
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string getUserPrel()
        {
            string user = "";
            try
            {
                user = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("USER_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

            }
            catch (Exception e)
            {
                user = "";
            }

            return user;

        }

        private string getPassPrel()
        {
            string pass = "";
            try
            {
                pass = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("PASS_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

            }
            catch (Exception e)
            {
                pass = "";
            }

            return pass;

        }

        private string getDirPrel(string carpeta)
        {
            string dir = "";
            try
            {
                dir = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();
                dir += carpeta;

            }
            catch (Exception e)
            {
                dir = "";
            }

            return dir;

        }
    }
    public class DetalleContab
    {
        public string POS_TYPE;
        public string COMP_CODE;
        public string BUS_AREA;
        public string POST_KEY;
        public string ACCOUNT;
        public string COST_CENTER;
        public string BALANCE;
        public string TEXT;
        public string SALES_ORG;
        public string DIST_CHANEL;
        public string DIVISION;
        public string INV_REF;
        public string PAY_TERM;
        public string JURIS_CODE;
        public string SALES_DIST;
        public string CUSTOMER;
        public string PRODUCT;
        public string TAX_CODE;
        public string PLANT;
        public string REF_KEY1;
        public string REF_KEY3;
        public string ASSIGNMENT;
        public string QTY;
        public string BASE_UNIT;
        public string AMOUNT_LC;
        public string RETENCION_ID;
    }

}