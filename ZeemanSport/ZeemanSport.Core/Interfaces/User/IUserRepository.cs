using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.User
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUserNameAsync(string userName);
        Task<User> CreateAsync(User user, string password);
        Task<User> UpdateAsync(User user);
        Task<bool> ValidatePasswordAsync(int userId, string password);
    }
}
