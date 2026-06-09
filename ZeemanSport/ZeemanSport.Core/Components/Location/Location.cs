using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Location
{
    public class Location
    {
        #region Fields

        private int _id;
        private string? _name;
        private int _capacity;
        private int _widthInSeats;
        private int _heightInSeats;
        private LocationType _locationType;
        private bool _isActive;

        #endregion

        #region Constructor

        public Location()
        {
            _id = 0;
            _capacity = 0;
            _widthInSeats = 0;
            _heightInSeats = 0;
        }

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
        }

        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        public int Capacity
        {
            get => _capacity;
            set => _capacity = value;
        }

        public int WidthInSeats
        {
            get => _widthInSeats;
            set => _widthInSeats = value;
        }

        public int HeightInSeats
        {
            get => _heightInSeats;
            set => _heightInSeats = value;
        }

        public LocationType LocationType
        {
            get => _locationType;
            set => _locationType = value;
        }

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
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
