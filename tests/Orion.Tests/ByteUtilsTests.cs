using Orion.Foundations.Extensions;

namespace Orion.Tests;

[TestFixture]
public class ByteUtilsTests
{
    [Test]
    public void TestPrintHex()
    {
        var bytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var str = bytes.HumanizedContent();

        Assert.That(str, Is.EqualTo("[0x01, 0x02, 0x03, 0x04, 0x05]"));
    }
}
