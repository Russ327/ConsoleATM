using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ATMLibrary
{
    
    public class User
    {
        //User id, represents the primary auto incrementing key in the database.
        private readonly long _userID;

        //User pin number for the account.
        private ushort _atmPin;

        public User(long id, ushort atmPin)
        {
            _userID = id;
            _atmPin = atmPin;
        }

        #region Properties

        public long UserID
        {
            get
            {
                return _userID;
            }
        }

        public ushort AtmPin
        {
            get
            {
                return _atmPin;
            }

            set
            {
                _atmPin = value;
            }
        }

        #endregion
    }
}
