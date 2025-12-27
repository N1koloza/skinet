using System;
using System.Collections.Concurrent;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class CartService : ICartService
{
    private static readonly ConcurrentDictionary<string, ShoppingCart> _carts
            = new ConcurrentDictionary<string, ShoppingCart>();

    public Task<ShoppingCart?> GetCartAsync(string cartId)
    {
        _carts.TryGetValue(cartId, out var cart);
        return Task.FromResult(cart);
    }

    public Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart)
    {
        _carts[cart.id] = cart;
        return Task.FromResult(cart);
    }

    public Task<bool> DeleteCartAsync(string cartId)
    {
        return Task.FromResult(_carts.TryRemove(cartId, out _));
    }
}
