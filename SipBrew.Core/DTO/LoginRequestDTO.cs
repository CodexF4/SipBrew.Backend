using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.DTO
{
    public class LoginRequestDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }
}
