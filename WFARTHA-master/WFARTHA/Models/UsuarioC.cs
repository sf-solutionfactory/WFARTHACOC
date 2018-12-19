using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using WFARTHA.Entities;

namespace WFARTHA.Models
{
    public partial class UsuarioC
    {

        [DataType(DataType.Text)]
        [DisplayName("Users")]
        [UIHint("List")]
        public List<SelectListItem> usersLista { get; set; }
        public List<SelectListItem> sociedadLista { get; set; }



        public int VERSION { get; set; }
        public string ID_SOCIEDAD { get; set; }
        public string SOCIEDAD { get; set; }
        public string ID_USUARIO { get; set; }
        public string USUARIO { get; set; }
        public int STEP_FASE { get; set; }
        public string ID { get; set; }
        public string PASS { get; set; }
        public string NOMBRE { get; set; }
        public string APELLIDO_P { get; set; }
        public string APELLIDO_M { get; set; }
        public string EMAIL { get; set; }
        public string SPRAS_ID { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
        public Nullable<int> PUESTO_ID { get; set; }
        public string MANAGER { get; set; }
        public string BACKUP_ID { get; set; }
        public string BUNIT { get; set; }
        public string BUKRS { get; set; }
        public string BUTXT { get; set; }
        public int ROL { get; set; }

        public string Age { get; set; }



        public UsuarioC()
        {
            WFARTHAEntities data = new WFARTHAEntities();
            usersLista = new List<SelectListItem>();
            sociedadLista = new List<SelectListItem>();
            var qUsuario = from u in data.USUARIOs where (u.ACTIVO == true) select u;
            var qSociedad = from s in data.SOCIEDADs where (s.ACTIVO == true) select s;
            var listData = qUsuario.ToList();
            var SociedadData = qSociedad.ToList();
            foreach (var Data in listData)
            {

                string nombre = string.Format("{0} - {1} {2} {3}",
                Data.ID,
                Data.NOMBRE,
                Data.APELLIDO_P,
                Data.APELLIDO_M);


                usersLista.Add(new SelectListItem() {
                    Value = Data.ID.ToString(),
                    Text = nombre,
                });
            }

            foreach (var Data in SociedadData)
            {
                string sociedad = string.Format("{0} - {1}",
                Data.BUKRS,
                Data.BUTXT);

                sociedadLista.Add(new SelectListItem()
                {
                    Value = Data.BUKRS.ToString(),
                    Text = sociedad,
                });
            }
        }
     
    }

   


    public partial class UsuarioCt
    {
        public int VERSION { get; set; }
        public string ID_SOCIEDAD { get; set; }
        public string ID_USUARIO { get; set; }
        public int STEP_FASE { get; set; }

    }

}