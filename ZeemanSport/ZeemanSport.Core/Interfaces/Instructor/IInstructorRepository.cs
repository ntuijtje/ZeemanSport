using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Instructor
{
    public interface IInstructorRepository
    {
        Task<IReadOnlyCollection<Instructor>> GetAllAsync();
        Task<Instructor?> GetByIdAsync(int id);
        Task<Instructor> SaveAsync(Instructor instructor);
        Task<bool> DeleteAsync(int id);
    }
}
