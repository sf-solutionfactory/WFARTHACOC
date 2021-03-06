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
    
    public partial class SOCIEDAD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SOCIEDAD()
        {
            this.DET_APROB = new HashSet<DET_APROB>();
            this.DET_APROB0 = new HashSet<DET_APROB0>();
            this.DET_PROVEEDOR = new HashSet<DET_PROVEEDOR>();
            this.DET_SOCIEDAD = new HashSet<DET_SOCIEDAD>();
            this.DET_TIPOPRESUPUESTO = new HashSet<DET_TIPOPRESUPUESTO>();
            this.DOCUMENTOes = new HashSet<DOCUMENTO>();
            this.RETENCION_PROV = new HashSet<RETENCION_PROV>();
            this.PROYECTOes = new HashSet<PROYECTO>();
        }
    
        public string BUKRS { get; set; }
        public string BUTXT { get; set; }
        public string ORT01 { get; set; }
        public string SUBREGIO { get; set; }
        public string WAERS { get; set; }
        public string SPRAS { get; set; }
        public string NAME1 { get; set; }
        public string KTOPL { get; set; }
        public bool ACTIVO { get; set; }
        public string REGION { get; set; }
        public string STCD1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_APROB> DET_APROB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_APROB0> DET_APROB0 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_PROVEEDOR> DET_PROVEEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_SOCIEDAD> DET_SOCIEDAD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_TIPOPRESUPUESTO> DET_TIPOPRESUPUESTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTO> DOCUMENTOes { get; set; }
        public virtual MONEDA MONEDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RETENCION_PROV> RETENCION_PROV { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PROYECTO> PROYECTOes { get; set; }
    }
}
