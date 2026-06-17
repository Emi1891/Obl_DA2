using System.Security.Claims;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.API.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    public IActionResult SignUp([FromBody] UserDto newUser)
    {
        _userService.CreateUser(newUser);
        return Created("User created correctly.", null);
    }

    [Authorize(Policy = nameof(Permission.CreateUser))]
    [HttpPost("admin")]
    public IActionResult CreateUserWithRole([FromBody] CreateUserDto newUser)
    {
        _userService.CreateUserWithRole(newUser);
        return Created("User created correctly.", null);
    }

    [Authorize(Policy = nameof(Permission.UpdateUser))]
    [HttpPut]
    public IActionResult UpdateUser([FromBody] UserDto user)
    {
        _userService.EnsureNotSelf(user.email, User.FindFirst(ClaimTypes.Email)!.Value);
        _userService.UpdateUser(user);
        return Ok("User updated correctly.");
    }

    [Authorize(Policy = nameof(Permission.GetUsers))]
    [HttpGet]
    public IActionResult GetUsers([FromQuery] UserFiltersDto filters)
    {
        var users = _userService.GetUsers(filters);
        return Ok(users);
    }

    [Authorize(Policy = nameof(Permission.DeleteUser))]
    [HttpDelete("{email}")]
    public IActionResult DeleteUser(string email)
    {
        _userService.EnsureNotSelf(email, User.FindFirst(ClaimTypes.Email)!.Value);
        _userService.DeleteUser(email);
        return Ok("User deleted correctly.");
    }
}
