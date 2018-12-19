using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class ProyectoSociedadNuevo
    {
        public IEnumerable<WFARTHA.Entities.DET_PROYECTO_DEC_V> L { get; set; }
        public ProyectoSociedad N { get; set; }
    }
}