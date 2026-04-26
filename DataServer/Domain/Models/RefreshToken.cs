using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RefreshToken : BaseModel<Guid>
    {
        public User User { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpireAt { get; set; }
        public bool IsExpired { get; set; }
        public RefreshToken(User user, DateTime expireAt)
        {
            Id = Guid.NewGuid();
            User = user;
            ExpireAt = expireAt;
            IsExpired = false;
        }
        public void SetExpired()
        {
            IsExpired = true;
        }
    }
}
