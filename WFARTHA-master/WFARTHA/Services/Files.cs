    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WFARTHA.Services
{
    public class Files
    {

        public string createDir(string path)
        {

            string ex = "";

            // Specify the path to save the uploaded file to.
            //string savePath = path + documento + "\\";
            //string savePath = path + ejercicio + "\\" + documento + "\\";//RSG 01.08.2018
            string savePath = path;
            // Create the path and file name to check for duplicates.
            string pathToCheck = savePath;

            try
            {
                ////using (Impersonation.LogonUser("192.168.1.77", "EQUIPO", "0906", LogonType.NewCredentials))
                ////{
                if (!System.IO.File.Exists(pathToCheck))
                {
                    //No existe, se necesita crear
                    DirectoryInfo dir = new DirectoryInfo(pathToCheck);

                    dir.Create();

                }
                ////}

                //file.SaveAs(Server.MapPath(savePath)); //Guardarlo el cualquier parte dentro del proyecto <add key="URL_SAVE" value="\Archivos\" />
                //System.IO.File.Create(savePath,100,FileOptions.DeleteOnClose, )
                //System.IO.File.Copy(copyFrom, savePath);
                //f.CopyTo(savePath,true);
            }
            catch (Exception e)
            {
                ex = "No se puede crear el directorio para guardar los archivos";
            }

            return ex;
        }

        public string SaveFile(HttpPostedFileBase file, string path, string documento, out string exception, out string pathsaved, string ejercicio)
        {
            string ex = "";
            //string exdir = "";
            // Get the name of the file to upload.
            string fileName = file.FileName;//System.IO.Path.GetExtension(file.FileName);    // must be declared in the class above

            // Specify the path to save the uploaded file to.
            //string savePath = path + documento + "\\";//RSG 01.08.2018
            string savePath = path + ejercicio + "\\" + documento + "\\";//RSG 01.08.2018

            // Create the path and file name to check for duplicates.
            string pathToCheck = savePath;

            // Append the name of the file to upload to the path.
            savePath += fileName;

            // Call the SaveAs method to save the uploaded
            // file to the specified directory.
            //file.SaveAs(Server.MapPath(savePath));

            //file to domain
            //Parte para guardar archivo en el servidor
            ////using (Impersonation.LogonUser("192.168.1.77", "EQUIPO", "0906", LogonType.NewCredentials))
            ////{
            //fileName = file.SaveAs(file, Server.MapPath("~/Nueva carpeta/") + file.FileName);
            try
            {


                //Guardar el archivo
                file.SaveAs(savePath);


            }
            catch (Exception e)
            {
                ex = "";
                ex = fileName;
            }
            ////}

            //Guardarlo en la base de datos
            if (ex == "")
            {

            }
            pathsaved = savePath;
            exception = ex;
            return fileName;
        }

    }
}