using System;
using Wd3w.AspNetCore.EasyTesting.SampleApi;

namespace Wd3w.AspNetCore.EasyTesting.Test.Common
{
    public abstract class EasyTestingTestBase : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        protected readonly SystemUnderTest<Startup> SUT;

        protected EasyTestingTestBase()
        {
            SUT = new SystemUnderTest<Startup>();
        }

        public void Dispose()
        {
            SUT?.Dispose();
        }
    }
}