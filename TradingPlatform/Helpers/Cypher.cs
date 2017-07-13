using System;
using System.Security.Cryptography;
using System.Text;
using NLog;

namespace TradingPlatform.Helpers
{
    public class Cypher : ICypher
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private const string Key = "ZmIwYzg0NmIyMWQxMzc4MWE5NGQzOTJiYTVhYzRlYzM5NjJhN2JkYQ==";
        private const string publicKey = "VZxf4STG+-h?ktEVxIJuFhW{i(,nMLiqW|%EeK9AfZ,Bsu9k1=R!5*M1{&p}%!v1";

        public string CreateSignature(string data)
        {
            return BytesToBase64(HashToSha1(Key + data + Key));
        }

        public bool CheckSignature(string data, string signature)
        {
            bool isValid = false;
            try
            {
                var checksignature = BytesToBase64(HashToSha1(Key + data + Key));
                if (signature.Equals(checksignature))
                {
                    isValid = true;
                }
                return isValid;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

       
        private static string BytesToBase64(byte[] hashBytes)
        {
            return System.Convert.ToBase64String(hashBytes);
        }


        private static string ConvertToBase64(string plainText)
        {
            return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        private byte[] HashToSha1(string keydatakey)
        {
            byte[] data = Encoding.UTF8.GetBytes(keydatakey);
            byte[] result;
            SHA1 sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(data);
            return result;
        }
    }
}