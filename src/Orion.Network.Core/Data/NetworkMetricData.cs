namespace Orion.Network.Core.Data;

public class NetworkMetricData
{
    public string SessionId { get; set; }

    public long BytesIn { get; set; }

    public long BytesOut { get; set; }

    public long PacketsIn { get; set; }

    public long PacketsOut { get; set; }

    public void AddBytesIn(long bytes)
    {
        BytesIn += bytes;
    }

    public void AddBytesOut(long bytes)
    {
        BytesOut += bytes;
    }

    public void AddPacketsIn(long packets = 1)
    {
        PacketsIn += packets;
    }

    public void AddPacketsOut(long packets = 1)
    {
        PacketsOut += packets;
    }

    public override string ToString()
    {
        return
            $"SessionId: {SessionId}, BytesIn: {BytesIn}, BytesOut: {BytesOut}, PacketsIn: {PacketsIn}, PacketsOut: {PacketsOut}";
    }
}
