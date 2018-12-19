
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    public partial class Appsetting
    {
        public int ID { get; set; }
        public string NOMBRE { get; set; }
        public string VALUE { get; set; }
        public Boolean ACTIVO { get; set; }
    }
}