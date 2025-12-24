namespace TimeOfEnter.Helper;

public class JWT
{
    public required string SecritKey { get; set; }
    public required string AudienceIP { get; set; }
    public required string IssuerIP { get; set; }
    public double Expiration { get; set; }
}
