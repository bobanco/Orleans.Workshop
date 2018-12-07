using Azro.Core.Services.Api.Utils;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Azro.Core.Services.Impl.Utils
{
    public class GrainDeactivator : IGrainDeactivator
    {
        private readonly IGrainActivationContext context;
        private readonly IGrainRuntime runtime;

        public GrainDeactivator(IGrainActivationContext context, IGrainRuntime runtime)
        {
            this.context = context;
            this.runtime = runtime;
        }

        public Task Deactivate()
        {
            this.runtime.DeactivateOnIdle(this.context.GrainInstance);
            return Task.CompletedTask;
        }
    }
}
