using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;
using MyApiProject.Models;

namespace MyApiProject.Services;

public interface IUserService
{
    Task<IEnumerable<SysUser>> GetAllUsersAsync();
}

public class UserService : IUserService
{
    private readonly PostgresContext _context;

    public UserService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SysUser>> GetAllUsersAsync()
    {
        // 這裡可以加入額外的商業邏輯，例如篩選、格式轉換等
        return await _context.SysUsers.ToListAsync();
    }
}
