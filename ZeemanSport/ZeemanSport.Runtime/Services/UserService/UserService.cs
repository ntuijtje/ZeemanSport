using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.User;
using ZeemanSport.Core.User;

namespace ZeemanSport.Runtime.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse?> LoginAsync(LoginUserRequest request)
        {
            User user = await _userRepository.GetByUserNameAsync(request.UserName);

            if (user == null)
                return null;

            bool passwordIsValid = await _userRepository.ValidatePasswordAsync(user.Id, request.Password);

            if (!passwordIsValid)
                return null;

            return MapToResponse(user);
        }

        public async Task<UserResponse> RegisterAsync(RegisterUserRequest request)
        {
            User user = new User
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserRole = request.UserRole
            };

            User savedUser = await _userRepository.CreateAsync(user, request.Password ?? string.Empty);

            return MapToResponse(savedUser);
        }

        public async Task<UserResponse?> GetAsync(int id)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return null;

            return MapToResponse(user);
        }

        public async Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request)
        {
            User? existingUser = await _userRepository.GetByIdAsync(id);

            if (existingUser == null)
                return null;

            existingUser.UserName = request.UserName;
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.UserRole = request.UserRole;

            User updatedUser = await _userRepository.UpdateAsync(existingUser);

            return MapToResponse(updatedUser);
        }

        private static UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserRole = user.UserRole
            };
        }
    }
}
