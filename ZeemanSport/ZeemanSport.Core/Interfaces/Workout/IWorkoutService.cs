using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Workout;

namespace ZeemanSport.Core.Workout
{
    public interface IWorkoutService
    {
        Task<IReadOnlyCollection<WorkoutResponse>> GetAllAsync();
        Task<WorkoutResponse> GetByIdAsync(int id);
        Task<WorkoutResponse> CreateAsync(CreateWorkoutRequest request);
        Task<WorkoutResponse> UpdateAsync(int id, UpdateWorkoutRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
