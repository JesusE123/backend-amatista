namespace backend_amatista.Models.DTO
{
    public class SaleDetailDTO
    {
        public int IdSale { get; set; }

        public int IdProduct { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}
