using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Location;
using ZeemanSport.Core.Session;

namespace ZeemanSport.Core.Session
{
    public class Session
    {
        #region Fields

        private int _id;
        private int _workoutId;
        private int _instructorId;
        private int _locationId;
        private DateTime _startTime;
        private int _durationMinutes;
        private SessionStatus _status;
        private Seat[,] _seats;

        #endregion

        #region Constructor

        public Session()
        {
            _id = 0;
            _status = SessionStatus.Scheduled;
            _seats = new Seat[0,0];
        }

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
        }

        public int WorkoutId
        {
            get => _workoutId;
            set => _workoutId = value;
        }

        public int InstructorId
        {
            get => _instructorId;
            set => _instructorId = value;
        }

        public int LocationId
        {
            get => _locationId;
            set => _locationId = value;
        }

        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        public int DurationMinutes
        {
            get => _durationMinutes;
            set => _durationMinutes = value;
        }

        public SessionStatus Status
        {
            get => _status;
            set => _status = value;
        }

        public Seat[,] Seats
        {
            get => _seats;
            set => _seats = value;
        }

        #endregion

        #region Methods

        public void SetId(int id)
        {
            if (_id == 0)
                _id = id;
        }

        public void CreateSeats(int width, int height)
        {
            Seat[,] seats = new Seat[width, height];

            for (int h = 0; h < height; h++)
                for (int w = 0; w < width; w++)
                    seats[w, h] = new Seat()
                    {
                        IsReserved = false,
                        IsSelected = false,
                        LocationId = _locationId,
                        RowIndex = h,
                        ColumnIndex = w,
                    };

            _seats = seats;
        }

        #endregion
    }
}
