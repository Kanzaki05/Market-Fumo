//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Market.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_categoria
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_categoria()
        {
            this.tbl_producto = new HashSet<tbl_producto>();
        }
    
        public int cat_id { get; set; }
        public string cat_nombre { get; set; }
        public string cat_imagen { get; set; }
        public Nullable<int> cat_fk_ad { get; set; }
        public int cat_estado { get; set; }
    
        public virtual tbl_admin tbl_admin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_producto> tbl_producto { get; set; }
    }
}