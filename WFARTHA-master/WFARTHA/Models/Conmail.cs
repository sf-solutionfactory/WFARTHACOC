using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using WFARTHA.Entities;

namespace WFARTHA.Models
{
    public partial class Conmail
    {

        [DataType(DataType.Text)]
        [DisplayName("SSL")]
        [UIHint("List")]
        public List<SelectListItem> sslLista { get; set; }

        public string ID { get; set; }
        public string HOST { get; set; }
        public int PORT { get; set; }
        public bool SSL { get; set; }
        public string MAIL { get; set; }
        public string PASS { get; set; }
        public bool ACTIVO { get; set; }
        


        public Conmail()
        {
            sslLista = new List<SelectListItem>();
            sslLista.Add(new SelectListItem()
            {
                Value = "0",
                Text = "NO",
            });
            sslLista.Add(new SelectListItem()
            {
                Value = "1",
                Text = "SI",
            });

        }
     
    }
}