public class ScradaParticipant
{
    public ParticipantIdentifier ParticipantIdentifier { get; set; }
    public BusinessEntity BusinessEntity { get; set; }
    public List<DocumentType> DocumentTypes { get; set; }
}

public class ParticipantIdentifier
{
    public string Scheme { get; set; }
    public string Id { get; set; }
}

public class BusinessEntity
{
    public string Name { get; set; }
    public string LanguageCode { get; set; }
    public string CountryCode { get; set; }
}

public class DocumentType
{
    public string Scheme { get; set; }
    public string Value { get; set; }
    public ProcessIdentifier ProcessIdentifier { get; set; }
}

public class ProcessIdentifier
{
    public string Scheme { get; set; }
    public string Value { get; set; }
    public List<Endpoint> Endpoints { get; set; }
}

public class Endpoint
{
    public string ReferenceAddress { get; set; }
    public string TechnicalContactUrl { get; set; }
    public string TechnicalInformationUrl { get; set; }
}