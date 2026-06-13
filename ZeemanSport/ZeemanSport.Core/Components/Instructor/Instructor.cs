using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Instructor
{
    public class Instructor
    {
        #region Fields

        private int _id;
        private string? _name;
        private string? _photoUrl;
        private bool _isActive;

        #endregion

        #region Constructor

        public Instructor()
        {
            _id = 0;
            _isActive = true;
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

        public string? PhotoUrl
        {
            get => _photoUrl;
            set => _photoUrl = value;
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
