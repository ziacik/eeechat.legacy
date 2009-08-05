using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using KolikSoftware.Eee.Service.Properties;

namespace KolikSoftware.Eee.Service
{
    public class EeeEncService : EeePhpService
    {
        Encoding encoding = Encoding.UTF8;

        public EeeEncService(string phpServiceAddress, ProxySettings proxySettings, bool clientInstalled, string clientVersion, string versionNumber) : base(phpServiceAddress, proxySettings, clientInstalled, clientVersion, versionNumber)
        {
        }

        public override bool AddMessage(int roomId, string recipientLogin, string message, string externalFrom)
        {
            message = Encrypt(message);

            return base.AddMessage(roomId, recipientLogin, message, externalFrom);
        }

        protected override string AdjustResponse(string page, string openTag, string closeTag, string response)
        {
            if (page == "getmessages.php")
            {
                string content = response.Substring(openTag.Length, response.Length - openTag.Length - closeTag.Length);

                content = Decrypt(content);

                return openTag + content + closeTag;
            }
            else
            {
                return base.AdjustResponse(page, openTag, closeTag, response);
            }
        }

        string Encrypt(string text)
        {
            int maxLength = 60;
            int position = 0;

            StringBuilder encrypted = new StringBuilder();

            while (position < text.Length)
            {
                int length = maxLength;
                if (text.Length - position < length)
                    length = text.Length - position;

                string chunk = text.Substring(position, length);

                byte[] data = this.encoding.GetBytes(chunk);

                while (data.Length > maxLength)
                {
                    length -= 10;

                    chunk = text.Substring(position, length);
                    data = this.encoding.GetBytes(chunk);
                }

                X509Certificate2 myCertificate;

                try
                {
                    myCertificate = new X509Certificate2(Resources.Private);
                }
                catch
                {
                    throw new CryptographicException("Unable to open key file.");
                }

                RSACryptoServiceProvider rsaObj;
                if (myCertificate.HasPrivateKey)
                {
                    rsaObj = (RSACryptoServiceProvider)myCertificate.PublicKey.Key;
                }
                else
                {
                    throw new CryptographicException("Private key not contained within certificate.");
                }

                if (rsaObj == null)
                    return string.Empty;

                byte[] encryptedBytes;

                try
                {
                    encryptedBytes = rsaObj.Encrypt(data, false);
                }
                catch
                {
                    throw new CryptographicException("Unable to encrypt data.");
                }

                if (encryptedBytes.Length != 0)
                {
                    encrypted.Append(Convert.ToBase64String(encryptedBytes));
                    encrypted.Append("<-chunk->");
                }

                position += length;
            }
            
            return encrypted.ToString();
        }

        string Decrypt(string data)
        {
            string[] chunks = data.Split(new string[] { "<-chunk->" }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder decrypted = new StringBuilder();
            X509Certificate2 myCertificate;

            try
            {
                myCertificate = new X509Certificate2(Resources.Private);
            }
            catch
            {
                throw new CryptographicException("Unable to open key file.");
            }

            RSACryptoServiceProvider rsaObj;
            if (myCertificate.HasPrivateKey)
            {
                rsaObj = (RSACryptoServiceProvider)myCertificate.PrivateKey;
            }
            else
            {
                throw new CryptographicException("Private key not contained within certificate.");
            }

            if (rsaObj == null)
                return string.Empty;

            foreach (string chunk in chunks)
            {
                byte[] decryptedBytes = rsaObj.Decrypt(Convert.FromBase64String(chunk), false);

                if (decryptedBytes.Length != 0)
                    decrypted.Append(this.encoding.GetString(decryptedBytes));
            }

            return decrypted.ToString();
        }
    }
}
