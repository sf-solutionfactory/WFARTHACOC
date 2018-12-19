using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using WFARTHA.Entities;

namespace WFARTHA.Services
{
    public class Email
    {
        private WFARTHAEntities db = new WFARTHAEntities();
        public void enviaMailC(decimal id, bool ban, string spras, string UrlDirectory, string page, string image, string emailsto) //MGC 09-10-2018 Envío de correos
        {
            //int pagina = 203; //ID EN BASE DE DATOS
            //ViewBag.Title = "Solicitud";

            if (id != 0)
            {
                DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
                if (dOCUMENTO != null)
                {

                    //dOCUMENTO.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                    //                                        & a.VTWEG.Equals(dOCUMENTO.VTWEG)
                    //                                        & a.SPART.Equals(dOCUMENTO.SPART)
                    //                                        & a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();

                    //MGC 08-10-2018.2 Obtener los datos para el correo
                    //Obtener el mail del creador
                    
                    var workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderByDescending(a => a.POS).FirstOrDefault();
                    //ViewBag.acciones = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id) & a.ESTATUS.Equals("P") & a.USUARIOA_ID.Equals(User.Identity.Name)).FirstOrDefault();

                    string mailt = ConfigurationManager.AppSettings["mailt"];
                    string mtest = ConfigurationManager.AppSettings["mailtest"]; //B20180803 MGC Correos
                    string mailTo = "";
                    if (mtest == "X")
                        mailTo = "matias.gallegos@sf-solutionfactory.com";// mailt; //B20180803 MGC Correos  //MGC 08-10-2018.2 Obtener los datos para el correo
                    else
                        //mailTo = workflow.USUARIO.EMAIL;//MGC 09-10-2018 Envío de correos
                        mailTo = emailsto;//MGC 09-10-2018 Envío de correos
                    CONMAIL conmail = db.CONMAILs.Find(mailt);
                    if (conmail != null)
                    {
                        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(conmail.MAIL, mailTo);
                        SmtpClient client = new SmtpClient();
                        if (conmail.SSL)
                        {
                            client.Port = (int)conmail.PORT;
                            client.EnableSsl = conmail.SSL;
                            client.UseDefaultCredentials = false;
                            client.Credentials = new NetworkCredential(conmail.MAIL, conmail.PASS);
                        }
                        else
                        {
                            client.UseDefaultCredentials = true;
                            client.Credentials = new NetworkCredential(conmail.MAIL, conmail.PASS);
                        }
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Host = conmail.HOST;


                        if (workflow == null)
                            mail.Subject = "N" + dOCUMENTO.NUM_DOC + "-" + DateTime.Now.ToShortTimeString();
                        else
                            mail.Subject = workflow.ESTATUS + dOCUMENTO.NUM_DOC + "-" + DateTime.Now.ToShortTimeString();
                        mail.IsBodyHtml = true;
                        //UrlDirectory = UrlDirectory.Substring(0, UrlDirectory.LastIndexOf("/"));
                        UrlDirectory = UrlDirectory.Replace("Solicitudes/Create", "Correos/" + page);
                        UrlDirectory = UrlDirectory.Replace("Solicitudes/Details", "Correos/" + page);
                        UrlDirectory = UrlDirectory.Replace("Solicitudes/Edit", "Correos/" + page);
                        UrlDirectory = UrlDirectory.Replace("Flujos/Procesa", "Correos/" + page);
                        //UrlDirectory += "/" + dOCUMENTO.NUM_DOC + "?mail=true"; //B20180803 MGC Correos
                        UrlDirectory += "/" + dOCUMENTO.NUM_DOC + ""; //B20180803 MGC Correos
                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(UrlDirectory);
                        myRequest.Method = "GET";
                        WebResponse myResponse = myRequest.GetResponse();
                        StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                        string result = sr.ReadToEnd();
                        sr.Close();
                        myResponse.Close();

                        //mail.Body = result;//B20180803 MGC Correos
                        if (image == "")
                        {
                            image = System.Web.HttpContext.Current.Server.MapPath("~/images/artha_logo.jpg");
                        }

                        mail.AlternateViews.Add(Mail_Body(result, image));//B20180803 MGC Correos
                        mail.IsBodyHtml = true;//B20180803 MGC Correos

                        client.Send(mail);

                    }

                }
            }
            //return View(dOCUMENTO);
        }


