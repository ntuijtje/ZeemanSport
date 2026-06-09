using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Workout
{
    public class Workout
    {
        #region Fields

        private int _id;
        private string? _name;
        private string? _description;
        private bool _isActive;

        #endregion

        #region Constructor

        public Workout()
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

        public string? Description
        {
            get => _description;
            set => _description = value;
        }

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        #endregion

        public void SetId(int id)
        {
            if (_id == 0)
                _id = id;
        }
    }
}
