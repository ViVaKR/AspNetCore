using System;

namespace ApiWriterOrKR.Entities;

public class Offer
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
