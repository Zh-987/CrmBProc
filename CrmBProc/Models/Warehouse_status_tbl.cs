//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrmBProc.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Warehouse_status_tbl
    {
        public Warehouse_status_tbl()
        {
            this.Warehouse_sup_tbl = new HashSet<Warehouse_sup_tbl>();
        }
    
        public int Warehouse_status_id { get; set; }
        public string Warehouse_status_name { get; set; }
    
        public virtual ICollection<Warehouse_sup_tbl> Warehouse_sup_tbl { get; set; }
    }
}