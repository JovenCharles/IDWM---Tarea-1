using courses_dotnet_api.Src.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace courses_dotnet_api.Src.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _dataContext;

    public UserRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await _dataContext.Users.AnyAsync(user => user.Email == email);
    }

    public async Task<bool> UserExistsByRutAsync(string rut)
    {
        return await _dataContext.Users.AnyAsync(user => user.Rut == rut);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return 0 < await _dataContext.SaveChangesAsync();
    }

    public async Task<bool> VerifyPasswordAsync(string email, string password)
    {
        var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false; // User not found
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return false; // Password hash does not match
            }
        }

        return true; // Password is correct
    }


}
