#if !DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "IMCDKeyMgr", ContainerStateType.Stateful)]
public interface IContainerStatefulIMCDKeyMgr : IContainerRpc
{
    // 保活
    Task Touch();

    Task<CDKey> CreateCDKey(CDKey cdkey);

    Task<List<CDKey>> GetCDKeyList();
    
    Task<bool> UpdateCDKeyOne(CDKey cdkey);

    Task<bool> DeleteCDKeyOne(string guid);

    Task<IMErrorCode> ExchangeCdkey(string player_guid, string code);
}

#endif