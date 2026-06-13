using ZeemanSport.Core.Contracts.Session;
using ZeemanSport.Core.Session;

namespace ZeemanSport.Runtime.Services
{
    public class SessionService : ISessionService
    {
        // Safety cap so a misconfigured recurrence can never create an unbounded number of sessions.
        private const int MaxOccurrences = 366;

        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IReadOnlyCollection<SessionResponse>> GetScheduleAsync(DateTime from, DateTime to)
        {
            return await _sessionRepository.GetScheduleAsync(from, to);
        }

        public async Task<SessionResponse?> GetByIdAsync(int id)
        {
            return await _sessionRepository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyCollection<SessionResponse>> GetInstructorSessionsAsync(int instructorId, DateTime from, DateTime to)
        {
            return await _sessionRepository.GetInstructorSessionsAsync(instructorId, from, to);
        }

        public async Task<IReadOnlyCollection<SessionResponse>> CreateAsync(CreateSessionRequest request)
        {
            List<SessionResponse> created = new List<SessionResponse>();

            foreach (DateTime startTime in ExpandOccurrences(request))
            {
                Session session = new Session
                {
                    WorkoutId = request.WorkoutId,
                    InstructorId = request.InstructorId ?? 0,
                    LocationId = request.LocationId,
                    StartTime = startTime,
                    DurationMinutes = request.DurationMinutes,
                    Status = SessionStatus.Scheduled
                };

                created.Add(await _sessionRepository.SaveAsync(session));
            }

            return created;
        }

        public async Task<SessionResponse?> UpdateAsync(int id, UpdateSessionRequest request)
        {
            SessionResponse? existing = await _sessionRepository.GetByIdAsync(id);

            if (existing == null)
                return null;

            Session session = new Session
            {
                WorkoutId = request.WorkoutId,
                InstructorId = request.InstructorId ?? 0,
                LocationId = request.LocationId,
                StartTime = request.StartTime,
                DurationMinutes = request.DurationMinutes,
                Status = request.Status
            };
            session.SetId(id);

            return await _sessionRepository.SaveAsync(session);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _sessionRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Expands a create request into the concrete start times to schedule, honouring the
        /// recurrence type and either an end date or a fixed number of repetitions.
        /// </summary>
        private static IEnumerable<DateTime> ExpandOccurrences(CreateSessionRequest request)
        {
            if (request.RecurrenceType == RecurrenceType.None)
            {
                yield return request.StartTime;
                yield break;
            }

            int maxCount = request.RepeatCount.HasValue
                ? Math.Min(request.RepeatCount.Value, MaxOccurrences)
                : MaxOccurrences;

            DateTime current = request.StartTime;

            for (int count = 0; count < maxCount; count++)
            {
                if (request.RepeatUntil.HasValue && current.Date > request.RepeatUntil.Value.Date)
                    yield break;

                yield return current;

                current = request.RecurrenceType switch
                {
                    RecurrenceType.Daily => current.AddDays(1),
                    RecurrenceType.Weekly => current.AddDays(7),
                    RecurrenceType.Monthly => current.AddMonths(1),
                    _ => current
                };
            }
        }
    }
}
