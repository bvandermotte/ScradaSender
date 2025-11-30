namespace ScradaSender.Agents.Models
{
    public class OutboundDocumentStatus
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ExternalReference { get; set; }
        public string PeppolSenderID { get; set; }
        public string PeppolReceiverID { get; set; }
        public string PeppolC1CountryCode { get; set; }
        public DateTime? PeppolC2Timestamp { get; set; }
        public string? PeppolC2SeatID { get; set; }
        public string? PeppolC2MessageID { get; set; }
        public string? PeppolC3MessageID { get; set; }
        public DateTime? PeppolC3Timestamp { get; set; }
        public string? PeppolC3SeatID { get; set; }
        public string? PeppolConversationID { get; set; }
        public string? PeppolSbdhInstanceID { get; set; }
        public string PeppolDocumentTypeScheme { get; set; }
        public string PeppolDocumentTypeValue { get; set; }
        public string PeppolProcessScheme { get; set; }
        public string PeppolProcessValue { get; set; }
        public string? SalesInvoiceID { get; set; }
        public string Status { get; set; }
        public int Attempt { get; set; }
        public string ErrorMessage { get; set; }
    }
}
