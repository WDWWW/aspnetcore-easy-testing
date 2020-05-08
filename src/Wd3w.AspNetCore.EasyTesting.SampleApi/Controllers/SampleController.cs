using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Models;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;

namespace Wd3w.AspNetCore.EasyTesting.SampleApi.Controllers
{
    [ApiController]
    [Route("api/sample")]
    public class SampleController
    {
        private readonly ILogger<SampleController> _logger;
        private readonly ISampleService _service;

        public SampleController(ILogger<SampleController> logger, ISampleService service)
        {
            _logger = logger;
            _service = service;
        }


        [HttpGet("data")]
        public ActionResult<SampleDataResponse> GetSampleData()
        {
            _logger.LogInformation("Call Get SampleData API!");
            return new SampleDataResponse
            {
                Data = _service.GetSampleDate()
            };
        }
    }
}