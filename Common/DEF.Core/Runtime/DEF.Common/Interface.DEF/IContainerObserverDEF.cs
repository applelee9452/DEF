using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF
{
    [ContainerRpcObserver("DEF")]
    public interface IContainerObserverDEF : IContainerRpcObserver
    {
        Task SyncSceneSnapshot2Client(string scene_name, EntityData entity_data);

        Task SyncEntitySnapshot2Client(string scene_name, EntityData entity_data);

        Task SyncDelta2Client(string scene_name, byte[] data);
    }
}