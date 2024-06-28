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
                if (UserExists(user.Username, user.Email))
                {
                    response.StatusCode = 400;
                    response.Message = $"username or email was taken";
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
                var userModel = new UserModel
                {
                    Id = id,
                    Fullname = user.Fullname,
                    Username = user.Username,
                    Email = user.Email,
                    Address = user.Address,
                    UpdatedBy = user.UpdatedBy,
                    UpdatedOn = DateTime.Now
                };
                _context.Users.Attach(userModel);
                _context.Entry(userModel).State = EntityState.Modified;
                _context.Entry(userModel).Property(x => x.CreatedBy).IsModified = false;
                _context.Entry(userModel).Property(x => x.CreatedOn).IsModified = false;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Username, user.Email))
                    {
                        response.StatusCode = 400;
                        response.Message = $"user not found";
                        _logger.LogWarning(response.Message);
                        return response;
                    }
                    else
                    {
                        throw;
                    }
                }
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

        private bool UserExists(string username, string email)
        {
            return _context.Users.Any(e => e.Username == username || e.Email == email);
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
