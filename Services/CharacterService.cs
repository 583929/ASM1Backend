using GameApi.Data;
using GameApi.DTOs;
using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Services;

public class CharacterService
{
    private readonly AppDbContext _db;

    public CharacterService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Tạo nhân vật cho tài khoản đang đăng nhập. Level = 1, Hp = 100 theo mặc định của Model.</summary>
    public async Task<CharacterResponse> CreateAsync(int accountId, CreateCharacterRequest request)
    {
        var character = new Character
        {
            Name = request.Name,
            Class = request.Class,
            AccountId = accountId
        };

        _db.Characters.Add(character);
        await _db.SaveChangesAsync();

        return ToResponse(character);
    }

    /// <summary>Sửa nhân vật. Trả null nếu không có hoặc không thuộc về accountId.</summary>
    public async Task<CharacterResponse?> UpdateAsync(int accountId, int characterId, UpdateCharacterRequest request)
    {
        var character = await _db.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.AccountId == accountId);
        if (character == null) return null;

        character.Name = request.Name;
        character.Class = request.Class;
        character.Level = request.Level!.Value;
        character.Hp = request.Hp!.Value;

        await _db.SaveChangesAsync();

        return ToResponse(character);
    }

    private static CharacterResponse ToResponse(Character c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Class = c.Class,
        Level = c.Level,
        Hp = c.Hp,
        AccountId = c.AccountId
    };
}
