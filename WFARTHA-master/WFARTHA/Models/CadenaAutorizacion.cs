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
    public partial class CadenaAutorizacion
    {

        [DataType(DataType.Text)]
        [DisplayName("Users")]
        [UIHint("List")]
        public List<SelectListItem> usersLista { get; set; }




        public int VERSION { get; set; }
        public string USUARIOC_ID { get; set; }
        public string ID_RUTA_AGENTE { get; set; }
        public string DESCRIPCION { get; set; }
        public string USUARIOA_ID { get; set; }
        public System.DateTime FECHAC { get; set; }
        public int STEP_FASE { get; set; }
        public int STEP_ACCION { get; set; }
        public decimal LIM_SUP { get; set; }
        public string AGENTE_SIG { get; set; }
        public Nullable<bool> OVERRIDE { get; set; }

        public IEnumerable<WFARTHA.Entities.DET_AGENTECCV> C { get; set; }
        public IEnumerable<WFARTHA.Entities.DET_AGENTECAV> A { get; set; }
        public CadenaAutorizacion N { get; set; }


        public List<DET_AGENTECAR> LDET_ACA { get; set; }//MGC

        public CadenaAutorizacion()
        {
            WFARTHAEntities data = new WFARTHAEntities();
            usersLista = new List<SelectListItem>();

            var qUsuario = from u in data.USUARIOs where (u.ACTIVO == true) select u;

            var listData = qUsuario.ToList();

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
        }

    }


}