//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WFARTHA.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class DET_APROB0
    {
        public int VERSION { get; set; }
        public string ID_SOCIEDAD { get; set; }
        public string ID_USUARIO { get; set; }
        public int STEP_FASE { get; set; }
    
        public virtual SOCIEDAD SOCIEDAD { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
