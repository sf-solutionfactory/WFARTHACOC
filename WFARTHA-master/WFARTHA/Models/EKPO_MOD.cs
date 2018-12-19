using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class EKPO_MOD
    {
        public string EBELN { get; set; }
        public string EBELP { get; set; }
        public Nullable<System.DateTime> BEDAT { get; set; }
        public string MATNR { get; set; }
        public string TXZ01 { get; set; }
        public string MATKL { get; set; }
        public string WERKS { get; set; }
        public string LGORT { get; set; }
        public Nullable<decimal> MENGE { get; set; }
        public string MEINS { get; set; }
        public Nullable<decimal> NETPR { get; set; }
        public string WAERS { get; set; }
        public Nullable<decimal> PEINH { get; set; }
        public Nullable<decimal> MENGE_DEL { get; set; }
        public Nullable<decimal> NETPR_DEL { get; set; }
        public Nullable<decimal> MENGE_BIL { get; set; }
        public Nullable<decimal> NETPR_BIL { get; set; }
        public string MWSKZ { get; set; }
        public string SAKTO { get; set; }
        public string KNTTP { get; set; }
        public string PS_PSP_PNR { get; set; }
        public string KOSTL { get; set; }
        public string ESTATUS { get; set; }
        public string EREKZ { get; set; }
        public Nullable<decimal> NETWR { get; set; }
    }
}