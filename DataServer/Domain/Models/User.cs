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
        public string Name { get; set; } = null!;
        public string Password { get; set; }
        public List<Item> Items { get; set; }
        public User(string username, string name, string password, List<Item> items)
        {
            Id = Guid.NewGuid();
            Username = username;
            Name = name;
            Password = password;
            Items = items;
        }
        public User()
        {

        }
    }
}
