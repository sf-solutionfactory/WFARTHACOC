using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WFARTHA.Models
{
    public class CadenaAutorizacionNuevo
    {
        //public IEnumerable<WFARTHA.Entities.DET_AGENTECC> C { get; set; }
        public IEnumerable<WFARTHA.Entities.DET_AGENTECCV> C { get; set; }
        public CadenaAutorizacion N { get; set; }

    }
}