using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class DOCUMENTOA_MOD
    {
        public decimal NUM_DOC { get; set; }
        public decimal POSD { get; set; }
        public int POS { get; set; }
        public string TIPO { get; set; }
        public string DESC { get; set; }
        public string CLASE { get; set; }
        public int STEP_WF { get; set; }
        public string USUARIO_ID { get; set; }
        public string PATH { get; set; }
        public bool ACTIVO { get; set; }
    }
}