using System;
using System.Collections.Generic;

namespace backend_amatista.Models;

public partial class SaleDetail
{
    public int IdSaleDetail { get; set; }

    public int IdSale { get; set; }

    public int IdProduct { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual Sale IdSaleNavigation { get; set; } = null!;
}
