using Orleans.Streams;

namespace DEF;

public abstract class ContainerStatefulStream
{
}

public class ContainerStatefulStream<T> : ContainerStatefulStream
{
    IAsyncStream<T> AsyncStream { get; set; }

    public ContainerStatefulStream(ContainerStateful container, string name_space, string stream_guid)
    {
        var stream_provider = container.GetStreamProvider2();
        AsyncStream = stream_provider.GetStream<T>(stream_guid);
    }

    public ContainerStatefulStream(ContainerStatefulNoReentrant container, string name_space, string stream_guid)
    {
        var stream_provider = container.GetStreamProvider2();
        AsyncStream = stream_provider.GetStream<T>(stream_guid);
    }

    public Task OnNextAsync(T obj)
    {
        return AsyncStream.OnNextAsync(obj);
    }

    public Task OnCompletedAsync()
    {
        return AsyncStream.OnCompletedAsync();
    }

    public Task OnErrorAsync(Exception ex)
    {
        return AsyncStream.OnErrorAsync(ex);
    }
}