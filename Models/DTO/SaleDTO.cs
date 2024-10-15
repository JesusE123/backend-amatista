using BackendAmatista.Models.DTO;

namespace backendAmatista.Models.DTO
{
    public class SaleDTO
    {
        public int InvoiceNumber { get; set; }
        public decimal Total { get; set; }
        public string Customer { get; set; } = string.Empty;

        public string Cuit { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; }
       
    }
}
