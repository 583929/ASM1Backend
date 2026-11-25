using System.Security.Claims;
using GameApi.DTOs;
using GameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers;

[ApiController]
[Authorize]
[Route("api/characters")]
public class CharactersController : ControllerBase
{
    private readonly CharacterService _characters;

    public CharactersController(CharacterService characters)
    {
        _characters = characters;
    }

    // Id tài khoản luôn lấy từ token, không lấy từ body hay URL
    private int CurrentAccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateCharacterRequest request)
    {
        var result = await _characters.CreateAsync(CurrentAccountId, request);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCharacterRequest request)
    {
        var result = await _characters.UpdateAsync(CurrentAccountId, id, request);
        if (result == null)
            return NotFound(new { message = "Không tìm thấy nhân vật của bạn" });

        return Ok(result);
    }
}
