using Hestify;

namespace Wd3w.AspNetCore.EasyTesting.Hestify
{
    public static class HestifyExtensionHelper
    {
        public static HestifyClient Resource(this SystemUnderTestBase sut, string relativeUri)
        {
            return new HestifyClient(sut.CreateClient(), relativeUri);
        }
    }
}