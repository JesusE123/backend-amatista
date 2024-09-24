namespace backend_amatista.Models
{
    public class Prueba
    {
        public class ReportModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public List<ReportItem> Items { get; set; }
        }

        public class ReportItem
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
        }
    }
}
