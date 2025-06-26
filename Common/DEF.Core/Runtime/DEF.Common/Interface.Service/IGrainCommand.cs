#if !DEF_CLIENT

using Orleans;
using System.Threading.Tasks;

namespace DEF;

public interface IGrainCommand : IGrainWithStringKey
{
    Task Command(string cmd, string[] args);
}

#endif
