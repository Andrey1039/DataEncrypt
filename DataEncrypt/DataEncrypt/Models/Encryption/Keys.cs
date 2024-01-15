using System;
using System.Text;
using System.Security.Cryptography;

namespace DataEncrypt.Models.Encryption
{
    static class Keys
    {
        // Генерация нового ключа (32 байта)
        public static byte[] GenerateNewKey()
        {
            byte[] key = new byte[32];

            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);

            return key;
        }

        // Чтение ключа из ресурсов
        public static string ReadKey(string currentKeyVal)
        {
            string key = Properties.Settings.Default.key;
            byte[] byteKey = Array.Empty<byte>();

            try
            {
                byteKey = Convert.FromBase64String(key);
            }
            catch
            {
                byteKey = GenerateNewKey();
                SaveNewKey(byteKey);
            }

            if (byteKey.Length == 32 && !currentKeyVal.Equals(string.Empty))
                return Encoding.UTF8.GetString(byteKey);
            else
            {
                byteKey = GenerateNewKey();
                SaveNewKey(byteKey);
                return Encoding.UTF8.GetString(byteKey);
            }
        }

        // Сохранение нового ключа в ресурсах
        public static void SaveNewKey(byte[] key)
        {
            Array.Resize(ref key, 32);
            Properties.Settings.Default.key = Convert.ToBase64String(key);
        }
    }
}
