using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Reservation;

namespace ZeemanSport.Core.Reservation
{
    public interface IReservationService
    {
        Task<IReadOnlyCollection<ReservationResponse>> GetForUserAsync(int userId, DateTime? from, DateTime? to);
        Task<IReadOnlyCollection<ReservationResponse>> GetHistoryAsync(int userId);
        Task<ReservationResponse> ReserveAsync(CreateReservationRequest request);
        Task<ReservationResponse?> CancelAsync(int sessionId, int userId);
        Task<ReservationResponse?> CheckInAsync(CheckInRequest request);
        Task<IReadOnlyCollection<ParticipantResponse>> GetParticipantsAsync(int sessionId);

        Task<WaitlistEntryResponse> JoinWaitlistAsync(WaitlistRequest request);
        Task<bool> LeaveWaitlistAsync(int sessionId, int userId);
        Task<IReadOnlyCollection<WaitlistEntryResponse>> GetWaitlistAsync(int sessionId);
    }
}
