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
    
    public partial class PAGINA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PAGINA()
        {
            this.CAMPOS = new HashSet<CAMPOS>();
            this.PAGINATs = new HashSet<PAGINAT>();
            this.PERMISO_PAGINA = new HashSet<PERMISO_PAGINA>();
        }
    
        public int ID { get; set; }
        public string URL { get; set; }
        public string TITULO { get; set; }
        public Nullable<int> CARPETA_ID { get; set; }
        public string ICON { get; set; }
        public bool MOSTRAR { get; set; }
        public bool ACTIVO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAMPOS> CAMPOS { get; set; }
        public virtual CARPETA CARPETA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGINAT> PAGINATs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERMISO_PAGINA> PERMISO_PAGINA { get; set; }
    }
}
