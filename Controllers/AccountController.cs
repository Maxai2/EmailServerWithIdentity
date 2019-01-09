using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private IAccountService accountService;

    public AccountController(IAccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpPost("Login")] // api/account/login
    public IActionResult Login([FromBody]LoginRequest model)
    {
        LoginResponse resp = accountService.LogIn(model.Login, model.Password);

        if (resp == null)
            return BadRequest();
        else
            return new JsonResult(resp);
    }

    [HttpPost("Token")] // api/account/token
    public IActionResult UpdateToken([FromBody]string refreshToken)
    {
        LoginResponse resp = accountService.UpdateToken(refreshToken);

        if (resp == null)
            return StatusCode(401);
        else
            return new JsonResult(resp);
    }

    [Authorize]
    [HttpGet("")] // api/account
    public IActionResult GetInfo()
    {
        string id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        if (id == null)
            return NotFound();

        Account acc = accountService.GetAccount(Int32.Parse(id));

        if (acc == null)
            return NotFound();

        return new JsonResult(acc);
    }

    [Authorize]
    [HttpGet("LogOut")] // api/account/logout
    public IActionResult LogOut()
    {
        string id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        if (id == null)
            return Ok();

        accountService.LogOut(Int32.Parse(id));
        return Ok();
    }
}