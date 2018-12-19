using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class DOCUMENTOP_MODSTR
    {
        public decimal NUM_DOC { get; set; }
        public decimal POS { get; set; }
        public string ACCION { get; set; }
        public string FACTURA { get; set; }
        public string TCONCEPTO { get; set; }//LEJGG 05-11-18
        public string GRUPO { get; set; }
        public string CUENTA { get; set; }
        public string NOMCUENTA { get; set; }
        public string TIPOIMP { get; set; }
        public string IMPUTACION { get; set; }
        public string CCOSTO { get; set; }//LEJGG 05-11-18
        public string MONTO { get; set; }
        public string IVA { get; set; }
        public string TEXTO { get; set; }
        public string TOTAL { get; set; }
        public string IMPUESTOT { get; set; }  //FRT06112018 Se egreaga para mostrar en pantalla
        public string MWSKZ { get; set; }  //FRT08112018 Se egreaga para mostrar en pantalla
    }
}