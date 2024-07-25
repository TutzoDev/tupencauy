namespace tupencauywebapi.Models
{
    public class AuthReturn
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public string? TenantId { get; set; }
        public string? IdUser { get; set;}
    }
}
