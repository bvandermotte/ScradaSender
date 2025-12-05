using System.Text.Json.Serialization;

namespace ScradaSender.Agents.Models
{
    public class PeppolDocumentsResponse
    {
        public List<PeppolDocument> Results { get; set; }

        [JsonPropertyName("__count")]
        public int Count { get; set; }
    }

    public class PeppolDocument
    {
        public Guid Id { get; set; }
        public int InternalNumber { get; set; }
        public string PeppolSenderScheme { get; set; }
        public string PeppolSenderID { get; set; }
        public string PeppolReceiverScheme { get; set; }
        public string PeppolReceiverID { get; set; }
        public string PeppolC1CountryCode { get; set; }
        public DateTime PeppolC2Timestamp { get; set; }
        public string PeppolC2SeatID { get; set; }
        public string PeppolC2MessageID { get; set; }
        public Guid PeppolC3IncomingUniqueID { get; set; }
        public string PeppolC3MessageID { get; set; }
        public DateTime PeppolC3Timestamp { get; set; }
        public string PeppolConversationID { get; set; }
        public Guid PeppolSbdhInstanceID { get; set; }
        public string PeppolProcessScheme { get; set; }
        public string PeppolProcessValue { get; set; }
        public string PeppolDocumentTypeScheme { get; set; }
        public string PeppolDocumentTypeValue { get; set; }
    }
}
