using System.Threading.Tasks;

namespace DEF
{
    public interface IRpcInfo
    {
        bool IsUnity { get; set; }
        string SourceServiceName { get; set; }
        string TargetServiceName { get; set; }
    }
}