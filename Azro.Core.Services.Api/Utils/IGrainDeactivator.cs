using Orleans.Runtime;
using System.Threading.Tasks;

namespace Azro.Core.Services.Api.Utils
{
    public interface IGrainDeactivator : IGrainExtension
    {
        Task Deactivate();
    }
}
