using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using ScradaSender.Agents.Interfaces;
using ScradaSender.Agents.Models;
using ScradaSender.Options;

namespace ScradaSender.Agents
{
    public class ScradaServiceAgent(
        IOptions<ScradaSettings> scradaOptions,
        HttpClient httpClient, 
        ILogger<ScradaServiceAgent> logger) : IScradaServiceAgent
    {
        readonly ScradaSettings scradaSettings = scradaOptions.Value;

        public async Task<ScradaParticipant> CheckIfCompanyExist(string peppolIdentifierValue)
        {
            try
            {
                var result = await httpClient.GetAsync(
                    $"/v1/company/{scradaSettings.CompanyId}/" +
                    $"peppol/lookup/{scradaSettings.PeppolIdentifierScheme}/{scradaSettings.PeppolIdentifierTypeBE}:" +
                    $"{peppolIdentifierValue}");

                return await result.Content.ReadFromJsonAsync<ScradaParticipant>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not reach Scrada.");
                throw;
            }
        }

        public async Task<Guid> SendDocument(XDocument invoice)
        {
            try
            {
                var xmlString = invoice.Declaration != null
                    ? invoice.Declaration.ToString() + Environment.NewLine + invoice.ToString()
                    : invoice.ToString();

                var content = new StringContent(xmlString, Encoding.UTF8, "application/xml");
                var result = await httpClient.PostAsync($"/v1/company/{scradaSettings.CompanyId}/peppol/outbound/document", content);

                if(!result.IsSuccessStatusCode)
                {
                    logger.LogError("Error while sending the document to Peppol. Status: {status}. Content: {content}", 
                        result.StatusCode,
                        result.Content.ReadAsStringAsync());

                    throw new Exception("Document could not be sent.");
                }

                return new Guid(await result.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not reach Scrada.");
                throw;
            }
        }

        public async Task<OutboundDocumentStatus> CheckDocumentStatus(string outboundDocumentId)
        {
            try
            {
                var result = httpClient.GetAsync(
                    $"/v1/company/{scradaSettings.CompanyId}/" +
                    $"/peppol/outbound/document/{outboundDocumentId}/info");

                return await result.Result.Content.ReadFromJsonAsync<OutboundDocumentStatus>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not reach Scrada.");
                throw;
            }
        }
    }
}
