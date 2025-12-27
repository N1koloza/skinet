using System;

namespace Core.Entities;

public class ShoppingCart
{
    public required string id { get; set; } = string.Empty;
    public List<CartItem> items { get; set; } = [];

}

