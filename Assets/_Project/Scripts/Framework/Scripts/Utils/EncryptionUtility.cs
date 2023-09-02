using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Zeff.Extensions
{
    public static class EncryptionUtility
    {
        public static string Encrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    byte[] iv = aesAlg.IV;
                    byte[] encryptedContent = msEncrypt.ToArray();

                    // Combine IV and encrypted content for storage/transmission
                    byte[] combined = new byte[iv.Length + encryptedContent.Length];
                    Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
                    Buffer.BlockCopy(encryptedContent, 0, combined, iv.Length, encryptedContent.Length);

                    return Convert.ToBase64String(combined);
                }
            }
        }

        public static string Decrypt(string encryptedText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Extract IV and encrypted content from the combined data
                byte[] combined = Convert.FromBase64String(encryptedText);
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                byte[] encryptedContent = new byte[combined.Length - iv.Length];

                Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(combined, iv.Length, encryptedContent, 0, encryptedContent.Length);

                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(encryptedContent))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}