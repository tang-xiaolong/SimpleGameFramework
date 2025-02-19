using NUnit.Framework;

[TestFixture]
public class AesEncryptorTests : EncryptorTestBase
{
    [Test]
    public void AesEncryptor_SameKey_ShouldProduceSameResult()
    {
        var encryptor1 = new AesEncryptor("test-key");
        var encryptor2 = new AesEncryptor("test-key");
        var input = "Test String";
        
        Assert.That(encryptor1.Encrypt(input), Is.EqualTo(encryptor2.Encrypt(input)));
    }

    [TestCaseSource(nameof(AesKeys))]
    public void AesEncryptor_DifferentKeys_BasicTest(string key)
    {
        var encryptor = new AesEncryptor(key);
        VerifyEncryption(encryptor, "Test String");
    }

    [TestCaseSource(nameof(TestData))]
    public void AesEncryptor_DifferentInputs_WithDefaultKey(string input)
    {
        var encryptor = new AesEncryptor("default-test-key");
        VerifyEncryption(encryptor, input);
    }

    [Test]
    public void AesEncryptor_DifferentKeys_ShouldProduceDifferentResults()
    {
        var encryptor1 = new AesEncryptor("key1");
        var encryptor2 = new AesEncryptor("key2");
        var input = "Test String";
        
        Assert.That(encryptor1.Encrypt(input), Is.Not.EqualTo(encryptor2.Encrypt(input)));
    }

    [Test]
    public void AesEncryptor_PerformanceTest()
    {
        var encryptor = new AesEncryptor("performance-test-key");
        foreach (var data in TestData)
        {
            MeasurePerformance(encryptor, data, "AES Encryption");
        }
    }
}
