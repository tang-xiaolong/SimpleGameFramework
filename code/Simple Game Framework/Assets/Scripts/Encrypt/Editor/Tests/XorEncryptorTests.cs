using NUnit.Framework;

[TestFixture]
public class XorEncryptorTests : EncryptorTestBase
{
    [Test]
    public void XorEncryptor_SameKey_ShouldProduceSameResult()
    {
        var encryptor1 = new XorEncryptor(0x55);
        var encryptor2 = new XorEncryptor(0x55);
        var input = "Test String";
        
        Assert.That(encryptor1.Encrypt(input), Is.EqualTo(encryptor2.Encrypt(input)));
    }

    [TestCaseSource(nameof(XorKeys))]
    public void XorEncryptor_DifferentKeys_BasicTest(byte key)
    {
        var encryptor = new XorEncryptor(key);
        VerifyEncryption(encryptor, "Test String");
    }

    [TestCaseSource(nameof(TestData))]
    public void XorEncryptor_DifferentInputs_WithDefaultKey(string input)
    {
        var encryptor = new XorEncryptor(0x55);
        VerifyEncryption(encryptor, input);
    }

    [Test]
    public void XorEncryptor_PerformanceTest()
    {
        var encryptor = new XorEncryptor(0x55);
        foreach (var data in TestData)
        {
            MeasurePerformance(encryptor, data, "XOR Encryption");
        }
    }
}