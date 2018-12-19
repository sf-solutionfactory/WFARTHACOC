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
    public class SociedadesUsuario
    {
        [DataType(DataType.Text)]
        [DisplayName("Sociedad")]
        [UIHint("List")]
        public List<SelectListItem> usersLista { get; set; }
        public List<SelectListItem> sociedadLista { get; set; }
        public List<SelectListItem> presupuestoLista { get; set; }



    
        public string BUKRS { get; set; }
        public string ID_USER { get; set; }
        public string TIPOPRE { get; set; }
        public string BUTXT { get; set; }
        public string NOMBRE { get; set; }
        public string DESC { get; set; }



        public SociedadesUsuario()
        {
            WFARTHAEntities data = new WFARTHAEntities();
            usersLista = new List<SelectListItem>();
            sociedadLista = new List<SelectListItem>();
            presupuestoLista = new List<SelectListItem>();
            var qUsuario = from u in data.USUARIOs where (u.ACTIVO == true) select u;
            var qSociedad = from s in data.SOCIEDADs where (s.ACTIVO == true) select s;
            var qPresupuesto = from p in data.TIPOPRESUPUESTOes where (p.ESTATUS == true) select p;
            var listData = qUsuario.ToList();
            var SociedadData = qSociedad.ToList();
            var PresupuestoData = qPresupuesto.ToList();
            foreach (var Data in listData)
            {

                string nombre = string.Format("{0} - {1} {2} {3}",
                Data.ID,
                Data.NOMBRE,
                Data.APELLIDO_P,
                Data.APELLIDO_M);


                usersLista.Add(new SelectListItem()
                {
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

            foreach (var Data in PresupuestoData)
            {
                string presupuesto = string.Format("{0} - {1}",
                Data.TIPOPRE,
                Data.DESC);

                presupuestoLista.Add(new SelectListItem()
                {
                    Value = Data.TIPOPRE.ToString(),
                    Text = presupuesto,
                });
            }

        }

    }
}