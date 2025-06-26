using Orleans.Streams;

namespace DEF;

public abstract class ContainerStatefulStreamSub
{
}

public class ContainerStatefulStreamSub<T> : ContainerStatefulStreamSub
{
    StreamSubscriptionHandle<T> StreamSub { get; set; }

    public async Task SubAsync(ContainerStateful container, string name_space, string stream_guid, Func<T, StreamSequenceToken, Task> on_next_async)
    {
        var stream_provider = container.GetStreamProvider2();
        var async_stream = stream_provider.GetStream<T>(stream_guid);

        StreamSub = await async_stream.SubscribeAsync(on_next_async);
    }

    public async Task SubAsync(ContainerStatefulNoReentrant container, string name_space, string stream_guid, Func<T, StreamSequenceToken, Task> on_next_async)
    {
        var stream_provider = container.GetStreamProvider2();
        var async_stream = stream_provider.GetStream<T>(stream_guid);

        StreamSub = await async_stream.SubscribeAsync(on_next_async);
    }

    public async Task UnsubAsync()
    {
        if (StreamSub != null)
        {
            await StreamSub.UnsubscribeAsync();

            StreamSub = null;
        }
    }
}