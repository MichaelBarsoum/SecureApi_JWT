namespace Secure_Api_Using_JWT.Helpers
{
    public class JWT
    {
        public string KEY { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInDays { get; set; }
    }
}
