using System;
using System.Collections.Generic;
using System.Text;

namespace ATMLibrary.Accounts
{
    public class BankAccount
    {
        //represents the bank account id. This is just the primary key in the database of this account.
        private readonly long _id;
        //the balance represents the available funds in said bank account.
        private readonly long _balance;


        internal BankAccount(long id, long balance)
        {
            _id = id;
            _balance = balance;
        }

        public long ID
        {
            get
            {
                return _id;
            }
        }

        public long Balance
        {
            get
            {
                return _balance;
            }
        }
    }
}
