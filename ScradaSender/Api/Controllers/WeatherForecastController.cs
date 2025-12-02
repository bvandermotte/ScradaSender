using Microsoft.AspNetCore.Mvc;
using ScradaSender.Agents.Interfaces;

namespace ScradaSender.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(IScradaServiceAgent scradaAgent) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await scradaAgent.CheckIfCompanyExist("0432106690"));
        }
    }
}
