namespace ACP.Features.Animals
{
    public class SubmitApplicationRequest
    {
        public int AnimalId { get; set; }
        public string? Reason { get; set; }
    }

    public class ConfirmCodeRequest
    {
        public int TransactionId { get; set; }
        public string Code { get; set; }
    }
}
