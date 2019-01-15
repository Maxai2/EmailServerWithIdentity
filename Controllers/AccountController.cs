using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private IAccountService accountService;

    public AccountController(IAccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpPost("Registration")]
    public async Task<IActionResult> Registration([FromBody]RegistrationRequest model)
    {
        try
        {
            var resp = await accountService.Registration(model.Email, model.Login, model.Password);

            return new JsonResult(resp);
        }
        catch (CustomException ex)
        {
            return BadRequest(ex.ListErrors);
        }
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody]LoginRequest model)
    {
        var resp = accountService.LogIn(model.Login, model.Password);

        if (resp == null)
            return BadRequest(resp.Exception);
        else
            return new JsonResult(resp);
    }

    [HttpPost("Token")] // api/account/token
    public IActionResult UpdateToken([FromBody]string refreshToken)
    {
        var resp = accountService.UpdateToken(refreshToken);

        if (resp == null)
            return BadRequest(resp.Exception);
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

        var acc = accountService.GetAccount(id);

        if (acc == null)
            return BadRequest(acc.Exception);

        return new JsonResult(acc);
    }

    [Authorize]
    [HttpGet("LogOut")] // api/account/logout
    public IActionResult LogOut()
    {
        string id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        string refreshToken = accountService.GetRefreshToken(Int32.Parse(id));

        if (id == null)
            return Ok();

        accountService.LogOut(Int32.Parse(id), refreshToken);
        return Ok();
    }
}