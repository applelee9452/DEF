using DEF;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.CCenter;

[ProtoContract]
[Serializable]
[GenerateSerializer]
public class ImportExportCfg
{
    [ProtoMember(1)]
    [Id(0)]
    public string NameSpace { get; set; }

    [ProtoMember(2)]
    [Id(1)]
    public string Desc { get; set; }

    [ProtoMember(3)]
    [Id(2)]
    public Dictionary<string, string> MapCfg { get; set; }
}

[ProtoContract]
[Serializable]
[GenerateSerializer]
public class ImportExportCfgs
{
    [ProtoMember(1)]
    [Id(0)]
    public List<ImportExportCfg> List { get; set; }
}

[ContainerRpc("DEF.CCenter", "Manager", ContainerStateType.Stateless)]
public interface IContainerManager : IContainerRpc
{
    Task<List<DataNameSpace>> GetAllNameSpace();

    Task<DataNameSpace> AddNameSpace(string name_space, string desc);

    Task<bool> RemoveNameSpace(string _id);

    Task<DataNameSpace> UpdateNameSpace(string _id, string name_space_new, string desc_new);

    Task<List<DataCfg>> GetAllCfg();

    Task<DataCfg> AddCfgItem(string _id, string key, string value);

    Task<DataCfg> RemoveCfgItem(string _id, string key);

    Task<DataCfg> UpdateCfgItem(string _id, string key, string value);

    Task<ImportExportCfgs> ExportAllCfg();

    Task ImportAllCfg(ImportExportCfgs list);
}