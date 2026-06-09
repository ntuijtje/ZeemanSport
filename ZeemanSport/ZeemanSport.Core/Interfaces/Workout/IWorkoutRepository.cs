using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Workout
{
    public interface IWorkoutRepository
    {
        Task<IReadOnlyCollection<Workout>> GetAllAsync();
        Task<Workout> GetByIdAsync(int id);
        Task<Workout> SaveAsync(Workout workout);
        Task<bool> DeleteAsync(int id);
    }
}
