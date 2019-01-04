using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFARTHA.Entities;

namespace WFARTHA.Models
{
    public class DET_AGENTECA_MOD
    {
        public List<DET_AGENTECAR> LDET_ACA { get; set; }
    }

    public partial class DET_AGENTECAR
    {
        
        public int STEP_FASE { get; set; }
        public string AGENTE_SIG { get; set; }
    }
}