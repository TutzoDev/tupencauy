namespace tupencauywebapi.DTOs
{
    public class DepositDTO
    {
        public string idUser { get; set; }
        public IFormFile paymentReceipt { get; set; }
        public double depositAmount { get; set; }
    }
}
