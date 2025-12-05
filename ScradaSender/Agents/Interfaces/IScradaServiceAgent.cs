using System.Xml.Linq;
using ScradaSender.Agents.Models;
using ScradaSender.Agents.Models.Base;

namespace ScradaSender.Agents.Interfaces
{
    public interface IScradaServiceAgent
    {
        Task<Response<T>> CheckIfCompanyExistAsync<T>(string peppolIdentifierValue);
        Task<Response<T>> SendDocumentAsync<T>(XDocument invoice, string supplierScheme, string supplierId, string customerScheme, string customerId, string extRef, string countryCode = "BE");
        Task<Response<T>> CheckDocumentStatusAsync<T>(string outboundDocumentId);
        Task<Response<T>> GetDocumentsAsync<T>();
    }
}