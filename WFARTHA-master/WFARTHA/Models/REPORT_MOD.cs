using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class REPORT_MOD
    {
        public string Tsol { get; set; }
        public string Fecha { get; set; }
        public decimal Num_doc { get; set; }
        public string Num_pre { get; set; }
        public string Bukrs { get; set; }
        public string Moneda { get; set; }
        public decimal Monto { get; set; }
        public string Usuario { get; set; }
        public string Estatus { get; set; }
        public string Pagado { get; set; }
        public string Epagado { get; set; }
        public string Payer { get; set; }
    }
}