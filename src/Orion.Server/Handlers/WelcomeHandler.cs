using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Events.Users;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.EventBus;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Irc.Core.Commands.Replies;

namespace Orion.Server.Handlers;

public class WelcomeHandler : BaseIrcCommandListener, IEventBusListener<UserAuthenticatedEvent>
{
    private readonly IVersionService _versionService;


    public WelcomeHandler(
        ILogger<WelcomeHandler> logger, IrcCommandListenerContext context, IVersionService versionService
    ) : base(logger, context)
    {
        _versionService = versionService;
        SubscribeToEventBus(this);
    }

    private RplISupport BuildISupport(string nickName)
    {
        var rplISupport = new RplISupport();

        var limits = Config.Irc.Limits;

        rplISupport.ServerName = ServerHostName;
        rplISupport.AwayLen = limits.AwayLength;
        rplISupport.CaseMapping = "rfc1459";
        rplISupport.ChannelLen = limits.ChanLength;
        rplISupport.Parameters["CHANMODES"] = limits.ChanModes;
        rplISupport.Network = ServerHostName;
        rplISupport.HostLen = limits.HostLength;
        rplISupport.UserLen = limits.UserLength;
        rplISupport.NickLen = limits.NickLength;
        rplISupport.TopicLen = limits.TopicLength;
        rplISupport.MaxTargets = limits.MaxTargets;

        rplISupport.Nickname = nickName;

        return rplISupport;
    }

    public async Task HandleAsync(UserAuthenticatedEvent @event, CancellationToken cancellationToken = default)
    {
        var session = GetSession(@event.SessionId);


        await session.SendCommandAsync(
            RplWelcome.Create(
                ServerHostName,
                session.NickName,
                ListenerContext.ServerContextData.NetworkName,
                session.FullAddress
            )
        );

        await session.SendCommandAsync(
            (RplYourHost.Create(
                ServerHostName,
                session.NickName,
                "orionirc-server " + _versionService.GetVersionInfo().Version
            ))
        );

        await session.SendCommandAsync(
            RplCreated.Create(ServerHostName, session.NickName, null, ListenerContext.ServerContextData.ServerStartTime)
        );


        await session.SendCommandAsync(
            RplMyInfo.Create(
                ServerHostName,
                Config.Irc.Limits.UserModes,
                Config.Irc.Limits.ChannelModes,
                session.NickName
            )
        );


        await session.SendCommandAsync(
            BuildISupport(session.NickName)
        );


        await session.SendCommandAsync(
            RplLuserClient.Create(
                ServerHostName,
                session.NickName,
                ListenerContext.SessionService.TotalSessions,
                ListenerContext.SessionService.TotalInvisibleSessions,
                1
            )
        );

        await session.SendCommandAsync(
            RplLuserOp.Create(ServerHostName, session.NickName, ListenerContext.SessionService.TotalOpers)
        );

        await session.SendCommandAsync(
            RplLocalUsers.Create(
                ServerHostName,
                session.NickName,
                ListenerContext.SessionService.TotalSessions,
                ListenerContext.SessionService.MaxSessions
            )
        );

        await session.SendCommandAsync(
            RplGlobalUsers.Create(
                ServerHostName,
                session.NickName,
                ListenerContext.SessionService.TotalSessions,
                ListenerContext.SessionService.MaxSessions
            )
        );

        await session.SendCommandAsync(
            RplLuserMe.Create(ServerHostName, session.NickName, ListenerContext.SessionService.TotalSessions, 1)
        );

        await session.SendCommandAsync(RplMotdStart.Create(ServerHostName, session.NickName));

        foreach (var line in await LoadMotdAsync())
        {
            await session.SendCommandAsync(RplMotd.Create(ServerHostName, session.NickName, line));
        }

        await session.SendCommandAsync(RplEndOfMotd.Create(ServerHostName, session.NickName));
    }

    private async Task<List<string>> LoadMotdAsync()
    {
        var motd = new List<string>();

        if (Config.Irc.Motd.StartsWith("file://"))
        {
            var filePath = Path.Combine(ListenerContext.AppContext.Directories.Root, Config.Irc.Motd[7..]);

            if (!File.Exists(filePath))
            {
                throw new Exception("Motd file not found");
            }

            using var reader = new StreamReader(filePath);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line != null)
                {
                    motd.Add(TranslateText(line));
                }
            }
        }

        return motd;
    }
}
