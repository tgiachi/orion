using Moq;
using NUnit.Framework;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Network.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Orion.Tests.Sessions;

[TestFixture]
public class IrcUserSessionTests : IDisposable
{
    private IrcUserSession _session;
    private Mock<IIrcCommandService> _mockCommandService;
    private Mock<INetworkTransportManager> _mockTransportManager;



    [SetUp]
    public void Setup()
    {
        _mockCommandService = new Mock<IIrcCommandService>();
        _mockTransportManager = new Mock<INetworkTransportManager>();

        _session = new IrcUserSession();
        _session.SetCommandService(_mockCommandService.Object);
        _session.SetNetworkTransportManager(_mockTransportManager.Object);
    }

    [Test]
    public void Initialize_ShouldResetAllProperties()
    {
        // Arrange
        _session.NickName = "TestNick";
        _session.UserName = "TestUser";
        _session.RealName = "Test User";
        _session.HostName = "test.host";
        _session.Endpoint = "192.168.1.1:12345";
        _session.VHostName = "vhost.example.com";
        _session.IsPasswordValid = true;
        _session.ApplyModeChanges("+iw");
        _session.SetAway("Gone fishing");

        // Act
        _session.Initialize();

        // Assert
        Assert.That(_session.NickName, Is.EqualTo(string.Empty));
        Assert.That(_session.UserName, Is.EqualTo(string.Empty));
        Assert.That(_session.RealName, Is.EqualTo(string.Empty));
        Assert.That(_session.HostName, Is.EqualTo(string.Empty));
        Assert.That(_session.RemoteAddress, Is.EqualTo(string.Empty));
        Assert.That(_session.RemotePort, Is.EqualTo(0));
        Assert.That(_session.VHostName, Is.Null);
        Assert.That(_session.IsPasswordValid, Is.False);
        Assert.That(_session.IsAway, Is.False);
        Assert.That(_session.ModesString, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ApplyModeChanges_ShouldAddAndRemoveModes()
    {
        // Act
        _session.ApplyModeChanges("+iwo");

        // Assert
        Assert.That(_session.HasMode('i'), Is.True);
        Assert.That(_session.HasMode('w'), Is.True);
        Assert.That(_session.HasMode('o'), Is.True);
        Assert.That(_session.IsInvisible, Is.True);
        Assert.That(_session.IsOperator, Is.True);
        Assert.That(_session.ReceivesWallops, Is.True);

        // Act
        _session.ApplyModeChanges("-iw");

        // Assert
        Assert.That(_session.HasMode('i'), Is.False);
        Assert.That(_session.HasMode('w'), Is.False);
        Assert.That(_session.HasMode('o'), Is.True); // 'o' mode should still be set
        Assert.That(_session.IsInvisible, Is.False);
        Assert.That(_session.IsOperator, Is.True);
        Assert.That(_session.ReceivesWallops, Is.False);
    }

    [Test]
    public void SetAway_ShouldSetAwayStatusAndMessage()
    {
        // Act
        _session.SetAway("Gone fishing");

        // Assert
        Assert.That(_session.IsAway, Is.True);
        Assert.That(_session.AwayMessage, Is.EqualTo("Gone fishing"));
    }

    [Test]
    public void SetBack_ShouldClearAwayStatusAndMessage()
    {
        // Arrange
        _session.SetAway("Gone fishing");

        // Act
        _session.SetBack();

        // Assert
        Assert.That(_session.IsAway, Is.False);
        Assert.That(_session.AwayMessage, Is.Null);
    }

    [Test]
    public void FullAddress_ShouldUseVHostWhenAvailable()
    {
        // Arrange
        _session.NickName = "nick";
        _session.UserName = "user";
        _session.HostName = "host.example.com";
        _session.VHostName = "vhost.example.com";

        // Act & Assert
        Assert.That(_session.FullAddress, Is.EqualTo("nick!user@vhost.example.com"));
    }

    [Test]
    public void FullAddress_ShouldUseHostNameWhenVHostNotAvailable()
    {
        // Arrange
        _session.NickName = "nick";
        _session.UserName = "user";
        _session.HostName = "host.example.com";
        _session.VHostName = null;

        // Act & Assert
        Assert.That(_session.FullAddress, Is.EqualTo("nick!user@host.example.com"));
    }


    [Test]
    public void Endpoint_ShouldSetAddressAndPort()
    {
        // Act
        _session.Endpoint = "192.168.1.1:12345";

        // Assert
        Assert.That(_session.RemoteAddress, Is.EqualTo("192.168.1.1"));
        Assert.That(_session.RemotePort, Is.EqualTo(12345));
    }

    [Test]
    public void Endpoint_ShouldThrowWhenInvalidFormat()
    {
        // Act & Assert
        Assert.That(() => _session.Endpoint = "invalid_endpoint",
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public async Task SendCommandAsync_ShouldCallCommandService()
    {
        // Arrange
        var command1 = new Mock<IIrcCommand>().Object;
        var command2 = new Mock<IIrcCommand>().Object;
        _session.SessionId = "test_session";

        // Act
        await _session.SendCommandAsync(command1, command2);

        // Assert
        _mockCommandService.Verify(s =>
            s.SendCommandAsync("test_session", command1), Times.Once);
        _mockCommandService.Verify(s =>
            s.SendCommandAsync("test_session", command2), Times.Once);
    }

    [Test]
    public async Task DisconnectAsync_ShouldCallTransportManager()
    {
        // Arrange
        _session.SessionId = "test_session";

        // Act
        await _session.DisconnectAsync();

        // Assert
        _mockTransportManager.Verify(m =>
            m.DisconnectAsync("test_session"), Times.Once);
    }

    [Test]
    public void IsAuthenticated_ShouldBeTrueWhenUserAndNickSent()
    {
        // Arrange
        _session.NickName = "nick";
        _session.UserName = "user";

        // Act & Assert
        Assert.That(_session.IsAuthenticated, Is.True);
    }

    [Test]
    public void IsAuthenticated_ShouldBeFalseWhenUserNotSent()
    {
        // Arrange
        _session.NickName = "nick";
        _session.UserName = "";

        // Act & Assert
        Assert.That(_session.IsAuthenticated, Is.False);
    }

    [Test]
    public void IsAuthenticated_ShouldBeFalseWhenNickNotSent()
    {
        // Arrange
        _session.NickName = "";
        _session.UserName = "user";

        // Act & Assert
        Assert.That(_session.IsAuthenticated, Is.False);
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}

[TestFixture]
public class MotdCommandsTests
{
    [Test]
    public void RplMotdStart_Create_ShouldSetAllProperties()
    {
        // Act
        var cmd = RplMotdStart.Create("irc.server.com", "nick");

        // Assert
        Assert.That(cmd.ServerName, Is.EqualTo("irc.server.com"));
        Assert.That(cmd.Nickname, Is.EqualTo("nick"));

    }





    [Test]
    public void RplEndOfMotd_Create_ShouldSetAllProperties()
    {
        // Act
        var cmd = RplEndOfMotd.Create("irc.server.com", "nick");

        // Assert
        Assert.That(cmd.ServerName, Is.EqualTo("irc.server.com"));
        Assert.That(cmd.Nickname, Is.EqualTo("nick"));
    }

    [Test]
    public void RplEndOfMotd_Write_ShouldFormatCorrectly()
    {
        // Arrange
        var cmd = new RplEndOfMotd
        {
            ServerName = "irc.server.com",
            Nickname = "nick",
        };

        // Act
        string result = cmd.Write();

        // Assert
        Assert.That(result, Is.EqualTo(":irc.server.com 376 nick :End of /MOTD command."));
    }




}
