using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocalWebModule.Controllers;

[ApiController]
[Route("api/auth")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRegisterService _registerService;
    private readonly IUpdateService _updateService;
    private readonly IAuthService _authService;

    public UserController(IUserService userService,  IRegisterService registerService, IUpdateService updateService, IAuthService authService)
    {
        _userService = userService;
        _registerService = registerService;
        _updateService = updateService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{login}/login")]
    public async Task<IActionResult> GetByLogin(string login)
    {
        try
        {
            var user = await _userService.GetByLoginAsync(login);

            if (user == null)
            {
                return NotFound($"Пользователь с логином {login} не найден.");
            }

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{email}/email")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        try
        {
            var user = await _userService.GetByEmailAsync(email);

            if (user == null)
            {
                return NotFound($"Пользователь с email {email} не найден.");
            }

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Create([FromBody] RegisterUserRequest request)
    {
       
        try
        {
            var user = await _registerService.RegisterAsync(request);
            UserResponse userResponse = new  UserResponse(user.Id, user.Email, user.Login, user.CreatedAt);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, userResponse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("auth")]
    public async Task<IActionResult> Auth([FromBody] AuthUserRequest request)
    {
        try
        {
            var user = await _authService.LoginAsync(request);
            if (user == null)
            {
                return BadRequest("Неверный логин или пароль");
            }
            var userResponse = new  UserResponse(user.Id, user.Email, user.Login, user.CreatedAt);
            return Ok(userResponse);
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            bool updated = await _updateService.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound("Пользователь не найден.");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            bool deleted = await _userService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Пользователь не найден.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
