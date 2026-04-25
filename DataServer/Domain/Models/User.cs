using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : BaseModel<Guid>
    {
        public string Username { get; set; }
        public int PasswordHash { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
