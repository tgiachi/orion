using Orion.Core.Server.Data.Channels;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc.Channels;
using Orion.Core.Server.Exceptions.Channels;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Data.Channels;
using Orion.Irc.Core.Filters;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Irc.Core.Types;

namespace Orion.Server.Services.Irc;

public class ChannelManagerService : IChannelManagerService
{
    private readonly ILogger _logger;

    private readonly IEventBusService _eventBusService;

    private readonly Dictionary<string, ChannelData> _channels = new(StringComparer.OrdinalIgnoreCase);

    private readonly IIrcSessionService _sessionService;

    private readonly IrcServerContextData _serverContextData;

    public ChannelManagerService(
        ILogger<ChannelManagerService> logger, IEventBusService eventBusService, IrcServerContextData serverContextData,
        IIrcSessionService sessionService
    )
    {
        _logger = logger;
        _eventBusService = eventBusService;
        _serverContextData = serverContextData;
        _sessionService = sessionService;
    }

    public bool ChannelExists(string channelName)
    {
        if (string.IsNullOrWhiteSpace(channelName))
        {
            throw new ArgumentException("Channel name cannot be null or empty.", nameof(channelName));
        }

        return _channels.ContainsKey(channelName);
    }

    public ChannelData? GetChannel(string channelName)
    {
        if (string.IsNullOrWhiteSpace(channelName))
        {
            throw new ArgumentException("Channel name cannot be null or empty.", nameof(channelName));
        }

        _channels.TryGetValue(channelName, out var channelData);
        return channelData;
    }


    public async Task<ChannelJoinResult> JoinChannelAsync(
        IrcUserSession session, string channelName, string? channelPassword = null
    )
    {
        var result = new ChannelJoinResult();

        if (!ChannelExists(channelName))
        {
            await CreateChannelAsync(channelName, session);
        }

        var channelData = GetChannel(channelName);

        if (channelData.HasKey && channelData.Key != channelPassword)
        {
            result.Exception = new ChannelInvalidPasswordException(channelName);

            result.AddJoinedUserCommand(
                ErrBadChannelKey.Create(_serverContextData.ServerName, session.NickName, channelData.Key)
            );

            return result;
        }

        if (channelData.UserLimit.HasValue && channelData.MemberCount >= channelData.UserLimit)
        {
            result.Exception = new ChannelFullException(channelName);
            return result;
        }


        channelData.AddMember(session.NickName);

        if (channelData.MemberCount == 1)
        {
            // If the channel is empty, add the user as operator
            channelData.SetOperator(session.NickName, true);
        }


        foreach (var member in channelData.GetMemberList())
        {
            result.AddMemberCommand(member, JoinCommand.CreateForChannels(session.FullAddress, channelName));
            result.AddMemberCommand(member, (await GetNamesAsync(member, channelName)).ToArray());

        }


        result.AddJoinedUserCommand(
            RplChannelModeIs.Create(
                _serverContextData.ServerName,
                session.NickName,
                channelData.Name,
                channelData.GetModeString()
            )
        );

        result.AddJoinedUserCommand(
            RplCreationTime.Create(
                _serverContextData.ServerName,
                session.NickName,
                channelData.Name,
                channelData.CreationTime
            )
        );

        result.AddJoinedUserCommand((await GetTopicsAsync(session.NickName, channelName)).ToArray());
        result.AddJoinedUserCommand((await GetNamesAsync(session.NickName, channelName)).ToArray());


        var membership = channelData.GetMembership(session.NickName);

        if (membership.IsOperator)
        {
            result.AddJoinedUserCommand(
                ModeCommand.CreateWithModes(
                    _serverContextData.ServerName,
                    channelName,
                    new[]
                    {
                        new ModeChangeType(true, 'o', session.NickName)
                    }
                )
            );

        }


        ///TODO: Add ban list


        result.IsSuccess = true;
        result.ChannelData = channelData;

        return result;
    }


    public Task<List<ChannelTopicEntry>> GetChannelTopicsAsync(bool hidePrivateChannels = true)
    {
        var channelTopics = new List<ChannelTopicEntry>();

        foreach (var channel in _channels.Values)
        {
            if (hidePrivateChannels && channel.IsPrivate)
            {
                continue;
            }

            channelTopics.Add(new ChannelTopicEntry(channel.Name, channel.MemberCount, channel.Topic));
        }

        return Task.FromResult(channelTopics);
    }

    public bool UserInChannel(IrcUserSession session, string channelName)
    {
        if (string.IsNullOrWhiteSpace(channelName))
        {
            throw new ArgumentException("Channel name cannot be null or empty.", nameof(channelName));
        }

        return _channels.TryGetValue(channelName, out var channelData) && channelData.IsMember(session.NickName);
    }

    public List<string> GetUsersInChannel(string channelName)
    {
        if (!ChannelExists(channelName))
        {
            return [];
        }

        var channelData = GetChannel(channelName);

        return channelData.GetMemberList().ToList();
    }

    public Task<bool> PartChannel(string nickName, string channelName, string? partMessage = null)
    {
        var session = _sessionService.FindByNickName(nickName);
        return session == null ? Task.FromResult(false) : PartChannel(session, channelName, partMessage);
    }

