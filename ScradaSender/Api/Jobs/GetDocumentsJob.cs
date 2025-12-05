using Microsoft.Extensions.Logging;
using ScradaSender.Agents.Interfaces;
using ScradaSender.Agents.Models;
using ScradaSender.Api.Jobs.Interface;
using static ScradaSender.Shared.Enums.Enums;

namespace ScradaSender.Api.Jobs
{
    public class GetDocumentsJob(
        IScradaServiceAgent scradaAgent,
        ILogger<GetDocumentsJob> logger) : IGetDocumentsJob
    {
        public async Task GetDocumentsAsync()
        {
            var response = await scradaAgent.GetDocumentsAsync<PeppolDocumentsResponse>();
        }
    }
}
