using LaptopShop.BLL.DTOs;
using LaptopShop.DAL.Entities;
using LaptopShop.DAL.Repositories;

namespace LaptopShop.BLL.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync(includeProperties: "Role");
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            Address = u.Address,
            Phone = u.Phone,
            Avatar = u.Avatar,
            RoleId = u.RoleId,
            RoleName = u.Role?.Name
        });
    }

    public async Task<UserDto> GetUserByIdAsync(long id)
    {
        var users = await _userRepository.GetAllAsync(u => u.Id == id, includeProperties: "Role");
        var u = users.FirstOrDefault();
        if (u == null) return null;

        return new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            Address = u.Address,
            Phone = u.Phone,
            Avatar = u.Avatar,
            RoleId = u.RoleId,
            RoleName = u.Role?.Name
        };
    }

    public async Task<UserDto> CreateUserAsync(UserCreateDto dto)
    {
        var user = new User
        {
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName = dto.FullName,
            Address = dto.Address,
            Phone = dto.Phone,
            RoleId = dto.RoleId,
            Avatar = string.IsNullOrEmpty(dto.Avatar) ? "default.jpg" : dto.Avatar
        };

        await _userRepository.InsertAsync(user);
        await _userRepository.SaveAsync();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Address = user.Address,
            Phone = user.Phone,
            RoleId = user.RoleId
        };
    }

    public async Task UpdateUserAsync(long id, UserCreateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            user.Email = dto.Email;
            user.FullName = dto.FullName;
            user.Address = dto.Address;
            user.Phone = dto.Phone;
            user.RoleId = dto.RoleId;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
        }
    }

    public async Task<UserDto> LoginAsync(string email, string password)
    {
        var users = await _userRepository.GetAllAsync(u => u.Email == email, includeProperties: "Role");
        var user = users.FirstOrDefault();
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null; // Invalid credentials
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Address = user.Address,
            Phone = user.Phone,
            Avatar = user.Avatar,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name
        };
    }

    public async Task DeleteUserAsync(long id)
    {
        await _userRepository.DeleteAsync(id);
        await _userRepository.SaveAsync();
    }
}
