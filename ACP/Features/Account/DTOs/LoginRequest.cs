using System.ComponentModel.DataAnnotations;

namespace ACP.Features.Account
{
    public class LoginRequest
    {
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        public string ClientType { get; set; } = "ACP_Web_App";
    }
}
