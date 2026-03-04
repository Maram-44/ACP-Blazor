namespace ACP.Models.Customers
{
    public class CustomerProfile : Customer
    {
        // حقول إضافية من جدول الـ Users
        public string UserName { get; set; } = null!;
        public int UserId { get; set; }

        // حقول اختيارية لتغيير كلمة المرور
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
