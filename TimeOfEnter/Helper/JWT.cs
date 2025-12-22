namespace TimeOfEnter.Helper
{
    public class JWT
    {
        public string SecritKey { get; set; }
        public string AudienceIP { get; set; }
        public string IssuerIP { get; set; }
        public double Expiration {  get; set; }
    }
}
