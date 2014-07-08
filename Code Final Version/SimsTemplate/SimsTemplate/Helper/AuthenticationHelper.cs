using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace SimsTemplate.Helper
{

    /// <summary>
    /// @Author: Aubrey Fowler
    /// @Date: Oct 30, 2012
    /// SALT and SHAW1 for passwords
    /// </summary>
    public static class AuthenticationHelper
    {

        /// <summary>
        /// Secret Salt string.
        /// </summary>
        private static string SALT = "Vtfhm23hG2Pk2TkUPd/EBA==";

        /// <summary>
        /// @Author: Aubrey Fowler
        /// ENCRYPT Passwords
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string ENCRYPT_ME(string original)
        {
            return SAW1_Me(Salt_Me(original));
        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// SALT the string.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static string Salt_Me(string original) 
        {
            return original + SALT;
        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// SHA1 the string
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static string SAW1_Me(string original)
        {
        
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = UE.GetBytes(original);
            SHA1Managed hashString = new SHA1Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex;
        }



    }


}