using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Models
{
    //MGC 11-12-2018 Agregar Contabilizador 0
    public class CadenaAutorizadores
    {
        public int vc1 { set; get; }
        public int vc2 { set; get; }
        public List<CadenaAutorizadoresItem> cadenal { set; get; }
    }

    public class CadenaAutorizadoresItem
    {
        public string fase { set; get; }
        public string autorizador { set; get; }
    }
}