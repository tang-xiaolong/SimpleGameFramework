using System;
using System.Text;

public class XorEncryptor : IEncryptor
{
    private readonly byte _key;
 
    public XorEncryptor(byte key)
    {
        _key = key;
    }
 
    public string Encrypt(string info)
    {
        if (string.IsNullOrEmpty(info))
        {
            return info;
        }
 
        byte[] infoBytes = Encoding.UTF8.GetBytes(info);
        byte[] encryptedBytes = new byte[infoBytes.Length];
 
        for (int i = 0; i < infoBytes.Length; i++)
        {
            encryptedBytes[i] = (byte)(infoBytes[i] ^ _key);
        }
 
        return Convert.ToBase64String(encryptedBytes);
    }
 
    public string Decrypt(string info)
    {
        if (string.IsNullOrEmpty(info))
        {
            return info;
        }
 
        byte[] infoBytes = Convert.FromBase64String(info);
        byte[] decryptedBytes = new byte[infoBytes.Length];
 
        for (int i = 0; i < infoBytes.Length; i++)
        {
            decryptedBytes[i] = (byte)(infoBytes[i] ^ _key);
        }
 
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}