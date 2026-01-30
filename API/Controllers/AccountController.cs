using System;
using API.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest("Failed to create user" + string.Join(",", result.Errors.Select(e => e.Description)));
        }

        return Ok();
    }

    // [HttpPost("login")]
    // public async Task<ActionResult> Login()
    // {
    //     // Login logic will go here

    //     return Ok();
    // }
}
