using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.User
{
    public class User : IUser
    {
        #region Fields

        private int _id;
        private string? _userName;
        private string? _password;
        private string? _firstName;
        private string? _lastName;
        private UserRole _userRole;

        #endregion

        #region Constructor

        public User()
        {
            _id = 0;
        }

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
        }

        public string? UserName
        {
            get => _userName;
            set => _userName = value;
        }

        public string? Password
        {
            get => _password;
            set => _password = value;
        }

        public string? FirstName
        {
            get => _firstName; 
            set => _firstName = value;
        }

        public string? LastName
        {
            get => _lastName;
            set => _lastName = value;
        }

        public UserRole UserRole
        {
            get => _userRole;
            set => _userRole = value;
        }

        #endregion

        public void SetId(int Id)
        {
            if (_id == 0)
                _id = Id;
        }
    }
}
