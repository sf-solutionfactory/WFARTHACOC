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
    
    public partial class DOCUMENTOP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DOCUMENTOP()
        {
            this.DOCUMENTOAs = new HashSet<DOCUMENTOA>();
            this.DOCUMENTORPs = new HashSet<DOCUMENTORP>();
        }
    
        public decimal NUM_DOC { get; set; }
        public decimal POS { get; set; }
        public string ACCION { get; set; }
        public string FACTURA { get; set; }
        public string TCONCEPTO { get; set; }
        public string GRUPO { get; set; }
        public string CUENTA { get; set; }
        public string TIPOIMP { get; set; }
        public string IMPUTACION { get; set; }
        public string CCOSTO { get; set; }
        public decimal MONTO { get; set; }
        public string MWSKZ { get; set; }
        public decimal IVA { get; set; }
        public decimal TOTAL { get; set; }
        public string TEXTO { get; set; }
    
        public virtual DOCUMENTO DOCUMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTOA> DOCUMENTOAs { get; set; }
        public virtual DOCUMENTOCOC DOCUMENTOCOC { get; set; }
        public virtual IMPUESTO IMPUESTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTORP> DOCUMENTORPs { get; set; }
    }
}
