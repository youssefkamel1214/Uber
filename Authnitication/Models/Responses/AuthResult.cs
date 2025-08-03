namespace Authnitication.Models.Responses
{
    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool success { get; set; }
        public List <string> Error { get; set; }
    }
}
