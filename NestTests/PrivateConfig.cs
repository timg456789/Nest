using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace NestTests
{
    public class PrivateConfig
    {
        
        [JsonProperty("secretPadding")]
        public string SecretPadding { get; set; }

        [JsonProperty("secretPassword")]
        public string SecretPassword { get; set; }

        [JsonProperty("secretInitializationVector")]
        public string SecretInitializationVector { get; set; }

        [JsonProperty("nestEncryptedProductId")]
        public string NestEncryptedProductId { get; set; }

        [JsonProperty("nestEncryptedProductSecret")]
        public string NestEncryptedProductSecret { get; set; }

        [JsonProperty("nestEncryptedAuthUrl")]
        public string NestEncryptedAuthUrl { get; set; }

        [JsonProperty("nestEncryptedAccessToken")]
        public string NestEncryptedAccessToken { get; set; }

        [JsonProperty("nestS3Bucket")]
        public string NestS3Bucket { get; set; }

        [JsonProperty("nestS3Key")]
        public string NestS3Key { get; set; }
        
        public string NestDecryptedProductId => Decrypt(NestEncryptedProductId);
        public string NestDecryptedProductSecret => Decrypt(NestEncryptedProductSecret);
        public string NestDecryptedAuthUrl => Decrypt(NestEncryptedAuthUrl);
        public string NestDecryptedAccessToken => Decrypt(NestEncryptedAccessToken);
        
        public string Decrypt(string encryptedSecret)
        {
            var decryptedIp = Decrypt(
                encryptedSecret,
                SecretPassword,
                SecretInitializationVector);
            return decryptedIp.Replace(SecretPadding, string.Empty);
        }

        public static string PersonalJson => "C:\\Users\\peon\\Desktop\\projects\\Memex\\personal.json";

        public static PrivateConfig CreateFromPersonalJson()
        {
            return Create(PersonalJson);
        }

        private static PrivateConfig Create(string fullPath)
        {
            var json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<PrivateConfig>(json);
        }
        
        public static string Decrypt(string encrypted, string keyText, string initializationVectorText)
        {
            SHA256 crypt = SHA256.Create();
            byte[] key = crypt.ComputeHash(Encoding.UTF8.GetBytes(keyText));

            byte[] ivFull = crypt.ComputeHash(Encoding.UTF8.GetBytes(initializationVectorText));
            byte[] initializationVector = new byte[16];
            Array.Copy(ivFull, initializationVector, initializationVector.Length);

            string plaintext;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = initializationVector;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encrypted)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    plaintext = srDecrypt.ReadToEnd();
                }
            }

            return plaintext;
        }
        
    }
}
