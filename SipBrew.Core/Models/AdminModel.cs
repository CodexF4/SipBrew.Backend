using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.Models
{
    public class AdminModel
    {
        public int Id { get; set; }
        public string UserName { get ; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
