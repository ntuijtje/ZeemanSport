using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Reservation;

namespace ZeemanSport.Core.Reservation
{
    public interface IReservationRepository
    {
        Task<IReadOnlyCollection<ReservationResponse>> GetByUserAsync(int userId, DateTime from, DateTime to);
        Task<ReservationResponse?> GetActiveAsync(int sessionId, int userId);
        Task<ReservationResponse?> GetByIdAsync(int id);
        Task<int> CountUserWeekReservationsAsync(int userId, DateTime weekStart, DateTime weekEnd);
        Task<bool> IsSeatTakenAsync(int sessionId, int seatRow, int seatColumn);
        Task<IReadOnlyCollection<ParticipantResponse>> GetParticipantsAsync(int sessionId);
        Task<ReservationResponse> SaveAsync(Reservation reservation);

        Task<WaitlistEntryResponse?> GetWaitlistEntryAsync(int sessionId, int userId);
        Task<IReadOnlyCollection<WaitlistEntryResponse>> GetWaitlistAsync(int sessionId);
        Task<WaitlistEntryResponse> AddToWaitlistAsync(int sessionId, int userId);
        Task<bool> RemoveFromWaitlistAsync(int sessionId, int userId);
    }
}
