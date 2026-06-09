using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Session;

namespace ZeemanSport.Core.Session
{
    public interface ISessionService
    {
        Task<IReadOnlyCollection<SessionResponse>> GetScheduleAsync(DateTime from, DateTime to);
        Task<SessionResponse> GetByIdAsync(int id);
        Task<IReadOnlyCollection<SessionResponse>> GetInstructorSessionsAsync(int instructorId, DateTime from, DateTime to);
        Task<IReadOnlyCollection<SessionResponse>> CreateAsync(CreateSessionRequest request);
        Task<SessionResponse> UpdateAsync(int id, UpdateSessionRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
