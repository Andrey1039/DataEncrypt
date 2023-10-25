using System;
using System.Text;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace DataEncrypt.Encryption
{
    static class Keys
    {
        // Генерация нового ключа (32 байта)
        private static byte[] GenerateNewKey()
        {
            byte[] key = new byte[32];

            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);

            return key;
        }

        // Сохранение ключа в реестр
        public static string SaveNewKey(string path, byte[] key)
        {
            using (RegistryKey currentUser = Registry.CurrentUser)
            {
                currentUser.CreateSubKey(path, true);

                using (RegistryKey currentUserKey = currentUser.OpenSubKey(path, true))
                {
                    currentUserKey.SetValue("key", ProtectedData.Protect(key, null, DataProtectionScope.CurrentUser));
                }                   
            }
                
            return Encoding.UTF8.GetString(key);
        }

        // Чтение ключа из реестра
        private static string ReadCurrentKey(string path)
        {
            byte[] key = Array.Empty<byte>();

            using (RegistryKey currentUser = Registry.CurrentUser)
            {
                currentUser.CreateSubKey(path, true);

                try
                {
                    RegistryKey currentUserKey = currentUser.OpenSubKey(path, true);
                    key = (byte[])currentUserKey.GetValue("key");
                }
                catch
                {
                    return "";
                }
            }      

            if (key == null) return "";
            key = ProtectedData.Unprotect(key, null, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(key);
        }

        // Генерация ключа / чтение ключа из реестра
        public static string GenerateKey(bool checkBox)
        {
            string path = @"SOFTWARE\\DataEncrypt";
            string newKey = string.Empty;

            if (checkBox == true)
            {
                newKey = SaveNewKey(path, GenerateNewKey());
            }
            else
            {
                newKey = ReadCurrentKey(path);
                if (newKey == string.Empty)
                    newKey = SaveNewKey(path, GenerateNewKey());
            }

            return newKey;
        }
    }
}
