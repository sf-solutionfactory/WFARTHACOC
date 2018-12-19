using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class DOCUMENTOR_MOD
    {

        public string LIFNR { get; set; }
        public string BUKRS { get; set; }
        public string WITHT { get; set; }
        public string WT_WITHCD { get; set; }
        public string DESC { get; set; }
        public Nullable<decimal> POS { get; set; }
        public Nullable<decimal> BIMPONIBLE { get; set; }
        public Nullable<decimal> IMPORTE_RET { get; set; }
    }
    public class listRet
    {

        public string LIFNR { get; set; }
        public string BUKRS { get; set; }
        public string WITHT { get; set; }
        public string WT_WITHCD { get; set; }
        public string DESC { get; set; }
    }
}