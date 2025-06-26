namespace DEF.Gateway;

public interface ITickBase
{
    Task StartAsync();

    Task Update(float tm);

    Task StopAsync();
}

public class TickMgr
{
    ITickBase Kcp { get; set; }

    public void UseKcp(ITickBase tick)
    {
        Kcp = tick;
    }

    public Task StartAsync()
    {
        if (Kcp != null)
        {
            return Kcp.StartAsync();
        }

        return Task.CompletedTask;
    }

    public Task Update(float tm)
    {
        if (Kcp != null)
        {
            return Kcp.Update(tm);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        if (Kcp != null)
        {
            return Kcp.StopAsync();
        }

        return Task.CompletedTask;
    }
}