using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public class ReportFirmasResult
    {
        public Nullable<int> fasen { get; set; }
        public Nullable<decimal> num_doc { get; set; }
        public string faseletrero { get; set; }
        public Nullable<int> loop { get; set; }
        public string usuariocadena { get; set; }
        public string usuarioa_id { get; set; }
        public string usuariod_id { get; set; }
        public string estatus { get; set; }
        public Nullable<System.DateTime> fechac { get; set; }
        public Nullable<System.DateTime> fecham { get; set; }
    }
}