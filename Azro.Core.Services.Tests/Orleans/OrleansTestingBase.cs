using System;

namespace Azro.Core.Services.Tests.Orleans
{
    public abstract class OrleansTestingBase
    {
        protected static readonly Random Random = new Random();

        protected static long GetRandomGrainId()
        {
            return Random.Next();
        }
    }
}
