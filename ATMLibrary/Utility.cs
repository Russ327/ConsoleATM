using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ATMLibrary
{
    public class Utility
    {
        public static string HashPassword(string input)
        {
            using (MD5 hash = MD5.Create())
            {
                byte[] hashedBytes = hash.ComputeHash(Encoding.ASCII.GetBytes(input));
                return BitConverter.ToString(hashedBytes);
            }
        }
    }
}
