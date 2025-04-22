using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Services.SaveSystem
{
    public class SaveEncryptor
    {
        // Key for encryption (16, 24, or 32 bytes)
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("AAAAAAAAAAAAAAAA");

        // Initialization Vector (IV) for AES encryption
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("BBBBBBBBBBBBBBBB");
        
        public static string EncryptJson(string json)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream
                            swEncrypt.Write(json);
                        }
                    }
                    // Return the encrypted bytes from the memory stream
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string DecryptJson(string encryptedJson)
        {
            var cipherText = Convert.FromBase64String(encryptedJson);

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
