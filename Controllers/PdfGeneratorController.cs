using Microsoft.AspNetCore.Mvc;
using backend_amatista.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAmatista.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly DbamatistaContext _dbamatistaContext;
        public ReportController(DbamatistaContext dbamatistaContext)
        {
            _dbamatistaContext = dbamatistaContext;
        }
        [HttpGet]
        public async Task<IActionResult> createReport(int id_sale)
        {
            
            var report = await (from s in _dbamatistaContext.Sales
                                join sd in _dbamatistaContext.SaleDetails on s.IdSale equals sd.IdSale
                                join p in _dbamatistaContext.Products on sd.IdProduct equals p.IdProduct
                                where s.IdSale == id_sale 
                                select new
                                {
                                    ID_Sale = s.IdSale,
                                    ProductName = p.Name,
                                    Quantity = sd.Quantity,
                                    SubTotal = sd.SubTotal,
                                    price = p.Price,
                                    Customer = s.Customer,
                                    InvoiceNumber = s.InvoiceNumber,
                                    Total = s.Total,
                                    Notes = s.Notes,
                                    Date = s.Date,
                                    paymentMethod = s.PaymentMethod,
                                    Codigo = p.Item
                                }).ToListAsync();

            
            if (report == null || !report.Any())
            {
                return NotFound($"No se encontraron ventas con el ID_SALE: {id_sale}");
            }

            return Ok(report);
        }



    }
}