    public async Task<bool> SetTopic(IrcUserSession session, string channelName, string topicName)
    {
        if (!ChannelExists(channelName))
        {
            session.SendCommandAsync(
                ErrNoSuchChannel.Create(_serverContextData.ServerName, session.NickName, channelName)
            );

            return false;
        }

        var channelData = GetChannel(channelName);

        if (!channelData.IsMember(session.NickName))
        {
            session.SendCommandAsync(
                ErrNotOnChannel.Create(_serverContextData.ServerName, session.NickName, channelName)
            );

            return false;
        }

        if (!channelData.IsOperator(session.NickName) && !session.IsOperator)
        {
            session.SendCommandAsync(
                ErrChanOpPrivsNeeded.Create(_serverContextData.ServerName, session.NickName, channelName)
            );

            return false;
        }

        channelData.SetTopic(session.FullAddress, topicName);

        var topicCommand = RplTopic.Create(
            _serverContextData.ServerName,
            session.NickName,
            channelName,
            topicName
        );

        foreach (var member in channelData.GetMemberList())
        {
            var memberSession = _sessionService.FindByNickName(member);

            await memberSession.SendCommandAsync(topicCommand);
        }

        return true;
    }

    public Task<bool> SetTopic(string nickName, string channelName, string topicName)
    {
        var session = _sessionService.FindByNickName(nickName);
        return session == null ? Task.FromResult(false) : SetTopic(session, channelName, topicName);
    }

    public async Task<List<string>> GetChannelsForNickNameAsync(string nickName)
    {
        var channels = (from channel in _channels.Values where channel.IsMember(nickName) select channel.Name).ToList();

        return channels;
    }

    public async Task<List<IIrcCommand>> GetNamesAsync(string nickName, string channelName)
    {
        var session = _sessionService.FindByNickName(nickName);

        if (session == null)
        {
            return [];
        }


        if (!ChannelExists(channelName))
        {
            await session.SendCommandAsync(
                ErrNoSuchChannel.Create(_serverContextData.ServerName, session.NickName, channelName)
            );

            return [];
        }

        var channelData = GetChannel(channelName);
        var commandResult = new List<IIrcCommand>();

        foreach (var namesChunk in channelData.GetPrefixedMemberList().Chunk(5))
        {
            commandResult.Add(
                RplNameReply.Create(_serverContextData.ServerName, session.NickName, channelData.Name, namesChunk)
            );
        }

        commandResult.Add(RplEndOfNames.Create(_serverContextData.ServerName, session.NickName, channelData.Name));


        return commandResult;
    }

    public async Task<List<IIrcCommand>> GetTopicsAsync(string nickName, string channelName)
    {
        var session = _sessionService.FindByNickName(nickName);

        if (session == null)
        {
            return [];
        }

        if (!ChannelExists(channelName))
        {
            await session.SendCommandAsync(
                ErrNoSuchChannel.Create(_serverContextData.ServerName, session.NickName, channelName)
            );

            return [];
        }

        var channelData = GetChannel(channelName);

        var commandResult = new List<IIrcCommand>();

        if (channelData.HaveTopic)
        {
            commandResult.Add(
                RplTopic.Create(_serverContextData.ServerName, session.NickName, channelData.Name, channelData.Topic)
            );

            commandResult.Add(
                RplTopicWhoTime.Create(
                    _serverContextData.ServerName,
                    nickName,
                    channelData.Name,
                    channelData.TopicSetBy,
                    channelData.TopicSetTime
                )
            );
        }
        else
        {
            commandResult.Add(RplNoTopic.Create(_serverContextData.ServerName, session.NickName, channelData.Name));
        }

        return commandResult;
    }

    public async Task<List<IIrcCommand>> ListChannelsAsync(string nickName, string[]? channels = null, string? query = null)
    {
        var filter = new ListQueryFilter();


        if (query != null)
        {
            filter = ListQueryFilter.Parse(query);
        }

        var channResult = _channels.Values.Where(s => filter.Matches(
                    s.MemberCount,
                    (int)(DateTime.Now - s.CreationTime).TotalMinutes,
                    (int)(DateTime.Now - s.TopicSetTime).TotalMinutes
                )
            )
            .ToList();


        var result = new List<IIrcCommand>();


        result.Add(RplListStart.Create(_serverContextData.ServerName, nickName));

        foreach (var channel in channResult)
        {
            result.Add(RplList.Create(
                _serverContextData.ServerName,
                nickName,
                channel.Name,
                channel.MemberCount,
                channel.Topic ?? null
            ));
        }

        result.Add(RplListEnd.Create(_serverContextData.ServerName, nickName));


        return result;
    }

    public async Task<bool> PartChannel(IrcUserSession session, string channelName, string? partMessage = null)
    {
        if (!ChannelExists(channelName))
        {
            return false;
        }

        var channelData = GetChannel(channelName);

        if (!channelData.IsMember(session.NickName))
        {
            return false;
        }

        channelData.RemoveMember(session.NickName);


        if (channelData.MemberCount == 0)
        {
            _channels.Remove(channelName);
            await _eventBusService.PublishAsync(new ChannelDeletedEvent(channelName, session.NickName));
        }
        else
        {
            var partCommand = PartCommand.CreateForChannel(session.FullAddress, channelName, partMessage);

            await session.SendCommandAsync(partCommand);

            foreach (var member in channelData.GetMemberList())
            {
                var memberSession = _sessionService.FindByNickName(member);
                if (memberSession != null && memberSession != session)
                {
                    memberSession.SendCommandAsync(partCommand);
                }
            }
        }


        return true;
    }


    private async Task<ChannelData> CreateChannelAsync(string channelName, IrcUserSession createdBy)
    {
        var channelData = new ChannelData(channelName)
        {
            Founder = createdBy.FullAddress
        };

        _channels.Add(channelName, channelData);


        channelData.SetTopicProtection();

        _logger.LogDebug("Channel {ChannelName} created.", channelName);

        await _eventBusService.PublishAsync(new ChannelCreatedEvent(channelName, createdBy.NickName));

        return channelData;
    }
}
