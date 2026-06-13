using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Instructor;

namespace ZeemanSport.Core.Instructor
{
    public interface IInstructorService
    {
        Task<IReadOnlyCollection<InstructorResponse>> GetAllAsync();
        Task<InstructorResponse?> GetByIdAsync(int id);
        Task<InstructorResponse> CreateAsync(CreateInstructorRequest request);
        Task<InstructorResponse?> UpdateAsync(int id, UpdateInstructorRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
