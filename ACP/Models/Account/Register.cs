using System.ComponentModel.DataAnnotations;
using ACP.Models.Customers;
using Microsoft.AspNetCore.Components.Forms;

namespace ACP.Models.Account
{
    public class Register
    {
        public Customer CustomerDetails { get; set; }
        public string Password { get; set; }

    }
}
