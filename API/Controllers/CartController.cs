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
    public async Task<ActionResult<ShoppingCart>> GetCartById(string Id)
    {
        var cart = await _cartService.GetCartAsync(Id);
        return Ok(cart ?? new ShoppingCart { id = Id });
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
    public async Task<ActionResult> DeleteCart(string Id)
    {
        var result = await _cartService.DeleteCartAsync(Id);
        if (!result)
        {
            return BadRequest("Problem deleting the cart");
        }

        return Ok();
    }
}
