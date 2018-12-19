using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class DOCUMENTOCOC_MOD
    {
        public decimal NUM_DOC { get; set; }
        public decimal POSD { get; set; }
        public Nullable<int> POS { get; set; }
        public string MATNR { get; set; }
        public string PS_PSP_PNR { get; set; }
        public string WAERS { get; set; }
        public Nullable<decimal> MENGE_BIL { get; set; }
        public string MEINS { get; set; }
        public decimal TOTAL { get; set; }
    }
}