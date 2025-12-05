using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using ScradaSender.Agents.Interfaces;
using ScradaSender.Agents.Models;
using ScradaSender.Agents.Models.Base;
using ScradaSender.Shared.Options;

namespace ScradaSender.Agents
{
    public class ScradaServiceAgent(
        IOptions<ScradaSettings> scradaOptions,
        HttpClient httpClient, 
        ILogger<ScradaServiceAgent> logger) : IScradaServiceAgent
    {
        readonly ScradaSettings scradaSettings = scradaOptions.Value;

        public async Task<Response<T>> CheckIfCompanyExistAsync<T>(string peppolIdentifierValue)
        {
            try
            {
                var result = await httpClient.GetAsync(
                    $"/v1/company/{scradaSettings.CompanyId}/" +
                    $"peppol/lookup/{scradaSettings.PeppolIdentifierScheme}/{scradaSettings.PeppolIdentifierTypeBE}:" +
                    $"{peppolIdentifierValue}");

                if (!result.IsSuccessStatusCode)
                {
                    var resultContent = await result.Content.ReadAsStringAsync();
                    return new Response<T>
                    {
                        Error = $"Error while sending the document to Peppol. Status: {result.StatusCode}. Content: {resultContent}"
                    };
                }

                return new Response<T>
                {
                    ResponseObject = await result.Content.ReadFromJsonAsync<T>()
                };
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    Error = $"Something wrong happened. Ex: {ex.Message}"
                };
            }
        }

        public async Task<Response<T>> SendDocumentAsync<T>(XDocument invoice, string supplierScheme, string supplierId, string customerScheme, string customerId, string extRef, string countryCode = "BE")
        {
            try
            {
                var xmlString = invoice.Declaration != null
                    ? invoice.Declaration.ToString() + Environment.NewLine + invoice.ToString()
                    : invoice.ToString();

                var content = new StringContent(xmlString, Encoding.UTF8, "application/xml");

                var request = new HttpRequestMessage(HttpMethod.Post, $"/v1/company/{scradaSettings.CompanyId}/peppol/outbound/document")
                {
                    Content = content
                };

                request.Headers.Add("x-scrada-peppol-sender-scheme", scradaSettings.PeppolIdentifierScheme);
                request.Headers.Add("x-scrada-peppol-sender-id", $"{supplierScheme}:{supplierId}");
                request.Headers.Add("x-scrada-peppol-receiver-Scheme", scradaSettings.PeppolIdentifierScheme);
                request.Headers.Add("x-scrada-peppol-receiver-id", $"{customerScheme}:{customerId}");
                request.Headers.Add("x-scrada-peppol-c1-country-code", countryCode);
                request.Headers.Add("x-scrada-peppol-document-type-scheme", "busdox-docid-qns");
                request.Headers.Add("x-scrada-peppol-document-type-value", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2::Invoice##urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0::2.1");
                request.Headers.Add("x-scrada-peppol-process-scheme", "cenbii-procid-ubl");
                request.Headers.Add("x-scrada-peppol-process-value", "urn:fdc:peppol.eu:2017:poacc:billing:01:1.0");
                request.Headers.Add("x-scrada-external-reference", extRef);

                var result = await httpClient.SendAsync(request);

                if (!result.IsSuccessStatusCode)
                {
                    var resultContent = await result.Content.ReadAsStringAsync();
                    return new Response<T>
                    {
                        Error = $"Error while sending the document to Peppol. Status: {result.StatusCode}. Content: {resultContent}"
                    };
                }

                return new Response<T>
                {
                    ResponseObject = await result.Content.ReadFromJsonAsync<T>()
                };
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    Error = $"Something wrong happened. Ex: {ex.Message}"
                };
            }
        }

        public async Task<Response<T>> CheckDocumentStatusAsync<T>(string outboundDocumentId)
        {
            try
            {
                var result = await httpClient.GetAsync(
                    $"/v1/company/{scradaSettings.CompanyId}" +
                    $"/peppol/outbound/document/{outboundDocumentId}/info");

                if (!result.IsSuccessStatusCode)
                {
                    var resultContent = await result.Content.ReadAsStringAsync();
                    return new Response<T>
                    {
                        Error = $"Error while getting status from Peppol. Status: {result.StatusCode}. Content: {resultContent}"
                    };
                }

                return new Response<T>
                {
                    ResponseObject = await result.Content.ReadFromJsonAsync<T>()
                };
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    Error = $"Could not reach Scrada. Ex: {ex.Message}"
                };
            }
        }

        public async Task<Response<T>> GetDocumentsAsync<T>()
        {
            try
            {
                var result = await httpClient.GetAsync($"/v1/company/{scradaSettings.CompanyId}/peppol/inbound/document/unconfirmed");

                if (!result.IsSuccessStatusCode)
                {
                    var resultContent = await result.Content.ReadAsStringAsync();
                    return new Response<T>
                    {
                        Error = $"Error while getting documents. Status: {result.StatusCode}. Content: {resultContent}"
                    };
                }

                return new Response<T>
                {
                    ResponseObject = await result.Content.ReadFromJsonAsync<T>()
                };
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    Error = $"Something wrong happened. Ex: {ex.Message}"
                };
            }
        }
    }
}
