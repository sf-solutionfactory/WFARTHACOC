using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class REPORT_MOD
    {
        public List<string> Tsol { get; set; }
        public List<string> Bukrs { get; set; }
        public List<string> Fecha { get; set; }
        public List<string> Payer { get; set; }
        public List<string> Num_pre { get; set; }
        public List<string> User { get; set; }
        public List<string> Num_doc { get; set; }
        public List<string> Monto { get; set; }
        public List<string> Moneda { get; set; }
        public List<string> Estatus { get; set; }
        public List<string> Pagado { get; set; }
    }
}