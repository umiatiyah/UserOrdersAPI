using TechnicalTestDOT.Models;
using TechnicalTestDOT.Payloads.Request;
using TechnicalTestDOT.Payloads.Response;

namespace TechnicalTestDOT.Contracts
{
    public interface IUserRepository
    {
        Task<CommonResponse> GetUsers();
        Task<CommonResponse> CreateUser(UserRequest user);
        Task<CommonResponse> UpdateUser(int id, UserRequest user);
        Task<CommonResponse> GetUser(string username);
        Task<CommonResponse> DeleteUser(string username);
    }
}
