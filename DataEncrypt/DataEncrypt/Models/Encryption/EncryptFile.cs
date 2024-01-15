using System.IO;
using System.Security.Cryptography;

namespace DataEncrypt.Models.Encryption
{
    static class EncryptFile
    {
        // Шифрование файлов
        public static void EncryptFiles(string path, byte[] key)
        {
            string tmpPath = Path.GetTempFileName();

            using (FileStream reader = File.OpenRead(path))
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    using (FileStream writer = File.Create(tmpPath))
                    {
                        writer.Write(aes.IV);
                        using (CryptoStream cryptoStream = new CryptoStream(writer, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
                        {
                            reader.CopyTo(cryptoStream);
                        }
                    }
                }
            }
            
            File.Delete(path);
            File.Move(tmpPath, path);
        }

        // Расшифрование файлов
        public static void DecryptFiles(string path, byte[] key)
        {
            string tmpPath = Path.GetTempFileName();

            using (FileStream reader = File.OpenRead(path))
            {
                byte[] initVector = new byte[16];
                reader.Read(initVector);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = initVector;

                    using (CryptoStream cryptoStream = new CryptoStream(reader, aes.CreateDecryptor(), CryptoStreamMode.Read, true))
                    {
                        using (FileStream writer = File.Create(tmpPath))
                        {
                            cryptoStream.CopyTo(writer);
                        }
                    }                   
                }
            }

            File.Delete(path);
            File.Move(tmpPath, path);
        }
    }
}
