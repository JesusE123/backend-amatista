using System;
using System.Collections.Generic;

namespace backend_amatista.Models;

public partial class Sale
{
    public int IdSale { get; set; }

    public int InvoiceNumber { get; set; }

    public decimal Total { get; set; }

    public string Customer { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
