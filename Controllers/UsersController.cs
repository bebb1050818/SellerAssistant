using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiProject.Models;
using MyApiProject.Services;

namespace MyApiProject.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SysUser>>> GetUsers()
    {
        return Ok(await _userService.GetAllUsersAsync());
    }
}
