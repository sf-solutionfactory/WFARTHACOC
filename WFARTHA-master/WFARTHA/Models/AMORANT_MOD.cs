using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class AMORANT_MOD
    {
        public int POS { get; set; }
        public decimal NUM_DOC { get; set; }
        public string EBELN { get; set; }
        public string EBELP { get; set; }
        public string BELNR { get; set; }
        public Nullable<decimal> GJAHR { get; set; }
        public Nullable<decimal> BUZEI { get; set; }
        public Nullable<decimal> ANTAMOR { get; set; }
        public string WAERS { get; set; }
        public Nullable<decimal> TANT { get; set; }
        public Nullable<decimal> ANTTRANS { get; set; }
        public Nullable<decimal> ANTXAMORT { get; set; }
    }
}