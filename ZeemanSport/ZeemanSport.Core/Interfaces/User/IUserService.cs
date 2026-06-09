using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.User;

namespace ZeemanSport.Core.User
{
    public interface IUserService
    {
        Task<UserResponse> LoginAsync(LoginUserRequest request);
        Task<UserResponse> RegisterAsync(RegisterUserRequest request);
        Task<UserResponse> GetAsync(int id);
        Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
    }
}
