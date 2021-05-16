
namespace DataEntityService.Entities
{
    public class PurchaseOrderDetails
    {
        public int POLineItemId { get; set; }

        public int PurchaseOrderId { get; set; }

        public int ItemId { get; set; }

        public bool Void { get; set; }

        public decimal Price { get; set; }
    }
}
