using AutoMapper;
using backend_amatista.Models;
using backend_amatista.Models.DTO;
using backendAmatista.Models;
using backendAmatista.Models.DTO;
using BackendAmatista.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace backendAmatista.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly DbamatistaContext _dbamatistaContext;
        private readonly IMapper _mapper;
        public SaleController(DbamatistaContext dbamatistaContext, IMapper mapper)
        {
            _dbamatistaContext = dbamatistaContext;
            _mapper = mapper;
        }


        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> ListSales(
     [FromQuery] int? limit,
     [FromQuery] int? page,
     [FromQuery] string? query,
     [FromQuery] DateTime? fromDate,
     [FromQuery] DateTime? toDate)
        {
            // Crear la consulta inicial para obtener todas las ventas
            var salesQuery = from s in _dbamatistaContext.Sales
                             join sd in _dbamatistaContext.SaleDetails on s.IdSale equals sd.IdSale
                             join p in _dbamatistaContext.Products on sd.IdProduct equals p.IdProduct
                             select new
                             {
                                 IdSale = s.IdSale,
                                 ReceiptNumber = s.InvoiceNumber,
                                 Total = sd.SubTotal,
                                 Customer = s.Customer,
                                 PaymentMethod = s.PaymentMethod,
                                 Remarks = s.Notes,
                                 Date = s.Date,
                                 ProductName = p.Name,
                             };

            // Filtrar las ventas según el query proporcionado
            if (!string.IsNullOrEmpty(query))
            {
                salesQuery = salesQuery.Where(s =>
                    s.Customer.Contains(query) ||
                    s.ProductName.Contains(query));
            }

            if (fromDate.HasValue)
            {
                salesQuery = salesQuery.Where(s => s.Date >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                // Aumentar el toDate un día y restar un segundo para incluir hasta el final del día
                var endDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                salesQuery = salesQuery.Where(s => s.Date <= endDate);
            }

            // Agrupar las ventas por ReceiptNumber y Customer
            var groupedSales = salesQuery
                .GroupBy(s => new { s.ReceiptNumber, s.Customer, s.PaymentMethod, s.Date })
                .Select(g => new
                {
                    ReceiptNumber = g.Key.ReceiptNumber,
                    Customer = g.Key.Customer,
                    PaymentMethod = g.Key.PaymentMethod,
                    Date = g.Key.Date,
                    Total = g.Sum(x => x.Total), // Sumar el total de productos para el mismo recibo
                    Products = g.Select(x => x.ProductName).Distinct().ToList() // Lista de productos comprados
                });

            // Obtener el total de ventas agrupadas antes de aplicar la paginación
            var totalSales = await groupedSales.CountAsync();

            // Aplicar la paginación
            if (page.HasValue && limit.HasValue && page.Value > 0 && limit.Value > 0)
            {
                int skip = (page.Value - 1) * limit.Value;
                groupedSales = groupedSales.Skip(skip).Take(limit.Value);
            }

            // Devolver el resultado con las ventas paginadas y el total de ventas agrupadas
            return Ok(new
            {
                TotalSales = totalSales,
                Data = groupedSales
            });
        }







        [HttpPost]
        [Route("CreateSaleDetail")]
        public async Task<IActionResult> CreateSaleDetail([FromBody] SaleDetailDTO saleDetailDto)
        {
            if (saleDetailDto == null)
            {
                return BadRequest("SaleDetailDTO is null");
            }

            // Mapea el DTO a la entidad
            var saleDetail = _mapper.Map<SaleDetail>(saleDetailDto);

            // Agrega la entidad a la base de datos
            await _dbamatistaContext.SaleDetails.AddAsync(saleDetail);
            await _dbamatistaContext.SaveChangesAsync();

            // Retorna el resultado
            return Ok(saleDetail);
        }



        [HttpPost]
        [Route("CreateSale")]
        public async Task<IActionResult> CreateSale([FromBody] SaleDTO saleDto)
        {
            if (saleDto == null)
            {
                return BadRequest("SaleDTO is null");
            }

           
            var sale = _mapper.Map<Sale>(saleDto);

            
            var nextInvoiceNumber = await _dbamatistaContext.Sales
                .MaxAsync(s => (int?)s.InvoiceNumber) ?? 0; 

            sale.InvoiceNumber = nextInvoiceNumber + 1; 

           
            await _dbamatistaContext.Sales.AddAsync(sale);
            await _dbamatistaContext.SaveChangesAsync();

            return Ok(new { IdSale = sale.IdSale });
        }
    }
}
