namespace BackendAmatista.Models.DTO
{
    public class ProductDTO
    {
        
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int Stock { get; set; }
        public string Item { get; set; }
        public int ID_Category { get; set; }
        public bool Active { get; set; }
    }
}