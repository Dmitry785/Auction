using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : BaseModel<Guid>
    {
        public string? OriginalId { get; set; }
        public string Username { get; set; } = null!;
        public List<WalletCurrency> Currencies { get; set; } = new List<WalletCurrency>();
        public string Name { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime RegisterDate { get; set; }
        public User(string username, DateTime registerDate, string name, string passwordHash, List<WalletCurrency> currencies, string? originalId = null)
        {
            Id = Guid.NewGuid();
            OriginalId = originalId;
            RegisterDate = registerDate;
            Username = username;
            Name = name;
            PasswordHash = passwordHash;
        }
        public User()
        {

        }
    }
}
