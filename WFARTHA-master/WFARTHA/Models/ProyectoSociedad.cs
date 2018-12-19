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
    public partial class ProyectoSociedad
    {

        [DataType(DataType.Text)]
        [DisplayName("Proyectos")]
        [UIHint("List")]
        public List<SelectListItem> proyectoLista { get; set; }
        public List<SelectListItem> sociedadLista { get; set; }

        public string ID_PSPNR { get; set; }
        public string NOMBRE { get; set; }
        public string ID_BUKRS { get; set; }
        public string BUTXT { get; set; }

        public ProyectoSociedad()
        {
            WFARTHAEntities data = new WFARTHAEntities();
            proyectoLista = new List<SelectListItem>();
            sociedadLista = new List<SelectListItem>();
            var qProyecto = from u in data.PROYECTOes  select u;
            var qSociedad = from s in data.SOCIEDADs where (s.ACTIVO == true) select s;
            var listData = qProyecto.ToList();
            var SociedadData = qSociedad.ToList();
            foreach (var Data in listData)
            {

                string proyecto = string.Format("{0} - {1}",
                Data.ID_PSPNR,
                Data.NOMBRE);
                proyectoLista.Add(new SelectListItem() {
                    Value = Data.ID_PSPNR.ToString(),
                    Text = proyecto,
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
    
}