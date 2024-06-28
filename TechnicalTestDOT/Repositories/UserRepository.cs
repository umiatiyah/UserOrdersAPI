using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using TechnicalTestDOT.Contracts;
using TechnicalTestDOT.Data;
using TechnicalTestDOT.Models;
using TechnicalTestDOT.Payloads.Request;
using TechnicalTestDOT.Payloads.Response;

namespace TechnicalTestDOT.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<UserRepository> _logger;
        private readonly IMemoryCache _cache;
        public UserRepository(DatabaseContext context, ILogger<UserRepository> logger, IMemoryCache cache) 
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }
        public async Task<CommonResponse> GetUsers()
        {
            CommonResponse response = new();
            try
            {
                if (!_cache.TryGetValue("users", out List<UserModel>? users))
                {
                    users = await _context.Users.Include(u => u.Orders).ToListAsync();
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(1));

                    _cache.Set("users", users, cacheEntryOptions);
                }
                var dataUser = new List<UserModel>();
                if (users != null)
                    dataUser = users;

                if (dataUser.Count == 0)
                {
                    response.StatusCode = 204;
                    response.Message = $"user empty";
                    _logger.LogWarning(response.Message);
                    return response;
                }
                response.Data = dataUser;
                response.StatusCode = 200;
                response.Message = "Successfully get users";
                _logger.LogInformation(response.Message);
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.StatusCode = 500;
                response.Message = "An error occurred while getting users.";
                _logger.LogError(ex, response.Message);
            }
            return response;
        }

        public async Task<CommonResponse> CreateUser(UserRequest user)
        {
            CommonResponse response = new();
            try
            {
                if (UserNameExists(user.Username))
                {
                    response.StatusCode = 400;
                    response.Message = $"username `{user.Username}` is already taken";
                    _logger.LogWarning(response.Message);
                    return response;
                }
                if (EmailExists(user.Email))
                {
                    response.StatusCode = 400;
                    response.Message = $"email `{user.Email}` is already taken";
                    _logger.LogWarning(response.Message);
                    return response;
                }
                var userModel = new UserModel
                {
                    Fullname = user.Fullname,
                    Username = user.Username,
                    Email = user.Email,
                    Address = user.Address,
                    CreatedBy = user.CreatedBy,
                    CreatedOn = DateTime.Now
                };
                _context.Users.Add(userModel);
                await _context.SaveChangesAsync();
                response.Data = GetUser(user.Username).Result.Data;
                response.StatusCode = 200;
                response.Message = "Successfully created user";
                _logger.LogInformation(response.Message);
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.StatusCode = 500;
                response.Message = $"An error occurred while create user with username `{user.Username}`";
                _logger.LogError(ex, response.Message);
            }
            return response;
        }

        public async Task<CommonResponse> UpdateUser(int id, UserRequest user)
        {
            CommonResponse response = new();
            try
            {
                var userExists = await _context.Users.FindAsync(id);
                if (userExists == null)
                {
                    response.StatusCode = 404;
                    response.Message = $"User with ID `{id}` not found";
                    _logger.LogWarning(response.Message);
                    return response;
                }

                if (!userExists.Username.Equals(user.Username) && UserNameExists(user.Username))
                {
                    response.StatusCode = 400;
                    response.Message = $"username `{user.Username}` is already taken";
                    _logger.LogWarning(response.Message);
                    return response;
                }
                if (!userExists.Email.Equals(user.Email) && EmailExists(user.Email))
                {
                    response.StatusCode = 400;
                    response.Message = $"email `{user.Email}` is already taken";
                    _logger.LogWarning(response.Message);
                    return response;
                }
                userExists.Fullname = user.Fullname;
                userExists.Username = user.Username;
                userExists.Email = user.Email;
                userExists.Address = user.Address;
                userExists.UpdatedBy = user.UpdatedBy;
                userExists.UpdatedOn = DateTime.Now;

                await _context.SaveChangesAsync();

                response.Data = GetUser(user.Username).Result.Data;
                response.StatusCode = 200;
                response.Message = "Successfully updated user";
                _logger.LogInformation(response.Message);
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.StatusCode = 500;
                response.Message = $"An error occurred while update user with username `{user.Username}`";
                _logger.LogError(ex, response.Message);
            }
            return response;
        }

        private bool UserNameExists(string username)
        {
            return _context.Users.Any(e => e.Username == username);
        }
        private bool EmailExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }

        public async Task<CommonResponse> GetUser(string username)
        {
            CommonResponse response = new();
            try
            {
                var user = await _context.Users.Include(u => u.Orders)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    response.StatusCode = 404;
                    response.Message = $"the user with username `{username}` not found";
                    _logger.LogWarning(response.Message);
                    return response;
                }

                response.Data = user;
                response.StatusCode = 200;
                response.Message = "Successfully get user";
                _logger.LogInformation(response.Message);
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.StatusCode = 500;
                response.Message = $"An error occurred while getting the user with username `{username}`.";
                _logger.LogError(ex, response.Message);
            }
            return response;
        }

        public async Task<CommonResponse> DeleteUser(string username)
        {
            CommonResponse response = new();
            try
            {
                var user = GetUser(username).Result.Data;
                if (user == null)
                {
                    response.StatusCode = 404;
                    response.Message = $"the user with username `{username}` not found";
                    _logger.LogWarning(response.Message);
                    return response;
                }

                _context.Users.Remove((UserModel)user);
                await _context.SaveChangesAsync();

                response.Data = user;
                response.StatusCode = 200;
                response.Message = "Successfully deleted user";
                _logger.LogInformation(response.Message);
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.StatusCode = 500;
                response.Message = $"An error occurred while delete the user with username `{username}`.";
                _logger.LogError(ex, response.Message);
            }
            return response;
        }
    }
}
