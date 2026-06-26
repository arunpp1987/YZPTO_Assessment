using YPTO.BAL.DTOs;

namespace YPTO.BAL.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserById(int id);
        Task<IEnumerable<UserDto>> GetAllUsers(string? status);
    }
}
