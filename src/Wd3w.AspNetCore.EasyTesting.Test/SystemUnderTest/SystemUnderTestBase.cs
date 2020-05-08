using System;
using Wd3w.AspNetCore.EasyTesting.SampleApi;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public abstract class SystemUnderTestBase : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        protected readonly SystemUnderTest<Startup> SUT;

        protected SystemUnderTestBase()
        {
            SUT = new SystemUnderTest<Startup>();
        }

        public void Dispose()
        {
            SUT?.Dispose();
        }
    }
}