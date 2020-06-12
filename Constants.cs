namespace OAuthServer
{
    public class Constants
    {
        public const string Audience = @"http://localhst:44382";
        public const string Issuer = Audience;
        public const string Secret = "not_too_short_secret_otherwise_it_might_error";
    }
}