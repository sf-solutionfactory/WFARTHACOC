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
    public class ReportEsqueleto2
    {
        public string crearPDF(List<ReportSols> RS)
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
            PdfPTable tablaDatos2 = new PdfPTable(17);
            PdfPTable tablaDatos3 = new PdfPTable(1);
            PdfPTable tablaDatos4 = new PdfPTable(6);

            string ruta = "";
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                DateTime fechaCreacion = DateTime.Now;
                string nombreArchivo = string.Format("{0}.pdf", fechaCreacion.ToString(@"yyyyMMdd") + "_" + fechaCreacion.ToString(@"HHmmss"));
                string rutaCompleta = HttpContext.Current.Server.MapPath("~/PdfTemp/" + nombreArchivo);
                FileStream fsDocumento = new FileStream(rutaCompleta, FileMode.Create);
                //PASO UNO DEMINIMOS EL TIPO DOCUMENTO CON LOS RESPECTIVOS MARGENES (A4,IZQ,DER,TOP,BOT)
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 60f, 30f);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fsDocumento);

                try
                {
                    pdfDoc.Open();

                    iTextSharp.text.Image imagen2 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/images/artha.png"));
                    PdfPCell celLineaCabecera = new PdfPCell { Image = imagen2, Border = 0, Padding = 3, FixedHeight = 60f, HorizontalAlignment = PdfPCell.ALIGN_LEFT };
                    tabCabecera.AddCell(celLineaCabecera);
                    tabCabecera.TotalWidth = pdfDoc.PageSize.Width;
                    tabCabecera.WriteSelectedRows(0, -1, 0, pdfDoc.PageSize.Top, pdfWriter.DirectContent);

                    //Tabla
                    tablaDatos2.SetWidthPercentage(new float[] { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 }, PageSize.A4.Rotate());
                    PdfPCell Cabecera1 = new PdfPCell(new Paragraph("Tipo de Solicitud", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera1);
                    PdfPCell Cabecera2 = new PdfPCell(new Paragraph("Fecha", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera2);
                    PdfPCell Cabecera3 = new PdfPCell(new Paragraph("No. Portal", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera3);
                    PdfPCell Cabecera4 = new PdfPCell(new Paragraph("No. SAP", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera4);
                    PdfPCell Cabecera5 = new PdfPCell(new Paragraph("Proyecto", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera5);
                    PdfPCell Cabecera6 = new PdfPCell(new Paragraph("Sociedad", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera6);
                    PdfPCell Cabecera7 = new PdfPCell(new Paragraph("Moneda", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera7);
                    PdfPCell Cabecera8 = new PdfPCell(new Paragraph("Monto", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera8);
                    PdfPCell Cabecera9 = new PdfPCell(new Paragraph("Explicacion", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera9);
                    PdfPCell Cabecera10 = new PdfPCell(new Paragraph("Solicitante", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera10);
                    PdfPCell Cabecera11 = new PdfPCell(new Paragraph("Usuario", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera11);
                    PdfPCell Cabecera12 = new PdfPCell(new Paragraph("WorkFlow", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera12);
                    PdfPCell Cabecera13 = new PdfPCell(new Paragraph("Estatus Solicitud", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera13);
                    PdfPCell Cabecera14 = new PdfPCell(new Paragraph("Total de Dias", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera14);
                    PdfPCell Cabecera15 = new PdfPCell(new Paragraph("Pagado", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera15);
                    PdfPCell Cabecera16 = new PdfPCell(new Paragraph("EPagado", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera16);
                    PdfPCell Cabecera17 = new PdfPCell(new Paragraph("Via de Pago", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cabecera17);
                    bool col = true;
                    BaseColor fondo;
                    foreach (ReportSols sol in RS)
                    {
                        if (col == true)
                        {
                            fondo = new BaseColor(220, 220, 220);
                            col = false;
                        }
                        else
                        {
                            fondo = new BaseColor(255, 255, 255);
                            col = true;
                        }
                        PdfPCell Cuerpo1 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Tsol) ? sol.Tsol : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo1);
                        PdfPCell Cuerpo2 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Fecha) ? sol.Fecha : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo2);
                        PdfPCell Cuerpo3 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Num_doc.ToString()) ? sol.Num_doc.ToString() : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo3);
                        PdfPCell Cuerpo4 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Num_pre) ? sol.Num_pre : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo4);
                        PdfPCell Cuerpo5 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Pspnr) ? sol.Pspnr : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo5);
                        PdfPCell Cuerpo6 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Bukrsn) ? sol.Bukrsn : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo6);
                        PdfPCell Cuerpo7 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Moneda) ? sol.Moneda : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo7);
                        PdfPCell Cuerpo8 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Montol) ? sol.Montol : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo8);
                        PdfPCell Cuerpo9 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Explicacion) ? sol.Explicacion : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo9);
                        PdfPCell Cuerpo10 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Usuarion) ? sol.Usuarion : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo10);
                        PdfPCell Cuerpo11 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Usuario) ? sol.Usuario : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo11);
                        PdfPCell Cuerpo12 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Wf) ? sol.Wf : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo12);
                        PdfPCell Cuerpo13 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Estatus) ? sol.Estatus : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo13);
                        PdfPCell Cuerpo14 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Totald.ToString()) ? sol.Totald.ToString() : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo14);
                        PdfPCell Cuerpo15 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Pagado) ? sol.Pagado : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo15);
                        PdfPCell Cuerpo16 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Epagado) ? sol.Epagado : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo16);
                        PdfPCell Cuerpo17 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(sol.Viapago   ) ? sol.Viapago : ""), letraTab)) { Border = 1, BackgroundColor = fondo, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos2.AddCell(Cuerpo17);
                    }
                    pdfDoc.Add(tablaDatos2);
                    pdfDoc.Add(new Chunk("\n"));

                    ////Tabla Observaciones
                    //tablaDatos3.SetWidthPercentage(new float[] { 600 }, PageSize.A4);
                    //PdfPCell Cabecera5 = new PdfPCell(new Paragraph("Observaciones de la Solicitud", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Cabecera5);
                    //PdfPCell Observacion = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(RC.CONCEPTO) ? RC.CONCEPTO : null), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Observacion);
                    ////PdfPCell Espacio1 = new PdfPCell(new Paragraph(" ", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Espacio1);
                    ////PdfPCell Solicitante = new PdfPCell(new Paragraph("Solicitante: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Solicitante);
                    ////PdfPCell Soporte = new PdfPCell(new Paragraph("Soporte: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Soporte);
                    ////PdfPCell Gerencia = new PdfPCell(new Paragraph("Gerencia: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Gerencia);
                    ////PdfPCell Sub = new PdfPCell(new Paragraph("Sub Dirección: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Sub);
                    ////PdfPCell Cuentas = new PdfPCell(new Paragraph("Cuentas por Pagar: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Cuentas);
                    ////PdfPCell Espacio2 = new PdfPCell(new Paragraph(" ", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Espacio2);
                    //PdfPCell Tesoreria = new PdfPCell(new Paragraph("Tesorería: " + "", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Tesoreria);
                    ////PdfPCell Espacio3 = new PdfPCell(new Paragraph(" ", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos3.AddCell(Espacio3);
                    //pdfDoc.Add(tablaDatos3);
                    //pdfDoc.Add(new Chunk("\n"));

                    //Tabla Firmas
                    //tablaDatos4.SetWidthPercentage(new float[] { 50, 110, 110, 110, 110, 110 }, PageSize.A4);
                    //PdfPCell Titulo = new PdfPCell(new Paragraph("Firmas Electrónicas", letraTabNegrita)) { Border = 0, Colspan = 6, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Titulo);
                    //foreach (ReportFirmas fir in RF)
                    //{
                    //    PdfPCell Cabecera6 = new PdfPCell(new Paragraph("Sección", letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera6);
                    //    PdfPCell Cabecera7 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero1) ? fir.faseletrero1 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera7);
                    //    PdfPCell Cabecera8 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero2) ? fir.faseletrero2 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera8);
                    //    PdfPCell Cabecera9 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero3) ? fir.faseletrero3 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera9);
                    //    PdfPCell Cabecera10 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero4) ? fir.faseletrero4 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera10);
                    //    PdfPCell Cabecera11 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.faseletrero5) ? fir.faseletrero5 : ""), letraTabNegrita)) { Border = 1, BackgroundColor = new BaseColor(0, 53, 100), BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Cabecera11);
                    //    PdfPCell Co1 = new PdfPCell(new Paragraph("Co./Fi.", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co1);
                    //    PdfPCell Co2 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena1) ? fir.usuariocadena1 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co2);
                    //    PdfPCell Co3 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena2) ? fir.usuariocadena2 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co3);
                    //    PdfPCell Co4 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena3) ? fir.usuariocadena3 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co4);
                    //    PdfPCell Co5 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena4) ? fir.usuariocadena4 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co5);
                    //    PdfPCell Co6 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.usuariocadena5) ? fir.usuariocadena5 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Co6);
                    //    PdfPCell Fecha1 = new PdfPCell(new Paragraph("Fecha", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha1);
                    //    PdfPCell Fecha2 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham1) ? fir.fecham1 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha2);
                    //    PdfPCell Fecha3 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham2) ? fir.fecham2 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha3);
                    //    PdfPCell Fecha4 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham3) ? fir.fecham3 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha4);
                    //    PdfPCell Fecha5 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham4) ? fir.fecham4 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha5);
                    //    PdfPCell Fecha6 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.fecham5) ? fir.fecham5 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Fecha6);
                    //    PdfPCell Valida1 = new PdfPCell(new Paragraph("Valida", letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida1);
                    //    PdfPCell Valida2 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida1) ? fir.valida1 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida2);
                    //    PdfPCell Valida3 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida2) ? fir.valida2 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida3);
                    //    PdfPCell Valida4 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida3) ? fir.valida3 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida4);
                    //    PdfPCell Valida5 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida4) ? fir.valida4 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida5);
                    //    PdfPCell Valida6 = new PdfPCell(new Paragraph((!string.IsNullOrEmpty(fir.valida5) ? fir.valida5 : ""), letraTab)) { Border = 1, BorderColor = new BaseColor(240, 240, 240) }; tablaDatos4.AddCell(Valida6);
                    //}
                    //pdfDoc.Add(tablaDatos4);
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