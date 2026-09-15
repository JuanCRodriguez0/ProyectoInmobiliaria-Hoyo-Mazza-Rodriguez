using System.Security.Cryptography;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public static string HashClave(string claveTextoPlano)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(claveTextoPlano, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerificarClave(string claveTextoPlano, string claveAlmacenada)
        {
            var partes = claveAlmacenada.Split('.');
            if (partes.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(partes[0]);
            byte[] hashGuardado = Convert.FromBase64String(partes[1]);
            byte[] hashIngresado = Rfc2898DeriveBytes.Pbkdf2(claveTextoPlano, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

            return CryptographicOperations.FixedTimeEquals(hashGuardado, hashIngresado);
        }
    }
}