using ZeemanSport.Core.Contracts.Reservation;
using ZeemanSport.Core.Contracts.Session;
using ZeemanSport.Core.Contracts.Subscription;
using ZeemanSport.Core.Location;
using ZeemanSport.Core.Reservation;
using ZeemanSport.Core.Session;
using ZeemanSport.Core.Subscription;

namespace ZeemanSport.Runtime.Services
{
    public class ReservationService : IReservationService
    {
        // A session may be reserved up to one week ahead.
        private const int ReservationWindowDays = 7;
        // A reservation can be cancelled up to one hour before the session starts.
        private const int CancellationCutoffHours = 1;
        // Members on the "twice a week" tier may hold at most two reservations per calendar week.
        private const int TwiceWeeklyWeeklyLimit = 2;

        private readonly IReservationRepository _reservationRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ReservationService(
            IReservationRepository reservationRepository,
            ISessionRepository sessionRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _reservationRepository = reservationRepository;
            _sessionRepository = sessionRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<IReadOnlyCollection<ReservationResponse>> GetForUserAsync(int userId, DateTime? from, DateTime? to)
        {
            DateTime rangeStart = from?.Date ?? DateTime.Now.Date;
            DateTime rangeEnd = to ?? rangeStart.AddYears(1);

            return await _reservationRepository.GetByUserAsync(userId, rangeStart, rangeEnd);
        }

        public async Task<IReadOnlyCollection<ReservationResponse>> GetHistoryAsync(int userId)
        {
            DateTime now = DateTime.Now;

            return await _reservationRepository.GetByUserAsync(userId, now.AddYears(-1), now);
        }

        public async Task<ReservationResponse> ReserveAsync(CreateReservationRequest request)
        {
            SessionResponse? session = await _sessionRepository.GetByIdAsync(request.SessionId);

            if (session == null)
                throw new InvalidOperationException("The session does not exist.");

            if (session.Status != SessionStatus.Scheduled)
                throw new InvalidOperationException("The session is not open for reservations.");

            DateTime now = DateTime.Now;

            if (session.StartTime <= now)
                throw new InvalidOperationException("The session has already started.");

            if (session.StartTime.Date > now.Date.AddDays(ReservationWindowDays))
                throw new InvalidOperationException("A session can only be reserved up to one week in advance.");

            ReservationResponse? existing = await _reservationRepository.GetActiveAsync(request.SessionId, request.UserId);

            if (existing != null)
                throw new InvalidOperationException("You already have a reservation for this session.");

            if (session.ReservedCount >= session.Capacity)
                throw new InvalidOperationException("The session is full. Join the waitlist instead.");

            SubscriptionResponse? subscription = await _subscriptionRepository.GetActiveByUserAsync(request.UserId);

            if (subscription == null)
                throw new InvalidOperationException("An active subscription is required to reserve a session.");

            if (subscription.AccessTier == AccessTier.TwiceWeekly)
            {
                DateTime weekStart = GetWeekStart(session.StartTime);
                int weekCount = await _reservationRepository.CountUserWeekReservationsAsync(request.UserId, weekStart, weekStart.AddDays(7));

                if (weekCount >= TwiceWeeklyWeeklyLimit)
                    throw new InvalidOperationException("Your subscription allows at most two sessions per week.");
            }

            (int? seatRow, int? seatColumn) = await ResolveSeatAsync(session, request);

            Reservation reservation = new Reservation
            {
                SessionId = request.SessionId,
                UserId = request.UserId,
                Status = ReservationStatus.Reserved,
                SeatRow = seatRow,
                SeatColumn = seatColumn,
                ReservedAt = now
            };

            return await _reservationRepository.SaveAsync(reservation);
        }

        public async Task<ReservationResponse?> CancelAsync(int sessionId, int userId)
        {
            ReservationResponse? active = await _reservationRepository.GetActiveAsync(sessionId, userId);

            if (active == null)
                return null;

            SessionResponse? session = await _sessionRepository.GetByIdAsync(sessionId);

            if (session != null && DateTime.Now > session.StartTime.AddHours(-CancellationCutoffHours))
                throw new InvalidOperationException("A reservation can only be cancelled up to one hour before the session starts.");

            Reservation reservation = ToDomain(active);
            reservation.Status = ReservationStatus.Cancelled;

            ReservationResponse cancelled = await _reservationRepository.SaveAsync(reservation);

            // The freed spot becomes reservable again. Members on the waitlist are notified through
            // the app's push channel that a spot opened up; delivering that notification is handled
            // outside this API example.
            return cancelled;
        }

        public async Task<ReservationResponse?> CheckInAsync(CheckInRequest request)
        {
            ReservationResponse? active = await _reservationRepository.GetActiveAsync(request.SessionId, request.UserId);

            if (active == null)
                return null;

            Reservation reservation = ToDomain(active);
            reservation.Status = ReservationStatus.CheckedIn;
            reservation.CheckedInAt = DateTime.Now;
            reservation.CheckInMethod = request.CheckInMethod;

            return await _reservationRepository.SaveAsync(reservation);
        }

        public async Task<IReadOnlyCollection<ParticipantResponse>> GetParticipantsAsync(int sessionId)
        {
            return await _reservationRepository.GetParticipantsAsync(sessionId);
        }

        public async Task<WaitlistEntryResponse> JoinWaitlistAsync(WaitlistRequest request)
        {
            SessionResponse? session = await _sessionRepository.GetByIdAsync(request.SessionId);

            if (session == null)
                throw new InvalidOperationException("The session does not exist.");

            if (session.ReservedCount < session.Capacity)
                throw new InvalidOperationException("The session is not full; reserve a spot directly.");

            ReservationResponse? reservation = await _reservationRepository.GetActiveAsync(request.SessionId, request.UserId);

            if (reservation != null)
                throw new InvalidOperationException("You already have a reservation for this session.");

            WaitlistEntryResponse? existing = await _reservationRepository.GetWaitlistEntryAsync(request.SessionId, request.UserId);

            if (existing != null)
                return existing;

            return await _reservationRepository.AddToWaitlistAsync(request.SessionId, request.UserId);
        }

        public async Task<bool> LeaveWaitlistAsync(int sessionId, int userId)
        {
            return await _reservationRepository.RemoveFromWaitlistAsync(sessionId, userId);
        }

        public async Task<IReadOnlyCollection<WaitlistEntryResponse>> GetWaitlistAsync(int sessionId)
        {
            return await _reservationRepository.GetWaitlistAsync(sessionId);
        }

        private async Task<(int? SeatRow, int? SeatColumn)> ResolveSeatAsync(SessionResponse session, CreateReservationRequest request)
        {
            if (session.LocationType != LocationType.SpinningRoom)
                return (null, null);

            if (!request.SeatRow.HasValue || !request.SeatColumn.HasValue)
                throw new InvalidOperationException("A bike must be selected for a spinning session.");

            bool seatExists = session.Seats != null
                && session.Seats.Any(seat => seat.RowIndex == request.SeatRow.Value && seat.ColumnIndex == request.SeatColumn.Value);

            if (!seatExists)
                throw new InvalidOperationException("The selected bike does not exist in this room.");

            if (await _reservationRepository.IsSeatTakenAsync(session.Id, request.SeatRow.Value, request.SeatColumn.Value))
                throw new InvalidOperationException("The selected bike is already taken.");

            return (request.SeatRow, request.SeatColumn);
        }

        private static Reservation ToDomain(ReservationResponse response)
        {
            Reservation reservation = new Reservation
            {
                SessionId = response.SessionId,
                UserId = response.UserId,
                Status = response.Status,
                SeatRow = response.SeatRow,
                SeatColumn = response.SeatColumn,
                ReservedAt = response.ReservedAt,
                CheckedInAt = response.CheckedInAt,
                CheckInMethod = response.CheckInMethod
            };
            reservation.SetId(response.Id);

            return reservation;
        }

        private static DateTime GetWeekStart(DateTime date)
        {
            int diff = (7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7;

            return date.Date.AddDays(-diff);
        }
    }
}
