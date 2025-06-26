namespace DEF.UCenter;

public class ContainerStatelessVCoinService : ContainerStateless, IContainerStatelessVCoinService
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }
}