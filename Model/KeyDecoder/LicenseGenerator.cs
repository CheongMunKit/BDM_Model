using System;
using System.IO;
using System.Security.Cryptography;

namespace Model.Protection
{
    public class LicenseManager
    {
        static bool IsLicenseFound = false;
        static string filePath = @"C:\Users\Public\Documents\microsoft\";
        static string fileName = filePath + "EuresysLicense.txt";
        static string password = "09210Jms";
        const int SaltSize = 8;

        static void Encrypt(string content)
        {
            Directory.CreateDirectory(filePath);
            FileInfo targetFile = new FileInfo(fileName);

            var keyGenerator = new Rfc2898DeriveBytes(password, SaltSize);
            var rijndael = Rijndael.Create();

            // BlockSize, KeySize in bit --> divide by 8
            rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
            rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

            using (var fileStream = targetFile.Create())
            {
                // write random salt
                fileStream.Write(keyGenerator.Salt, 0, SaltSize);

                using (var cryptoStream = new CryptoStream(fileStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(content);
                    cryptoStream.Write(data, 0, data.Length);
                }
            }
        }
        static string Decrypt()
        {
            FileInfo sourceFile = new FileInfo(fileName);

            // read salt
            var fileStream = sourceFile.OpenRead();
            var salt = new byte[SaltSize];
            fileStream.Read(salt, 0, SaltSize);

            // initialize algorithm with salt
            var keyGenerator = new Rfc2898DeriveBytes(password, salt);
            var rijndael = Rijndael.Create();
            rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
            rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

            // decrypt
            string data = "";
            using (var cryptoStream = new CryptoStream(fileStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
            {
                using (StreamReader reader = new StreamReader(cryptoStream))
                {
                    data = reader.ReadToEnd();
                }
            }

            return data;
        }

        public static void CreateLicense()
        {
            Encrypt(KeyDecoder.GetWindowsProductKey());
        }
        public static void CheckLicense()
        {
            if (IsLicenseFound) return;
            else
            {
                if (!File.Exists(fileName)) throw new Exception("Euresys License Not Found");
                if (KeyDecoder.GetWindowsProductKey() != Decrypt()) { throw new Exception("Euresys License Not Found"); }
                IsLicenseFound = true;
            }

        }
    }
}
