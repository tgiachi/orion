namespace Orion.Irc.Core.Data.Messages;

/// <summary>
/// Provides standard server notice messages used during IRC session lifecycle
/// </summary>
public static class ServerNotices
{
    /// <summary>
    /// Messages related to hostname lookups and connection
    /// </summary>
    public static class Connection
    {
        public static string LookingUpHostname => "*** Looking up your hostname...";
        public static string HostnameLookupFailed => "*** Couldn't resolve your hostname; using your IP address instead";
        public static string CheckingForClones => "*** Checking Ident";
        public static string ConnectingToHost => "*** Found your hostname";

        public static string ConnectionEstablished(string host) => $"*** Connected to {host}";
        // Add other connection-related messages as needed
    }

    /// <summary>
    /// Messages related to authentication and registration
    /// </summary>
    public static class Authentication
    {
        public static string AwaitingRegistration => "*** Please wait while we process your connection";

        public static string RegistrationComplete => "*** Registration completed";
        // Add other authentication-related messages as needed
    }


}
