namespace SparkLink.Helper
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
        public string Audience { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }

        public bool ValidateIssuerSigninngkey { get; set; }
        public bool ValidateLifeTime { get; set; }
        

    }
}
