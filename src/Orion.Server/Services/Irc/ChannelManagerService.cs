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

namespace Orion.Server.Services.Irc;

public class ChannelManagerService : IChannelManagerService
{
    private readonly ILogger _logger;

    private readonly IEventBusService _eventBusService;

    private readonly Dictionary<string, ChannelData> _channels = new(StringComparer.OrdinalIgnoreCase);

    private readonly IrcServerContextData _serverContextData;

    public ChannelManagerService(
        ILogger<ChannelManagerService> logger, IEventBusService eventBusService, IrcServerContextData serverContextData
    )
    {
        _logger = logger;
        _eventBusService = eventBusService;
        _serverContextData = serverContextData;
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

        if (channelData.MemberCount == 0)
        {
            // If the channel is empty, add the user as operator
            channelData.SetOperator(session.NickName, true);
        }


        result.AddJoinedUserCommand(
            RplNameReply.Create(
                _serverContextData.ServerName,
                session.NickName,
                channelData.Name,
                channelData.GetPrefixedMemberList()
            )
        );


        result.AddJoinedUserCommand(
            RplEndOfNames.Create(_serverContextData.ServerName, session.NickName, channelData.Name)
        );

        result.AddJoinedUserCommand(
            RplCreationTime.Create(
                _serverContextData.ServerName,
                session.NickName,
                channelData.Name,
                channelData.CreationTime
            )
        );

        if (channelData.HaveTopic)
        {
            result.AddJoinedUserCommand(
                RplTopic.Create(_serverContextData.ServerName, session.NickName, channelData.Name, channelData.Topic)
            );
        }
        else
        {
            result.AddJoinedUserCommand(
                RplNoTopic.Create(_serverContextData.ServerName, session.NickName, channelData.Name)
            );
        }


        result.AddJoinedUserCommand(
            RplChannelModeIs.Create(
                _serverContextData.ServerName,
                session.NickName,
                channelData.Name,
                channelData.GetModeString()
            )
        );

        ///TODO: Add ban list

        foreach (var member in channelData.GetMemberList())
        {
            result.AddMemberCommand(member, JoinCommand.CreateForChannels(session.FullAddress, channelName));
        }


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


    private async Task<ChannelData> CreateChannelAsync(string channelName, IrcUserSession createdBy)
    {
        var channelData = new ChannelData(channelName)
        {
            Founder = createdBy.FullAddress
        };

        _channels.Add(channelName, channelData);

        _logger.LogDebug("Channel {ChannelName} created.", channelName);

        await _eventBusService.PublishAsync(new ChannelCreatedEvent(channelName, createdBy.NickName));

        return channelData;
    }
}
