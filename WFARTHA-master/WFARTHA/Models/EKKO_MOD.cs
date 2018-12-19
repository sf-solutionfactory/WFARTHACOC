using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class EKKO_MOD
    {
        public string EBELN { get; set; }
        public string BUKRS { get; set; }
        public string BSART { get; set; }
        public string LIFNR { get; set; }
        public string RETTP { get; set; }
        public Nullable<decimal> RETPC { get; set; }
        public string DPTYP { get; set; }
        public Nullable<decimal> DPPCT { get; set; }
        public Nullable<decimal> DPAMT { get; set; }
        public string ESTATUS { get; set; }
    }
}