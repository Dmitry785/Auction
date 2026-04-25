using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RefreshToken : BaseModel<Guid>
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime ExpireAt { get; set; }
        public bool IsExpired { get; set; }
    }
}
