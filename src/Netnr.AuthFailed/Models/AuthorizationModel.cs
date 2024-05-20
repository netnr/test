namespace Netnr.AuthFailed.Models
{
    public class AuthorizationBaseModel
    {
        public long UserId { get; set; } = 0;
        public string UserAccount { get; set; }
        public DateTime IssuedUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresUtc { get; set; } = DateTime.UtcNow.AddMinutes(30);
    }
}
