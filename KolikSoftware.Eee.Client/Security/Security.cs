using System;
using System.Security.Cryptography;
using System.Text;

namespace KolikSoftware.Eee.Client
{
	public class Security
	{
        const string K_l_u = "EteuefsjkkevtdgSdfdo4ri";

		public static string CreatePasswordHash(string pwd, string salt)
		{
			string saltAndPwd = String.Concat(pwd, salt);
			string hashedPwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "SHA1");
			return hashedPwd;
		}

		public static string CreateSalt(int size)
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] buff = new byte[size];
			rng.GetBytes(buff);
			return Convert.ToBase64String(buff);
		}

        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = K_l_u;

            /// If hashing use get hashcode regards to your key.
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                /// Always release the resources and flush data of the Cryptographic service provide. Best Practice.
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            /// Set the secret key for the tripleDES algorithm.
            tdes.Key = keyArray;

            /// Mode of operation. there are other 4 modes. We choose ECB(Electronic code Book).
            tdes.Mode = CipherMode.ECB;
            
            /// Padding mode(if any extra byte added).
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();

            /// Transform the specified region of bytes array to resultArray.

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            
            /// Release resources held by TripleDes Encryptor.
            tdes.Clear();
            /// Return the encrypted data into unreadable string format.

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            
            /// Get the byte code of the string.
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = K_l_u;

            if (useHashing)
            {
                /// If hashing was used get the hash code with regards to your key.

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                /// Release any resource held by the MD5CryptoServiceProvider.
                hashmd5.Clear();
            }
            else
            {
                /// If hashing was not implemented get the byte code of the key.
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            
            /// Set the secret key for the tripleDES algorithm.
            tdes.Key = keyArray;

            /// Mode of operation. there are other 4 modes. We choose ECB(Electronic code Book).
            tdes.Mode = CipherMode.ECB;

            /// Padding mode(if any extra byte added).
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            /// Release resources held by TripleDes Encryptor.

            tdes.Clear();
            
            /// Return the Clear decrypted text.
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
	}
}
