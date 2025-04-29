namespace Orion.Core.Server.Interfaces.Sessions;

/// <summary>
///  Represents a network session.
/// </summary>
public interface INetworkSession : IDisposable
{
    /// <summary>
    ///  Gets the unique identifier for the session.
    /// </summary>
    string Id { get; set; }


    /// <summary>
    ///  Initializes the session every time it is created by object pool.
    /// </summary>
    void Initialize();
}
