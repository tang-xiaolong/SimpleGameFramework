using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AesEncryptor : IEncryptor
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public AesEncryptor(string key)
    {
        using (var sha256 = SHA256.Create())
        {
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            _iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        }
    }

    public string Encrypt(string info)
    {
        if (string.IsNullOrEmpty(info)) return info;

        using (Aes aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(info);
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    public string Decrypt(string info)
    {
        if (string.IsNullOrEmpty(info)) return info;

        using (Aes aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(info)))
            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (StreamReader streamReader = new StreamReader(cryptoStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
