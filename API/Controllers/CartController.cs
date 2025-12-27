using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CartController : BaseApiController
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string cartId)
    {
        var cart = await _cartService.GetCartAsync(cartId);
        return Ok(cart ?? new ShoppingCart { id = cartId });
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updatedCart = await _cartService.UpdateCartAsync(cart);
        if (updatedCart == null)
        {
            return BadRequest("Problem updating the cart");
        }
        return Ok(updatedCart);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCart(string cartId)
    {
        var result = await _cartService.DeleteCartAsync(cartId);
        if (!result)
        {
            return BadRequest("Problem deleting the cart");
        }

        return Ok();
    }
}
