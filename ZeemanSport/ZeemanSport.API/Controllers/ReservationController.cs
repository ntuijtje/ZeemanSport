using Microsoft.AspNetCore.Mvc;
using ZeemanSport.Core.Contracts.Reservation;
using ZeemanSport.Core.Reservation;

namespace ZeemanSport.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("User/{userId:int}", Name = "GetUserReservations")]
        public async Task<ActionResult<IReadOnlyCollection<ReservationResponse>>> GetForUser(int userId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            IReadOnlyCollection<ReservationResponse> reservations = await _reservationService.GetForUserAsync(userId, from, to);

            return Ok(reservations);
        }

        [HttpGet("User/{userId:int}/History", Name = "GetUserReservationHistory")]
        public async Task<ActionResult<IReadOnlyCollection<ReservationResponse>>> GetHistory(int userId)
        {
            IReadOnlyCollection<ReservationResponse> reservations = await _reservationService.GetHistoryAsync(userId);

            return Ok(reservations);
        }

        [HttpGet("Session/{sessionId:int}/Participants", Name = "GetSessionParticipants")]
        public async Task<ActionResult<IReadOnlyCollection<ParticipantResponse>>> GetParticipants(int sessionId)
        {
            IReadOnlyCollection<ParticipantResponse> participants = await _reservationService.GetParticipantsAsync(sessionId);

            return Ok(participants);
        }

        [HttpPost(Name = "CreateReservation")]
        public async Task<ActionResult<ReservationResponse>> Reserve(CreateReservationRequest request)
        {
            try
            {
                ReservationResponse reservation = await _reservationService.ReserveAsync(request);

                return CreatedAtAction(nameof(GetForUser), new { userId = reservation.UserId }, reservation);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("Session/{sessionId:int}/User/{userId:int}/Cancel", Name = "CancelReservation")]
        public async Task<ActionResult<ReservationResponse>> Cancel(int sessionId, int userId)
        {
            try
            {
                ReservationResponse? reservation = await _reservationService.CancelAsync(sessionId, userId);

                if (reservation == null)
                    return NotFound();

                return Ok(reservation);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("CheckIn", Name = "CheckInReservation")]
        public async Task<ActionResult<ReservationResponse>> CheckIn(CheckInRequest request)
        {
            ReservationResponse? reservation = await _reservationService.CheckInAsync(request);

            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        [HttpGet("Session/{sessionId:int}/Waitlist", Name = "GetWaitlist")]
        public async Task<ActionResult<IReadOnlyCollection<WaitlistEntryResponse>>> GetWaitlist(int sessionId)
        {
            IReadOnlyCollection<WaitlistEntryResponse> waitlist = await _reservationService.GetWaitlistAsync(sessionId);

            return Ok(waitlist);
        }

        [HttpPost("Waitlist", Name = "JoinWaitlist")]
        public async Task<ActionResult<WaitlistEntryResponse>> JoinWaitlist(WaitlistRequest request)
        {
            try
            {
                WaitlistEntryResponse entry = await _reservationService.JoinWaitlistAsync(request);

                return Ok(entry);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("Session/{sessionId:int}/User/{userId:int}/Waitlist", Name = "LeaveWaitlist")]
        public async Task<ActionResult> LeaveWaitlist(int sessionId, int userId)
        {
            bool removed = await _reservationService.LeaveWaitlistAsync(sessionId, userId);

            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
