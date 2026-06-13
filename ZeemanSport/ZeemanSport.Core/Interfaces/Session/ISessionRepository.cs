using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Session;

namespace ZeemanSport.Core.Session
{
    public interface ISessionRepository
    {
        Task<IReadOnlyCollection<SessionResponse>> GetScheduleAsync(DateTime from, DateTime to);
        Task<SessionResponse?> GetByIdAsync(int id);
        Task<IReadOnlyCollection<SessionResponse>> GetInstructorSessionsAsync(int instructorId, DateTime from, DateTime to);
        Task<SessionResponse> SaveAsync(Session session);
        Task<bool> DeleteAsync(int id);
    }
}
