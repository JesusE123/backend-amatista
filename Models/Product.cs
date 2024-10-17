using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend_amatista.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public int IdCategory { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? Item { get; set; }

    public bool Active { get; set; }


    [JsonIgnore]
    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
