using System;
using NUnit.Framework;
using System.Text;
using System.Diagnostics;

public class EncryptorTestBase
{
    protected static readonly string[] TestData = new[]
    {
        // 空值和边界测试
        "",
        " ",
        "  ",
        "\n",
        "\r\n",
        "\t",
        
        // 基础ASCII文本
        "Hello, World!",
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
        "abcdefghijklmnopqrstuvwxyz",
        "0123456789",
        
        // 特殊字符组合
        "!@#$%^&*()_+-=[]{}|;:,.<>?",
        "`~!@#$%^&*()_+-=[]\\{}|;':\",./<>?",
        "................................................",
        "&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&",
        "\0\0\0\0\0",  // null字符
        new string('\u0001', 10),  // 控制字符
        
        // Unicode文本
        "你好，世界！",
        "안녕하세요 세계!",
        "Привет, мир!",
        "مرحبا بالعالم!",
        "🌟🌙⭐🌎🌍🌏💫✨",  // Emoji
        "Hello™®©",  // 特殊Unicode符号
        
        // HTML/XML类似内容
        "<html><body><h1>Hello</h1></body></html>",
        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
        "<root><child attr=\"value\">content</child></root>",
        
        // JSON类似内容
        "{\"key\":\"value\",\"number\":123,\"boolean\":true}",
        "[1,2,3,{\"nested\":\"object\"}]",
        
        // Base64类似内容
        "TWFuIGlzIGRpc3Rpbmd1aXNoZWQsIG5vdCBvbmx5IGJ5IGhpcyByZWFzb24sIGJ1dCBieSB0aGlz",
        
        // URL类似内容
        "https://www.example.com/path?param1=value1&param2=value2",
        
        // 文件路径类似内容
        @"C:\Program Files\Application\file.txt",
        "/usr/local/bin/application",
        
        // 混合内容
        "Hello123!@#$%你好World世界",
        "12345ABC!@#$%你好안녕하세요Привет",
        
        // 不同长度的重复内容
        new string('A', 10),    // 短文本
        new string('B', 100),   // 中等文本
        new string('C', 1024),  // 1KB
        new string('D', 4096),  // 4KB
        
        // 随机生成的长文本
        GenerateRandomString(2048),   // 2KB随机ASCII
        GenerateRandomUnicodeString(2048),  // 2KB随机Unicode
        
        // 模拟实际数据
        "user123@example.com",  // 电子邮件
        "+1 (234) 567-8900",    // 电话号码
        "4111-1111-1111-1111",  // 信用卡格式
        "2023-12-31T23:59:59Z", // ISO日期时间
        
        // 超长文本
        Convert.ToBase64String(Encoding.UTF8.GetBytes(new string('E', 10000))),  // ~13KB
        Convert.ToBase64String(Encoding.UTF8.GetBytes(GenerateRandomString(10000)))  // ~13KB随机
    };

    // 生成随机ASCII字符串的辅助方法
    private static string GenerateRandomString(int length)
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        var stringChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        return new string(stringChars);
    }

    // 生成随机Unicode字符串的辅助方法
    private static string GenerateRandomUnicodeString(int length)
    {
        var random = new Random();
        var stringChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = (char)random.Next(0x4E00, 0x9FFF); // 基本汉字范围
        }
        return new string(stringChars);
    }

    protected static readonly string[] AesKeys = new[]
    {
        "shortkey",  // 短密钥
        "this-is-a-very-long-key-for-testing",  // 长密钥
        "123456789",  // 数字密钥
        "特殊密钥@#$%",  // 特殊字符密钥
    };

    protected static readonly byte[] XorKeys = new[]
    {
        (byte)0x55,  // 交替位模式
        (byte)0xFF,  // 全1
        (byte)0x00,  // 全0
        (byte)0xA5,  // 混合模式
    };

    protected void VerifyEncryption(IEncryptor encryptor, string input)
    {
        var encrypted = encryptor.Encrypt(input);
        var decrypted = encryptor.Decrypt(encrypted);
        
        Assert.That(decrypted, Is.EqualTo(input), "Decrypted text should match original");
        if (!string.IsNullOrEmpty(input))
        {
            Assert.That(encrypted, Is.Not.EqualTo(input), "Encrypted text should not match original");
        }
    }

    protected void MeasurePerformance(IEncryptor encryptor, string input, string testName)
    {
        var sw = new Stopwatch();
        
        sw.Start();
        var encrypted = encryptor.Encrypt(input);
        var encryptTime = sw.ElapsedMilliseconds;
        
        sw.Restart();
        var decrypted = encryptor.Decrypt(encrypted);
        var decryptTime = sw.ElapsedMilliseconds;
        
        TestContext.WriteLine($"{testName} - Input length: {input.Length}");
        TestContext.WriteLine($"Encrypt time: {encryptTime}ms");
        TestContext.WriteLine($"Decrypt time: {decryptTime}ms");
    }
}

