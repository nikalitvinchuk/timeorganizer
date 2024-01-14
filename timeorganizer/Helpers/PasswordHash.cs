using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace timeorganizer.Helpers
{
    public class Passwordhash
    {
#pragma warning disable SYSLIB0023 // Typ lub składowa jest przestarzała
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
#pragma warning restore SYSLIB0023 // Typ lub składowa jest przestarzała
        private static readonly int Saltzise = 16;
        private static readonly int HashSize = 20;
        private static readonly int Interations = 1000;

        public static string HashPassword(string password)
        {
            byte[] salt;
            rng.GetBytes(salt = new byte[Saltzise]);
#pragma warning disable SYSLIB0041 // Typ lub składowa jest przestarzała
            var key = new Rfc2898DeriveBytes(password, salt, Interations);

            var hash = key.GetBytes(HashSize);

            var hashBytes = new byte[Saltzise + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, Saltzise);
            Array.Copy(hash, 0, hashBytes, Saltzise, HashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);
            return base64Hash;
        }
        public static bool Veryfypass(string password, string base64hash)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(base64hash);

                var salt = new byte[Saltzise];
                Array.Copy(hashBytes, 0, salt, 0, Saltzise);

                var key = new Rfc2898DeriveBytes(password, salt, Interations);

                byte[] hash = key.GetBytes(HashSize);
                for (var i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + Saltzise] != hash[i])
                    {
                        return false;
                    }
                }
                return true;
                {

                }
            }catch (Exception ex)
            {
                return false;
            }
        }
    }
}
