namespace tupencauywebapi.Models
{
    public class EditUserReq
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
