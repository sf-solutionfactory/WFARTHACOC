using DocumentFormat.OpenXml.Drawing.Charts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WFARTHA.Entities;

namespace WFARTHA.Models
{
    public class ReportEsqueleto
    {
        public string crearPDF(ReportCabecera RC, List<ReportDetalle> RD, List<ReportFirmas> RF)
        {
            var fncolor = new BaseColor(255, 255, 255);
            iTextSharp.text.Font letraTabNegrita = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, fncolor);
            iTextSharp.text.Font letraTab = FontFactory.GetFont(FontFactory.HELVETICA, 7);
            iTextSharp.text.Font negritaPeque = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            iTextSharp.text.Font normalPeque = FontFactory.GetFont(FontFactory.HELVETICA, 11);
            iTextSharp.text.Font normalMasPeque = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            iTextSharp.text.Font titulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            PdfPTable tabCabecera = new PdfPTable(1);
            PdfPTable tablaDatos = new PdfPTable(1);
            PdfPTable tablaDatos1 = new PdfPTable(2);
            PdfPTable tablaDatos2 = new PdfPTable(4);
            PdfPTable tablaDatos3 = new PdfPTable(1);
            PdfPTable tablaDatos4 = new PdfPTable(6);

            string ruta = "";
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                DateTime fechaCreacion = DateTime.Now;
                string nombreArchivo = string.Format("{0}.pdf", RC.NUM_DOC + "-" + fechaCreacion.ToString(@"yyyyMMdd"));
                string rutaCompleta = HttpContext.Current.Server.MapPath("~/PdfTemp/" + nombreArchivo);
                FileStream fsDocumento = new FileStream(rutaCompleta, FileMode.Create);
                //PASO UNO DEMINIMOS EL TIPO DOCUMENTO CON LOS RESPECTIVOS MARGENES (A4,IZQ,DER,TOP,BOT)
                Document pdfDoc = new Document(PageSize.A4, 30f, 30f, 60f, 30f);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fsDocumento);