        public void enviaMailUsuario(string emailsto, int pass , string nombre, string paterno, string materno, string id) //MGC 09-10-2018 Envío de correos
        {
            string mailt = ConfigurationManager.AppSettings["mailt"];
            string mtest = ConfigurationManager.AppSettings["mailtest"]; //B20180803 MGC Correos
            string mailTo = "";
            if (mtest == "X")
                mailTo = "fernando.rojas@sf-solutionfactory.com";// mailt; //B20180803 MGC Correos  //MGC 08-10-2018.2 Obtener los datos para el correo
            else
                //mailTo = workflow.USUARIO.EMAIL;//MGC 09-10-2018 Envío de correos
                mailTo = emailsto;//MGC 09-10-2018 Envío de correos



            CONMAIL conmail = db.CONMAILs.Find(mailt);
            
            if (conmail != null)
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(conmail.MAIL, mailTo);
                SmtpClient client = new SmtpClient();
                if (conmail.SSL)
                {
                    client.Port = (int)conmail.PORT;
                    client.EnableSsl = conmail.SSL;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(conmail.MAIL, conmail.PASS);
                }
                else
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(conmail.MAIL, conmail.PASS);
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Host = conmail.HOST;


                mail.Subject = "Usuario Creado Portal Artha Capital";
                mail.IsBodyHtml = true;


                //string cuerpo = string.Format("Estimado: {1} {2} {3}: {0}Las credenciales para utilizar el portal son las siguientes:{0}Usuario: {4}{0}Contraseña: {5}", Environment.NewLine, nombre,paterno, materno,id,pass);
                string cuerpo = "Estimado: "+ nombre +" "+ paterno +" "+ materno + " \r\n Las credenciales para utilizar el portal son las siguientes:\r\nUsuario: " + id + "\r\nContraseña:" + pass + "";
                    

                    

                //mail.Body = result;//B20180803 MGC Correos

                var image = System.Web.HttpContext.Current.Server.MapPath("~/images/artha_logo.jpg");
              
                mail.AlternateViews.Add(Mail_Body_Usuario(image,cuerpo));//B20180803 MGC Correos
                mail.IsBodyHtml = true;//B20180803 MGC Correos

                client.Send(mail);

            }


        }


        private AlternateView Mail_Body_Usuario( string path, string cuerpo)
        {

            //string path = "";
            //path = HttpContext.Current.Server.MapPath("/images/logo_kellogg.png");

            //string path = "C:/Users/matias/Documents/GitHub/TAT004/TAT001/images/logo_kellogg.png";// HttpContext.Current.Server.MapPath(@"images/6792532.jpg");
            LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
            Img.ContentId = "logo_img";

        

            AlternateView AV =
            AlternateView.CreateAlternateViewFromString(cuerpo, null, MediaTypeNames.Text.Html);
            AV.LinkedResources.Add(Img);
            return AV;
        }

        private AlternateView Mail_Body(string strr, string path)
        {

            //string path = "";
            //path = HttpContext.Current.Server.MapPath("/images/logo_kellogg.png");

            //string path = "C:/Users/matias/Documents/GitHub/TAT004/TAT001/images/logo_kellogg.png";// HttpContext.Current.Server.MapPath(@"images/6792532.jpg");
            LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
            Img.ContentId = "logo_img";

            strr = strr.Replace("\"miimg_id\"", "cid:logo_img");

            AlternateView AV =
            AlternateView.CreateAlternateViewFromString(strr, null, MediaTypeNames.Text.Html);
            AV.LinkedResources.Add(Img);
            return AV;
        }

    }
}