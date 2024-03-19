using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;

public partial class Coupon
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public DateTime Expiration { get; set; }
}
