using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class ReportSols
    {
        public string Tsol { get; set; }
        public string Fecha { get; set; }
        public DateTime Fechad { get; set; }
        public decimal Num_doc { get; set; }
        public string Num_pre { get; set; }
        public string Pspnr { get; set; }
        public string Id_pspnr { get; set; }
        public string Bukrs { get; set; }
        public string Bukrsn { get; set; }
        public string Moneda { get; set; }
        public decimal Monto { get; set; }
        public string Montol { get; set; }
        public string Explicacion { get; set; }
        public string Usuarion { get; set; }
        public string Usuario { get; set; }
        public string Wf { get; set; }
        public string Estatus { get; set; }
        public string Totald { get; set; }
        public string EstPago { get; set; }
    }
}