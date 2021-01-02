namespace PharmaSitModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [Key]
        public int OrdId { get; set; }

        public int? ClntId { get; set; }

        public int? ProdId { get; set; }

        public int? PhmId { get; set; }

        public virtual Client Client { get; set; }

        public virtual Pharmacy Pharmacy { get; set; }

        public virtual Product Product { get; set; }
    }
}
