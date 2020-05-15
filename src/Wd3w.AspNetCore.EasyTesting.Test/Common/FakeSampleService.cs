using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;

namespace Wd3w.AspNetCore.EasyTesting.Test.Common
{
    public class FakeSampleService : ISampleService
    {
        public string Message { get; set; } = "Fake!";

        public string GetSampleDate()
        {
            return Message;
        }
    }
}