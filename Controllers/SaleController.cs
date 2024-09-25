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
        public async Task<IActionResult> ListSales()
        {
            var sales = await (from s in _dbamatistaContext.Sales
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
                                   Quantity = sd.Quantity
                               }).ToListAsync();

            return Ok(sales);
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

           
            await _dbamatistaContext.Sales.AddAsync(sale);
            await _dbamatistaContext.SaveChangesAsync();

           
            return Ok(new { IdSale = sale.IdSale });

        }
    }
}
