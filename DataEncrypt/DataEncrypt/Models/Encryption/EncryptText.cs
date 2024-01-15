using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace DataEncrypt.Models.Encryption
{
    static class EncryptText
    {
        // Шифрование текста
        public static string EncryptString(string originalText, byte[] key)
        {
            string result = string.Empty;

            using (Aes aes = Aes.Create()) 
            {
                aes.Key = key;
                using (MemoryStream output = new MemoryStream())
                {
                    output.Write(aes.IV);

                    using (CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
                    {
                        cryptoStream.Write(Encoding.UTF8.GetBytes(originalText));
                    }

                    result = Convert.ToBase64String(output.ToArray());
                }
            };

            return result;
        }

        // Расшифрование текста
        public static string DecryptString(string chiperText, byte[] key)
        {
            string result = string.Empty;

            using (MemoryStream input = new MemoryStream(Convert.FromBase64String(chiperText)))
            {
                byte[] initVector = new byte[16];
                input.Read(initVector);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = initVector;

                    using (CryptoStream cryptoStream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read, true))
                    {
                        using (MemoryStream output = new MemoryStream())
                        {
                            cryptoStream.CopyTo(output);
                            result = Encoding.UTF8.GetString(output.ToArray());
                        }
                    }
                }
            }

            return result;
        }
    }
}
