using YPTO.BAL.Services.Interfaces;
using YPTO.BAL.DTOs;
using YPTO.BAL.Utitlities;
using System.Linq.Expressions;
using System.Text.Json;
using System.Net.Http.Json;

namespace YPTO.BAL.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
       
        public UserService(HttpClient httpClient) { 
            _httpClient = httpClient;       
        }
       
        public async Task<UserDto> GetUserById(int id)
        {
            try{
                if (id <= 0)
                    throw new ArgumentException(nameof(id));

                var users = await _httpClient.GetFromJsonAsync<List<UserDto>>($"users?id={id}");

                return users?.FirstOrDefault();
            }
             catch (HttpRequestException ex)
            {
                throw new Exception("Unable to connect to the User API.", ex);
            }
        }
        public async Task<IEnumerable<UserDto>> GetAllUsers(string? status)
        {
            try
            {
                string strStatus = string.IsNullOrEmpty(status) ? "" : $"?status={status.ToLower()}";
                return await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>($"users{strStatus}");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Unable to connect to the User API.", ex);
            }
        }   
     
    }
}