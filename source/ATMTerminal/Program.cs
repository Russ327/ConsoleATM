using ATMLibrary;
using ATMLibrary.Accounts;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ATMTerminal
{
   
    class Program
    {
        private enum AtmState
        {
            MainScreen,
            SelectPin,
            UserPortal
        }

        //Asking visual studio to tell me to stop making it read only.
        #pragma warning disable IDE0044 // Add readonly modifier
        private static AtmState s_currentState = AtmState.MainScreen;
        #pragma warning restore IDE0044 // Add readonly modifier

        private static User s_currentUser;
     
        static void Main(string[] args)
        {
            bool processActive = true;

            //No way to use an actual card reader so this uses a login system. This is not a real world application
            //but rather a demonstration that I know how to use SQL.
            Console.WriteLine("Please log in or register to continue. Parameters are email and password");
            Console.WriteLine("For example, login test@test.com password123");

            while(processActive == true)
            {
                if(ExecuteUpdate() == false)
                {
                    processActive = false;
                    break;
                }
                Thread.Sleep(750);
            }
        }

        private static bool ExecuteUpdate()
        {
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) == false)
            {
                if (input.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                switch (s_currentState)
                {
                    case AtmState.MainScreen:
                        ExecuteMainMenu(input);
                        break;

                    case AtmState.SelectPin:
                        ExecuteSelectPinMenu(input);
                        break;

                    case AtmState.UserPortal:
                        ExecutePortalMenu(input);
                        break;
                }
            }
            return true;
        }

        private static void ExecuteMainMenu(string input)
        {
            string[] args = input.Split(" ");
            if (args.Length == 3)
            {
                if (args[0].Equals("register", StringComparison.InvariantCultureIgnoreCase))
                {
                    CheckRegisterCMD(args);
                }
                else if(args[0].Equals("login", StringComparison.InvariantCultureIgnoreCase))
                {
                    CheckLoginCMD(args);
                }
            }
            else
            {
                Console.WriteLine("USAGE: action email password");
            }
        }

        
        private static void CheckRegisterCMD(string[] args)
        {
            if (IsEmail(args[1]) && args[2].Length >= 5)
            {
                if (UserManager.EmailExists(args[1]) == false)
                {
                    if(UserManager.RegisterUser(args[1], args[2]) == true)
                    {
                        Console.WriteLine("Please log in to continue!");
                        s_currentState = AtmState.MainScreen;
                    }
                    else
                    {
                        Console.WriteLine("Email in use");
                    }
                }
                else
                {
                    Console.WriteLine("Email in use");
                }
            }

            else
            {
                Console.WriteLine("ERROR: Either that was an invalid email or your password was less than 5 characters.");
            }
        }
        
        private static void CheckLoginCMD(string[] args)
        {
            if (IsEmail(args[1]) && args[2].Length >= 5)
            {
                if (UserManager.EmailExists(args[1]) == true)
                {
                    VerifyLogin(args);
                }
                else
                {
                    Console.WriteLine("That email is not registered.");
                }
            }
            else
            {
                Console.WriteLine("Either your email is not valid or you have entered too short of a password");
            }
        }

        private static void VerifyLogin(string[] args)
        {
            s_currentUser = UserManager.LoginUser(args[1], args[2]);
            if (s_currentUser != null)
            {
                VerifyPinExists();
            }
            else
            {
                Console.WriteLine("Account login failed");
            }
        }

        private static void VerifyPinExists()
        {
            if (s_currentUser.AtmPin != ushort.MaxValue)
            {
                SendToPortal();
            }
            else
            {
                Console.WriteLine("You have not selected a pin yet. Please type a four digit numeric code below");
                s_currentState = AtmState.SelectPin;
            }
        }

        /// <summary>
        /// Called whent he console reaches the portal. This method parses the input passed and determines
        /// a users desired action. There is a lot of code going on here so it is split into multiple
        /// different methods.
        /// </summary>
        /// <param name="input">input passed to the portal.</param>

        private static void ExecutePortalMenu(string input)
        {
            string[] args = input.Split(" ");
            if (args.Length > 0)
            {
                if (args[0].Equals("accounts", StringComparison.InvariantCultureIgnoreCase))
                {
                    List<BankAccount> accounts = AccountManager.FetchAccounts(s_currentUser);
                    Console.WriteLine("Opened bank accounts");
                    foreach(BankAccount bankAcc in accounts)
                    {
                        Console.WriteLine("AccountID: {0} AccountType : {1} Balance: {2}", bankAcc.ID, bankAcc.ToString(), bankAcc.Balance);
                    }
                    accounts.Clear();
                }

                else if(args[0].Equals("createaccount",StringComparison.InvariantCultureIgnoreCase))
                {
                    if(args.Length == 2)
                    {
                        Type acc = null;
                        if (args[1].Equals("checking", StringComparison.InvariantCultureIgnoreCase))
                        {
                            acc = typeof(CheckingAccount);
                        }
                        else if(args[1].Equals("savings", StringComparison.InvariantCultureIgnoreCase))
                        {
                            acc = typeof(SavingsAccount);
                        }
                        else if(args[1].Equals("business", StringComparison.InvariantCultureIgnoreCase))
                        {
                            acc = typeof(BusinessAccount);
                        }

                        if(AccountManager.InsertAccount(acc))
                        {

                        }
                    }
                    else
                    {
                        Console.WriteLine("USAGE createaccount [type] - Valid types are checking, savings, business");
                    }
                }
            }
        }

        /// <summary>
        /// Sends user to the portal, printing available commands to the console
        /// </summary>
        private static void SendToPortal()
        {
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("Logged in. Welcome to the user portal. Available commands are ");
            Console.WriteLine("accounts - used to view current bank accounts opened with us.");
            Console.WriteLine("createaccount type - used to create a new bank account. Valid types are checking, savings, business");
            Console.WriteLine("You can quit at anytime by typing 'quit' or logout by typing 'logout'");
            s_currentState = AtmState.UserPortal;
        }


        /// <summary>
        /// Takes given input and sets it to the current pin.
        /// </summary>
        /// <param name="input">desired pin</param>
        private static void ExecuteSelectPinMenu(string input)
        {
            if (input.Length != 4)
            {
                if (ushort.TryParse(input, out ushort pin))
                {
                    if (UserManager.UpdatePin(s_currentUser.UserID, pin) == 0)
                    {
                        SendToPortal();
                    }
                    else
                    {
                        Console.WriteLine("Either the user is not in the database or is in it twice. Please seek a DBA");
                    }
                }
                else
                {
                    Console.WriteLine("Pin must be a numeric value");
                }
            }
            else
            {
                Console.WriteLine("Pin must be a 4 digit numeric value");
            }
        }

        private static bool IsEmail(string text)
        {
            if (text.Contains('@') && text.Contains('.'))
                return true;
            return false;
        }

        
    }
}
