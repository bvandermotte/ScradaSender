using System.Xml.Linq;
using ScradaSender.Agents.Models;

namespace ScradaSender.Agents.Interfaces
{
    public interface IScradaServiceAgent
    {
        Task<ScradaParticipant> CheckIfCompanyExist(string peppolIdentifierValue);
        Task<Guid> SendDocument(XDocument invoice);
        Task<OutboundDocumentStatus> CheckDocumentStatus(string outboundDocumentId);
    }
}