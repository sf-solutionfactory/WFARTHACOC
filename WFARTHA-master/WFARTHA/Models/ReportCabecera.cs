using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class ReportCabecera
    {
        public string NUM_DOC { get; set; }
        public string NUM_PRE { get; set; }
        public string SOCIEDAD_ID { get; set; }
        public string SOCIEDAD_TEXT { get; set; }
        public string PAYER_ID { get; set; }
        public string PAYER_NAME1 { get; set; }

        public string MONTO_DOC_MD { get; set; }

        public string CONDICIONES_ID { get; set; }
        public string CONDICIONES_TEXT { get; set; }
        public string CONCEPTO { get; set; }

        //public Nullable<System.DateTime> FECHAC { get; set; }
        //public string MONEDA_ID { get; set; }
        //public Nullable<System.DateTime> FECHACON { get; set; }
        //public string ID_PSPNR { get; set; }
        //public string ID_PSPNR_NOMBRE { get; set; }
        //public string FECHACF { get; set; }
        //public string FECHACONF { get; set; }
        //public string SOCIEDAD { get; set; }
        //public string MONTOLETRA { get; set; }
        //public string PROVEEDOR { get; set; }
        //public string PROYECTO { get; set; }
        //public string CONDICIONES { get; set; }
    }
}