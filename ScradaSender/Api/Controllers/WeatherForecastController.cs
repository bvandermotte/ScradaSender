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
            return Ok(await scradaAgent.CheckIfCompanyExistAsync<ScradaParticipant>("0432106690"));
        }
    }
}
