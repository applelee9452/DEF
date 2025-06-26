using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DEF;

public interface IServiceNodeBuilder
{
    IServiceCollection Services { get; }
}