                try
                {
                    pdfDoc.Open();

                    iTextSharp.text.Image imagen2 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/images/artha.png"));
                    PdfPCell celLineaCabecera = new PdfPCell { Image = imagen2, Border = 0, Padding = 3, FixedHeight = 60f, HorizontalAlignment = PdfPCell.ALIGN_RIGHT };
                    tabCabecera.AddCell(celLineaCabecera);
                    tabCabecera.TotalWidth = pdfDoc.PageSize.Width;
                    tabCabecera.WriteSelectedRows(0, -1, 0, pdfDoc.PageSize.Top, pdfWriter.DirectContent);

                    Paragraph frase1;

                    frase1 = new Paragraph("Solicitud de Cheques", titulo) { Alignment = Element.ALIGN_LEFT }; pdfDoc.Add(frase1);
                    pdfDoc.Add(new Chunk(""));

                    //Cabecera
                    pdfDoc.Add(new Chunk(""));
                    tablaDatos1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaDatos1.SetWidthPercentage(new float[] { 100, 500 }, PageSize.A4);
                    PdfPCell celda1 = new PdfPCell(new Paragraph("Folio Portal: ", negritaPeque)); celda1.Border = 0; tablaDatos1.AddCell(celda1);
                    PdfPCell celda2 = new PdfPCell(new Paragraph(RC.NUM_DOC, normalPeque)); celda2.Border = 0; tablaDatos1.AddCell(celda2);
                    PdfPCell celda3 = new PdfPCell(new Paragraph("Folio SAP: ", negritaPeque)); celda3.Border = 0; tablaDatos1.AddCell(celda3);
                    PdfPCell celda4 = new PdfPCell(new Paragraph(RC.NUM_PRE, normalPeque)); celda4.Border = 0; tablaDatos1.AddCell(celda4);
                    PdfPCell celda5 = new PdfPCell(new Paragraph("Compañia: ", negritaPeque)); celda5.Border = 0; tablaDatos1.AddCell(celda5);
                    PdfPCell celda6 = new PdfPCell(new Paragraph(RC.SOCIEDAD_ID + " " + RC.SOCIEDAD_TEXT, normalPeque)); celda6.Border = 0; tablaDatos1.AddCell(celda6);
                    PdfPCell celda7 = new PdfPCell(new Paragraph("Nominar a: ", negritaPeque)); celda7.Border = 0; tablaDatos1.AddCell(celda7);
                    PdfPCell celda8 = new PdfPCell(new Paragraph(RC.PAYER_ID + " " + RC.PAYER_NAME1, normalPeque)); celda8.Border = 0; tablaDatos1.AddCell(celda8);
                    PdfPCell celda9 = new PdfPCell(new Paragraph("Pagar en: ", negritaPeque)); celda9.Border = 0; tablaDatos1.AddCell(celda9);
                    PdfPCell celda10 = new PdfPCell(new Paragraph("", normalPeque)); celda10.Border = 0; tablaDatos1.AddCell(celda10);
                    PdfPCell celda11 = new PdfPCell(new Paragraph("Importe: ", negritaPeque)); celda11.Border = 0; tablaDatos1.AddCell(celda11);
                    PdfPCell celda12 = new PdfPCell(new Paragraph(RC.MONTO_DOC_MD, normalPeque)); celda12.Border = 0; tablaDatos1.AddCell(celda12);
                    PdfPCell celda13 = new PdfPCell(new Paragraph("Fecha de Contabilización:", negritaPeque)); celda13.Border = 0; tablaDatos1.AddCell(celda13);
                    PdfPCell celda14 = new PdfPCell(new Paragraph("", normalPeque)); celda14.Border = 0; tablaDatos1.AddCell(celda14);
                    PdfPCell celda15 = new PdfPCell(new Paragraph("Prioridad de Pago:", negritaPeque)); celda15.Border = 0; tablaDatos1.AddCell(celda15);
                    PdfPCell celda16 = new PdfPCell(new Paragraph(RC.CONDICIONES_ID + " " + RC.CONDICIONES_TEXT, normalPeque)); celda16.Border = 0; tablaDatos1.AddCell(celda16);
                    pdfDoc.Add(tablaDatos1);
                    pdfDoc.Add(new Chunk("\n"));

                    //Tabla de detalle
                    tablaDatos2.SetWidthPercentage(new float[] { 250, 250, 50, 50 }, PageSize.A4);
                    PdfPCell Titulo1 = new PdfPCell(new Paragraph(" ", letraTabNegrita)) { Border = 0, Colspan = 4, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Titulo1);
                    PdfPCell Cabecera1 = new PdfPCell(new Paragraph("Concepto Afectado", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera1);
                    PdfPCell Cabecera2 = new PdfPCell(new Paragraph("Descripción", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera2);
                    PdfPCell Cabecera3 = new PdfPCell(new Paragraph("Factura", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera3);
                    PdfPCell Cabecera4 = new PdfPCell(new Paragraph("Importe", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera4);
                    bool col = true;
                    BaseColor fondo;
                    foreach (ReportDetalle det in RD)
                    {
                        if (col == true)
                        {
                            fondo = new BaseColor(230, 230, 230);
                            col = false;
                        }
                        else
                        {
                            fondo = new BaseColor(255, 255, 255);
                            col = true;
                        }
                        PdfPCell Cuerpo1 = new PdfPCell(new Paragraph(det.CONCEPTO + "\n" + (!string.IsNullOrEmpty(det.TEXTO) ? det.TEXTO : null), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo1);
                        PdfPCell Cuerpo2 = new PdfPCell(new Paragraph(det.DESCRIPCION, letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo2);
                        PdfPCell Cuerpo3 = new PdfPCell(new Paragraph(det.FACTURA, letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo3);
                        PdfPCell Cuerpo4 = new PdfPCell(new Paragraph("$" + det.IMPORTE, letraTab)) { Border = 1, BackgroundColor = fondo, HorizontalAlignment = Element.ALIGN_RIGHT, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo4);
                    }
                    PdfPCell Total1 = new PdfPCell(new Paragraph("", letraTabNegrita)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Total1);
                    PdfPCell Total2 = new PdfPCell(new Paragraph("", letraTabNegrita)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Total2);
                    PdfPCell Total3 = new PdfPCell(new Paragraph("Total", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Total3);
                    PdfPCell Total4 = new PdfPCell(new Paragraph("$" + RC.MONTO_DOC_MD, letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), HorizontalAlignment = Element.ALIGN_RIGHT, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Total4);
                    pdfDoc.Add(tablaDatos2);
                    pdfDoc.Add(new Chunk("\n"));

                    //Tabla Observaciones
                    tablaDatos3.SetWidthPercentage(new float[] { 600 }, PageSize.A4);
                    PdfPCell Cabecera5 = new PdfPCell(new Paragraph("Observaciones de la Solicitud", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Cabecera5);
                    PdfPCell Observacion = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(RC.CONCEPTO) ? RC.CONCEPTO : null), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Observacion);
                    //PdfPCell Espacio1 = new PdfPCell(new Paragraph(" ", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Espacio1);
                    //PdfPCell Solicitante = new PdfPCell(new Paragraph("Solicitante: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Solicitante);
                    //PdfPCell Soporte = new PdfPCell(new Paragraph("Soporte: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Soporte);
                    //PdfPCell Gerencia = new PdfPCell(new Paragraph("Gerencia: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Gerencia);
                    //PdfPCell Sub = new PdfPCell(new Paragraph("Sub Dirección: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Sub);
                    //PdfPCell Cuentas = new PdfPCell(new Paragraph("Cuentas por Pagar: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Cuentas);
                    //PdfPCell Espacio2 = new PdfPCell(new Paragraph(" ", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Espacio2);
                    PdfPCell Tesoreria = new PdfPCell(new Paragraph("Tesorería: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Tesoreria);
                    //PdfPCell Espacio3 = new PdfPCell(new Paragraph(" ", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Espacio3);
                    pdfDoc.Add(tablaDatos3);
                    pdfDoc.Add(new Chunk("\n"));

                    //Tabla Firmas
                    tablaDatos4.SetWidthPercentage(new float[] { 50, 110, 110, 110, 110, 110 }, PageSize.A4);
                    PdfPCell Titulo = new PdfPCell(new Paragraph("Firmas Electrónicas", letraTabNegrita)) { Border = 0, Colspan = 6, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Titulo);
                    foreach (ReportFirmas fir in RF)
                    {
                        PdfPCell Cabecera6 = new PdfPCell(new Paragraph("Sección", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera6);
                        PdfPCell Cabecera7 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero1) ? fir.faseletrero1 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera7);
                        PdfPCell Cabecera8 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero2) ? fir.faseletrero2 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera8);
                        PdfPCell Cabecera9 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero3) ? fir.faseletrero3 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera9);
                        PdfPCell Cabecera10 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero4) ? fir.faseletrero4 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera10);
                        PdfPCell Cabecera11 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero5) ? fir.faseletrero5 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera11);
                        PdfPCell Co1 = new PdfPCell(new Paragraph("Co./Fi.", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co1);
                        PdfPCell Co2 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena1) ? fir.usuariocadena1 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co2);
                        PdfPCell Co3 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena2) ? fir.usuariocadena2 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co3);
                        PdfPCell Co4 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena3) ? fir.usuariocadena3 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co4);
                        PdfPCell Co5 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena4) ? fir.usuariocadena4 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co5);
                        PdfPCell Co6 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena5) ? fir.usuariocadena5 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co6);
                        PdfPCell Fecha1 = new PdfPCell(new Paragraph("Fecha", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha1);
                        PdfPCell Fecha2 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham1) ? fir.fecham1 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha2);
                        PdfPCell Fecha3 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham2) ? fir.fecham2 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha3);
                        PdfPCell Fecha4 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham3) ? fir.fecham3 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha4);
                        PdfPCell Fecha5 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham4) ? fir.fecham4 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha5);
                        PdfPCell Fecha6 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham5) ? fir.fecham5 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha6);
                        PdfPCell Valida1 = new PdfPCell(new Paragraph("Valida", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida1);
                        PdfPCell Valida2 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida1) ? fir.valida1 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida2);
                        PdfPCell Valida3 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida2) ? fir.valida2 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida3);
                        PdfPCell Valida4 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida3) ? fir.valida3 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida4);
                        PdfPCell Valida5 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida4) ? fir.valida4 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida5);
                        PdfPCell Valida6 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida5) ? fir.valida5 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida6);
                    }
                    pdfDoc.Add(tablaDatos4);
                    pdfDoc.Close();

                    ruta = "/PdfTemp/" + nombreArchivo;
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                return ruta;
            }
        }
    }
}