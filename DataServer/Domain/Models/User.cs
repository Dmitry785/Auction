using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : BaseModel<Guid>
    {
        public string Username { get; set; } = null!;
        public int PasswordHash { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = null!;
        public User(string username, int hashPassword, List<RefreshToken> refreshTokens)
        {
            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = hashPassword;
            RefreshTokens = refreshTokens;
        }
        public User()
        {

        }
    }
}
