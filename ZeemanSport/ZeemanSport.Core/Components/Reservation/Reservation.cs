using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Reservation
{
    public class Reservation
    {
        #region Fields

        private int _id;
        private int _sessionId;
        private int _userId;
        private ReservationStatus _status;
        private int? _seatRow;
        private int? _seatColumn;
        private DateTime _reservedAt;
        private DateTime? _checkedInAt;
        private CheckInMethod? _checkInMethod;

        #endregion

        #region Constructor

        public Reservation()
        {
            _id = 0;
            _status = ReservationStatus.Reserved;
        }

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
        }

        public int SessionId
        {
            get => _sessionId;
            set => _sessionId = value;
        }

        public int UserId
        {
            get => _userId;
            set => _userId = value;
        }

        public ReservationStatus Status
        {
            get => _status;
            set => _status = value;
        }

        public int? SeatRow
        {
            get => _seatRow;
            set => _seatRow = value;
        }

        public int? SeatColumn
        {
            get => _seatColumn;
            set => _seatColumn = value;
        }

        public DateTime ReservedAt
        {
            get => _reservedAt;
            set => _reservedAt = value;
        }

        public DateTime? CheckedInAt
        {
            get => _checkedInAt;
            set => _checkedInAt = value;
        }

        public CheckInMethod? CheckInMethod
        {
            get => _checkInMethod;
            set => _checkInMethod = value;
        }

        #endregion

        #region Methods

        public void SetId(int id)
        {
            if (_id == 0)
                _id = id;
        }

        #endregion
    }
}
