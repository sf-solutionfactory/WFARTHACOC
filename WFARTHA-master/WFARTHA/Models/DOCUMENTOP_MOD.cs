using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class DOCUMENTOP_MOD
    {
        public decimal NUM_DOC { get; set; }
        public decimal POS { get; set; }
        public string ACCION { get; set; }
        public string FACTURA { get; set; }
        public string TCONCEPTO { get; set; }
        public string GRUPO { get; set; }
        public string CUENTA { get; set; }
        public string NOMCUENTA { get; set; }
        public string TIPOIMP { get; set; }
        public string IMPUTACION { get; set; }
        public string CCOSTO { get; set; }
        public decimal MONTO { get; set; }
        public string MWSKZ { get; set; }
        public decimal IVA { get; set; }
        public string TEXTO { get; set; }
        public decimal TOTAL { get; set; }
    }
}