namespace backend_amatista.Models.DTO
{
    public class SalesRequest
    {
        public int? Limit { get; set; }
        public int? Page { get; set; }
        public string Query { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
