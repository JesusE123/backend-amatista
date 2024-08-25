using backend_amatista.Models;
using backendAmatista.Models;
using backendAmatista.Models.DTO;
using BackendAmatista.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace backendAmatista.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly DbamatistaContext _dbamatistaContext;
        public SaleController(DbamatistaContext dbamatistaContext)
        {
            _dbamatistaContext = dbamatistaContext;
        }


        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> ListSales()
        {
            var sales = await _dbamatistaContext.Sales
     .Select(s => new
     {
         IdSale = s.IdSale,
         ReceiptNumber = s.InvoiceNumber,
         Total = s.Total,
         Customer = s.Customer,
         PaymentMethod = s.PaymentMethod,
         Remarks = s.Notes,
         Date = s.Date
     })
     .ToListAsync();

            return Ok(sales);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> createSale([FromBody] SaleDTO detail)
        {
            if (detail == null)
            {
                BadRequest("invalid data");
            }

            var newSale = new Sale
            {
                Customer = detail.Customer,
                InvoiceNumber = detail.InvoiceNumber,
                Total = detail.Total,
                PaymentMethod = detail.PaymentMethod,
                Notes = detail.Notes,
                

            };

            _dbamatistaContext.Sales.Add(newSale);
            await _dbamatistaContext.SaveChangesAsync();

            
            
            return Ok(newSale);

        }


    }
}
