using LaptopShop.BLL.DTOs;

namespace LaptopShop.BLL.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(long id);
    Task<UserDto> CreateUserAsync(UserCreateDto userDto);
    Task<UserDto> LoginAsync(string email, string password);
    Task UpdateUserAsync(long id, UserCreateDto userDto);
    Task DeleteUserAsync(long id);
}
