using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : BaseModel<string>
    {
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Balance {  get; set; }
        public User(string originalId, string username, string name, int balance)
        {
            Id = originalId;
            Username = username;
            Name = name;
            Balance = balance;
        }
        public User()
        {
        }
    }
}
