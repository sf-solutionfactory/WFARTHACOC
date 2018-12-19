using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    public partial class Usuario
    {

        [DataType(DataType.Text)]
        [DisplayName("Users")]
        [UIHint("List")]
        public List<SelectListItem> UsersLista { get; set; }
        public List<SelectListItem> SociedadLista { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            //this.CARTAs = new HashSet<CARTA>();
            //this.DOCUMENTOes = new HashSet<DOCUMENTO>();
            //this.FLUJOes = new HashSet<FLUJO>();
            //this.FLUJOes1 = new HashSet<FLUJO>();
            //this.MIEMBROS = new HashSet<MIEMBRO>();
            //this.PRESUPSAPHs = new HashSet<PRESUPSAPH>();
            //this.PRESUPUESTOHs = new HashSet<PRESUPUESTOH>();

            WFARTHAEntities data = new WFARTHAEntities();

            SociedadLista = new List<SelectListItem>();
            UsersLista = new List<SelectListItem>();
            var qSociedad = from s in data.SOCIEDADs where (s.ACTIVO == true) select s;
            var SociedadData = qSociedad.ToList();
            var qUsers = from s in data.PUESTOTs where (s.SPRAS_ID == "ES") select s;
            var UsersData = qUsers.ToList();

            foreach (var Data in SociedadData)
            {
                string sociedad = string.Format("{0} - {1}",
                Data.BUKRS,
                Data.BUTXT);

                SociedadLista.Add(new SelectListItem()
                {
                    Value = Data.BUKRS.ToString(),
                    Text = sociedad,
                });
            }

            foreach (var Data in UsersData)
            {
                string users = string.Format("{0} - {1}",
                Data.PUESTO_ID,
                Data.TXT50);

                UsersLista.Add(new SelectListItem()
                {
                    Value = Data.PUESTO_ID.ToString(),
                    Text = users,
                });
            }
        }

        public string ID { get; set; }
        public string PASS { get; set; }
        public string NOMBRE { get; set; }
        public string APELLIDO_P { get; set; }
        public string APELLIDO_M { get; set; }
        public string EMAIL { get; set; }
        public string SPRAS_ID { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
        public Nullable<int> PUESTO_ID { get; set; }
        public string MANAGER { get; set; }
        public string BACKUP_ID { get; set; }
        public string BUNIT { get; set; }
        public int ROL { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CARTA> CARTAs { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<DOCUMENTO> DOCUMENTOes { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<FLUJO> FLUJOes { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<FLUJO> FLUJOes1 { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<MIEMBRO> MIEMBROS { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<PRESUPSAPH> PRESUPSAPHs { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<PRESUPUESTOH> PRESUPUESTOHs { get; set; }
        //public virtual PUESTO PUESTO { get; set; }
        //public virtual SPRA SPRA { get; set; }
    }
}