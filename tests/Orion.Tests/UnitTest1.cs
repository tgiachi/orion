using System.Diagnostics;
using System.Text;
using Orion.Network.Core.Parsers;

namespace Orion.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ParseLargeRandomMessageBuffer_PerformanceTest()
    {
        const int messageCount = 1_000_000;
        var random = new Random();
        var sb = new StringBuilder();

        string[] newlines = ["\n", "\r", "\r\n"];

        for (int i = 0; i < messageCount; i++)
        {
            sb.Append($"msg_{i}");
            sb.Append(newlines[random.Next(0, newlines.Length)]);
        }

        var buffer = Encoding.UTF8.GetBytes(sb.ToString());
        var memory = new ReadOnlyMemory<byte>(buffer);

        var sw = Stopwatch.StartNew();
        var result = NewLineMessageParser.FastParseMessages(memory);
        sw.Stop();

        Console.WriteLine($"Parsed {result.Count} messages in {sw.ElapsedMilliseconds} ms");


        Assert.That(result.Count, Is.EqualTo(messageCount));
    }

    [Test]
    public void ParseLargeRandomMessageBuffer_RegexPerformanceTest()
    {
        const int messageCount = 1_000_000;
        var random = new Random();
        var sb = new StringBuilder();

        string[] newlines = ["\n", "\r", "\r\n"];

        for (int i = 0; i < messageCount; i++)
        {
            sb.Append($"msg_{i}");
            sb.Append(newlines[random.Next(0, newlines.Length)]);
        }

        var buffer = Encoding.UTF8.GetBytes(sb.ToString());
        var memory = new ReadOnlyMemory<byte>(buffer);

        var sw = Stopwatch.StartNew();
        var result = NewLineMessageParser.ParseMessages(memory);
        sw.Stop();

        Console.WriteLine($"Parsed {result.Length} messages in {sw.ElapsedMilliseconds} ms");


        Assert.That(result.Count, Is.EqualTo(messageCount));
    }
}
