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
     [FromQuery] string query,
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
                                 ProductItem = p.Item,
                                 ProductPrice = p.Price,
                                 Quantity = sd.Quantity // Renombrado a Quantity para mayor claridad
                             };

            // Filtrar las ventas según el query proporcionado
            if (!string.IsNullOrEmpty(query))
            {
                salesQuery = salesQuery.Where(s => s.Customer == query);
            }

            if (fromDate.HasValue)
            {
                salesQuery = salesQuery.Where(s => s.Date >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                salesQuery = salesQuery.Where(s => s.Date <= endDate);
            }

            // Agrupar las ventas por ReceiptNumber, Customer, PaymentMethod y Date
            var groupedSales = salesQuery
                .GroupBy(s => new { s.ReceiptNumber, s.Customer, s.PaymentMethod, s.Date })
                .Select(g => new
                {
                    ReceiptNumber = g.Key.ReceiptNumber,
                    Customer = g.Key.Customer,
                    PaymentMethod = g.Key.PaymentMethod,
                    Date = g.Key.Date,
                    Total = g.Sum(x => x.Total),
                    Products = g.GroupBy(x => new { x.ProductItem, x.ProductName, x.ProductPrice })
                                .Select(pg => new
                                {
                                    Name = pg.Key.ProductName,
                                    Code = pg.Key.ProductItem,
                                    Price = pg.Key.ProductPrice,
                                    Quantity = pg.Sum(x => x.Quantity) // Sumar la cantidad de cada producto
                                }).ToList()
                });

            var totalSales = await groupedSales.CountAsync();

            if (page.HasValue && limit.HasValue && page.Value > 0 && limit.Value > 0)
            {
                int skip = (page.Value - 1) * limit.Value;
                groupedSales = groupedSales.Skip(skip).Take(limit.Value);
            }

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

            
            var saleDetail = _mapper.Map<SaleDetail>(saleDetailDto);

           
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

        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomersForCurrentMonth()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            var result = await (from s in _dbamatistaContext.Sales
                                where s.Date.Year == currentYear && s.Date.Month == currentMonth
                                group s by s.Customer into customerGroup
                                orderby customerGroup.Sum(x => x.Total) descending
                                select new
                                {
                                    CustomerName = customerGroup.Key,
                                    TotalSpent = customerGroup.Sum(x => x.Total)
                                }).Take(3).ToListAsync(); 
            return Ok(result);
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenueForCurrentMonth()
        {
            var currentDate = DateTime.Now;
            var threeMonthsAgo = currentDate.AddMonths(-3);

            var totalRevenueLastThreeMonths = await (from s in _dbamatistaContext.Sales
                                                     where s.Date >= threeMonthsAgo && s.Date <= currentDate
                                                     group s by new { s.Date.Year, s.Date.Month } into monthlyGroup
                                                     orderby monthlyGroup.Key.Year descending, monthlyGroup.Key.Month descending
                                                     select new
                                                     {
                                                         Month = new DateTime(monthlyGroup.Key.Year, monthlyGroup.Key.Month, 1).ToString("MMMM yyyy"),
                                                         TotalRevenue = monthlyGroup.Sum(x => x.Total)
                                                     }).ToListAsync();

            return Ok(totalRevenueLastThreeMonths);
        }



    }
}
