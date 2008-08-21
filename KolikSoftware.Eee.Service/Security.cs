using System;
using System.Security.Cryptography;
using System.Security;
using System.Runtime.InteropServices;

namespace KolikSoftware.Eee.Service
{
    public class Security
    {
        public static string CreatePasswordHash(SecureString password, string salt)
        {
            HashAlgorithm passwordHash = SHA1.Create();

            IntPtr pwd = IntPtr.Zero;
            
            try
            {
                /*foreach (char c in salt)
                {
                    password.AppendChar(c);
                }*/

                byte[] passwordBytes = new byte[password.Length * 2];
                byte[] clearBytes = new byte[passwordBytes.Length];

                Marshal.Copy((pwd = Marshal.SecureStringToBSTR(password)), passwordBytes, 0, passwordBytes.Length);
                Marshal.Copy(clearBytes, 0, pwd, clearBytes.Length);

                //TODO: hey, this is thus not being safe...
                string passwordInText = Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(password));
                string saltAndPwd = String.Concat(passwordInText, salt);
                string hashedPwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "SHA1");


                //passwordHash.ComputeHash(passwordBytes, 0, passwordBytes.Length);                
                Array.Copy(clearBytes, passwordBytes, clearBytes.Length);

                return hashedPwd;
            }
            finally
            {
                if (pwd != IntPtr.Zero) Marshal.FreeBSTR(pwd);
            }
        }

        public static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
    }
}
