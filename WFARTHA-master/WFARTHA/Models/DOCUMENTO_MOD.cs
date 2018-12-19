using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFARTHA.Entities;

namespace WFARTHA.Models
{
    public class DOCUMENTO_MOD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DOCUMENTO_MOD()
        {
            this.DOCUMENTOPs = new HashSet<DOCUMENTOP>();
        }

        public decimal NUM_DOC { get; set; }
        public decimal NUM_PRE { get; set; }
        public string TSOL_ID { get; set; }
        public string TALL_ID { get; set; }
        public string SOCIEDAD_ID { get; set; }
        public Nullable<decimal> CANTIDAD_EV { get; set; }
        public string USUARIOC_ID { get; set; }
        public string USUARIOD_ID { get; set; }
        public Nullable<System.DateTime> FECHAD { get; set; }
        public Nullable<System.DateTime> FECHAC { get; set; }
        public Nullable<System.TimeSpan> HORAC { get; set; }
        public Nullable<System.DateTime> FECHAC_PLAN { get; set; }
        public Nullable<System.DateTime> FECHAC_USER { get; set; }
        public Nullable<System.TimeSpan> HORAC_USER { get; set; }
        public string ESTATUS { get; set; }
        public string ESTATUS_C { get; set; }
        public string ESTATUS_SAP { get; set; }
        public string ESTATUS_WF { get; set; }
        public Nullable<decimal> DOCUMENTO_REF { get; set; }
        public string CONCEPTO { get; set; }
        public string NOTAS { get; set; }
        public Nullable<decimal> MONTO_DOC_MD { get; set; }
        public Nullable<decimal> MONTO_FIJO_MD { get; set; }
        public Nullable<decimal> MONTO_BASE_GS_PCT_MD { get; set; }
        public Nullable<decimal> MONTO_BASE_NS_PCT_MD { get; set; }
        public Nullable<decimal> MONTO_DOC_ML { get; set; }
        public Nullable<decimal> MONTO_FIJO_ML { get; set; }
        public Nullable<decimal> MONTO_BASE_GS_PCT_ML { get; set; }
        public Nullable<decimal> MONTO_BASE_NS_PCT_ML { get; set; }
        public Nullable<decimal> MONTO_DOC_ML2 { get; set; }
        public Nullable<decimal> MONTO_FIJO_ML2 { get; set; }
        public Nullable<decimal> MONTO_BASE_GS_PCT_ML2 { get; set; }
        public Nullable<decimal> MONTO_BASE_NS_PCT_ML2 { get; set; }
        public Nullable<decimal> PORC_ADICIONAL { get; set; }
        public string IMPUESTO { get; set; }
        public string ESTATUS_EXT { get; set; }
        public string PAYER_ID { get; set; }
        public string MONEDA_ID { get; set; }
        public string MONEDAL_ID { get; set; }
        public string MONEDAL2_ID { get; set; }
        public Nullable<decimal> TIPO_CAMBIO { get; set; }
        public Nullable<decimal> TIPO_CAMBIOL { get; set; }
        public Nullable<decimal> TIPO_CAMBIOL2 { get; set; }
        public string NO_FACTURA { get; set; }
        public Nullable<System.DateTime> FECHAD_SOPORTE { get; set; }
        public string METODO_PAGO { get; set; }
        public string NO_PROVEEDOR { get; set; }
        public Nullable<int> PASO_ACTUAL { get; set; }
        public string AGENTE_ACTUAL { get; set; }
        public Nullable<System.DateTime> FECHA_PASO_ACTUAL { get; set; }
        public Nullable<int> PUESTO_ID { get; set; }
        public string GALL_ID { get; set; }
        public Nullable<int> CONCEPTO_ID { get; set; }
        public string DOCUMENTO_SAP { get; set; }
        public Nullable<System.DateTime> FECHACON { get; set; }
        public Nullable<System.DateTime> FECHA_BASE { get; set; }
        public string REFERENCIA { get; set; }
        public string CONDICIONES { get; set; }
        public string TEXTO_POS { get; set; }
        public string ASIGNACION_POS { get; set; }
        public string CLAVE_CTA { get; set; }
        public List<DOCUMENTOP_MOD> DOCUMENTOP { get; set; } //Agregado
        public List<DOCUMENTOP_MODSTR> DOCUMENTOPSTR { get; set; } //Agregado
        public List<DOCUMENTOR_MOD> DOCUMENTOR { get; set; } //Agregado
        public List<DOCUMENTORP_MOD> DOCUMENTORP { get; set; } //Agregado LEJ14.09.2018
        public List<DOCUMENTOA> DOCUMENTOAL { get; set; }//Agregado LEJ14.09.2018
        public List<WFARTHA.Controllers.Anexo> Anexo { get; set; }
        public List<DOCUMENTOA_TAB> DOCUMENTOA_TAB { get; set; }//Agregado LEJGG02.11.2018
        public string DESC_CONDICION { get; set; }   //aGERGADO frt 06112018
        public string ESTATUS_PRE { get; set; } //MGC 17-12-2018 Reprocesar Archivo preliminar

        public virtual SOCIEDAD SOCIEDAD { get; set; }
        public virtual TSOL TSOL { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTOP> DOCUMENTOPs { get; set; }
    }
}