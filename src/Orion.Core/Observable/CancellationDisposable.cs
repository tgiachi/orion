namespace Orion.Core.Observable;

public class CancellationDisposable : IDisposable
{
    private readonly CancellationTokenSource _cts;
    public CancellationDisposable(CancellationTokenSource cts) => _cts = cts;
    public void Dispose() => _cts.Cancel();
}
