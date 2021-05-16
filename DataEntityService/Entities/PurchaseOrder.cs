
namespace DataEntityService.Entities
{
    using System;
    using System.Collections.Generic;

    public class PurchaseOrder
    {
        public PurchaseOrder()
        {
            PurchaseOrderDetails = new List<PurchaseOrderDetails>();
        }

        public int PurchaseOrderId { get; set; }

        public int UserId { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public int SourceId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool Void { get; set; }

        public IList<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
    }
}
